using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace animeload.Web
{
    using System.Collections.Generic;
    using System.Net;
    using System.Text.RegularExpressions;

    public class LinkItem
    {
        public string Href;
        public string Text;
        public int depth = 0;
        public string parentUrl;
    }

    static class LinkFinder
    {
        private static WebClient w = new WebClient() { Encoding = Encoding.UTF8 };
        public static int MaxDepth = 3;
        public static string ProjectFolder = "";
        public static List<LinkItem> Find(string url, int depth)
        {
            Console.WriteLine("depth:{0}", depth);
            List<LinkItem> list = new List<LinkItem>();

            string file;
            try
            {
                file = HttpHelper.InvokeHttpGet(url, true);
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Time out!");
                return list;
            }
            catch (WebException)
            {
                Console.WriteLine("Time out!");
                TextFileHelper.WriteFailedUrl(url, ProjectFolder + "\\failed.txt");
                return list;
            }
            catch (NotSupportedException)
            {
                Console.WriteLine("Url invalid!");
                TextFileHelper.WriteFailedUrl(url, ProjectFolder + "\\failed.txt");
                return list;
            }

            // 1.
            // Find all matches in file.
            MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)",
                RegexOptions.Singleline);

            MatchCollection mx = Regex.Matches(file, "<img.+?src=[\"'](.+?)[\"'].+?>",
                RegexOptions.Singleline);

            // 2.
            // Loop over each match.
            //如果深度过了，就只访问图片
            if (depth < MaxDepth)
                foreach (Match m in m1)
                {
                    string value = m.Groups[1].Value;
                    LinkItem i = new LinkItem();

                    // 3.
                    // Get href attribute.
                    Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                    RegexOptions.Singleline);
                    if (m2.Success)
                    {
                        i.Href = m2.Groups[1].Value;
                    }

                    // 4.
                    // Remove inner tags from text.
                    string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                    RegexOptions.Singleline);
                    i.Text = t;
                    i.depth = depth;
                    i.parentUrl = url;

                    if (i.Href == "/") continue;
                    if (i.Href == null) continue;
                    foreach (var item in UserSetting.IgnoreDomain)
                    {
                        if (i.Href.Contains(item)) continue;
                    }

                    list.Add(i);
                }

            foreach (Match m in mx)
            {
                string value = m.Groups[1].Value;
                LinkItem i = new LinkItem();

                // 3.
                // Get href attribute.
                i.Href = value;

                // 4.
                // Remove inner tags from text.
                //string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                //RegexOptions.Singleline);
                //i.Text = t;
                i.depth = depth;
                i.parentUrl = url;

                if (i.Href == "/") continue;
                if (i.Href == null) continue;
                foreach (var item in UserSetting.IgnoreDomain)
                {
                    if (i.Href.Contains(item)) continue;
                }

                list.Add(i);
            }
            return list;
        }
    }
}
