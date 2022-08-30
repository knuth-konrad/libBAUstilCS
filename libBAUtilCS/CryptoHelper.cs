using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using static libBAUtilCS.FilesystemHelper;

namespace libBAUtilCS
{

   // ToDo: Implement whole class properly. VB.NET translation fails because of C# missing '\' operator
   // See https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
   // and https://docs.microsoft.com/en-us/dotnet/standard/security/walkthrough-creating-a-cryptographic-application

   /// <summary>
   /// En-/Decryption helper.
   /// </summary>
   /// <remarks>
   /// Source: https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
   /// </remarks>
   public sealed class CryptoHelper
   {
      #region Declarations
      // This constant is used to determine the key size of the encryption algorithm in bits.
      // We divide this by 8 within the code below to get the equivalent number of bytes.
      private const int Keysize = 256;

      // This constant determines the number of iterations for the password bytes generation function.
      private const int DerivationIterations = 1000;

      /// <summary>
      /// Optional password for en-/decryption passed via constructor 
      /// </summary>
      /// <remarks>Passwords passed as method parameters take precedence!</remarks>
      public string Passphrase = String.Empty;
      #endregion

      #region Methods - Public

      /// <summary>
      /// Encrypts a string.
      /// </summary>
      /// <param name="plainText">Text to be encrypted.</param>
      /// <param name="passPhrase">Passphrase for encryption.</param>
      /// <returns>Encrypted <paramref name="plainText"/></returns>
      /// <remarks>
      /// Source: https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
      /// </remarks>
      public string EncryptString(string plainText, string passPhrase = "")
      {
         
         // Passphrase set on object initialization?
         if (passPhrase == String.Empty && Passphrase != String.Empty)
         {
            passPhrase = Passphrase;
         }
         
         // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
         // so that the same Salt and IV values can be used when decrypting.  
         var saltStringBytes = Generate256BitsOfRandomEntropy();
         var ivStringBytes = Generate256BitsOfRandomEntropy();
         var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
         using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
         {
            var keyBytes = password.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
               symmetricKey.BlockSize = 256;
               symmetricKey.Mode = CipherMode.CBC;
               symmetricKey.Padding = PaddingMode.PKCS7;
               using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
               {
                  using (var memoryStream = new MemoryStream())
                  {
                     using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                     {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                        var cipherTextBytes = saltStringBytes;
                        cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                        cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                        memoryStream.Close();
                        cryptoStream.Close();
                        return Convert.ToBase64String(cipherTextBytes);
                     }
                  }
               }
            }
         }
      }

      /// <summary>
      /// Decrypts a string.
      /// </summary>
      /// <param name="cipherText">Text to be decrypted.</param>
      /// <param name="passPhrase">Passphrase for decryption. Must obviously be the same passphrase with which the text was encrypted.</param>
      /// <returns>Decrypted <paramref name="cipherText"/></returns>
      /// <remarks>
      /// Source: https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
      /// </remarks>
      public string DecryptString(string cipherText, string passPhrase = "")
      {

         // Passphrase set on object initialization?
         if (passPhrase == String.Empty && Passphrase != String.Empty)
         {
            passPhrase = Passphrase;
         }

         // Get the complete stream of bytes that represent:
         // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
         var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
         // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
         var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
         // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
         var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
         // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
         var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

         using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
         {
            var keyBytes = password.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
               symmetricKey.BlockSize = 256;
               symmetricKey.Mode = CipherMode.CBC;
               symmetricKey.Padding = PaddingMode.PKCS7;
               using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
               {
                  using (var memoryStream = new MemoryStream(cipherTextBytes))
                  {
                     using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                     {
                        var plainTextBytes = new byte[cipherTextBytes.Length];
                        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        memoryStream.Close();
                        cryptoStream.Close();
                        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                     }
                  }
               }
            }
         }
      }

      /// <summary>
      /// Encrypts the contents of a file.
      /// </summary>
      /// <param name="inFile">File with contents to be encrypted.</param>
      /// <param name="outFile">Encrypted content will be written in this file.</param>
      /// <param name="passPhrase">Passphrase for encryption.</param>
      /// <param name="overwriteExisting">Overwrite <paramref name="outFile"/> if it already exists?</param>
      /// <returns><see langword="true"/> if the operation succeeds, <see langword="false"/> otherwise.</returns>
      public bool EncryptFile(string inFile, string outFile, string passPhrase = "", bool overwriteExisting = false)
      {

         // Safe guards
         if (!FileExists(inFile)) { throw new ArgumentException("File " + inFile + " does not exist."); }
         if (FileExists(outFile) && !overwriteExisting) { throw new ArgumentException("File " + outFile + " already exist."); }

         // Passphrased set on obect initialisation?
         if (passPhrase == String.Empty && Passphrase != String.Empty)
         {
            passPhrase = Passphrase;
         }

         string filePlainContent = File.ReadAllText(inFile, Encoding.UTF8);
         string fileEncryptedContent = EncryptString(filePlainContent, passPhrase);

         File.WriteAllText(outFile, fileEncryptedContent, Encoding.UTF8);

         return true;
      }

      /// <summary>
      /// Decrypts the contents of a file.
      /// </summary>
      /// <param name="inFile">File with encrypted contents.</param>
      /// <param name="outFile">Decrypted content will be written in this file.</param>
      /// <param name="passPhrase">Passphrase for decryption. Must obviously be the same passphrase with which the contents was encrypted.</param>
      /// <param name="overwriteExisting">Overwrite <paramref name="outFile"/> if it already exists?</param>
      /// <returns><see langword="true"/> if the operation succeeds, <see langword="false"/> otherwise.</returns>
      public bool DecryptFile(string inFile, string outFile, string passPhrase = "", bool overwriteExisting = false)
      {

         // Safe guards
         if (!FileExists(inFile)) { throw new ArgumentException("File " + inFile + " does not exist."); }
         if (FileExists(outFile) && !overwriteExisting) { throw new ArgumentException("File " + outFile + " already exist."); }

         // Passphrased set on obect initialisation?
         if (passPhrase == String.Empty && Passphrase != String.Empty)
         {
            passPhrase = Passphrase;
         }

         string fileEncryptedContent = File.ReadAllText(inFile, Encoding.UTF8);
         string fileDecryptedContent = DecryptString(fileEncryptedContent, passPhrase);

         File.WriteAllText(outFile, fileDecryptedContent, Encoding.UTF8);

         return true;
      }
      #endregion

      private static byte[] Generate256BitsOfRandomEntropy()
      {
         var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
         using (var rngCsp = new RNGCryptoServiceProvider())
         {
            // Fill the array with cryptographically secure random bytes.
            rngCsp.GetBytes(randomBytes);
         }
         return randomBytes;
      }  // Generate256BitsOfRandomEntropy

      #region "Constructor / Finalize"
      /// <summary>
      /// Initializes a new instance object.
      /// </summary>
      /// <param name="passPhrase">Passphrase used for en-/decryption.</param>
      public CryptoHelper(string passPhrase = "") {
         Passphrase = passPhrase;
         }
   #endregion
}  // CryptoHelper
}
