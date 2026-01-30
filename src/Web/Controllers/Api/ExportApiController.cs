using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.Web.Controllers.Api
{
    [Route("api/export")]
    [ApiController]
    public class ExportApiController : ControllerBase
    {
        private readonly IRepository<CatalogItem> _itemRepository;

        public ExportApiController(IRepository<CatalogItem> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        // =========================================================================
        // API 1: SỬA ĐỔI - XUẤT FILE JSON CỦA 1 SẢN PHẨM CỤ THỂ (CÓ ĐỊNH DANH ID)
        // URL: GET api/export/product/{id}
        // Ví dụ: api/export/product/1
        // =========================================================================
        [HttpGet("product/{id}")]
        public async Task<IActionResult> ExportProductById(int id)
        {
            // 1. Tìm sản phẩm theo ID (Định danh cụ thể)
            var item = await _itemRepository.GetByIdAsync(id);

            // 2. Kiểm tra nếu không tìm thấy (tránh lỗi null)
            if (item == null)
            {
                return NotFound(new { Message = $"Không tìm thấy sản phẩm có ID = {id}" });
            }

            // 3. Chuyển sang JSON và trả về file
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(item, jsonOptions);
            var fileBytes = Encoding.UTF8.GetBytes(jsonString);

            // Đặt tên file theo ID sản phẩm để dễ quản lý
            return File(fileBytes, "application/json", $"product_{id}.json");
        }

        // =========================================================================
        // API 2: GIỮ NGUYÊN - LỌC THEO CATEGORY/BRAND
        // URL: GET api/export/filter?brandId=1
        // =========================================================================
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredProducts([FromQuery] int? brandId, [FromQuery] int? typeId)
        {
            var spec = new CatalogFilterSpecification(brandId, typeId);
            var items = await _itemRepository.ListAsync(spec);
            return Ok(items);
        }

        // =========================================================================
        // API 3: PROTECTED FILTER (TOKEN TRÊN URL)
        // URL mẫu: api/export/secure-filter?token=abc-xyz&brandId=1
        // =================================================================
        [HttpGet("secure-filter")]
        public async Task<IActionResult> GetProtectedData(
            [FromQuery] string token,   // <--- THAY ĐỔI: Nhận token trực tiếp từ URL
            [FromQuery] int? brandId, 
            [FromQuery] int? typeId)
        {
            // 1. LOGIC KIỂM TRA TOKEN (DYNAMIC)
            // Kiểm tra xem token có trong danh sách ActiveTokens (bên AuthController) không
            if (string.IsNullOrEmpty(token) || !AuthApiController.ActiveTokens.Contains(token))
            {
                return Unauthorized(new { Message = "Lỗi xác thực: Token trên URL không đúng hoặc đã hết hạn!" });
            }

            // 2. NẾU ĐÚNG -> LẤY DỮ LIỆU (Logic cũ giữ nguyên)
            var spec = new CatalogFilterSpecification(brandId, typeId);
            var items = await _itemRepository.ListAsync(spec);

            return Ok(new
            {
                Status = "Authorized Success (Via URL Token)",
                TokenUsed = token,
                Data = items
            });
        }
    }
}