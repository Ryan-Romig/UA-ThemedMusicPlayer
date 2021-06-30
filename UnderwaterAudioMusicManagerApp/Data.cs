using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;



namespace UnderwaterAudioMusicManagerApp
{
    class Data
    {


        List<String> musicFolders = new List<string>();



        Dictionary<string, List<Track>> playlistCollection = new Dictionary<string,List<Track>>();



        List<Track> playlist = new List<Track>();
            


        void addMusicFolder(string folderPath)
        {
            musicFolders.Add(folderPath);
        }

        void populatePlaylist(List<Track> playlist)
        {

            
        }



     Track searchForSelectedFile(string fileName, Dictionary<string, Track> library)
        {
            if (fileName != null && library.ContainsKey(fileName.ToString()))
            {
                return library[fileName];

            }
            return null;
        }



        void createPlaylist()
        {
            //playlistCollection.Add();

        }


         string getDefaultMusicFolder()
        {
            string defaultMusicFolderPath = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)).FullName;
            return defaultMusicFolderPath;
        }
    }
}
