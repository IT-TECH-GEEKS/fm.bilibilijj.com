﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace animeload
{
    public class FileDownloader
    {
        WebClient webClient;
        Stopwatch sw = new Stopwatch();

        public FileDownloader()
        {

        }

        public async Task DownloadFile(string urlAddress, string location)
        {
            using (webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                // The variable that will be holding the url address (making sure it starts with http://)
                Uri URL = urlAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? new Uri(urlAddress) : new Uri("http://" + urlAddress);

                // Start the stopwatch which we will be using to calculate the download speed
                sw.Start();

                try
                {
                    // Start downloading the file
                    await webClient.DownloadFileTaskAsync(URL, location);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;

                }
            }
        }

        // The event that will fire whenever the progress of the WebClient is changed
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Calculate download speed and output it to labelSpeed.
            //labelSpeed.Text = string.Format("{0} kb/s", (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));

            // Update the progressbar percentage only when the value is not the same.
            //progressBar.Value = e.ProgressPercentage;

            // Show the percentage on our label.
            //labelPerc.Text = e.ProgressPercentage.ToString() + "%";

            // Update the label with how much data have been downloaded so far and the total size of the file we are currently downloading
            //labelDownloaded.Text = string.Format("{0} MB's / {1} MB's",
            //    (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
            //    (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));
            Console.Write("\r{0}% downloaded at speed {1} kb/s", e.ProgressPercentage, (e.BytesReceived / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"));
        }

        // The event that will trigger when the WebClient is completed
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            // Reset the stopwatch.
            sw.Reset();

            if (e.Cancelled == true)
            {
                Console.WriteLine("\rDownload Cancelled!");
            }
            else
            {
                Console.WriteLine("\rDownload completed!");
            }
        }
    }
}
