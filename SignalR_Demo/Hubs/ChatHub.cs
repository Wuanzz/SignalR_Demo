using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR_Demo.Models;

namespace SignalR_Demo.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Chỉ nhận 1 tham số 'message' từ Client
        public async Task SendMessage(string message)
        {
            // Tự động lấy tên người dùng từ Cookie
            string user = Context.User?.Identity?.Name ?? "Unknown";

            // Lưu vào Database
            var chatMsg = new ChatMessage
            {
                User = user,
                Message = message,
                Timestamp = DateTime.Now
            };

            _context.ChatMessages.Add(chatMsg);
            await _context.SaveChangesAsync();

            // Phát tin nhắn đi (truyền đủ 2 tham số xuống lại Client)
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // 2. Xử lý trạng thái đang gõ (Chỉ nhận 1 tham số từ Client)
        public async Task Typing(string user)
        {
            await Clients.Others.SendAsync("UserTyping", user);
        }

        // 3. Thông báo người mới vào
        public override async Task OnConnectedAsync()
        {
            string userName = Context.User?.Identity?.Name ?? "Ai đó";
            await Clients.Others.SendAsync("ReceiveSystemMessage", $"{userName} vừa tham gia phòng chat.");
            await base.OnConnectedAsync();
        }

        // 4. Thông báo người rời đi
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userName = Context.User?.Identity?.Name ?? "Ai đó";
            await Clients.All.SendAsync("ReceiveSystemMessage", $"{userName} đã rời khỏi phòng chat.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}