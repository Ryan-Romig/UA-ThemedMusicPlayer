using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;

namespace UnderwaterAudioMusicManagerApp
{
    public class UnderwaterAudioMediaPlayer : System.Windows.Media.MediaPlayer
    {
        public class NestedClass
        {
            public static string nestedString;

        }

        public BindingList<Track> mediaLibrary = new BindingList<Track>();
        public static string playerStopped = "stopped";
        public static string playerPaused = "paused";
        public static string playerPlaying = "playing";
        public string playState = playerStopped;
        public static string nextMedia;
        public static string previousMedia;
        public bool isShuffle;
        public bool isRepeat;
        //public List<Track> mediaLibrary = new List<Track>();
        public Track currentMedia = new Track();
        public DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        public static string supportedMusicFileTypes = "Music(.mp3) (.wav) (.aac) (.mp4) (.flac) (.wma) |*.mp3;*.wav;*.aac;*.mp4;*.m4a;*.flac;*.wma";


        private void getTrackTags(Track track, int i, string[] songFileName, string[] songFilePath)
        {
            track.fileName = System.IO.Path.GetFileNameWithoutExtension(songFileName[i]);
            track.filePath = songFilePath[i];
            var tag = TagLib.File.Create(track.filePath);
            track.artist = tag.Tag.FirstPerformer;
            track.duration = tag.Properties.Duration;
            track.genre = tag.Tag.FirstGenre;
            track.album = tag.Tag.Album;
            track.songName = tag.Tag.Title;

        }
        private void getTrackTags(Track track, int i, string[] songFilePath)
        {
            track.fileName = System.IO.Path.GetFileNameWithoutExtension(songFilePath[i]);
            track.filePath = songFilePath[i];
            var tag = TagLib.File.Create(track.filePath);
            track.artist = tag.Tag.FirstPerformer;
            track.duration = tag.Properties.Duration;
            track.genre = tag.Tag.FirstGenre;
            track.album = tag.Tag.Album;
            track.songName = tag.Tag.Title;

        }


        private void loadPreviousLibrary()
        {
            MessageBox.Show(String.Format("{0}", (File.Exists("library.plst")).ToString()));
            if (File.Exists("library.plst"))
            {
                string[] savedPlaylist = File.ReadAllLines("library.plst");
                MessageBox.Show(String.Format("{0}", savedPlaylist.Length.ToString()));
                Dictionary<string, Track> dic = new Dictionary<string, Track>();
                foreach(Track song in mediaLibrary)
                {
                    dic.Add(song.filePath, song);
                }
                for (int i = 0; i < savedPlaylist.Length; i++)
                {
                    Track track = new Track();
                    track.fileName = System.IO.Path.GetFileNameWithoutExtension(savedPlaylist[i]);
                    track.filePath = savedPlaylist[i];
                    var tag = TagLib.File.Create(track.filePath);
                    track.artist = tag.Tag.FirstPerformer;
                    track.duration = tag.Properties.Duration;
                    track.genre = tag.Tag.FirstGenre;
                    track.album = tag.Tag.Album;
                    track.songName = tag.Tag.Title;
                    if (dic.ContainsKey(track.filePath) != true)
                    {
                        mediaLibrary.Add(track); // sets up library database
                    }
                    else
                    {

                    }
                    
                  
                    
                  
                       
                    






                }
                
            }


        }

        void loadMusicFromDefaultMusicFolderIntoLibraryOnProgramStart()
        {
            string defaultMusicFolderPath = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)).FullName;
            string[] filesInDefaultDirectory = Directory.GetFiles(defaultMusicFolderPath, "*.*").Where(file => file.ToLower().EndsWith(".mp3") || file.ToLower().EndsWith(".wav") || file.ToLower().EndsWith(".flac") || file.ToLower().EndsWith(".m4a") || file.ToLower().EndsWith(".mp4") || file.ToLower().EndsWith(".wma") || file.ToLower().EndsWith(".aac")).ToArray(); //|| file.ToLower().EndsWith(".EXT")

            for (int i = 0; i < filesInDefaultDirectory.Length; i++)
            {
                Track track = new Track();
                track.fileName = System.IO.Path.GetFileNameWithoutExtension(filesInDefaultDirectory[i]);
                track.filePath = filesInDefaultDirectory[i];
                var tag = TagLib.File.Create(track.filePath);
                track.artist = tag.Tag.FirstPerformer;
                track.duration = tag.Properties.Duration;
                track.genre = tag.Tag.FirstGenre;
                track.album = tag.Tag.Album;
                track.songName = tag.Tag.Title;
                mediaLibrary.Add(track); // sets up library database
            }
            




        }

        


        public UnderwaterAudioMediaPlayer()
        {

            loadMusicFromDefaultMusicFolderIntoLibraryOnProgramStart();
            loadPreviousLibrary();

        }



        public void pause()
        {
            this.Pause();
            this.playState = playerPaused;
            this.timer.Stop();
            
        }

        public void play()
        {
            this.Play();
            this.playState = playerPlaying;
            this.timer.Start();
            
        }

        public void stop()
        {
            this.Stop();
            this.playState = playerStopped;
            this.timer.Stop();
        }

        public void loadSongIntoPlayer(System.Uri filePath)
        {
            this.Open(filePath);
            this.stop();

        }
        public void loadSongIntoPlayer(Track track)
        {
            this.Open(new Uri(track.filePath));
            this.stop();
            this.currentMedia = track;

        }

        public void loadSongIntoPlayer(String filePath)
        {
            this.Open(new Uri(filePath));
            this.stop();

        }



    }
}
