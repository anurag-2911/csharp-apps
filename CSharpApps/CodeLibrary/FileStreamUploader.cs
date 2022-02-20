using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace CodeLibrary
{
    public class FileStreamUploader
    {
        public void Test()
        {
            UploadStreamData();

        }

        private void UploadStreamData()
        {
            string sourceFile = Environment.CurrentDirectory + "\\input2.txt";

            bool isFileCreated = CreateFile(sourceFile, 6); // x mb file
            if (isFileCreated)
            {
                long modifiedDate, totalchunkcount;
                int currentchunkcount, bytesRead;
                byte[] buffer;
                Stream sourceStream = null;

                ReadFileDetails(sourceFile, out modifiedDate, out currentchunkcount, out buffer, out totalchunkcount, out bytesRead,
                                out sourceStream);

                while (bytesRead > 0)
                {
                    try
                    {
                        currentchunkcount++;
                        WebClientWithResponse webClient = new WebClientWithResponse();
                        webClient.TimeOut = 10000000;
                        webClient.Headers.Add("Content-Type", "application/octet-stream");
                        NameValueCollection queryString = GetQueryStringParameters("test", sourceFile, modifiedDate.ToString(),
                                                          totalchunkcount, currentchunkcount, false);
                        string q = string.Join("&", queryString.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(queryString[a])));
                        webClient.QueryString.Clear();
                        webClient.QueryString.Add(queryString);
                        string URI = "https://localhost:443/zenworks-extcontent/upload";


                        Stream destinationStream = webClient.OpenWrite(URI, "POST");

                        destinationStream.Write(buffer, 0, bytesRead);
                        destinationStream.Close();

                        HttpResponseDetails response = webClient.httpResponseDetails;

                        bytesRead = sourceStream.Read(buffer, 0, buffer.Length);

                    }
                    catch (WebException webex)
                    {
                        Console.WriteLine(webex.ToString());
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        break;

                    }


                }

                sourceStream.Close();
            }
        }

        private static void ReadFileDetails(string sourceFile, out long modifiedDate, out int currentchunkcount, out byte[] buffer, 
                                            out long totalchunkcount, out int bytesRead, out Stream sourceStream)
        {
            FileInfo fileInfo = new FileInfo(sourceFile);

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

            modifiedDate = Convert.ToInt64((fileInfo.LastWriteTime - epoch).TotalMilliseconds);
            long fileSize = fileInfo.Length;

            long chunksize = 8 * 1024 * 1024;

           

            currentchunkcount = 0;
            sourceStream = File.OpenRead(sourceFile);

            buffer = new byte[chunksize];
            totalchunkcount = fileSize / chunksize + ((fileSize % chunksize != 0) ? 1 : 0);
            bytesRead = sourceStream.Read(buffer, 0, buffer.Length);
        }

        private NameValueCollection GetQueryStringParameters(string fileType, string sourceFile, string lastModifiedDate,
                                                   long totalChunkCount, int currentChunk, bool overwrite)
        {
            NameValueCollection queryParams = new NameValueCollection();
            queryParams["fileName"] = Path.GetFileName(sourceFile);
            queryParams["fileType"] = fileType;
            queryParams["totalChunks"] = totalChunkCount.ToString();
            queryParams["currentChunk"] = currentChunk.ToString();
            queryParams["lastModifiedTime"] = lastModifiedDate;
            queryParams["overwrite"] = overwrite.ToString();
            return queryParams;
        }

        private bool CreateFile(string fileName, int sizeInMb)
        {
            const int blockSize = 1024 * 8;
            const int blocksPerMb = (1024 * 1024) / blockSize;
            byte[] data = new byte[blockSize];
            Random rng = new Random();
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                using (FileStream stream = File.OpenWrite(fileName))
                {
                    // There 
                    for (int i = 0; i < sizeInMb * blocksPerMb; i++)
                    {
                        rng.NextBytes(data);
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Overrides GetWebResponse method to read the response from HttpWebResponse 
    /// </summary>
    public class WebClientWithResponse : WebClient
    {

        public int TimeOut { get; set; }

        public HttpResponseDetails httpResponseDetails { get; private set; }

        /// <summary>
        /// Adds Timeout to the WebRequest
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        //protected override WebRequest GetWebRequest(Uri address)
        //{
        //    var webRequest = base.GetWebRequest(address);
        //    if (webRequest != null)
        //    {
        //        webRequest.Timeout = TimeOut;
        //        var httpRequest = webRequest as HttpWebRequest;
        //        if (httpRequest != null)
        //        {
        //            httpRequest.ServerCertificateValidationCallback = 
        //                delegate (object s,X509Certificate certificate,X509Chain chain,
        //                                         SslPolicyErrors sslPolicyErrors) 
        //                {
        //                    return true;
        //                };
        //        }

        //    }
        //    return webRequest;
        //}
       
        /// <summary>
        /// Parse the response for the WebRequest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);

            HttpWebResponse httpResponse = response as HttpWebResponse;

            if (httpResponse != null)
            {
                httpResponseDetails = new HttpResponseDetails();
                httpResponseDetails.StatusCode = httpResponse.StatusCode;
                httpResponseDetails.StatusDesciption = httpResponse.StatusDescription;

                using (var stream = httpResponse.GetResponseStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        var responseBytes = ms.ToArray();
                        if (responseBytes != null)
                        {
                            httpResponseDetails.Response = Encoding.UTF8.GetString(responseBytes);
                        }
                    }
                }
            }
            return response;
        }


    }
    /// <summary>
    /// Class to read http response from server
    /// </summary>
    public class HttpResponseDetails
    {
        public string Response { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public string StatusDesciption { get; set; }

        public const string CurrentChunkUpdatedSuccessfully = "File updated successfully";

        public const string TotalChunksUpdatedSuccessfully = "File upload completed successfully";

        public const string FileAlreadExists = "File Already Exists!";

        public const string EmptyFile = "File Is Empty";

        public WebExceptionStatus WebExceptionStatus = WebExceptionStatus.Success;
    }
}
