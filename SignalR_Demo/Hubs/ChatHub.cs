using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR_Demo.Models;
using System.Collections.Concurrent;

namespace SignalR_Demo.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        // Lưu trữ danh sách người dùng đang online 
        private static readonly ConcurrentDictionary<string, string> _onlineUsers = new ConcurrentDictionary<string, string>();

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        // Xử lý gửi tin nhắn 
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

        // Xử lý trạng thái đang gõ 
        public async Task Typing(string user)
        {
            await Clients.Others.SendAsync("UserTyping", user);
        }

        // Thông báo người mới vào
        public override async Task OnConnectedAsync()
        {
            string userName = Context.User?.Identity?.Name ?? "Ai đó";
            string connectionId = Context.ConnectionId;

            // Thêm người này vào sổ
            _onlineUsers.TryAdd(connectionId, userName);

            // Cập nhật danh sách (Distinct để lọc trùng nếu 1 người mở 2 tab)
            var currentUsers = _onlineUsers.Values.Distinct().ToList();

            // Bắn danh sách mới nhất cho TẤT CẢ mọi người
            await Clients.All.SendAsync("UpdateOnlineUsers", currentUsers);

            await Clients.Others.SendAsync("ReceiveSystemMessage", $"{userName} vừa tham gia phòng chat.");
            await base.OnConnectedAsync();
        }

        // Thông báo người rời đi
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userName = Context.User?.Identity?.Name ?? "Ai đó";
            string connectionId = Context.ConnectionId;

            // Xóa người này khỏi sổ
            _onlineUsers.TryRemove(connectionId, out _);

            // Cập nhật lại danh sách
            var currentUsers = _onlineUsers.Values.Distinct().ToList();

            // Bắn danh sách cập nhật cho TẤT CẢ mọi người
            await Clients.All.SendAsync("UpdateOnlineUsers", currentUsers);

            await Clients.All.SendAsync("ReceiveSystemMessage", $"{userName} đã rời khỏi phòng chat.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}