namespace CodeLibrary.practice
{
    public class LoopHoles
    {
        public void Test()
        {
            //NewMethod();
        }

        private static void NewMethod()
        {
            for (int i = 0; i < 5; i++)
            {

                if (true)
                {

                    if (true)
                    {
                        if (true)
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
