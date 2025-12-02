// demo-data.js - Dữ liệu demo cho toàn bộ hệ thống

const DEMO_DATA = {
    // Người dùng
    users: [
        { maND: 1, tenDangNhap: 'admin', matKhau: 'admin123', hoTen: 'Đỗ Xuân Dũng', vaiTro: 'Admin', email: 'dxdung@hotel.com', sdt: '0901234567', trangThai: 'active' },
        { maND: 2, tenDangNhap: 'letan01', matKhau: 'letan123', hoTen: 'Trần Mai Lan', vaiTro: 'LeTan', email: 'tmlan@hotel.com', sdt: '0902345678', trangThai: 'active' },
        { maND: 3, tenDangNhap: 'ketoan01', matKhau: 'ketoan123', hoTen: 'Lưu Đình Quang Minh', vaiTro: 'KeToan', email: 'ldquangminh@hotel.com', sdt: '0903456789', trangThai: 'active' },
        { maND: 4, tenDangNhap: 'letan02', matKhau: 'letan123', hoTen: 'Phạm Thị Hoa', vaiTro: 'LeTan', email: 'pthoa@hotel.com', sdt: '0904567890', trangThai: 'active' },
    ],
    
    // Loại phòng
    roomTypes: [
        { maLoaiPhong: 1, ma: 'STD', ten: 'Standard', moTa: 'Phòng tiêu chuẩn với đầy đủ tiện nghi cơ bản', soKhachToiDa: 2 },
        { maLoaiPhong: 2, ma: 'SUP', ten: 'Superior', moTa: 'Phòng cao cấp với view đẹp và diện tích rộng rãi', soKhachToiDa: 2 },
        { maLoaiPhong: 3, ma: 'DEL', ten: 'Deluxe', moTa: 'Phòng sang trọng với nội thất cao cấp', soKhachToiDa: 3 },
        { maLoaiPhong: 4, ma: 'JR-SUI', ten: 'Junior Suite', moTa: 'Suite nhỏ với khu vực tiếp khách riêng', soKhachToiDa: 4 },
        { maLoaiPhong: 5, ma: 'EXE-SUI', ten: 'Executive Suite', moTa: 'Suite cao cấp dành cho khách VIP', soKhachToiDa: 4 },
        { maLoaiPhong: 6, ma: 'FAM', ten: 'Family Room', moTa: 'Phòng gia đình rộng rãi', soKhachToiDa: 6 },
        { maLoaiPhong: 7, ma: 'PRES', ten: 'Presidential Suite', moTa: 'Suite tổng thống siêu sang trọng', soKhachToiDa: 8 },
    ],
    
    // Phòng
    rooms: [
        // Tầng 1 - Standard
        { maPhong: 1, soPhong: '101', maLoaiPhong: 1, tinhTrang: 'SanSang' },
        { maPhong: 2, soPhong: '102', maLoaiPhong: 1, tinhTrang: 'DaThue' },
        { maPhong: 3, soPhong: '103', maLoaiPhong: 1, tinhTrang: 'DonDep' },
        { maPhong: 4, soPhong: '104', maLoaiPhong: 1, tinhTrang: 'SanSang' },
        { maPhong: 5, soPhong: '105', maLoaiPhong: 1, tinhTrang: 'SanSang' },
        { maPhong: 6, soPhong: '106', maLoaiPhong: 1, tinhTrang: 'BaoTri' },
        { maPhong: 7, soPhong: '107', maLoaiPhong: 1, tinhTrang: 'SanSang' },
        { maPhong: 8, soPhong: '108', maLoaiPhong: 1, tinhTrang: 'SanSang' },
        
        // Tầng 2 - Superior
        { maPhong: 9, soPhong: '201', maLoaiPhong: 2, tinhTrang: 'SanSang' },
        { maPhong: 10, soPhong: '202', maLoaiPhong: 2, tinhTrang: 'DaThue' },
        { maPhong: 11, soPhong: '203', maLoaiPhong: 2, tinhTrang: 'SanSang' },
        { maPhong: 12, soPhong: '204', maLoaiPhong: 2, tinhTrang: 'DonDep' },
        { maPhong: 13, soPhong: '205', maLoaiPhong: 2, tinhTrang: 'SanSang' },
        { maPhong: 14, soPhong: '206', maLoaiPhong: 2, tinhTrang: 'SanSang' },
        
        // Tầng 3 - Deluxe
        { maPhong: 15, soPhong: '301', maLoaiPhong: 3, tinhTrang: 'SanSang' },
        { maPhong: 16, soPhong: '302', maLoaiPhong: 3, tinhTrang: 'SanSang' },
        { maPhong: 17, soPhong: '303', maLoaiPhong: 3, tinhTrang: 'DaThue' },
        { maPhong: 18, soPhong: '304', maLoaiPhong: 3, tinhTrang: 'SanSang' },
        { maPhong: 19, soPhong: '305', maLoaiPhong: 3, tinhTrang: 'SanSang' },
        
        // Tầng 4 - Junior Suite
        { maPhong: 20, soPhong: '401', maLoaiPhong: 4, tinhTrang: 'SanSang' },
        { maPhong: 21, soPhong: '402', maLoaiPhong: 4, tinhTrang: 'SanSang' },
        { maPhong: 22, soPhong: '403', maLoaiPhong: 4, tinhTrang: 'DaThue' },
        
        // Tầng 5 - Executive Suite
        { maPhong: 23, soPhong: '501', maLoaiPhong: 5, tinhTrang: 'SanSang' },
        { maPhong: 24, soPhong: '502', maLoaiPhong: 5, tinhTrang: 'SanSang' },
        
        // Tầng 6 - Family Room
        { maPhong: 25, soPhong: '601', maLoaiPhong: 6, tinhTrang: 'SanSang' },
        { maPhong: 26, soPhong: '602', maLoaiPhong: 6, tinhTrang: 'DaThue' },
        
        // Tầng 7 - Presidential Suite
        { maPhong: 27, soPhong: '701', maLoaiPhong: 7, tinhTrang: 'SanSang' },
    ],
    
    // Giá phòng
    prices: [
        { maGia: 1, maLoaiPhong: 1, tuNgay: '2025-01-01', denNgay: '2025-12-31', giaMoiDem: 850000, giaMoiGio: 160000 },
        { maGia: 2, maLoaiPhong: 2, tuNgay: '2025-01-01', denNgay: '2025-12-31', giaMoiDem: 1300000, giaMoiGio: 220000 },
        { maGia: 3, maLoaiPhong: 3, tuNgay: '2025-01-01', denNgay: '2025-12-31', giaMoiDem: 1950000, giaMoiGio: 320000 },
        { maGia: 4, maLoaiPhong: 4, tuNgay: '2025-01-01', denNgay: '2025-12-31', giaMoiDem: 2700000, giaMoiGio: 430000 },
        { maGia: 5, maLoaiPhong: 5, tuNgay: '2025-01-01', denNgay: '2025-12-31', giaMoiDem: 3800000, giaMoiGio: 650000 },
        { maGia: 6, maLoaiPhong: 6, tuNgay: '2025-01-01', denNgay: '2025-12-31', giaMoiDem: 3000000, giaMoiGio: 480000 },
        { maGia: 7, maLoaiPhong: 7, tuNgay: '2025-01-01', denNgay: '2025-12-31', giaMoiDem: 8500000, giaMoiGio: 1300000 },
    ],
    
    // Khách hàng
    customers: [
        { maKhach: 1, hoTen: 'Nguyễn Văn A', dienThoai: '0901234567', email: 'nva@email.com', cmnd: '123456789012', diaChi: '123 Đường ABC, Quận 1, TP.HCM' },
        { maKhach: 2, hoTen: 'Trần Thị B', dienThoai: '0902345678', email: 'ttb@email.com', cmnd: '234567890123', diaChi: '456 Đường DEF, Quận 2, TP.HCM' },
        { maKhach: 3, hoTen: 'Lê Văn C', dienThoai: '0903456789', email: 'lvc@email.com', cmnd: '345678901234', diaChi: '789 Đường GHI, Quận 3, TP.HCM' },
        { maKhach: 4, hoTen: 'Phạm Thị D', dienThoai: '0904567890', email: 'ptd@email.com', cmnd: '456789012345', diaChi: '321 Đường JKL, Quận 4, TP.HCM' },
        { maKhach: 5, hoTen: 'Hoàng Văn E', dienThoai: '0905678901', email: 'hve@email.com', cmnd: '567890123456', diaChi: '654 Đường MNO, Quận 5, TP.HCM' },
    ],
    
    // Đặt phòng
    bookings: [
        { maDatPhong: 1, maDat: 'DP001', maKhach: 1, maPhong: 2, maLoaiPhong: 1, ngayNhan: '2025-11-10T14:00:00', ngayTra: '2025-11-12T12:00:00', soKhach: 2, trangThai: 'DaNhan', nguoiTao: 2, ghiChu: 'Khách VIP' },
        { maDatPhong: 2, maDat: 'DP002', maKhach: 2, maPhong: 10, maLoaiPhong: 2, ngayNhan: '2025-11-11T14:00:00', ngayTra: '2025-11-13T12:00:00', soKhach: 2, trangThai: 'DaNhan', nguoiTao: 2, ghiChu: '' },
        { maDatPhong: 3, maDat: 'DP003', maKhach: 3, maPhong: 17, maLoaiPhong: 3, ngayNhan: '2025-11-12T14:00:00', ngayTra: '2025-11-14T12:00:00', soKhach: 3, trangThai: 'DaNhan', nguoiTao: 2, ghiChu: 'Gia đình có trẻ em' },
        { maDatPhong: 4, maDat: 'DP004', maKhach: 4, maPhong: 22, maLoaiPhong: 4, ngayNhan: '2025-11-13T14:00:00', ngayTra: '2025-11-15T12:00:00', soKhach: 4, trangThai: 'DaNhan', nguoiTao: 2, ghiChu: '' },
        { maDatPhong: 5, maDat: 'DP005', maKhach: 5, maPhong: 26, maLoaiPhong: 6, ngayNhan: '2025-11-14T14:00:00', ngayTra: '2025-11-16T12:00:00', soKhach: 5, trangThai: 'DaNhan', nguoiTao: 2, ghiChu: 'Gia đình lớn' },
        { maDatPhong: 6, maDat: 'DP006', maKhach: 1, maPhong: null, maLoaiPhong: 1, ngayNhan: '2025-11-20T14:00:00', ngayTra: '2025-11-22T12:00:00', soKhach: 2, trangThai: 'DaDat', nguoiTao: 2, ghiChu: 'Đặt trước' },
    ],
    
    // Dịch vụ
    services: [
        { maDV: 1, ma: 'BREAKFAST', ten: 'Bữa sáng buffet', donGia: 250000, thue: 10 },
        { maDV: 2, ma: 'LUNCH', ten: 'Bữa trưa set menu', donGia: 350000, thue: 10 },
        { maDV: 3, ma: 'DINNER', ten: 'Bữa tối cao cấp', donGia: 500000, thue: 10 },
        { maDV: 4, ma: 'MASSAGE', ten: 'Massage thư giãn 60 phút', donGia: 800000, thue: 10 },
        { maDV: 5, ma: 'SPA', ten: 'Gói spa toàn thân', donGia: 1200000, thue: 10 },
        { maDV: 6, ma: 'LAUNDRY', ten: 'Giặt là nhanh', donGia: 150000, thue: 10 },
        { maDV: 7, ma: 'AIRPORT', ten: 'Đưa đón sân bay', donGia: 300000, thue: 10 },
        { maDV: 8, ma: 'TOUR-CITY', ten: 'Tour tham quan thành phố', donGia: 800000, thue: 10 },
        { maDV: 9, ma: 'TOUR-BEACH', ten: 'Tour biển cả ngày', donGia: 1200000, thue: 10 },
        { maDV: 10, ma: 'MINIBAR', ten: 'Mini bar phòng', donGia: 200000, thue: 10 },
        { maDV: 11, ma: 'WIFI', ten: 'WiFi cao tốc 24h', donGia: 100000, thue: 0 },
        { maDV: 12, ma: 'GYM', ten: 'Phòng gym 1 ngày', donGia: 200000, thue: 10 },
        { maDV: 13, ma: 'POOL', ten: 'Hồ bơi và jacuzzi', donGia: 150000, thue: 10 },
        { maDV: 14, ma: 'KARAOKE', ten: 'Phòng karaoke 2 tiếng', donGia: 600000, thue: 10 },
        { maDV: 15, ma: 'MEETING', ten: 'Thuê phòng họp 4 tiếng', donGia: 2000000, thue: 10 },
    ],
    
    // Hóa đơn
    invoices: [
        { maHD: 1, soHD: 'HD2025110001', maKhach: 1, maND: 3, ngayLap: '2025-11-10T10:30:00', tongTien: 2350000, hinhThucThanhToan: 'TienMat', soTienDaTra: 2350000 },
        { maHD: 2, soHD: 'HD2025110002', maKhach: 2, maND: 3, ngayLap: '2025-11-11T09:15:00', tongTien: 3000000, hinhThucThanhToan: 'The', soTienDaTra: 3000000 },
        { maHD: 3, soHD: 'HD2025110003', maKhach: 3, maND: 3, ngayLap: '2025-11-12T11:20:00', tongTien: 4800000, hinhThucThanhToan: 'ChuyenKhoan', soTienDaTra: 2000000 },
        { maHD: 4, soHD: 'HD2025110004', maKhach: 4, maND: 3, ngayLap: '2025-11-13T10:45:00', tongTien: 5600000, hinhThucThanhToan: 'TienMat', soTienDaTra: 0 },
        { maHD: 5, soHD: 'HD2025110005', maKhach: 5, maND: 3, ngayLap: '2025-11-14T14:30:00', tongTien: 6500000, hinhThucThanhToan: 'The', soTienDaTra: 0 },
    ],
    
    // Chi tiết hóa đơn
    invoiceDetails: [
        // Hóa đơn 1
        { maCTHD: 1, maHD: 1, maDatPhong: 1, maDV: null, soLuong: 2, donGia: 850000 },
        { maCTHD: 2, maHD: 1, maDatPhong: null, maDV: 1, soLuong: 2, donGia: 250000 },
        { maCTHD: 3, maHD: 1, maDatPhong: null, maDV: 7, soLuong: 1, donGia: 300000 },
        
        // Hóa đơn 2
        { maCTHD: 4, maHD: 2, maDatPhong: 2, maDV: null, soLuong: 2, donGia: 1300000 },
        { maCTHD: 5, maHD: 2, maDatPhong: null, maDV: 1, soLuong: 4, donGia: 250000 },
        
        // Hóa đơn 3
        { maCTHD: 6, maHD: 3, maDatPhong: 3, maDV: null, soLuong: 2, donGia: 1950000 },
        { maCTHD: 7, maHD: 3, maDatPhong: null, maDV: 1, soLuong: 6, donGia: 250000 },
        { maCTHD: 8, maHD: 3, maDatPhong: null, maDV: 4, soLuong: 1, donGia: 800000 },
        
        // Hóa đơn 4
        { maCTHD: 9, maHD: 4, maDatPhong: 4, maDV: null, soLuong: 2, donGia: 2700000 },
        { maCTHD: 10, maHD: 4, maDatPhong: null, maDV: 5, soLuong: 2, donGia: 1200000 },
        
        // Hóa đơn 5
        { maCTHD: 11, maHD: 5, maDatPhong: 5, maDV: null, soLuong: 2, donGia: 3000000 },
        { maCTHD: 12, maHD: 5, maDatPhong: null, maDV: 1, soLuong: 10, donGia: 250000 },
    ],
};

// Helper functions to get demo data
const DemoDataHelper = {
    /**
     * Get user by username and password
     */
    login(username, password) {
        return DEMO_DATA.users.find(u => 
            u.tenDangNhap === username && u.matKhau === password
        );
    },
    
    /**
     * Get room with type info
     */
    getRoomWithType(roomId) {
        const room = DEMO_DATA.rooms.find(r => r.maPhong === roomId);
        if (!room) return null;
        
        const roomType = DEMO_DATA.roomTypes.find(rt => rt.maLoaiPhong === room.maLoaiPhong);
        return { ...room, loaiPhong: roomType };
    },
    
    /**
     * Get booking with full info
     */
    getBookingWithDetails(bookingId) {
        const booking = DEMO_DATA.bookings.find(b => b.maDatPhong === bookingId);
        if (!booking) return null;
        
        const customer = DEMO_DATA.customers.find(c => c.maKhach === booking.maKhach);
        const room = booking.maPhong ? this.getRoomWithType(booking.maPhong) : null;
        const roomType = DEMO_DATA.roomTypes.find(rt => rt.maLoaiPhong === booking.maLoaiPhong);
        
        return {
            ...booking,
            khach: customer,
            phong: room,
            loaiPhong: roomType
        };
    },
    
    /**
     * Get invoice with details
     */
    getInvoiceWithDetails(invoiceId) {
        const invoice = DEMO_DATA.invoices.find(i => i.maHD === invoiceId);
        if (!invoice) return null;
        
        const customer = DEMO_DATA.customers.find(c => c.maKhach === invoice.maKhach);
        const details = DEMO_DATA.invoiceDetails.filter(d => d.maHD === invoiceId);
        
        // Enrich details with service/booking info
        const enrichedDetails = details.map(detail => {
            if (detail.maDV) {
                const service = DEMO_DATA.services.find(s => s.maDV === detail.maDV);
                return { ...detail, dv: service };
            } else if (detail.maDatPhong) {
                const booking = this.getBookingWithDetails(detail.maDatPhong);
                return { ...detail, datPhong: booking };
            }
            return detail;
        });
        
        return {
            ...invoice,
            khach: customer,
            chiTiet: enrichedDetails
        };
    },
    
    /**
     * Get available rooms for date range
     */
    getAvailableRooms(checkIn, checkOut, roomTypeId = null) {
        // Simple logic: return rooms with status SanSang
        let available = DEMO_DATA.rooms.filter(r => r.tinhTrang === 'SanSang');
        
        if (roomTypeId) {
            available = available.filter(r => r.maLoaiPhong === roomTypeId);
        }
        
        return available.map(r => this.getRoomWithType(r.maPhong));
    },
    
    /**
     * Calculate room price for date range
     */
    calculateRoomPrice(roomTypeId, checkIn, checkOut) {
        const price = DEMO_DATA.prices.find(p => p.maLoaiPhong === roomTypeId);
        if (!price) return 0;
        
        const nights = Math.ceil((new Date(checkOut) - new Date(checkIn)) / (1000 * 60 * 60 * 24));
        return price.giaMoiDem * nights;
    }
};

// Export for use in other files
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { DEMO_DATA, DemoDataHelper };
}