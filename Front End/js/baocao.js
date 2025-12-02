// API Configuration
const API_BASE_URL = 'https://localhost:7105';
let currentCharts = { doughnut: null, bar: null };

// Lấy token từ localStorage
function getAuthToken() {
  return localStorage.getItem('token');
}

// Format tiền VNĐ
const formatVND = (value) => {
  return new Intl.NumberFormat('vi-VN', { 
    style: 'currency', 
    currency: 'VND', 
    minimumFractionDigits: 0 
  }).format(value);
};

// Show error message
function showError(message) {
  const container = document.getElementById('error-container');
  container.innerHTML = `<div class="error-message">❌ ${message}</div>`;
  console.error('Error:', message);
}

// Clear error message
function clearError() {
  document.getElementById('error-container').innerHTML = '';
}

// Update filter visibility
document.getElementById('loaiFilter').addEventListener('change', function() {
  const loai = this.value;
  const thangLabel = document.getElementById('thangLabel');
  const thangFilter = document.getElementById('thangFilter');
  const ngayLabel = document.getElementById('ngayLabel');
  const ngayFilter = document.getElementById('ngayFilter');
  const tuanLabel = document.getElementById('tuanLabel');
  const tuanFilter = document.getElementById('tuanFilter');

  thangLabel.style.display = 'none';
  thangFilter.style.display = 'none';
  ngayLabel.style.display = 'none';
  ngayFilter.style.display = 'none';
  tuanLabel.style.display = 'none';
  tuanFilter.style.display = 'none';

  if (loai === 'ngay') {
    thangLabel.style.display = 'inline';
    thangFilter.style.display = 'inline';
    ngayLabel.style.display = 'inline';
    ngayFilter.style.display = 'inline';
  } else if (loai === 'tuan') {
    tuanLabel.style.display = 'inline';
    tuanFilter.style.display = 'inline';
  } else if (loai === 'thang' || loai === 'quy') {
    thangLabel.style.display = 'inline';
    thangFilter.style.display = 'inline';
  }
});

// Load báo cáo
async function loadBaoCao() {
  const loai = document.getElementById('loaiFilter').value;
  const nam = document.getElementById('namFilter').value;
  const thang = document.getElementById('thangFilter').value;
  const ngay = document.getElementById('ngayFilter').value;
  const tuan = document.getElementById('tuanFilter').value;

  // Validate năm
  if (!nam || isNaN(nam)) {
    showError('Vui lòng chọn năm hợp lệ');
    return;
  }

  // Build API URL correctly
  let apiUrl = `${API_BASE_URL}/api-user/HoaDon/BaoCao?loai=${loai}&nam=${nam}`;
  
  if (loai === 'ngay') {
    if (!ngay || isNaN(ngay) || !thang || isNaN(thang)) {
      showError('Vui lòng chọn ngày và tháng hợp lệ');
      return;
    }
    apiUrl += `&thang=${thang}&ngay=${ngay}`;
  } else if (loai === 'tuan') {
    if (!tuan || isNaN(tuan)) {
      showError('Vui lòng chọn tuần hợp lệ');
      return;
    }
    // FIX: Thêm năm cho truy vấn tuần
    apiUrl += `&tuan=${tuan}&nam=${nam}`;
  } else if (loai === 'thang') {
    if (!thang || isNaN(thang)) {
      showError('Vui lòng chọn tháng hợp lệ');
      return;
    }
    apiUrl += `&thang=${thang}`;
  } else if (loai === 'quy') {
    if (!thang || isNaN(thang)) {
      showError('Vui lòng chọn tháng để xác định quý');
      return;
    }
    // FIX: Thêm tháng cho truy vấn quý
    apiUrl += `&thang=${thang}`;
  }

  console.log('API URL:', apiUrl);

  const token = getAuthToken();
  if (!token) {
    showError('⚠️ Bạn chưa đăng nhập! Token không tồn tại trong localStorage.');
    return;
  }

  document.getElementById('loading-container').style.display = 'block';
  clearError();

  try {
    const response = await fetch(apiUrl, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    console.log('Response status:', response.status);

    if (!response.ok) {
      if (response.status === 401) {
        showError('Token hết hạn! Vui lòng đăng nhập lại.');
        localStorage.removeItem('token');
        return;
      }
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const result = await response.json();
    console.log('API Response:', result);

    if (result.success && result.data) {
      // Validate data
      const data = result.data;
      if (data.tongTien === undefined || data.soHoaDon === undefined || data.soPhong === undefined) {
        showError('Dữ liệu trả về không đầy đủ');
        return;
      }
      displayBaoCao(data, loai);
      createCharts(data, loai);
    } else {
      showError(result.message || 'Không thể tải báo cáo');
    }
  } catch (error) {
    console.error('Fetch error:', error);
    showError('Không thể kết nối đến máy chủ! ' + error.message);
  } finally {
    document.getElementById('loading-container').style.display = 'none';
  }
}

// Display báo cáo
function displayBaoCao(data, loai) {
  // Validate and set default values
  const tongTien = data.tongTien || 0;
  const soHoaDon = data.soHoaDon || 0;
  const soPhong = data.soPhong || 0;

  document.getElementById('tongTien').textContent = formatVND(tongTien);
  document.getElementById('soHoaDon').textContent = soHoaDon;
  document.getElementById('soPhong').textContent = soPhong;
  
  const giaTriTB = soHoaDon > 0 ? tongTien / soHoaDon : 0;
  document.getElementById('giaTriTB').textContent = formatVND(giaTriTB);

  // Calculate per day stats
  let soDays = 1;
  if (loai === 'thang') soDays = 30;
  else if (loai === 'quy') soDays = 90;
  else if (loai === 'nam') soDays = 365;
  else if (loai === 'tuan') soDays = 7;

  document.getElementById('statDoanhThuNgay').textContent = formatVND(tongTien / soDays);
  document.getElementById('statHoaDonNgay').textContent = (soHoaDon / soDays).toFixed(1);
  document.getElementById('statPhongNgay').textContent = (soPhong / soDays).toFixed(1);
  
  // Assume 50 total rooms for occupancy calculation
  const tyLeLapDay = soPhong > 0 ? ((soPhong / (50 * soDays)) * 100).toFixed(1) : 0;
  document.getElementById('statTyLe').textContent = tyLeLapDay + '%';

  clearError();
}

// Create charts
function createCharts(data, loai) {
  // Destroy old charts
  if (currentCharts.doughnut) currentCharts.doughnut.destroy();
  if (currentCharts.bar) currentCharts.bar.destroy();

  const tongTien = data.tongTien || 0;
  const soHoaDon = data.soHoaDon || 0;
  const soPhong = data.soPhong || 0;

  // Doughnut Chart - Revenue breakdown
  const doughnutCtx = document.getElementById('doughnutChart').getContext('2d');
  currentCharts.doughnut = new Chart(doughnutCtx, {
    type: 'doughnut',
    data: {
      labels: ['Doanh thu', 'Còn lại'],
      datasets: [{
        data: [tongTien, Math.max(100000000 - tongTien, 0)],
        backgroundColor: [
          'rgba(102, 126, 234, 0.8)',
          'rgba(229, 231, 235, 0.5)'
        ],
        borderColor: [
          'rgba(102, 126, 234, 1)',
          'rgba(229, 231, 235, 1)'
        ],
        borderWidth: 2
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom',
          labels: {
            padding: 20,
            font: { size: 14, weight: 'bold' }
          }
        },
        title: {
          display: true,
          text: 'Phân Bổ Doanh Thu',
          font: { size: 16, weight: 'bold' },
          color: '#1f2937'
        }
      }
    }
  });

  // Bar Chart - Performance metrics
  const barCtx = document.getElementById('barChart').getContext('2d');
  const giaTriTB = soHoaDon > 0 ? (tongTien / soHoaDon) / 100000 : 0;
  
  currentCharts.bar = new Chart(barCtx, {
    type: 'bar',
    data: {
      labels: ['Hóa Đơn', 'Phòng thuê', 'Giá trị TB (x100k)'],
      datasets: [{
        label: 'Số lượng',
        data: [
          soHoaDon, 
          soPhong, 
          Math.round(giaTriTB)
        ],
        backgroundColor: [
          'rgba(59, 130, 246, 0.8)',
          'rgba(139, 92, 246, 0.8)',
          'rgba(245, 158, 11, 0.8)'
        ],
        borderColor: [
          'rgba(59, 130, 246, 1)',
          'rgba(139, 92, 246, 1)',
          'rgba(245, 158, 11, 1)'
        ],
        borderWidth: 2,
        borderRadius: 10
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false
        },
        title: {
          display: true,
          text: 'Chỉ Số Hiệu Suất',
          font: { size: 16, weight: 'bold' },
          color: '#1f2937'
        }
      },
      scales: {
        y: {
          beginAtZero: true,
          grid: {
            color: 'rgba(0, 0, 0, 0.05)'
          }
        },
        x: {
          grid: {
            display: false
          }
        }
      }
    }
  });
}

// Chuyển tab
function showSection(section) {
  document.querySelectorAll('.content').forEach(el => el.classList.remove('active'));
  document.getElementById(section).classList.add('active');
  document.querySelectorAll('.tab-btn').forEach(el => el.classList.remove('active'));
  document.querySelectorAll('.tab-btn')[section === 'reports' ? 0 : 1].classList.add('active');
}

// Check token on load
window.addEventListener('load', () => {
  const token = getAuthToken();
  if (!token) {
    showError('⚠️ Bạn chưa đăng nhập! Vui lòng đăng nhập để xem báo cáo.');
    console.log('Token không tồn tại trong localStorage');
  } else {
    console.log('Token đã tìm thấy:', token.substring(0, 20) + '...');
    loadBaoCao();
  }

  document.getElementById('loaiFilter').dispatchEvent(new Event('change'));
  loadUserInfo();
});

// Lấy thông tin từ token
function loadUserInfo() {
  const token = localStorage.getItem('token');
  if (!token) {
    document.getElementById('userNameDisplay').textContent = 'Guest';
    return;
  }

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const hoTen = payload.HoTen || payload.unique_name || 'User';
    const vaiTro = payload.role || payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || 'Nhân viên';

    const avatarChar = hoTen.charAt(0).toUpperCase();

    // Cập nhật các nơi hiển thị
    document.getElementById('userNameDisplay').textContent = hoTen;
    document.getElementById('dropdownUserName').textContent = hoTen;
    document.getElementById('dropdownUserRole').textContent = vaiTro === 'Admin' ? 'Quản trị viên' : vaiTro;
    document.getElementById('userAvatar').textContent = avatarChar;
    document.getElementById('dropdownAvatar').textContent = avatarChar;
  } catch (e) {
    console.log('Không đọc được token:', e);
  }
}

// Đăng xuất
function logout() {
  if (confirm('Bạn có chắc chắn muốn đăng xuất không?')) {
    localStorage.removeItem('token');
    window.location.href = 'index.html';
  }
}