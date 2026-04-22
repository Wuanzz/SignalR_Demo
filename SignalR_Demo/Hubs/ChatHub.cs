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

        // Ghi nhớ người dùng ở phòng nào
        private static readonly ConcurrentDictionary<string, string> _userRooms = new ConcurrentDictionary<string, string>();

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        // XỬ LÝ VÀO PHÒNG CHAT (VÀ THÔNG BÁO CHUYỂN PHÒNG)
        public async Task JoinRoom(string roomName)
        {
            string connectionId = Context.ConnectionId;
            string userName = Context.User?.Identity?.Name ?? "Ai đó";

            // Nếu người này đang ở phòng khác, rút họ ra khỏi phòng đó và thông báo cho phòng cũ
            if (_userRooms.TryGetValue(connectionId, out string oldRoom))
            {
                await Groups.RemoveFromGroupAsync(connectionId, oldRoom);
                await Clients.Group(oldRoom).SendAsync("ReceiveSystemMessage", $"{userName} đã chuyển sang phòng khác.");
            }

            // Ghi danh họ vào phòng mới
            _userRooms[connectionId] = roomName; // Cập nhật sổ
            await Groups.AddToGroupAsync(connectionId, roomName); // Lệnh của SignalR

            // Thông báo cho những người trong phòng mới
            await Clients.OthersInGroup(roomName).SendAsync("ReceiveSystemMessage", $"{userName} vừa tham gia phòng.");
        }

        // XỬ LÝ GỬI TIN NHẮN 
        public async Task SendMessage(string roomName, string message)
        {
            // Tự động lấy tên người dùng từ Cookie
            string user = Context.User?.Identity?.Name ?? "Unknown";

            // Lưu vào Database
            var chatMsg = new ChatMessage
            {
                User = user,
                Message = message,
                Room = roomName,
                Timestamp = DateTime.Now
            };

            _context.ChatMessages.Add(chatMsg);
            await _context.SaveChangesAsync();

            // Phát tin nhắn đi (CHỈ GỬI CHO NHỮNG NGƯỜI TRONG PHÒNG)
            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
        }

        // XỬ LÝ TRẠNG THÁI ĐANG GÕ 
        public async Task Typing(string roomName)
        {
            string userName = Context.User?.Identity?.Name ?? "Unknown";
            // Bắn tín hiệu cho những người khác TRONG CÙNG PHÒNG
            await Clients.OthersInGroup(roomName).SendAsync("UserTyping", userName);
        }

        // THÔNG BÁO NGƯỜI MỚI VÀO HỆ THỐNG
        public override async Task OnConnectedAsync()
        {
            string userName = Context.User?.Identity?.Name ?? "Ai đó";
            string connectionId = Context.ConnectionId;

            // Thêm người này vào sổ online
            _onlineUsers.TryAdd(connectionId, userName);

            // Cập nhật danh sách (Distinct để lọc trùng nếu 1 người mở 2 tab)
            var currentUsers = _onlineUsers.Values.Distinct().ToList();

            // Bắn danh sách mới nhất cho TẤT CẢ mọi người để cập nhật Sidebar
            await Clients.All.SendAsync("UpdateOnlineUsers", currentUsers);

            // Đã xóa dòng thông báo "vừa tham gia" ở đây vì hàm JoinRoom sẽ đảm nhiệm việc đó
            await base.OnConnectedAsync();
        }

        // 5. THÔNG BÁO NGƯỜI RỜI ĐI 
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userName = Context.User?.Identity?.Name ?? "Ai đó";
            string connectionId = Context.ConnectionId;

            // Tìm xem người này tắt trình duyệt khi đang ở phòng nào
            if (_userRooms.TryGetValue(connectionId, out string roomName))
            {
                // Chỉ báo cho phòng đó biết là họ đã thoát
                await Clients.Group(roomName).SendAsync("ReceiveSystemMessage", $"{userName} đã rời khỏi hệ thống.");
                _userRooms.TryRemove(connectionId, out _);
            }

            // Xóa người này khỏi sổ online
            _onlineUsers.TryRemove(connectionId, out _);

            // Cập nhật lại danh sách sidebar
            var currentUsers = _onlineUsers.Values.Distinct().ToList();

            // Bắn danh sách cập nhật cho TẤT CẢ mọi người
            await Clients.All.SendAsync("UpdateOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }
}