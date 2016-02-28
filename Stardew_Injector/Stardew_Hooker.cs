using Microsoft.Xna.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
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

            //Farmer movement speed

            MethodDefinition movementSpeedMethod = GetMethodDefinition(GetTypeDefinition("Farmer"), "getMovementSpeed");

            var ilProcessor = movementSpeedMethod.Body.GetILProcessor();
            var firstInstruction = ilProcessor.Body.Instructions.First();
            var lastInstruction = ilProcessor.Body.Instructions[ilProcessor.Body.Instructions.Count - 1];


            //CAN CHANGE WHEN GAME IS UPDATED....MIGHT BREAK DURING NEXT UPDATE
            ilProcessor.Replace(ilProcessor.Body.Instructions[23], ilProcessor.Create(OpCodes.Ldc_I4_4));
            Console.WriteLine("Replaced line 23 with opcode ldc.i4.4, removing diagonal movement check");

            string text = System.IO.File.ReadAllText(@"movementSpeed.txt");
            int speed = Int32.Parse(text);

            Console.WriteLine("Modifying movement speed...Adding: " + speed + " to the total movement speed");

            ilProcessor.InsertBefore(lastInstruction, ilProcessor.Create(OpCodes.Ldc_I4, speed));
            ilProcessor.InsertBefore(lastInstruction, ilProcessor.Create(OpCodes.Add));

            Console.WriteLine("");
            Console.WriteLine("Finished modifiying: " + movementSpeedMethod.FullName);
            Console.WriteLine("");
            Console.WriteLine("Final value: ");
            Console.WriteLine("");

            for (int i = 0; i < ilProcessor.Body.Instructions.Count; i++)
                Console.WriteLine(i + ":" + ilProcessor.Body.Instructions[i]);

          



        }

    }
}
