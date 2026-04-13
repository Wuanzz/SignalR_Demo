using System.ComponentModel.DataAnnotations;

namespace SignalR_Demo.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Lưu text bình thường
        public string FullName { get; set; } = string.Empty; // Tên sẽ hiển thị trong khung chat
    }
}
