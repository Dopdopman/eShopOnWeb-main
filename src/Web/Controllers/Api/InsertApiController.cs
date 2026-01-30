using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System.Threading.Tasks;
// Thêm dòng này để gọi được biến ActiveTokens từ AuthController
using Microsoft.eShopWeb.Web.Controllers.Api; 

namespace Microsoft.eShopWeb.Web.Controllers.Api
{
    [Route("api/insert")]
    [ApiController]
    public class InsertApiController : ControllerBase
    {
        private readonly IRepository<CatalogItem> _itemRepository;

        // Constructor để nạp Repository
        public InsertApiController(IRepository<CatalogItem> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        // =================================================================
        // API INSERT PRODUCT (DÙNG TOKEN ĐỘNG TỰ SINH)
        // URL mẫu: api/insert/product?token=xxxx-yyyy-zzzz&name=AoMoi&price=100...
        // =================================================================
        [HttpPost("product")]
        public async Task<IActionResult> CreateProductUrlParam(
            [FromQuery] string token,   // <--- Token nhận từ URL
            [FromQuery] string name,
            [FromQuery] decimal? price,
            [FromQuery] int? brandId,
            [FromQuery] int? typeId,
            [FromQuery] string description = "No desc")
        {
            // --- 1. KIỂM TRA TOKEN (LOGIC MỚI: CHECK LIST ACTIVE) ---
            
            // Logic: Token không được rỗng VÀ Token phải nằm trong danh sách ActiveTokens bên AuthApiController
            if (string.IsNullOrEmpty(token) || !AuthApiController.ActiveTokens.Contains(token))
            {
                return Ok(new 
                { 
                    ErrorCode = 401, 
                    Message = "Auth Failed: Token không hợp lệ hoặc đã hết hạn (Vui lòng đăng nhập lại để lấy token mới)!" 
                });
            }

            // --- 2. KIỂM TRA DỮ LIỆU (VALIDATION) -> Trả về Mã 1 ---
            if (string.IsNullOrEmpty(name) || price == null || price <= 0 || brandId == null || typeId == null)
            {
                return Ok(new 
                { 
                    ErrorCode = 1, 
                    Message = "Missing Data: Thiếu tên, giá (phải > 0), hoặc mã loại/thương hiệu." 
                });
            }

            // --- 3. THÊM VÀO DATABASE -> Trả về Mã 0 ---
            try
            {
                var newItem = new CatalogItem(
                    typeId.Value,
                    brandId.Value,
                    description,
                    name,
                    price.Value,
                    "default_image.png"
                );

                await _itemRepository.AddAsync(newItem);

                return Ok(new 
                { 
                    ErrorCode = 0, 
                    Message = "Insert Success (Authenticated via Dynamic Token)", 
                    ProductId = newItem.Id 
                });
            }
            catch (System.Exception ex)
            {
                return Ok(new { ErrorCode = 99, Message = "System Error: " + ex.Message });
            }
        }
    }
}