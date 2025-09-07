namespace ChatApp.Api.DTOs
{
    public class SendMessageDto
    {
        public string Content { get; set; } = string.Empty;
    }

    public class MessageResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public UserDto User { get; set; } = null!;
    }
}