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
using System.IO;
using System.Text;
using System.Windows.Media.Animation;
using System.Linq;
using System.Windows.Data;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media.Imaging;




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
        //private void setTitleBarDragging()
        //{
        //    SourceInitialized += (s, e) =>
        //    {
        //        IntPtr handle = (new WindowInteropHelper(this)).Handle;
        //        HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowProc));
        //    };

        //}


        //private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    switch (msg)
        //    {
        //        case 0x0024:
        //            WmGetMinMaxInfo(hwnd, lParam);
        //            handled = true;
        //            break;
        //    }
        //    return (IntPtr)0;
        //}
        //private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        //{
        //    MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
        //    int MONITOR_DEFAULTTONEAREST = 0x00000002;
        //    IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
        //    if (monitor != IntPtr.Zero)
        //    {
        //        MONITORINFO monitorInfo = new MONITORINFO();
        //        GetMonitorInfo(monitor, monitorInfo);
        //        RECT rcWorkArea = monitorInfo.rcWork;
        //        RECT rcMonitorArea = monitorInfo.rcMonitor;
        //        mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
        //        mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
        //        mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
        //        mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
        //    }
        //    Marshal.StructureToPtr(mmi, lParam, true);
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //public struct POINT
        //{
        //    /// <summary>x coordinate of point.</summary>
        //    public int x;
        //    /// <summary>y coordinate of point.</summary>
        //    public int y;
        //    /// <summary>Construct a point of coordinates (x,y).</summary>
        //    public POINT(int x, int y)
        //    {
        //        this.x = x;
        //        this.y = y;
        //    }
        //}
        //[StructLayout(LayoutKind.Sequential)]
        //public struct MINMAXINFO
        //{
        //    public POINT ptReserved;
        //    public POINT ptMaxSize;
        //    public POINT ptMaxPosition;
        //    public POINT ptMinTrackSize;
        //    public POINT ptMaxTrackSize;
        //};

        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        //public class MONITORINFO
        //{
        //    public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        //    public RECT rcMonitor = new RECT();
        //    public RECT rcWork = new RECT();
        //    public int dwFlags = 0;
        //}

        //[StructLayout(LayoutKind.Sequential, Pack = 0)]
        //public struct RECT
        //{
        //    public int left;
        //    public int top;
        //    public int right;
        //    public int bottom;
        //    public static readonly RECT Empty = new RECT();
        //    public int Width { get { return Math.Abs(right - left); } }
        //    public int Height { get { return bottom - top; } }
        //    public RECT(int left, int top, int right, int bottom)
        //    {
        //        this.left = left;
        //        this.top = top;
        //        this.right = right;
        //        this.bottom = bottom;
        //    }
        //    public RECT(RECT rcSrc)
        //    {
        //        left = rcSrc.left;
        //        top = rcSrc.top;
        //        right = rcSrc.right;
        //        bottom = rcSrc.bottom;
        //    }
        //    public bool IsEmpty { get { return left >= right || top >= bottom; } }
        //    public override string ToString()
        //    {
        //        if (this == Empty) { return "RECT {Empty}"; }
        //        return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
        //    }
        //    public override bool Equals(object obj)
        //    {
        //        if (!(obj is Rect)) { return false; }
        //        return (this == (RECT)obj);
        //    }
        //    /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
        //    public override int GetHashCode() => left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
        //    /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
        //    public static bool operator ==(RECT rect1, RECT rect2) { return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom); }
        //    /// <summary> Determine if 2 RECT are different(deep compare)</summary>
        //    public static bool operator !=(RECT rect1, RECT rect2) { return !(rect1 == rect2); }
        //}

        //[DllImport("user32")]
        //internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        //[DllImport("User32")]
        //internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        ////above is for proper borderless window    

        //public class SliderTools : DependencyObject
        //{
        //    public static bool GetMoveToPointOnDrag(DependencyObject obj) { return (bool)obj.GetValue(MoveToPointOnDragProperty); }
        //    public static void SetMoveToPointOnDrag(DependencyObject obj, bool value) { obj.SetValue(MoveToPointOnDragProperty, value); }
        //    public static readonly DependencyProperty MoveToPointOnDragProperty = DependencyProperty.RegisterAttached("MoveToPointOnDrag", typeof(bool), typeof(SliderTools), new PropertyMetadata
        //    {
        //        PropertyChangedCallback = (obj, changeEvent) =>
        //        {
        //            var slider = (Slider)obj;
        //            if ((bool)changeEvent.NewValue)
        //                slider.MouseMove += (obj2, mouseEvent) =>
        //                {
        //                    if (mouseEvent.LeftButton == MouseButtonState.Pressed)
        //                        slider.RaiseEvent(new MouseButtonEventArgs(mouseEvent.MouseDevice, mouseEvent.Timestamp, MouseButton.Left)
        //                        {
        //                            RoutedEvent = UIElement.PreviewMouseLeftButtonDownEvent,
        //                            Source = mouseEvent.Source,
        //                        });
        //                };
        //        }
        //    });
        //}






        //----------------------------------------------------------------------------------//     //----------------------------------------------------------------------------------//        //----------------------------------------------------------------------------------//

















        //-------------------------------Start Logic-------------------------------------------//









        int mediumResponsiveWindowSize = 570;

        UnderwaterAudioMediaPlayer player = new UnderwaterAudioMediaPlayer();     
        
        string supportedMusicFileTypes = UnderwaterAudioMediaPlayer.supportedMusicFileTypes;

        ListBox playlistBox = new ListBox();
        Random random = new Random();


        private void createMediaEventHandlers()
        {            
            player.MediaOpened += new EventHandler(player_MediaOpened);
            player.MediaEnded += new EventHandler(player_MediaEnded);
            
        }

        private void createTimerEventTickAndSetInterval()
        {
            player.timer.Tick += new EventHandler(timer_Tick);
            player.timer.Interval = TimeSpan.FromMilliseconds(200);
        }
        public int timer1TickCount;
        


        public MainWindow()
        {
            InitializeComponent();
            createMediaEventHandlers();
            createTimerEventTickAndSetInterval();
           




        }



        public void setColumnNames()
        {
                      
            libraryListView.Columns[0].Header = "Name";
            libraryListView.Columns[1].Header = "Artist";
            libraryListView.Columns[2].Header = "Album";
            libraryListView.Columns[3].Header = "Genre";
            

        }
 

        //Event Handlers//
        private void player_MediaEnded(object sender, EventArgs e)
        {
            if (player.isRepeat)
            {
                player.loadSongIntoPlayer(player.currentMedia);
                startPlaying();
            }
            else if (player.isShuffle)
            {
                selectShuffleSong();
                startPlaying();
            }
            else { 
                    if(player.currentPlaylistIndex < player.mediaLibrary.Count -1)
                    {
                        player.currentPlaylistIndex++;
                        player.Position = new System.TimeSpan(0);
                        player.pausedPosition = new System.TimeSpan(0);
                        player.loadSongIntoPlayer(player.currentPlaylist[player.currentPlaylistIndex]);               
                        startPlaying();
                        
                    }
                    else if (player.currentPlaylistIndex == player.mediaLibrary.Count-1)
                    {
                    stopPlaying();
                    
                    }

                    
                }

        }


        private void player_MediaOpened(object sender, EventArgs e)
        {
            String trackName = player.currentMedia.fileName;
            trackProgressSlider.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
            trackDurationLabel.Content = player.NaturalDuration.TimeSpan.ToString(@"m\:ss");
            nowPlayingLabel.Content = player.currentMedia.fileName;
            player.Position = new TimeSpan(0);
            foreach(Grid icon in playlistListView.Children)
            {
                highlightFunction(icon, playlistListView);

            }


        }

        private void timer_Tick(object sender, EventArgs e)
        {
            
            timeElapsedLabel.Content = player.Position.ToString(@"m\:ss");
            
            if (player.NaturalDuration.HasTimeSpan)
            {
                trackProgressBar.Value = (player.Position.TotalSeconds / player.NaturalDuration.TimeSpan.TotalSeconds) * 100;

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

        private void handleShuffleClick()
        {
            toggleShuffle();
            if (player.isRepeat)
            {
                toggleRepeat();
            }   

        }
       

        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            handleShuffleClick();


        }


        private void trackProgressSlider_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
    
            if (player.Source!= null)
            {
                if(player.NaturalDuration != Duration.Automatic)
                {
                    
                    player.Position = TimeSpan.FromSeconds(trackProgressSlider.Value);
                    trackProgressBar.Value = player.Position.TotalSeconds / player.NaturalDuration.TimeSpan.TotalSeconds;
                    player.pausedPosition = TimeSpan.FromSeconds(trackProgressSlider.Value);
                    
                }
                
            }

        }

        private void volumeSlider_Change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            double playerVolume = volumeSlider.Value / 100;
            player.Volume = playerVolume;


        }

        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            libraryListView.MaxWidth = getWindowSize();
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


        //-------------------------------------------------------------------------------//
       //--------------------Functions--------------------------------------------------//


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
        
        private void startPlaying()
        {         
           if(player.currentMedia.filePath != null)
            {
                if (player.Source != new Uri(player.currentMedia.filePath))
                {
                    if (player.isShuffle)
                    {
                        selectShuffleSong();
                        player.Position = new TimeSpan(0);
                        player.pausedPosition = new TimeSpan(0);
                    }
                    else
                    {
                        player.loadSongIntoPlayer(player.currentPlaylist[player.currentPlaylistIndex]);
                        player.Position = new TimeSpan(0);
                        player.pausedPosition = new TimeSpan(0);
                    }

                    



                }
                else 
                {
                    player.Position = player.pausedPosition;
                    player.play();
                    swapPlayToPauseButton();
                    player.timer.Start();
                }
                

            }
            else
            {
                if (player.isShuffle)
                {
                    selectShuffleSong();
                    player.stop();
                    player.play();
                    swapPlayToPauseButton();
                }
                else
                {
                    if(player.selectedTrack != null)
                    {
                        player.loadSongIntoPlayer(player.selectedTrack);
                        player.stop();
                        player.play();
                        swapPlayToPauseButton();
                    }
                   
                }

               
            }
              
                
        }

        private void pausePlaying()
        {
            player.pause();
            player.pausedPosition = player.Position;

            swapPauseToPlayButton();
            player.timer.Stop();
        }

       

        //private Track searchForSelectedFile(string itemName, Dictionary<string, Track> list)
        //{
        //    if (itemName != null && list.ContainsKey(itemName.ToString()))
        //    {
        //        return list[itemName];

        //    }
        //    return null;
        //}

        private void rewindButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (player.isShuffle)
            {
                selectShuffleSong();
            }
            else if (player.Position > new TimeSpan(0, 0, 0, 1, 550))
            {               
                    startPlaying();           
            }
            else
            {
                selectPreviousSong();
            }


        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            closeApp();
        }
        private void stopPlaying()
        {
            player.stop();
            player.playState = UnderwaterAudioMediaPlayer.playerStopped;
            swapPauseToPlayButton();
        }

        private void selectShuffleSong()
        {
            int nextSongindex = random.Next(player.currentPlaylist.Count - 1);
            player.loadSongIntoPlayer(player.currentPlaylist[nextSongindex]);
            player.currentPlaylistIndex = nextSongindex;
            startPlaying();
           
            if (player.playState != UnderwaterAudioMediaPlayer.playerPlaying)
            {
                stopPlaying();
            }
 
           
        }





        private void selectNextSong()
        {
          

                       
                
                if(player.playState == UnderwaterAudioMediaPlayer.playerPlaying)
                {
                    stopPlaying();
                    if(player.currentPlaylistIndex < player.currentPlaylist.Count - 1)
                    {
                        player.currentPlaylistIndex++;
                        player.loadSongIntoPlayer(player.currentPlaylist[player.currentPlaylistIndex]);
                        startPlaying();
                    }
                   
                }
                else if(player.playState == UnderwaterAudioMediaPlayer.playerStopped || player.playState == UnderwaterAudioMediaPlayer.playerPaused)
                {
                    stopPlaying();
                    if (player.currentPlaylistIndex < player.currentPlaylist.Count - 1)
                    {
                        player.currentPlaylistIndex++;
                        player.loadSongIntoPlayer(player.currentPlaylist[player.currentPlaylistIndex]);
                   
                    }
                    
                }
               
           

        }

        private void selectPreviousSong()
        {
           

                
                    stopPlaying();           
            if (player.currentPlaylistIndex > 0)
            {
                player.currentPlaylistIndex--;
                player.loadSongIntoPlayer(player.currentPlaylist[player.currentPlaylistIndex]);



                if (player.playState == UnderwaterAudioMediaPlayer.playerPlaying)
                {
                    startPlaying();

                }

                else
                {
                    player.loadSongIntoPlayer(player.currentPlaylist[player.currentPlaylistIndex]);
                    startPlaying();
                }

            }
              


            

        }





        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.isShuffle)
            {
                selectShuffleSong();
            }
            else
            {
                selectNextSong();
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
            openFilePopup.Filter = supportedMusicFileTypes;
            //openFilePopup.Filter = "Music |*.mp3;*.wav";
            if (openFilePopup.ShowDialog() == true)
            {

                string[] songFileName = openFilePopup.SafeFileNames;
               string[] songFilePath = openFilePopup.FileNames;

                for(int i= 0; i < songFilePath.Length; i++)
                {
                    Track track = new Track();
                    getTrackTags(track, i,songFileName,songFilePath);
                    Dictionary<string, Track> dic = new Dictionary<string, Track>();
                    foreach(Track song in player.mediaLibrary)
                    {
                        if(dic.ContainsKey(song.filePath) != true)
                        {
                            dic.Add(song.filePath, song);

                        }
                    }

                    if(dic.ContainsKey(track.filePath) != true)
                    {
                        player.mediaLibrary.Add(track);
                        

                    }

                }

            }
            //libraryListView.ItemsSource = null;
            //libraryListView.ItemsSource = player.mediaLibrary;
            
            
        }
        

        private void getTrackTags(Track track, int i,string[] songFileName, string[] songFilePath)
        {
            track.fileName = System.IO.Path.GetFileNameWithoutExtension(songFileName[i]);
            track.filePath = songFilePath[i];
            var tag = TagLib.File.Create(track.filePath);
            track.artist = tag.Tag.FirstPerformer;
            track.duration = tag.Properties.Duration;
            track.genre = tag.Tag.FirstGenre;
            track.album = tag.Tag.Album;
            track.songName = tag.Tag.Title;
            setTrackAlbumArt(track);

            
        }


        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if
                (player.playState == UnderwaterAudioMediaPlayer.playerPlaying)
            {
                pausePlaying();

            }            
        }
      
        private void swapShuffleIcons()
        {
            if (player.isShuffle)
            {
                shuffleButton.Visibility = Visibility.Visible;
                shuffleButtonOn.Visibility = Visibility.Collapsed;
            }
            else if(player.isShuffle == false)
            {
                shuffleButton.Visibility = Visibility.Collapsed;
                shuffleButtonOn.Visibility = Visibility.Visible;
            }
            
        }
        private void toggleShuffle()
        {
            if(player.isShuffle)
            {
                swapShuffleIcons();
                player.isShuffle = false;
                
                

            }
            else if(player.isShuffle == false)
            {

                swapShuffleIcons();
         
                player.isShuffle = true;
                




            }

        }
        private void toggleRepeat()
        {
            if (player.isRepeat)
            {
                
                player.isRepeat = false;
                repeatButton.Visibility = Visibility.Visible;
                repeatButtonOn.Visibility = Visibility.Collapsed;

            }
            else if (player.isRepeat != true)
            {
                
                player.isRepeat = true;
                repeatButton.Visibility = Visibility.Collapsed;
                repeatButtonOn.Visibility = Visibility.Visible;


            }

        }
        private void setTrackPositionFromSlider()
        {
            player.Position = TimeSpan.FromSeconds(trackProgressSlider.Value);
            trackProgressBar.Value = player.Position.TotalSeconds / player.NaturalDuration.TimeSpan.TotalSeconds;
            player.pausedPosition = TimeSpan.FromSeconds(trackProgressSlider.Value);
        }


        private void handlePlayButtonClick()
        {
            
            startPlaying();          
            
        }


        private void Slider_OnMouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                player.isScrubbing = true;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var slider = (Slider)sender;
                System.Windows.Point position = e.GetPosition(slider);
                double d = 1.0d / slider.ActualWidth * position.X;
                var p = slider.Maximum * d;
                slider.Value = p;
                if(player.Source != null && player.NaturalDuration != Duration.Automatic)
                {
                    trackProgressBar.Value = trackProgressBar.Value = (player.Position.TotalSeconds / player.NaturalDuration.TimeSpan.TotalSeconds) * 100;
                }
                else
                {
                    trackProgressSlider.Maximum = 100;
                    trackProgressBar.Value = trackProgressSlider.Value;
                }
            }
        }

        private void savePlaylist(string playlistName, List<Track> list)
        {

            System.IO.StreamWriter playlist = new System.IO.StreamWriter(playlistName + ".plst");

            foreach (var track in list)
            {
                playlist.WriteLine(track.filePath);
            }
            playlist.Close();

        }

        private void savePlaylist(string playlistName)
        {

            System.IO.StreamWriter playlist = new System.IO.StreamWriter(playlistName + ".plst");
           
            foreach (var track in player.mediaLibrary)
            {                 
                playlist.WriteLine(track.filePath);
            }
            playlist.Close();

        }

        private void closeApp()
        {
            savePlaylist("library");
            foreach(Playlist playlist in player.playlistCollection)
            {
                savePlaylist(playlist.Name, playlist.playlist);
            }        
            Close();
            
           

        }
        private void showDelphinPage()
        {
            libraryPage.Visibility = Visibility.Collapsed;
            playlistPage.Visibility = Visibility.Collapsed;
            delphinPage.Visibility = Visibility.Visible;
            tabTitleLabel.Content = "Delphin";
            tabTitleLabel.Visibility = Visibility.Visible;
            slideRightToLeftAnimation(titleLabelBar);
        }
        private void delphinNavTab_Click(object sender, RoutedEventArgs e)
        {
            showDelphinPage();
        }
        private void slideRightToLeftAnimation(UIElement element)
        {
            element.Visibility = Visibility.Visible;
            TranslateTransform trans = new TranslateTransform();
            element.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(mainWindow.ActualWidth, 0, TimeSpan.FromMilliseconds(350));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            
        }
        
    private void showLibraryPage()
        {
            libraryPage.Visibility = Visibility.Visible;
            delphinPage.Visibility = Visibility.Collapsed;
            playlistPage.Visibility = Visibility.Collapsed;
            tabTitleLabel.Content = "Library";
            slideRightToLeftAnimation(titleLabelBar);
            for (int i = 0; i < playlistListView.Children.Count; i++)
            {
                Grid grid = (Grid)playlistListView.Children[i];
                for (int j = 0; j < grid.Children.Count; j++)
                {
                    grid.Children.RemoveAt(j);
                  
                }
                playlistListView.Children.RemoveAt(i);


            }
            playlistListView.Children.Clear();
        






        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            showLibraryPage();
            //foreach (Grid item in playlistListView.Children)
            //{
            //    item.Children.RemoveRange(0, playlistListView.Children.Count);
            //}
        }

       
        private void repeatButton_Click(object sender, RoutedEventArgs e)
        {
            toggleRepeat();
            if (player.isShuffle)
            {
                toggleShuffle();
            }
        }
 
        private void showPlaylistPage()
        {
            libraryPage.Visibility = Visibility.Collapsed;
            delphinPage.Visibility = Visibility.Collapsed;
            playlistPage.Visibility = Visibility.Visible;
            tabTitleLabel.Content = "Playlists";
            slideRightToLeftAnimation(titleLabelBar);
           

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            showPlaylistPage();
        }

       private void deleteTrack()
        {
            playlistListView.Children.Clear();
        }


        public void removeSongFromLibrary()
        {

            if(player.mediaLibrary.Count > 0 && libraryListView.SelectedIndex >= 0)
            {
                int previousIndex = libraryListView.SelectedIndex;
                player.mediaLibrary.Remove(player.mediaLibrary[previousIndex]);
                libraryListView.ItemsSource = null;
                libraryListView.ItemsSource = player.mediaLibrary;


                if (previousIndex < libraryListView.Items.Count - 1)
                {

                    libraryListView.SelectedIndex = previousIndex++;

                }
                else
                {
                    if (libraryListView.SelectedIndex + 1 < player.mediaLibrary.Count)
                    {
                        libraryListView.SelectedIndex = player.mediaLibrary.Count - 1;
                    }
                }

            }

            
           
            
        }

        private void setTrackAlbumArt(Track track)
        {
            TagLib.File tag = TagLib.File.Create(track.filePath);

            if(tag.Tag.Pictures.FirstOrDefault() != null)
            {
                var pic = tag.Tag.Pictures.FirstOrDefault();
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();

                track.albumArt = bitmap;
            }
            else
            {
                track.albumArt = null;
            }
          
        }
        private Image getRandomImageFromWeb()
        {
            Image image = new Image();

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("https://picsum.photos/200");
            bitmapImage.EndInit();
            image.Source = bitmapImage;
            image.Width = 60;
            image.Height = 60;
            image.SetValue(Grid.RowProperty, 0);
            image.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            image.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            image.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));
            return image;
        }

        private Image getAlbumImageFromTag(Track track)
        {
            Image image = new Image();
            image.Source = track.albumArt;
            
            image.Width = 60;
            image.Height = 60;
            image.SetValue(Grid.RowProperty, 0);
            image.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            image.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            image.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));
            return image;
        }

        private Grid createAlbumIcon()
        {
            Grid albumStack = new Grid();
            albumStack.Width = 120;
            albumStack.VerticalAlignment = VerticalAlignment.Center;
            albumStack.HorizontalAlignment = HorizontalAlignment.Center;
            albumStack.MinHeight = 120;
            albumStack.Margin = new Thickness(10, 10, 10, 10);
            Border border = new Border();
            border.CornerRadius = new CornerRadius(14);
            border.Background = new SolidColorBrush(Color.FromArgb(100, 0, 100, 200));
            border.Width = 120;
            border.Height = 120;
            albumStack.Children.Add(border);

            return albumStack;
        }
        private void addAlbumThumbnail(Playlist playlist, WrapPanel wrapPanel)
        {
            Grid albumStack = createAlbumIcon();
            albumStack.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            albumStack.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            albumStack.Margin = new Thickness(10, 10, 10, 10);
            albumStack.Children.Add(new TextBlock() { Text = playlist.Name, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 16, TextTrimming = TextTrimming.CharacterEllipsis }); ;
            
            
            //image.Width = 60;
            //image.Height = 60;
            //image.SetValue(Grid.RowProperty, 0);
            //image.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            //image.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            //image.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));

            Playlist selectedItem = playlist;
            var firstTrack = selectedItem.playlist.FirstOrDefault();
            BitmapImage image = firstTrack.albumArt;

            if (firstTrack.albumArt != null)
            {
                
                Image albumImage = new Image();
                albumImage.Source = firstTrack.albumArt;
                albumImage.Width = 60;
                albumImage.Height = 60;
                albumImage.SetValue(Grid.RowProperty, 0);
                albumImage.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                albumImage.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                albumImage.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));
                albumStack.Children.Add(albumImage);
            }
            else
            {
                albumStack.Children.Add(getRandomImageFromWeb());

            }
              
            
            wrapPanel.Children.Add(albumStack);
            foreach (Grid playlistIcon in wrapPanel.Children)
            {
                playlistIcon.MouseEnter += PlaylistIcon_MouseEnter;
                playlistIcon.MouseLeave += PlaylistIcon_MouseLeave;
                playlistIcon.MouseDown += PlaylistIcon_MouseDown;

                

            }
        }

        private void PlaylistIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {







            Grid panel = (Grid)sender;
            var albumArtImage = panel.Children[2];
            TextBlock nameText = (TextBlock)panel.Children[1];
            var border = panel.Children[0];
            foreach (Grid icon in playlistView.Children)
            {
                highlightFunction(icon, playlistView);

            }
            panel.Background = new SolidColorBrush(Color.FromArgb(200, 100, 100, 200));
            Playlist playlist = player.playlistCollection.Where(playlists => playlists.Name == nameText.Text).ToList().FirstOrDefault();
            if (playlist == null)
            {
                playlist = new Playlist();
                playlist.playlist = player.mediaLibrary;
            }
            player.selectedPlaylist = playlist;
       






            //double click activates playing
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
              
                
                player.currentPlaylist = playlist.playlist;
                foreach(Track track in player.currentPlaylist)
                {
                    
                    addTrackThumbnail(track, playlistListView);
                    //addTrackThumbnail();

                }
                showPlaylistPage();
                foreach (Grid icon in playlistListView.Children)
                {
                    highlightFunction(icon, playlistListView);

                }



            }

        }

        private void PlaylistIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            iconHoverEffectMouseLeave(sender);
        }

        private void iconHoverEffect(object sender)
        {

            Grid panel = (Grid)sender;
            var albumArtImage = panel.Children[2];
            var nameText = panel.Children[1];
            var border = panel.Children[0];
            
            albumArtImage.RenderTransform = new ScaleTransform(scaleX: 1.2, scaleY: 1.2);
            albumArtImage.RenderTransformOrigin = new Point(x: .5, y: .5);

            
         






        }


        private void iconHoverEffectMouseLeave(object sender)
        {
            Grid panel = (Grid)sender;
            var albumArtImage = panel.Children[2];
            var nameText = panel.Children[1];
            var border = panel.Children[0];

            albumArtImage.RenderTransform = new ScaleTransform(scaleX: 1, scaleY: 1);
            nameText.SetValue(Grid.RowSpanProperty, 1);

            

        }

        private void PlaylistIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            iconHoverEffect(sender);
        }

        private void addTrackThumbnail(Track track, WrapPanel wrapPanel)
        {

            Grid albumStack = createAlbumIcon();
            albumStack.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            albumStack.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            albumStack.Margin = new Thickness(10, 10, 10, 10);       


            var selectedItem = track;
            albumStack.Children.Add(new TextBlock() { Text = selectedItem.fileName, VerticalAlignment = VerticalAlignment.Top, FontSize = 16 });


            if (selectedItem.albumArt != null)
            {

                Image albumImage = new Image();
                  albumImage.Source =  selectedItem.albumArt;
                albumImage.Width = 60;
                albumImage.Height = 60;
                albumImage.SetValue(Grid.RowProperty, 0);
                albumImage.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                albumImage.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                albumImage.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));
                albumStack.Children.Add(albumImage);
            }
            else
            {
                albumStack.Children.Add(getRandomImageFromWeb());

            }




          
            

                

                wrapPanel.Children.Add(albumStack);
                foreach (Grid element in wrapPanel.Children)
                {
                    element.MouseEnter += Element_MouseEnter;
                    element.MouseLeave += Element_MouseLeave;
                    element.MouseDown += Element_MouseDown;

                }
            }

            private void addTrackThumbnail()
        {
            foreach(Track track in player.mediaLibrary)
            {
                Grid albumStack = createAlbumIcon();
                albumStack.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                albumStack.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                albumStack.Margin = new Thickness(10, 10, 10, 10);
                albumStack.Children.Add(new TextBlock() { Text = track.fileName, VerticalAlignment = VerticalAlignment.Top,HorizontalAlignment = HorizontalAlignment.Center, FontSize = 16, TextTrimming = TextTrimming.CharacterEllipsis }); ;

                Image image = getRandomImageFromWeb();
                //image.Width = 60;
                //image.Height = 60;
                //image.SetValue(Grid.RowProperty, 0);
                //image.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                //image.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                //image.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));

                

                var selectedItem = track;          


                if(track.albumArt != null)
                {
                    Image albumImage = new Image();
                        albumImage.Source  = track.albumArt;
                    albumImage.Width = 60;
                    albumImage.Height = 60;
                    albumImage.SetValue(Grid.RowProperty, 0);
                    albumImage.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                    albumImage.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    albumImage.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));
                    albumStack.Children.Add(albumImage);
                }
                else
                {
                    albumStack.Children.Add(image);

                }



                
                playlistView.Children.Add(albumStack);

               
               albumStack.MouseEnter += Element_MouseEnter;
               albumStack.MouseLeave += Element_MouseLeave;
               albumStack.MouseDown += Element_MouseDown;

                //foreach (StackPanel element in playlistView.Children)
                //{
                //    element.MouseEnter += Element_MouseEnter;
                //    element.MouseLeave += Element_MouseLeave;
                //    element.MouseDown += Element_MouseDown;

                //}
            }          


           
        }

        private void MainWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void addToPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.selectedTrack != null)
            {
                playlistBox.Items.Add(player.selectedTrack.fileName);
            }
            if(playlistBox.Items.Count > 0 && playlistPanel.Children.Count < 2)
            {
                Button button = new Button();
                button.Content = "Save Playlist";
                button.Click += Button_Click1;


                playlistPanel.Children.Add(button);


                playlistPanel.Children.Add(playlistBox);
            }
            


        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {

            List<Track> list = new List<Track>();
            foreach(String item in playlistBox.Items)
            {
                string fileName = item.ToString();
                     
                list.Add(player.mediaLibrary.Where(track => track.fileName == fileName).ToList().FirstOrDefault());
            }
            Playlist playlist = new Playlist();
            playlist.playlist = list;
            playlist.Name = String.Format("Playlist{0}", player.playlistCollection.Count);
            player.playlistCollection.Add(playlist);
            playlistBox.Items.Clear();
            if(list.Count <= 0)
            {

            }
            else
            {
                addAlbumThumbnail(playlist, playlistView);

            }
        }

       private void highlightFunction(Grid icon, WrapPanel panel)
        {


              var albumArtImage = icon.Children[2];
                TextBlock nameText = (TextBlock)icon.Children[1];
                var border = icon.Children[0];


                if (nameText.Text == player.currentMedia.fileName)
                {
                    icon.Background = new SolidColorBrush(Color.FromArgb(100, 0, 200, 200));

                }
                else
                {
                    icon.Background = new SolidColorBrush(Color.FromArgb(0, 0, 100, 200));
                }


        }
        private void highlightFunction(string name, WrapPanel panel)
        {

            Grid icon = (Grid)playlistListView.Children[player.currentPlaylistIndex];
            icon.Background = new SolidColorBrush(Color.FromArgb(0, 0, 100, 200));

            if (name == player.currentMedia.fileName)
            {
                icon.Background = new SolidColorBrush(Color.FromArgb(100, 0, 200, 200));

            }
           


        }

        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid panel = (Grid)sender;
            var albumArtImage = panel.Children[2];
            TextBlock nameText = (TextBlock)panel.Children[1];
            var border = panel.Children[0];
            foreach (Grid icon in playlistListView.Children)
            {
                highlightFunction(icon, playlistListView);

            }
            panel.Background = new SolidColorBrush(Color.FromArgb(200, 100, 100, 200));
            
            
            var selectedTrack = player.currentPlaylist.Where(track => track.fileName == nameText.Text).ToList().FirstOrDefault();
            player.selectedTrack = selectedTrack;
            player.currentPlaylistIndex = player.currentPlaylist.IndexOf(selectedTrack);

           







            //double click activates playing
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                    panel.Background = new SolidColorBrush(Color.FromArgb(100, 0, 200, 200));
                


                player.currentMedia = selectedTrack;
                if (player.Source != new Uri(player.currentMedia.filePath))
                {
                    player.loadSongIntoPlayer(selectedTrack);

                    startPlaying();
                }
                else
                {
                    startPlaying();
                }
             

            }
           







        }



      
       private void importSavedPlaylistFiles()
        {
            for (int i = 0; i < 20; i++)
            {
                    string playlistName = string.Format("Playlist{0}.plst", i.ToString());


                    if (File.Exists(playlistName))
                    {
                        string[] savedPlaylist = File.ReadAllLines(playlistName);
                        List<Track> playlist = new List<Track>();
                        foreach (string filePath in savedPlaylist)
                        {

                            playlist.Add(player.mediaLibrary.Where(track => track.filePath == filePath).ToList().FirstOrDefault());
                        }
                        Playlist playlist1 = new Playlist();
                        playlist1.playlist = playlist;
                        playlist1.Name = playlistName;
                        player.playlistCollection.Add(playlist1);
                       addAlbumThumbnail(playlist1, playlistListView);
                       
                    }

            }
        }



        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            iconHoverEffectMouseLeave(sender);
        }

        private void Element_MouseEnter(object sender, MouseEventArgs e)
        {
            iconHoverEffect(sender);
            

           

        }

 

        private Track[] convertDictionaryValuesToArray(Dictionary<String, Track> dictionary)
        {
            return (new List<Track>(dictionary.Values)).ToArray();
        }

        private String[] convertDictionaryKeysToArray(Dictionary<String, Track> dictionary)
        {
            return (new List<string>(dictionary.Keys)).ToArray();


        }

        private string pickRandomSong(List<Track> playlist)
        {
            Random random = new Random();

            return playlist[random.Next(playlist.Count)].ToString();

        }






        private void closeButtonBackground_MouseEnter(object sender, MouseEventArgs e)
        {
            var element = closeButtonBackground as Border;
            element.Background = new SolidColorBrush(Color.FromArgb(100, 250, 0, 0));
        }

        private void closeButtonBackground_MouseLeave(object sender, MouseEventArgs e)
        {
           
            var element = closeButtonBackground as Border;
            element.Background = null;
            closeButton.Width = 14;
            closeButton.Height = 14;
        }




        private void titleLabelBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            showLibraryPage();
        }


        private void closeButtonBackground_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            var element = closeButtonBackground as Border;
            element.Background = new SolidColorBrush(Color.FromArgb(100, 200, 0, 0));
            closeButton.Width = 10;
            closeButton.Height = 10;

        }

        private void closeButtonBackground_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            closeApp();
        }



        private void closeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            closeButton.Width = 10;
            closeButton.Height = 10;
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            
            handlePlayButtonClick();          

        }

        private void libraryListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if(libraryListView.SelectedItem != null)
            //{
            //    IEnumerable<Track> selectedItem = player.mediaLibrary.Where(track => track == libraryListView.SelectedItem);
            //    player.currentPlaylistIndex = libraryListView.SelectedIndex;
            //    player.loadSongIntoPlayer(selectedItem);
            //    player.currentPlaylistIndex = libraryListView.SelectedIndex;
            //    startPlaying();
            //}
           
        }

        private void libraryListView_Loaded(object sender, RoutedEventArgs e)
        {
            


            //libraryListView.ItemsSource = player.mediaLibrary;
            //libraryListView.Columns.RemoveAt(libraryListView.Items.Count -1);
            ////libraryListView.DataContext = player.mediaLibrary;
            //if(libraryListView.Items.Count > 0)
            //{
            //    libraryListView.SelectedIndex = 0;

            //}
            //setColumnNames();


        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            //removeSongFromLibrary();
            deleteTrack();
        }

        private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            showLibraryPage();
         
        }

        private void TabItem_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            showDelphinPage();
        }

        private void playlistView_Loaded(object sender, RoutedEventArgs e)
        {
            Playlist playlist = new Playlist();
            playlist.playlist = player.mediaLibrary;
            playlist.Name = "Library";
            addAlbumThumbnail(playlist, playlistView);
            //addTrackThumbnail();
        }

        private void playlistListView_Loaded(object sender, RoutedEventArgs e)
        {
            
            
            //importSavedPlaylistFiles();
            
        }

    }
    
}
