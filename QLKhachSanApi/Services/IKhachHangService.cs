using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public interface IKhachHangService
    {
        Task<IEnumerable<KhachHang>> GetAllAsync();
        Task<KhachHang?> GetByIdAsync(int id);
        Task<KhachHang> AddAsync(KhachHang entity);
        Task UpdateAsync(KhachHang entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}


