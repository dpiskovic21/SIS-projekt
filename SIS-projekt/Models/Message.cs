using System.Text.Json.Serialization;

namespace SIS_projekt.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; } = null!;

        public int ChannelId { get; set; }
        [JsonIgnore]
        public Channel Channel { get; set; } = null!;
    }
}
