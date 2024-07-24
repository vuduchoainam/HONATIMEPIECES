using HONATIMEPIECES.Data;
using HONATIMEPIECES.DTOs.ProductDTO;
using HONATIMEPIECES.Models;

namespace HONATIMEPIECES.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<Product>> SearchAsync(SearchProductDTO searchProductDTO);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task SaveChangesAsync();
        Task DeleteAsync(int id);
        Task<Product> GetProductByIdAsync(int id);
        Task<int> CountWithSlugAsync(string slug);
    }
}
