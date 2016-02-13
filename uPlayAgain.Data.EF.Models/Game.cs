using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using uPlayAgain.Data.Utils.Images;

namespace uPlayAgain.Data.EF.Models
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GameId { get; set; }
      
        public string ShortName { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public string Title { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public string Description { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public int? ImportId { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        //[JsonIgnore]
        public byte[] Image {get; set;}       
        public byte[] ImageThumb { get; set; }

        public string GenreId { get; set; }
        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; }
        public string PlatformId { get; set; }
        [ForeignKey("PlatformId")]
        public virtual Platform Platform { get; set; }

        public byte[] Resize()
        {
            if (Image != null)
            {
                using (var ms = new MemoryStream(Image))
                {
                    Image img = System.Drawing.Image.FromStream(ms);
                    int fixedWidth = 65;
                    double widthPercentage = (double)fixedWidth / (double)img.Width;
                    Bitmap b = ImageUtilities.ResizeImage(img, fixedWidth, (int)(img.Height * widthPercentage));
                    ImageConverter converter = new ImageConverter();
                    return (byte[])converter.ConvertTo(b, typeof(byte[]));
                }
            }
            return null;
        }
    }
}