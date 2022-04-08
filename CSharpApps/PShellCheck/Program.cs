using System;
using System.Management.Automation;

namespace PShellCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Using the PowerShell.Create and AddCommand
                // methods, create a command pipeline.
                PowerShell ps = PowerShell.Create().AddCommand("Sort-Object");

                // Using the PowerShell.Invoke method, run the command
                // pipeline using the supplied input.
                foreach (PSObject result in ps.Invoke(new int[] { 3, 1, 6, 2, 5, 4 }))
                {
                    Console.WriteLine("{0}", result);
                } // End foreach.
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.ReadKey();

        } // End Main.
    }
}
