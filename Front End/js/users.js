// users.js - Sửa lỗi mapping dữ liệu với API
const API_BASE_URL = 'https://localhost:7105';
let allUsers = [];
let currentToken = '';
let isApiMode = false;
let editingUserId = null;

// Demo data fallback
const demoUsers = [
    { maNguoiDung: 1, tenDangNhap: 'admin', hoTen: 'Quản trị viên', vaiTro: 'Admin', email: 'admin@hotel.com', soDienThoai: '0901234567', trangThai: 'Active' },
    { maNguoiDung: 2, tenDangNhap: 'letan01', hoTen: 'Nguyễn Văn A', vaiTro: 'LeTan', email: 'letan@hotel.com', soDienThoai: '0912345678', trangThai: 'Active' },
    { maNguoiDung: 3, tenDangNhap: 'ketoan01', hoTen: 'Trần Thị B', vaiTro: 'KeToan', email: 'ketoan@hotel.com', soDienThoai: '0923456789', trangThai: 'Active' },
    { maNguoiDung: 4, tenDangNhap: 'khach01', hoTen: 'Lê Văn C', vaiTro: 'Khach', email: 'khach@gmail.com', soDienThoai: '0934567890', trangThai: 'Inactive' }
];

// Kiểm tra quyền Admin
function checkAdminAuth() {
    currentToken = localStorage.getItem('token');
    
    if (!currentToken) {
        showWarning('Bạn chưa đăng nhập. Đang sử dụng chế độ demo.');
        return false;
    }

    try {
        const tokenPayload = JSON.parse(atob(currentToken.split('.')[1]));
        const role = tokenPayload.role || tokenPayload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
        
        if (role !== 'Admin') {
            showError('Bạn không có quyền truy cập trang này!');
            setTimeout(() => {
                window.location.href = '/hoadon.html';
            }, 2000);
            return false;
        }
        
        return true;
    } catch (error) {
        console.error('Token parse error:', error);
        return false;
    }
}

// Gọi API với xử lý lỗi
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

    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
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
    return data;
}

// Load danh sách người dùng
async function loadUsers() {
    try {
        showLoading();
        
        // Thử kết nối với endpoint gốc của bạn
        const response = await fetchAPI('/api-admin/NguoiDung');
        
        if (response.success) {
            // Map từ API format sang frontend format
            allUsers = (response.data || []).map(user => ({
                maNguoiDung: user.maND || user.MaND,
                tenDangNhap: user.tenDangNhap || user.TenDangNhap,
                hoTen: user.hoTen || user.HoTen,
                vaiTro: user.vaiTro || user.VaiTro,
                email: user.email || user.Email || '',
                soDienThoai: user.soDienThoai || user.SoDienThoai || '',
                trangThai: 'Active'
            }));
            
            isApiMode = true;
            displayUsers(allUsers);
            updateStats();
        } else {
            throw new Error(response.message || 'API Error');
        }
    } catch (error) {
        console.error('❌ API Error:', error);
        console.warn('API không khả dụng, chuyển sang chế độ demo:', error.message);
        loadDemoData();
    }
}

// Load dữ liệu demo
function loadDemoData() {
    allUsers = [...demoUsers];
    isApiMode = false;
    showWarning('Không thể kết nối API. Đang sử dụng dữ liệu demo.');
    displayUsers(allUsers);
    updateStats();
}

// Hiển thị danh sách người dùng
function displayUsers(users) {
    const container = document.getElementById('usersTableContainer');
    
    if (users.length === 0) {
        container.innerHTML = '<p style="text-align: center; color: #999; padding: 40px;">Không có người dùng nào</p>';
        return;
    }

    let html = `
        <table style="width: 100%; border-collapse: collapse; margin-top: 15px;">
            <thead>
                <tr style="background: #f8f9fa;">
                    <th style="padding: 12px; text-align: left; border-bottom: 2px solid #dee2e6;">Username</th>
                    <th style="padding: 12px; text-align: left; border-bottom: 2px solid #dee2e6;">Họ tên</th>
                    <th style="padding: 12px; text-align: left; border-bottom: 2px solid #dee2e6;">Vai trò</th>
                    <th style="padding: 12px; text-align: left; border-bottom: 2px solid #dee2e6;">Email</th>
                    <th style="padding: 12px; text-align: left; border-bottom: 2px solid #dee2e6;">SĐT</th>
                    <th style="padding: 12px; text-align: center; border-bottom: 2px solid #dee2e6;">Thao tác</th>
                </tr>
            </thead>
            <tbody>
    `;

    users.forEach(user => {
        const roleColor = {
            'Admin': '#dc3545',
            'LeTan': '#17a2b8',
            'KeToan': '#ffc107',
            'Khach': '#28a745'
        }[user.vaiTro] || '#6c757d';

        html += `
            <tr style="border-bottom: 1px solid #dee2e6;">
                <td style="padding: 12px;"><strong>${user.tenDangNhap}</strong></td>
                <td style="padding: 12px;">${user.hoTen}</td>
                <td style="padding: 12px;">
                    <span style="background: ${roleColor}; color: white; padding: 4px 10px; border-radius: 12px; font-size: 12px; font-weight: 600;">
                        ${getRoleText(user.vaiTro)}
                    </span>
                </td>
                <td style="padding: 12px;">${user.email || '-'}</td>
                <td style="padding: 12px;">${user.soDienThoai || '-'}</td>
                <td style="padding: 12px; text-align: center;">
                    <button onclick='editUser(${JSON.stringify(user).replace(/'/g, "&#39;")})' 
                            style="background: #ffc107; color: #333; border: none; padding: 6px 12px; border-radius: 5px; cursor: pointer; margin-right: 5px;">
                        ✏️ Sửa
                    </button>
                    <button onclick="deleteUser(${user.maNguoiDung}, '${user.tenDangNhap}')" 
                            style="background: #dc3545; color: white; border: none; padding: 6px 12px; border-radius: 5px; cursor: pointer;">
                        🗑️ Xóa
                    </button>
                </td>
            </tr>
        `;
    });

    html += '</tbody></table>';
    container.innerHTML = html;
}

// Lọc người dùng
function filterUsers() {
    const search = document.getElementById('searchInput').value.toLowerCase();
    const role = document.getElementById('roleFilter').value;

    const filtered = allUsers.filter(user => {
        const matchSearch = !search || 
            user.tenDangNhap.toLowerCase().includes(search) ||
            user.hoTen.toLowerCase().includes(search);
        const matchRole = !role || user.vaiTro === role;
        return matchSearch && matchRole;
    });

    displayUsers(filtered);
}

// Mở modal thêm người dùng
function openAddModal() {
    editingUserId = null;
    document.getElementById('modalTitle').textContent = 'Thêm Người Dùng';
    document.getElementById('userForm').reset();
    document.getElementById('password').required = true;
    document.getElementById('password').placeholder = 'Nhập mật khẩu';
    document.getElementById('userModal').style.display = 'flex';
}

// Sửa người dùng
function editUser(user) {
    editingUserId = user.maNguoiDung;
    document.getElementById('modalTitle').textContent = 'Sửa Người Dùng';
    document.getElementById('username').value = user.tenDangNhap;
    document.getElementById('fullName').value = user.hoTen;
    document.getElementById('role').value = user.vaiTro;
    document.getElementById('email').value = user.email || '';
    document.getElementById('phone').value = user.soDienThoai || '';
    
    // Mật khẩu không bắt buộc khi sửa
    document.getElementById('password').required = false;
    document.getElementById('password').value = '';
    document.getElementById('password').placeholder = 'Để trống nếu không đổi mật khẩu';
    
    document.getElementById('userModal').style.display = 'flex';
}

// Xóa người dùng
async function deleteUser(userId, username) {
    if (!confirm(`Bạn có chắc chắn muốn xóa người dùng "${username}"?`)) {
        return;
    }

    try {
        if (isApiMode) {
            console.log('🗑️ Deleting user:', userId);
            const response = await fetchAPI(`/api-admin/NguoiDung/${userId}`, {
                method: 'DELETE'
            });

            if (response.success) {
                alert('✅ Xóa người dùng thành công!');
                loadUsers();
            } else {
                throw new Error(response.message || 'Xóa thất bại');
            }
        } else {
            // Demo mode
            allUsers = allUsers.filter(u => u.maNguoiDung !== userId);
            alert('✅ Đã xóa (Demo mode)');
            displayUsers(allUsers);
            updateStats();
        }
    } catch (error) {
        console.error('❌ Delete Error:', error);
        alert('❌ Lỗi: ' + error.message);
    }
}

// Đóng modal
function closeModal() {
    document.getElementById('userModal').style.display = 'none';
    document.getElementById('userForm').reset();
}

// Submit form
document.getElementById('userForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const username = document.getElementById('username').value.trim();
    const password = document.getElementById('password').value;
    const fullName = document.getElementById('fullName').value.trim();
    const role = document.getElementById('role').value;
    const email = document.getElementById('email').value.trim();
    const phone = document.getElementById('phone').value.trim();

    // Validate
    if (!editingUserId && password.length < 6) {
        alert('⚠️ Mật khẩu phải có ít nhất 6 ký tự!');
        return;
    }

    // **QUAN TRỌNG: Map sang format mà API C# mong đợi**
    const userData = {
        TenDangNhap: username,
        HoTen: fullName,
        VaiTro: role
    };

    // Chỉ gửi mật khẩu nếu có nhập
    if (password) {
        userData.MatKhau = password;
    }

    try {
        if (isApiMode) {
            let response;
            
            if (editingUserId) {
                // Cập nhật - GỌI đúng endpoint /api-admin/NguoiDung/{id}
                console.log('📝 Updating user:', editingUserId, userData);
                response = await fetchAPI(`/api-admin/NguoiDung/${editingUserId}`, {
                    method: 'PUT',
                    body: JSON.stringify(userData)
                });
            } else {
                // Thêm mới - Mật khẩu bắt buộc
                if (!password) {
                    alert('⚠️ Mật khẩu là bắt buộc khi thêm người dùng mới!');
                    return;
                }
                console.log('➕ Creating user:', userData);
                response = await fetchAPI('/api-admin/NguoiDung', {
                    method: 'POST',
                    body: JSON.stringify(userData)
                });
            }

            if (response.success) {
                alert(`✅ ${editingUserId ? 'Cập nhật' : 'Thêm'} người dùng thành công!`);
                closeModal();
                loadUsers();
            } else {
                throw new Error(response.message || 'Thao tác thất bại');
            }
        } else {
            // Demo mode
            if (editingUserId) {
                const index = allUsers.findIndex(u => u.maNguoiDung === editingUserId);
                if (index !== -1) {
                    allUsers[index] = { 
                        ...allUsers[index], 
                        tenDangNhap: username,
                        hoTen: fullName,
                        vaiTro: role,
                        email: email,
                        soDienThoai: phone
                    };
                }
            } else {
                allUsers.push({
                    maNguoiDung: Date.now(),
                    tenDangNhap: username,
                    hoTen: fullName,
                    vaiTro: role,
                    email: email,
                    soDienThoai: phone,
                    trangThai: 'Active'
                });
            }
            alert(`✅ ${editingUserId ? 'Cập nhật' : 'Thêm'} thành công (Demo mode)`);
            closeModal();
            displayUsers(allUsers);
            updateStats();
        }
    } catch (error) {
        console.error('❌ Submit Error:', error);
        alert('❌ Lỗi: ' + error.message);
    }
});

// Cập nhật thống kê
function updateStats() {
    document.getElementById('totalUsers').textContent = allUsers.length;
    document.getElementById('adminCount').textContent = allUsers.filter(u => u.vaiTro === 'Admin').length;
    document.getElementById('staffCount').textContent = allUsers.filter(u => u.vaiTro === 'LeTan' || u.vaiTro === 'KeToan').length;
    document.getElementById('activeCount').textContent = allUsers.filter(u => u.trangThai === 'Active').length;
}

// Helper functions
function getRoleText(role) {
    const roles = {
        'Admin': 'Quản trị',
        'LeTan': 'Lễ tân',
        'KeToan': 'Kế toán',
        'Khach': 'Khách'
    };
    return roles[role] || role;
}

function showLoading() {
    document.getElementById('usersTableContainer').innerHTML = '<div style="text-align: center; padding: 40px; color: #666;">Đang tải dữ liệu...</div>';
}

function showWarning(message) {
    const warning = document.createElement('div');
    warning.className = 'warning-message';
    warning.style.cssText = 'background: #fff3cd; border-left: 4px solid #ffc107; color: #856404; padding: 15px; border-radius: 8px; margin-bottom: 20px;';
    warning.innerHTML = `<strong>⚠️ Thông báo:</strong> ${message}`;
    
    const container = document.querySelector('.main-content');
    container.insertBefore(warning, container.children[1]);
}

function showError(message) {
    const error = document.createElement('div');
    error.style.cssText = 'background: #f8d7da; border-left: 4px solid #dc3545; color: #721c24; padding: 15px; border-radius: 8px; margin-bottom: 20px;';
    error.innerHTML = `<strong>❌ Lỗi:</strong> ${message}`;
    
    const container = document.querySelector('.main-content');
    container.insertBefore(error, container.children[1]);
}

function logout() {
    if (confirm('Bạn có muốn đăng xuất?')) {
        localStorage.removeItem('token');
        window.location.href = '/index.html';
    }
}

// Lấy chữ cái đầu tên người dùng từ token hoặc localStorage
    window.addEventListener('load', () => {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          const payload = JSON.parse(atob(token.split('.')[1]));
          const hoTen = payload.HoTen || payload["HoTen"] || "User";
          document.getElementById('userInitial').textContent = hoTen.charAt(0).toUpperCase();
        } catch (e) {
          document.getElementById('userInitial').textContent = 'U';
        }
      }
    });

    checkRole(['Admin']);
// Initialize
if (checkAdminAuth()) {
    loadUsers();
} else {
    loadDemoData();
}