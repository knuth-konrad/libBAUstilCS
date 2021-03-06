using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;


/// <summary>
///  General purpose string handling/formatting helpers
///  <remarks>
///  VB.NET / C# equivalents: https://sites.harding.edu/fmccown/vbnet_csharp_comparison.html
/// </remarks>
/// </summary>
public class StringUtil
{
   #region "Declares"
   /// Bytes to <unit> - Function Bytes2FormattedString()

   /// <summary>Data storage units</summary>
   /// <seealso cref="Bytes2FormattedString"/>
   public enum ESizeUnits : ulong
   {
      B = 1024,
      KB = B * B,
      MB = KB * B,
      GB = MB * B,
      TB = GB * B

   }
   #endregion

   /// <summary>
   /// Creates a formatted string representing the size in its proper 'spelled out' unit
   /// (Bytes, KB etc.)
   /// </summary>
   /// <param name="uintBytes">Number of bytes to transform</param>
   /// <returns>
   /// Spelled out size, e.g. 1030 -> '1KB'
   /// </returns>
   /// <remarks>
   /// Author: dbasnett<br />
   /// Source: http://www.vbforums.com/showthread.php?634675-RESOLVED-Bytes-to-MB-etc
   /// </remarks>
   public static string Bytes2FormattedString(ulong uintBytes)
   {
      string sUnits = string.Empty, szAsStr = string.Empty;
      float dblInUnits;

      if (uintBytes < (ulong)ESizeUnits.B)
      {
         szAsStr = uintBytes.ToString("n0");
         sUnits = "Bytes";
      }
      else if (uintBytes <= (ulong)ESizeUnits.KB)
      {
         dblInUnits = uintBytes / (ulong)ESizeUnits.B;
         szAsStr = dblInUnits.ToString("n1");
         sUnits = "KB";
      }
      else if (uintBytes <= (ulong)ESizeUnits.MB)
      {
         dblInUnits = uintBytes / (ulong)ESizeUnits.KB;
         szAsStr = dblInUnits.ToString("n1");
         sUnits = "MB";
      }
      else if (uintBytes <= (ulong)ESizeUnits.GB)
      {
         dblInUnits = uintBytes / (ulong)ESizeUnits.MB;
         szAsStr = dblInUnits.ToString("n1");
         sUnits = "GB";
      }
      else
      {
         dblInUnits = uintBytes / (ulong)ESizeUnits.GB;
         szAsStr = dblInUnits.ToString("n1");
         sUnits = "TB";
      }

      return System.String.Format("{0} {1}", szAsStr, sUnits);
   }

   /// <summary>
   /// Creates a formatted string representing the size in its proper 'spelled out' unit
   /// (Bytes, KB etc.)
   /// </summary>
   /// <param name="uintBytes">Number of bytes to transform</param>
   /// <returns>
   /// Spelled out size, e.g. 1030 -> '1KB'
   /// </returns>
   /// <remarks>
   /// Author: dbasnett<br />
   /// Source: http://www.vbforums.com/showthread.php?634675-RESOLVED-Bytes-to-MB-etc
   /// </remarks>
   public static string Bytes2FormattedString(ulong uintBytes, bool largestUnitOnly = true)
   {

      ulong uintDivisor;
      string sUnits = string.Empty, szAsStr = string.Empty;

      if (largestUnitOnly)
      {
         return Bytes2FormattedString(uintBytes);
      }

      while (uintBytes > 0)
      {
         if (uintBytes / (ulong)(1024 ^ 4) > 0)
         {
            // TB
            uintDivisor = uintBytes / (ulong)(1024 ^ 4);
            uintBytes = uintBytes - (uintDivisor * (ulong)(1024 ^ 4));
            sUnits = "GB ";
         }
         else if (uintBytes / (ulong)(1024 ^ 3) > 0)
         {
            // GB
            uintDivisor = uintBytes / (ulong)(1024 ^ 3);
            uintBytes = uintBytes - (uintDivisor * (ulong)(1024 ^ 3));
            sUnits = "GB ";
         }
         else if (uintBytes / (ulong)(1024 ^ 2) > 0)
         {
            // MB
            uintDivisor = uintBytes / (ulong)(1024 ^ 2);
            uintBytes = uintBytes - (uintDivisor * (ulong)(1024 ^ 2));
            sUnits = "MB ";
         }
         else if (uintBytes / (ulong)(1024 ^ 1) > 0)
         {
            // KB
            uintDivisor = uintBytes / (ulong)(1024);
            uintBytes = uintBytes - (uintDivisor * (ulong)(1024 ^ 1));
            sUnits = "KB ";
         }
         else
         {
            // B
            uintDivisor = uintBytes / (ulong)(1024 ^ 0);
            uintBytes = uintBytes - (uintDivisor * (ulong)(1024 ^ 0));
            sUnits = "B ";
         }

         szAsStr = szAsStr + uintDivisor.ToString("n0") + sUnits;

      }  // while

      return szAsStr.TrimEnd();
   }

   /// <summary>
   /// Replacement for VB6's Chr() function.
   /// </summary>
   /// <param name="ansiValue">ANSI value for which to return a string</param>
   /// <returns>
   /// ANSI String-Representation of <paramref name="ansiValue"/>
   /// </returns>
   /// <remarks>
   /// Source: https://stackoverflow.com/questions/36976240/c-sharp-char-from-int-used-as-string-the-real-equivalent-of-vb-chr?lq=1
   /// </remarks>
   public static string Chr(Int32 ansiValue)
   {
      return Char.ConvertFromUtf32(ansiValue);
   }

   /// <summary>
   /// Replacement for VB6's Chr() function.
   /// </summary>
   /// <param name="ansiValue">ANSI value for which to return a string</param>
   /// <returns>
   /// ANSI String-Representation of <paramref name="ansiValue"/>
   /// </returns>
   public static string Chr(UInt32 ansiValue)
   {
      // Return Char.ConvertFromUtf32(CType(ansiValue, Int32))
      return Convert.ToChar(ansiValue).ToString();
   }

   /// <summary>
   /// Capitalize the first letter of a string.
   /// </summary>
   /// <param name="sText">Source string</param>
   /// <param name="sCulture">Specific culture string e.g. "en-US"</param>
   /// <returns>
   /// <paramref name="sText"/> with the first letter capitalized.
   /// </returns>
   /// <remarks>
   /// Source: https://social.msdn.microsoft.com/Forums/vstudio/en-US/c0872f6d-2975-43e6-872a-d2ba7901ed0e/convert-first-letter-of-string-to-capital?forum=csharpgeneral
   /// </remarks>
   public static string MCase(string sText, string sCulture = "")
   {

      TextInfo ti;

      try
      {
         if (sCulture.Length > 0)
         {
            ti = new CultureInfo(sCulture, false).TextInfo;
            return ti.ToTitleCase(sText);
         }
         else
         {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(sText);
         }
      }
      catch
      {
         return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(sText);
      }

   }

   /// <summary>
   /// Implements VB6's Left$() functionality.
   /// </summary>
   /// <param name="source">Source string</param>
   /// <param name="leftChars">Number of characters to return</param>
   /// <returns>
   /// For leftChars ...<br />
   ///    &gt; source.Length: source<br />
   ///    = 0: Empty string<br />
   ///    &lt; 0: Position from the end of source, e.g. Left("1234567890", -2) -> "12345678"
   /// </returns>
   /// <remarks>
   /// Source: Developed from https://stackoverflow.com/questions/844059/net-equivalent-of-the-old-vb-leftstring-length-function/12481156
   /// </remarks>
   public static string Left(string source, int leftChars)
   {

      if (System.String.IsNullOrEmpty(source) || leftChars == 0)
      {
         return System.String.Empty;
      }
      else if (leftChars > source.Length)
      {
         return source;
      }
      else if (leftChars < 0)
      {
         return source.Substring(0, source.Length + leftChars);
      }
      else
      {
         return source.Substring(0, Math.Min(leftChars, (int)(source.Length)));
      }

   }

   /// <summary>
   /// Implements VB's/PB's Right$() functionality.
   /// </summary>
   /// <param name="source">Source string</param>
   /// <param name="rightChars">Number of characters to return</param>
   /// <returns>
   /// For rightChars ...<br />
   ///    &gt; source.Length: source<br />
   ///    = 0: Empty string<br />
   ///    &lt; 0: Position from the start of source, e.g. Right("1234567890", -2) -&gt; "34567890"
   /// </returns>
   /// <remarks>
   /// Source: Developed from https://stackoverflow.com/questions/844059/net-equivalent-of-the-old-vb-leftstring-length-function/12481156
   /// </remarks>
   public static string Right(string source, int rightChars)
   {
      if (System.String.IsNullOrEmpty(source) || rightChars == 0)
      {
         return string.Empty;
      }
      else if (rightChars > source.Length)
      {
         return source;
      }
      else if (rightChars < 0)
      {
         return source.Substring(Math.Abs(rightChars));
      }
      else
      {
         return source.Substring(source.Length - rightChars, rightChars);
      }
   }

   /// <summary>
   /// Implements VB6's/PB's Mid$() functionality, as .NET's String.SubString()
   /// differs in its behavior that it raises an exception if startIndex > source.Length, 
   /// whereas Mid$() returns an empty string in such a case.
   /// </summary>
   /// <param name="source">Source string</param>
   /// <param name="startIndex">(0-based) start</param>
   /// <param name="length">Number of chars to return</param>
   /// <returns>
   /// For <paramref name="startIndex"/> &gt; <paramref name="source"/>.Length: <see cref="String.Empty"/>
   /// For <paramref name="length"/> &gt; <paramref name="startIndex"/> + <paramref name="source"/>.Length: all of <paramref name="source"/> from <paramref name="startIndex"/>
   /// </returns>
   /// <remarks>
   /// Source: Developed from https://stackoverflow.com/questions/844059/net-equivalent-of-the-old-vb-leftstring-length-function/12481156
   /// </remarks>
   public static string Mid(string source, int startIndex, int length = 0)
   {
      // Safe guards
      if (System.String.IsNullOrEmpty(source) || (startIndex > source.Length))
      {
         return System.String.Empty;
      }
      if (startIndex < 0)
      {
         throw new ArgumentOutOfRangeException("startIndex", "Must be 0 or greater.");
      }
      if (length < 0)
      {
         throw new ArgumentOutOfRangeException("length", "Must be 0 or greater.");
      }

      // Adjust length, if needed
      try
      {
         if (startIndex + length > source.Length || length == 0)
         {
            return source.Substring(startIndex - 1);
         }
         else
         {
            return source.Substring(startIndex - 1, length);
         }
      }
      catch
      {
         return System.String.Empty;
      }
   }

   /// <summary>
   /// Encloses <paramref name="text"/> with double quotation marks (").
   /// </summary>
   /// <param name="text">Wrap this string in quotation marks.</param>
   /// <returns><paramref name="text"/> enclosed in double quotation marks (")</returns>
   public static string EnQuote(string text)
   {
      return '"' + text + '"';
      /// Convert.ToChar(34).ToString + text + Convert.ToChar(34).ToString;
   }

   #region "Method String()"
   /// <summary>
   /// Mimics VB6's String() Function
   /// </summary>
   /// <param name="character">Character to use</param>
   /// <param name="count">Number of characters</param>
   /// <returns>String of <paramref name="count"/> x <paramref name="character"/></returns>
   
   public static string String (char character, Int32 count)
      {
         return new System.String(character, (int)count);
      }
   
 #endregion
} // class StringUtil
