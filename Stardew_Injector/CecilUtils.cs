using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stardew_Injector
{
    public struct ScannerState
    {
        public ILProcessor ILProcessor;
        public Instruction Instruction;

        public ScannerState(ILProcessor ilProc, Instruction ins)
        {
            ILProcessor = ilProc;
            Instruction = ins;
        }

        public ScannerState Previous(Func<Instruction, bool> until = null)
        {
            if (until != null)
            {
                Instruction cur = this.Instruction;
                do
                {
                    cur = cur.Previous;
                } while (!until(cur));
                return new ScannerState(this.ILProcessor, cur);
            }
            return new ScannerState(this.ILProcessor, Instruction.Previous);
        }

        public ScannerState Next(Func<Instruction, bool> until = null)
        {
            if (until != null)
            {
                Instruction cur = this.Instruction;
                do
                {
                    cur = cur.Next;
                } while (!until(cur));
                return new ScannerState(this.ILProcessor, cur);
            }
            return new ScannerState(this.ILProcessor, Instruction.Next);
        }

        public ScannerState Last()
        {
            var instructions = this.ILProcessor.Body.Instructions;
            return new ScannerState(this.ILProcessor, instructions[instructions.Count - 1]);
        }

        public ScannerState First()
        {
            var instructions = this.ILProcessor.Body.Instructions;
            return new ScannerState(this.ILProcessor, instructions[0]);
        }

        public ScannerState ReplaceCreate(OpCode opcode)
        {
            Instruction ins = this.ILProcessor.Create(opcode);
            this.ILProcessor.Replace(this.Instruction, ins);
            return new ScannerState(this.ILProcessor, ins);
        }

        public ScannerState ReplaceCreate(OpCode opcode, object arg)
        {
            Instruction ins = this.ILProcessor.Create(opcode, arg as dynamic);
            this.ILProcessor.Replace(this.Instruction, ins);
            return new ScannerState(this.ILProcessor, ins);
        }

        public ScannerState CreateBefore(OpCode opcode)
        {
            Instruction ins = this.ILProcessor.Create(opcode);
            this.ILProcessor.InsertBefore(this.Instruction, ins);
            return new ScannerState(this.ILProcessor, ins);
        }

        public ScannerState CreateBefore(OpCode opcode, object arg)
        {
            Instruction ins = this.ILProcessor.Create(opcode, arg as dynamic);
            this.ILProcessor.InsertBefore(this.Instruction, ins);
            return new ScannerState(this.ILProcessor, ins);
        }

        public ScannerState CreateAfter(OpCode opcode)
        {
            Instruction ins = this.ILProcessor.Create(opcode);
            this.ILProcessor.InsertAfter(this.Instruction, ins);
            return new ScannerState(this.ILProcessor, ins);
        }

        public ScannerState CreateAfter(OpCode opcode, object arg)
        {
            Instruction ins = this.ILProcessor.Create(opcode, arg as dynamic);
            this.ILProcessor.InsertAfter(this.Instruction, ins);
            return new ScannerState(this.ILProcessor, ins);
        }
    }

    public static class CecilUtils
    {
        public static ScannerState Scanner(this MethodDefinition me)
        {
            return new ScannerState(me.Body.GetILProcessor(), me.Body.Instructions[0]);
        }

        public static ScannerState FindSetField(this MethodDefinition me, string fieldName)
        {
            var instruction = me.Body.Instructions
                .FirstOrDefault(i => i.OpCode == OpCodes.Stsfld && (i.Operand as FieldDefinition).Name == fieldName);
            return new ScannerState(me.Body.GetILProcessor(), instruction);
        }

        public static ScannerState FindLoadField(this MethodDefinition me, string fieldName)
        {
            var instruction = me.Body.Instructions
                .FirstOrDefault(i => {
                    if (i.OpCode != OpCodes.Ldfld && i.OpCode != OpCodes.Ldsfld)
                        return false;
                    if (i.Operand is FieldDefinition && (i.Operand as FieldDefinition).Name == fieldName)
                        return true;
                    if (i.Operand is FieldReference && (i.Operand as FieldReference).Name == fieldName)
                        return true;
                    return false;
                });
            return new ScannerState(me.Body.GetILProcessor(), instruction);
        }

        public static ScannerState FindLoadConstant(this MethodDefinition me, int val)
        {
            var instruction = me.Body.Instructions
                .FirstOrDefault(i => i.OpCode == OpCodes.Ldc_I4 && (int)i.Operand == val);
            return new ScannerState(me.Body.GetILProcessor(), instruction);
        }

        public static ScannerState FindLoadConstant(this MethodDefinition me, float val)
        {
            var instruction = me.Body.Instructions
                .FirstOrDefault(i => i.OpCode == OpCodes.Ldc_R4 && (float)i.Operand == val);
            return new ScannerState(me.Body.GetILProcessor(), instruction);
        }

        public static MethodDefinition FindMethod(this ModuleDefinition me, string name)
        {
            var nameSplit = name.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            if (nameSplit.Length < 2)
                throw new ArgumentException("Invalid method full name", "name");

            var currentType = me.Types.FirstOrDefault(t => t.FullName == nameSplit[0]);
            if (currentType == null)
                return null;

            return currentType.Methods.FirstOrDefault(m => m.Name == nameSplit[1]);
        }
    }
}
