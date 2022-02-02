using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CodeLibrary
{
    public class UploadService : IUploadService
    {
        public bool TestMessage(string msg)
        {
            return true;
        }
        public UploadService()
        {

        }
        public void Test()
        {
            IPAddress addr = IPAddress.Parse("win-30qs6lmodrp.epm.blr.novell.com:443/");
            string[] vs = new string[] { "1", "2", "3" };
            int i = 1;
            while(vs.Length > 0)
            {
                string val = i.ToString();
                i++;
                vs = vs.Except(new string[] { val }).ToArray();
                if(i==2)
                {
                    break;
                }
            }

            TestChunks();

        }

        private static void TestChunks()
        {
            string file = @"C:\work\learn\projects\cSharp-apps\CSharpApps\lib\data\test.txt";
            string outfile = Path.GetDirectoryName(file) + "\\out.txt";
            Stream inputStream = File.OpenRead(file);


            int bytesRead = -1;
            FileInfo fileInfo = new FileInfo(file);
            long fileSize = fileInfo.Length;
            long chunkSize = 5*1024 * 1024; // 5 mb
            long chunkCount = 0;
            long chk = fileSize % chunkSize;
            long chkcnt = chunkSize + ((fileSize % chunkSize != 0) ? 1 : 0);
            long totalChunkCount = fileSize / chunkSize + ((fileSize % chunkSize != 0) ? 1 : 0);
            byte[] buffer = new byte[chunkSize];  

            string relativePath = Path.GetFileNameWithoutExtension(file);
            //Stream outputStream = File.OpenWrite(outfile);
           
            
            
            if (File.Exists(outfile))
            {
                File.Delete(outfile);
            }
            FileStream outputStream = new FileStream(outfile, FileMode.Append);
            

            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
            while (bytesRead > 0)
            {
                HandleChunks(inputStream, ref bytesRead, ref chunkCount, buffer, outputStream,totalChunkCount);

            }
            //outputStream.Close();
        }

        private static void HandleChunks(Stream inputStream, ref int bytesRead, ref long chunkCount, byte[] buffer, FileStream outputStream,long totalChunkCount)
        {
            chunkCount++;
            Console.WriteLine("bytes read " + bytesRead);
            outputStream.Write(buffer, 0, bytesRead);

            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
            outputStream.Flush();
            if(chunkCount==totalChunkCount)
            {
                outputStream.Close();
            }
        }





    }

}
