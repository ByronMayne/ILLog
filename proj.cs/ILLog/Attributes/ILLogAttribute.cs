using System;

namespace ILLog
{
  public enum ILLogTypes
  {
    /// <summary>
    /// This log will be called every time it's hit.
    /// </summary>
    Always,
    /// <summary>
    /// This log will be called once ever. 
    /// </summary>
    Once,
    /// <summary>
    /// This log will be called when the method has different 
    /// arguments from the last time it was called. 
    /// </summary>
    ArguementsChanged,
  }

  [AttributeUsage(AttributeTargets.Method)]
  public class ILLogAttribute : Attribute
  {
    private ILLogTypes m_LogType;

    /// <summary>
    /// Gets or sets the type of log that this attribute uses. 
    /// </summary>
    public ILLogTypes logType
    {
      get { return m_LogType; }
      set { m_LogType = value; }
    }

    /// <summary>
    /// The default constructor for our type. Default value is <see cref="ILLogTypes.Always"/>
    /// </summary>
    public ILLogAttribute()
    {
      m_LogType = ILLogTypes.Always;
    }


    /// <summary>
    /// Creates a new instance of ILogAttribute and sets it's log type. 
    /// </summary>
    /// <param name="logType"></param>
    public ILLogAttribute(ILLogTypes logType)
    {
      m_LogType = logType;
    }

    public void TestMethod()
    {

    }
  }
}
