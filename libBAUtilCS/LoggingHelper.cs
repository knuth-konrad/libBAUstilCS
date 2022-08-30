using System;
using System.Linq;
using System.Reflection;

using NLog;

namespace libBAUtilCS
{
  /// <summary>
  /// (NLog) logging helper methods
  /// </summary>
  public class LoggingHelper
  {

    /// <summary>
    /// Return a list of parameters info passed to a method.
    /// </summary>
    /// <param name="mb">The current method.</param>
    /// <param name="param">Parameters passed to the above method.</param>
    /// <returns>Parameters</returns>
    /// <remarks>
    /// When passing the actual parameters in <paramref name="param"/>, ideally these 
    /// should be passed in the order they appear in the method prototype.
    /// </remarks>
    /// <example>
    /// Console.WriteLine(GetMethodParameters(System.Reflection.MethodBase.GetCurrentMethod()));
    /// </example>
    public static string GetMethodParameters(MethodBase mb, params object[] param)
    {
      ParameterInfo[] pis = mb.GetParameters();
      string result = "Parameters:\n";

      try
      {
        foreach (ParameterInfo pi in pis)
        {
          if (pi.IsOut)
          {
            result += String.Format(" - {0} (out)\n", pi.Name);
          }
          else
          {
            result += String.Format(" - {0}\n", pi.Name);
          }
          result += String.Format("  Type    : {0}\n", pi.ParameterType);
          result += String.Format("  Position: {0}\n", pi.Position.ToString());
          if (pi.HasDefaultValue)
          {
            result += String.Format("  Default : {0}\n", pi.DefaultValue.ToString());
          }
        }
        if (param != null && param.GetLength(0) > 0)
        {
          result += "Values:\n";
          foreach (object o in param)
          {
            result += String.Format("  {0}\n", o.ToString());
          }
        }
      }
      catch { }

      return result;
    } // GetMethodParameters()

    /// <summary>
    /// Retrieve the current (text) log file's name and path
    /// </summary>
    /// <returns>NLog's log file incl. full path</returns>
    /// <remarks>
    /// Source: https://stackoverflow.com/questions/7332393/how-can-i-query-the-path-to-an-nlog-log-file
    /// </remarks>
    public static String GetNLogCurrentLogFile()
    {
      NLog.Targets.FileTarget fileTarget = LogManager.Configuration.AllTargets.FirstOrDefault(t => t is NLog.Targets.FileTarget) as NLog.Targets.FileTarget;
      return fileTarget == null ? String.Empty : fileTarget.FileName.Render(new LogEventInfo { Level = LogLevel.Info });
    } // GetNLogCurrentLogFile()

    /// <summary>
    /// Sets the current (text) log file's name and path
    /// </summary>
    /// <param name="newLogfile">NLog's new log file incl. full path</param>
    /// <param name="targetFilename">
    /// Set the name for this file target. If <see cref="String.Empty"/>, set the first <see cref="NLog.Targets.FileTarget"/>'s file name.
    /// </param>
    /// <returns>NLog's previous log file incl. full path</returns>
    /// <remarks>
    /// Source: https://capnjosh.com/blog/programatically-change-a-logging-targets-path-and-file-name-for-nlog/
    /// </remarks>
    public static String SetNLogCurrentLogFile(String newLogfile, String targetFilename = "")
    {

      NLog.Targets.FileTarget target;
      String oldLog = libECKDUtilCS.LoggingHelper.GetNLogCurrentLogFile();

      if (targetFilename.Length < 1)
      {
        target = LogManager.Configuration.AllTargets.FirstOrDefault(t => t is NLog.Targets.FileTarget) as NLog.Targets.FileTarget;
      }
      else
      {
        target = (NLog.Targets.FileTarget)NLog.LogManager.Configuration.FindTargetByName(targetFilename);
      }

      if (target != null)
      {
        target.FileName = newLogfile;
        NLog.LogManager.ReconfigExistingLoggers();
      }

      return oldLog;

    } // SetNLogCurrentLogFile()

  } // class LoggingHelper
}
