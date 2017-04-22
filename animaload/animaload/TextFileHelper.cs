using animeload.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace animeload
{
    public class TextFileHelper
    {
        public static void SaveToFile(List<LinkItem> store, string storeFileName, List<string> visited, string visitedFileName)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            jsonSerialiser.MaxJsonLength = 147483644;
            var json = jsonSerialiser.Serialize(store);
            System.IO.StreamWriter file = new System.IO.StreamWriter(storeFileName);
            file.WriteLine(json);
            file.Close();
            var visitedJson = jsonSerialiser.Serialize(visited);
            System.IO.StreamWriter fileVisited = new System.IO.StreamWriter(visitedFileName);
            fileVisited.WriteLine(visitedJson);
            fileVisited.Close();
        }

        public static void ReadToMemory(out List<LinkItem> store, string storeFileName, out List<string> visited, string visitedFileName)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            jsonSerialiser.MaxJsonLength = 147483644;
            string text = System.IO.File.ReadAllText(storeFileName);
            store = jsonSerialiser.Deserialize<List<LinkItem>>(text);
            string visitedText = System.IO.File.ReadAllText(visitedFileName);
            visited = jsonSerialiser.Deserialize<List<string>>(visitedText);
        }

        public static void SaveImageUrls(List<string> urls, string fileName)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            jsonSerialiser.MaxJsonLength = 147483644;
            var json = jsonSerialiser.Serialize(urls);
            System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
            file.WriteLine(json);
            file.Close();
        }

        public static void ReadImageUrls(out List<string> urls, string fileName)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            jsonSerialiser.MaxJsonLength = 147483644;
            string text = System.IO.File.ReadAllText(fileName);
            urls = jsonSerialiser.Deserialize<List<string>>(text);
        }

        public static void SaveInterestingUrls(List<string> urls, string fileName)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            jsonSerialiser.MaxJsonLength = 147483644;
            var json = jsonSerialiser.Serialize(urls);
            System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);
            file.WriteLine(json);
            file.Close();
        }

        public static void ReadInterestingUrls(out List<string> urls, string fileName)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            jsonSerialiser.MaxJsonLength = 147483644;
            string text = System.IO.File.ReadAllText(fileName);
            urls = jsonSerialiser.Deserialize<List<string>>(text);
        }

        public static void WriteFailedUrl(string url, string fileName)
        {
            if (!File.Exists(fileName))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    sw.WriteLine(url);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(fileName))
                {
                    sw.WriteLine(url);
                }
            }
        }

        public static void SaveProject(ImageLoaderProject project)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            jsonSerialiser.MaxJsonLength = 147483644;
            var json = jsonSerialiser.Serialize(project);
            System.IO.StreamWriter file = new System.IO.StreamWriter(project.projectName + "\\project.json");
            file.WriteLine(json);
            file.Close();
        }

        public static ImageLoaderProject ReadProject(string projectName)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            jsonSerialiser.MaxJsonLength = 147483644;
            try
            {
                string text = System.IO.File.ReadAllText(projectName + "\\project.json");
                return jsonSerialiser.Deserialize<ImageLoaderProject>(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static List<string> ReadIgnores()
        {
            var jsonSerialiser = new JavaScriptSerializer();
            jsonSerialiser.MaxJsonLength = 147483644;
            try
            {
                string text = System.IO.File.ReadAllText("ignore.json");
                return jsonSerialiser.Deserialize<List<string>>(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<string>();
            }
        }
        public static CookieContainer ReadCookie()
        {
            CookieContainer cookieContainer = new CookieContainer();
            string[] text = System.IO.File.ReadAllLines("cookie.txt");
            foreach (var t in text)
            {
                string ct = t.Trim();
                if (string.IsNullOrWhiteSpace(ct)) continue;
                if (ct.IndexOf("=") < 0) continue;
                string key = ct.Substring(0, ct.IndexOf("="));
                string val = ct.Substring(ct.IndexOf("=") + 1);
                cookieContainer.Add(new Uri("http://qq.com"), new Cookie(key, val));
            }
            return cookieContainer;
        }
        public static void writeResponse(string res)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("response.htm");
            file.WriteLine(res);
            file.Close();
        }
    }
}
