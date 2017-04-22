using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace animeload
{
    public class MusicListItem
    {
        public string A128 { get; set; }
        public string A320 { get; set; }
        public string AnimeName { get; set; }
        public string ID { get; set; }
        public string MusicID { get; set; }
        public string ROW { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        public string Title { get; set; }
        public string Top { get; set; }
        public string WuSun { get; set; }

        public bool HasSequence = false;

        public SingleMusic GetSingleMusic()
        {
            int goid = Convert.ToInt32(MusicID);
            try
            {
                return new SingleMusic(goid);
            }
            catch
            {
                return null;
            }
        }

        public void FromJson(object partJson)
        {
            Dictionary<string, object> music = partJson as Dictionary<string, object>;
            try
            {
                Title = music["Title"].ToString();
                AnimeName = music["AnimeName"].ToString();
                MusicID = music["MusicID"].ToString();
                A128 = music["A128"].ToString();
                A320 = music["A320"].ToString();
                ID = music["ID"].ToString();
                if (music.Keys.Contains("ROW"))
                {
                    ROW = music["ROW"].ToString();
                    HasSequence = true;
                }
                else
                {
                    HasSequence = false;
                }
                Time = music["Time"].ToString();
                Top = music["Top"].ToString();
                WuSun = music["WuSun"].ToString();
            }
            catch
            {
            }
        }
    }
}
