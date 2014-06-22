﻿using System;
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
            vm.init("sys.print('Hello World')");
           
            
            Console.ReadKey();
            vm.close();
        }
    }
}
