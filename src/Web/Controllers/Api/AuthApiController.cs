using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Microsoft.eShopWeb.Web.Controllers.Api
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        // 1. KHO CHỨA TOKEN (Lưu tạm trong RAM)
        // Trong thực tế, cái này sẽ lưu trong Redis hoặc Database
        public static List<string> ActiveTokens = new List<string>();

        // ============================================================
        // API 4: ĐĂNG NHẬP ĐỂ LẤY TOKEN (Login)
        // URL: POST api/auth/get-token
        // Body: { "username": "admin", "password": "123" }
        // ============================================================
        [HttpPost("get-token")]
        public IActionResult LoginAndGetToken([FromBody] LoginRequest request)
        {
            // 2. GIẢ LẬP CHECK USER/PASS
            // (Bạn có thể check trong database nếu muốn, ở đây mình demo cứng)
            if (request.Username == "admin" && request.Password == "Password123!")
            {
                // 3. TỰ ĐỘNG SINH TOKEN NGẪU NHIÊN
                // Guid.NewGuid() sẽ tạo ra một chuỗi không trùng lặp (vd: 8a6f...-12d...)
                string newToken = Guid.NewGuid().ToString();

                // 4. LƯU TOKEN VÀO KHO (Để lát nữa API Insert còn biết mà check)
                ActiveTokens.Add(newToken);

                // 5. TRẢ VỀ CHO NGƯỜI DÙNG
                return Ok(new 
                { 
                    Message = "Login Success", 
                    GeneratedToken = newToken, // <--- Token tự sinh đây
                    ExpiredTime = DateTime.Now.AddMinutes(30)
                });
            }

            return Unauthorized(new { Message = "Sai tài khoản hoặc mật khẩu!" });
        }
    }

    // Class hứng dữ liệu gửi lên
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}