namespace SIS_projekt.DTO
{
    public class CreateMessageDTO
    {
        public string Content { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int ChannelId { get; set; }
    }
}
