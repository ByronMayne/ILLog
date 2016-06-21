using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ILLog
{
  public static class TypeDefinitionExtensions
  {
    public static MethodDefinition GetMethod(this TypeDefinition instance, string name)
    {
      for(int i = 0; i < instance.Methods.Count; i++)
      {
        MethodDefinition methodDef = instance.Methods[i];

        if( string.Compare( methodDef.Name, name ) == 0 )
        {
          return methodDef;
        }
      }
      return null;
    }
  }
}
