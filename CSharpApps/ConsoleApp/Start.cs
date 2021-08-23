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
            foreach (var f in Directory.GetFiles(".", "*.dll"))
            {
                Assembly assembly = Assembly.LoadFrom(f);
                Type[] type = assembly.GetTypes();
                foreach (var item in type)
                {
                    object obj = Activator.CreateInstance(item);
                    MethodInfo minfo = item.GetMethod("Test");
                    minfo.Invoke(obj, null);
                }
            }
        }
    }
}
