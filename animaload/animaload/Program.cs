using animeload;
using animeload.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace animaload
{
    class Program
    {

        #region 系统引用
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            int uFlags);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);
        #endregion

        #region 窗口变换
        private static void Maximize()
        {
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3
        }
        private static void Minimize()
        {
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 6);
        }
        private static void Restore()
        {
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 9);
        }
        #endregion

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        //[STAThread]//访问剪切板必须将此Attribute给Main函数
        static void Main(string[] args)
        {
            stayTop();
            //FastInput();
            TestFiled(false);

            uint LWA_ALPHA = 0x2;
            IntPtr Handle = GetConsoleWindow();
            SetLayeredWindowAttributes(Handle, 0, 128, LWA_ALPHA);

            Console.Title = "ANIME MUSIC LOADER";
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            bool exit = false;
            bool musicPlay = true;
            bool askError = true;
            bool playComplete = true;
            bool crazyPic = false;
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

            while (!exit)
            {
                string tempFolder = folderPath;
                Console.WriteLine("Q/EXIT/STOP:退出");
                Console.Write("请输入存储目录，或者直接回车使用默认‘" + folderPath + "’目录：");
                folderPath = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(folderPath))
                {
                    folderPath = tempFolder;
                }
                if (folderPath.ToUpper() == "Q" || folderPath.ToUpper() == "EXIT" || folderPath.ToUpper() == "STOP") break;
                if (folderPath.ToUpper() == "TOP")
                {
                    stayTop();
                    folderPath = tempFolder;
                    continue;
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
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("出现异常：" + ex.Message);
                        Console.ForegroundColor = ConsoleColor.White;

                        Console.ReadLine();
                        return;
                    }
                }
                int isize = 0;
                bool hasSize = false;
                while (!hasSize)
                {
                    Console.WriteLine("S/SEARCH:进入搜索|JP:Lazy的下载模式|DG:点歌|SET/SETTING:设置更多参数");
                    if (crazyPic) Console.WriteLine("当前为残暴图片模式");
                    Console.Write("进入其他模式或者输入数量直接开始下载：");
                    string size = Console.ReadLine().Trim().ToUpper();
                    if (size.ToUpper() == "S" || size.ToUpper() == "SEARCH")
                    {
                        SearchMusic(folderPath, player);
                        continue;
                    }
                    if (size == "DG")
                    {
                        DianGe(folderPath, player);
                        continue;
                    }
                    if (size == "STOP")
                    {
                        if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                        {
                            player.controls.stop();
                        }
                        continue;
                    }
                    if (size == "TOP")
                    {
                        stayTop();
                        continue;
                    }
                    if (size == "PLAY")
                    {
                        if (player.playState != WMPLib.WMPPlayState.wmppsPlaying)
                        {
                            RandomPlay(player, currentList);
                        }
                        continue;
                    }
                    if (size == "NEXT")
                    {
                        RandomPlay(player, currentList);
                        continue;
                    }
                    if (size.ToUpper() == "JP")
                    {
                        JustPlay(folderPath, player);
                        continue;
                    }
                    if (size.ToUpper() == "SET" || size.ToUpper() == "SETTING")
                    {
                        Console.WriteLine("残暴图片模式？请郑重输入大写'Y'：");
                        string picOnly = Console.ReadLine();
                        if (picOnly == "Y")
                        {
                            crazyPic = true;
                            continue;
                        }
                        else
                        {
                            crazyPic = false;
                        }

                        Console.Write("是否直接播放歌曲(Y*/N)：");
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
                            Console.WriteLine("是否为试听模式(Y/N):");
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
                        Console.Write("是否异常中断(Y*/N)：");
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
                        if (isize >= 0)
                        {
                            hasSize = true;
                        }
                    }
                    catch
                    {

                    }
                }
                if (musicPlay && currentList.count > 0 && !crazyPic && player.playState != WMPLib.WMPPlayState.wmppsPlaying) { RandomPlay(player, currentList); }
                int count = 0;
                while (count < isize)
                {
                    SingleMusic music = null;
                    try
                    {
                        music = new SingleMusic();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取新歌曲出现异常：" + ex.Message);
                        Console.ForegroundColor = ConsoleColor.White;

                        Console.Write("是否重试(Y*/N/W(for brower)):");
                        string keyWord = Console.ReadLine().Trim().ToUpper();
                        if (keyWord == "N") break;
                        if (keyWord == "W") Process.Start("http://fm.bilibilijj.com/");
                        continue;
                    }

                    Console.WriteLine("****************第" + (count + 1).ToString() + "个****************");

                    try
                    {
                        bool newLoad;
                        string filePath = "";
                        if (crazyPic)
                        {
                            filePath = music.Download(folderPath, out newLoad, true);
                        }
                        else
                        {
                            filePath = music.Download(folderPath, out newLoad);
                        }
                        if (newLoad)
                            count++;
                        if (musicPlay && !crazyPic && !string.IsNullOrWhiteSpace(filePath))
                        {
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
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("出现异常：" + ex.Message);
                        Console.ForegroundColor = ConsoleColor.White;

                        if (!askError) continue;
                        Console.Write("是否继续(Y/N):");
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
            }
            Console.WriteLine("谢谢使用");
            Console.ReadKey();
        }

        private static void FastInput()
        {
            List<AnimeMusic> amList = new List<AnimeMusic>();
            while (true)
            {
                AnimeMusic am = new AnimeMusic();
                string wd = "";
                Console.Write("MusicName:");
                wd = Console.ReadLine();
                if (wd == "over") break;
                if (string.IsNullOrEmpty(wd))
                {
                    wd = Clipboard.GetText();
                }
                am.MusicName = wd;
                Console.Write("AnimeName:");
                wd = Console.ReadLine();
                if (string.IsNullOrEmpty(wd))
                {
                    wd = Clipboard.GetText();
                }
                am.AnimeName = wd;
                Console.Write("TypeFlag:");
                wd = Console.ReadLine();
                if (string.IsNullOrEmpty(wd))
                {
                    wd = Clipboard.GetText();
                }
                am.TypeFlag = wd;
                Console.Write("DownloadUrl:");
                wd = Console.ReadLine();
                if (string.IsNullOrEmpty(wd))
                {
                    wd = Clipboard.GetText();
                }
                am.DownloadUrl = wd;
                amList.Add(am);
            }
            foreach (var item in amList)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", item.AnimeName, item.MusicName, item.TypeFlag, item.DownloadUrl);
            }
            Console.ReadLine();
        }

        #region 窗口保持置顶
        /// <summary>
        /// 保持置顶
        /// </summary>
        public static void stayTop()
        {
            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

            SetWindowPos(hWnd,
                new IntPtr(HWND_TOPMOST),
                0, 0, 0, 0,
                SWP_NOMOVE | SWP_NOSIZE);
            Console.WindowHeight = 3;
        }
        #endregion

        private static void PlayWarnVoice(int times = 1)
        {
            WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();
            WMPLib.IWMPPlaylist currentList = null;
            currentList = player.currentPlaylist;
            WMPLib.IWMPMedia media = player.newMedia("invite.mp3");
            currentList.appendItem(media);
            player.currentPlaylist = currentList;
            while (--times > 0)
            {
                Thread.Sleep(5000);
                player.controls.playItem(media);
            }
        }

        private static void TestFiled(bool act)
        {
            if (!act) return;

            string url = "https://ssl.ptlogin2.qq.com/jump?pt_clientver=5425&pt_src=1&keyindex=9&ptlang=2052&clientuin=429169540&clientkey=00015794B5EB0068B73DC2237404EBD40B5145B04E4B69D4B72D9C696B1FFB7A3BF854DDC701BA7D9990F939883B5082F559961AF31D99D49CE87768BD413996DD438DA574BF1C3FAF62E76EB84C463467A81151510C4428F09D743D748D3A6BD44D8AB31A3554B19D13178E6A56A560&u1=http%3A%2F%2Fuser.qzone.qq.com%2F429169540%2Finfocenter";
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Timeout = 8000;
            httpRequest.Method = "GET";
            httpRequest.KeepAlive = true;
            httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.CookieContainer = TextFileHelper.ReadCookie();

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
            TextFileHelper.writeResponse(result);
            Console.WriteLine(result);

            Console.ReadLine();

            bool reTry = true;
            while (reTry)
            {
                try
                {
                    HttpHelper.InvokeHttpGet("http://www.baidu.com", false);
                    break;
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("重试？");
                    PlayWarnVoice();
                    Console.ReadLine();
                    continue;
                }
            }

            #region old code
            //Console.WriteLine("Load...");
            //string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.jpg");
            //WebsiteToImage websiteToImage = new WebsiteToImage("http://w.ihx.cc/", tempPath);
            //websiteToImage.Generate();
            //Console.WriteLine("Set...");
            //Wallpaper.Set(tempPath);
            //Console.WriteLine("Seted!");
            //Console.WriteLine(MusicHelper.GetNewMusicListJson());
            #endregion
            string baseUrl = "";
            string projectName = "";
            int depth = 1;
            int waiting = 2000;
            int minFileSize = 50000;
            UserSetting.IgnoreDomain = TextFileHelper.ReadIgnores();
            ImageLoaderProject pro = new ImageLoaderProject();
            Console.Write("请输入项目名（只包含数字和字母）：");
            string pjInput = Console.ReadLine();
            projectName = pjInput.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
            Console.WriteLine("项目名将为：{0}", projectName);

            if (!Directory.Exists(projectName))
            {
                pro.projectName = projectName;
                Directory.CreateDirectory(projectName);
                Console.WriteLine("没找到历史记录，新建项目");
                Console.Write("输入项目初始网址：");
                baseUrl = Console.ReadLine();
                baseUrl = baseUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? baseUrl : "http://" + baseUrl;
                pro.baseUrl = baseUrl;

                Console.Write("请输入查找层数：");

                while (!Int32.TryParse(Console.ReadLine(), out depth)) Console.Write("数字无效");
                Console.Write("输入访问时间等待：");
                while (!Int32.TryParse(Console.ReadLine(), out waiting)) Console.Write("数字无效");
                Console.Write("输入下载图片最小值：");
                while (!Int32.TryParse(Console.ReadLine(), out minFileSize)) Console.Write("数字无效");
                Console.Write("兴趣正则：");
                pro.regex = Console.ReadLine();
                pro.depth = depth;
                pro.waiting = waiting;
                pro.minFileSize = minFileSize;
                pro.creationTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                TextFileHelper.SaveProject(pro);
            }
            else
            {
                pro = TextFileHelper.ReadProject(projectName);
                if (pro == null)
                {
                    pro = new ImageLoaderProject();
                    pro.projectName = projectName;
                    Console.WriteLine("无效项目");
                    Console.Write("输入项目初始网址：");
                    baseUrl = Console.ReadLine();
                    baseUrl = baseUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? baseUrl : "http://" + baseUrl;
                    pro.baseUrl = baseUrl;

                    Console.Write("请输入查找层数：");

                    while (!Int32.TryParse(Console.ReadLine(), out depth)) Console.Write("数字无效");
                    Console.Write("输入访问时间等待：");
                    while (!Int32.TryParse(Console.ReadLine(), out waiting)) Console.Write("数字无效");
                    Console.Write("输入下载图片最小值：");
                    while (!Int32.TryParse(Console.ReadLine(), out minFileSize)) Console.Write("数字无效");
                    Console.Write("兴趣正则：");
                    pro.regex = Console.ReadLine();
                    pro.depth = depth;
                    pro.depth = waiting;
                    pro.minFileSize = minFileSize;
                    pro.creationTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                    TextFileHelper.SaveProject(pro);
                }
                else
                {
                    Console.WriteLine("项目\t{0}", pro.projectName);
                    Console.WriteLine("基地址\t{0}", pro.baseUrl);
                    Console.WriteLine("深度\t{0}", pro.depth);
                    Console.WriteLine("等待\t{0}", pro.waiting);
                    Console.WriteLine("时间\t{0}", pro.creationTime);
                }
            }

            string storeFile = projectName + "\\" + "store.json";
            string visitedFile = projectName + "\\" + "visited.json";
            Regex r = new Regex(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*");
            Regex regex;
            if (string.IsNullOrWhiteSpace(pro.regex))
                regex = new Regex(@"^http://www.jdlingyu.moe/\d{2,5}/$");
            else
                regex = new Regex(pro.regex);
            List<LinkItem> store = new List<LinkItem>();
            List<string> visted = new List<string>();
            FileDownloader loder = new FileDownloader();
            string folderPath = projectName + "\\image\\";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            if (File.Exists(storeFile) && File.Exists(visitedFile))
            {
                TextFileHelper.ReadToMemory(out store, storeFile, out visted, visitedFile);
                Console.WriteLine("从历史中读取");
                Console.WriteLine("store:" + store.Count);
                Console.WriteLine("visited:" + visted.Count);
            }
            else
            {
                Console.WriteLine("没有历史文件,从基地址开始");
                foreach (var link in LinkFinder.Find(pro.baseUrl, 0))
                {
                    bool ignoreUrl = false;
                    ignoreUrl |= store.Where(p => p.Href == link.Href).Count() > 0;
                    //ignoreUrl |= visted.Where(p => p == link.Href).Count() > 0;
                    if (!ignoreUrl)
                    {
                        store.Add(link);
                    }
                }
            }
            //foreach (var item in store)
            //{
            //    Console.WriteLine(item.Href);
            //}
            //Console.ReadLine();
            if (pro.waiting < 200) pro.waiting = 2000;
            LinkFinder.MaxDepth = pro.depth;
            LinkFinder.ProjectFolder = pro.projectName;
            UserSetting.project = pro;
            int debugCounter = 0;

            //while (store.Where(p => p.Href != null && p.Href.IndexOf("www.001si.com") > 0).FirstOrDefault() != null)
            //{
            //    LinkItem item = store.Where(p => p.Href != null && p.Href.IndexOf("www.001si.com") > 0).FirstOrDefault();
            //    visted.Add(item.Href);
            //    store.Remove(item);
            //    debugCounter++;
            //}
            //TextFileHelper.SaveToFile(store, storeFile, visted, visitedFile);
            //Console.WriteLine("操作结束{0}", debugCounter);

            //Console.ReadLine();
            while (store.Count > 0)
            {
                Console.Write(debugCounter + "\t");
                LinkItem item = null;
                if (store.Count > 1000)
                {
                    item = store.Where(p => p.Href != null && p.Href.EndsWith(".jpg")).FirstOrDefault();
                }
                if (item == null)
                {
                    item = store.FirstOrDefault();
                }
                if (item != null && !string.IsNullOrWhiteSpace(item.Href))
                {
                    //访问过的就删除
                    if (visted.Contains(item.Href))
                    {
                        store.Remove(item);
                        debugCounter++;
                        continue;
                    }
                    Match m = r.Match(item.Href);
                    if (m.Success && !item.Href.EndsWith(".jpg") && !item.Href.EndsWith(".png") && !item.Href.EndsWith(".jpeg"))
                    {
                        Thread.Sleep(pro.waiting);
                        Console.WriteLine("Visiting\t" + item.Href);
                        foreach (var link in LinkFinder.Find(item.Href, item.depth + 1))
                        {
                            bool ignoreUrl = false;
                            ignoreUrl |= store.Where(p => p.Href == link.Href).Count() > 0;
                            ignoreUrl |= visted.Where(p => p == link.Href).Count() > 0;
                            if (!ignoreUrl)
                            {
                                store.Add(link);
                            }
                        }
                    }
                    else if (!m.Success)
                    {
                        Thread.Sleep(pro.waiting);
                        item.Href = ConbimeUrl(item.parentUrl, item.Href);
                        Console.WriteLine("Visiting\t" + item.Href);

                        foreach (var link in LinkFinder.Find(item.Href, item.depth + 1))
                        {
                            bool ignoreUrl = false;
                            ignoreUrl |= (store.Where(p => p.Href == link.Href).Count() > 0);
                            ignoreUrl |= (visted.Where(p => p == link.Href).Count() > 0);
                            if (!ignoreUrl)
                            {
                                store.Add(link);
                            }
                        }
                    }
                    if (item.Href.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && (item.Href.EndsWith(".jpg") || item.Href.EndsWith(".jpeg") || item.Href.EndsWith(".png")))
                    {
                        Thread.Sleep(pro.waiting);
                        Console.WriteLine("Downloading\t" + item.Href);
                        string fileName = item.Href.Substring(item.Href.LastIndexOf("/"));
                        fileName = item.Href.Replace("http://", "").Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
                            .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
                        string filePath = folderPath + fileName;
                        if (!File.Exists(filePath))
                            MusicHelper.DownLoad(item.Href, filePath);
                    }
                }
                if (!visted.Contains(item.Href))
                {
                    visted.Add(item.Href);
                }
                store.Remove(item);
                debugCounter++;
                Console.WriteLine("Count:" + store.Count());
                if (debugCounter % 20 == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("存档中...");
                    TextFileHelper.SaveToFile(store, storeFile, visted, visitedFile);
                    TextFileHelper.SaveImageUrls(store.Where(p => p.Href != null && p.Href.EndsWith(".jpg")).Select(p => p.Href).ToList(), projectName + "\\image.json");
                    TextFileHelper.SaveInterestingUrls(store.Where(p => p.Href != null && regex.IsMatch(p.Href)).Select(p => p.Href).Union(visted.Where(p => p != null && regex.IsMatch(p))).ToList(), projectName + "\\interesting.json");
                    Console.WriteLine("已存档！");
                    Console.ResetColor();
                }
                if (debugCounter % 2000 == 0)
                {
                    Console.Write("已执行{0}次，是否继续（Y*/N）:", debugCounter);
                    string continueInput = Console.ReadLine().ToUpper().Trim();
                    if (continueInput == "N") break;
                }
            }
            //string[] urls = ImageFinder.GetHtmlImageUrlList(HttpHelper.InvokeHttpGet("http://www.jdlingyu.moe/12316/", true));
            //foreach (var item in urls)
            //{
            //    Console.WriteLine(item);
            //}

            //foreach (var item in store)
            //{
            //    if (item.Href.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && item.Href.EndsWith(".jpg"))
            //    {
            //        Thread.Sleep(2000);
            //        Console.WriteLine("Downloading\t" + item.Href);
            //        string fileName = item.Href.Substring(item.Href.LastIndexOf("/"));
            //        fileName = fileName.Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "")
            //            .Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
            //        string filePath = folderPath + fileName;
            //        MusicHelper.DownLoad(item.Href, filePath);
            //    }
            //}

            Console.WriteLine("Progress Completed!");
            Console.ReadLine();
        }

        private static string ConbimeUrl(string parent, string node)
        {
            if (parent == null || node == null) return node;
            string header = parent.StartsWith("http://") ? "http://" : "";
            header = parent.StartsWith("https://") ? "https://" : header;
            parent = parent.Substring(header.Length);
            string domain;
            if (parent.IndexOf("/") < 0)
                domain = parent;
            else
                domain = parent.Substring(0, parent.IndexOf("/"));
            if (node.StartsWith("/"))
                return header + domain + node;
            else
                return header + domain + "/" + node;

            string sp = "/";
            char[] spl = sp.ToCharArray();
            string[] p = parent.Substring(header.Length - 1).Split(spl, StringSplitOptions.RemoveEmptyEntries);
            string[] c = node.Split(spl, StringSplitOptions.RemoveEmptyEntries);
            int st = p.Length;

            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] == c[0])
                {
                    st = i;
                    break;
                }
            }
            bool comb = true;
            if (st != p.Length)
            {
                for (int i = st, j = 0; i < p.Length && j < c.Length; i++, j++)
                {
                    comb &= (p[i] == c[j]);
                }
            }
            StringBuilder sb = new StringBuilder(header);
            if (!comb) st = p.Length;
            for (int i = 0; i < st; i++)
            {
                sb.Append(p[i]);
                sb.Append("/");
            }
            for (int i = 0; i < c.Length; i++)
            {
                sb.Append(c[i]);
                sb.Append("/");
            }
            return sb.ToString().TrimEnd('/');
        }

        /// <summary>
        /// 随机播放列表音乐
        /// </summary>
        /// <param name="player"></param>
        /// <param name="currentList"></param>
        private static void RandomPlay(WMPLib.WindowsMediaPlayer player, WMPLib.IWMPPlaylist currentList)
        {
            player.currentPlaylist = currentList;
            // Create a random number generator. 
            System.Random randGenerator = new System.Random();

            // Store the count of all media items in the media collection.
            int count = player.currentPlaylist.count;

            // Get a random integer using the count as the max value.
            int rand = randGenerator.Next(count);

            // Make the random media item the current media item.
            player.currentMedia = player.currentPlaylist.get_Item(rand);
            player.controls.play();
        }

        #region 三种下载播放模式
        private static void JustPlay(string folderPath, WMPLib.WindowsMediaPlayer player)
        {
            FileDownloader loader = new FileDownloader();
            if (player == null) player = new WMPLib.WindowsMediaPlayer();
            WMPLib.IWMPMedia currentPlay = null;
            WMPLib.IWMPMedia nextPlay = null;
            WMPLib.IWMPPlaylist currentList = null;
            SingleMusic music = null;

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

            bool newLoad;
            string filePath = "";
            while (true)
            {
                music = new SingleMusic();
                filePath = music.Download(folderPath, out newLoad);
                if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    break;
                }
            }
            currentPlay = player.newMedia(filePath);
            player.currentPlaylist = currentList;
            currentList.appendItem(currentPlay);
            player.controls.playItem(currentPlay);
            Console.WriteLine("开始播放" + currentPlay.name);

            while (true)
            {
                music = new SingleMusic();
                filePath = music.Download(folderPath, out newLoad);
                if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    break;
                }
            }

            currentPlay = player.newMedia(filePath);
            currentList.appendItem(currentPlay);
            Console.WriteLine(currentPlay.name + "加入播放列表");

            bool ignore = false;
            WMPLib._WMPOCXEvents_CurrentItemChangeEventHandler handler = (o) =>
            {
                if (ignore) return;
                ignore = true;

                Console.WriteLine("正在播放" + currentPlay.name);

                while (true)
                {
                    music = new SingleMusic();
                    filePath = music.Download(folderPath, out newLoad);
                    if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                    {
                        break;
                    }
                }
                nextPlay = player.newMedia(filePath);
                currentList.appendItem(nextPlay);
                Console.WriteLine(nextPlay.name + "加入播放列表");
                ignore = false;
            };
            player.CurrentItemChange += handler;



            Console.WriteLine("退出请回车");
            Console.ReadLine();
            player.CurrentItemChange -= handler;
        }

        private static void SearchMusic(string folderPath, WMPLib.WindowsMediaPlayer player)
        {
            if (player == null) player = new WMPLib.WindowsMediaPlayer();
            WMPLib.IWMPMedia currentPlay = null;
            WMPLib.IWMPPlaylist currentList = null;
            if (player.playlistCollection.getByName("Search").count > 0)
            {
                currentList = player.playlistCollection.getByName("Search").Item(0);
            }
            else
            {
                currentList = player.playlistCollection.newPlaylist("Search");
            }
            currentList.clear();
            SingleMusic music = null;
            bool exit = false;
            while (!exit)
            {
                MusicList mlist = new MusicList();
                Console.Write("请输入动漫名或者歌曲名：");
                string keyWord = Console.ReadLine();
                if (keyWord.ToUpper().Trim() == "Q" || keyWord.ToUpper().Trim() == "QUIT" || keyWord.ToUpper().Trim() == "E" || keyWord.ToUpper().Trim() == "EXIT")
                {
                    exit = true;
                    break;
                }

                try
                {
                    mlist.SearchMusic(keyWord);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("查询出现异常：" + ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                    continue;
                }
                if (mlist.Musics.Count() <= 0)
                {
                    Console.WriteLine("没有找到歌曲,请更换查询");
                    continue;
                }
                Maximize();
                while (true)
                {
                    bool newload;
                    string filePath;
                    mlist.FormatePrint();
                    Console.Write("请输入点歌号码:");
                    string sindex = Console.ReadLine().Trim().ToUpper();
                    if (sindex.ToUpper().Trim().Contains("ALL"))
                    {
                        Restore();
                        foreach (var item in mlist.Musics)
                        {
                            music = item.GetSingleMusic();
                            if (music == null)
                            {
                                Console.WriteLine("无效歌曲：{0}-{1}", item.AnimeName, item.Title);
                                continue;
                            }
                            filePath = music.Download(folderPath, out newload);
                            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                            {
                                Console.WriteLine("{0}-{1}下载好像出了点故障( ▼-▼ )", item.AnimeName, item.Title);
                                continue;
                            }
                            currentPlay = player.newMedia(filePath);
                            currentList.appendItem(currentPlay);

                            if (player.playState != WMPLib.WMPPlayState.wmppsPlaying)
                            {
                                player.currentPlaylist = currentList;
                                try
                                {
                                    player.controls.playItem(currentPlay);
                                }
                                catch { }
                                Console.WriteLine("开始播放" + currentPlay.name);
                            }
                        }
                        continue;
                    }
                    if (sindex == "R")
                    {
                        break;
                    }
                    if (sindex == "Q" || sindex == "QUIT" || sindex == "E" || sindex == "EXIT")
                    {
                        Restore();
                        exit = true;
                        break;
                    }
                    if (sindex == "STOP")
                    {
                        if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                        {
                            player.controls.stop();
                        }
                    }

                    int index;
                    bool parsed = Int32.TryParse(sindex, out index);

                    if (!parsed)
                    {
                        Console.WriteLine("无效序列，请重新输入");
                        continue;
                    }
                    Restore();
                    music = mlist.GetSingleMusic(index);
                    if (music == null)
                    {
                        Console.WriteLine("无效序列，请重新输入");
                        continue;
                    }
                    filePath = music.Download(folderPath, out newload);
                    if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                    {
                        Console.WriteLine("下载好像出了点故障( ▼-▼ )");
                        continue;
                    }
                    currentPlay = player.newMedia(filePath);
                    currentList.appendItem(currentPlay);

                    if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                    {
                        Console.Write("当前正在播放，加入播放列表*(A)或者直接播放(I):");
                        string wd = Console.ReadLine().ToUpper().Trim();
                        if (wd == "I")
                        {
                            player.currentPlaylist = currentList;
                            player.controls.playItem(currentPlay);
                            Console.WriteLine("开始播放" + currentPlay.name);
                        }
                    }
                    else
                    {
                        player.currentPlaylist = currentList;
                        try
                        {
                            player.controls.playItem(currentPlay);
                        }
                        catch { }
                        Console.WriteLine("开始播放" + currentPlay.name);
                    }
                }
            }
        }

        private static void DianGe(string folderPath, WMPLib.WindowsMediaPlayer player)
        {
            if (player == null) player = new WMPLib.WindowsMediaPlayer();
            WMPLib.IWMPMedia currentPlay = null;
            WMPLib.IWMPPlaylist currentList = null;
            if (player.playlistCollection.getByName("Search").count > 0)
            {
                currentList = player.playlistCollection.getByName("Search").Item(0);
            }
            else
            {
                currentList = player.playlistCollection.newPlaylist("Search");
            }
            currentList.clear();
            SingleMusic music = null;
            bool exit = false;
            int page = 10;
            while (!exit)
            {
                MusicList mlist = new MusicList();
                Console.WriteLine("*********************************************************************");
                Console.WriteLine("********************************第" + page + "页*******************************");
                try
                {
                    mlist.GetNewMusics(page);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("查询出现异常：" + ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("是否重新查询(Y*/N):");
                    string word = Console.ReadLine().Trim().ToUpper();
                    if (word == "N") break;
                    continue;
                }
                if (mlist.Musics.Count <= 0)
                {
                    page++;
                    continue;
                }
                Maximize();    // make console fullscreen
                while (true)
                {
                    string filePath;
                    bool newload;
                    mlist.FormatePrint();
                    Console.WriteLine("********************************第" + page + "页*******************************");
                    Console.WriteLine("*********************************************************************");
                    Console.Write("请输入点歌号码:");
                    //###
                    //string sindex = Console.ReadLine().Trim().ToUpper();
                    //###
                    string sindex = "ALL";
                    if (sindex.ToUpper().Trim().Contains("ALL"))
                    {
                        Restore();    // make console normal
                        foreach (var item in mlist.Musics)
                        {
                            music = item.GetSingleMusic();
                            if (music == null)
                            {
                                Console.WriteLine("无效歌曲：{0}-{1}", item.AnimeName, item.Title);
                                continue;
                            }
                            filePath = music.Download(folderPath, out newload, true);
                            //###
                            continue;
                            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                            {
                                Console.WriteLine("{0}-{1}下载好像出了点故障( ▼-▼ )", item.AnimeName, item.Title);
                                continue;
                            }
                            currentPlay = player.newMedia(filePath);
                            currentList.appendItem(currentPlay);

                            if (player.playState != WMPLib.WMPPlayState.wmppsPlaying)
                            {
                                player.currentPlaylist = currentList;
                                try
                                {
                                    //player.controls.playItem(currentPlay);
                                }
                                catch { }
                                Console.WriteLine("开始播放" + currentPlay.name);
                            }
                        }
                        //###continue;
                        page++;
                        break;
                    }
                    if (sindex == "F")
                    {
                        page++;
                        break;
                    }
                    if (sindex == "R")
                    {
                        page++;
                        break;
                    }
                    if (sindex == "I")
                    {
                        page = 1;
                        break;
                    }
                    if (sindex == "B")
                    {
                        page--;
                        if (page <= 0) page = 1;
                        break;
                    }
                    if (sindex == "Q" || sindex == "QUIT" || sindex == "E" || sindex == "EXIT")
                    {
                        Restore();    // make console normal
                        exit = true;
                        break;
                    }
                    if (sindex == "STOP")
                    {
                        if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                        {
                            player.controls.stop();
                            continue;
                        }
                    }
                    int index;
                    bool parsed = Int32.TryParse(sindex, out index);

                    if (!parsed)
                    {
                        Console.WriteLine("无效序列，请重新输入");
                        continue;
                    }
                    music = mlist.GetSingleMusic(index);
                    if (music == null)
                    {
                        Console.WriteLine("无效序列，请重新输入");
                        continue;
                    }
                    Restore();   // make console normal
                    filePath = music.Download(folderPath, out newload);
                    if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                    {
                        Console.WriteLine("下载好像出了点故障( ▼-▼ )");
                        continue;
                    }
                    currentPlay = player.newMedia(filePath);
                    currentList.appendItem(currentPlay);
                    if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
                    {
                        Console.Write("当前正在播放，加入播放列表*(A)或者直接播放(I):");
                        string keyWord = Console.ReadLine().ToUpper().Trim();
                        if (keyWord == "I")
                        {
                            player.currentPlaylist = currentList;
                            player.controls.playItem(currentPlay);
                            Console.WriteLine("开始播放" + currentPlay.name);
                        }
                    }
                    else
                    {
                        player.currentPlaylist = currentList;
                        try
                        {
                            player.controls.playItem(currentPlay);
                        }
                        catch { }
                        Console.WriteLine("开始播放" + currentPlay.name);
                    }
                }
            }
        }
        #endregion
    }
}
