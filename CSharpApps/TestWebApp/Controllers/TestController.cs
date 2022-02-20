using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Net;

namespace TestWebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class zenworks_content : ControllerBase
    {
        [HttpPost("uploadFile")]
        public HttpStatusCode TestMethod()
        {
            string outfile = GetOutputFile();

            HttpContext curContext = this.HttpContext;
           
            
            if (curContext != null)
            {
                WriteToOuputFile(outfile, curContext);

            }

            return HttpStatusCode.BadRequest;

        }
              
        private static string GetOutputFile()
        {
            string file = @"C:\work\learn\projects\cSharp-apps\CSharpApps\lib\data\test.txt";
            string outfile = Path.GetDirectoryName(file) + "\\result.txt";
            return outfile;
        }

        private static void WriteToOuputFile(string outfile, HttpContext curContext)
        {
            long? totalBytes = curContext.Request.ContentLength;
            string encoding = curContext.Request.ContentType;

            StringValues currentChunk = GetQueryStringParameters(curContext);

            byte[] buffer = new byte[totalBytes.Value];
            curContext.Request.Body.ReadAsync(buffer, 0, Convert.ToInt32(totalBytes.Value));
            if (!string.IsNullOrEmpty(currentChunk))
            {
                if (Convert.ToInt32(currentChunk.ToString()) == 1)
                {
                    try
                    {
                        if (System.IO.File.Exists(outfile))
                        {
                            System.IO.File.Delete(outfile);
                        }
                    }
                    catch { }

                }
            }
            FileStream outputStream = new FileStream(outfile, FileMode.Append);
            outputStream.Write(buffer, 0, Convert.ToInt32(totalBytes.Value));
            outputStream.Close();

        }

        private static StringValues GetQueryStringParameters(HttpContext curContext)
        {
            StringValues fileName = string.Empty;
            curContext.Request.Query.TryGetValue("fileName", out fileName);
            StringValues consumerType = string.Empty;
            curContext.Request.Query.TryGetValue("consumerType", out consumerType);
            StringValues totalChunks = string.Empty;
            curContext.Request.Query.TryGetValue("totalChunks", out totalChunks);
            StringValues currentChunk = string.Empty;
            curContext.Request.Query.TryGetValue("currentChunk", out currentChunk);
            StringValues lastModifiedTime = string.Empty;
            curContext.Request.Query.TryGetValue("lastModifiedTime", out lastModifiedTime);
            StringValues overWrite = string.Empty;
            curContext.Request.Query.TryGetValue("overWrite", out overWrite);
            return currentChunk;
        }

    }
}
