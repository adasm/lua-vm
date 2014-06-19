using System;
using System.Collections.Generic;
using System.Text;

namespace lua_vm
{
    class BindableClass
    {
        [VirtualMachine.Method("print")]
        public void Print(string str)
        {
            Console.WriteLine(str);
        }
    }
}
