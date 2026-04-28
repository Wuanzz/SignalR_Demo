## 💬 Ứng dụng Chat Real-time với ASP.NET Core SignalR

Đây là dự án demo ứng dụng Chat thời gian thực (Real-time) sử dụng **ASP.NET Core SignalR** kết hợp với kiến trúc **MVC** và **Entity Framework Core**. Dự án tập trung vào việc mô phỏng một hệ thống giao tiếp nội bộ với các tính năng từ cơ bản đến nâng cao như chia phòng chat, gửi ảnh và theo dõi trạng thái hoạt động của người dùng.

## 🚀 Các tính năng nổi bật

* **Real-time Messaging:** Gửi và nhận tin nhắn ngay lập tức không cần tải lại trang (WebSockets/SignalR).

* **Xác thực người dùng:** Đăng nhập bằng Cookie Authentication (Mô phỏng bằng plaintext password cho mục đích demo).

* **Chia phòng Chat (Group Chat):** Định tuyến tin nhắn theo từng phòng riêng biệt (Chung, Học tập, Giải trí).

* **Quản lý trạng thái:** Hiển thị danh sách người dùng đang Online/Offline theo thời gian thực.

* **Gửi hình ảnh:** Hỗ trợ Upload file ảnh từ máy tính lên Server và hiển thị ngay lập tức.

* **Trải nghiệm người dùng (UX):**

  * Hiệu ứng "Ai đó đang gõ..." (Typing indicator).
  * Tự động cuộn xuống tin nhắn mới nhất.
  * Lightbox: Click để phóng to hình ảnh.
  * Phân biệt giao diện tin nhắn của bản thân và người khác.

## 🛠️ Công nghệ sử dụng

* **Backend:** C#, ASP.NET Core MVC, SignalR
* **Database:** SQL Server, Entity Framework Core (Code First)
* **Frontend:** HTML5, CSS3, Vanilla JavaScript, Bootstrap 5

## ⚙️ Hướng dẫn cài đặt và chạy dự án

### 1. Yêu cầu hệ thống

* .NET 8.0 SDK (Hoặc phiên bản tương đương)
* Visual Studio 2022
* SQL Server Management Studio

### 2. Cấu hình Database

Mở file `appsettings.json` và kiểm tra lại chuỗi kết nối `DefaultConnection`. Cập nhật lại tên Server (Ví dụ: `(localdb)\mssqllocaldb` hoặc `.\SQLEXPRESS`) cho phù hợp với máy của bạn:

```plaintext
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SignalR_Demo_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### 3. Tạo cơ sở dữ liệu (Migrations)

Mở dự án bằng Visual Studio, vào **Tools** -> **NuGet Package Manager** -> **Package Manager Console** và chạy lệnh sau để tự động tạo Database cùng các bảng `Users` và `ChatMessages`:

```plaintext
Update-Database
```

### 4. Khởi tạo dữ liệu mẫu (Seed Data)

Mở SQL Server Management Studio (SSMS), trỏ vào Database vừa tạo và chạy đoạn script sau để thêm các tài khoản test:

```plaintext
USE SignalR_Demo_DB;
INSERT INTO Users (Username, Password, FullName)
VALUES 
    ('huy', '123', N'Phạm Nhật Huy'),
    ('duy', '123', N'Nguyễn Thanh Nhựt Duy'),
    ('quan', '123', N'Cao Huỳnh Minh Quân')
```

### 5. Chạy ứng dụng

* Nhấn **F5** hoặc nút **Run** trong Visual Studio.
* Sử dụng một trong các tài khoản phía trên (Mật khẩu chung là `123`) để đăng nhập.
* Khuyến nghị mở ứng dụng trên 2 trình duyệt khác nhau (hoặc 1 tab thường, 1 tab ẩn danh) để test trải nghiệm chat Real-time và chuyển phòng.