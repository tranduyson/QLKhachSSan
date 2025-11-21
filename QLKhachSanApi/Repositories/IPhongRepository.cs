using QLKhachSanApi.Models;

namespace QLKhachSanApi.Repositories
{
    public interface IPhongRepository : IRepository<Phong>
    {
        Task<IEnumerable<Phong>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
    }
}
