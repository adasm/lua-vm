using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using LuaInterface;

namespace lua_vm
{
    class VirtualMachine
    {
        public class Method : Attribute
        {
            string luaMethodName;
            public Method(string luaMethodName)
            {
                this.luaMethodName = luaMethodName;
            }

            public string getLuaMethodName()
            {
                return luaMethodName;
            }
        }

        protected Lua lua = new Lua();

        public object[] init(string code)
        {
            return lua.DoString(code);
        }

        public void close()
        {
            lua.Close();
        }

        public void bind(string name, object obj, string methodName)
        {
            lua.RegisterFunction(name, obj, obj.GetType().GetMethod(methodName));
        }


        public void bind(string name, object obj, bool onlySpecifiedMethods = true)
        {
            lua.NewTable(name);
            Type objType = obj.GetType();
            foreach (MethodInfo mInfo in objType.GetMethods())
            {
                bind(name, obj, mInfo, onlySpecifiedMethods);
            }
        }

        protected void bind(string name, object obj, MethodInfo methodInfo, bool checkAttributes = true)
        {
            if (checkAttributes)
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(methodInfo))
                {
                    if (attr.GetType() == typeof(Method))
                    {
                        Method method = (Method)attr;
                        lua.RegisterFunction(name + "." + method.getLuaMethodName(), obj, methodInfo);
                    }
                }
            }
            else
            {
                lua.RegisterFunction(name + "." + methodInfo.Name, obj, methodInfo);
            }
        }

        public void set(string name, object obj)
        {
            lua[name] = obj;
        }

        public object get(string name)
        {
            return lua[name];
        }

        public object[] call(string funcName, params object[] param)
        {
            LuaFunction func = lua.GetFunction(funcName);
            return func.Call(new object[] {});
        }

        public object[] call(string funcName)
        {
            return call(funcName, new object[] { });
        }

    }
}
