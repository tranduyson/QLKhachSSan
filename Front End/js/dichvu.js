const API_BASE_URL = 'https://localhost:7105';
let allServices = [];
let currentToken = localStorage.getItem('token') || '';
let currentHoaDon = null;
let usedServices = [];

// Gọi API chuẩn
async function callAPI(endpoint, options = {}) {
    const headers = { 'Content-Type': 'application/json' };
    if (currentToken) headers['Authorization'] = `Bearer ${currentToken}`;
    
    try {
        const res = await fetch(`${API_BASE_URL}${endpoint}`, { ...options, headers });
        if (!res.ok) {
            const err = await res.text();
            throw new Error(err || `HTTP ${res.status}`);
        }
        return await res.json();
    } catch (error) {
        console.error('API Error:', error);
        throw error;
    }
}

// Hiển thị thông báo
function showAlert(message, type = 'info') {
    const container = document.getElementById('alertContainer');
    const alertClass = type === 'success' ? 'alert-success' : type === 'error' ? 'alert-error' : 'alert-info';
    container.innerHTML = `<div class="alert ${alertClass}">${message}</div>`;
    setTimeout(() => container.innerHTML = '', 5000);
}

// ======================== LOAD DỊCH VỤ ========================
async function loadServices() {
    try {
        const res = await callAPI('/api-common/DichVu');
        allServices = (res.success ? res.data : []).map(sv => ({
            maDV: sv.maDV,
            maCode: (sv.ma || '').toUpperCase(),
            tenDV: sv.ten || 'Dịch vụ',
            donGia: parseFloat(sv.donGia) || 0,
            thue: parseFloat(sv.thue) || 0
        }));
        displayServices();
        populateServiceSelect();
    } catch (e) {
        console.error('Lỗi load dịch vụ:', e);
        showAlert('Không tải được danh sách dịch vụ', 'error');
        // Fallback data
        allServices = [
            { maDV: 8, maCode: 'SPA', tenDV: 'Massage thư giãn', donGia: 800000, thue: 10 },
            { maDV: 9, maCode: 'TOUR-BEACH', tenDV: 'Tour biển cả ngày', donGia: 1200000, thue: 10 }
        ];
        displayServices();
        populateServiceSelect();
    }
}

function displayServices() {
    const grid = document.getElementById('serviceGrid');
    if (!allServices.length) {
        grid.innerHTML = `<div class="empty-state"><div class="empty-state-icon">📋</div><p>Chưa có dịch vụ nào</p></div>`;
        return;
    }
    grid.innerHTML = allServices.map(sv => `
        <div class="service-card">
            <div style="display: flex; justify-content: space-between; align-items: start; margin-bottom: 10px;">
                <span class="service-code">Mã: ${sv.maCode}</span>
            </div>
            <div class="service-name">${sv.tenDV}</div>
            <div class="service-price">${formatPrice(sv.donGia)}</div>
            <div class="service-tax">Thuế: ${sv.thue}%</div>
            <div class="service-actions">
                <button class="btn btn-warning" onclick="openEditService(${sv.maDV})">Sửa</button>
                <button class="btn btn-danger" onclick="deleteService(${sv.maDV})">Xóa</button>
            </div>
        </div>
    `).join('');
}

// ======================== CRUD DỊCH VỤ ========================
function openAddModal() {
    document.getElementById('modalTitle').textContent = 'Thêm Dịch Vụ Mới';
    document.getElementById('serviceForm').reset();
    document.getElementById('serviceId').value = '';
    showModal('serviceModal');
}

function openEditService(maDV) {
    const sv = allServices.find(s => s.maDV === maDV);
    if (!sv) return;
    document.getElementById('modalTitle').textContent = 'Sửa Dịch Vụ';
    document.getElementById('serviceId').value = sv.maDV;
    document.getElementById('serviceMa').value = sv.maCode;
    document.getElementById('serviceTen').value = sv.tenDV;
    document.getElementById('serviceDonGia').value = sv.donGia;
    document.getElementById('serviceThue').value = sv.thue;
    showModal('serviceModal');
}

async function saveService(e) {
    e.preventDefault();
    const id = document.getElementById('serviceId').value;
    const data = {
        maDV: id ? parseInt(id) : undefined,
        ma: document.getElementById('serviceMa').value.trim().toUpperCase(),
        ten: document.getElementById('serviceTen').value.trim(),
        donGia: parseFloat(document.getElementById('serviceDonGia').value),
        thue: parseFloat(document.getElementById('serviceThue').value)
    };
    
    if (!data.ten || isNaN(data.donGia)) {
        return showAlert('Vui lòng nhập đầy đủ!', 'error');
    }

    try {
        if (id) {
            await callAPI(`/api-common/DichVu/${id}`, { method: 'PUT', body: JSON.stringify(data) });
        } else {
            delete data.maDV;
            await callAPI('/api-common/DichVu', { method: 'POST', body: JSON.stringify(data) });
        }
        closeModal('serviceModal');
        loadServices();
        showAlert('Lưu dịch vụ thành công!', 'success');
    } catch (err) {
        showAlert('Lỗi lưu dịch vụ: ' + err.message, 'error');
    }
}

async function deleteService(maDV) {
    if (!confirm('Xóa dịch vụ này?')) return;
    try {
        await callAPI(`/api-common/DichVu/${maDV}`, { method: 'DELETE' });
        loadServices();
        showAlert('Xóa dịch vụ thành công!', 'success');
    } catch (err) {
        showAlert('Lỗi xóa dịch vụ: ' + err.message, 'error');
    }
}

// ======================== TAB SỬ DỤNG DỊCH VỤ ========================

// Load danh sách hóa đơn
// ======================== LOAD CHỈ HÓA ĐƠN CHƯA THANH TOÁN ========================
async function loadBookings() {
    const select = document.getElementById('bookingSelect');
    select.innerHTML = '<option value="">-- Đang tải... --</option>';

    try {
        const res = await callAPI('/api-user/HoaDon');
        if (!res.success) throw new Error(res.message || 'Lỗi tải danh sách hóa đơn');

        const hoaDons = res.data || [];

        // === LỌC CHỈ NHỮNG HÓA ĐƠN CÒN NỢ ===
        const hoaDonChuaThanhToan = hoaDons.filter(hd => {
            const conNo = hd.soTienConNo ?? (hd.tongTien - (hd.soTienDaTra || 0));
            return conNo > 0;
        });

        // Sắp xếp theo ngày lập mới nhất trước
        hoaDonChuaThanhToan.sort((a, b) => new Date(b.ngayLap) - new Date(a.ngayLap));

        if (hoaDonChuaThanhToan.length === 0) {
            select.innerHTML = '<option value="">-- Không có hóa đơn nào còn nợ --</option>';
            showAlert('Tất cả hóa đơn đã được thanh toán xong!', 'info');
            return;
        }

        select.innerHTML = '<option value="">-- Chọn hóa đơn cần thêm dịch vụ --</option>';

        hoaDonChuaThanhToan.forEach(hd => {
            const conNo = hd.soTienConNo ?? (hd.tongTien - (hd.soTienDaTra || 0));

            const opt = document.createElement('option');
            opt.value = hd.maHD;
            opt.textContent = `HD${hd.soHD} - Khách ${hd.maKhach} - Còn nợ: ${formatPrice(conNo)}`;
            select.appendChild(opt);
        });

        showAlert(`Đã tải ${hoaDonChuaThanhToan.length} hóa đơn còn nợ`, 'success');
    } catch (err) {
        console.error('Lỗi loadBookings:', err);
        select.innerHTML = '<option value="">Lỗi tải danh sách</option>';
        showAlert('Không tải được danh sách hóa đơn: ' + err.message, 'error');
    }
}

// Khi chọn hóa đơn → load thông tin + dịch vụ đã dùng
async function loadBookingServices() {
    const maHD = document.getElementById('bookingSelect').value;
    if (!maHD) {
        document.getElementById('selectedBookingInfo').style.display = 'none';
        document.getElementById('serviceUsageList').innerHTML = '';
        document.getElementById('summarySection').style.display = 'none';
        document.getElementById('addServiceBtn').style.display = 'none';
        currentHoaDon = null;
        usedServices = [];
        return;
    }

    try {
        // Lấy thông tin hóa đơn
        const hdRes = await callAPI(`/api-user/HoaDon/${maHD}`);
        if (!hdRes.success) throw new Error(hdRes.message);
        currentHoaDon = hdRes.data;

        // Lấy chi tiết hóa đơn (bao gồm cả phòng và dịch vụ)
        const ctRes = await callAPI(`/api-user/HoaDon/Getpayment/${maHD}`);
        if (!ctRes.success) throw new Error(ctRes.message);

        // Lọc chỉ lấy dịch vụ (maDV != null)
        usedServices = (ctRes.data?.chiTiet || []).filter(ct => ct.maDV != null);

        displaySelectedBookingInfo();
        displayUsedServices();
        document.getElementById('addServiceBtn').style.display = 'block';
    } catch (err) {
        console.error('Chi tiết lỗi:', err);
        showAlert('Lỗi tải thông tin: ' + err.message, 'error');
    }
}

function displaySelectedBookingInfo() {
    if (!currentHoaDon) return;
    const infoDiv = document.getElementById('selectedBookingInfo');
    infoDiv.style.display = 'block';
    
    const conNo = parseFloat(currentHoaDon.soTienConNo) || 0;
    infoDiv.innerHTML = `
        <strong>Hóa đơn:</strong> ${currentHoaDon.soHD} | 
        <strong>Khách:</strong> ${currentHoaDon.maKhach} | 
        <strong>Tổng tiền hiện tại:</strong> ${formatPrice(currentHoaDon.tongTien)} 
        ${conNo > 0 ? `<span style="color:#dc3545"> (Còn nợ: ${formatPrice(conNo)})</span>` : ''}
    `;
    document.getElementById('summarySection').style.display = 'block';
}

function displayUsedServices() {
    const container = document.getElementById('serviceUsageList');
    if (!usedServices.length) {
        container.innerHTML = '<p style="color:#999; text-align:center; padding:20px;">Chưa sử dụng dịch vụ nào</p>';
        return;
    }

    container.innerHTML = usedServices.map(item => {
        const sv = allServices.find(s => s.maDV === item.maDV) || { tenDV: 'Không rõ', donGia: item.donGia };
        const thanhTien = (parseFloat(item.donGia) || 0) * (parseInt(item.soLuong) || 0);
        return `
            <div class="service-usage-item">
                <div class="info">
                    <strong>${sv.tenDV}</strong><br>
                    <small>${item.soLuong} × ${formatPrice(item.donGia)} = ${formatPrice(thanhTien)}</small>
                </div>
                <div class="price">${formatPrice(thanhTien)}</div>
                <div class="actions">
                    <button class="btn btn-danger btn-sm" onclick="removeService(${item.maCTHD})">Xóa</button>
                </div>
            </div>
        `;
    }).join('');
}

// Đổ dữ liệu dịch vụ vào select
function populateServiceSelect() {
    const select = document.getElementById('usageServiceId');
    select.innerHTML = '<option value="">-- Chọn dịch vụ --</option>';
    allServices.forEach(sv => {
        const opt = document.createElement('option');
        opt.value = sv.maDV;
        opt.textContent = `${sv.tenDV} - ${formatPrice(sv.donGia)}`;
        select.appendChild(opt);
    });
}

function updateServicePreview() {
    const maDV = +document.getElementById('usageServiceId').value;
    const sl = +document.getElementById('usageQuantity').value || 1;
    const preview = document.getElementById('servicePreview');
    
    if (!maDV) {
        preview.style.display = 'none';
        return;
    }
    
    const sv = allServices.find(s => s.maDV === maDV);
    if (!sv) return;
    
    const thanhTien = sv.donGia * sl;
    preview.style.display = 'block';
    preview.innerHTML = `<strong>Tạm tính:</strong> ${sl} × ${formatPrice(sv.donGia)} = <strong style="color:#28a745;">${formatPrice(thanhTien)}</strong>`;
}

function openAddServiceModal() {
    if (!currentHoaDon) {
        showAlert('Vui lòng chọn hóa đơn trước!', 'error');
        return;
    }
    document.getElementById('serviceUsageForm').reset();
    document.getElementById('servicePreview').style.display = 'none';
    document.getElementById('usageServiceId').value = '';
    document.getElementById('usageQuantity').value = 1;
    showModal('addServiceUsageModal');
}

// Thêm dịch vụ vào hóa đơn
async function addServiceToBooking(e) {
    e.preventDefault();
    const btn = document.getElementById('submitServiceBtn');
    btn.disabled = true;
    btn.innerHTML = 'Đang xử lý...';

    const maDV = +document.getElementById('usageServiceId').value;
    const sl = +document.getElementById('usageQuantity').value;

    if (!maDV || !sl || !currentHoaDon) {
        showAlert('Vui lòng chọn dịch vụ và số lượng!', 'error');
        btn.disabled = false;
        btn.textContent = 'Xác Nhận Thêm';
        return;
    }

    const sv = allServices.find(s => s.maDV === maDV);
    if (!sv) {
        showAlert('Dịch vụ không tồn tại!', 'error');
        btn.disabled = false;
        btn.textContent = 'Xác Nhận Thêm';
        return;
    }

    const thanhTien = sv.donGia * sl;
    const chiTiet = {
        maHD: currentHoaDon.maHD,
        maDV: maDV,
        maDatPhong: null,
        soLuong: sl,
        donGia: sv.donGia,
        thanhTien: thanhTien
    };

    try {
        // 1. Thêm chi tiết hóa đơn
        const ctRes = await callAPI('/api-user/ChiTietHoaDon', {
            method: 'POST',
            body: JSON.stringify(chiTiet)
        });
        if (!ctRes.success) throw new Error(ctRes.message || 'Thêm chi tiết thất bại');

        // 2. Cập nhật tổng tiền hóa đơn
        const newTotal = (parseFloat(currentHoaDon.tongTien) || 0) + thanhTien;
        const newConNo = (parseFloat(currentHoaDon.soTienConNo) || 0) + thanhTien;
        
        const updateHD = {
            maHD: currentHoaDon.maHD,
            soHD: currentHoaDon.soHD,
            maKhach: currentHoaDon.maKhach,
            maND: currentHoaDon.maND,
            ngayLap: currentHoaDon.ngayLap,
            tongTien: newTotal,
            hinhThucThanhToan: currentHoaDon.hinhThucThanhToan || '',
            soTienDaTra: currentHoaDon.soTienDaTra || 0,
            soTienConNo: newConNo
        };

        const hdRes = await callAPI(`/api-user/HoaDon/${currentHoaDon.maHD}`, {
            method: 'PUT',
            body: JSON.stringify(updateHD)
        });
        if (!hdRes.success) throw new Error(hdRes.message || 'Cập nhật hóa đơn thất bại');

        currentHoaDon.tongTien = newTotal;
        currentHoaDon.soTienConNo = newConNo;

        showAlert(`Đã thêm "${sv.tenDV}" × ${sl} - Tổng mới: ${formatPrice(newTotal)}`, 'success');
        closeModal('addServiceUsageModal');
        await loadBookingServices();
    } catch (err) {
        console.error(err);
        showAlert('Lỗi: ' + err.message, 'error');
    } finally {
        btn.disabled = false;
        btn.textContent = 'Xác Nhận Thêm';
    }
}

// Xóa dịch vụ khỏi hóa đơn
async function removeService(maCTHD) {
    if (!confirm('Xóa dịch vụ này khỏi hóa đơn?')) return;

    try {
        const item = usedServices.find(x => x.maCTHD === maCTHD);
        if (!item) throw new Error('Không tìm thấy dịch vụ');

        const thanhTien = (parseFloat(item.donGia) || 0) * (parseInt(item.soLuong) || 0);

        // 1. Xóa chi tiết
        const delRes = await callAPI(`/api-user/ChiTietHoaDon/${maCTHD}`, { method: 'DELETE' });
        if (!delRes.success) throw new Error(delRes.message || 'Xóa chi tiết thất bại');

        // 2. Cập nhật lại tổng tiền
        const newTotal = (parseFloat(currentHoaDon.tongTien) || 0) - thanhTien;
        const newConNo = (parseFloat(currentHoaDon.soTienConNo) || 0) - thanhTien;
        
        const updateHD = {
            maHD: currentHoaDon.maHD,
            soHD: currentHoaDon.soHD,
            maKhach: currentHoaDon.maKhach,
            maND: currentHoaDon.maND,
            ngayLap: currentHoaDon.ngayLap,
            tongTien: newTotal,
            hinhThucThanhToan: currentHoaDon.hinhThucThanhToan || '',
            soTienDaTra: currentHoaDon.soTienDaTra || 0,
            soTienConNo: newConNo
        };

        const hdRes = await callAPI(`/api-user/HoaDon/${currentHoaDon.maHD}`, {
            method: 'PUT',
            body: JSON.stringify(updateHD)
        });
        if (!hdRes.success) throw new Error(hdRes.message || 'Cập nhật hóa đơn thất bại');

        currentHoaDon.tongTien = newTotal;
        currentHoaDon.soTienConNo = newConNo;

        showAlert(`Đã xóa dịch vụ - Tổng mới: ${formatPrice(newTotal)}`, 'success');
        await loadBookingServices();
    } catch (err) {
        console.error(err);
        showAlert('Lỗi xóa dịch vụ: ' + err.message, 'error');
    }
}

// ======================== MODAL & TAB ========================
function showModal(id) {
    document.getElementById(id).classList.add('show');
}

function closeModal(id) {
    document.getElementById(id).classList.remove('show');
}

function switchTab(tab) {
    document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
    document.querySelectorAll('.content-card').forEach(c => c.style.display = 'none');

    if (tab === 'list') {
        document.querySelector('.tab-btn:first-child').classList.add('active');
        document.getElementById('listTab').style.display = 'block';
    } else {
        document.querySelector('.tab-btn:last-child').classList.add('active');
        document.getElementById('usageTab').style.display = 'block';
        loadBookings();
    }
}

function formatPrice(v) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    }).format(v || 0);
}

function logout() {
    if (confirm('Bạn có muốn đăng xuất?')) {
        localStorage.removeItem('token');
        window.location.href = '/index.html';
    }
}

checkRole(['Admin', 'KeToan','LeTan']);
// ======================== KHỞI TẠO ========================
window.onload = () => {
    loadServices();
    switchTab('list');
};