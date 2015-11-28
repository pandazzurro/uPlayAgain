using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using uPlayAgain.Data.Utils.Converters;

namespace uPlayAgain.Models
{
    public class UserLogin
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public byte[] Image { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [JsonConverter(typeof(DbGeographyConverter))]
        public DbGeography PositionUser { get; set; }        
    }
}
