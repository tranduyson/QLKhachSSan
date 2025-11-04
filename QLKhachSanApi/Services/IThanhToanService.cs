using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public interface IThanhToanService
    {
        Task<IEnumerable<ThanhToan>> GetAllWithDatPhongAsync();
        Task<ThanhToan?> GetByIdWithDatPhongAsync(int id);
        Task<IEnumerable<ThanhToan>> GetByDatPhongAsync(int maDatPhong);
        Task<ThanhToan> AddAsync(ThanhToan entity);
        Task UpdateAsync(ThanhToan entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}


