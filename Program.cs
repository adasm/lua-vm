using System;
using System.Collections.Generic;
using System.Text;
using LuaInterface;

namespace lua_vm
{
    class Program
    {
        static void Main(string[] args)
        {
            VirtualMachine vm = new VirtualMachine();

            vm.bind("sys", new BindableClass());
            vm.bind("vm", new VirtualMachine(), false); //inception
            vm.init("sys.line('Lua Binding C# example')");

            string line;
            while (true)
            {
                vm.init("sys.write('\\n>>')");
                line = Console.ReadLine();
                if (line.Equals("")) break;

                object[] ret = vm.init(line);
                if(ret == null)
                    Console.WriteLine(vm.lastException != null ? vm.lastException.Message : "Code error");
                else
                {
                    foreach(object o in ret) {
                        Console.Write(o + ", ");
                    }
                }
            }

            vm.close();
        }
    }
}
