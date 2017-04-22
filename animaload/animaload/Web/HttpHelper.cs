using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace animeload.Web
{
    public class HttpHelper
    {
        public static string InvokeHttpGet(string url, bool keepSign)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Timeout = 2000;
            httpRequest.Method = "GET";
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36";
            httpRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
            string result = sr.ReadToEnd();
            if (!keepSign)
            {
                result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            }
            sr.Close();
            return result;
        }

        public static string InvokeHttpPost(string url, string postValue, bool keepSign)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;

            byte[] data = encoding.GetBytes(postValue);

            try
            {
                // 设置参数
                request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string result = sr.ReadToEnd();
                if (!keepSign)
                {
                    result = result.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                }
                sr.Close();
                return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string InvokeHttpPostFile(string url, Stream fileStream, string fileName, string contentType)
        {
            //LogAgent.Log("InvokeHttpPostFile", "Invoke Started");
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
            Stream memStream = new System.IO.MemoryStream();
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition:  form-data; name=\"{0}\";\r\n\r\n{1}";
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: {2}\r\n\r\n";
            memStream.Write(boundarybytes, 0, boundarybytes.Length);

            string header = string.Format(headerTemplate, "file", fileName, contentType);
            //string header = string.Format(headerTemplate, "uplTheFile", files[i]);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            memStream.Write(headerbytes, 0, headerbytes.Length);
            //FileStream fileStream = new FileStream(files[i], FileMode.Open,
            //FileAccess.Read);
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, bytesRead);
            }
            //LogAgent.Log("InvokeHttpPostFile", "FileStream has Written To Request");

            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            fileStream.Close();

            httpWebRequest.ContentLength = memStream.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            //LogAgent.Log("InvokeHttpPostFile", "GetRequestStream");
            memStream.Position = 0;
            byte[] tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            //LogAgent.Log("InvokeHttpPostFile", "Written Request Stream.");
            //requestStream.Close();
            try
            {
                //LogAgent.Log("InvokeHttpPostFile", "Getting Response.");
                WebResponse webResponse = httpWebRequest.GetResponse();
                //LogAgent.Log("InvokeHttpPostFile", " Response Gotten.");
                Stream stream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string var = reader.ReadToEnd();
                requestStream.Close();
                return var;
            }
            catch (Exception ex)
            {
                requestStream.Close();
                return ex.ToString();
            }
        }
    }
}