using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeLibrary
{
    public class StatTest
    {
        static int count;
        static object lockobj=new object();
        public static void Increment()
        {
            lock(lockobj)
            {
                count++;
                Trace.WriteLine(string.Format(" increment : {0}, count : {1}",Thread.CurrentThread.Name,count));
            }
        }

        public static void Decrement()
        {
            lock(lockobj)
            {
                count--;
                Trace.WriteLine(string.Format(" decrement : {0}, count : {1}", Thread.CurrentThread.Name,count));
            }
        }
    }
}
