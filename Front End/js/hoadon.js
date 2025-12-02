const API_BASE_URL = 'https://localhost:7105';
        let allInvoices = [];
        let currentToken = '';
        let isApiMode = false;

        // Demo data fallback
        const demoInvoices = [
            { maHD: 1, soHD: 'HD001', maKhach: 7, ngayLap: '2025-10-06T11:00:00', tongTien: 10800000, soTienDaTra: 10800000, soTienConNo: 0, hinhThucThanhToan: 'ChuyenKhoan' },
            { maHD: 2, soHD: 'HD002', maKhach: 8, ngayLap: '2025-10-07T14:00:00', tongTien: 5400000, soTienDaTra: 5400000, soTienConNo: 0, hinhThucThanhToan: 'TienMat' },
            { maHD: 3, soHD: 'HD003', maKhach: 9, ngayLap: '2025-10-08T09:30:00', tongTien: 8200000, soTienDaTra: 8200000, soTienConNo: 0, hinhThucThanhToan: 'The' },
            { maHD: 10, soHD: 'HD010', maKhach: 10, ngayLap: '2025-10-05T15:00:00', tongTien: 3500000, soTienDaTra: 0, soTienConNo: 3500000, hinhThucThanhToan: '' },
            { maHD: 11, soHD: 'HD011', maKhach: 11, ngayLap: '2025-10-09T10:00:00', tongTien: 6800000, soTienDaTra: 3000000, soTienConNo: 3800000, hinhThucThanhToan: '' },
            { maHD: 12, soHD: 'HD012', maKhach: 12, ngayLap: '2025-10-10T16:20:00', tongTien: 4200000, soTienDaTra: 0, soTienConNo: 4200000, hinhThucThanhToan: '' }
        ];

        const demoInvoiceDetails = {
            1: {
                maHD: 1,
                tongTien: 10800000,
                chiTiet: [
                    { maCTHD: 1, maHD: 1, maDatPhong: 7, maDV: null, soLuong: 5, donGia: 2000000, thanhTien: 10000000, soHD: 'HD001', ngayLap: '2025-10-06T11:00:00', tongTien: 10800000, hinhThucThanhToan: 'ChuyenKhoan', soTienDaTra: 10800000, soTienConNo: 0 },
                    { maCTHD: 2, maHD: 1, maDatPhong: null, maDV: 1, soLuong: 4, donGia: 200000, thanhTien: 800000, soHD: 'HD001', ngayLap: '2025-10-06T11:00:00', tongTien: 10800000, hinhThucThanhToan: 'ChuyenKhoan', soTienDaTra: 10800000, soTienConNo: 0 }
                ]
            },
            10: {
                maHD: 10,
                tongTien: 3500000,
                chiTiet: [
                    { maCTHD: 10, maHD: 10, maDatPhong: 15, maDV: null, soLuong: 2, donGia: 1500000, thanhTien: 3000000, soHD: 'HD010', ngayLap: '2025-10-05T15:00:00', tongTien: 3500000, hinhThucThanhToan: '', soTienDaTra: 0, soTienConNo: 3500000 },
                    { maCTHD: 11, maHD: 10, maDatPhong: null, maDV: 2, soLuong: 1, donGia: 500000, thanhTien: 500000, soHD: 'HD010', ngayLap: '2025-10-05T15:00:00', tongTien: 3500000, hinhThucThanhToan: '', soTienDaTra: 0, soTienConNo: 3500000 }
                ]
            }
        };

        // Kiểm tra token
        function checkAuth() {
            currentToken = localStorage.getItem('token');
            return true;
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

            if (response.status === 401) {
                throw new Error('UNAUTHORIZED');
            }

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const data = await response.json();
            return data;
        }

        // Load danh sách hóa đơn
        async function loadInvoices() {
            try {
                document.getElementById('invoicesList').innerHTML = '<div class="loading">Đang tải dữ liệu...</div>';
                
                const response = await fetchAPI('/api-user/HoaDon');
                
                if (response.success) {
                    allInvoices = response.data || [];
                    isApiMode = true;
                    updateStats();
                    displayInvoices(allInvoices);
                } else {
                    throw new Error(response.message || 'API Error');
                }
            } catch (error) {
                console.warn('Chuyển sang dữ liệu demo');
                loadDemoData();
            }
        }

        // Load dữ liệu demo
        function loadDemoData() {
            allInvoices = [...demoInvoices];
            isApiMode = false;
            updateStats();
            displayInvoices(allInvoices);
        }

        function displayInvoices(invoices) {
            const container = document.getElementById('invoicesList');
            
            if (invoices.length === 0) {
                container.innerHTML = '<p style="text-align: center; color: #999; padding: 40px;">Không có hóa đơn nào</p>';
                return;
            }

            let html = '<table><thead><tr>';
            html += '<th>Số HĐ</th><th>Ngày lập</th><th>Mã khách</th><th>Tổng tiền</th><th>Đã trả</th><th>Còn nợ</th><th>Trạng thái</th><th>Thao tác</th>';
            html += '</tr></thead><tbody>';

            invoices.forEach(inv => {
                const conNo = inv.soTienConNo || 0;
                const status = conNo === 0 ? 'paid' : (inv.soTienDaTra > 0 ? 'partial' : 'unpaid');
                const statusClass = status === 'paid' ? 'status-paid' : (status === 'partial' ? 'status-partial' : 'status-unpaid');
                const statusText = status === 'paid' ? 'Đã thanh toán' : (status === 'partial' ? 'Thanh toán 1 phần' : 'Chưa thanh toán');

                html += '<tr>';
                html += `<td><strong>${inv.soHD}</strong></td>`;
                html += `<td>${formatDate(inv.ngayLap)}</td>`;
                html += `<td>${inv.maKhach}</td>`;
                html += `<td>${formatPrice(inv.tongTien)}</td>`;
                html += `<td>${formatPrice(inv.soTienDaTra)}</td>`;
                html += `<td>${formatPrice(conNo)}</td>`;
                html += `<td><span class="status-badge ${statusClass}">${statusText}</span></td>`;
                html += `<td><button class="btn btn-info" onclick="viewInvoice(${inv.maHD})">👁️ Xem</button></td>`;
                html += '</tr>';
            });

            html += '</tbody></table>';
            container.innerHTML = html;
        }

        // Xem chi tiết hóa đơn
        async function viewInvoice(maHD) {
            try {
                document.getElementById('invoiceDetail').innerHTML = '<div class="loading">Đang tải chi tiết...</div>';
                document.getElementById('invoiceModal').classList.add('show');

                if (isApiMode) {
                    const response = await fetchAPI(`/api-user/HoaDon/Getpayment/${maHD}`);
                    
                    if (response.success && response.data) {
                        displayInvoiceDetail(response.data);
                    } else {
                        throw new Error('Không thể tải chi tiết');
                    }
                } else {
                    const detail = demoInvoiceDetails[maHD] || createDemoDetail(maHD);
                    displayInvoiceDetail(detail);
                }
            } catch (error) {
                const detail = demoInvoiceDetails[maHD] || createDemoDetail(maHD);
                displayInvoiceDetail(detail);
            }
        }

        function createDemoDetail(maHD) {
            const invoice = allInvoices.find(inv => inv.maHD === maHD);
            if (!invoice) return null;

            return {
                maHD: invoice.maHD,
                tongTien: invoice.tongTien,
                chiTiet: [
                    { 
                        maCTHD: 1, 
                        maHD: invoice.maHD, 
                        maDatPhong: 1, 
                        maDV: null, 
                        soLuong: 2, 
                        donGia: invoice.tongTien / 2, 
                        thanhTien: invoice.tongTien,
                        soHD: invoice.soHD,
                        ngayLap: invoice.ngayLap,
                        tongTien: invoice.tongTien,
                        hinhThucThanhToan: invoice.hinhThucThanhToan,
                        soTienDaTra: invoice.soTienDaTra,
                        soTienConNo: invoice.soTienConNo
                    }
                ]
            };
        }

        function displayInvoiceDetail(invoice) {
            const details = invoice.chiTiet || [];
            const firstDetail = details[0] || {};
            
            const tongTien = firstDetail.tongTien || invoice.tongTien || 0;
            const soTienDaTra = firstDetail.soTienDaTra || 0;
            const conNo = firstDetail.soTienConNo || 0;
            const thue = tongTien * 0.08;
            const tongChuaThue = tongTien - thue;

            let html = `
                <div class="invoice-header">
                    <h1>HÓA ĐƠN</h1>
                    <p>Số hóa đơn: <strong>${firstDetail.soHD}</strong></p>
                    <p>Ngày: ${formatDate(firstDetail.ngayLap)}</p>
                </div>

                <div class="invoice-info">
                    <div class="invoice-section">
                        
                    </div>
                    <div class="invoice-section">
                        <h3>Thông tin thanh toán</h3>
                        <p><strong>Mã hóa đơn:</strong> ${invoice.maHD}</p>
                        <p><strong>Hình thức:</strong> ${getPaymentMethod(firstDetail.hinhThucThanhToan)}</p>
                    </div>
                </div>

                <table class="invoice-table">
                    <thead>
                        <tr>
                            <th>STT</th>
                            <th>Mô tả</th>
                            <th>Đơn giá</th>
                            <th>SL</th>
                            <th>Thành tiền</th>
                        </tr>
                    </thead>
                    <tbody>
            `;

            details.forEach((item, idx) => {
                const moTa = item.maDatPhong ? `Phòng ${item.maDatPhong})` : `Dịch vụ #${item.maDV}`;
                html += `
                    <tr>
                        <td>${idx + 1}</td>
                        <td>${moTa}</td>
                        <td>${formatPrice(item.donGia)}</td>
                        <td>${item.soLuong}</td>
                        <td>${formatPrice(item.thanhTien)}</td>
                    </tr>
                `;
            });

            html += `</tbody></table><div class="invoice-total"><div class="total-line"><span>Tổng chưa thuế:</span><strong>${formatPrice(tongChuaThue)}</strong></div><div class="total-line"><span>VAT (8%):</span><strong>${formatPrice(thue)}</strong></div><div class="total-line grand-total"><span>TỔNG CỘNG:</span><strong>${formatPrice(tongTien)}</strong></div><div class="total-line"><span>Đã thanh toán:</span><strong style="color: #28a745;">${formatPrice(soTienDaTra)}</strong></div><div class="total-line" style="color: ${conNo > 0 ? '#dc3545' : '#28a745'};"><span>Còn nợ:</span><strong>${formatPrice(conNo)}</strong></div></div><div style="margin-top: 30px; padding: 15px; background: #f8f9fa; border-radius: 8px;"><p style="margin-top: 10px; font-style: italic;">Cảm ơn quý khách đã sử dụng dịch vụ!</p></div>`;
            document.getElementById('invoiceDetail').innerHTML = html;
        }

        function filterInvoices() {
            let filtered = [...allInvoices];
            
            const status = document.getElementById('statusFilter').value;
            const search = document.getElementById('searchInput').value.toLowerCase();
            
            if (status) {
                filtered = filtered.filter(inv => {
                    const conNo = inv.soTienConNo || 0;
                    if (status === 'paid') return conNo === 0;
                    if (status === 'unpaid') return conNo === inv.tongTien;
                    if (status === 'partial') return conNo > 0 && conNo < inv.tongTien;
                    return true;
                });
            }
            
            if (search) {
                filtered = filtered.filter(inv => 
                    inv.soHD.toLowerCase().includes(search) ||
                    inv.maKhach.toString().includes(search)
                );
            }
            
            displayInvoices(filtered);
        }

        function updateStats() {
            const totalInvoices = allInvoices.length;
            const paidInvoices = allInvoices.filter(inv => (inv.soTienConNo || 0) === 0).length;
            const unpaidInvoices = totalInvoices - paidInvoices;
            const totalRevenue = allInvoices.reduce((sum, inv) => sum + (inv.tongTien || 0), 0);

            document.getElementById('totalInvoices').textContent = totalInvoices;
            document.getElementById('paidInvoices').textContent = paidInvoices;
            document.getElementById('unpaidInvoices').textContent = unpaidInvoices;
            document.getElementById('totalRevenue').textContent = formatPrice(totalRevenue).replace('₫', '').trim() + 'đ';
        }

        function closeModal() {
            document.getElementById('invoiceModal').classList.remove('show');
        }

        function formatPrice(price) {
            return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price || 0);
        }

        function formatDate(dateStr) {
            if (!dateStr) return '';
            return new Date(dateStr).toLocaleDateString('vi-VN');
        }

        function getPaymentMethod(method) {
            const methods = {
                'TienMat': 'Tiền mặt',
                'The': 'Thẻ',
                'ChuyenKhoan': 'Chuyển khoản'
            };
            return methods[method] || 'Chưa thanh toán';
        }

        function logout() {
            if (confirm('Bạn có muốn đăng xuất?')) {
                localStorage.removeItem('token');
                window.location.href = '/index.html';
            }
        }
        checkRole(['Admin','LeTan','KeToan']);
        // Initialize
        checkAuth();
        loadInvoices();