using System.ComponentModel.DataAnnotations;

namespace SignalR_Demo.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        public string User { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}