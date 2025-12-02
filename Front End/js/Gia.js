// ================ Gia.js - QUẢN LÝ GIÁ PHÒNG (ĐÃ SỬA) ================

const API_BASE_URL = "https://localhost:7105/api-common";
let allPrices = [];
let roomTypes = [];
let editingPrice = null;

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
      showError("Không kết nối được API. Kiểm tra:\n1. API đã chạy chưa?\n2. Truy cập https://localhost:7105/api-common/Gia trên browser để accept certificate");
    }
    
    throw err;
  }
}

// Load loại phòng
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
    console.error(err);
    showError("Lỗi kết nối server - Kiểm tra API đã chạy chưa");
  }
}

// Load giá
async function loadPrices() {
  try {
    const res = await api("/Gia");
    if (res?.success) {
      allPrices = res.data || [];
      renderPrices();
    } else {
      showError("Không tải được danh sách giá");
    }
  } catch (err) {
    console.error(err);
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

// Hiển thị bảng giá
function renderPrices() {
  const tbody = document.getElementById("pricesTableBody");
  tbody.innerHTML = "";

  if (!allPrices || allPrices.length === 0) {
    tbody.innerHTML = `<tr><td colspan="6" style="text-align:center;color:#999;padding:30px;">Chưa có mức giá nào</td></tr>`;
    return;
  }

  allPrices.forEach(p => {
    const type = roomTypes.find(t => 
      (t.maLoaiPhong || t.MaLoaiPhong) === (p.maLoaiPhong || p.MaLoaiPhong)
    );
    const typeName = type ? (type.ten || type.Ten) : "Không xác định";

    const row = document.createElement("tr");
    row.innerHTML = `
      <td>${typeName}</td>
      <td>${formatDate(p.tuNgay || p.TuNgay)}</td>
      <td>${formatDate(p.denNgay || p.DenNgay)}</td>
      <td>${formatCurrency(p.giaMoiDem || p.GiaMoiDem)}</td>
      <td>${(p.giaMoiGio || p.GiaMoiGio) ? formatCurrency(p.giaMoiGio || p.GiaMoiGio) : "-"}</td>
      <td class="action-btns">
        <button class="btn-sm btn-edit" onclick="editPrice(${p.maGia || p.MaGia})">Sửa</button>
        <button class="btn-sm btn-delete" onclick="deletePrice(${p.maGia || p.MaGia})">Xóa</button>
      </td>
    `;
    tbody.appendChild(row);
  });
}

// Format helper
function formatDate(str) {
  if (!str) return "N/A";
  const date = new Date(str);
  return date.toLocaleDateString("vi-VN");
}

function formatCurrency(num) {
  if (!num) return "0 ₫";
  return new Intl.NumberFormat("vi-VN", { 
    style: "currency", 
    currency: "VND" 
  }).format(num);
}

// Modal
function openAddPriceModal() {
  editingPrice = null;
  document.getElementById("modalTitle").textContent = "Thêm Mức Giá";
  document.getElementById("priceForm").reset();
  document.getElementById("priceModal").style.display = "block";
}

function editPrice(id) {
  const price = allPrices.find(p => (p.maGia || p.MaGia) == id);
  if (!price) return;

  editingPrice = price;
  document.getElementById("modalTitle").textContent = "Chỉnh Sửa Giá";
  document.getElementById("roomTypeSelect").value = price.maLoaiPhong || price.MaLoaiPhong;
  document.getElementById("fromDate").value = (price.tuNgay || price.TuNgay).slice(0, 10);
  document.getElementById("toDate").value = (price.denNgay || price.DenNgay).slice(0, 10);
  document.getElementById("pricePerNight").value = price.giaMoiDem || price.GiaMoiDem;
  document.getElementById("pricePerHour").value = price.giaMoiGio || price.GiaMoiGio || "";
  document.getElementById("notes").value = price.ghiChu || price.GhiChu || "";
  document.getElementById("priceModal").style.display = "block";
}

function closeModal() {
  document.getElementById("priceModal").style.display = "none";
}

// Lưu giá (thêm / sửa)
document.getElementById("priceForm").addEventListener("submit", async e => {
  e.preventDefault();

  const roomTypeId = document.getElementById("roomTypeSelect").value;
  const fromDate = document.getElementById("fromDate").value;
  const toDate = document.getElementById("toDate").value;
  const pricePerNight = document.getElementById("pricePerNight").value;
  const pricePerHour = document.getElementById("pricePerHour").value;
  const notes = document.getElementById("notes").value.trim();

  if (!roomTypeId || !fromDate || !toDate || !pricePerNight) {
    return showError("Vui lòng nhập đầy đủ thông tin bắt buộc!");
  }

  const data = {
    maLoaiPhong: parseInt(roomTypeId),
    tuNgay: fromDate,
    denNgay: toDate,
    giaMoiDem: parseFloat(pricePerNight),
    giaMoiGio: pricePerHour ? parseFloat(pricePerHour) : null,
    ghiChu: notes || null,
  };

  try {
    if (editingPrice) {
      await api(`/Gia/${editingPrice.maGia || editingPrice.MaGia}`, {
        method: "PUT",
        body: JSON.stringify(data),
      });
      showSuccess("Cập nhật giá thành công!");
    } else {
      await api("/Gia", {
        method: "POST",
        body: JSON.stringify(data),
      });
      showSuccess("Thêm giá thành công!");
    }
    closeModal();
    await loadPrices();
  } catch (err) {
    console.error(err);
    showError(err.message || "Lưu thất bại! Vui lòng thử lại.");
  }
});

// Xóa giá
async function deletePrice(id) {
  if (!confirm("Xóa mức giá này?")) return;
  try {
    await api(`/Gia/${id}`, { method: "DELETE" });
    showSuccess("Đã xóa!");
    await loadPrices();
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
  
  await loadRoomTypes();
  await loadPrices();
});
checkRole(['Admin']);
// Đóng modal khi click ngoài
window.addEventListener("click", e => {
  const modal = document.getElementById("priceModal");
  if (e.target === modal) closeModal();
});