using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

namespace libBAUtilCS
{
  /// <summary>
  /// Separate code and (this) info data from ConsoleHelper,
  /// allowing for easier editing this info.
  /// </summary>
  public class ConHelperData
  {

    //  Copyright notice values

    /// <summary>
    /// Default application developer/author name.
    /// </summary>
    public const string COPY_AUTHOR = "Knuth Konrad";
    /// <summary>
    /// Default company name.
    /// </summary>
    public const string COPY_COMPANYNAME = "BasicAware";

    // Console defaults
    
    /// <summary>
    /// Default line separation characters.
    /// </summary>
    public const string CON_SEPARATOR = "---";

    private string msAuthor = COPY_AUTHOR;
    private string msCompany = COPY_COMPANYNAME;
    private string msLineSeparator = CON_SEPARATOR;
    private Int32 mlStartYear = -1;

    #region Properties - Public

    /// <summary>
    /// Application developer name
    /// </summary>
    public string Author
    {
      get { return msAuthor; }
      set { msAuthor = value; }
    }

    /// <summary>
    /// Company / Copyright holder name
    /// </summary>
    public string Company
    {
      get { return msCompany; }
      set { msCompany = value; }
    }

    /// <summary>
    /// Line separator
    /// </summary>
    public string LineSeparator
    {
      get { return msLineSeparator; }
      set { msLineSeparator = value; }
    }

    /// <summary>
    /// The copyright's starting year
    /// </summary>
    public Int32 StartYear
    {
      get { return mlStartYear; }
      set { mlStartYear = value; }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="authorName">Application developer</param>
    /// <param name="companyName">Copyright holder</param>
    /// <param name="lineSeparator">Line separator</param>
    /// <param name="startYear">Copyright notice start year</param>
    public ConHelperData(string authorName = COPY_AUTHOR, string companyName = COPY_COMPANYNAME,
      string lineSeparator = CON_SEPARATOR, Int32 startYear = -1)
    {
      Author = authorName;
      Company = companyName;
      LineSeparator = lineSeparator;
      StartYear = startYear;
    }

    #endregion

  }
}
