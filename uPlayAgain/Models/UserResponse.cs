using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;
using uPlayAgain.Converters;

namespace uPlayAgain.Models
{
    public class UserResponse
    {
        public string Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Mail { get; set; }
        public float FeedbackAvg{ get; set; }
        public int FeedbackCount { get; set; }
        [JsonConverter(typeof(DbGeographyConverter))]
        public DbGeography PositionUser { get; set; }
        public byte[] Image { get; set; }
        public DateTimeOffset LastLogin { get; set; }
        public int GameInLibrary { get; set; }
        public List<int>LibrariesId { get; set; }
    }
}