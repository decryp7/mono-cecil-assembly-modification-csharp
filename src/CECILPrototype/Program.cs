using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CECILPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            //var targetAssembly = AssemblyDefinition.ReadAssembly("../../../TestApp/bin/debug/TestApp.exe");
            //var mainModuleDefinition = targetAssembly.MainModule;
            //var targetType = targetAssembly.MainModule.Types.FirstOrDefault(e => e.Name == "Program");
            //var targetMethod = targetType.Methods.FirstOrDefault(e => e.Name == "Main");
            //var processor = targetMethod.Body.GetILProcessor();

            //Console.WriteLine("---------------Before-------------------");
            //foreach (var bodyInstruction in targetMethod.Body.Instructions)
            //{
            //    Console.WriteLine(bodyInstruction);
            //}
            //Console.WriteLine("");

            ////create new method
            //var newMethod = new MethodDefinition("SayHello",
            //    MethodAttributes.Private | MethodAttributes.Static, targetAssembly.MainModule.TypeSystem.String);
            //var newMethodProcessor = newMethod.Body.GetILProcessor();
            //var helloString = newMethodProcessor.Create(OpCodes.Ldstr, "Hello World! Application is modified!");
            //newMethod.Body.Instructions.Insert(0, helloString);
            //newMethod.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Ret));
            //targetType.Methods.Add(newMethod);

            ////Call the new method with Console.WriteLine()
            //MethodReference writeLineMethod =
            //    targetAssembly.MainModule.ImportReference(typeof(Console).GetMethod("WriteLine",
            //        new Type[] { typeof(string) }));
            //targetMethod.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Call, newMethod));
            //targetMethod.Body.Instructions.Insert(2, Instruction.Create(OpCodes.Call, writeLineMethod));

            //Console.WriteLine("---------------After-------------------");
            //foreach (var bodyInstruction in targetMethod.Body.Instructions)
            //{
            //    Console.WriteLine(bodyInstruction);
            //}
            //Console.WriteLine("");

            var targetAssembly = AssemblyDefinition.ReadAssembly("TestApp.exe");
            var mainModuleDefinition = targetAssembly.MainModule;
            var targetType = targetAssembly.MainModule.Types.FirstOrDefault(e => e.Name == "DerivedClass");
            var baseTypeDefinition = targetType.BaseType as TypeDefinition;

            var sayHelloVirtualMethod = baseTypeDefinition.Methods.FirstOrDefault(e => e.Name == "SayHello");
            MethodDefinition overrideSayHelloMethod = new MethodDefinition("SayHello", sayHelloVirtualMethod.Attributes,
                mainModuleDefinition.TypeSystem.String)
            {
                Name = sayHelloVirtualMethod.Name,
                Attributes = sayHelloVirtualMethod.Attributes & ~MethodAttributes.NewSlot
            };

            overrideSayHelloMethod.Attributes |= MethodAttributes.ReuseSlot;
            overrideSayHelloMethod.ImplAttributes = sayHelloVirtualMethod.ImplAttributes;
            overrideSayHelloMethod.SemanticsAttributes = sayHelloVirtualMethod.SemanticsAttributes;
            var overrideSayHelloMethodProcessor = overrideSayHelloMethod.Body.GetILProcessor();
            var helloString = overrideSayHelloMethodProcessor.Create(OpCodes.Ldstr, "Hello from Overridden SayHello in Derived Class!");
            overrideSayHelloMethod.Body.Instructions.Insert(0, helloString);
            overrideSayHelloMethod.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Ret));
            targetType.Methods.Add(overrideSayHelloMethod);

            targetAssembly.Write("Modified_TestApp.exe");

            Console.WriteLine("TestApp.exe is modified to Modified_TestApp.exe. Press enter to exit...");
            Console.ReadLine();
        }
    }
}
