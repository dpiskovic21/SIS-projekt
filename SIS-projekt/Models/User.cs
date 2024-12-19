namespace SIS_projekt.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public ICollection<Channel> Channels { get; set; } = new List<Channel>();

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
