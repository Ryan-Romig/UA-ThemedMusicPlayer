using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.Windows.Threading;


namespace UnderwaterAudioMusicManagerApp
{
    public class UnderwaterAudioMediaPlayer : System.Windows.Media.MediaPlayer
    {

        public static string playerStopped = "stopped";
        public static string playerPaused = "paused";
        public static string playerPlaying = "playing";
        public string playState = playerStopped;
        public static string nextMedia;
        public static string previousMedia;
        public bool isShuffle;
        public bool isRepeat;
       public List<Track> mediaLibrary = new List<Track>();
        public Track currentMedia = new Track();
        public DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        public static string supportedMusicFileTypes = "Music(.mp3) (.wav) (.aac) (.mp4) (.flac) (.wma) |*.mp3;*.wav;*.aac;*.mp4;*.m4a;*.flac;*.wma";



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
            this.currentMedia = mediaLibrary[1];
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
