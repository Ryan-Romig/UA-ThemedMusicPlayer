using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace UnderwaterAudioMusicManagerApp
{
    public class Track
    {
        public string fileName { get; set; }
        public string filePath;
        public string songName { get; set; }
        public string artist { get; set; }
        public string genre { get; set; }
        public string album { get; set; }
        [XmlIgnoreAttribute]
        public TimeSpan duration;
        [XmlIgnoreAttribute]
        public BitmapImage albumArt {get;set;}
        


      
    }
            
            
}
        
    

    

