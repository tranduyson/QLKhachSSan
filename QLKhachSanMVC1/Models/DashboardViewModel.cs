using System.Collections.Generic;

namespace HotelManagement.Models
{
    public class DashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int TodaysBookings { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public IEnumerable<DatPhong> RecentBookings { get; set; } = new List<DatPhong>();
        public Dictionary<string, int> RoomStatusBreakdown { get; set; } = new();
    }
}

