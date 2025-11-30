// LoaiPhong Management JavaScript
window.LoaiPhongApp = {
    // Initialize index page
    initIndexPage() {
        this.loadLoaiPhongList();
        this.setupEventListeners();
    },

    // Initialize create form
    initCreateForm() {
        this.setupCreateForm();
    },

    // Initialize edit form
    initEditForm(id) {
        this.loadLoaiPhongDetails(id);
        this.setupEditForm();
    },

    // Initialize details page
    initDetailsPage(id) {
        this.loadLoaiPhongDetails(id);
        this.setupDetailsPage();
    },

    // Load loai phong list
    async loadLoaiPhongList() {
        try {
            document.getElementById('loadingSpinner').style.display = 'block';
            document.getElementById('dataContainer').style.display = 'none';
            document.getElementById('errorMessage').style.display = 'none';

            const data = await HotelApp.apiFetch('/LoaiPhong');
            
            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('dataContainer').style.display = 'block';
            
            this.renderLoaiPhongTable(data);
        } catch (error) {
            console.error('Error loading loai phong list:', error);
            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('errorMessage').style.display = 'block';
            document.getElementById('errorText').textContent = error.message;
            HotelApp.showError('Không thể tải danh sách loại phòng: ' + error.message);
        }
    },

    // Render loai phong table
    renderLoaiPhongTable(data) {
        const tbody = document.getElementById('loaiPhongTableBody');
        tbody.innerHTML = '';

        if (!data || data.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center text-muted py-4">
                        <i class="fas fa-inbox fa-2x mb-2"></i><br>
                        Không có dữ liệu
                    </td>
                </tr>
            `;
            return;
        }

        data.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.maLoaiPhong || ''}</td>
                <td>${item.tenLoaiPhong || ''}</td>
                <td title="${item.moTa || ''}">${item.moTa || ''}</td>
                <td>${HotelApp.formatCurrency(item.giaMoiDem || 0)}</td>
                <td>${item.soGiuong || ''}</td>
                <td>${item.dienTich ? item.dienTich + ' m²' : ''}</td>
                <td>
                    <div class="btn-group btn-group-sm" role="group">
                        <a href="/LoaiPhong/Details/${item.maLoaiPhong}" class="btn btn-outline-info" title="Xem chi tiết">
                            <i class="fas fa-eye"></i>
                        </a>
                        <a href="/LoaiPhong/Edit/${item.maLoaiPhong}" class="btn btn-outline-warning" title="Sửa">
                            <i class="fas fa-edit"></i>
                        </a>
                        <button type="button" class="btn btn-outline-danger" onclick="LoaiPhongApp.deleteLoaiPhong(${item.maLoaiPhong})" title="Xóa">
                            <i class="fas fa-trash"></i>
                        </button>
                    </div>
                </td>
            `;
            tbody.appendChild(row);
        });
    },

    // Setup event listeners for index page
    setupEventListeners() {
        // Delete confirmation modal
        const deleteModal = document.getElementById('deleteModal');
        if (deleteModal) {
            const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');
            if (confirmDeleteBtn) {
                confirmDeleteBtn.addEventListener('click', () => {
                    if (this.deleteId) {
                        this.confirmDelete();
                    }
                });
            }
        }
    },

    // Setup create form
    setupCreateForm() {
        const form = document.getElementById('createForm');
        const submitBtn = document.getElementById('submitBtn');

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            if (!HotelApp.validateForm('createForm')) {
                return;
            }

            try {
                HotelApp.showLoading(submitBtn);
                
                const formData = new FormData(form);
                const data = Object.fromEntries(formData.entries());
                
                // Convert numeric fields
                data.giaMoiDem = parseFloat(data.giaMoiDem);
                data.soGiuong = parseInt(data.soGiuong);
                data.dienTich = data.dienTich ? parseFloat(data.dienTich) : null;

                await HotelApp.apiFetch('/LoaiPhong', {
                    method: 'POST',
                    body: JSON.stringify(data)
                });

                HotelApp.showSuccess('Thêm loại phòng thành công!');
                window.location.href = '/LoaiPhong';
            } catch (error) {
                console.error('Error creating loai phong:', error);
                HotelApp.showError('Không thể thêm loại phòng: ' + error.message);
            } finally {
                HotelApp.hideLoading(submitBtn, 'Lưu');
            }
        });
    },

    // Setup edit form
    setupEditForm() {
        const form = document.getElementById('editForm');
        const submitBtn = document.getElementById('submitBtn');

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            
            if (!HotelApp.validateForm('editForm')) {
                return;
            }

            try {
                HotelApp.showLoading(submitBtn);
                
                const formData = new FormData(form);
                const data = Object.fromEntries(formData.entries());
                const id = data.maLoaiPhong;
                
                // Convert numeric fields
                data.giaMoiDem = parseFloat(data.giaMoiDem);
                data.soGiuong = parseInt(data.soGiuong);
                data.dienTich = data.dienTich ? parseFloat(data.dienTich) : null;

                await HotelApp.apiFetch(`/LoaiPhong/${id}`, {
                    method: 'PUT',
                    body: JSON.stringify(data)
                });

                HotelApp.showSuccess('Cập nhật loại phòng thành công!');
                window.location.href = '/LoaiPhong';
            } catch (error) {
                console.error('Error updating loai phong:', error);
                HotelApp.showError('Không thể cập nhật loại phòng: ' + error.message);
            } finally {
                HotelApp.hideLoading(submitBtn, 'Cập nhật');
            }
        });
    },

    // Setup details page
    setupDetailsPage() {
        const editBtn = document.getElementById('editBtn');
        const deleteBtn = document.getElementById('deleteBtn');

        if (editBtn) {
            editBtn.addEventListener('click', (e) => {
                e.preventDefault();
                const id = document.getElementById('id').textContent;
                window.location.href = `/LoaiPhong/Edit/${id}`;
            });
        }

        if (deleteBtn) {
            deleteBtn.addEventListener('click', () => {
                const id = document.getElementById('id').textContent;
                this.deleteLoaiPhong(id);
            });
        }
    },

    // Load loai phong details
    async loadLoaiPhongDetails(id) {
        try {
            document.getElementById('loadingSpinner').style.display = 'block';
            document.getElementById('errorMessage').style.display = 'none';
            
            const editForm = document.getElementById('editForm');
            const detailsContent = document.getElementById('detailsContent');
            
            if (editForm) editForm.style.display = 'none';
            if (detailsContent) detailsContent.style.display = 'none';

            const data = await HotelApp.apiFetch(`/LoaiPhong/${id}`);
            
            document.getElementById('loadingSpinner').style.display = 'none';
            
            // Fill edit form if exists
            if (editForm) {
                document.getElementById('id').value = data.maLoaiPhong;
                document.getElementById('tenLoaiPhong').value = data.tenLoaiPhong || '';
                document.getElementById('giaMoiDem').value = data.giaMoiDem || '';
                document.getElementById('soGiuong').value = data.soGiuong || '';
                document.getElementById('dienTich').value = data.dienTich || '';
                document.getElementById('moTa').value = data.moTa || '';
                editForm.style.display = 'block';
            }
            
            // Fill details content if exists
            if (detailsContent) {
                document.getElementById('id').textContent = data.maLoaiPhong || '';
                document.getElementById('tenLoaiPhong').textContent = data.tenLoaiPhong || '';
                document.getElementById('giaMoiDem').textContent = HotelApp.formatCurrency(data.giaMoiDem || 0);
                document.getElementById('soGiuong').textContent = data.soGiuong || '';
                document.getElementById('dienTich').textContent = data.dienTich ? data.dienTich + ' m²' : '';
                document.getElementById('moTa').textContent = data.moTa || '';
                detailsContent.style.display = 'block';
            }
        } catch (error) {
            console.error('Error loading loai phong details:', error);
            document.getElementById('loadingSpinner').style.display = 'none';
            document.getElementById('errorMessage').style.display = 'block';
            document.getElementById('errorText').textContent = error.message;
            HotelApp.showError('Không thể tải thông tin loại phòng: ' + error.message);
        }
    },

    // Delete loai phong
    async deleteLoaiPhong(id) {
        try {
            const confirmed = await HotelApp.confirm(
                'Bạn có chắc chắn muốn xóa loại phòng này không?',
                'Xác nhận xóa'
            );
            
            if (!confirmed) return;

            await HotelApp.apiFetch(`/LoaiPhong/${id}`, {
                method: 'DELETE'
            });

            HotelApp.showSuccess('Xóa loại phòng thành công!');
            
            // Redirect to index if on details page, otherwise reload
            if (window.location.pathname.includes('/Details/')) {
                window.location.href = '/LoaiPhong';
            } else {
                this.loadLoaiPhongList();
            }
        } catch (error) {
            console.error('Error deleting loai phong:', error);
            HotelApp.showError('Không thể xóa loại phòng: ' + error.message);
        }
    }
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Auto-initialize based on current page
    const path = window.location.pathname;
    
    if (path.includes('/LoaiPhong/Index') || path === '/LoaiPhong' || path === '/LoaiPhong/') {
        LoaiPhongApp.initIndexPage();
    }
});
