using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;



namespace UnderwaterAudioMusicManagerApp
{
   public class Data
    {
        public List<Playlist> playlistCollection = new List<Playlist>();
        public Playlist mediaLibrary = new Playlist();
       public  List<string> savedFolderPaths = new List<string>();

       public Data()
        {
            mediaLibrary.Name = "Library";
        }
    }
}
