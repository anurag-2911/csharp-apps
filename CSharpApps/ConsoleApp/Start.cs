using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Start
    {
        static void Main(string[] args)
        {
            // call every method that has name Test
            try
            {
                RunMethod();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            

            Console.ReadKey();
        }

        private static void RunMethod()
        {
            try
            {
                foreach (var f in Directory.GetFiles(".", "*.dll"))
                {
                    Assembly assembly = Assembly.LoadFrom(f);
                    if (assembly != null)
                    {
                        Type[] type = assembly.GetTypes();
                        if (type != null)
                        {
                            foreach (var item in type)
                            {
                                object obj = Activator.CreateInstance(item);
                                if (item != null)
                                {
                                    MethodInfo minfo = item.GetMethod("Test");
                                    if (minfo != null)
                                    {
                                        minfo.Invoke(obj, null);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
