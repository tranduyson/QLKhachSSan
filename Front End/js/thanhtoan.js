const API_BASE_URL = 'https://localhost:7105';
        let allInvoices = [];
        let selectedInvoice = null;
        let selectedPaymentMethod = null;
        let currentToken = '';
        let isApiMode = false;

        // Demo data fallback
        const demoInvoices = [
            { maHD: 10, soHD: 'HD010', maKhach: 10, ngayLap: '2025-10-05T15:00:00', tongTien: 3500000, soTienDaTra: 0, soTienConNo: 3500000, hinhThucThanhToan: '' },
            { maHD: 11, soHD: 'HD011', maKhach: 11, ngayLap: '2025-10-09T10:00:00', tongTien: 6800000, soTienDaTra: 3000000, soTienConNo: 3800000, hinhThucThanhToan: '' },
            { maHD: 12, soHD: 'HD012', maKhach: 12, ngayLap: '2025-10-10T16:20:00', tongTien: 4200000, soTienDaTra: 0, soTienConNo: 4200000, hinhThucThanhToan: '' },
            { maHD: 13, soHD: 'HD013', maKhach: 13, ngayLap: '2025-10-11T09:00:00', tongTien: 5500000, soTienDaTra: 2000000, soTienConNo: 3500000, hinhThucThanhToan: '' }
        ];

        const demoInvoiceDetails = {
            10: {
                maHD: 10,
                tongTien: 3500000,
                chiTiet: [
                    { maCTHD: 10, maHD: 10, maDatPhong: 15, maDV: null, soLuong: 2, donGia: 1500000, thanhTien: 3000000, soHD: 'HD010', ngayLap: '2025-10-05T15:00:00', tongTien: 3500000, hinhThucThanhToan: '', soTienDaTra: 0, soTienConNo: 3500000 },
                    { maCTHD: 11, maHD: 10, maDatPhong: null, maDV: 2, soLuong: 1, donGia: 500000, thanhTien: 500000, soHD: 'HD010', ngayLap: '2025-10-05T15:00:00', tongTien: 3500000, hinhThucThanhToan: '', soTienDaTra: 0, soTienConNo: 3500000 }
                ]
            },
            11: {
                maHD: 11,
                tongTien: 6800000,
                chiTiet: [
                    { maCTHD: 12, maHD: 11, maDatPhong: 16, maDV: null, soLuong: 3, donGia: 2000000, thanhTien: 6000000, soHD: 'HD011', ngayLap: '2025-10-09T10:00:00', tongTien: 6800000, hinhThucThanhToan: '', soTienDaTra: 3000000, soTienConNo: 3800000 },
                    { maCTHD: 13, maHD: 11, maDatPhong: null, maDV: 3, soLuong: 2, donGia: 400000, thanhTien: 800000, soHD: 'HD011', ngayLap: '2025-10-09T10:00:00', tongTien: 6800000, hinhThucThanhToan: '', soTienDaTra: 3000000, soTienConNo: 3800000 }
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

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const data = await response.json();
            return data;
        }

        // Load danh sách hóa đơn chưa thanh toán
        async function loadUnpaidInvoices() {
            try {
                document.getElementById('invoicesGrid').innerHTML = '<div class="loading">Đang tải dữ liệu...</div>';
                
                const response = await fetchAPI('/api-user/HoaDon');
                
                if (response.success) {
                    const data = response.data || [];

                    // Lọc hóa đơn chưa thanh toán
                    allInvoices = data.filter(inv => {
                        const conNo = inv.soTienConNo ?? (inv.tongTien - inv.soTienDaTra);
                        return conNo > 0;
                    });

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
            const grid = document.getElementById('invoicesGrid');
            
            if (invoices.length === 0) {
                grid.innerHTML = `
                    <div class="empty-state" style="grid-column: 1/-1;">
                        <div class="empty-state-icon">😊</div>
                        <h3>Không có hóa đơn chưa thanh toán</h3>
                        <p>Tất cả hóa đơn đã được thanh toán</p>
                    </div>
                `;
                return;
            }

            grid.innerHTML = invoices.map(inv => {
                const conNo = inv.soTienConNo || (inv.tongTien - inv.soTienDaTra);
                const isPartial = inv.soTienDaTra > 0 && conNo > 0;

                return `
                    <div class="invoice-card" onclick='openInvoiceDetail(${JSON.stringify(inv).replace(/'/g, "&#39;")})'>
                        <div class="invoice-card-header">
                            <div class="invoice-number">${inv.soHD}</div>
                            <span class="status-badge" style="${isPartial ? 'background: #ffc107; color: #333;' : ''}">
                                ${isPartial ? 'Một phần' : 'Chưa TT'}
                            </span>
                        </div>
                        <div class="customer-name">Mã khách: ${inv.maKhach}</div>
                        <div class="invoice-info">📅 ${formatDate(inv.ngayLap)}</div>
                        ${isPartial ? `<div class="invoice-info" style="color: #28a745;">✅ Đã trả: ${formatPrice(inv.soTienDaTra)}</div>` : ''}
                        <div class="amount-section">
                            <div class="amount-label">${isPartial ? 'Còn nợ' : 'Tổng tiền'}</div>
                            <div class="amount-value">${formatPrice(conNo)}</div>
                        </div>
                    </div>
                `;
            }).join('');
        }

        async function openInvoiceDetail(invoice) {
            selectedInvoice = invoice;
            selectedPaymentMethod = null;
            
            try {
                document.getElementById('invoiceDetailContent').innerHTML = '<div class="loading">Đang tải chi tiết...</div>';
                document.getElementById('invoiceModal').classList.add('show');

                if (isApiMode) {
                    const response = await fetchAPI(`/api-user/HoaDon/Getpayment/${invoice.maHD}`);
                    
                    if (response.success && response.data) {
                        displayPaymentDetail(response.data, invoice);
                    } else {
                        throw new Error('Không thể tải chi tiết');
                    }
                } else {
                    const detail = demoInvoiceDetails[invoice.maHD] || createDemoDetail(invoice);
                    displayPaymentDetail(detail, invoice);
                }
            } catch (error) {
                const detail = demoInvoiceDetails[invoice.maHD] || createDemoDetail(invoice);
                displayPaymentDetail(detail, invoice);
            }
        }

        function createDemoDetail(invoice) {
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

        function displayPaymentDetail(invoiceDetail, invoice) {
            const details = invoiceDetail.chiTiet || [];
            const firstDetail = details[0] || {};
            
            const conNo = invoice.soTienConNo || (invoice.tongTien - invoice.soTienDaTra);
            const tongTien = firstDetail.tongTien || invoice.tongTien;
            const soTienDaTra = firstDetail.soTienDaTra || invoice.soTienDaTra;
            const chuaThue = tongTien / 1.08;
            const thue = tongTien - chuaThue;

            const content = document.getElementById('invoiceDetailContent');
            content.innerHTML = `
                <div class="invoice-detail-section">
                    <h3>Thông tin hóa đơn</h3>
                    <div class="detail-row">
                        <span class="detail-label">Số hóa đơn</span>
                        <span class="detail-value">${invoice.soHD}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Ngày lập</span>
                        <span class="detail-value">${formatDate(invoice.ngayLap)}</span>
                    </div>
                    <div class="detail-row">
                        <span class="detail-label">Mã khách hàng</span>
                        <span class="detail-value">${invoice.maKhach}</span>
                    </div>
                </div>

                <h3 style="margin: 20px 0 15px; color: #2d5016;">Chi tiết dịch vụ</h3>
                <table class="items-table">
                    <thead>
                        <tr>
                            <th>Mô tả</th>
                            <th style="text-align: center;">SL</th>
                            <th style="text-align: right;">Đơn giá</th>
                            <th style="text-align: right;">Thành tiền</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${details.map(item => {
                            const moTa = item.maDatPhong ? `Phòng (Đặt phòng #${item.maDatPhong})` : `Dịch vụ #${item.maDV}`;
                            return `
                                <tr>
                                    <td>${moTa}</td>
                                    <td style="text-align: center;">${item.soLuong}</td>
                                    <td style="text-align: right;">${formatPrice(item.donGia)}</td>
                                    <td style="text-align: right;"><strong>${formatPrice(item.thanhTien)}</strong></td>
                                </tr>
                            `;
                        }).join('')}
                    </tbody>
                </table>

                <div class="total-section">
                    <div class="total-row">
                        <span>Tạm tính</span>
                        <strong>${formatPrice(chuaThue)}</strong>
                    </div>
                    <div class="total-row">
                        <span>VAT (8%)</span>
                        <strong>${formatPrice(thue)}</strong>
                    </div>
                    <div class="total-row">
                        <span>Tổng cộng</span>
                        <strong>${formatPrice(tongTien)}</strong>
                    </div>
                    ${soTienDaTra > 0 ? `
                        <div class="total-row" style="color: #28a745;">
                            <span>Đã thanh toán</span>
                            <strong>-${formatPrice(soTienDaTra)}</strong>
                        </div>
                    ` : ''}
                    <div class="total-row grand-total">
                        <span>CÒN PHẢI TRẢ</span>
                        <strong>${formatPrice(conNo)}</strong>
                    </div>
                </div>

                <div class="payment-section">
                    <h3>Thanh toán</h3>
                    
                    <div class="form-group">
                        <label>Hình thức thanh toán *</label>
                        <div class="payment-methods">
                            <div class="payment-method" onclick="selectPaymentMethod('TienMat')">
                                <div class="payment-method-icon">💵</div>
                                <div class="payment-method-label">Tiền mặt</div>
                            </div>
                            <div class="payment-method" onclick="selectPaymentMethod('The')">
                                <div class="payment-method-icon">💳</div>
                                <div class="payment-method-label">Thẻ</div>
                            </div>
                            <div class="payment-method" onclick="selectPaymentMethod('ChuyenKhoan')">
                                <div class="payment-method-icon">🏦</div>
                                <div class="payment-method-label">Chuyển khoản</div>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Số tiền thanh toán *</label>
                        <input type="number" id="paymentAmount" value="${conNo}" min="0" max="${conNo}"
                               style="font-size: 18px; font-weight: bold; color: #dc3545;">
                        <small style="color: #666; font-size: 12px; margin-top: 5px; display: block;">
                            Tối đa: ${formatPrice(conNo)}
                        </small>
                    </div>

                    <button class="btn btn-success" onclick="processPayment()" id="paymentBtn"
                            style="width: 100%; padding: 15px; font-size: 16px;">
                        ✅ Xác Nhận Thanh Toán
                    </button>
                </div>
            `;
        }

        function selectPaymentMethod(method) {
            selectedPaymentMethod = method;
            document.querySelectorAll('.payment-method').forEach(el => {
                el.classList.remove('selected');
            });
            event.currentTarget.classList.add('selected');
        }

        async function processPayment() {
            if (!selectedPaymentMethod) {
                alert('⚠️ Vui lòng chọn hình thức thanh toán!');
                return;
            }

            const amount = parseFloat(document.getElementById('paymentAmount').value);
            const conNo = selectedInvoice.soTienConNo || (selectedInvoice.tongTien - selectedInvoice.soTienDaTra);

            if (!amount || amount <= 0) {
                alert('⚠️ Vui lòng nhập số tiền thanh toán!');
                return;
            }

            if (amount > conNo) {
                alert('⚠️ Số tiền thanh toán không được vượt quá số tiền còn nợ!');
                return;
            }

            const paymentBtn = document.getElementById('paymentBtn');
            paymentBtn.disabled = true;
            paymentBtn.textContent = 'Đang xử lý...';

            try {
                if (isApiMode) {
                    const paymentData = {
                        soTienTra: amount,
                        hinhThucThanhToan: selectedPaymentMethod,
                        tinhTrang: "SanSang"
                    };

                    const response = await fetchAPI(`/api-user/HoaDon/payment/${selectedInvoice.maHD}`, {
                        method: 'PUT',
                        body: JSON.stringify(paymentData)
                    });

                    if (response.success) {
                        showSuccess();
                    } else {
                        throw new Error(response.message || 'Thanh toán thất bại');
                    }
                } else {
                    // Demo mode
                    await new Promise(resolve => setTimeout(resolve, 1000));
                    
                    const invoice = allInvoices.find(i => i.maHD === selectedInvoice.maHD);
                    if (invoice) {
                        invoice.soTienDaTra += amount;
                        invoice.soTienConNo -= amount;
                        invoice.hinhThucThanhToan = selectedPaymentMethod;
                        
                        if (invoice.soTienConNo <= 0) {
                            allInvoices = allInvoices.filter(i => i.maHD !== selectedInvoice.maHD);
                        }
                    }
                    
                    showSuccess();
                }
            } catch (error) {
                console.error('Error:', error);
                alert('❌ Lỗi thanh toán: ' + error.message);
                paymentBtn.disabled = false;
                paymentBtn.textContent = '✅ Xác Nhận Thanh Toán';
            }
        }

        function showSuccess() {
            closeModal();
            document.getElementById('successModal').classList.add('show');

            setTimeout(() => {
                if (isApiMode) {
                    loadUnpaidInvoices();
                } else {
                    updateStats();
                    displayInvoices(allInvoices);
                }
            }, 500);
        }

        function closeModal() {
            document.getElementById('invoiceModal').classList.remove('show');
        }

        function closeSuccessModal() {
            document.getElementById('successModal').classList.remove('show');
        }

        function filterInvoices() {
            const search = document.getElementById('searchInput').value.toLowerCase();
            
            const filtered = allInvoices.filter(inv => {
                return inv.soHD.toLowerCase().includes(search) ||
                       inv.maKhach.toString().includes(search);
            });
            
            displayInvoices(filtered);
        }

        function updateStats() {
            const unpaidInvoices = allInvoices.length;
            const partialPaid = allInvoices.filter(inv => inv.soTienDaTra > 0).length;
            const totalAmount = allInvoices.reduce((sum, inv) => {
                const conNo = inv.soTienConNo || (inv.tongTien - inv.soTienDaTra);
                return sum + conNo;
            }, 0);

            document.getElementById('totalUnpaid').textContent = unpaidInvoices;
            document.getElementById('partialPaid').textContent = partialPaid;
            document.getElementById('totalAmount').textContent = formatPrice(totalAmount).replace('₫', '').trim() + 'đ';
        }

        function formatPrice(price) {
            return new Intl.NumberFormat('vi-VN', { 
                style: 'currency', 
                currency: 'VND',
                minimumFractionDigits: 0
            }).format(price || 0);
        }

        function formatDate(dateStr) {
            if (!dateStr) return '';
            return new Date(dateStr).toLocaleDateString('vi-VN');
        }

        function logout() {
            if (confirm('Bạn có muốn đăng xuất?')) {
                localStorage.removeItem('token');
                window.location.href = '/index.html';
            }
        }

        // Initialize
        checkAuth();
        checkRole(['Admin', 'KeToan','LeTan']);
        loadUnpaidInvoices();