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
            string luaName;
            public Method(string luaName)
            {
                this.luaName = luaName;
            }

            public string getLuaName()
            {
                return luaName;
            }
        }

        private Lua lua = new Lua();

        public void close()
        {
            lua.Close();
        }

        public void bind(string name, object obj, string methodName)
        {
            lua.RegisterFunction(name, obj, obj.GetType().GetMethod(methodName));
        }

        public void bind(object obj)
        {
            Type objType = obj.GetType();
            foreach (MethodInfo mInfo in objType.GetMethods())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                {
                    if (attr.GetType() == typeof(Method))
                    {
                        Method method = (Method)attr;
                        if(lua != null)
                        lua.RegisterFunction(method.getLuaName(), obj, mInfo);
                    }
                }
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

        public object[] exec(string code)
        {
            return lua.DoString(code);
        }

    }
}
