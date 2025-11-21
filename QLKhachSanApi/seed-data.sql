-- Thêm dữ liệu mẫu vào bảng LoaiPhong
INSERT INTO LoaiPhong (TenLoaiPhong, MoTa) VALUES 
(N'Phòng đơn', N'Phòng 1 giường'),
(N'Phòng đôi', N'Phòng 2 giường'),
(N'Phòng VIP', N'Phòng VIP cao cấp');

-- Thêm dữ liệu mẫu vào bảng Phong (sau khi có LoaiPhong)
-- Lấy MaLoaiPhong từ bảng LoaiPhong (thường sẽ là 1, 2, 3)
INSERT INTO Phong (SoPhong, MaLoaiPhong, TinhTrang) VALUES
(N'101', 1, N'Trống'),
(N'102', 1, N'Trống'),
(N'201', 2, N'Trống'),
(N'202', 2, N'Trống'),
(N'301', 3, N'Trống');
