using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace animeload
{
    public static class MusicHelper
    {

        public static string GetMusicJson(int goid = 0)
        {
            string url = "http://fm.bilibilijj.com/DouGa/Music";
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Timeout = 8000;
            httpRequest.Method = "POST";
            httpRequest.KeepAlive = true;
            httpRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            httpRequest.Headers.Add("Origin", "http://fm.bilibilijj.com");
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Referer = "http://fm.bilibilijj.com/";
            httpRequest.CookieContainer = new CookieContainer();
            httpRequest.CookieContainer.Add(new Uri("http://fm.bilibilijj.com"), new Cookie("safedog-flow-item", "BAED8738A7AC62B429336A746F459873"));
            httpRequest.CookieContainer.Add(new Uri("http://fm.bilibilijj.com"), new Cookie("CNZZDATA5767580", "cnzz_eid%3D840110351-1469095732-http%253A%252F%252Fwww.bilibilijj.com%252F%26ntime%3D1469117850"));
            httpRequest.AutomaticDecompression = DecompressionMethods.GZip;
            //safedog-flow-item=BAED8738A7AC62B429336A746F459873; CNZZDATA5767580=cnzz_eid%3D840110351-1469095732-http%253A%252F%252Fwww.bilibilijj.com%252F%26ntime%3D1469101274
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36";

            string Data = "id={0}&t=0&goid={1}&Playtype=0&c=0";
            if (goid == 0) { Data = string.Format(Data, 0, 0); }
            else
            {
                Random rd = new Random();
                int id = rd.Next(1000, 9999);
                Data = string.Format(Data, id, goid);
            }
            Encoding u8 = Encoding.UTF8;
            byte[] data = u8.GetBytes(Data);
            httpRequest.ContentLength = data.Length;
            Stream newStream = httpRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
            string result = sr.ReadToEnd();
            sr.Close();
            return result;
        }

        public static string GetNewMusicListJson(int page = 1)
        {
            string url = "http://fm.bilibilijj.com/DouGa/GetNewMusicList?Page={0}&t={1}";
            Random rd = new Random();
            url = string.Format(url, page, rd.NextDouble());
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Timeout = 8000;
            httpRequest.Method = "GET";
            httpRequest.KeepAlive = true;
            httpRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            httpRequest.Headers.Add("Origin", "http://fm.bilibilijj.com");
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Referer = "http://fm.bilibilijj.com/";
            httpRequest.CookieContainer = new CookieContainer();
            httpRequest.CookieContainer.Add(new Uri("http://fm.bilibilijj.com"), new Cookie("safedog-flow-item", "BAED8738A7AC62B429336A746F459873"));
            httpRequest.CookieContainer.Add(new Uri("http://fm.bilibilijj.com"), new Cookie("CNZZDATA5767580", "cnzz_eid%3D840110351-1469095732-http%253A%252F%252Fwww.bilibilijj.com%252F%26ntime%3D1469158552"));
            httpRequest.AutomaticDecompression = DecompressionMethods.GZip;
            //safedog-flow-item=BAED8738A7AC62B429336A746F459873; CNZZDATA5767580=cnzz_eid%3D840110351-1469095732-http%253A%252F%252Fwww.bilibilijj.com%252F%26ntime%3D1469101274
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36";

            //string d = Uri.EscapeDataString(queryStr);
            //double t = new Random().NextDouble();
            //string stringData = string.Format("Data={0}&t={1}", d, t);
            //Encoding u8 = Encoding.UTF8;
            //byte[] data = u8.GetBytes(stringData);
            //httpRequest.ContentLength = data.Length;
            //Stream newStream = httpRequest.GetRequestStream();
            //newStream.Write(data, 0, data.Length);
            //newStream.Close();
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
            string result = sr.ReadToEnd();
            sr.Close();
            return result;
        }

        public static string SearchMusicJson(string queryStr)
        {
            string url = "http://fm.bilibilijj.com/DouGa/MusicSearch";
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Timeout = 8000;
            httpRequest.Method = "POST";
            httpRequest.KeepAlive = true;
            httpRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            httpRequest.Headers.Add("Origin", "http://fm.bilibilijj.com");
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Referer = "http://fm.bilibilijj.com/";
            httpRequest.CookieContainer = new CookieContainer();
            httpRequest.CookieContainer.Add(new Uri("http://fm.bilibilijj.com"), new Cookie("safedog-flow-item", "BAED8738A7AC62B429336A746F459873"));
            httpRequest.CookieContainer.Add(new Uri("http://fm.bilibilijj.com"), new Cookie("CNZZDATA5767580", "cnzz_eid%3D840110351-1469095732-http%253A%252F%252Fwww.bilibilijj.com%252F%26ntime%3D1469158552"));
            httpRequest.AutomaticDecompression = DecompressionMethods.GZip;
            //safedog-flow-item=BAED8738A7AC62B429336A746F459873; CNZZDATA5767580=cnzz_eid%3D840110351-1469095732-http%253A%252F%252Fwww.bilibilijj.com%252F%26ntime%3D1469101274
            httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36";

            string d = Uri.EscapeDataString(queryStr);
            double t = new Random().NextDouble();
            string stringData = string.Format("Data={0}&t={1}", d, t);
            Encoding u8 = Encoding.UTF8;
            byte[] data = u8.GetBytes(stringData);
            httpRequest.ContentLength = data.Length;
            Stream newStream = httpRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
            string result = sr.ReadToEnd();
            sr.Close();
            return result;
        }

        public static void SearchMusic(string folderPath)
        {
            FileDownloader loader = new FileDownloader();
            WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
            WMPLib.IWMPPlaylist currentList = null;
            string queryStr = "";
            while (string.IsNullOrWhiteSpace(queryStr))
            {
                Console.Write("请输入查询内容：");
                queryStr = Console.ReadLine();
            }
            if (queryStr.ToUpper() == "E") return;
            string result = SearchMusicJson(queryStr);
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            Object[] musicList = null;
            try
            {
                musicList = (Object[])Jss.DeserializeObject(result);
            }
            catch
            {
                Console.WriteLine("服务器大概是累了，还是等几个小时，顺便使用http://fm.bilibilijj.com/在线听歌吧");
                Console.WriteLine(result);
                return;
            }
            try
            {
                if (musicList == null || musicList.Length <= 0) return;
                foreach (var item in musicList)
                {
                    Dictionary<string, object> music = item as Dictionary<string, object>;

                    string title = music["Title"].ToString();
                    string AnimeName = music["AnimeName"].ToString();
                    string highurl = music["A320"].ToString();
                    if (string.IsNullOrWhiteSpace(highurl))
                    {
                        highurl = music["A128"].ToString();
                    }
                    if (string.IsNullOrWhiteSpace(highurl))
                    {
                        continue;
                    }
                    string MusicID = music["MusicID"].ToString();
                    title = title.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                                .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                    AnimeName = AnimeName.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                                .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                    string filePath = folderPath + AnimeName + "-" + title + ".mp3";
                    Console.WriteLine("查找到音乐:" + title + "-" + AnimeName);
                    Console.WriteLine("下载地址：" + highurl);
                    if (highurl.Contains("pan.baidu.com"))
                    {
                        Console.WriteLine("只支持手动下载");
                        continue;
                    }

                    Console.WriteLine("开始下载:" + AnimeName + "-" + title);
                    if (!System.IO.File.Exists(filePath))
                    {
                        loader.DownloadFile(highurl, filePath).Wait();
                        Console.WriteLine();
                    }
                    Console.WriteLine(AnimeName + "-" + title + "下载完毕");
                    Console.WriteLine("是否播放(Y*/N/A):");
                    string playStr = Console.ReadLine().Trim().ToUpper();
                    if (playStr.Trim().ToUpper() != "N")
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            if (currentList == null)
                            {
                                if (player.playlistCollection.getByName("Search").count > 0)
                                {
                                    currentList = player.playlistCollection.getByName("Search").Item(0);
                                }
                                else
                                {
                                    currentList = player.playlistCollection.newPlaylist("Search");
                                }
                                WMPLib.IWMPMedia currentMedia = player.newMedia(filePath);
                                currentList.appendItem(currentMedia);
                                player.currentPlaylist = currentList;
                                player.controls.playItem(currentMedia);
                            }
                            else
                            {
                                WMPLib.IWMPMedia currentMedia = player.newMedia(filePath);
                                currentList.appendItem(currentMedia);
                                if (playStr != "A")
                                {
                                    player.currentPlaylist = currentList;
                                    player.controls.playItem(currentMedia);
                                }
                            }
                        }
                    }
                }

            }
            catch
            {
            }
            if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                Console.WriteLine("停止播放？");
                string stop = Console.ReadLine();
                if (stop.ToUpper() != "N")
                {
                    player.controls.stop();
                    player.close();
                }
            }
        }

        public static string downLoadNext(string folderPath, FileDownloader loader)
        {
            string result = GetMusicJson();
            string filePath = "";
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            Dictionary<string, object> mic = null;
            try
            {
                mic = (Dictionary<string, object>)Jss.DeserializeObject(result);
            }
            catch
            {
                Console.WriteLine("服务器大概是累了，还是等几个小时，顺便使用http://fm.bilibilijj.com/在线听歌吧");
                return "N";
                result.Substring(result.IndexOf("http"), result.IndexOf("Music") - result.IndexOf("http"));
            }
            try
            {
                string title = mic["title"].ToString();
                string highurl = mic["highurl"].ToString();
                string name = mic["name"].ToString();
                string bg = mic["bg"].ToString();
                //http://www.jjdouga.com//Logo/2648/bg.jpg
                string mcName = name.Substring(0, name.IndexOf("<span"));
                Console.WriteLine("开始下载" + title + "-" + mcName);
                title = title.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                    .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                mcName = mcName.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                    .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                string imgPath = folderPath + "bg\\" + title + ".jpg";
                if (!Directory.Exists(folderPath + "bg\\"))
                {
                    Directory.CreateDirectory(folderPath + "bg\\");
                }
                string imageUrl = "http://www.jjdouga.com/" + bg;
                Wallpaper.Set(new Uri(imageUrl));
                if (!File.Exists(imgPath))
                {
                    loader.DownloadFile(imageUrl, imgPath).Wait();
                    Console.WriteLine();
                }
                filePath = folderPath + title + "-" + mcName + ".mp3";
                if (File.Exists(filePath))
                {
                    Console.WriteLine("文件已存在");
                    return filePath;
                }
                loader.DownloadFile(highurl, filePath).Wait();
                Console.WriteLine();
                Console.WriteLine(title + "-" + mcName + "下载完毕");
                return filePath;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("下载出错：" + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
                return "N";
            }
        }

        [Obsolete("please use FileDownloader", false)]
        public static void DownLoad(string url, string filePath)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 15000;
                req.Method = "GET";
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                if (UserSetting.project != null && resp.ContentLength < UserSetting.project.minFileSize)
                {
                    Console.WriteLine("小图");
                }
                else
                    using (Stream qrcodeWebStream = resp.GetResponseStream())
                    {
                        //拷贝
                        using (var filestream = File.Create(filePath))
                        {
                            qrcodeWebStream.CopyTo(filestream);
                        }

                    }
            }
            catch (WebException)
            {
                TextFileHelper.WriteFailedUrl(url, Web.LinkFinder.ProjectFolder + "\\failed.txt");
            }
        }
    }
}
