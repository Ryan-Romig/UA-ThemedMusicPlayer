using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
//using WMPLib; for old windows media player way




namespace UnderwaterAudioMusicManagerApp
{
    //Color Pallet//

    /* 
    Main ---  #009BD8
    2nd Blue -- #00B7DD
    Blue Green-- #00CFCC
    Green  -----#41E3AB
    Light Green --- #A4F187
    Yello --- #F9F871
    */

    public partial class MainWindow : Window
    {

        private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }
            return (IntPtr)0;
        }
        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>x coordinate of point.</summary>
            public int x;
            /// <summary>y coordinate of point.</summary>
            public int y;
            /// <summary>Construct a point of coordinates (x,y).</summary>
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public static readonly RECT Empty = new RECT();
            public int Width { get { return Math.Abs(right - left); } }
            public int Height { get { return bottom - top; } }
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }
            public bool IsEmpty { get { return left >= right || top >= bottom; } }
            public override string ToString()
            {
                if (this == Empty) { return "RECT {Empty}"; }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }
            public override bool Equals(object obj)
            {
                if (!(obj is Rect)) { return false; }
                return (this == (RECT)obj);
            }
            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        //above is for proper borderless window




        public MainWindow()
        {


            InitializeComponent();
            //borderless window stuff. dont touch below. keep ths block after InitializeComponent();
            SourceInitialized += (s, e) =>
            {
                IntPtr handle = (new WindowInteropHelper(this)).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
            };
            //borderless window stuff. dont touch above

            player.MediaOpened += new EventHandler(player_MediaOpened);
            
        }


        private void player_MediaOpened(object sender, EventArgs e)
        {
            trackProgressSlider.Maximum = player.NaturalDuration.TimeSpan.TotalMilliseconds;
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            
            timeElapsedLabel.Content = player.Position.ToString(@"m\:ss");
            trackProgressBar.Value = (player.Position.TotalSeconds / player.NaturalDuration.TimeSpan.TotalSeconds) * 100;
        }

        //--------------------------------------------------------


        //public static WindowsMediaPlayer player = new WindowsMediaPlayer(); ------------ OLD Windows media player way
        private static MediaPlayer player = new MediaPlayer();
        
        String[] songFileName, songFilePath;
        bool isPlaying;
        bool isPaused;
        Dictionary<string, Track> musicLibrary = new Dictionary<string, Track>();
        int mediumResponsiveWindowSize = 570;



        

        private void loadSongIntoPlayer(System.Uri filePath)
        {            
            player.Open(filePath);
            player.Stop();
            
        }

        private double getWindowSize()
        {
            return mainWindow.Width;
        }

        private void swapPlayToPauseButton()
        {
            playButton.Visibility = Visibility.Collapsed;
            pauseButton.Visibility = Visibility.Visible;

        }

        private void swapPauseToPlayButton()
        {
            playButton.Visibility = Visibility.Visible;
            pauseButton.Visibility = Visibility.Collapsed;

        }






        
        private void startPlaying() //queue the start playing
        {
            player.Play();
            isPaused = false;
            isPlaying = true;
            swapPlayToPauseButton();

        }


        private void pausePlaying()
        {
            player.Pause();
            isPaused = true;
            isPlaying = false;
            swapPauseToPlayButton();
        }


        private string getFilePathFromSelectedFile(string itemName, Dictionary<string, Track> list) 
        {
            if (itemName != null && list.ContainsKey(itemName.ToString()))
            {
                return list[itemName].filePath;

            }
            return null;
        }



    


        //private void selectPreviousSong()
        //{
        //    if (playlistBox.SelectedIndex! < 0)
        //    {
        //        playlistBox.SelectedIndex--;
        //    }

        //}

        private Track searchForSelectedFile(string itemName, Dictionary<string, Track> list)
        {
            if (itemName != null && list.ContainsKey(itemName.ToString()))
            {
                return list[itemName];

            }
            return null;
        }

        private void rewindButton_Click(object sender, RoutedEventArgs e)
        {
            //if (playlistBox.SelectedIndex < 0)
            //{
            //    return;
            //}
            //else
            //{
            //    selectPreviousSong();
            //    if (isPlaying == true)
            //    {
            //        //startPlayingSelectedItem(playlistBox.SelectedItem.ToString());
            //    }
            //}

        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //private void selectNextSong()
        //{
        //    if (playlistBox.SelectedIndex >= 0)
        //    {
        //        playlistBox.SelectedIndex++;
        //    }

        //}

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            //selectNextSong();
            //if (isPlaying)
            //{
            //    //startPlayingSelectedItem(playlistBox.SelectedItem.ToString());
            //}
        }




        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            openImportDialog();

        }

        private void openImportDialog()
        {
            OpenFileDialog openFilePopup = new OpenFileDialog();
            openFilePopup.Multiselect = true;
            openFilePopup.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFilePopup.ShowDialog() == true)
            {

                songFileName = openFilePopup.SafeFileNames;
                songFilePath = openFilePopup.FileNames;

                for(int i= 0; i < songFilePath.Length; i++)
                {
                    Track track = new Track();
                    getTrackTags(track, i);


                    if (musicLibrary.ContainsKey(track.fileName) != true)
                    {

                        addTrackToLibrary(track, i);
                    }
                    
                }

            }
        }
        private void addTrackToLibrary(Track track, int i)
        {
            musicLibrary.Add(track.fileName.ToString(), track);
            playlistBox.Items.Add(track.fileName.ToString());
        }

        private void getTrackTags(Track track, int i)
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


        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if
                (isPlaying)
                //(player.playState == WMPPlayState.wmppsPlaying)
            {
                pausePlaying();

            }

            
        }
        public bool isShuffle;
        private void swapShuffleIcons()
        {
            if (isShuffle)
            {
                shuffleButton.Visibility = Visibility.Visible;
                shuffleButtonOn.Visibility = Visibility.Collapsed;
            }
            else if(isShuffle == false)
            {
                shuffleButton.Visibility = Visibility.Collapsed;
                shuffleButtonOn.Visibility = Visibility.Visible;
            }
            
        }
        private void toggleShuffle()
        {
            if(isShuffle)
            {
                swapShuffleIcons();
                isShuffle = false;
                
                

            }
            else if(isShuffle == false)
            {

                swapShuffleIcons();
         
                isShuffle = true;
                




            }

        }
        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            toggleShuffle();
            
           
        }



        private void trackProgressSlider_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void volumeSlider_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           
                double playerVolume = volumeSlider.Value / 100;
                player.Volume = playerVolume;           
                

          
        }
  









        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mainWindow.Width <= mediumResponsiveWindowSize)
            {

                volumeSlider.Visibility = Visibility.Collapsed;
                volumeButton.Visibility = Visibility.Visible;

            }
            if (mainWindow.Width > mediumResponsiveWindowSize)
            {

                volumeSlider.Visibility = Visibility.Visible;
                volumeButton.Visibility = Visibility.Collapsed;


            }
        }


        private void handlePlayButtonClick()
        {
            if (isPaused)
            {
                player.Play();
                isPaused = false;
                isPlaying = true;
                swapPlayToPauseButton();
            }

            else
            {
                if (playlistBox.SelectedItem == null)
                {
                    return;
                }

                else
                {

                    loadSongIntoPlayer(new System.Uri(getFilePathFromSelectedFile(playlistBox.SelectedItem.ToString(), musicLibrary)));
                    player.Play();
                    startPlaying();
                    isPlaying = true;
                    isPaused = false;
                    swapPlayToPauseButton();

                }
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            handlePlayButtonClick();
            

        }
    }
}
