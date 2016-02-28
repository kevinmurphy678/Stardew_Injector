using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stardew_Injector
{
    class Program
    {

        private static Stardew_Hooker hooker = new Stardew_Hooker();

        static void Main(string[] args)
        {
            hooker.Initialize();
            hooker.ApplyHooks();
            hooker.Finialize();
         
            hooker.Run();
            Console.ReadLine();
        }

    }
}
