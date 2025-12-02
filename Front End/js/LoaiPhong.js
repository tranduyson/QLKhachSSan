// ================ LoaiPhong.js - QUẢN LÝ LOẠI PHÒNG (ĐÃ SỬA) ================

const API_BASE_URL = "https://localhost:7105/api-common";
let allTypes = [];
let editingType = null;

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
      showError("Không kết nối được API. Kiểm tra:\n1. API đã chạy chưa?\n2. Truy cập https://localhost:7105/api-common/LoaiPhong trên browser để accept certificate");
    }
    
    throw err;
  }
}

// Load danh sách loại phòng
async function loadTypes() {
  try {
    const res = await api("/LoaiPhong");
    if (res?.success) {
      allTypes = res.data || [];
      renderTypes();
    } else {
      showError("Không thể tải danh sách loại phòng");
    }
  } catch (err) {
    console.error("Lỗi load loại phòng:", err);
    showError("Lỗi kết nối server - Kiểm tra API đã chạy chưa");
  }
}

// Hiển thị bảng loại phòng
function renderTypes() {
  const tbody = document.getElementById("typesTableBody");
  tbody.innerHTML = "";

  if (!allTypes || allTypes.length === 0) {
    tbody.innerHTML = `<tr><td colspan="5" style="text-align:center;color:#999;padding:40px;">Chưa có loại phòng nào</td></tr>`;
    return;
  }

  allTypes.forEach(t => {
    const row = document.createElement("tr");
    row.innerHTML = `
      <td><strong>${t.ma || t.Ma}</strong></td>
      <td>${t.ten || t.Ten}</td>
      <td>${(t.moTa || t.MoTa) || "<em style='color:#999;'>Không có mô tả</em>"}</td>
      <td style="text-align:center;">${t.soKhachToiDa || t.SoKhachToiDa} người</td>
      <td class="action-btns">
        <button class="btn-sm btn-edit" onclick="editType(${t.maLoaiPhong || t.MaLoaiPhong})">Sửa</button>
        <button class="btn-sm btn-delete" onclick="deleteType(${t.maLoaiPhong || t.MaLoaiPhong})">Xóa</button>
      </td>
    `;
    tbody.appendChild(row);
  });
}

// Mở modal thêm loại phòng
function openAddTypeModal() {
  editingType = null;
  document.getElementById("modalTitle").textContent = "Thêm Loại Phòng";
  document.getElementById("typeForm").reset();
  document.getElementById("typeModal").style.display = "block";
}

// Mở modal sửa loại phòng
function editType(id) {
  const type = allTypes.find(t => (t.maLoaiPhong || t.MaLoaiPhong) == id);
  if (!type) return;

  editingType = type;
  document.getElementById("modalTitle").textContent = "Sửa Loại Phòng";
  document.getElementById("typeCode").value = type.ma || type.Ma;
  document.getElementById("typeName").value = type.ten || type.Ten;
  document.getElementById("typeDescription").value = type.moTa || type.MoTa || "";
  document.getElementById("maxGuests").value = type.soKhachToiDa || type.SoKhachToiDa;
  document.getElementById("typeModal").style.display = "block";
}

// Đóng modal
function closeModal() {
  document.getElementById("typeModal").style.display = "none";
}

// Lưu loại phòng (thêm / sửa)
document.getElementById("typeForm").addEventListener("submit", async e => {
  e.preventDefault();

  const code = document.getElementById("typeCode").value.trim().toUpperCase();
  const name = document.getElementById("typeName").value.trim();
  const description = document.getElementById("typeDescription").value.trim();
  const maxGuests = parseInt(document.getElementById("maxGuests").value);

  if (!code || !name || maxGuests < 1) {
    return showError("Vui lòng nhập đầy đủ thông tin hợp lệ!");
  }

  const data = {
    ma: code,
    ten: name,
    moTa: description || null,
    soKhachToiDa: maxGuests,
  };

  try {
    if (editingType) {
      await api(`/LoaiPhong/${editingType.maLoaiPhong || editingType.MaLoaiPhong}`, {
        method: "PUT",
        body: JSON.stringify(data),
      });
      showSuccess("Cập nhật loại phòng thành công!");
    } else {
      await api("/LoaiPhong", {
        method: "POST",
        body: JSON.stringify(data),
      });
      showSuccess("Thêm loại phòng thành công!");
    }
    closeModal();
    await loadTypes();
  } catch (err) {
    showError(err.message || "Lưu thất bại! Vui lòng kiểm tra lại.");
    console.error(err);
  }
});

// Xóa loại phòng
async function deleteType(id) {
  if (!confirm("Xóa loại phòng này? Các phòng đang dùng loại này sẽ bị ảnh hưởng!")) return;

  try {
    await api(`/LoaiPhong/${id}`, { method: "DELETE" });
    showSuccess("Đã xóa loại phòng!");
    await loadTypes();
  } catch (err) {
    showError(err.message || "Không thể xóa! Có thể loại phòng đang được sử dụng.");
    console.error(err);
  }
}

// Toast đơn giản
function showSuccess(msg) { 
  alert("✅ Thành công: " + msg); 
}

function showError(msg) { 
  alert("❌ Lỗi: " + msg); 
}

// Khởi động ứng dụng
document.addEventListener("DOMContentLoaded", async () => {
  // Kiểm tra token (optional - bỏ nếu chưa có login)
  // if (!localStorage.getItem("token")) {
  //   alert("Vui lòng đăng nhập!");
  //   window.location.href ="login.html";
  //   return;
  // }
  checkRole(['Admin']);
  await loadTypes();
});

// Đóng modal khi click ngoài
window.addEventListener("click", e => {
  const modal = document.getElementById("typeModal");
  if (e.target === modal) closeModal();
});