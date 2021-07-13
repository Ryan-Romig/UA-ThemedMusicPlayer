using System.Collections.Generic;




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
