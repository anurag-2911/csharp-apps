using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLibrary.practice
{
    public class LoopHoles
    {
        public void Test()
        {
            for (int i = 0; i < 5; i++)
            {
                if (true)
                {
                    if(true)
                    {
                        if(!true)
                        {
                            
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {

                }
            }
        }
    }
}
