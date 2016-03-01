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

        public bool Finalize()
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
            if (this.m_vAssembly == null)
                return false;

            Console.WriteLine("Starting Stardew Valley...");

            m_vAssembly.EntryPoint.Invoke(null, new object[] { new string[0] });

            return true;
        }

        public void ApplyHooks()
        {
            InjectMovementSpeed();

            if (Config.SecondsPerTenMinutes != 7)
                InjectClockScale();

            if (Config.EnableEasyFishing)
                InjectEasyFishing();

            if (Config.EnableAlwaysSpawnFishingBubble)
                InjectMoreBubbles();

            if (Config.EnableDebugMode)
                InjectDebugMode();


        }

        private void InjectDebugMode()
        {
            this.m_vModDefinition.FindMethod("StardewValley.Program::.cctor")
                .FindSetField("releaseBuild").Previous()
                .ReplaceCreate(OpCodes.Ldc_I4_0);

            Console.WriteLine("Enabled debug mode.");
        }

        private void InjectMoreBubbles()
        {
            this.m_vModDefinition.FindMethod("StardewValley.GameLocation::performTenMinuteUpdate")
                .FindLoadField("currentLocation").Next(i => i.ToString().Contains("NextDouble")).Next()
                    .ReplaceCreate(OpCodes.Ldc_R8, 1.1);

            Console.WriteLine("Forced each area to always spawn a fishing bubble.");
        }

        private void InjectEasyFishing()
        {
            this.m_vModDefinition.FindMethod("StardewValley.Menus.BobberBar::update")
                .FindLoadConstant(694)
                .Next(i => i.OpCode == OpCodes.Ldc_R4)
                    .ReplaceCreate(OpCodes.Ldc_R4, 0.001f)
                .Next(i => i.OpCode == OpCodes.Ldc_R4)
                    .ReplaceCreate(OpCodes.Ldc_R4, 0.001f);

            Console.WriteLine("Replaced fish escape constants for all bobbers & bobber id 694 with 0.001, slowing it down.");
        }

        private void InjectClockScale()
        {
            int timeScale = Config.SecondsPerTenMinutes;
            timeScale *= 1000;

            this.m_vModDefinition.FindMethod("StardewValley.Game1::UpdateGameClock")
                .FindLoadConstant(7000f)
                    .ReplaceCreate(OpCodes.Ldc_R4, timeScale * 1.0f)
                .Next(i => i.OpCode == OpCodes.Ldc_R4 && (float)i.Operand == 7000f)
                    .ReplaceCreate(OpCodes.Ldc_R4, timeScale * 1.0f)
                .Next(i => i.OpCode == OpCodes.Ldc_I4 && (int)i.Operand == 7000)
                    .ReplaceCreate(OpCodes.Ldc_I4, timeScale);

            Console.WriteLine("Updated lighting for new timescale ({0}).", timeScale);
        }

        private void InjectMovementSpeed()
        {
           

            if(Config.EnableTweakedDiagonalMovement)
            {
                this.m_vModDefinition.FindMethod("StardewValley.Farmer::getMovementSpeed")
               .FindLoadField("movementDirections").Next(i => i.OpCode == OpCodes.Ldc_I4_1)
                   .ReplaceCreate(OpCodes.Ldc_I4_4);

                Console.WriteLine("Removed diagonal movement check.");
            }

            if(Config.RunSpeed > 0)
            {
                this.m_vModDefinition.FindMethod("StardewValley.Farmer::getMovementSpeed")
              .FindLoadField("movementDirections").Last().CreateBefore(OpCodes.Ldc_R4, (float)Config.RunSpeed).CreateAfter(OpCodes.Add);

                Console.WriteLine("Added run speed: " + Config.RunSpeed);
            }

         
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
