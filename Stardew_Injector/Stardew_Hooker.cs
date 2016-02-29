using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stardew_Injector
{
    public class Stardew_Hooker
    {

        private AssemblyDefinition m_vAsmDefinition = null;
        private ModuleDefinition m_vModDefinition = null;
        private Assembly m_vAssembly = null;
        private Type m_vMainType = null;

        //Helper methods: thanks to http://www.unknowncheats.me/forum/c/74398-c-loader-hooking-mono-cecil.html
        public MethodDefinition GetMethodDefinition(TypeDefinition t, String methodName)
        {
            return (from MethodDefinition m in t.Methods
                    where m.Name == methodName
                    select m).FirstOrDefault();
        }

   
        public FieldDefinition GetFieldDefinition(TypeDefinition t, String fieldName)
        {
            return (from FieldDefinition f in t.Fields
                    where f.Name == fieldName
                    select f).FirstOrDefault();
        }


        public PropertyDefinition GetPropertyDefinition(TypeDefinition t, String propName)
        {
            return (from PropertyDefinition p in t.Properties
                    where p.Name == propName
                    select p).FirstOrDefault();
        }

  
        public TypeDefinition GetTypeDefinition(String typeName)
        {
            return (from TypeDefinition t in this.m_vModDefinition.Types
                    where t.Name == typeName
                    select t).FirstOrDefault();
        }


        public Type GetType(String typeName)
        {
            return (from Type t in this.m_vAssembly.GetTypes()
                    where t.Name == typeName
                    select t).FirstOrDefault();
        }

       
        public MethodInfo GetMethod(String methodName)
        {
            return (from MethodInfo m in this.m_vMainType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    where m.Name == methodName
                    select m).FirstOrDefault();
        }

      
        public T GetMainField<T>(String fieldName)
        {
            var field = (from FieldInfo f in this.m_vMainType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                         where f.Name == fieldName
                         select f).FirstOrDefault();

            if (field == null)
                return default(T);

            return (T)field.GetValue(this.m_vMainType);
        }

        public void SetMainField<T>(String fieldName, T objValue)
        {
            var field = (from FieldInfo f in this.m_vMainType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                         where f.Name == fieldName
                         select f).FirstOrDefault();

            if (field == null)
                return;

            field.SetValue(this.m_vMainType, objValue);
        }

        public bool Initialize()
        {
            Console.WriteLine("Initiating StarDew_Injector....");
            try
            {
                this.m_vAsmDefinition = AssemblyDefinition.ReadAssembly(@"Stardew Valley.exe");
                this.m_vModDefinition = this.m_vAsmDefinition.MainModule;
                return true;
            }
            catch { return false; }
        }

        public bool Finialize()
        {
            try
            {
                if (this.m_vAsmDefinition == null)
                    return false;

                using (MemoryStream mStream = new MemoryStream())
                {
                    // Write the edited data to the memory stream..
                    this.m_vAsmDefinition.Write(mStream);

                    // Load the new assembly from the memory stream buffer..
                    this.m_vAssembly = Assembly.Load(mStream.GetBuffer());

                    return true;
                }
            }
            catch { return false; }
        }

        public bool Run()
        {
         
            {
                if (this.m_vAssembly == null)
                    return false;

                Console.WriteLine("Starting Stardew Valley...");

                m_vAssembly.EntryPoint.Invoke(null, new object[]{new string[0]});

                return true;
            }
          
        }

        /*
        Unused Hook methods

        public static void PreInitialize()
        {
           
        }
        public static void PostInitialize()
        {
           
        }

        public static void PreUpdate(GameTime gameTime)
        {

        }
        public static void PostUpdate(GameTime gameTime)
        {
            
        }

        public static void PreDraw(GameTime gameTime)
        {

        }
        public static void PostDraw(GameTime gameTime)
        {

        }
        
        */

        public void ApplyHooks()
        {
            /*
          Unused Hooks

          MethodDefinition initMethod = GetMethodDefinition(GetTypeDefinition("Game1"), "Initialize");
          var initProc = initMethod.Body.GetILProcessor();
          initProc.InsertBefore(initMethod.Body.Instructions[0], initProc.Create(OpCodes.Call,
              m_vModDefinition.ImportReference(typeof(Stardew_Hooker).GetMethod("PreInitialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
              ));

          initProc.InsertBefore(initMethod.Body.Instructions[initMethod.Body.Instructions.Count - 2], initProc.Create(OpCodes.Call,
              m_vModDefinition.ImportReference(typeof(Stardew_Hooker).GetMethod("PostInitialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
              ));

          MethodDefinition updateMethod = GetMethodDefinition(GetTypeDefinition("Game1"), "Update");

          var updateProc = updateMethod.Body.GetILProcessor();

          var updateInst = updateMethod.Body.Instructions[0];
          updateProc.InsertBefore(updateInst, updateProc.Create(OpCodes.Ldarg_1)); // push gameTime
          updateProc.InsertBefore(updateInst, updateProc.Create(OpCodes.Call,
              this.m_vModDefinition.ImportReference(typeof(Stardew_Hooker).GetMethod("PreUpdate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
              ));

          updateProc.InsertBefore(updateMethod.Body.Instructions[updateMethod.Body.Instructions.Count - 1], updateProc.Create(OpCodes.Ldarg_1)); // push gameTime
          updateProc.InsertBefore(updateMethod.Body.Instructions[updateMethod.Body.Instructions.Count - 1], updateProc.Create(OpCodes.Call,
              this.m_vModDefinition.ImportReference(typeof(Stardew_Hooker).GetMethod("PostUpdate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
              ));

          MethodDefinition drawMethod = GetMethodDefinition(GetTypeDefinition("Game1"), "Draw");

          var drawProc = drawMethod.Body.GetILProcessor();
          var drawInst = drawMethod.Body.Instructions[0];
          drawProc.InsertBefore(drawInst, drawProc.Create(OpCodes.Ldarg_1)); // push gameTime
          drawProc.InsertBefore(drawInst, drawProc.Create(OpCodes.Call,
              this.m_vModDefinition.ImportReference(typeof(Stardew_Hooker).GetMethod("PreDraw", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
              ));

          drawProc.InsertBefore(drawMethod.Body.Instructions[drawMethod.Body.Instructions.Count - 1], drawProc.Create(OpCodes.Ldarg_1));
          drawProc.InsertBefore(drawMethod.Body.Instructions[drawMethod.Body.Instructions.Count - 1], drawProc.Create(OpCodes.Call,
              this.m_vModDefinition.ImportReference(typeof(Stardew_Hooker).GetMethod("PostDraw", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
              ));

          */

            InjectMovementSpeed();
            InjectClockScale();
            InjectEasyFishing();
            InjectMoreBubbles();
        }

        private void InjectMoreBubbles()
        {
            bool enabled = false;
            try
            {
                enabled = bool.Parse(ConfigurationManager.AppSettings["EnableAlwaysSpawnFishingBubble"]);
            }
            catch
            {
                // ignored
            }

            if (!enabled) return;

            MethodDefinition movementSpeedMethod = GetMethodDefinition(GetTypeDefinition("GameLocation"), "performTenMinuteUpdate");
           // DumpInstructionsToFile(movementSpeedMethod);
            var ilProcessor = movementSpeedMethod.Body.GetILProcessor();
            bool changed = false;
            for (int i = 0; i < ilProcessor.Body.Instructions.Count; i++)
            {
                var instruction = ilProcessor.Body.Instructions[i];
                if(instruction.ToString().ToLower().Contains("nextdouble"))
                {
                    var start = ilProcessor.Body.Instructions.Skip(i);
                    var end = start.Take(5);
                    if(end.Any(ins => ins.ToString().ToLower().Contains("stardewvalley.farm")))
                    {
                        changed = true;
                        ilProcessor.Replace(ilProcessor.Body.Instructions[i+1], ilProcessor.Create(OpCodes.Ldc_R8, 1.1));
                        Console.WriteLine("Replacing line {0} with ldc.r8 1.1, forces each area to have a fishing bubble area", i + 1);
                        break;
                    }
                }
            }
            if(!changed)
            {
                Console.WriteLine("Couldn't find il to change for fishing bubbles.");
            }
            for (int i = 0; i < ilProcessor.Body.Instructions.Count; i++)
                Console.WriteLine(i + ":" + ilProcessor.Body.Instructions[i]);
        }

        private void InjectEasyFishing()
        {
            bool enabled = false;
            try
            {
                enabled = bool.Parse(ConfigurationManager.AppSettings["EnableEasyFishing"]);
            }
            catch
            {
                // ignored
            }

            if (!enabled) return;

            MethodDefinition movementSpeedMethod = GetMethodDefinition(GetTypeDefinition("BobberBar"), "update");
            //DumpInstructionsToFile(movementSpeedMethod);

            var ilProcessor = movementSpeedMethod.Body.GetILProcessor();
            var firstInstruction = ilProcessor.Body.Instructions.First();
            var lastInstruction = ilProcessor.Body.Instructions[ilProcessor.Body.Instructions.Count - 1];

            ilProcessor.Replace(ilProcessor.Body.Instructions[907], ilProcessor.Create(OpCodes.Ldc_R4, 0.001f));
            Console.WriteLine("Replaced line 907 with opcode ldc.r4 with a value of 0.001, Slows down fish escape for all bobbers");

            ilProcessor.Replace(ilProcessor.Body.Instructions[909], ilProcessor.Create(OpCodes.Ldc_R4, 0.001f));
            Console.WriteLine("Replaced line 909 with opcode ldc.r4 with a value of 0.001, Slows down fish escape for bobber id: 694");

            for (int i = 0; i < ilProcessor.Body.Instructions.Count; i++)
                Console.WriteLine(i + ":" + ilProcessor.Body.Instructions[i]);
        }

        private void InjectClockScale()
        {
            int timeScale = 7;
            try
            {
                timeScale = int.Parse(ConfigurationManager.AppSettings["SecondsPerTenMinutes"]);
            }
            catch
            {
                // ignored
            }

            if (timeScale == 7) return;

            timeScale *= 1000;

            MethodDefinition movementSpeedMethod = GetMethodDefinition(GetTypeDefinition("Game1"), "UpdateGameClock");

            var ilProcessor = movementSpeedMethod.Body.GetILProcessor();
            var firstInstruction = ilProcessor.Body.Instructions.First();
            var lastInstruction = ilProcessor.Body.Instructions[ilProcessor.Body.Instructions.Count - 1];

            ilProcessor.Replace(ilProcessor.Body.Instructions[42], ilProcessor.Create(OpCodes.Ldc_R4, timeScale * 1.0f));
            Console.WriteLine("Replaced line 42 with opcode ldc.r4 with a value of {0}, Updates lighting for new time scale", timeScale);

            ilProcessor.Replace(ilProcessor.Body.Instructions[89], ilProcessor.Create(OpCodes.Ldc_R4, timeScale * 1.0f));
            Console.WriteLine("Replaced line 89 with opcode ldc.r4 with a value of {0}, Updates lighting for new time scale part 2", timeScale);

            ilProcessor.Replace(ilProcessor.Body.Instructions[136], ilProcessor.Create(OpCodes.Ldc_I4, timeScale));
            Console.WriteLine("Replaced line 136 with opcode ldc.i4 with a value of {0}, Updates lighting for new time scale part 2", timeScale);

            for (int i = 0; i < ilProcessor.Body.Instructions.Count; i++)
                Console.WriteLine(i + ":" + ilProcessor.Body.Instructions[i]);
        }

        private void InjectMovementSpeed()
        {
            MethodDefinition movementSpeedMethod = GetMethodDefinition(GetTypeDefinition("Farmer"), "getMovementSpeed");

            var ilProcessor = movementSpeedMethod.Body.GetILProcessor();
            var firstInstruction = ilProcessor.Body.Instructions.First();
            var lastInstruction = ilProcessor.Body.Instructions[ilProcessor.Body.Instructions.Count - 1];

            //Basic pattern checking. Un-tested, patch could still break it!
            for (int i = 0; i < ilProcessor.Body.Instructions.Count; i++)
            {
               if(ilProcessor.Body.Instructions[i].ToString().Contains("Enumerable"))
               {
                    //Next instruction
                    ilProcessor.Replace(ilProcessor.Body.Instructions[i+1], ilProcessor.Create(OpCodes.Ldc_I4_4));
                    Console.WriteLine("Replaced line " + i + "  with opcode ldc.i4.4, removing diagonal movement check");
                }
            }

            var speed = 1f;
            try
            {
                speed = float.Parse(ConfigurationManager.AppSettings["RunSpeed"]);
            }
            catch
            {
                // ignored
            }

            Console.WriteLine("Modifying movement speed...Adding: " + speed + " to the total movement speed");

            ilProcessor.InsertBefore(lastInstruction, ilProcessor.Create(OpCodes.Ldc_R4, (float)speed));
            ilProcessor.InsertBefore(lastInstruction, ilProcessor.Create(OpCodes.Add));

            Console.WriteLine("");
            Console.WriteLine("Finished modifiying: " + movementSpeedMethod.FullName);
            Console.WriteLine("");
            Console.WriteLine("Final value: ");
            Console.WriteLine("");

            for (int i = 0; i < ilProcessor.Body.Instructions.Count; i++)
                Console.WriteLine(i + ":" + ilProcessor.Body.Instructions[i]);
        }
        private void DumpInstructionsToFile(MethodDefinition methodDefinition)
        {
            var fileName = string.Format("{0}.{1}.txt", methodDefinition.DeclaringType.Name, methodDefinition.Name);

            using (var stream = File.OpenWrite(Path.Combine(".", fileName)))
                using(var writer = new StreamWriter(stream))
            {
                var ilProcessor = methodDefinition.Body.GetILProcessor();
                for (int i = 0; i < ilProcessor.Body.Instructions.Count; i++)
                    writer.WriteLine((i) + ":" + ilProcessor.Body.Instructions[i]);
            }
        }
    }
}
