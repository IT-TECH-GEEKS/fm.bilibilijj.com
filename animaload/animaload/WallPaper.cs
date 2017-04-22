using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace animeload
{
    public sealed class Wallpaper
    {
        Wallpaper() { }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched,
            Fit,
            Fill
        }

        public static void Set(Uri imageUri, Style style = Style.Fill)
        {
            System.IO.Stream s = new System.Net.WebClient().OpenRead(imageUri.ToString());

            System.Drawing.Image img = System.Drawing.Image.FromStream(s);
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.jpg");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Jpeg);

            SetWallpaper(style, tempPath);
        }

        public static void Set(string filePath, Style style = Style.Stretched)
        {
            string tempPath = filePath;

            SetWallpaper(style, tempPath);
        }

        private static void SetWallpaper(Style style, string tempPath)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            //设置壁纸风格和展开方式  
            //在Control Panel\Desktop中的两个键值将被设置  
            // TileWallpaper  
            //  0: 图片不被平铺   
            //  1: 被平铺   
            // WallpaperStyle  
            //  0:  0表示图片居中，1表示平铺  
            //  2:  拉伸填充整个屏幕  
            //  6:  拉伸适应屏幕并保持高度比  
            //  10: 图片被调整大小裁剪适应屏幕保持纵横比 

            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }
            if (style == Style.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

    }
}
