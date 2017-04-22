using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace animeload
{
    public class MusicList
    {
        public List<MusicListItem> Musics;
        public MusicList()
        {
            Musics = new List<MusicListItem>();
        }
        public void SearchMusic(string keyWord)
        {
            Musics.Clear();
            string result = MusicHelper.SearchMusicJson(keyWord);
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            Object[] musicList = null;
            try
            {
                musicList = (Object[])Jss.DeserializeObject(result);
            }
            catch
            {
                throw;
            }
            try
            {
                if (musicList == null || musicList.Length <= 0) return;
                foreach (var item in musicList)
                {
                    MusicListItem musicItem = new MusicListItem();
                    musicItem.FromJson(item);
                    Musics.Add(musicItem);
                }
            }
            catch
            {
                throw;
            }
        }

        public void GetNewMusics(int page)
        {
            Musics.Clear();
            string result = MusicHelper.GetNewMusicListJson(page);
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            Object[] musicList = null;
            try
            {
                musicList = (Object[])Jss.DeserializeObject(result);
            }
            catch
            {
                throw;
            }
            try
            {
                if (musicList == null || musicList.Length <= 0) return;
                foreach (var item in musicList)
                {
                    MusicListItem musicItem = new MusicListItem();
                    musicItem.FromJson(item);
                    Musics.Add(musicItem);
                }
            }
            catch
            {
                throw;
            }
        }

        public SingleMusic GetSingleMusic(int index)
        {
            if (index < Musics.Count())
            {
                return Musics[index].GetSingleMusic();
            }
            else
            {
                MusicListItem item = Musics.Where(p => p.MusicID == index.ToString()).FirstOrDefault();
                if (item != null)
                    return item.GetSingleMusic();
                return null;
            }
        }

        public void FormatePrint()
        {
            if (Musics != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("*******************************************************************");
                foreach (var item in Musics)
                {
                    Console.WriteLine("{0}\t{1}\t{2}-{3}", Musics.IndexOf(item), item.MusicID, item.AnimeName, item.Title);
                }
                Console.WriteLine("*******************************************************************");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
