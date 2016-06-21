using UnityEditor;
using UnityEngine;
using Mono.Cecil.Cil;
using Mono.Cecil;
using System.Linq;

namespace ILLog
{
  public class MenuItems
  {
    [InitializeOnLoadMethod]
    public static void GetTypes()
    {
      var assemblyCShap = AssemblyDefinition.ReadAssembly(Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp.dll");
      var engineAssembly = AssemblyDefinition.ReadAssembly(Application.dataPath + "/../Library/UnityAssemblies/UnityEngine.dll");

      var debugType = AssemblyUtility.GetTypeFromAssembly<Debug>(AssemblyTypes.CSharpEngine);
      var logMethod = debugType.GetMethod("Log");

      var attributType = AssemblyUtility.GetTypeFromAssembly<Debug>(AssemblyTypes.ILLog);

      var improtedLogMethod = assemblyCShap.MainModule.Import(logMethod);
      assemblyCShap.MainModule.Import(attributType);

      foreach (var module in assemblyCShap.Modules)
      {
        foreach (var type in module.Types)
        {
          if (type.HasMethods)
          {
            foreach (var method in type.Methods)
            {
              ILProcessor ilProcessor = method.Body.GetILProcessor();
              var attributes = method.CustomAttributes;

              for (int i = 0; i < attributes.Count; i++)
              {
                CustomAttribute customAttribute = attributes[i];

                if (customAttribute.AttributeType.FullName == typeof(ILLogAttribute).FullName)
                {
                  var args = customAttribute.ConstructorArguments;

                  if (args.Count > 0)
                  {

                    for (int x = 0; x < args.Count; x++)
                    {

                      ILLogTypes logType = (ILLogTypes)args[x].Value;

                      Debug.Log("Log Type: " + logType.ToString());

                      if (logType == ILLogTypes.Once)
                      {
                        // Create field 
                        string fieldName = "ILLog_" + method.Name + "__Invoked";
                        FieldDefinition logField = new FieldDefinition(fieldName, FieldAttributes.Private, module.Import(typeof(bool)));
                        type.Fields.Add(logField);

                        // IL is written bottom up so this logic seems backwards.

                        // Creating the closing brace. 
                        var closeBrace = ilProcessor.Create(OpCodes.Ret);


                        // Insert '{'
                        ilProcessor.InsertBefore(method.Body.Instructions[0], closeBrace);

                        // Set the logField boolean to the active argument slow defined below.
                        ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Stfld, logField));
                        // Set the active argument slot to ldc_Ir_1 (true)
                        ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Ldc_I4_1));
                        // Load the first argument field. 
                        ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Ldarg_0));

                        // Setup the next method that you want to invoke (Debug.Log)
                        ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Call, improtedLogMethod));

                        // Set the argument for the function defined above. 
                        ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Ldstr, method.FullName)); // <- Injects a string

                        // Start of the if statement saying it's scope ends when it get's to the close brace (first thing defined) 
                        ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Brtrue, closeBrace));

                        // Set the if statement to only be true if logField is true. Loading it into slot zero. 
                        ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Ldfld, logField));

                        // Start of the if statement (loading a variable into slot 0)
                        ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Ldarg_0));

                      }
                    }

                  }
                  else
                  {
                    ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Call, improtedLogMethod));
                    //ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Ldarg_0)); // <- Injects 'this'
                    ilProcessor.InsertBefore(method.Body.Instructions[0], ilProcessor.Create(OpCodes.Ldstr, method.FullName)); // <- Injects a string

                  }
                }
              }
            }
          }
        }
        module.Write(Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp.dll");
      }
    }

  }
}
