// ✅ HYBRID MODE: Tự động thử cả Gateway và User API
const GATEWAY_URL = 'https://localhost:7105'; // Gateway API (cho team)
const USER_API_URL = 'https://localhost:7141'; // User API (fallback)
const ADMIN_API_URL = 'https://localhost:7105'; // Admin API (nếu cần)
let API_BASE_URL = GATEWAY_URL; // Bắt đầu với Gateway

let allBookings = [];
let customers = [];
let rooms = [];
let roomTypes = [];
let currentToken = '';
let isApiMode = false;

let currentTab = 'checkin';
let currentBooking = null;

// Trạng thái booking
const BOOKING_STATUS = {
    RESERVED: 'DaDat',
    CHECKED_IN: 'DaNhan',
    CHECKED_OUT: 'DaTra',
    CANCELLED: 'DaHuy'
};

// ========================
// DEMO DATA FALLBACK
// ========================
const demoBookings = [
    {
        maDatPhong: 19,
        maDat: 'DP019',
        maKhach: 19,
        tenKhach: 'Nguyễn Thị Hòa',
        dienThoai: '0918901234',
        maPhong: 4,
        soPhong: '104',
        maLoaiPhong: 'Deluxe',
        ngayNhan: '2024-09-25T14:00:00',
        ngayTra: '2024-09-27T12:00:00',
        soKhach: 1,
        trangThai: 'DaDat',
        ghiChu: 'Solo traveler'
    },
    {
        maDatPhong: 18,
        maDat: 'DP018',
        maKhach: 18,
        tenKhach: 'Trần Thế Thiên',
        dienThoai: '0917890123',
        maPhong: 33,
        soPhong: '303',
        maLoaiPhong: 'VIP',
        ngayNhan: '2024-09-23T16:00:00',
        ngayTra: '2024-09-25T11:00:00',
        soKhach: 4,
        trangThai: 'DaNhan',
        ghiChu: 'Currently staying'
    }
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
// GỌI API - HYBRID MODE
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
// LOAD DỮ LIỆU
// ========================
async function loadInitialData() {
    try {
        showLoading();
        
        console.log('📞 Loading bookings từ API...');
        // ✅ Thử nhiều endpoint khác nhau
        let response;
        try {
            response = await fetchAPI('/api-user/DatPhong'); // Thử admin endpoint trước
        } catch (err) {
            console.warn('⚠️ /api-admin/DatPhong failed, trying /api/DatPhong...');
            response = await fetchAPI('/api-user/DatPhong'); // Fallback sang user endpoint
        }
        
        if (response.success) {
            console.log('✅ API Response:', response);
            
            allBookings = (response.data || []).map(normalizeBookingFromAPI);
            
            // Load thêm dữ liệu khác nếu cần
            await loadCustomersAndRooms();
            
            isApiMode = true;
            enrichBookings();
            switchTab('checkin');
            
            console.log('✅ Đã load xong dữ liệu qua API');
        } else {
            throw new Error(response.message || 'API Error');
        }
    } catch (error) {
        console.error('❌ API Error:', error.message);
        console.warn('⚠️ API không khả dụng, chuyển sang chế độ demo');
        loadDemoData();
    }
}

async function loadCustomersAndRooms() {
    try {
        // Load Khách hàng - Thử nhiều endpoint
        let custResponse;
        try {
            custResponse = await fetchAPI('/api-user/Khach');
        } catch (err) {
            console.warn('⚠️ /api-admin/Khach failed, trying /api/Khach...');
            custResponse = await fetchAPI('/api-user/Khach').catch(() => null);
        }
        
        if (custResponse && custResponse.success) {
            customers = (custResponse.data || []).map(normalizeCustomerFromAPI);
            console.log('✅ Loaded customers:', customers.length);
        } else {
            customers = [];
        }

        // Load Phòng - Thử nhiều endpoint
        let roomsResponse;
        try {
            roomsResponse = await fetchAPI('/api-common/Phong');
        } catch (err) {
            console.warn('⚠️ /api-admin/Phong failed, trying /api/Phong...');
            roomsResponse = await fetchAPI('/api-common/Phong').catch(() => null);
        }
        
        if (roomsResponse && roomsResponse.success) {
            rooms = (roomsResponse.data || []).map(normalizeRoomFromAPI);
            console.log('✅ Loaded rooms:', rooms.length);
        } else {
            rooms = [];
        }

        // Load Loại phòng - Thử nhiều endpoint
        let typesResponse;
        try {
            typesResponse = await fetchAPI('/api-common/LoaiPhong');
        } catch (err) {
            console.warn('⚠️ /api-admin/LoaiPhong failed, trying /api/LoaiPhong...');
            typesResponse = await fetchAPI('/api-common/LoaiPhong').catch(() => null);
        }
        
        if (typesResponse && typesResponse.success) {
            roomTypes = (typesResponse.data || []).map(normalizeRoomTypeFromAPI);
            console.log('✅ Loaded room types:', roomTypes.length);
        } else {
            roomTypes = [];
        }
    } catch (err) {
        console.error('❌ Load customers/rooms error:', err);
    }
}

// ========================
// LOAD DEMO DATA
// ========================
function loadDemoData() {
    allBookings = [...demoBookings];
    customers = [];
    rooms = [];
    roomTypes = [];
    isApiMode = false;
    
    showWarning('Không thể kết nối API. Đang sử dụng dữ liệu demo.');
    enrichBookings();
    switchTab('checkin');
}

// ========================
// NORMALIZE DATA
// ========================
function normalizeBookingFromAPI(b) {
    return {
        maDatPhong: b.maDatPhong || b.MaDatPhong,
        maDat: b.maDat || b.MaDat || `DP${String(b.maDatPhong || b.MaDatPhong).padStart(3, '0')}`,
        maKhach: b.maKhach || b.MaKhach,
        maPhong: b.maPhong || b.MaPhong,
        maLoaiPhong: b.maLoaiPhong || b.MaLoaiPhong,
        ngayNhan: b.ngayNhan || b.NgayNhan,
        ngayTra: b.ngayTra || b.NgayTra,
        soKhach: b.soKhach || b.SoKhach || 1,
        trangThai: b.trangThai || b.TrangThai || BOOKING_STATUS.RESERVED,
        ghiChu: b.ghiChu || b.GhiChu || ''
    };
}

function normalizeCustomerFromAPI(c) {
    return {
        maKhach: c.maKhach || c.MaKhach,
        hoTen: c.hoTen || c.HoTen || '',
        dienThoai: c.dienThoai || c.DienThoai || '',
        email: c.email || c.Email || '',
        diaChi: c.diaChi || c.DiaChi || ''
    };
}

function normalizeRoomFromAPI(r) {
    return {
        maPhong: r.maPhong || r.MaPhong,
        soPhong: r.soPhong || r.SoPhong || r.tenPhong || r.TenPhong || '',
        maLoaiPhong: r.maLoaiPhong || r.MaLoaiPhong,
        tang: r.tang || r.Tang,
        trangThai: r.trangThai || r.TrangThai || ''
    };
}

function normalizeRoomTypeFromAPI(t) {
    return {
        maLoaiPhong: t.maLoaiPhong || t.MaLoaiPhong,
        tenLoaiPhong: t.tenLoaiPhong || t.TenLoaiPhong || t.ten || t.Ten || '',
        soKhachToiDa: t.soKhachToiDa || t.SoKhachToiDa,
        moTa: t.moTa || t.MoTa || ''
    };
}

// ========================
// ENRICH BOOKINGS
// ========================
function enrichBookings() {
    allBookings.forEach(b => {
        const kh = customers.find(c => String(c.maKhach) === String(b.maKhach));
        if (kh) {
            b.tenKhach = kh.hoTen;
            b.dienThoai = kh.dienThoai;
        } else {
            b.tenKhach = `Khách #${b.maKhach || '?'}`;
            b.dienThoai = '';
        }

        const room = rooms.find(r => String(r.maPhong) === String(b.maPhong));
        if (room) {
            b.soPhong = room.soPhong;
            if (!b.maLoaiPhong && room.maLoaiPhong) {
                b.maLoaiPhong = room.maLoaiPhong;
            }
        }

        const rt = roomTypes.find(t => String(t.maLoaiPhong) === String(b.maLoaiPhong));
        if (rt) {
            b.tenLoaiPhong = rt.tenLoaiPhong;
        } else {
            b.tenLoaiPhong = b.maLoaiPhong || '';
        }
    });
}

// ========================
// UI FUNCTIONS
// ========================
function showLoading() {
    const container = document.getElementById('bookingsList');
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

function showSuccess(message) {
    alert('✅ ' + message);
}

function showError(message) {
    alert('❌ ' + message);
}

// ========================
// TAB SWITCHING
// ========================
function switchTab(tab) {
    currentTab = tab;
    
    const tabButtons = document.querySelectorAll('.tab-btn');
    tabButtons.forEach(btn => btn.classList.remove('active'));
    
    if (tab === 'checkin' && tabButtons[0]) tabButtons[0].classList.add('active');
    if (tab === 'staying' && tabButtons[1]) tabButtons[1].classList.add('active');
    if (tab === 'checkout' && tabButtons[2]) tabButtons[2].classList.add('active');
    
    loadBookings(tab);
}

function loadBookings(tab) {
    let filtered = [];
    
    if (tab === 'checkin') {
        filtered = allBookings.filter(b => b.trangThai === BOOKING_STATUS.RESERVED);
    } else if (tab === 'staying') {
        filtered = allBookings.filter(b => b.trangThai === BOOKING_STATUS.CHECKED_IN);
    } else if (tab === 'checkout') {
        filtered = allBookings.filter(b => b.trangThai === BOOKING_STATUS.CHECKED_OUT);
    }
    
    displayBookings(filtered);
}

function displayBookings(bookings) {
    const container = document.getElementById('bookingsList');
    if (!container) return;
    
    if (!bookings || bookings.length === 0) {
        container.innerHTML = '<p style="text-align:center;color:#999;padding:40px;">Không có đặt phòng nào</p>';
        return;
    }
    
    let html = '<table><thead><tr>';
    html += '<th>Mã đặt</th>';
    html += '<th>Khách hàng</th>';
    html += '<th>Phòng</th>';
    html += '<th>Loại phòng</th>';
    html += '<th>Ngày nhận</th>';
    html += '<th>Ngày trả</th>';
    html += '<th>Trạng thái</th>';
    html += '<th>Thao tác</th>';
    html += '</tr></thead><tbody>';
    
    bookings.forEach(b => {
        const statusInfo = getStatusInfo(b.trangThai);
        
        html += '<tr>';
        html += `<td>${b.maDat}</td>`;
        html += `<td>${b.tenKhach || ''}<br><small>${b.dienThoai || ''}</small></td>`;
        html += `<td><strong>${b.soPhong || ''}</strong></td>`;
        html += `<td>${b.tenLoaiPhong || ''}</td>`;
        html += `<td>${formatDateTime(b.ngayNhan)}</td>`;
        html += `<td>${formatDateTime(b.ngayTra)}</td>`;
        html += `<td><span class="status-badge ${statusInfo.cssClass}">${statusInfo.text}</span></td>`;
        html += '<td>';
        
        if (b.trangThai === BOOKING_STATUS.RESERVED) {
            html += `<button class="btn btn-success" onclick="openCheckInModal(${b.maDatPhong})">Check In</button>`;
        } else if (b.trangThai === BOOKING_STATUS.CHECKED_IN) {
            html += `<button class="btn btn-danger" onclick="openCheckOutModal(${b.maDatPhong})">Check Out</button>`;
            html += ` <button class="btn btn-warning" onclick="changeRoom(${b.maDatPhong})">Chuyển phòng</button>`;
        }
        
        html += '</td>';
        html += '</tr>';
    });
    
    html += '</tbody></table>';
    container.innerHTML = html;
}

function getStatusInfo(status) {
    switch (status) {
        case BOOKING_STATUS.RESERVED:
            return { cssClass: 'status-checkin', text: 'Chờ nhận' };
        case BOOKING_STATUS.CHECKED_IN:
            return { cssClass: 'status-staying', text: 'Đang ở' };
        case BOOKING_STATUS.CHECKED_OUT:
            return { cssClass: 'status-checkout', text: 'Đã trả' };
        case BOOKING_STATUS.CANCELLED:
            return { cssClass: 'status-checkout', text: 'Đã hủy' };
        default:
            return { cssClass: 'status-checkin', text: status || 'Không rõ' };
    }
}

function formatDateTime(dateStr) {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toLocaleDateString('vi-VN') + ' ' +
           d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
}

// ========================
// SEARCH
// ========================
function searchBookings() {
    const input = document.getElementById('searchInput');
    if (!input) return;
    
    const q = input.value.trim().toLowerCase();
    if (!q) {
        loadBookings(currentTab);
        return;
    }
    
    const matched = allBookings.filter(b => {
        const inCurrentTab =
            (currentTab === 'checkin' && b.trangThai === BOOKING_STATUS.RESERVED) ||
            (currentTab === 'staying' && b.trangThai === BOOKING_STATUS.CHECKED_IN) ||
            (currentTab === 'checkout' && b.trangThai === BOOKING_STATUS.CHECKED_OUT);
        
        if (!inCurrentTab) return false;
        
        return (
            String(b.maDat || '').toLowerCase().includes(q) ||
            String(b.tenKhach || '').toLowerCase().includes(q) ||
            String(b.soPhong || '').toLowerCase().includes(q)
        );
    });
    
    displayBookings(matched);
}

// ========================
// CHECK IN MODAL
// ========================
function openCheckInModal(maDatPhong) {
    const booking = allBookings.find(b => String(b.maDatPhong) === String(maDatPhong));
    if (!booking) return;
    
    currentBooking = booking;
    
    const modal = document.getElementById('checkinModal');
    const info = document.getElementById('checkinBookingInfo');
    if (!modal || !info) return;
    
    info.innerHTML = `
        <h3>Thông tin đặt phòng</h3>
        <p><strong>Mã đặt:</strong> ${booking.maDat}</p>
        <p><strong>Khách hàng:</strong> ${booking.tenKhach}</p>
        <p><strong>Phòng:</strong> ${booking.soPhong || ''} - ${booking.tenLoaiPhong || ''}</p>
        <p><strong>Thời gian:</strong> ${formatDateTime(booking.ngayNhan)} → ${formatDateTime(booking.ngayTra)}</p>
    `;
    
    document.getElementById('checkinForm').reset();
    modal.classList.add('show');
}

// ========================
// CHECK OUT MODAL
// ========================
function openCheckOutModal(maDatPhong) {
    const booking = allBookings.find(b => String(b.maDatPhong) === String(maDatPhong));
    if (!booking) return;
    
    currentBooking = booking;
    
    const modal = document.getElementById('checkoutModal');
    const info = document.getElementById('checkoutBookingInfo');
    if (!modal || !info) return;
    
    const nights = calculateNights(booking.ngayNhan, booking.ngayTra);
    const roomFee = nights * 800000;
    const serviceFee = 0;
    const total = roomFee + serviceFee;
    
    info.innerHTML = `
        <h3>Thông tin đặt phòng</h3>
        <p><strong>Mã đặt:</strong> ${booking.maDat}</p>
        <p><strong>Khách hàng:</strong> ${booking.tenKhach}</p>
        <p><strong>Phòng:</strong> ${booking.soPhong || ''} - ${booking.tenLoaiPhong || ''}</p>
        <p><strong>Số đêm:</strong> ${nights} đêm</p>
    `;
    
    document.getElementById('checkoutRoomFee').value = roomFee;
    document.getElementById('checkoutServiceFee').value = serviceFee;
    document.getElementById('checkoutTotal').value = total;
    document.getElementById('checkoutAmount').value = total;
    document.getElementById('checkoutPayment').value = '';
    document.getElementById('checkoutNote').value = '';
    
    modal.classList.add('show');
}

function calculateNights(checkIn, checkOut) {
    if (!checkIn || !checkOut) return 1;
    const d1 = new Date(checkIn);
    const d2 = new Date(checkOut);
    const diff = Math.ceil((d2 - d1) / (1000 * 60 * 60 * 24));
    return diff > 0 ? diff : 1;
}

// ========================
// MODAL CLOSE
// ========================
function closeModal(id) {
    const modal = document.getElementById(id);
    if (modal) modal.classList.remove('show');
}

// ========================
// FORM SUBMISSIONS
// ========================
document.addEventListener('DOMContentLoaded', () => {
    const checkinForm = document.getElementById('checkinForm');
    const checkoutForm = document.getElementById('checkoutForm');
    
    if (checkinForm) {
        checkinForm.addEventListener('submit', onCheckinSubmit);
    }
    
    if (checkoutForm) {
        checkoutForm.addEventListener('submit', onCheckoutSubmit);
    }
    
    // Load data
    if (checkAuth()) {
        loadInitialData();
    } else {
        loadDemoData();
    }
});

async function onCheckinSubmit(e) {
    e.preventDefault();
    if (!currentBooking) return;
    
    const cmnd = document.getElementById('checkinCMND')?.value.trim();
    const adults = Number(document.getElementById('checkinAdults')?.value || 1);
    const children = Number(document.getElementById('checkinChildren')?.value || 0);
    const deposit = Number(document.getElementById('checkinDeposit')?.value || 0);
    const note = document.getElementById('checkinNote')?.value || '';
    
    if (!cmnd) {
        showError('Vui lòng nhập CMND/CCCD.');
        return;
    }
    
    const body = {
        MaDatPhong: currentBooking.maDatPhong,
        SoNguoiLon: adults,
        SoTreEm: children,
        CMND: cmnd,
        TienCoc: deposit,
        GhiChu: note
    };
    
    try {
        if (isApiMode) {
            console.log('📤 Check In:', body);
            
            // ✅ Thử nhiều endpoint cho CheckIn
            let response;
            try {
                response = await fetchAPI('/api-admin/CheckIn', {
                    method: 'POST',
                    body: JSON.stringify(body)
                });
            } catch (err) {
                console.warn('⚠️ /api-admin/CheckIn failed, trying /api/CheckIn...');
                response = await fetchAPI('/api/CheckIn', {
                    method: 'POST',
                    body: JSON.stringify(body)
                });
            }
            
            if (response.success) {
                showSuccess('Check in thành công!');
                currentBooking.trangThai = BOOKING_STATUS.CHECKED_IN;
                closeModal('checkinModal');
                loadBookings(currentTab);
            } else {
                throw new Error(response.message || 'Check in thất bại');
            }
        } else {
            // Demo mode
            console.log('📋 Demo Check In:', body);
            showSuccess('Check in thành công (Demo mode)!');
            currentBooking.trangThai = BOOKING_STATUS.CHECKED_IN;
            closeModal('checkinModal');
            loadBookings(currentTab);
        }
    } catch (error) {
        console.error('❌ Check In Error:', error);
        showError('Lỗi: ' + error.message);
    }
}

async function onCheckoutSubmit(e) {
    e.preventDefault();
    if (!currentBooking) return;
    
    const roomFee = Number(document.getElementById('checkoutRoomFee')?.value || 0);
    const serviceFee = Number(document.getElementById('checkoutServiceFee')?.value || 0);
    const total = Number(document.getElementById('checkoutTotal')?.value || 0);
    const payment = document.getElementById('checkoutPayment')?.value || '';
    const amount = Number(document.getElementById('checkoutAmount')?.value || 0);
    const note = document.getElementById('checkoutNote')?.value || '';
    
    if (!payment) {
        showError('Vui lòng chọn phương thức thanh toán.');
        return;
    }
    
    const body = {
        MaDatPhong: currentBooking.maDatPhong,
        TienPhong: roomFee,
        TienDichVu: serviceFee,
        TongTien: total,
        SoTienThanhToan: amount,
        PhuongThucThanhToan: payment,
        GhiChu: note
    };
    
    try {
        if (isApiMode) {
            console.log('📤 Check Out:', body);
            
            // ✅ Thử nhiều endpoint cho CheckOut
            let response;
            try {
                response = await fetchAPI('/api-user/CheckOut', {
                    method: 'POST',
                    body: JSON.stringify(body)
                });
            } catch (err) {
                console.warn('⚠️ /api-user/CheckOut failed, trying /api/CheckOut...');
                response = await fetchAPI('/api/CheckOut', {
                    method: 'POST',
                    body: JSON.stringify(body)
                });
            }
            
            if (response.success) {
                showSuccess('Check out thành công! Hóa đơn đã được tạo.');
                currentBooking.trangThai = BOOKING_STATUS.CHECKED_OUT;
                closeModal('checkoutModal');
                loadBookings(currentTab);
            } else {
                throw new Error(response.message || 'Check out thất bại');
            }
        } else {
            // Demo mode
            console.log('📋 Demo Check Out:', body);
            showSuccess('Check out thành công (Demo mode)!');
            currentBooking.trangThai = BOOKING_STATUS.CHECKED_OUT;
            closeModal('checkoutModal');
            loadBookings(currentTab);
        }
    } catch (error) {
        console.error('❌ Check Out Error:', error);
        showError('Lỗi: ' + error.message);
    }
}

// ========================
// CHUYỂN PHÒNG
// ========================
async function changeRoom(maDatPhong) {
    const booking = allBookings.find(b => String(b.maDatPhong) === String(maDatPhong));
    if (!booking) return;
    
    const newRoom = prompt('Nhập mã phòng mới muốn chuyển tới:', booking.maPhong || '');
    if (!newRoom) return;
    
    const body = {
        MaDatPhong: booking.maDatPhong,
        MaPhongMoi: Number(newRoom)
    };
    
    try {
        if (isApiMode) {
            // ✅ Thử nhiều endpoint cho ChuyenPhong
            let response;
            try {
                response = await fetchAPI('/api-admin/ChuyenPhong', {
                    method: 'POST',
                    body: JSON.stringify(body)
                });
            } catch (err) {
                console.warn('⚠️ /api-admin/ChuyenPhong failed, trying /api/ChuyenPhong...');
                response = await fetchAPI('/api/ChuyenPhong', {
                    method: 'POST',
                    body: JSON.stringify(body)
                });
            }
            
            if (response.success) {
                showSuccess('Chuyển phòng thành công!');
                booking.maPhong = Number(newRoom);
                enrichBookings();
                loadBookings(currentTab);
            } else {
                throw new Error(response.message || 'Chuyển phòng thất bại');
            }
        } else {
            showSuccess('Chuyển phòng thành công (Demo mode)!');
            booking.maPhong = Number(newRoom);
            enrichBookings();
            loadBookings(currentTab);
        }
    } catch (error) {
        console.error('❌ Chuyển phòng Error:', error);
        showError('Lỗi: ' + error.message);
    }
}
checkRole(['Admin', 'LeTan']);
// ========================
// LOGOUT
// ========================
function logout() {
    if (confirm('Bạn có muốn đăng xuất?')) {
        localStorage.removeItem('token');
        window.location.href = '/index.html';
    }
}

// ========================
// EXPORT GLOBAL
// ========================
window.switchTab = switchTab;
window.searchBookings = searchBookings;
window.openCheckInModal = openCheckInModal;
window.openCheckOutModal = openCheckOutModal;
window.closeModal = closeModal;
window.changeRoom = changeRoom;
window.logout = logout;

console.log('🚀 Khởi động checkinout.js - HYBRID MODE');
console.log('🔄 Thử Gateway trước:', GATEWAY_URL);
console.log('🔄 Fallback User API:', USER_API_URL);