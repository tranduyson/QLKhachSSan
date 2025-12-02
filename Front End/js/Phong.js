// ================ Phong.js - QUẢN LÝ PHÒNG (ĐÃ SỬA) ================

const API_BASE_URL = "https://localhost:7105/api-common";
let allRooms = [];
let roomTypes = [];
let editingRoom = null;

// API wrapper với xử lý lỗi token
async function api(endpoint, options = {}) {
  const token = localStorage.getItem("token") || "";
  
  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      ...options,
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
        ...options.headers,
      },
    });

    // ⚠️ XỬ LÝ TOKEN HẾT HẠN
    if (response.status === 401) {
      localStorage.removeItem("token");
      alert("Phiên đăng nhập hết hạn! Vui lòng đăng nhập lại.");
      window.location.href ="login.html";
      return null;
    }

    let data;
    try {
      data = await response.json();
    } catch {
      data = null;
    }

    if (!response.ok) {
      console.error("API Error:", response.status, data);
      throw new Error(data?.message || `HTTP ${response.status}`);
    }

    return data;
    
  } catch (err) {
    console.error("Fetch Error:", err);
    
    // ⚠️ XỬ LÝ LỖI CERTIFICATE SSL (localhost)
    if (err.message.includes('Failed to fetch') || err.message.includes('NetworkError')) {
      showError("Không kết nối được API. Kiểm tra:\n1. API đã chạy chưa?\n2. Truy cập https://localhost:7105/api-common/Phong trên browser để accept certificate");
    }
    
    throw err;
  }
}

// Load danh sách loại phòng
async function loadRoomTypes() {
  try {
    const res = await api("/LoaiPhong");
    if (res?.success) {
      roomTypes = res.data || [];
      populateRoomTypeSelect();
    } else {
      showError("Không tải được danh sách loại phòng");
    }
  } catch (err) {
    console.error("Lỗi load loại phòng:", err);
    showError("Lỗi kết nối server - Kiểm tra API đã chạy chưa");
  }
}

// Load danh sách phòng
async function loadRooms() {
  try {
    const res = await api("/Phong");
    if (res?.success) {
      allRooms = res.data || [];
      renderRooms();
    } else {
      showError("Không tải được danh sách phòng");
    }
  } catch (err) {
    console.error("Lỗi load phòng:", err);
    showError("Lỗi kết nối server - Kiểm tra API đã chạy chưa");
  }
}

// Đổ dữ liệu vào select loại phòng
function populateRoomTypeSelect() {
  const select = document.getElementById("roomTypeSelect");
  select.innerHTML = '<option value="">-- Chọn loại phòng --</option>';
  roomTypes.forEach(t => {
    const opt = document.createElement("option");
    opt.value = t.maLoaiPhong || t.MaLoaiPhong;
    opt.textContent = t.ten || t.Ten;
    select.appendChild(opt);
  });
}

// Hiển thị bảng phòng
function renderRooms() {
  const tbody = document.getElementById("roomsTableBody");
  tbody.innerHTML = "";

  if (!allRooms || allRooms.length === 0) {
    tbody.innerHTML = `<tr><td colspan="4" style="text-align:center;color:#999;padding:30px;">Chưa có phòng nào</td></tr>`;
    return;
  }

  allRooms.forEach(room => {
    const type = roomTypes.find(t => 
      (t.maLoaiPhong || t.MaLoaiPhong) === (room.maLoaiPhong || room.MaLoaiPhong)
    );
    const typeName = type ? (type.ten || type.Ten) : "Không xác định";

    // Map trạng thái
    const statusMap = {
      'SanSang': { text: 'Sẵn sàng', class: 'status-ready' },
      'DaThue': { text: 'Đã thuê', class: 'status-occupied' },
      'BaoTri': { text: 'Bảo trì', class: 'status-maintenance' },
      'DonDep': { text: 'Dọn dẹp', class: 'status-cleaning' }
    };
    
    const status = room.trangThai || room.TrangThai || 'SanSang';
    const statusInfo = statusMap[status] || { text: status, class: 'status-ready' };

    const row = document.createElement("tr");
    row.innerHTML = `
      <td><strong>${room.soPhong || room.SoPhong}</strong></td>
      <td>${typeName}</td>
      <td><span class="status-badge ${statusInfo.class}">${statusInfo.text}</span></td>
      <td class="action-btns">
        <button class="btn-sm btn-edit" onclick="editRoom(${room.maPhong || room.MaPhong})">Sửa</button>
        <button class="btn-sm btn-delete" onclick="deleteRoom(${room.maPhong || room.MaPhong})">Xóa</button>
      </td>
    `;
    tbody.appendChild(row);
  });
}

// Mở modal thêm phòng
function openAddRoomModal() {
  editingRoom = null;
  document.getElementById("modalTitle").textContent = "Thêm Phòng";
  document.getElementById("roomForm").reset();
  document.getElementById("roomModal").style.display = "block";
}

// Sửa phòng
function editRoom(id) {
  const room = allRooms.find(r => (r.maPhong || r.MaPhong) == id);
  if (!room) return;

  editingRoom = room;
  document.getElementById("modalTitle").textContent = "Chỉnh Sửa Phòng";
  document.getElementById("roomNumber").value = room.soPhong || room.SoPhong;
  document.getElementById("roomTypeSelect").value = room.maLoaiPhong || room.MaLoaiPhong;
  document.getElementById("roomStatus").value = room.trangThai || room.TrangThai;
  document.getElementById("roomModal").style.display = "block";
}

function closeModal() {
  document.getElementById("roomModal").style.display = "none";
}

// Lưu phòng (thêm / sửa)
document.getElementById("roomForm").addEventListener("submit", async e => {
  e.preventDefault();

  const roomNumber = document.getElementById("roomNumber").value.trim();
  const roomTypeId = document.getElementById("roomTypeSelect").value;
  const roomStatus = document.getElementById("roomStatus").value;

  if (!roomNumber || !roomTypeId) {
    return showError("Vui lòng nhập đầy đủ thông tin!");
  }

  const data = {
    soPhong: roomNumber,
    maLoaiPhong: parseInt(roomTypeId),
    trangThai: roomStatus,
  };

  try {
    if (editingRoom) {
      await api(`/Phong/${editingRoom.maPhong || editingRoom.MaPhong}`, {
        method: "PUT",
        body: JSON.stringify(data),
      });
      showSuccess("Cập nhật phòng thành công!");
    } else {
      await api("/Phong", {
        method: "POST",
        body: JSON.stringify(data),
      });
      showSuccess("Thêm phòng thành công!");
    }
    closeModal();
    await loadRooms();
  } catch (err) {
    console.error(err);
    showError(err.message || "Lưu thất bại! Vui lòng thử lại.");
  }
});

// Xóa phòng
async function deleteRoom(id) {
  if (!confirm("Xóa phòng này?")) return;
  try {
    await api(`/Phong/${id}`, { method: "DELETE" });
    showSuccess("Đã xóa!");
    await loadRooms();
  } catch (err) {
    console.error(err);
    showError(err.message || "Xóa thất bại!");
  }
}

// Toast đơn giản
function showSuccess(msg) { 
  alert("✅ Thành công: " + msg); 
}

function showError(msg) { 
  alert("❌ Lỗi: " + msg); 
}

// Khởi động
document.addEventListener("DOMContentLoaded", async () => {
  // Kiểm tra token (optional - bỏ nếu chưa có login)
  // if (!localStorage.getItem("token")) {
  //   alert("Vui lòng đăng nhập!");
  //   window.location.href ="login.html";
  //   return;
  // }
  checkRole(['Admin']);
  await loadRoomTypes();
  await loadRooms();
});

// Đóng modal khi click ngoài
window.addEventListener("click", e => {
  const modal = document.getElementById("roomModal");
  if (e.target === modal) closeModal();
});