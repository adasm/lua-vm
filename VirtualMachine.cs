using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using LuaInterface;

namespace lua_vm
{
    class VirtualMachine
    {
        public class LuaBind : Attribute
        {
            string luaBindName;
            public LuaBind(string luaBindName)
            {
                this.luaBindName = luaBindName;
            }

            public string getLuaBindName()
            {
                return luaBindName;
            }
        }

        protected Lua lua = new Lua();

        public LuaException lastException = null;

        public object[] init(string code)
        {
           try
           {
                return lua.DoString(code);
           }
           catch (LuaException e)
           {
               lastException = e;
               return null;
           }
        }

        public void close()
        {
            lua.Close();
        }

        public void bind(string name, object obj, string methodName)
        {
            lua.RegisterFunction(name, obj, obj.GetType().GetMethod(methodName));
        }


        public void bind(string name, object obj, bool onlyWithSpecifiedAttributes = true)
        {
            lua.NewTable(name);
            Type objType = obj.GetType();
            foreach (MethodInfo methodInfo in objType.GetMethods())
            {
                bind(name, obj, methodInfo, onlyWithSpecifiedAttributes);
            }

            foreach (FieldInfo fieldInfo in objType.GetFields())
            {
                bind(name, obj, fieldInfo, onlyWithSpecifiedAttributes);
            }
        }

        protected void bind(string name, object obj, MethodInfo methodInfo, bool checkAttributes = true)
        {
            if (checkAttributes)
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(methodInfo))
                {
                    if (attr.GetType() == typeof(LuaBind))
                    {
                        LuaBind luaBind = (LuaBind)attr;
                        register(name + "." + luaBind.getLuaBindName(), obj, methodInfo);
                    }
                }
            }
            else
            {
                register(name + "." + methodInfo.Name, obj, methodInfo);
            }
        }

        protected void bind(string name, object obj, FieldInfo fieldInfo, bool checkAttributes = true)
        {
            if (checkAttributes)
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(fieldInfo))
                {
                    if (attr.GetType() == typeof(LuaBind))
                    {
                        LuaBind luaBind = (LuaBind)attr;
                        register(name + "." + luaBind.getLuaBindName(), fieldInfo.GetValue(obj));
                    }
                }
            }
            else
            {
                register(name + "." + fieldInfo.Name, fieldInfo.GetValue(obj));
            }
        }

        protected void register(string name, object obj, MethodInfo methodInfo)
        {
            lua.RegisterFunction(name, obj, methodInfo);
            //Console.WriteLine("Registered method " + name);
        }

        protected void register(string name, object obj)
        {
            lua[name] = obj;
            //Console.WriteLine("Registered field " + name + " with initial value " + obj);
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
            try
            {
                return func.Call(new object[] { });
            }
            catch (LuaException e)
            {
                lastException = e;
                return null;
            } 
        }

        public object[] call(string funcName)
        {
            return call(funcName, new object[] { });
        }

    }
}
