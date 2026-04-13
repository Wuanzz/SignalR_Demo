using Microsoft.AspNetCore.SignalR;

namespace SignalR_Demo.Hubs
{
    public class ChatHub : Hub
    {
        // 1. Gửi tin nhắn bình thường
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // 2. Xử lý tính năng "Ai đó đang gõ..."
        public async Task Typing(string user)
        {
            // Bắn tín hiệu "đang gõ" cho tất cả mọi người TRỪ người đang gõ
            await Clients.Others.SendAsync("UserTyping", user);
        }

        // 3. Thông báo hệ thống khi có người kết nối
        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("ReceiveSystemMessage", "Một người dùng vừa tham gia phòng chat.");
            await base.OnConnectedAsync();
        }

        // 4. Thông báo hệ thống khi có người thoát/đóng tab
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("ReceiveSystemMessage", "Một người dùng đã rời khỏi phòng chat.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}