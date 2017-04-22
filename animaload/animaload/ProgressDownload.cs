using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace animeload
{
    public class ProgressDownload
    {
        HttpWebRequest _request;
        IAsyncResult _responseAsyncResult;
        string _requestUrl;
        string _saveFilePath;
        public ProgressDownload(string requestUrl, string saveFilePath)
        {
            this._requestUrl = requestUrl;
            this._saveFilePath = saveFilePath;
            GetWebContent();
        }

        private void GetWebContent()
        {
            _request = WebRequest.Create(_requestUrl) as HttpWebRequest;
            _responseAsyncResult = _request.BeginGetResponse(ResponseCallback, null);
        }

        private void ResponseCallback(object state)
        {
            var response = _request.EndGetResponse(_responseAsyncResult) as HttpWebResponse;
            long contentLength = response.ContentLength;
            if (contentLength == -1)
            {
                // You'll have to figure this one out.
            }
            Stream responseStream = response.GetResponseStream();
            GetContentWithProgressReporting(responseStream, contentLength);
            response.Close();
        }

        private byte[] GetContentWithProgressReporting(Stream responseStream, long contentLength)
        {
            UpdateProgressBar(0);

            // Allocate space for the content
            var data = new byte[contentLength];
            int currentIndex = 0;
            int bytesReceived = 0;
            var buffer = new byte[256];
            do
            {
                bytesReceived = responseStream.Read(buffer, 0, 256);
                Array.Copy(buffer, 0, data, currentIndex, bytesReceived);
                currentIndex += bytesReceived;

                // Report percentage
                double percentage = (double)currentIndex / contentLength;
                UpdateProgressBar((int)(percentage * 100));
            } while (bytesReceived < contentLength);

            UpdateProgressBar(100);
            return data;
        }

        private void UpdateProgressBar(int percentage)
        {
            // If on a worker thread, marshal the call to the UI thread
            //if (progressBar1.InvokeRequired)
            //{
            //    progressBar1.Invoke(new Action<int>(UpdateProgressBar), percentage);
            //}
            //else
            //{
            //    progressBar1.Value = percentage;
            //}
        }
    }
}
