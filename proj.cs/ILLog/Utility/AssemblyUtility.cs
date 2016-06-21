using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace ILLog
{
  public enum AssemblyTypes
  {
    UnityEngine,
    UnityEditor,
    CSharpEngine,
    CSharpEditor,
    ILLog
  }


  public static class AssemblyUtility
  {

    private static AssemblyDefinition m_UnityEditorAssemblyDefinition;
    private static AssemblyDefinition m_UnityEngineAssemblyDefinition;
    private static AssemblyDefinition m_AssemblyCSharpAssemblyDefinition;
    private static AssemblyDefinition m_AssemblyCSharpEditorAssemblyDefinition;
    private static AssemblyDefinition m_ILLogAssemblyDefinition;

    /// <summary>
    /// Loads the Unity Editor Assembly from the library/UnityAssemblies folder or returns a cached version.
    /// </summary>
    public static AssemblyDefinition GetUnityEditorAssemblyDefinition()
    {
      if (m_UnityEditorAssemblyDefinition == null)
      {
        m_UnityEditorAssemblyDefinition = AssemblyDefinition.ReadAssembly(Application.dataPath + "/../Library/UnityAssemblies/UnityEditor.dll");
      }
      return m_UnityEditorAssemblyDefinition;
    }


    /// <summary>
    /// Loads the Unity Editor Assembly from the library/UnityAssemblies folder or returns a cached version.
    /// </summary>
    public static AssemblyDefinition GetUnityEngineAssemblyDefinition()
    {
      if (m_UnityEngineAssemblyDefinition == null)
      {
        m_UnityEngineAssemblyDefinition = AssemblyDefinition.ReadAssembly(Application.dataPath + "/../Library/UnityAssemblies/UnityEngine.dll");
      }
      return m_UnityEngineAssemblyDefinition;
    }


    /// <summary>
    /// Loads the Engine Assembly from the library/UnityAssemblies folder or returns a cached version. 13.5 x 10
    /// </summary>
    public static AssemblyDefinition GetCSharpEngineAssemblyDefinition()
    {
      if (m_AssemblyCSharpAssemblyDefinition == null)
      {
        m_AssemblyCSharpAssemblyDefinition = AssemblyDefinition.ReadAssembly(Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp.dll");
      }
      return m_AssemblyCSharpAssemblyDefinition;
    }

    /// <summary>
    /// Loads the Editor Assembly from the library/UnityAssemblies folder or returns a cached version.
    /// </summary>
    public static AssemblyDefinition GetCSharpEditorAssemblyDefinition()
    {
      if (m_AssemblyCSharpEditorAssemblyDefinition == null)
      {
        m_AssemblyCSharpEditorAssemblyDefinition = AssemblyDefinition.ReadAssembly(Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp-Editor.dll");
      }
      return m_AssemblyCSharpEditorAssemblyDefinition;
    }

    /// <summary>
    /// Gets the assembly that ILLog is contained within. 
    /// </summary>
    public static AssemblyDefinition GetILLogAssemblyDefinition()
    {
      Debug.Log(System.Reflection.Assembly.GetExecutingAssembly().Location);

      if (m_ILLogAssemblyDefinition == null)
      {
        m_ILLogAssemblyDefinition = AssemblyDefinition.ReadAssembly(System.Reflection.Assembly.GetExecutingAssembly().Location);
      }
      return m_ILLogAssemblyDefinition;

    }

    /// <summary>
    /// Gets the assembly from disk based on the enum sent in. 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static AssemblyDefinition GetAssembly(AssemblyTypes type)
    {
      switch (type)
      {
        case AssemblyTypes.UnityEngine:
          return GetUnityEngineAssemblyDefinition();
        case AssemblyTypes.UnityEditor:
          return GetUnityEditorAssemblyDefinition();
        case AssemblyTypes.CSharpEngine:
          return GetCSharpEngineAssemblyDefinition();
        case AssemblyTypes.CSharpEditor:
          return GetCSharpEditorAssemblyDefinition();
        case AssemblyTypes.ILLog:
          return GetILLogAssemblyDefinition();
        default:
          return null;
      }
    }


    /// <summary>
    /// Loops over the picked assembly and looks for a type that matches the generic. 
    /// If none is found this will return null.
    /// </summary>
    public static TypeDefinition GetTypeFromAssembly<T>(AssemblyTypes assemblyType)
    {
      AssemblyDefinition assemblyDef = GetAssembly(assemblyType);

      if (assemblyDef != null)
      {
        foreach (var module in assemblyDef.Modules)
        {
          foreach (var type in module.Types)
          {
            if (type.FullName == typeof(T).FullName)
            {
              return type;
            }
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Loops over the picked assembly and looks for a type with the same full name as the one sent in. 
    /// If none is found this will return null.
    /// </summary>
    public static TypeDefinition GetTypeFromAssembly(string fullname, AssemblyTypes assemblyType)
    {
      AssemblyDefinition assemblyDef = GetAssembly(assemblyType);

      if (assemblyDef != null)
      {
        foreach (var module in assemblyDef.Modules)
        {
          foreach (var type in module.Types)
          {
            if (type.FullName == fullname)
            {
              return type;
            }
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Takes an assembly type and tries to add a <see cref="ILLogModifiedAttribute"/> to it. This
    /// lets us know that it has been changed. 
    /// </summary>
    /// <param name="type">The assembly type you want to modify.</param>
    public static void MarkAssemblyAsModified(AssemblyTypes type)
    {
      // Grab our assembly.
      AssemblyDefinition definition = GetAssembly(type);

      // Check it's attributes
      for (int i = 0; i < definition.CustomAttributes.Count; i++)
      {
        // Grab our attribute
        CustomAttribute attribute = definition.CustomAttributes[i];

        // If our full names are the same we are the same type. 
        if (attribute.AttributeType.FullName == typeof(ILLogModifiedAttribute).FullName)
        {
          throw new System.Exception(string.Format("Assembly {0} already marked as modified", definition.FullName));
        }

        // No exception was thrown so we add a new one.


      }

    }
  }
}
