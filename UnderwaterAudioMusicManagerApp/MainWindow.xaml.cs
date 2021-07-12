using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Animation;
using System.Linq;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace UnderwaterAudioMusicManagerApp
{

    public partial class MainWindow : Window
    {
        //-------------------------------Start Logic-------------------------------------------//

        int mediumResponsiveWindowSize = 570;

        UnderwaterAudioMediaPlayer player = new UnderwaterAudioMediaPlayer();     
        
        string supportedMusicFileTypes = UnderwaterAudioMediaPlayer.supportedMusicFileTypes;
        ListBox playlistBox = new ListBox();
        Data data = new Data();
                        /* 
                   Main ---  #009BD8
                   2nd Blue -- #00B7DD
                   Blue Green-- #00CFCC
                   Green  -----#41E3AB
                   Light Green --- #A4F187
                   Yello --- #F9F871
                   */

                SolidColorBrush mainBlue = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#009BD8"));
                SolidColorBrush secondBlue = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#00B7DD"));
                SolidColorBrush blueGreen = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#00CFCC"));
                SolidColorBrush green = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#41E3AB"));
                SolidColorBrush lightGreen = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#A4F187"));
                SolidColorBrush yellow = new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#F9F871"));



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
        

        //---------------------------------------------MAIN INSTANTIATION----------------------------------------------------------------------------------------//
        public MainWindow()
        {
            InitializeComponent();
            createMediaEventHandlers();
            createTimerEventTickAndSetInterval();
            deserializeData();


            if (player.mediaLibrary.playlist != null)
            {               
                if(player.playlistCollection.Count > 0)
                {
                    if(player.playlistCollection[0].Name != "Library")
                    {
                        Playlist playlist = player.playlistCollection[0];
                        player.playlistCollection.Add(playlist);
                        player.playlistCollection[0] = player.mediaLibrary;
                    }
                    else
                    {
                        int index = player.playlistCollection.IndexOf(player.playlistCollection.Where(playlist => playlist.Name == "Library").ToList().FirstOrDefault());
                        player.playlistCollection[index] = player.mediaLibrary;
                    }
                 

                }
                else
                {
                    player.playlistCollection.Add(player.mediaLibrary);
                }
            }
            if(player.playlistCollection.Count > 0)
            {
                foreach (Playlist playlist in player.playlistCollection)
                {
                    if(playlist.playlist.Count == 0)
                    {
                        addNoPlaylistIcon(playlistView);
                    }
                    else
                    {
                        addAlbumThumbnail(playlist, playlistView);

                    }
                }
            }
            else
            {
                addNoPlaylistIcon(playlistView);
            }
         
            //File.Delete("library.plst");
            //importSavedPlaylistFiles();



        }



        //-----------------------------------------------------------Event Handlers-----------------------------------------------------------\\
        //-------------------------------------------------------------------------------------------------------------------------------------\\

        private void deserializeData()
        {
            if (File.Exists("data.xml"))
            {
                deserializeXmlData("data.xml");
                foreach(Playlist playlist in data.playlistCollection)
                {
                    foreach(Track track in playlist.playlist)
                    {
                        setTrackAlbumArt(track);
                    }
                }
                player.playlistCollection = data.playlistCollection;
                player.mediaLibrary = data.mediaLibrary;
            }
            else
            {
                player.mediaLibrary = new Playlist();
                player.mediaLibrary.playlist = new List<Track>();
                player.mediaLibrary.Name = "Library";
            }

       
        }



        private void player_MediaEnded(object sender, EventArgs e)
        {
            if (player.isRepeat)
            {
                player.loadSongIntoPlayer(player.currentMedia);
                startPlaying();
                player.selectedSongIndex = player.currentMediaIndex;

            }
            else if (player.isShuffle)
            {
                selectShuffleSong();
                startPlaying();
                player.selectedSongIndex = player.currentMediaIndex;

            }
            else { 
                    if(player.currentMediaIndex < player.currentPlaylist.Count -1)
                    {

                    selectNextSong();                       
                        player.Position = new System.TimeSpan(0);
                        player.pausedPosition = new System.TimeSpan(0);                        
                    startPlaying();
                    }             

                    
                }

        }

        private void player_MediaOpened(object sender, EventArgs e)
        {
            String trackName = player.currentMedia.fileName;
            trackDurationLabel.Content = player.NaturalDuration.TimeSpan.ToString(@"m\:ss");
            nowPlayingLabel.Content = player.currentMedia.fileName;
            trackProgressSlider.Maximum = player.NaturalDuration.TimeSpan.TotalSeconds;
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

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if
                (player.playState == UnderwaterAudioMediaPlayer.playerPlaying)
            {
                pausePlaying();

            }
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
        private void Slider_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
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
                if (player.Source != null && player.NaturalDuration != Duration.Automatic)
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
        private void delphinNavTab_Click(object sender, RoutedEventArgs e)
        {
            showDelphinPage();
        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (delphinPage.Visibility == Visibility.Visible)
            {
                //player.selectedPlaylist = player.playlistCollection.FirstOrDefault();
                showLibraryPage();

            }
            else
            {
                if (libraryPage.Visibility == Visibility.Collapsed)
                {
                    showLibraryPage();
                    //player.selectedPlaylist = player.playlistCollection.Where(playlists => playlists.playlist == player.currentPlaylist).ToList().FirstOrDefault();
                    foreach (Grid icon in playlistView.Children)
                    {
                        highlightFunction(icon, playlistView);
                    }


                }

            }
            
            
          



        }
        private void repeatButton_Click(object sender, RoutedEventArgs e)
        {
            toggleRepeat();
            if (player.isShuffle)
            {
                toggleShuffle();
            }
        }
        private void playlistButton_Click(object sender, RoutedEventArgs e)
        {
            showPlaylistPage();
        }
        private void NoMusicIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            openImportDialog();
            if(player.mediaLibrary.playlist != null)
            {
                if (player.mediaLibrary.playlist.Count > 0)
                {                                   
                    playlistView.Children.RemoveAt(0);
                    
                    addAlbumThumbnail(player.mediaLibrary, playlistView);
                }
            }
         

        }

        private void NoMusicIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            iconHoverEffectMouseLeave(sender);
        }

        private void NoMusicIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            iconHoverEffect(sender);
        }

        private void PlaylistIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            player.selectedPlaylistIndex = playlistView.Children.IndexOf((Grid)sender);
            player.selectedPlaylist = player.playlistCollection[player.selectedPlaylistIndex];


            Grid panel = (Grid)sender;
            var albumArtImage = panel.Children[2];
            TextBlock nameText = (TextBlock)panel.Children[1];
            Border border = (Border)panel.Children[0];

            //if(player.playlistCollection.Where(playlists => playlists.Name == nameText.Text).ToList().FirstOrDefault() == null)
            //{
            //    player.selectedPlaylist = player.mediaLibrary;
               
            //}
           
              
                 
    



            foreach (Grid icon in playlistView.Children)
            {
                highlightFunction(icon, playlistView);

            }

            //double click activates playing
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                 //player.currentMediaIndex = player.currentPlaylist.IndexOf(player.currentMedia);
                player.selectedSongIndex = player.currentMediaIndex;
               

                foreach (Track track in player.selectedPlaylist.playlist)
                {

                    addTrackThumbnail(track, playlistListView);


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


        //-----------------------------------------------------------Functions-----------------------------------------------------------\\
        //--------------------------------------------------------------------------------------------------------------------------------\\



        //-----------------------------------player functions-----------------------------------\\

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
        private void swapShuffleIcons()
        {
            if (player.isShuffle)
            {
                shuffleButton.Visibility = Visibility.Visible;
                shuffleButtonOn.Visibility = Visibility.Collapsed;
            }
            else if (player.isShuffle == false)
            {
                shuffleButton.Visibility = Visibility.Collapsed;
                shuffleButtonOn.Visibility = Visibility.Visible;
            }

        }
        private void startPlaying()
        {         
           if(player.currentMedia != null && player.currentMedia.filePath != null)
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
                        player.loadSongIntoPlayer(player.currentPlaylist[player.selectedSongIndex]);
                        player.currentMediaIndex = player.currentPlaylist.IndexOf(player.currentPlaylist[player.selectedSongIndex]);
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
                    player.stop();

                    selectShuffleSong();
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
                        player.currentPlaylist = player.selectedPlaylist.playlist;
                        player.currentMediaIndex = player.currentPlaylist.IndexOf(player.selectedTrack);

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
        private void stopPlaying()
        {
            player.stop();
            player.playState = UnderwaterAudioMediaPlayer.playerStopped;
            swapPauseToPlayButton();
        }
        private void selectShuffleSong()
        {
            Random random = new Random();
            int nextSongindex = random.Next((player.currentPlaylist.Count -1));
            
            player.currentMediaIndex = nextSongindex;
            player.selectedSongIndex = player.currentMediaIndex;


            if (player.playState == UnderwaterAudioMediaPlayer.playerPlaying)
            {
                player.loadSongIntoPlayer(player.currentPlaylist[nextSongindex]);

                startPlaying();

            }
            else
            {
                player.loadSongIntoPlayer(player.currentPlaylist[nextSongindex]);

            }

        }
        private void selectNextSong()
        {

            if (player.currentMediaIndex < player.currentPlaylist.Count - 1)
            {
                player.currentMediaIndex++;
                if (player.playState == UnderwaterAudioMediaPlayer.playerPlaying)
                {
                    player.loadSongIntoPlayer(player.currentPlaylist[player.currentMediaIndex]);

                    startPlaying();
                }
                else
                {
                    player.loadSongIntoPlayer(player.currentPlaylist[player.currentMediaIndex]);

                    stopPlaying();
                }
                player.selectedSongIndex = player.currentMediaIndex;

            }
            else
            {
                stopPlaying();
            }



        }
        private void selectPreviousSong()
        {
            if (player.currentMediaIndex > 0)
            {
                player.currentMediaIndex--;
                if (player.playState == UnderwaterAudioMediaPlayer.playerPlaying)
                {
                    player.loadSongIntoPlayer(player.currentPlaylist[player.currentMediaIndex]);

                    startPlaying();
                }
                else
                {
                    player.loadSongIntoPlayer(player.currentPlaylist[player.currentMediaIndex]);

                    stopPlaying();
                }

            }
            else
            {
                stopPlaying();
            }
            player.selectedSongIndex = player.currentMediaIndex;


        }
        private void openImportDialog()
        {
            OpenFileDialog openFilePopup = new OpenFileDialog();
            openFilePopup.Multiselect = true;
            openFilePopup.Filter = supportedMusicFileTypes;
            //openFilePopup.Filter = "Music |*.mp3;*.wav";
            if (openFilePopup.ShowDialog() == true)
            {
                List<string> ignoreList = new List<string>();
                if(player.mediaLibrary.playlist != null)
                {
                    foreach (Track track1 in player.mediaLibrary.playlist)
                    {
                        ignoreList.Add(track1.filePath);
                    }
                }
                else
                {
                    player.mediaLibrary.playlist = new List<Track>();
                    if(player.playlistCollection.Contains(player.playlistCollection.Where(playlist => playlist.Name == "Library").ToList().FirstOrDefault()))
                    {
                        int index = (player.playlistCollection.IndexOf(player.playlistCollection.Where(playlist => playlist.Name == "Library").ToList().FirstOrDefault()));
                        player.playlistCollection[index] = player.mediaLibrary;
                    }
                 


                }
           

                string[] songFileName = openFilePopup.SafeFileNames;
               string[] songFilePath = openFilePopup.FileNames;

                for (int i = 0; i < songFilePath.Length; i++)
                {
                    Track track = new Track();
                    getTrackTags(track, i, songFileName, songFilePath);
                    if (!ignoreList.Contains(songFilePath[i]))
                    {
                        player.mediaLibrary.playlist.Add(track);
                        if (playlistPage.Visibility == Visibility.Visible)
                        {
                            if (player.selectedPlaylist == player.mediaLibrary)
                            {
                                addTrackThumbnail(track, playlistListView);
                            }
                        }

                    }    
                    if(player.playlistCollection.Count > 0)
                    {
                      int index = player.playlistCollection.IndexOf(player.playlistCollection.Where(playlist => playlist.Name == "Library").ToList().First());
                        player.playlistCollection[index] = player.mediaLibrary;
                    }
                    else
                    {
                        player.playlistCollection.Add(player.mediaLibrary);
                    }
             
                    //updates library playlist in the collection. they must stay the same for proper functionality
                    if (player.playlistCollection.Where(playlist => playlist.Name == "Library") != null)
                    {
                        if(player.playlistCollection.Count > 0)
                        {
                            int index = player.playlistCollection.IndexOf(player.playlistCollection.Where(playlist => playlist.Name == "Library").ToList().First());
                            player.playlistCollection[index] = player.mediaLibrary;


                        }
                    }              

                }

            }           
            
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
        private void savePlaylist(string playlistName, List<Track> list)
        {

            File.Delete(playlistName + "plst");
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

            foreach (var track in player.mediaLibrary.playlist)
            {
                playlist.WriteLine(track.filePath);
            }
            playlist.Close();

        }


        private void deleteTrack()
        {

            if (player.selectedSongIndex >= 0)
            {
              
                if(playlistListView.Children.Count > 0)
                {
                    player.playlistCollection[player.selectedPlaylistIndex].playlist.RemoveAt(player.selectedSongIndex);
                    playlistListView.Children.RemoveAt(player.selectedSongIndex);
                }
                if (player.selectedSongIndex > player.playlistCollection[player.selectedPlaylistIndex].playlist.Count - 1)
                {
                    player.selectedSongIndex = player.playlistCollection[player.selectedPlaylistIndex].playlist.Count - 1;
                }
                foreach (Grid icon in playlistListView.Children)
                {
                    highlightFunction(icon, playlistListView);
                }

            }

            


        }


        //-----------------------------------UI-----------------------------------------------\\

        private void showPlaylistPage()
        {
            libraryPage.Visibility = Visibility.Collapsed;
            delphinPage.Visibility = Visibility.Collapsed;
            playlistPage.Visibility = Visibility.Visible;
            tabTitleLabel.Content = "Playlists";
            slideRightToLeftAnimation(titleLabelBar);

        }

        private void slideRightToLeftAnimation(UIElement element)
        {
            element.Visibility = Visibility.Visible;
            TranslateTransform trans = new TranslateTransform();
            element.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(mainWindow.ActualWidth, 0, TimeSpan.FromMilliseconds(350));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);

        }
        private double getWindowSize()
        {
            return mainWindow.Width;
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

        private void showLibraryPage()

        {
            delphinPage.Visibility = Visibility.Collapsed;
            playlistPage.Visibility = Visibility.Collapsed;
            libraryPage.Visibility = Visibility.Visible;
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





        //-----------------------------------wrapper functions-----------------------------------\\


        public void removeSongFromLibrary()
        {

            if (player.mediaLibrary.playlist.Count > 0 && libraryListView.SelectedIndex >= 0)
            {
                int previousIndex = libraryListView.SelectedIndex;
                player.mediaLibrary.playlist.Remove(player.mediaLibrary.playlist[previousIndex]);
                libraryListView.ItemsSource = null;
                libraryListView.ItemsSource = player.mediaLibrary.playlist;


                if (previousIndex < libraryListView.Items.Count - 1)
                {

                    libraryListView.SelectedIndex = previousIndex++;

                }
                else
                {
                    if (libraryListView.SelectedIndex + 1 < player.mediaLibrary.playlist.Count)
                    {
                        libraryListView.SelectedIndex = player.mediaLibrary.playlist.Count - 1;
                    }
                }

            }




        }

        private void handlePlayButtonClick()
        {
            
            startPlaying();          
            
        }

        private void saveData()
        {
            data.playlistCollection = player.playlistCollection;
            data.mediaLibrary = player.mediaLibrary;
            serializeData(data, "data.xml");
        }

        private void closeApp()
        {
            saveData();    


        }

        private void serializeData(Data data, string fileName)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Data));            
            System.IO.FileStream file = System.IO.File.Create(fileName);
            writer.Serialize(file, data);
            file.Close();            
        }
        public void deserializeXmlData(string fileName)
        {
            if (File.Exists(fileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Data));

                // Create a TextReader to read the file.
                FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate);
                TextReader reader = new StreamReader(fs);


                // Use the Deserialize method to restore the object's state.
                data = (Data)serializer.Deserialize(reader);

            }
        }


        private void deletePlaylist()
        {
            if (playlistView.Children.Count > 0)
            {

               
                playlistView.Children.RemoveAt(player.selectedPlaylistIndex);
                player.playlistCollection.Remove(player.selectedPlaylist);
                
                if(player.selectedPlaylist.Name == "Library")
                {
                    player.mediaLibrary = new Playlist();
                    player.mediaLibrary.Name = "Library";

                }

            }
            if(playlistView.Children.Count == 0)
            {
                addNoPlaylistIcon(playlistView);

            }          
            

            foreach (Grid icon in playlistListView.Children)
            {
                highlightFunction(icon, playlistView);
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
            image.SetValue(Grid.RowProperty, 1);
            image.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            image.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            image.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));
            return image;
        }
        private Grid createAlbumIcon()
        {
            Grid albumStack = new Grid();
            albumStack.RowDefinitions.Add(new RowDefinition());
            albumStack.RowDefinitions.Add(new RowDefinition());
            albumStack.RowDefinitions.Add(new RowDefinition());
            albumStack.Width = 120;
            albumStack.VerticalAlignment = VerticalAlignment.Center;
            albumStack.HorizontalAlignment = HorizontalAlignment.Center;
            albumStack.MinHeight = 120;
            albumStack.Margin = new Thickness(10, 10, 10, 10);
            Border border = new Border();
            border.SetValue(Grid.RowProperty, 1);
            border.CornerRadius = new CornerRadius(14);
            border.Background = mainBlue;
            border.Width = 100;
            border.Height = 100;
            albumStack.Children.Add(border);

            return albumStack;
        }
        private void addAlbumThumbnail(Playlist playlist, WrapPanel wrapPanel)
        {
            Grid albumStack = createAlbumIcon();
            albumStack.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            albumStack.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            albumStack.Margin = new Thickness(10, 10, 10, 10);
            TextBlock textblock = new TextBlock() { Text = playlist.Name, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 12,  };
            textblock.SetValue(Grid.RowProperty, 0);
            albumStack.Children.Add(textblock);       

            Playlist selectedItem = playlist;
            var firstTrack = selectedItem.playlist.FirstOrDefault();
            BitmapImage image = new BitmapImage();
            if(firstTrack != null)
            {
                try
                {
                    getAlbumImageFromTag(firstTrack);
                }
                catch
                {

                }
                if (firstTrack.albumArt != null)
                {
                    image = firstTrack.albumArt;
                    Image albumImage = new Image();
                    albumImage.Source = image;
                    albumImage.Width = 60;
                    albumImage.Height = 60;
                    albumImage.SetValue(Grid.RowProperty, 1);
                    albumImage.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                    albumImage.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    albumImage.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));
                    albumStack.Children.Add(albumImage);
                }
                else
                {
                    Image albumImage = getRandomImageFromWeb();
                    albumImage.SetValue(Grid.RowProperty, 1);
                    albumStack.Children.Add(albumImage);

                }

            }
            else
            {
                try
                {
                    getAlbumImageFromTag(firstTrack);
                }
                catch
                {

                }
                Image albumImage = getRandomImageFromWeb();
                albumImage.SetValue(Grid.RowProperty, 1);
                albumStack.Children.Add(albumImage);

            }



            wrapPanel.Children.Add(albumStack);
            foreach (Grid playlistIcon in wrapPanel.Children)
            {
                playlistIcon.MouseEnter += PlaylistIcon_MouseEnter;
                playlistIcon.MouseLeave += PlaylistIcon_MouseLeave;
                playlistIcon.MouseDown += PlaylistIcon_MouseDown;               

            }
        }
        private void addNoPlaylistIcon(WrapPanel wrapPanel)
        {
            Grid albumStack = createAlbumIcon();
            albumStack.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            albumStack.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            albumStack.Margin = new Thickness(10, 10, 10, 10);
            TextBlock textblock = new TextBlock() { Text = "Add Music", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, FontSize = 14 };
            textblock.SetValue(Grid.RowProperty, 1);
            albumStack.Children.Add(textblock);       
        

           

           
           
                //Image albumImage = getRandomImageFromWeb();
                //albumImage.SetValue(Grid.RowProperty, 1);
                //albumStack.Children.Add(albumImage);

            


            wrapPanel.Children.Add(albumStack);
            foreach (Grid noMusicIcon in wrapPanel.Children)
            {
                noMusicIcon.MouseEnter += NoMusicIcon_MouseEnter; ;
                noMusicIcon.MouseLeave += NoMusicIcon_MouseLeave;
                noMusicIcon.MouseDown += NoMusicIcon_MouseDown;

            }
        }



        private void iconHoverEffect(object sender)
        {

            Grid panel = (Grid)sender;
            var nameText = panel.Children[1];
            var border = panel.Children[0];
            if(panel.Children.Count > 2)
            {
                var albumArtImage = panel.Children[2];


                albumArtImage.RenderTransform = new ScaleTransform(scaleX: 1.2, scaleY: 1.2);
                albumArtImage.RenderTransformOrigin = new Point(x: .5, y: .5);

            }









        }


        private void iconHoverEffectMouseLeave(object sender)
        {
            Grid panel = (Grid)sender;
            var nameText = panel.Children[1];
            var border = panel.Children[0];
            if (panel.Children.Count >2 )
            {
                var albumArtImage = panel.Children[2];
                albumArtImage.RenderTransform = new ScaleTransform(scaleX: 1, scaleY: 1);

            }

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
            TextBlock artistName = new TextBlock() { Text = track.artist, VerticalAlignment = VerticalAlignment.Top, FontSize = 12 };
            artistName.SetValue(Grid.RowProperty, 2);


            var selectedItem = track;
            var textBlock = new TextBlock() { Text = selectedItem.fileName, VerticalAlignment = VerticalAlignment.Top, FontSize = 12 };
            textBlock.SetValue(Grid.RowProperty, 0);
            albumStack.Children.Add(textBlock);

            try
            {
                setTrackAlbumArt(track);

            }
            catch
            {

            }
            if (selectedItem.albumArt != null)
            {

                Image albumImage = new Image();
                albumImage.SetValue(Grid.RowProperty, 1);
                  albumImage.Source =  selectedItem.albumArt;
                albumImage.Width = 60;
                albumImage.Height = 60;
                albumImage.SetValue(Grid.RowProperty, 1);
                albumImage.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                albumImage.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                albumImage.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));
                albumStack.Children.Add(albumImage);
            }
            else
            {            
                
                    Image albumImage = getRandomImageFromWeb();
                    albumImage.SetValue(Grid.RowProperty, 1);
                    albumStack.Children.Add(albumImage);
                

            }

            albumStack.Children.Add(artistName);

            wrapPanel.Children.Add(albumStack);


            foreach (Grid element in wrapPanel.Children)
                {
                    element.MouseEnter += Element_MouseEnter;
                    element.MouseLeave += Element_MouseLeave;
                    element.MouseDown += Element_MouseDown;

                }
            }

        //    private void addTrackThumbnail()
        //{
        //    foreach(Track track in player.mediaLibrary)
        //    {
        //        Grid albumStack = createAlbumIcon();
        //        albumStack.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
        //        albumStack.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
        //        albumStack.Margin = new Thickness(10, 10, 10, 10);
        //        albumStack.Children.Add(new TextBlock() { Text = track.fileName, VerticalAlignment = VerticalAlignment.Top,HorizontalAlignment = HorizontalAlignment.Center, FontSize = 12, TextTrimming = TextTrimming.CharacterEllipsis });
        //        Image image = getRandomImageFromWeb();
        //        image.SetValue(Grid.RowProperty, 1);
                



        //        var selectedItem = track;          


        //        if(track.albumArt != null)
        //        {
        //            Image albumImage = new Image();
        //                albumImage.Source  = track.albumArt;
        //            albumImage.Width = 60;
        //            albumImage.Height = 60;
        //            albumImage.SetValue(Grid.RowProperty, 1);
        //            albumImage.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
        //            albumImage.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
        //            albumImage.SetValue(PaddingProperty, new Thickness(5, 5, 5, 5));
                  

        //            albumStack.Children.Add(albumImage);
        //        }
        //        else
        //        {
        //            albumStack.Children.Add(image);

        //        }


                
        //        playlistView.Children.Add(albumStack);

               
        //       albumStack.MouseEnter += Element_MouseEnter;
        //       albumStack.MouseLeave += Element_MouseLeave;
        //       albumStack.MouseDown += Element_MouseDown;
        //    }          


           
        //}

    

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
                button.Click += savePlaylistButton_Click;
                playlistPanel.Children.Add(button);
                playlistPanel.Children.Add(playlistBox);
            }         


        }

        private void savePlaylistButton_Click(object sender, RoutedEventArgs e)
        {

            List<Track> list = new List<Track>();
            foreach (string item in playlistBox.Items)
            {
                string fileName = item.ToString();

                list.Add(player.mediaLibrary.playlist.Where(track => track.fileName == fileName).ToList().FirstOrDefault());
            }
            Playlist playlist = new Playlist();
            playlist.playlist = list;
            playlist.Name = String.Format("Playlist{0}", player.playlistCollection.Count);
            player.playlistCollection.Add(playlist);
            playlistBox.Items.Clear();
            if (list.Count > 0)
            {
                addAlbumThumbnail(playlist, playlistView);

            }

        }



        private void highlightFunction(Grid icon, WrapPanel panel)
        {
            

            TextBlock nameText = (TextBlock)icon.Children[1];
            Border border = (Border)icon.Children[0];
            if (nameText.Text != "Add Music")
            {
                var albumArtImage = icon.Children[2];

            }

            var playlist = player.playlistCollection.Where(playlist => playlist.Name == nameText.Text).ToList().FirstOrDefault();

            // for track icons

            if (playlistPage.Visibility == Visibility.Visible)
            {

               if (playlistListView.Children.IndexOf(icon) == player.currentMediaIndex && player.currentPlaylist == player.selectedPlaylist.playlist)
                {
                    border.BorderBrush = yellow;
                    border.BorderThickness = new Thickness(3, 3, 3, 3);                   
                    
                }
                else if (playlistListView.Children.IndexOf(icon) == player.selectedSongIndex)
                {          
                
                        border.BorderBrush = green;
                        border.BorderThickness = new Thickness(3, 3, 3, 3);                  
                   

                }
                else
                {
                    border.BorderBrush = null;
                    

                }
            }

            //for library view icons

            else if(libraryPage.Visibility == Visibility.Visible)
            {
                if(playlist != null)
                {
                    if (player.currentPlaylist == playlist.playlist)
                    {
                        border.BorderBrush = yellow;
                        border.BorderThickness = new Thickness(3, 3, 3, 3);

                    }
                    else if (playlist.playlist == player.selectedPlaylist.playlist)
                    {
                        border.BorderBrush = green;
                        border.BorderThickness = new Thickness(3, 3, 3, 3);

                    }

                    else
                    {
                        border.BorderBrush = null;


                    }

                }
            
                   
               
                
            }
           



        }




    




        private void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Grid panel = (Grid)sender;
            var albumArtImage = panel.Children[2];
            TextBlock nameText = (TextBlock)panel.Children[1];
            var border = panel.Children[0];
            var selectedTrack = player.selectedPlaylist.playlist.Where(track => track.fileName == nameText.Text).ToList().FirstOrDefault();
            player.selectedTrack = selectedTrack;
            player.selectedSongIndex = playlistListView.Children.IndexOf(panel);
            foreach (Grid icon in playlistListView.Children)
            {
                highlightFunction(icon, playlistListView);

            }
        



            //double click activates playing
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                  
                player.currentMedia = selectedTrack;
                player.previouslyPlayedSongIndex = player.currentMediaIndex;
                player.currentMediaIndex = playlistListView.Children.IndexOf(panel);
                Playlist tempPlayist = player.playlistCollection.Where(playlist => playlist == player.selectedPlaylist).ToList().FirstOrDefault();
                player.currentPlaylist = tempPlayist.playlist;
              
                player.loadSongIntoPlayer(selectedTrack);
                startPlaying();
                foreach (Grid icon in playlistListView.Children)
                {
                    highlightFunction(icon, playlistListView);

                }

            }
           







        }



      
       private void importSavedPlaylistFiles()
        {
            for (int i = 0; i < 20; i++)
            {
                    string playlistName = string.Format("Playlist{0}", i.ToString());


                    if (File.Exists(playlistName))
                    {
                        string[] savedPlaylist = File.ReadAllLines(playlistName);
                        List<Track> playlist = new List<Track>();
                        foreach (string filePath in savedPlaylist)
                        {

                            playlist.Add(player.mediaLibrary.playlist.Where(track => track.filePath == filePath).ToList().FirstOrDefault());
                        }
                        Playlist playlist1 = new Playlist();
                        playlist1.playlist = playlist;
                        playlist1.Name = playlistName;
                        player.playlistCollection.Add(playlist1);
                       addAlbumThumbnail(playlist1, playlistView);
                       
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
            if(libraryPage.Visibility == Visibility.Visible)
            {
                deletePlaylist();

            }
            if (playlistPage.Visibility == Visibility.Visible)
            {
                deleteTrack();


            }
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
            
        }

        private void playlistListView_Loaded(object sender, RoutedEventArgs e)
        {    
            
     
            
        }

        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            closeApp();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
         
        }
    }
    
}
