using Microsoft.AspNetCore.SignalR;

namespace SignalR_Demo.Hubs
{
    // Lớp này kế thừa từ Hub để sử dụng các tính năng của SignalR
    public class ChatHub : Hub
    {
        // Hàm này sẽ được Client gọi để gửi tin nhắn
        public async Task SendMessage(string user, string message)
        {
            // Broadcast tin nhắn đến tất cả các Client đang kết nối
            // "ReceiveMessage" là tên hàm mà phía Client (JS) phải lắng nghe
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}