using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public interface IDatPhongService
    {
        Task<IEnumerable<DatPhong>> GetAllExpandedAsync();
        Task<DatPhong?> GetByIdExpandedAsync(int id);
        Task<DatPhong> CreateAsync(DatPhongRequest request);
        Task CheckInAsync(int id);
        Task CheckOutAsync(int id);
        Task UpdateAsync(DatPhong datPhong);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}


