using System.Text.Json.Serialization;

namespace SIS_projekt.Models
{
    public class UserChannel
    {
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; } = null!;

        public int ChannelId { get; set; }
        [JsonIgnore]
        public Channel Channel { get; set; } = null!;
    }
}
