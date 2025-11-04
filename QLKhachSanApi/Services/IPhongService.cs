using QLKhachSanApi.Models;

namespace QLKhachSanApi.Services
{
    public interface IPhongService
    {
        Task<IEnumerable<Phong>> GetAllWithLoaiPhongAsync();
        Task<Phong?> GetByIdWithLoaiPhongAsync(int id);
        Task<IEnumerable<Phong>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
        Task<Phong> AddAsync(Phong entity);
        Task UpdateAsync(Phong entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}


