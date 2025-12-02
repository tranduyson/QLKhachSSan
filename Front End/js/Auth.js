// -------------------------
// JWT utils
// -------------------------
function parseJwt(token) {
    try {
        return JSON.parse(atob(token.split('.')[1]));
    } catch (e) {
        return null;
    }
}

function getUserFromToken() {
    const token = localStorage.getItem("token");
    if (!token) return null;
    return parseJwt(token);
}

// -------------------------
// Cập nhật thông tin user trên trang
// -------------------------
function updateUserInfo(user) {
    // Top bar
    const nameTop = document.getElementById("userNameDisplay");
    const avatarTop = document.getElementById("userAvatar");

    // Dropdown
    const nameDropdown = document.getElementById("dropdownUserName");
    const roleDropdown = document.getElementById("dropdownUserRole");
    const avatarDropdown = document.getElementById("dropdownAvatar");

    // Lấy tên hiển thị
    const hoTen = user.HoTen || user.hoTen || user.name || user.username || "Người dùng";
    
    if (nameTop) nameTop.textContent = hoTen;
    if (avatarTop) avatarTop.textContent = hoTen.charAt(0).toUpperCase();

    if (nameDropdown) nameDropdown.textContent = hoTen;
    
    // Lấy role thực sự
    let roleName = user.role || user.Role || user.VaiTro || user["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || "Khách";
    let roleDisplay = "Khách";
    switch(roleName) {
        case "Admin": roleDisplay = "Quản trị viên"; break;
        case "LeTan": roleDisplay = "Lễ tân"; break;
        case "KeToan": roleDisplay = "Kế toán"; break;
        case "Manager": roleDisplay = "Quản lý"; break;
        case "User": roleDisplay = "Người dùng"; break;
    }
    if (roleDropdown) roleDropdown.textContent = roleDisplay;

    // Avatar dropdown (nếu có)
    if (avatarDropdown && user.avatar) {
        avatarDropdown.textContent = "";
        avatarDropdown.style.backgroundImage = `url(${user.avatar})`;
        avatarDropdown.style.backgroundSize = "cover";
        avatarDropdown.style.backgroundPosition = "center";
        avatarDropdown.style.borderRadius = "50%";
    }
}

// -------------------------
// Kiểm tra role
// -------------------------
function checkRole(requiredRoles) {
    const user = getUserFromToken();

    if (!user) {
        window.location.href = "login.html";
        return false;
    }

    // Lấy role từ JWT
    const role = user.role || user.Role || user.VaiTro || user["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    if (!role) {
        window.location.href = "unauthorized.html";
        return false;
    }

    if (!Array.isArray(requiredRoles)) requiredRoles = [requiredRoles];
    if (!requiredRoles.includes(role)) {
        window.location.href = "management.html";
        return false;
    }

    // Role hợp lệ → update info
    updateUserInfo(user);
    return true;
}

// -------------------------
// Logout
// -------------------------
function logout() {
    if (confirm('Bạn có muốn đăng xuất?')) {
        localStorage.removeItem('token');
        window.location.href = '/index.html';
    }
}

// -------------------------
// Load trang

