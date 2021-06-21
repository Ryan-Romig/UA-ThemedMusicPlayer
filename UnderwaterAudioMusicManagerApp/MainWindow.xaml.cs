using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WMPLib;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;

namespace UnderwaterAudioMusicManagerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

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


            //borderless window stuff. dont touch
            SourceInitialized += (s, e) =>
            {
                IntPtr handle = (new WindowInteropHelper(this)).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
            };
            //borderless window stuff. dont touch
        }





        //--------------------------------------------------------


        public static WindowsMediaPlayer player = new WindowsMediaPlayer();
        String[] songFileName, songFilePath;
        bool isPlayingSong = false;
        Dictionary<string, Track> musicLibrary = new Dictionary<string, Track>();
        string selectedFile;
        int mediumResponsiveWindowSize = 570;
       
        

    





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
            try
            {
                if (selectedFile != null){
                    loadSongIntoPlayer(getFilePathFromSelectedFile(selectedFile, musicLibrary));
                    player.controls.play();
                    isPlayingSong = true;
                    swapPlayToPauseButton();
                }
                
            }
            catch
            {
                Console.WriteLine("failed to load song");
            }
            
            
        }
        private string getFilePathFromSelectedFile(string itemName, Dictionary<string, Track> list) 
        {
            if (itemName != null && list.ContainsKey(itemName.ToString()))
            {
                return list[itemName].filePath;

            }
            return null;
        }
        private void loadSongIntoPlayer(string filePath)
        {

            player.URL = filePath;
        }
        private void selectPreviousSong()
        {
            if (playlistBox.SelectedIndex! < 0)
            {
                playlistBox.SelectedIndex--;
            }

        }

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
            if (playlistBox.SelectedIndex < 0)
            {
                return;
            }
            else
            {
                selectPreviousSong();
                if (player.playState == WMPPlayState.wmppsPlaying)
                {
                    startPlaying();
                }
            }
           
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void selectNextSong()
        {
            if (playlistBox.SelectedIndex >= 0)
            {
                playlistBox.SelectedIndex++;
            }
            
        }

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            selectNextSong();
            if (player.playState == WMPPlayState.wmppsPlaying)
            {
                startPlaying();
            }
        }
        

    

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            openImportDialog();



        }

        private void openImportDialog()
        {
            OpenFileDialog openFilePopup = new OpenFileDialog();
            openFilePopup.Multiselect = true;
            if (openFilePopup.ShowDialog() == true)
            {
                songFileName = openFilePopup.SafeFileNames;
                songFilePath = openFilePopup.FileNames;

                for(int i= 0; i < songFilePath.Length; i++)
                {
                    Track track = new Track();
                    track.fileName = System.IO.Path.GetFileNameWithoutExtension(songFileName[i]);
                    track.filePath = songFilePath[i];
                    var tag = TagLib.File.Create(track.filePath);
                    track.artist = tag.Tag.FirstPerformer;
                    track.duration = tag.Properties.Duration;
                    track.genre = tag.Tag.FirstGenre;
                    track.album = tag.Tag.Album;
                    track.songName = tag.Tag.Title;
                    if (musicLibrary.ContainsKey(track.fileName) != true)
                    {
                        musicLibrary.Add(track.fileName.ToString(), track);
                        playlistBox.Items.Add(track.fileName.ToString());
                        WMPLib.IWMPMedia media;
                        media = player.newMedia(track.filePath);
                    }
                    
                }

            }
        }

        private void pauseSong()
        {

            player.controls.pause();
            isPlayingSong = false;
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if
                (player.playState == WMPPlayState.wmppsPlaying)
            {
                pauseSong();
                swapPauseToPlayButton();

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
                player.settings.setMode("shuffle", true);
                isShuffle = true;
                




            }

        }
        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            toggleShuffle();
            
           
        }



        private void trackProgressSlider_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (player.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                double secondsOfSongEqualToOnePercent = (double)player.currentMedia.duration / 100;
                
                player.controls.currentPosition = trackProgressSlider.Value * secondsOfSongEqualToOnePercent;

                
          

                
            }
            trackProgressBar.Value = trackProgressSlider.Value;


        }

        private void volumeSlider_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double volume = volumeSlider.Value;
            player.settings.volume = Convert.ToInt32(volume);
            
        }

        private void playlistBox_KeyDown(object sender, KeyEventArgs e)
        {
            selectPreviousSong();
        }

        private void playlistBox_KeyUp(object sender, KeyEventArgs e)
        {
            selectNextSong();
        }

  

        private void playlistBox_Clicked(object sender, RoutedEventArgs e)
        {
            selectedFile = playlistBox.SelectedItem.ToString();
        }

  

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(mainWindow.Width <= mediumResponsiveWindowSize)
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

        private void trackProgressSlider_MouseEnter(object sender, MouseEventArgs e)
        {
            trackProgressSlider.Opacity = 0.0;
        }

        private void trackProgressSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            trackProgressSlider.Opacity = 0;
        }
        private void setTrackDurationLabel()
        {
            trackDurationLabel.Content = string.Format("{0:mm.ss}", player.currentMedia.durationString);


        }

        private void playlistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            startPlaying();
            
        }

        private void playlistBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            startPlaying();
            
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            

            //disables play button if nothing is selected
            if (playlistBox.SelectedIndex < 0)
            {
                return;
            }
            //runs if something is selected
            else
            {
                try
                {
                    loadSongIntoPlayer(getFilePathFromSelectedFile(selectedFile, musicLibrary)); //searches the dictionary musicLibrary for an item with key selectedFile
                    //this checks to makes sure something actually loaded into the player
                    if (player.URL == songFilePath[playlistBox.SelectedIndex].ToString())
                    {
                        player.controls.play();
                        isPlayingSong = true; // wont be needed once refactor to playState
                        swapPlayToPauseButton();
                    }
                    else
                    {
                        if (isPlayingSong == false)
                        {
                            if (player.URL == songFilePath[playlistBox.SelectedIndex].ToString())
                            {
                                player.controls.play();
                                isPlayingSong = true;
                                swapPlayToPauseButton();
                            }
                            else
                            {
                                startPlaying();
                                swapPlayToPauseButton();
                            }

                        }
                        else if (player.playState == WMPPlayState.wmppsPlaying)
                        {
                            swapPauseToPlayButton();
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("How dare you") ;
                }
                
            }
            
           

        }
    }
}
