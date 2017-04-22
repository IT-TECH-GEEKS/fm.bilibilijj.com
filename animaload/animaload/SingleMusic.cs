using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace animeload
{
    public class SingleMusic
    {
        public string animeurl { set; get; }
        public string bg { set; get; }
        public string click { set; get; }
        public string highurl { set; get; }
        public string id { set; get; }
        public string Lrc { set; get; }
        public string LuoMaYinLrc { set; get; }
        public string MusicID { set; get; }
        public string name { set; get; }
        public string sleep { set; get; }
        public string title { set; get; }
        public string top { set; get; }
        public string url { set; get; }
        public string ZhongWenLrc { set; get; }
        public string FullName
        {
            get
            {
                return title + "-" + mcName;
            }
        }
        public string mcName
        {
            get
            {
                if (name == null) return "";
                try
                {
                    return name.Substring(0, name.IndexOf("span ")).Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                        .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                }
                catch
                {
                    return name.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                        .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                }
            }
        }
        private const string bgBaseUrl = "http://www.jjdouga.com/";
        public string bgUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(bg))
                {
                    return null;
                }
                if (bg.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) return bg;
                return bgBaseUrl + bg;
            }
        }
        public SingleMusic()
        {
            string result = MusicHelper.GetMusicJson();
            FromJson(result);
        }

        public SingleMusic(int goid)
        {
            string result = MusicHelper.GetMusicJson(goid);
            FromJson(result);
        }

        public void FromJson(string MusicJson)
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            Dictionary<string, object> mic = null;
            try
            {
                mic = (Dictionary<string, object>)Jss.DeserializeObject(MusicJson);
            }
            catch
            {
                throw;
            }
            try
            {
                title = mic["title"].ToString().Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                    .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                name = mic["name"].ToString().Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                    .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                id = mic["id"].ToString();
                url = mic["url"].ToString();
                highurl = mic["highurl"].ToString();
                bg = mic["bg"].ToString();
                MusicID = mic["MusicID"].ToString();
                Lrc = mic["Lrc"].ToString();
                ZhongWenLrc = mic["ZhongWenLrc"].ToString();
                LuoMaYinLrc = mic["LuoMaYinLrc"].ToString();
                animeurl = mic["animeurl"].ToString();
                click = mic["click"].ToString();
                top = mic["top"].ToString();
                sleep = mic["sleep"].ToString();
            }
            catch
            {
                throw;
            }
        }

        public string Download(string folderPath, out bool isNewLoad, bool photoOnly = false, bool setWallpaper = true)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                isNewLoad = false;
                return "";
            }
            FileDownloader loader = new FileDownloader();
            Console.WriteLine("*********" + title + "-" + mcName + "**********");
            //Console.WriteLine(highurl);
            //Console.WriteLine(bgUrl);
            string filePath = folderPath + FullName + ".mp3";
            string imgPath = folderPath + "bg\\" + title + ".jpg";
            if (!Directory.Exists(folderPath + "bg\\"))
            {
                Directory.CreateDirectory(folderPath + "bg\\");
            }
            if (!string.IsNullOrWhiteSpace(bgUrl))
            {
                if (setWallpaper)
                    Wallpaper.Set(new Uri(bgUrl));
                if (!File.Exists(imgPath))
                {
                    loader.DownloadFile(bgUrl, imgPath).Wait();
                    Console.WriteLine();

                    if (photoOnly)
                    {
                        isNewLoad = true;
                        return imgPath;
                    }
                }
                if (photoOnly)
                {
                    isNewLoad = false;
                    return imgPath;
                }
            }
            else
            {
                if (photoOnly)
                {
                    isNewLoad = false;
                    return "";
                }
            }
            if (File.Exists(filePath))
            {
                Console.WriteLine("文件已存在");
                isNewLoad = false;
            }
            else
            {
                Console.WriteLine("开始下载" + FullName);
                loader.DownloadFile(highurl, filePath).Wait();
                Console.WriteLine();
                Console.WriteLine(title + "-" + mcName + "下载完毕");
                isNewLoad = true;
            }
            return filePath;
        }
    }
}
