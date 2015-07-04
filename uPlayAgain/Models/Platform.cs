using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace uPlayAgain.Models
{
    public class Platform
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string PlatformId { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public string Class { get; set; }
        [JsonProperty(Required = Required.AllowNull)]
        public string Name { get; set; }

        public int ImportId { get; set; }
    }
}