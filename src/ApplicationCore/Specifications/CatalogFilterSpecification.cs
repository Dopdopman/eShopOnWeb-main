using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications
{
    // Class này giúp lọc sản phẩm theo BrandId và TypeId
    public class CatalogFilterSpecification : Specification<CatalogItem>
    {
        public CatalogFilterSpecification(int? brandId, int? typeId)
        {
            // Nếu có brandId thì lọc theo brand, nếu có typeId thì lọc theo type
            // Nếu tham số là null thì lấy tất cả
            Query.Where(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
                             (!typeId.HasValue || i.CatalogTypeId == typeId));
        }
    }
}