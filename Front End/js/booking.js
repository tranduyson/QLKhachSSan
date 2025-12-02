// ✅ HYBRID MODE: Tự động thử cả Gateway và User API
const GATEWAY_URL = 'https://localhost:7105'; // Gateway API (cho team)
const USER_API_URL = 'https://localhost:7141'; // User API (fallback cho bạn)
let API_BASE_URL = GATEWAY_URL; // Bắt đầu với Gateway
let allBookings = [];
let allRooms = [];
let customers = [];
let services = [];
let currentToken = '';
let isApiMode = false;
let selectedRoom = null;

// ========================
// DEMO DATA FALLBACK
// ========================
const demoBookings = [
    { MaDatPhong: 1, MaDat: 'DP001', MaKhach: 1, MaPhong: 101, MaLoaiPhong: 1, NgayNhan: '2024-01-01', NgayTra: '2024-01-03', SoKhach: 2, TrangThai: 'Confirmed', GhiChu: '' },
    { MaDatPhong: 2, MaDat: 'DP002', MaKhach: 2, MaPhong: 102, MaLoaiPhong: 1, NgayNhan: '2024-01-02', NgayTra: '2024-01-05', SoKhach: 1, TrangThai: 'CheckedIn', GhiChu: '' }
];

const demoCustomers = [
    { MaKhach: 1, HoTen: 'Nguyễn Văn A', DienThoai: '0901234567' },
    { MaKhach: 2, HoTen: 'Trần Thị B', DienThoai: '0912345678' }
];

const demoServices = [
    { MaDV: 1, Ten: 'Giặt ủi', DonGia: 50000 },
    { MaDV: 2, Ten: 'Ăn sáng', DonGia: 100000 }
];

// ========================
// KIỂM TRA TOKEN 
// ========================
function checkAuth() {
    currentToken = localStorage.getItem('token');
    
    if (!currentToken) {
        showWarning('Bạn chưa đăng nhập. Đang sử dụng chế độ demo.');
        return false;
    }

    try {
        const tokenPayload = JSON.parse(atob(currentToken.split('.')[1]));
        const role = tokenPayload.role || tokenPayload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        console.log('✅ User role:', role);
        return true;
    } catch (error) {
        console.error('❌ Token parse error:', error);
        return false;
    }
}

// ========================
// GỌI API - HYBRID MODE: Tự động chuyển đổi
// ========================
async function fetchAPI(endpoint, options = {}) {
    if (!currentToken) {
        throw new Error('NO_TOKEN');
    }

    const defaultOptions = {
        headers: {
            'Authorization': `Bearer ${currentToken}`,
            'Content-Type': 'application/json',
            ...options.headers
        }
    };

    // 🔄 Thử Gateway trước
    let fullUrl = `${API_BASE_URL}${endpoint}`;
    console.log('📞 Trying:', fullUrl);

    try {
        const response = await fetch(fullUrl, {
            ...options,
            ...defaultOptions
        });

        if (response.status === 401 || response.status === 403) {
            throw new Error('UNAUTHORIZED');
        }

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.message || `HTTP ${response.status}`);
        }

        const data = await response.json();
        console.log('✅ Response from', API_BASE_URL, ':', data);
        return data;
    } catch (error) {
        // ⚠️ Nếu Gateway lỗi, thử User API
        if (API_BASE_URL === GATEWAY_URL) {
            console.warn('⚠️ Gateway failed, trying User API...');
            API_BASE_URL = USER_API_URL;
            fullUrl = `${API_BASE_URL}${endpoint}`;
            console.log('📞 Retry with:', fullUrl);
            
            try {
                const response = await fetch(fullUrl, {
                    ...options,
                    ...defaultOptions
                });

                if (!response.ok) {
                    throw new Error(`HTTP ${response.status}`);
                }

                const data = await response.json();
                console.log('✅ Response from User API:', data);
                return data;
            } catch (fallbackError) {
                console.error('❌ Both APIs failed:', fallbackError);
                throw fallbackError;
            }
        }
        throw error;
    }
}

// ========================
// LOAD DỮ LIỆU QUA GATEWAY
// ========================
async function loadInitialData() {
    try {
        showLoading();
        
        // ✅ GỌI QUA GATEWAY: /api/DatPhong (routing từ User API)
        console.log('📞 Gọi Gateway:', `${API_BASE_URL}/api-user/DatPhong`);
        const response = await fetchAPI('/api-user/DatPhong');
        
        if (response.success) {
            console.log('✅ API Response:', response);
            
            // Map dữ liệu từ API
            allBookings = (response.data || []).map(booking => ({
                MaDatPhong: booking.MaDatPhong || booking.maDatPhong,
                MaDat: booking.MaDat || booking.maDat,
                MaKhach: booking.MaKhach || booking.maKhach,
                MaPhong: booking.MaPhong || booking.maPhong,
                MaLoaiPhong: booking.MaLoaiPhong || booking.maLoaiPhong,
                NgayNhan: booking.NgayNhan || booking.ngayNhan,
                NgayTra: booking.NgayTra || booking.ngayTra,
                SoKhach: booking.SoKhach || booking.soKhach,
                TrangThai: booking.TrangThai || booking.trangThai,
                GhiChu: booking.GhiChu || booking.ghiChu || ''
            }));

            console.log('✅ Mapped bookings:', allBookings.length);

            // Tạo danh sách phòng từ bookings
            allRooms = generateRoomsFromBookings(allBookings);
            
            // Load thêm customers và services
            await loadCustomersAndServices();
            
            isApiMode = true;
            populateCustomers();
            populateServices();
            displayRooms(allRooms);
            updateStats();
            
            console.log('✅ Đã load xong dữ liệu qua Gateway');
        } else {
            throw new Error(response.message || 'API Error');
        }
    } catch (error) {
        console.error('❌ Gateway API Error:', error.message);
        console.warn('⚠️ API không khả dụng, chuyển sang chế độ demo');
        loadDemoData();
    }
}

async function loadCustomersAndServices() {
    try {
        // ✅ GỌI QUA GATEWAY: /api/Khach (routing từ Admin hoặc User API)
        console.log('📞 Loading customers qua Gateway...');
        let custResponse = await fetchAPI('/api-user/Khach').catch(err => {
            console.warn('⚠️ Gateway không có /api-user/Khach:', err.message);
            return null;
        });
        
        if (custResponse && custResponse.success) {
            customers = (custResponse.data || []).map(k => ({
                MaKhach: k.MaKhach || k.maKhach,
                HoTen: k.HoTen || k.hoTen,
                DienThoai: k.DienThoai || k.dienThoai || k.SDT
            }));
            console.log('✅ Loaded customers:', customers.length);
        } else {
            customers = demoCustomers;
            console.log('⚠️ Dùng demo customers');
        }

        // ✅ GỌI QUA GATEWAY: /api/DichVu (routing từ Admin hoặc User API)
        console.log('📞 Loading services qua Gateway...');
        let servResponse = await fetchAPI('/api-common/DichVu').catch(err => {
            console.warn('⚠️ Gateway không có /api-common/DichVu:', err.message);
            return null;
        });
        
        if (servResponse && servResponse.success) {
            services = (servResponse.data || []).map(s => ({
                MaDV: s.MaDV || s.maDV,
                Ten: s.Ten || s.ten,
                DonGia: s.DonGia || s.donGia
            }));
            console.log('✅ Loaded services:', services.length);
        } else {
            services = demoServices;
            console.log('⚠️ Dùng demo services');
        }
    } catch (err) {
        console.error('❌ Load customers/services error:', err);
        customers = demoCustomers;
        services = demoServices;
    }
}

// ========================
// LOAD DEMO DATA
// ========================
function loadDemoData() {
    allBookings = [...demoBookings];
    allRooms = generateRoomsFromBookings(demoBookings);
    customers = [...demoCustomers];
    services = [...demoServices];
    isApiMode = false;
    
    showWarning('Không thể kết nối API. Đang sử dụng dữ liệu demo.');
    populateCustomers();
    populateServices();
    displayRooms(allRooms);
    updateStats();
}

// ========================
// TẠO DANH SÁCH PHÒNG TỪ BOOKINGS
// ========================
function generateRoomsFromBookings(bookings) {
    const rooms = [];
    const roomMap = new Map();

    bookings.forEach(b => {
        if (b.MaPhong && !roomMap.has(b.MaPhong)) {
            rooms.push({
                MaPhong: b.MaPhong,
                SoPhong: `${b.MaPhong}`,
                MaLoaiPhong: b.MaLoaiPhong,
                TenLoaiPhong: getLoaiPhongName(b.MaLoaiPhong),
                Gia: 500000,
                TrangThai: b.TrangThai
            });
            roomMap.set(b.MaPhong, true);
        }
    });

    // Thêm phòng demo nếu không có dữ liệu
    if (rooms.length === 0) {
        for (let i = 101; i <= 110; i++) {
            rooms.push({
                MaPhong: i,
                SoPhong: `${i}`,
                MaLoaiPhong: 1,
                TenLoaiPhong: 'Standard',
                Gia: 500000,
                TrangThai: 'Confirmed'
            });
        }
    }

    return rooms;
}

function getLoaiPhongName(maLoai) {
    const types = { 
        1: 'Standard', 
        2: 'Superior', 
        3: 'Deluxe', 
        4: 'Junior Suite', 
        5: 'Executive Suite', 
        6: 'Family Room', 
        7: 'Presidential Suite' 
    };
    return types[maLoai] || 'Standard';
}

// ========================
// UI FUNCTIONS
// ========================
function showLoading() {
    const container = document.getElementById('roomsContainer');
    if (container) {
        container.innerHTML = '<div style="text-align:center;padding:40px;color:#666;">Đang tải dữ liệu...</div>';
    }
}

function showWarning(message) {
    const warning = document.createElement('div');
    warning.style.cssText = 'background:#fff3cd;border-left:4px solid #ffc107;color:#856404;padding:15px;border-radius:8px;margin-bottom:20px;';
    warning.innerHTML = `<strong>⚠️ Thông báo:</strong> ${message}`;
    
    const container = document.querySelector('.main-content');
    if (container && container.children.length > 1) {
        container.insertBefore(warning, container.children[1]);
    }
}

function populateCustomers() {
    const sel = document.getElementById("customerId");
    if (!sel) return;
    
    sel.innerHTML = `<option value="">-- Chọn khách hàng --</option>`;

    customers.forEach(k => {
        const hoTen = k.HoTen || k.hoTen || 'N/A';
        const sdt = k.DienThoai || k.dienThoai || k.SDT || '';
        const maKhach = k.MaKhach || k.maKhach;
        sel.innerHTML += `<option value="${maKhach}">${hoTen} - ${sdt}</option>`;
    });
}

function populateServices() {
    const sel = document.getElementById("serviceId");
    if (!sel) return;
    
    sel.innerHTML = "";

    services.forEach(s => {
        const ten = s.Ten || s.ten || 'N/A';
        const gia = s.DonGia || s.donGia || 0;
        const maDV = s.MaDV || s.maDV;
        sel.innerHTML += `<option value="${maDV}">${ten} - ${gia.toLocaleString()}đ</option>`;
    });
}

function updateStats() {
    const total = allRooms.length;
    const available = allRooms.filter(r => 
        ['Confirmed', 'DaDat', 'Pending'].includes(r.TrangThai)
    ).length;
    const occupied = allRooms.filter(r => 
        ['CheckedIn', 'DangSuDung', 'DaNhan'].includes(r.TrangThai)
    ).length;
    const maintenance = allRooms.filter(r => 
        ['Cancelled', 'CheckedOut', 'DaTra', 'BaoTri'].includes(r.TrangThai)
    ).length;

    document.getElementById("totalRooms").textContent = total;
    document.getElementById("availableRooms").textContent = available;
    document.getElementById("occupiedRooms").textContent = occupied;
    document.getElementById("maintenanceRooms").textContent = maintenance;
}

function displayRooms(rooms) {
    const box = document.getElementById("roomsContainer");
    
    if (!box) return;
    
    if (rooms.length === 0) {
        box.innerHTML = '<p style="text-align:center;color:#999;padding:40px;">Không có phòng nào</p>';
        return;
    }

    box.innerHTML = "";

    rooms.forEach(r => {
        const status = mapStatus(r.TrangThai);
        const soPhong = r.SoPhong || r.soPhong || 'N/A';
        const tenLoai = r.TenLoaiPhong || r.tenLoaiPhong || 'Standard';
        const gia = r.Gia || r.gia || 0;
        const maPhong = r.MaPhong || r.maPhong;

        const card = document.createElement('div');
        card.className = `room-card ${status.class}`;
        card.onclick = () => openBookingModal(maPhong);
        card.innerHTML = `
            <div class="room-number">Phòng ${soPhong}</div>
            <div class="room-type">${tenLoai}</div>
            <div class="room-price">${gia.toLocaleString()}đ</div>
            <span class="room-status status-${status.class}">
                ${status.text}
            </span>
        `;
        box.appendChild(card);
    });
}

function mapStatus(code) {
    const statusMap = {
        'Confirmed': { class: 'available', text: 'Đã đặt' },
        'DaDat': { class: 'available', text: 'Đã đặt' },
        'Pending': { class: 'available', text: 'Chờ xác nhận' },
        'CheckedIn': { class: 'occupied', text: 'Đang ở' },
        'DangSuDung': { class: 'occupied', text: 'Đang ở' },
        'DaNhan': { class: 'occupied', text: 'Đã nhận' },
        'CheckedOut': { class: 'maintenance', text: 'Đã trả' },
        'DaTra': { class: 'maintenance', text: 'Đã trả' },
        'Cancelled': { class: 'maintenance', text: 'Đã hủy' },
        'BaoTri': { class: 'maintenance', text: 'Bảo trì' }
    };

    return statusMap[code] || { class: 'available', text: 'Trống' };
}

// ========================
// MODAL & BOOKING
// ========================
window.openBookingModal = function (maPhong) {
    selectedRoom = allRooms.find(r => (r.MaPhong || r.maPhong) == maPhong);

    if (!selectedRoom) {
        alert('Không tìm thấy phòng!');
        return;
    }

    const soPhong = selectedRoom.SoPhong || selectedRoom.soPhong || 'N/A';
    const tenLoai = selectedRoom.TenLoaiPhong || selectedRoom.tenLoaiPhong || 'Standard';
    const gia = selectedRoom.Gia || selectedRoom.gia || 0;

    document.getElementById("selectedRoomDetails").innerHTML = `
        <h3>Phòng ${soPhong}</h3>
        <p>Loại: ${tenLoai}</p>
        <p>Giá: <strong>${gia.toLocaleString()}đ</strong>/đêm</p>
    `;

    document.getElementById("bookingModal").classList.add("show");
};

window.closeModal = function () {
    document.getElementById("bookingModal").classList.remove("show");
    document.getElementById("bookingForm").reset();
    document.getElementById("modalMessage").innerHTML = '';
    selectedRoom = null;
};

// ========================
// SUBMIT BOOKING - GỌI QUA GATEWAY
// ========================
document.getElementById("bookingForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    if (!selectedRoom) {
        alert('Vui lòng chọn phòng!');
        return;
    }

    const maKhach = Number(document.getElementById("customerId").value);
    if (!maKhach) {
        alert('Vui lòng chọn khách hàng!');
        return;
    }

    // ✅ MAP SANG FORMAT C# API - GIỐNG USERS.JS
    const bookingData = {
        MaDat: "DP" + Date.now(),
        MaKhach: maKhach,
        MaPhong: selectedRoom.MaPhong || selectedRoom.maPhong,
        MaLoaiPhong: selectedRoom.MaLoaiPhong || selectedRoom.maLoaiPhong || 1,
        NgayNhan: document.getElementById("checkInDateTime").value,
        NgayTra: document.getElementById("checkOutDateTime").value,
        SoKhach: Number(document.getElementById("adults").value) + Number(document.getElementById("children").value),
        TrangThai: "DaDat",
        GhiChu: document.getElementById("notes").value || ""
    };

    const msg = document.getElementById("modalMessage");

    try {
        if (isApiMode) {
            console.log('📤 Creating booking qua Gateway:', bookingData);
            
            // ✅ GỌI QUA GATEWAY: POST /api/DatPhong (routing tới User API)
            const response = await fetchAPI('/api-user/DatPhong', {
                method: 'POST',
                body: JSON.stringify(bookingData)
            });

            console.log('📥 API Response:', response);

            if (response.success) {
                msg.className = "success-message";
                msg.textContent = "✅ Đặt phòng thành công!";
                setTimeout(() => {
                    closeModal();
                    loadInitialData(); // Reload data
                }, 1500);
            } else {
                throw new Error(response.message || 'Đặt phòng thất bại');
            }
        } else {
            // Demo mode
            console.log('📋 Demo booking:', bookingData);
            msg.className = "success-message";
            msg.textContent = "✅ Đặt phòng thành công (Demo mode)!";
            setTimeout(closeModal, 1500);
        }
    } catch (error) {
        console.error('❌ Booking Error:', error);
        msg.className = "error-message";
        msg.textContent = "❌ Lỗi: " + error.message;
    }
});

// ========================
// FILTER FUNCTIONS
// ========================
window.filterRooms = function() {
    const typeFilter = document.getElementById('roomTypeFilter')?.value || '';
    const statusFilter = document.getElementById('statusFilter')?.value || '';

    let filtered = allRooms.filter(r => {
        const matchType = !typeFilter || (r.TenLoaiPhong || r.tenLoaiPhong) === typeFilter;
        const status = mapStatus(r.TrangThai);
        const matchStatus = !statusFilter || status.class === statusFilter;
        return matchType && matchStatus;
    });

    displayRooms(filtered);
};

window.toggleApiNotes = function() {
    const notes = document.getElementById('apiNotes');
    if (notes) {
        notes.style.display = notes.style.display === 'none' ? 'block' : 'none';
    }
};

// ========================
// LOGOUT
// ========================
function logout() {
    if (confirm('Bạn có muốn đăng xuất?')) {
        localStorage.removeItem('token');
        window.location.href = '/index.html';
    }
}
checkRole(['Admin','LeTan','KeToan']);
// ========================
// INITIALIZE
// ========================
window.addEventListener('DOMContentLoaded', () => {
    console.log('🚀 Khởi động booking.js - HYBRID MODE');
    console.log('🔄 Thử Gateway trước:', GATEWAY_URL);
    console.log('🔄 Fallback User API:', USER_API_URL);
    
    if (checkAuth()) {
        loadInitialData();
    } else {
        loadDemoData();
    }
});