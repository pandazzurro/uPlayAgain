using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using uPlayAgain.Data.EF.Models;

namespace uPlayAgain.GameImporter.Model
{
    public class GameDto : Game
    {
        public bool IsSelected { get; set; }
        public BitmapImage SourceImage
        {
            get
            {
                if (ImageThumb == null || ImageThumb.Length == 0)
                    return null;

                BitmapImage image = new BitmapImage();
                using (var mem = new MemoryStream(ImageThumb))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            }
        }
        
        public GameDto()
        {

        }
    }
}
