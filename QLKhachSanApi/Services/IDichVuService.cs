using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public interface IDichVuService
    {
        Task<IEnumerable<DichVu>> GetAllAsync();
        Task<DichVu?> GetByIdAsync(int id);
        Task<DichVu> AddAsync(DichVu entity);
        Task UpdateAsync(DichVu entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}


