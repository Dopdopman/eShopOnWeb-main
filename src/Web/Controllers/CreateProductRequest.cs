public class CreateProductRequest
{
    public string Name { get; set; }        // Tên sản phẩm
    public string Description { get; set; } // Mô tả
    public decimal Price { get; set; }      // Giá tiền
    public int CatalogBrandId { get; set; } // ID Thương hiệu
    public int CatalogTypeId { get; set; }  // ID Loại
}