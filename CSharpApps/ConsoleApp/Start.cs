using System;
using System.IO;
using System.Reflection;


namespace ConsoleApp
{
    class Start
    {
        static void Main(string[] args)
        {
            //HostWebService();
            //call every method that has name Test
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
                                try
                                {
                                    if (item.IsClass)
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
                                catch (Exception) { }
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
