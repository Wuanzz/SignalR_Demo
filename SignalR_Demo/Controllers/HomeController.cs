using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalR_Demo.Models;
using System.Diagnostics;

namespace SignalR_Demo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        // Thêm trường để lưu trữ IWebHostEnvironment
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            // Lấy 50 tin nhắn gần nhất
            var history = _context.ChatMessages
                                  .Where(m => m.Room == "Chung")
                                  .OrderByDescending(m => m.Timestamp)
                                  .Take(50)
                                  .OrderBy(m => m.Timestamp)
                                  .ToList();
            return View(history);
        }

        // Xử lý Upload Ảnh
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Không có file nào được chọn" });

            // Tạo thư mục 'uploads' trong wwwroot nếu chưa có
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Tạo tên file độc nhất (tránh trùng lặp)
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Lưu file vào ổ cứng Server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về đường dẫn để hiển thị trên web
            var imageUrl = $"/uploads/{fileName}";
            return Ok(new { url = imageUrl });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // API để lấy lịch sử chat 
        [HttpGet]
        public IActionResult GetChatHistory(string room)
        {
            var history = _context.ChatMessages
                                  .Where(m => m.Room == room)
                                  .OrderByDescending(m => m.Timestamp)
                                  .Take(50)
                                  .OrderBy(m => m.Timestamp)
                                  .Select(m => new {
                                      user = m.User,
                                      message = m.Message,
                                      timestamp = m.Timestamp.ToString("HH:mm dd/MM/yyyy")
                                  })
                                  .ToList();
            return Json(history);
        }
    }
}
