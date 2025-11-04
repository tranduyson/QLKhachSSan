using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public interface ILoaiPhongService
    {
        Task<IEnumerable<LoaiPhong>> GetAllAsync();
        Task<LoaiPhong?> GetByIdAsync(int id);
        Task<LoaiPhong> AddAsync(LoaiPhong entity);
        Task UpdateAsync(LoaiPhong entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}


