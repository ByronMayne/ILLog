using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILLog
{
  /// <summary>
  /// A simple attribute used to flag assemblies as being updated by ILLog. Tis 
  /// stops us from doubling up on our code base. 
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly)]
  public class ILLogModifiedAttribute : Attribute
  {

  }
}
