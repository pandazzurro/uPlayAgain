using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System;
using uPlayAgain.Converters;

namespace uPlayAgain.Models
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

        [JsonIgnore]
        public byte[] Image { get; set; }
        
        [JsonProperty(PropertyName = "Image", DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(BitmapConverter))]
        public Image Thumb
        {
            get
            {
                using (var ms = new MemoryStream(Image))
                {
                    Image img = System.Drawing.Image.FromStream(ms);
                    int fixedWidth = 100;
                    double widthPercentage = (double)fixedWidth / (double)img.Width;
                    return img.GetThumbnailImage(fixedWidth, (int)(img.Height * widthPercentage), () => false, IntPtr.Zero);
                }
            }
        }

        // Foreign key
        public string GenreId { get; set; }
        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; }
        public string PlatformId { get; set; }
        [ForeignKey("PlatformId")]
        public virtual Platform Platform { get; set; }
    }
}