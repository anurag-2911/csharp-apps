using CodeLibrary;
using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Web;


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

        private static void HostWebService()
        {
            Uri baseAddress = new Uri("http://localhost:8080/");

            WebServiceHost svcHost = new WebServiceHost(typeof(UploadService), baseAddress);

            try
            {
                svcHost.Open();

                Console.WriteLine("Service is running");
                Console.WriteLine("Press enter to quit...");
                Console.ReadLine();

                svcHost.Close();
            }
            catch (CommunicationException cex)
            {
                Console.WriteLine("An exception occurred: {0}", cex.Message);
                svcHost.Abort();
            }
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
