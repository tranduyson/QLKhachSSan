// DatPhong Management JavaScript
window.DatPhongApp = {
    phongIndex: 0,
    dichVuIndex: 0,
    currentAction: null,
    currentId: null,

    // Initialize index page
    initIndexPage() {
        this.loadDatPhongList();
        this.setupEventListeners();
    },

    // Initialize create form
    initCreateForm() {
        this.setupDynamicRows();
        this.setupCreateForm();
        // Tính tổng ban đầu
        this.calculateTotal();
    },

    // Initialize details page
    initDetailsPage(id) {
        this.loadDatPhongDetails(id);
        this.setupDetailsPage();
    },

    // Load dat phong list
    async loadDatPhongList() {
        try {
            document.getElementById('loadingSpinner').style.display = 'block';
            document.getElementById('dataContainer').style.display = 'none';
            document.getElementById('errorMessage').style.display = 'none';

            const data = await HotelApp.apiFetch('/DatPhong');

            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('dataContainer').style.display = 'block';

            this.renderDatPhongTable(data);
        } catch (error) {
            console.error('Error loading dat phong list:', error);
            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('errorMessage').style.display = 'block';
            document.getElementById('errorText').textContent = error.message;
            HotelApp.showError('Không thể tải danh sách đặt phòng: ' + error.message);
        }
    },

    // Render dat phong table
    renderDatPhongTable(data) {
        const tbody = document.getElementById('datPhongTableBody');
        tbody.innerHTML = '';

        if (!data || data.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="9" class="text-center text-muted py-4">
                        <i class="fas fa-inbox fa-2x mb-2"></i><br>
                        Không có dữ liệu
                    </td>
                </tr>
            `;
            return;
        }

        data.forEach(item => {
            const row = document.createElement('tr');
            const statusBadge = this.getStatusBadge(item.trangThai);
            const actionButtons = this.getActionButtons(item.maDatPhong, item.trangThai);

            row.innerHTML = `
                <td>${item.maDatPhong || ''}</td>
                <td>${item.khachHang?.hoTen || ''}</td>
                <td>${item.nhanVien?.hoTen || ''}</td>
                <td>${HotelApp.formatDate(item.ngayDat)}</td>
                <td>${HotelApp.formatDate(item.ngayNhan)}</td>
                <td>${HotelApp.formatDate(item.ngayTra)}</td>
                <td>${HotelApp.formatCurrency(item.tongTien || 0)}</td>
                <td>${statusBadge}</td>
                <td>
                    <div class="btn-group btn-group-sm" role="group">
                        <a href="/DatPhong/Details/${item.maDatPhong}" class="btn btn-outline-info" title="Xem chi tiết">
                            <i class="fas fa-eye"></i>
                        </a>
                        ${actionButtons}
                    </div>
                </td>
            `;
            tbody.appendChild(row);
        });
    },

    // Get status badge
    getStatusBadge(status) {
        const statusMap = {
            'DaDat': '<span class="badge bg-warning">Đã đặt</span>',
            'DaNhan': '<span class="badge bg-info">Đã nhận</span>',
            'DaTra': '<span class="badge bg-success">Đã trả</span>',
            'Huy': '<span class="badge bg-danger">Hủy</span>'
        };
        return statusMap[status] || '<span class="badge bg-secondary">' + status + '</span>';
    },

    // Get action buttons
    getActionButtons(id, status) {
        let buttons = '';

        if (status === 'DaDat') {
            buttons += `<button type="button" class="btn btn-outline-success" onclick="DatPhongApp.checkin(${id})" title="Check-in">
                <i class="fas fa-sign-in-alt"></i>
            </button>`;
        }

        if (status === 'DaNhan') {
            buttons += `<button type="button" class="btn btn-outline-warning" onclick="DatPhongApp.checkout(${id})" title="Check-out">
                <i class="fas fa-sign-out-alt"></i>
            </button>`;
        }

        return buttons;
    },

    // Setup event listeners
    setupEventListeners() {
        const actionModal = document.getElementById('actionModal');
        if (actionModal) {
            const confirmActionBtn = document.getElementById('confirmActionBtn');
            if (confirmActionBtn) {
                confirmActionBtn.addEventListener('click', () => {
                    this.confirmAction();
                });
            }
        }
    },

    // Setup create form
    setupCreateForm() {
        const form = document.getElementById('createForm');
        if (!form) return;

        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            if (!form.checkValidity()) {
                e.stopPropagation();
                form.classList.add('was-validated');
                return;
            }

            try {
                const submitBtn = form.querySelector('[type="submit"]');
                if (submitBtn) {
                    submitBtn.disabled = true;
                    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Đang xử lý...';
                }

                const formData = new FormData(form);

                // ✅ Collect data theo đúng DatPhongRequest (PascalCase)
                const data = {
                    MaKhachHang: parseInt(formData.get('MaKhachHang')),
                    MaNhanVien: formData.get('MaNhanVien') ?
                        parseInt(formData.get('MaNhanVien')) : null,
                    NgayDat: formData.get('NgayDat'),
                    NgayNhan: formData.get('NgayNhan'),
                    NgayTra: formData.get('NgayTra'),
                    TrangThai: formData.get('TrangThai') || 'Đã đặt',
                    ChiTietDatPhongs: [],
                    SuDungDichVus: []
                };

                // ✅ Collect phong data theo ChiTietDatPhongRequest (MaPhong, DonGia, SoDem)
                const phongRows = document.querySelectorAll('#phongContainer .phong-row');
                phongRows.forEach((row) => {
                    const maPhong = row.querySelector('.phong-select')?.value;
                    const donGia = parseFloat(row.querySelector('.don-gia-phong')?.value || 0);
                    const soDem = parseInt(row.querySelector('.so-dem')?.value || 0);

                    // Chỉ thêm nếu có đầy đủ thông tin hợp lệ
                    if (maPhong && donGia > 0 && soDem > 0) {
                        data.ChiTietDatPhongs.push({
                            MaPhong: parseInt(maPhong),
                            DonGia: donGia,
                            SoDem: soDem  // ✅ Đổi từ soLuong sang SoDem
                        });
                    }
                });

                // ✅ Collect dich vu data theo SuDungDichVuRequest (MaDichVu, SoLuong, DonGia)
                const dichVuRows = document.querySelectorAll('#dichVuContainer .dichvu-row');
                dichVuRows.forEach((row) => {
                    const maDichVu = row.querySelector('.dichvu-select')?.value;
                    const soLuong = parseInt(row.querySelector('.so-luong')?.value || 0);
                    const donGia = parseFloat(row.querySelector('.don-gia-dichvu')?.value || 0);

                    // Chỉ thêm nếu có đầy đủ thông tin hợp lệ
                    if (maDichVu && soLuong > 0 && donGia > 0) {
                        data.SuDungDichVus.push({
                            MaDichVu: parseInt(maDichVu),
                            SoLuong: soLuong,
                            DonGia: donGia
                        });
                    }
                });

                // ✅ Filter lần nữa để chắc chắn không có null
                data.ChiTietDatPhongs = data.ChiTietDatPhongs.filter(ct => ct !== null);
                data.SuDungDichVus = data.SuDungDichVus.filter(sv => sv !== null);

                console.log('Submitting data:', data);

                // ✅ Lấy AntiForgeryToken
                 token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;
                
                // ✅ Gọi API thực tế
                const response = await fetch('/DatPhong/Create', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': token || ''
                    },
                    body: JSON.stringify(data)
                });

                if (response.ok) {
                    // Kiểm tra xem có redirect không
                    if (response.redirected) {
                        window.location.href = response.url;
                    } else {
                        alert('Đặt phòng thành công!');
                        window.location.href = '/DatPhong/Index';
                    }
                } else {
                    // Xử lý lỗi validation từ server
                    const contentType = response.headers.get('content-type');
                    if (contentType && contentType.includes('application/json')) {
                        const errorData = await response.json();
                        console.error('Validation errors:', errorData);
                        this.displayValidationErrors(errorData);
                    } else {
                        throw new Error('Lỗi từ server: ' + response.statusText);
                    }
                }

            } catch (error) {
                console.error('Error creating dat phong:', error);
                alert('Không thể đặt phòng: ' + error.message);
            } finally {
                // ✅ Reset button về trạng thái ban đầu
                const submitBtn = form.querySelector('[type="submit"]');
                if (submitBtn) {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = 'Tạo đặt phòng';
                }
            }
        });
    },

    // ✅ Thêm hàm hiển thị lỗi validation
    displayValidationErrors(errors) {
        // Xóa lỗi cũ
        document.querySelectorAll('.validation-error').forEach(el => el.remove());

        // Hiển thị lỗi mới
        if (errors.errors) {
            for (const [field, messages] of Object.entries(errors.errors)) {
                const input = document.querySelector(`[name="${field}"]`);
                if (input) {
                    const errorDiv = document.createElement('div');
                    errorDiv.className = 'validation-error text-danger small mt-1';
                    errorDiv.textContent = messages.join(', ');
                    input.parentElement.appendChild(errorDiv);
                    input.classList.add('is-invalid');
                }
            }
        } else if (errors.title || errors.message) {
            alert(errors.title || errors.message);
        }
    },

    // Setup dynamic rows
    setupDynamicRows() {
        const addPhongBtn = document.getElementById('addPhongBtn');
        const addDichVuBtn = document.getElementById('addDichVuBtn');

        if (addPhongBtn) {
            addPhongBtn.addEventListener('click', () => {
                this.addPhongRow();
            });
        }

        if (addDichVuBtn) {
            addDichVuBtn.addEventListener('click', () => {
                this.addDichVuRow();
            });
        }

        // Setup existing rows
        this.setupExistingRows();
    },

    // Setup existing rows
    setupExistingRows() {
        document.querySelectorAll('.phong-row').forEach(row => {
            this.setupPhongRowEvents(row);
        });

        document.querySelectorAll('.dichvu-row').forEach(row => {
            this.setupDichVuRowEvents(row);
        });
    },

    // Add phong row
    addPhongRow() {
        const container = document.getElementById('phongContainer');
        if (!container) return;

        const index = container.querySelectorAll('.phong-row').length;

        const newRow = document.createElement('tr');
        newRow.className = 'phong-row';

        newRow.innerHTML = `
            <td>
                <select name="chiTietDatPhongs[${index}].PhongId" class="form-select phong-select" required>
                    <option value="">-- Chọn phòng --</option>
                </select>
            </td>
            <td>
                <input type="text" name="chiTietDatPhongs[${index}].DonGia" class="form-control don-gia-phong" readonly />
            </td>
            <td>
                <input type="number" name="chiTietDatPhongs[${index}].SoDem" class="form-control so-dem" min="1" value="1" required />
            </td>
            <td>
                <input type="number" name="chiTietDatPhongs[${index}].ThanhTien" class="form-control thanh-tien-phong" readonly />
            </td>
            <td>
                <button type="button" class="btn btn-outline-danger btn-sm remove-phong-btn rounded" title="Xóa dòng">
                    Xóa
                </button>
            </td>
        `;

        container.appendChild(newRow);

        // Load phong options
        this.loadPhongOptionsForRow(newRow);

        // Setup events
        this.setupPhongRowEvents(newRow);
    },

    // Add dich vu row
    addDichVuRow() {
        const container = document.getElementById('dichVuContainer');
        if (!container) return;

        const index = container.querySelectorAll('.dichvu-row').length;

        const newRow = document.createElement('tr');
        newRow.className = 'dichvu-row';

        newRow.innerHTML = `
            <td>
                <select name="suDungDichVus[${index}].DichVuId" class="form-select dichvu-select" required>
                    <option value="">-- Chọn dịch vụ --</option>
                </select>
            </td>
            <td>
                <input type="number" name="suDungDichVus[${index}].SoLuong" class="form-control so-luong" min="1" value="1" required />
            </td>
            <td>
                <input type="number" name="suDungDichVus[${index}].DonGia" class="form-control don-gia-dichvu" readonly />
            </td>
            <td>
                <input type="number" name="suDungDichVus[${index}].ThanhTien" class="form-control thanh-tien-dichvu" readonly />
            </td>
            <td>
                <button type="button" class="btn btn-outline-danger btn-sm remove-dichvu-btn rounded" title="Xóa dòng">
                    Xóa
                </button>
            </td>
        `;

        container.appendChild(newRow);

        // Load dich vu options
        this.loadDichVuOptionsForRow(newRow);

        // Setup events
        this.setupDichVuRowEvents(newRow);
    },

    // Load phong options for a row
    loadPhongOptionsForRow(row) {
        const select = row.querySelector('.phong-select');
        if (!select) return;

        try {
            if (window.DatPhongData && window.DatPhongData.phongOptions) {

                window.DatPhongData.phongOptions.filter(p => p.TinhTrang == 'Trong').forEach(item => {
                    const loaiphong = window.DatPhongData.loaiPhongOptions.find(e => e.MaLoaiPhong == item.maLoaiPhong);
                    const option = document.createElement('option');
                    option.value = item.MaPhong;
                    option.textContent = `${item.SoPhong} - ${loaiphong.TenLoaiPhong || 'N/A'} (${(loaiphong.GiaMoiDem || 0).toLocaleString('vi-VN')} đ/đêm)`;
                    option.dataset.gia = loaiphong.GiaMoiDem || 0;
                    select.appendChild(option);
                });
            }
        } catch (error) {
            console.error('Error loading phong options:', error);
        }
    },

    // Load dich vu options for a row
    loadDichVuOptionsForRow(row) {
        const select = row.querySelector('.dichvu-select');
        if (!select) return;

        try {
            if (window.DatPhongData && window.DatPhongData.dichVuOptions) {
                window.DatPhongData.dichVuOptions.forEach(item => {

                    const option = document.createElement('option');
                    option.value = item.maDichVu;
                    option.textContent = `${item.TenDichVu} (${(item.donGia || 0).toLocaleString('vi-VN')} đ)`;
                    option.dataset.gia = item.donGia || 0;
                    select.appendChild(option);
                });
            }
        } catch (error) {
            console.error('Error loading dich vu options:', error);
        }
    },

    // Setup phong row events - FIXED VERSION
    setupPhongRowEvents(row) {
        // Phong select change
        const phongSelect = row.querySelector('.phong-select');
        if (phongSelect) {
            phongSelect.addEventListener('change', (e) => {
                const selectedOption = e.target.selectedOptions[0];
                if (selectedOption && selectedOption.dataset.gia) {
                    const donGiaInput = row.querySelector('.don-gia-phong');
                    const soDemInput = row.querySelector('.so-dem');
                    const thanhTienInput = row.querySelector('.thanh-tien-phong');

                    // GÁN ĐƠN GIÁ VÀO INPUT (FIX 1)
                    donGiaInput.value = selectedOption.dataset.gia;

                    // TÍNH THÀNH TIỀN
                    const thanhTien = parseFloat(selectedOption.dataset.gia) * parseInt(soDemInput.value || 0);
                    thanhTienInput.value = thanhTien;

                    this.calculateTotal();
                }
            });
        }

        // So dem change - FIXED VERSION
        const soDemInput = row.querySelector('.so-dem');
        if (soDemInput) {
            soDemInput.addEventListener('input', () => {
                const phongSelect = row.querySelector('.phong-select');
                const selectedOption = phongSelect?.selectedOptions[0];

                // ĐỌC GIÁ TỪ OPTION ĐÃ CHỌN THAY VÌ INPUT (FIX 2)
                if (selectedOption && selectedOption.dataset.gia) {
                    const donGia = parseFloat(selectedOption.dataset.gia);
                    const soDem = parseInt(soDemInput.value || 0);
                    const thanhTienInput = row.querySelector('.thanh-tien-phong');

                    thanhTienInput.value = donGia * soDem;
                    this.calculateTotal();
                }
            });
        }

        // Remove button
        const removeBtn = row.querySelector('.remove-phong-btn');
        if (removeBtn) {
            removeBtn.addEventListener('click', () => {
                row.remove();
                this.calculateTotal();
            });
        }
    },

    // Setup dich vu row events
    setupDichVuRowEvents(row) {
        // Dich vu select change
        const dichVuSelect = row.querySelector('.dichvu-select');
        if (dichVuSelect) {
            dichVuSelect.addEventListener('change', (e) => {
                const selectedOption = e.target.selectedOptions[0];
                if (selectedOption && selectedOption.dataset.gia) {
                    const donGiaInput = row.querySelector('.don-gia-dichvu');
                    const soLuongInput = row.querySelector('.so-luong');
                    const thanhTienInput = row.querySelector('.thanh-tien-dichvu');

                    donGiaInput.value = selectedOption.dataset.gia;
                    const thanhTien = parseFloat(donGiaInput.value) * parseInt(soLuongInput.value || 0);
                    thanhTienInput.value = thanhTien;

                    this.calculateTotal();
                }
            });
        }

        // So luong change
        const soLuongInput = row.querySelector('.so-luong');
        if (soLuongInput) {
            soLuongInput.addEventListener('input', () => {
                const donGia = parseFloat(row.querySelector('.don-gia-dichvu')?.value || 0);
                const soLuong = parseInt(soLuongInput.value || 0);
                const thanhTienInput = row.querySelector('.thanh-tien-dichvu');
                thanhTienInput.value = donGia * soLuong;
                this.calculateTotal();
            });
        }

        // Remove button
        const removeBtn = row.querySelector('.remove-dichvu-btn');
        if (removeBtn) {
            removeBtn.addEventListener('click', () => {
                row.remove();
                this.calculateTotal();
            });
        }
    },

    // Calculate total
    calculateTotal() {
        let tongTienPhong = 0;
        let tongTienDichVu = 0;

        // Calculate phong total
        document.querySelectorAll('.phong-row').forEach(row => {
            const thanhTien = parseFloat(row.querySelector('.thanh-tien-phong')?.value || 0);
            tongTienPhong += thanhTien;
        });

        // Calculate dich vu total
        document.querySelectorAll('.dichvu-row').forEach(row => {
            const thanhTien = parseFloat(row.querySelector('.thanh-tien-dichvu')?.value || 0);
            tongTienDichVu += thanhTien;
        });

        // Update display
        const tongTienPhongEl = document.getElementById('tongTienPhong');
        const tongTienDichVuEl = document.getElementById('tongTienDichVu');
        const tongCongEl = document.getElementById('tongCong');

        if (tongTienPhongEl) {
            tongTienPhongEl.textContent = tongTienPhong.toLocaleString('vi-VN') + ' đ';
        }
        if (tongTienDichVuEl) {
            tongTienDichVuEl.textContent = tongTienDichVu.toLocaleString('vi-VN') + ' đ';
        }
        if (tongCongEl) {
            tongCongEl.textContent = (tongTienPhong + tongTienDichVu).toLocaleString('vi-VN') + ' đ';
        }
    },

    // Load dat phong details
    async loadDatPhongDetails(id) {
        try {
            document.getElementById('loadingSpinner').style.display = 'block';
            document.getElementById('errorMessage').style.display = 'none';
            document.getElementById('detailsContent').style.display = 'none';

            const data = await HotelApp.apiFetch(`/DatPhong/${id}`);

            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('detailsContent').style.display = 'block';

            this.renderDatPhongDetails(data);
        } catch (error) {
            console.error('Error loading dat phong details:', error);
            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('errorMessage').style.display = 'block';
            document.getElementById('errorText').textContent = error.message;
            HotelApp.showError('Không thể tải thông tin đặt phòng: ' + error.message);
        }
    },

    // Render dat phong details
    renderDatPhongDetails(data) {
        // Basic info
        document.getElementById('id').textContent = data.maDatPhong || '';
        document.getElementById('khachHang').textContent = data.khachHang?.hoTen || '';
        document.getElementById('nhanVien').textContent = data.nhanVien?.hoTen || '';
        document.getElementById('ngayDat').textContent = HotelApp.formatDate(data.ngayDat);
        document.getElementById('ngayNhan').textContent = HotelApp.formatDate(data.ngayNhan);
        document.getElementById('ngayTra').textContent = HotelApp.formatDate(data.ngayTra);
        document.getElementById('trangThai').innerHTML = this.getStatusBadge(data.trangThai);

        // Chi tiet phong
        this.renderChiTietPhong(data.chiTietDatPhongs || []);

        // Dich vu
        this.renderDichVu(data.suDungDichVus || []);

        // Total
        this.renderTotal(data);

        // Action buttons
        this.setupActionButtons(data.maDatPhong, data.trangThai);
    },

    // Render chi tiet phong
    renderChiTietPhong(chiTietPhongs) {
        const tbody = document.getElementById('chiTietPhongTableBody');
        tbody.innerHTML = '';

        if (chiTietPhongs.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">Không có dữ liệu</td></tr>';
            return;
        }

        chiTietPhongs.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.phong?.SoPhong || ''}</td>
                <td>${HotelApp.formatCurrency(item.donGia || 0)}</td>
                <td>${item.soDem || ''}</td>
                <td>${HotelApp.formatCurrency((item.donGia || 0) * (item.soDem || 0))}</td>
            `;
            tbody.appendChild(row);
        });
    },

    // Render dich vu
    renderDichVu(dichVus) {
        const tbody = document.getElementById('dichVuTableBody');
        tbody.innerHTML = '';

        if (dichVus.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center text-muted">Không có dữ liệu</td></tr>';
            return;
        }

        dichVus.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.dichVu?.tenDichVu || ''}</td>
                <td>${item.soLuong || ''}</td>
                <td>${HotelApp.formatCurrency(item.donGia || 0)}</td>
                <td>${HotelApp.formatCurrency((item.donGia || 0) * (item.soLuong || 0))}</td>
            `;
            tbody.appendChild(row);
        });
    },

    // Render total
    renderTotal(data) {
        const tongTienPhong = (data.chiTietDatPhongs || []).reduce((sum, item) =>
            sum + ((item.donGia || 0) * (item.soDem || 0)), 0);
        const tongTienDichVu = (data.suDungDichVus || []).reduce((sum, item) =>
            sum + ((item.donGia || 0) * (item.soLuong || 0)), 0);

        document.getElementById('tongTienPhong').textContent = HotelApp.formatCurrency(tongTienPhong);
        document.getElementById('tongTienDichVu').textContent = HotelApp.formatCurrency(tongTienDichVu);
        document.getElementById('tongCong').textContent = HotelApp.formatCurrency(tongTienPhong + tongTienDichVu);
    },

    // Setup action buttons
    setupActionButtons(id, status) {
        const checkinBtn = document.getElementById('checkinBtn');
        const checkoutBtn = document.getElementById('checkoutBtn');
        const cancelBtn = document.getElementById('cancelBtn');

        if (checkinBtn) checkinBtn.style.display = status === 'DaDat' ? 'inline-block' : 'none';
        if (checkoutBtn) checkoutBtn.style.display = status === 'DaNhan' ? 'inline-block' : 'none';
        if (cancelBtn) cancelBtn.style.display = status === 'DaDat' ? 'inline-block' : 'none';

        if (checkinBtn) {
            checkinBtn.onclick = () => this.checkin(id);
        }
        if (checkoutBtn) {
            checkoutBtn.onclick = () => this.checkout(id);
        }
        if (cancelBtn) {
            cancelBtn.onclick = () => this.cancel(id);
        }
    },

    // Setup details page
    setupDetailsPage() {
        // Action modal setup is already done in setupEventListeners
    },

    // Check-in
    async checkin(id) {
        this.currentAction = 'checkin';
        this.currentId = id;
        this.showActionModal('Check-in', 'Bạn có chắc chắn muốn check-in cho đặt phòng này không?');
    },

    // Check-out
    async checkout(id) {
        this.currentAction = 'checkout';
        this.currentId = id;
        this.showActionModal('Check-out', 'Bạn có chắc chắn muốn check-out cho đặt phòng này không?');
    },

    // Cancel
    async cancel(id) {
        this.currentAction = 'cancel';
        this.currentId = id;
        this.showActionModal('Hủy đặt phòng', 'Bạn có chắc chắn muốn hủy đặt phòng này không?');
    },

    // Show action modal
    showActionModal(title, message) {
        document.getElementById('actionModalTitle').textContent = title;
        document.getElementById('actionModalMessage').textContent = message;
        const modal = new bootstrap.Modal(document.getElementById('actionModal'));
        modal.show();
    },

    // Confirm action
    async confirmAction() {
        try {
            let endpoint = '';
            let method = 'PUT';

            switch (this.currentAction) {
                case 'checkin':
                    endpoint = `/DatPhong/${this.currentId}/checkin`;
                    break;
                case 'checkout':
                    endpoint = `/DatPhong/${this.currentId}/checkout`;
                    break;
                case 'cancel':
                    endpoint = `/DatPhong/${this.currentId}`;
                    method = 'DELETE';
                    break;
            }

            await HotelApp.apiFetch(endpoint, { method });

            const actionNames = {
                'checkin': 'Check-in',
                'checkout': 'Check-out',
                'cancel': 'Hủy đặt phòng'
            };

            HotelApp.showSuccess(`${actionNames[this.currentAction]} thành công!`);

            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('actionModal'));
            modal.hide();

            // Reload page or redirect
            if (window.location.pathname.includes('/Details/')) {
                this.loadDatPhongDetails(this.currentId);
            } else {
                this.loadDatPhongList();
            }
        } catch (error) {
            console.error('Error performing action:', error);
            HotelApp.showError('Không thể thực hiện thao tác: ' + error.message);
        }
    }
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function () {
    // Auto-initialize based on current page
    const path = window.location.pathname;

    if (path.includes('/DatPhong/Index') || path === '/DatPhong' || path === '/DatPhong/') {
        DatPhongApp.initIndexPage();
    }
});