using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace animaload
{
    class Program
    {
        static void Main(string[] args)
        {
            bool again = false;
            bool musicPlay = true;
            bool askError = true;
            bool playComplete = true;
            string folderPath = "F:\\AnimeMusic\\";
            WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
            WMPLib.IWMPPlaylist currentList = null;
            if (player.playlistCollection.getByName("anima list").count > 0)
            {
                currentList = player.playlistCollection.getByName("anima list").Item(0);
            }
            else
            {
                currentList = player.playlistCollection.newPlaylist("anima list");
            }
            currentList.clear();
            if (Directory.Exists(folderPath))
            {
                foreach (var item in Directory.EnumerateFiles(folderPath, "*.mp3"))
                {
                    WMPLib.IWMPMedia currentMedia = player.newMedia(item);
                    currentList.appendItem(currentMedia);
                }
            }

            while (!again)
            {
                string tempFolder = folderPath;
                Console.WriteLine("Q/EXIT/STOP:退出|JP:Lazy的下载模式");
                Console.Write("请输入存储目录，或者直接回车使用默认‘" + folderPath + "’目录：");
                folderPath = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(folderPath))
                {
                    folderPath = tempFolder;
                }
                if (folderPath.ToUpper() == "Q" || folderPath.ToUpper() == "EXIT" || folderPath.ToUpper() == "STOP") break;
                if (folderPath.ToUpper() == "JP")
                {
                    JustPlay();
                    break;
                }
                if (folderPath.LastOrDefault() != '\\')
                {
                    folderPath = folderPath + "\\";
                }
                if (!Directory.Exists(folderPath))
                {
                    try
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("出现异常：" + ex.Message);
                        Console.ReadLine();
                        return;
                    }
                }
                int isize = 0;
                bool hasSize = false;
                while (!hasSize)
                {
                    Console.WriteLine("S/SEARCH:进入搜索|SET/SETTING:设置更多参数");
                    Console.Write("需要下载数量：");
                    string size = Console.ReadLine();
                    if (size.ToUpper() == "S" || size.ToUpper() == "SEARCH")
                    {
                        SearchMusic(folderPath);
                        continue;
                    }
                    if (size.ToUpper() == "SET" || size.ToUpper() == "SETTING")
                    {
                        Console.Write("是否直接播放歌曲（Y*/N）：");
                        string playmusic = Console.ReadLine();
                        if (playmusic.Trim().ToUpper() == "N")
                        {
                            musicPlay = false;
                        }
                        else if (playmusic.Trim().ToUpper() == "Y")
                        {
                            musicPlay = true;
                        }
                        if (musicPlay)
                        {
                            Console.WriteLine("当前为直接播放音乐");
                            Console.WriteLine("是否为试听模式（Y/N）:");
                            string st = Console.ReadLine();
                            if (st.Trim().ToUpper() == "N")
                            {
                                playComplete = true;
                            }
                            else if (st.Trim().ToUpper() == "Y")
                            {
                                playComplete = false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("当前不播放音乐");
                            if (player.currentPlaylist == currentList && player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                            {
                                player.controls.stop();
                            }
                        }
                        Console.Write("是否异常中断（Y*/N）：");
                        string errorInter = Console.ReadLine();
                        if (errorInter.Trim().ToUpper() == "N")
                        {
                            askError = false;
                        }
                        else if (playmusic.Trim().ToUpper() == "Y")
                        {
                            askError = true;
                        }
                    }
                    try
                    {
                        isize = Convert.ToInt32(size);
                        if (isize > 0)
                        {
                            hasSize = true;
                        }
                    }
                    catch
                    {

                    }
                }
                if (musicPlay && currentList.count > 0) { player.currentPlaylist = currentList; player.controls.play(); }
                int count = 0;
                while (count < isize)
                {
                    string result = GetMusicJson();
                    //Response.Write(result);
                    JavaScriptSerializer Jss = new JavaScriptSerializer();
                    Dictionary<string, object> mic = null;
                    try
                    {
                        mic = (Dictionary<string, object>)Jss.DeserializeObject(result);
                    }
                    catch
                    {
                        Console.WriteLine("服务器大概是累了，还是等几个小时，顺便使用http://fm.bilibilijj.com/在线听歌吧");
                        break;
                        result.Substring(result.IndexOf("http"), result.IndexOf("Music") - result.IndexOf("http"));
                    }
                    try
                    {
                        string title = mic["title"].ToString();
                        string name = mic["name"].ToString();
                        //string id = mic["id"].ToString();
                        //string url1 = mic["url"].ToString();
                        string highurl = mic["highurl"].ToString();
                        //string bg = mic["bg"].ToString();
                        //string MusicID = mic["MusicID"].ToString();
                        //string Lrc = mic["Lrc"].ToString();
                        //string ZhongWenLrc = mic["ZhongWenLrc"].ToString();
                        //string LuoMaYinLrc = mic["LuoMaYinLrc"].ToString();
                        //string animeurl = mic["animeurl"].ToString();
                        //string click = mic["click"].ToString();
                        string mcName = name.Substring(0, name.IndexOf("<span"));
                        Console.WriteLine("***********第" + (count + 1).ToString() + "个*********");
                        Console.WriteLine("开始下载" + title + "-" + mcName);
                        title = title.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                            .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                        mcName = mcName.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                            .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                        string filePath = folderPath + title + "-" + mcName + ".mp3";
                        if (File.Exists(filePath))
                        {
                            Console.WriteLine("文件已存在，开始下一个");
                            continue;
                        }
                        DownLoad(highurl, filePath);
                        Console.WriteLine(title + "-" + mcName + "下载完毕");
                        count++;
                        if (musicPlay)
                            if (System.IO.File.Exists(filePath))
                            {
                                if (currentList == null)
                                {
                                    if (player.playlistCollection.getByName("anima list").count > 0)
                                    {
                                        currentList = player.playlistCollection.getByName("anima list").Item(0);
                                    }
                                    else
                                    {
                                        currentList = player.playlistCollection.newPlaylist("anima list");
                                    }
                                    WMPLib.IWMPMedia currentMedia = player.newMedia(filePath);
                                    currentList.appendItem(currentMedia);
                                    player.currentPlaylist = currentList;
                                    player.controls.playItem(currentMedia);
                                }
                                else //if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                                {
                                    WMPLib.IWMPMedia currentMedia = player.newMedia(filePath);
                                    currentList.appendItem(currentMedia);
                                    if (!playComplete || count == 1)
                                    {
                                        if (player.currentPlaylist == null || player.currentPlaylist != currentList)
                                            player.currentPlaylist = currentList;
                                        player.controls.playItem(currentMedia);
                                    }
                                }
                            }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("出现异常：" + ex.Message);
                        if (!askError) continue;
                        Console.Write("是否继续（Y/N）:");
                        string con = Console.ReadLine();
                        if (con.Trim().ToUpper() == "Y")
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                Console.WriteLine("共下载" + count + "个文件");
                //Console.Write("是否继续（Y/N）:");
                //string ag = Console.ReadLine();
                //if (ag.Trim().ToUpper() == "Y")
                //{
                //    continue;
                //}
                //else
                //{
                //    break;
                //}
            }
            Console.WriteLine("谢谢使用");
            Console.ReadKey();
        }

        private static string GetMusicJson()
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

            string stringData = "id=0&t=0&goid=0&Playtype=0&c=0";
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

        private static string SearchMusicJson(string queryStr)
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

        private static void SearchMusic(string folderPath)
        {
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
                        DownLoad(highurl, filePath);
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

        private static void DownLoad(string highurl, string filePath)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(highurl);
            req.Timeout = 700000;
            req.Method = "GET";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            using (Stream qrcodeWebStream = resp.GetResponseStream())
            {
                //拷贝
                using (var filestream = File.Create(filePath))
                {
                    qrcodeWebStream.CopyTo(filestream);
                }

            }
        }

        private static void JustPlay()
        {
            WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
            WMPLib.IWMPMedia currentPlay = null;
            WMPLib.IWMPMedia nextPlay = null;
            WMPLib.IWMPPlaylist currentList = null;
            string folderPath = "jp\\";

            //初始化播放列表
            if (player.playlistCollection.getByName("JustPlay").count > 0)
            {
                currentList = player.playlistCollection.getByName("JustPlay").Item(0);
                currentList.clear();
            }
            else
            {
                currentList = player.playlistCollection.newPlaylist("JustPlay");
            }

            string filePath = "";
            while (true)
            {
                string downResult = downLoadNext(folderPath);
                if (downResult != "N" && File.Exists(downResult))
                {
                    filePath = downResult;
                    break;
                }
            }
            currentPlay = player.newMedia(filePath);
            player.currentPlaylist = currentList;
            currentList.appendItem(currentPlay);
            player.controls.playItem(currentPlay);

            while (true)
            {
                string downResult = downLoadNext(folderPath);
                if (downResult != "N" && File.Exists(downResult))
                {
                    filePath = downResult;
                    break;
                }
            }

            currentPlay = player.newMedia(filePath);
            currentList.appendItem(currentPlay);

            bool ignore = false;
            player.CurrentItemChange += (o) =>
            {
                if (ignore) return;
                ignore = true;

                while (true)
                {
                    string downResult = downLoadNext(folderPath);
                    if (downResult != "N")
                    {
                        filePath = downResult;
                        break;
                    }
                }
                nextPlay = player.newMedia(filePath);
                currentList.appendItem(nextPlay);
                Console.WriteLine(nextPlay.name);
                ignore = false;
            };

        }

        private static string downLoadNext(string folderPath)
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
                Console.WriteLine("开始下载" + title);
                title = title.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                    .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                filePath = folderPath + title + ".mp3";
                if (File.Exists(filePath))
                {
                    Console.WriteLine("文件已存在，开始下一个");
                    return "N";
                }
                DownLoad(highurl, filePath);
                Console.WriteLine(title + "下载完毕");
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("下载出错：" + ex.Message);
                return "N";
            }
        }
    }
}
