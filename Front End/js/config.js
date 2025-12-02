// config.js - Cấu hình API và hằng số

const CONFIG = {
    // API Configuration
    API_BASE_URL: 'https://localhost:7105',
    API_TIMEOUT: 30000, // 30 seconds
    
    // Endpoints
    ENDPOINTS: {
        // Auth
        LOGIN: '/api-admin/Login',
        
        // Users
        USERS: '/api-admin/NguoiDung',
        USER_BY_ID: (id) => `/api-admin/NguoiDung/${id}`,
        
        // Rooms
        ROOMS: '/api-admin/Phong',
        ROOM_BY_ID: (id) => `/api-admin/Phong/${id}`,
        ROOM_TYPES: '/api-admin/LoaiPhong',
        ROOM_TYPE_BY_ID: (id) => `/api-admin/LoaiPhong/${id}`,
        ROOM_PRICES: '/api-admin/Gia',
        
        // Bookings
        BOOKINGS: '/api-admin/DatPhong',
        BOOKING_BY_ID: (id) => `/api-admin/DatPhong/${id}`,
        
        // Check In/Out
        CHECKIN: '/api-admin/CheckIn',
        CHECKOUT: '/api-admin/CheckOut',
        CHANGE_ROOM: '/api-admin/ChuyenPhong',
        
        // Services
        SERVICES: '/api-admin/DichVu',
        SERVICE_BY_ID: (id) => `/api-admin/DichVu/${id}`,
        USE_SERVICE: '/api-admin/SuDungDichVu',
        
        // Invoices
        INVOICES: '/api-admin/HoaDon',
        INVOICE_BY_ID: (id) => `/api-admin/HoaDon/${id}`,
        INVOICE_PAYMENT: '/api-admin/HoaDon/ThanhToan',
        UNPAID_INVOICES: '/api-admin/HoaDon/ChuaThanhToan',
        
        // Customers
        CUSTOMERS: '/api-admin/Khach',
        CUSTOMER_BY_ID: (id) => `/api-admin/Khach/${id}`,
        
        // Reports
        REPORT_REVENUE: '/api-admin/BaoCao/DoanhThu',
        REPORT_OCCUPANCY: '/api-admin/BaoCao/CongSuatPhong',
        REPORT_CHANNELS: '/api-admin/BaoCao/KenhDatPhong',
    },
    
    // Local Storage Keys
    STORAGE_KEYS: {
        TOKEN: 'token',
        USER: 'currentUser',
        REMEMBER_ME: 'rememberMe',
    },
    
    // User Roles
    ROLES: {
        ADMIN: 'Admin',
        RECEPTIONIST: 'LeTan',
        ACCOUNTANT: 'KeToan',
        CUSTOMER: 'Khach'
    },
    
    // Room Status
    ROOM_STATUS: {
        AVAILABLE: 'SanSang',
        OCCUPIED: 'DaThue',
        MAINTENANCE: 'BaoTri',
        CLEANING: 'DonDep'
    },
    
    // Booking Status
    BOOKING_STATUS: {
        RESERVED: 'DaDat',
        CHECKED_IN: 'DaNhan',
        CHECKED_OUT: 'DaTra',
        CANCELLED: 'Huy'
    },
    
    // Payment Methods
    PAYMENT_METHODS: {
        CASH: 'TienMat',
        CARD: 'The',
        TRANSFER: 'ChuyenKhoan',
        MIXED: 'KetHop'
    },
    
    // Room Types
    ROOM_TYPES: {
        STANDARD: 'Standard',
        SUPERIOR: 'Superior',
        DELUXE: 'Deluxe',
        JUNIOR_SUITE: 'Junior Suite',
        EXECUTIVE_SUITE: 'Executive Suite',
        FAMILY: 'Family Room',
        PRESIDENTIAL: 'Presidential Suite'
    },
    
    // Default Room Prices (VND)
    DEFAULT_PRICES: {
        'Standard': 800000,
        'Superior': 1200000,
        'Deluxe': 1800000,
        'Junior Suite': 2500000,
        'Executive Suite': 3500000,
        'Family Room': 2800000,
        'Presidential Suite': 8000000
    },
    
    // VAT Rate
    VAT_RATE: 0.08, // 8%
    
    // Business Rules
    BUSINESS_RULES: {
        MIN_PASSWORD_LENGTH: 6,
        MAX_PASSWORD_LENGTH: 50,
        DEFAULT_CHECKIN_TIME: '14:00',
        DEFAULT_CHECKOUT_TIME: '12:00',
        CANCELLATION_FEE_PERCENT: 20,
        MIN_DEPOSIT_PERCENT: 30,
        MAX_ADVANCE_BOOKING_DAYS: 365,
    },
    
    // Pagination
    PAGINATION: {
        DEFAULT_PAGE_SIZE: 20,
        PAGE_SIZE_OPTIONS: [10, 20, 50, 100]
    },
    
    // Date Formats
    DATE_FORMATS: {
        DISPLAY: 'DD/MM/YYYY',
        DISPLAY_WITH_TIME: 'DD/MM/YYYY HH:mm',
        API: 'YYYY-MM-DD',
        API_WITH_TIME: 'YYYY-MM-DDTHH:mm:ss'
    },
    
    // Messages
    MESSAGES: {
        SUCCESS: {
            LOGIN: 'Đăng nhập thành công!',
            LOGOUT: 'Đăng xuất thành công!',
            SAVE: 'Lưu thành công!',
            DELETE: 'Xóa thành công!',
            UPDATE: 'Cập nhật thành công!',
            CREATE: 'Tạo mới thành công!',
            CHECKIN: 'Check in thành công!',
            CHECKOUT: 'Check out thành công!',
            PAYMENT: 'Thanh toán thành công!',
        },
        ERROR: {
            NETWORK: 'Lỗi kết nối mạng!',
            UNAUTHORIZED: 'Phiên đăng nhập hết hạn!',
            FORBIDDEN: 'Bạn không có quyền thực hiện thao tác này!',
            NOT_FOUND: 'Không tìm thấy dữ liệu!',
            VALIDATION: 'Dữ liệu không hợp lệ!',
            SERVER: 'Lỗi máy chủ!',
            UNKNOWN: 'Có lỗi xảy ra!'
        },
        CONFIRM: {
            DELETE: 'Bạn có chắc chắn muốn xóa?',
            LOGOUT: 'Bạn có chắc chắn muốn đăng xuất?',
            CANCEL: 'Bạn có chắc chắn muốn hủy?',
        }
    },
    
    // Demo Mode
    DEMO_MODE: true, // Set to false when connecting to real API
};

/**
 * API Helper Functions
 */
const API = {
    /**
     * Get headers with authentication
     */
    getHeaders() {
        const headers = {
            'Content-Type': 'application/json',
        };
        
        const token = localStorage.getItem(CONFIG.STORAGE_KEYS.TOKEN);
        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }
        
        return headers;
    },
    
    /**
     * Handle API response
     */
    async handleResponse(response) {
        if (response.status === 401) {
            localStorage.removeItem(CONFIG.STORAGE_KEYS.TOKEN);
            window.location.href = '/index.html';
            throw new Error(CONFIG.MESSAGES.ERROR.UNAUTHORIZED);
        }
        
        if (response.status === 403) {
            throw new Error(CONFIG.MESSAGES.ERROR.FORBIDDEN);
        }
        
        if (response.status === 404) {
            throw new Error(CONFIG.MESSAGES.ERROR.NOT_FOUND);
        }
        
        if (!response.ok) {
            throw new Error(CONFIG.MESSAGES.ERROR.SERVER);
        }
        
        return response.json();
    },
    
    /**
     * GET request
     */
    async get(endpoint, params = {}) {
        const url = new URL(CONFIG.API_BASE_URL + endpoint);
        Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));
        
        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: this.getHeaders(),
            });
            return await this.handleResponse(response);
        } catch (error) {
            console.error('API GET Error:', error);
            throw error;
        }
    },
    
    /**
     * POST request
     */
    async post(endpoint, data) {
        try {
            const response = await fetch(CONFIG.API_BASE_URL + endpoint, {
                method: 'POST',
                headers: this.getHeaders(),
                body: JSON.stringify(data),
            });
            return await this.handleResponse(response);
        } catch (error) {
            console.error('API POST Error:', error);
            throw error;
        }
    },
    
    /**
     * PUT request
     */
    async put(endpoint, data) {
        try {
            const response = await fetch(CONFIG.API_BASE_URL + endpoint, {
                method: 'PUT',
                headers: this.getHeaders(),
                body: JSON.stringify(data),
            });
            return await this.handleResponse(response);
        } catch (error) {
            console.error('API PUT Error:', error);
            throw error;
        }
    },
    
    /**
     * DELETE request
     */
    async delete(endpoint) {
        try {
            const response = await fetch(CONFIG.API_BASE_URL + endpoint, {
                method: 'DELETE',
                headers: this.getHeaders(),
            });
            return await this.handleResponse(response);
        } catch (error) {
            console.error('API DELETE Error:', error);
            throw error;
        }
    }
};

// Export for use in other files
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { CONFIG, API };
}