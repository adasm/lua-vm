using System;
using System.Collections.Generic;
using System.Text;

namespace lua_vm
{
    class BindableClass
    {
        [VirtualMachine.LuaBind("a")]
        public Double var = 3.13;

        [VirtualMachine.LuaBind("write")]
        public void Print(string str)
        {
            Console.Write(str);
        }

        [VirtualMachine.LuaBind("line")]
        public void PrintLine(string str)
        {
            Console.WriteLine(str);
        }
    }
}
