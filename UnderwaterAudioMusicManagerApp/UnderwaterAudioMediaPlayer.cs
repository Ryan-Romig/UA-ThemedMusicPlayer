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
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace UnderwaterAudioMusicManagerApp
{
    public class UnderwaterAudioMediaPlayer : System.Windows.Media.MediaPlayer
    {
        //public  List<List<Track>> playlist = new List<List<Track>>();
        public List<Playlist> playlistCollection = new List<Playlist>();
        public List<Track> mediaLibrary = new List<Track>();
        //public BindingList<Track> mediaLibrary = new BindingList<Track>();
        public static string playerStopped = "stopped";
        public static string playerPaused = "paused";
        public static string playerPlaying = "playing";
        public string playState = playerStopped;
        public static string nextMedia;
        public static string previousMedia;
        public bool isShuffle;
        public bool isRepeat;
        public bool isScrubbing;
        public Track currentMedia = new Track();
        public DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        public static string supportedMusicFileTypes = "Music(.mp3) (.wav) (.aac) (.mp4) (.flac) (.wma) |*.mp3;*.wav;*.aac;*.mp4;*.m4a;*.flac;*.wma";
        public  System.TimeSpan pausedPosition;
        public int currentPlaylistIndex;
        public Track selectedTrack;
        public List<Track> currentPlaylist;
        public Playlist selectedPlaylist;


        public UnderwaterAudioMediaPlayer()
        {
            loadMusicFromDefaultMusicFolderIntoLibraryOnProgramStart();
            loadPreviousLibrary();
        }



        //used for getting tags when adding media using the add to library button
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
        //used with importing new songs from saved library.plst
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


        //takes a text file with extention .plst and reads each lines to an array
        public void loadPreviousLibrary()
        {

            if (File.Exists("library.plst"))
            {
                string[] savedPlaylist = File.ReadAllLines("library.plst");

                Dictionary<string, Track> dic = new Dictionary<string, Track>();
                foreach (Track song in mediaLibrary)
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
                    if(tag.Tag.Pictures.FirstOrDefault() != null )
                    {
                        var pic = tag.Tag.Pictures[0];
                        MemoryStream ms = new MemoryStream(pic.Data.Data);
                        ms.Seek(0, SeekOrigin.Begin);
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();
                        track.albumArt = new Image();
                        track.albumArt.Source = bitmap;
                    }
                    else
                    {
                        track.albumArt = null;
                    }
                   
                   
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


        //loads songs from default music folder. 
        void loadMusicFromDefaultMusicFolderIntoLibraryOnProgramStart()
        {
            //dumps all music files into an array 
            string defaultMusicFolderPath = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)).FullName;
            string[] filesInDefaultDirectory = Directory.GetFiles(defaultMusicFolderPath, "*.*").Where(file => file.ToLower().EndsWith(".mp3") || file.ToLower().EndsWith(".wav") || file.ToLower().EndsWith(".flac") || file.ToLower().EndsWith(".m4a") || file.ToLower().EndsWith(".mp4") || file.ToLower().EndsWith(".wma") || file.ToLower().EndsWith(".aac")).ToArray(); //|| file.ToLower().EndsWith(".EXT")

            //importing each music file into a Track object and saving to the main library
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
                mediaLibrary.Add(track);
                if (tag.Tag.Pictures.FirstOrDefault() != null)
                {
                    var pic = tag.Tag.Pictures[0];
                    MemoryStream ms = new MemoryStream(pic.Data.Data);
                    ms.Seek(0, SeekOrigin.Begin);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();

                    track.albumArt.Source = bitmap;
                }
                else
                {
                    track.albumArt = null;
                }
            }
        }
       

        //custom  functions that sets playstate
        public void pause()
        {
            
            this.Pause();
            pausedPosition = this.Position;
            this.playState = playerPaused;
            this.timer.Stop();
            
        }

        public void play()
        {
            this.timer.Stop();
            if(this.playState == playerPaused)
            {
                this.Position = pausedPosition;
            }
            else
            {
                this.Position = new TimeSpan(0);
            }
            this.Play();
            this.playState = playerPlaying;
            this.timer.Start();
            
        }

        public void stop()
        {
            this.Stop();
            this.playState = playerStopped;            
            this.Position = new System.TimeSpan(0);
            this.timer.Stop();
        }
        
        public void loadSongIntoPlayer(Track track)
        {
            this.Open(new Uri(track.filePath));
            this.stop();//remember that position is set to 0 here
            this.currentMedia = track;
           

        }
        public void loadSongIntoPlayer(IEnumerable<Track> track)
        {
            Track[] tracks =  track.ToArray();
            this.Open(new Uri(tracks[0].filePath));
            this.stop();//remember that position is set to 0 here
            this.currentMedia = tracks[0];


        }

    }
}
