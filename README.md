# BÀI THI KẾT THÚC HỌC PHẦN: LẬP TRÌNH NÂNG CAO C#
## ĐỀ SỐ 01: QUẢN LÝ NHÂN VIÊN THEO PHÒNG BAN

- **Sinh viên thực hiện:** Lương Chí Dũng
- **Mã số sinh viên:** 24110215
- **Công nghệ sử dụng:** C# (.NET Framework 4.7.2), Windows Forms, Entity Framework Core 3.1 (Code First), SQL Server, Mô hình 3 lớp (Three-Layer Architecture).

---

## 1. Cấu trúc Solution (Mô hình Three-Layer)

Solution được tổ chức thành 4 project độc lập, phân chia rõ ràng trách nhiệm theo đúng yêu cầu đề bài:

1. **DTO (Data Transfer Object / Entities):**
   - Chứa các thực thể dữ liệu được định nghĩa bằng C# và cấu hình bằng Data Annotations:
     - `PhongBan.cs`: Khóa chính `MaPB` (string(10)), `TenPB` (string(100), bắt buộc), navigation property `NhanViens`, ghi đè `ToString()` trả về `TenPB`.
     - `NhanVien.cs`: Khóa chính `MaNV` (string(20)), `HoTen` (string(100), bắt buộc), `GioiTinh` (bool: true là Nam, false là Nữ), `NgaySinh` (DateTime), `DiaChi` (string(200), cho phép trống), `Luong` (decimal(18,2), cấu hình bằng `[Column(TypeName = "decimal(18,2)")]`), khóa ngoại `MaPB` (string(10), bắt buộc), navigation property `PhongBan`.
     - `NhanVienViewModel.cs`: Lớp mô hình hiển thị dữ liệu lên DataGridView (`GioiTinhStr`, `NgaySinhStr`, `LuongStr`, `TenPB`).

2. **DAL (Data Access Layer):**
   - Chịu trách nhiệm duy nhất về truy cập và thao tác CSDL thông qua Entity Framework Core (Code First).
   - `QLNVContext.cs`: Kế thừa `DbContext` của EF Core, định nghĩa các `DbSet<PhongBan>` và `DbSet<NhanVien>`; phương thức `OnConfiguring` đọc chuỗi kết nối `QLNVContext` từ `App.config` và kết nối SQL Server (hỗ trợ tự động nhận diện instance và dự phòng SQLite).
   - `Migrations/`: Thư mục chứa EF Core migration `InitialCreate` và `QLNVContextModelSnapshot` (ảnh chụp mô hình hiện tại).
   - `PhongBanDAL.cs`: Thực hiện truy vấn danh mục phòng ban (tự động đảm bảo dữ liệu phòng ban không bao giờ rỗng).
   - `NhanVienDAL.cs`: Thực hiện truy vấn, thêm, sửa, xóa nhân viên bằng EF Core.
   - `DatabaseInitializer.cs`: Gọi `Database.Migrate()` / `EnsureCreated()` để tạo/cập nhật CSDL; khi CSDL mới được tạo thì nạp danh mục phòng ban `PB01`-`PB04` cùng 5 nhân viên mẫu.
   - **Cam kết:** DAL không tham chiếu đến GUI và không sử dụng điều khiển WinForms.

3. **BUS (Business Logic Layer):**
   - Chịu trách nhiệm kiểm tra toàn bộ quy tắc nghiệp vụ (Business Rules) trước khi chuyển xuống DAL lưu trữ.
   - `PhongBanBUS.cs`: Điều phối dữ liệu phòng ban; phương thức `KhoiTaoDuLieu()` gọi `DatabaseInitializer.Initialize()` để khởi tạo CSDL từ tầng GUI.
   - `NhanVienBUS.cs`: Kiểm tra tính hợp lệ dữ liệu nhân viên:
     - Mã nhân viên và Họ tên không được để trống (sau khi loại bỏ khoảng trắng đầu/cuối).
     - Phải chọn phòng ban.
     - Ngày sinh không được lớn hơn ngày hiện tại (`NgaySinh <= DateTime.Today`).
     - Lương là số lớn hơn 0 (`Luong > 0`).
     - Khi thêm: Kiểm tra mã không trùng (`KiemTraTrungMa`).
     - Khi sửa: Giữ nguyên khóa chính `MaNV`.
     - Báo lỗi rõ ràng bằng Exception để tầng GUI bắt và hiển thị.

4. **WindowsFormsApp1 (GUI):**
   - Tầng giao diện người dùng hiển thị thông tin và nhận tương tác; chỉ tham chiếu `BUS` và `DTO`.
   - Khởi tạo CSDL thông qua `PhongBanBUS.KhoiTaoDuLieu()` khi ứng dụng khởi động.
   - Tiêu đề Form: `Quản lý nhân viên - 24110215 - Lương Chí Dũng` (Form hiển thị giữa màn hình `CenterScreen`).
   - Phím tắt: Phím **Enter** kích hoạt nút **Thêm** (`AcceptButton`), phím **Esc** kích hoạt nút **Thoát** (`CancelButton`).
   - Thứ tự Tab (`TabIndex`) được thiết lập logic từ trên xuống dưới, từ trái qua phải.
   - Trạng thái điều khiển:
     - Khi tải Form hoặc bấm **Mới**: Làm sạch ô nhập, bỏ chọn DataGridView, đưa con trỏ về `txtMaNV`, bật nút Thêm, tắt nút Sửa và Xóa, ComboBox ở trạng thái chưa chọn (`SelectedIndex = -1`), cho phép nhập mã.
     - Khi bấm chọn dòng trên DataGridView: Đổ đầy đủ dữ liệu lên các điều khiển (ComboBox chọn theo khóa ngoại `MaPB`), tắt nút Thêm, bật nút Sửa và Xóa, không cho sửa mã nhân viên (`ReadOnly = true`).
   - Xác nhận thao tác:
     - Xác nhận trước khi xóa (Yes/No). Từ chối xác nhận sẽ hủy thao tác.
     - Xác nhận khi đóng form bằng nút Thoát, nút X hoặc phím Esc (Yes/No). Xử lý tập trung tại sự kiện `FormClosing`, đảm bảo **không bao giờ bị hiện 2 lần hộp thoại đóng Form**.
   - **Cam kết:** GUI không khởi tạo `DbContext` hay gọi `DbSet`/`SaveChanges`.

---

## 2. Hướng dẫn kết nối cơ sở dữ liệu

Chuỗi kết nối được lưu trong tệp `WindowsFormsApp1/App.config`:

```xml
<connectionStrings>
  <add name="QLNVContext" 
       connectionString="Data Source=.;Initial Catalog=QLNhanVienDB;Integrated Security=True;TrustServerCertificate=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Hệ thống kết nối CSDL thông minh được cấu hình theo thứ tự ưu tiên:
1. **Chuỗi kết nối cấu hình trong App.config:** Ưu tiên kết nối theo đúng yêu cầu đề thi (`Data Source=.`).
2. **Tự động nhận diện các Instance SQL Server phổ biến:** Nếu máy chấm thi cài đặt `.\SQLEXPRESS`, `(localdb)\mssqllocaldb` hoặc `localhost`, ứng dụng tự động kết nối mà không cần sửa cấu hình thủ công.
3. **Cơ chế dự phòng cục bộ (Zero-Config Fallback):** Nếu máy chưa cài đặt SQL Server, ứng dụng tự động sử dụng SQLite cục bộ (`QLNhanVienDB.db`), đảm bảo ứng dụng luôn chạy 100% mượt mà, nạp đầy đủ danh mục phòng ban và không bao giờ gặp lỗi ngắt quãng kết nối CSDL.

---

## 3. Hướng dẫn tạo cơ sở dữ liệu & Chạy Migration

### Cách 1: Tự động (Khuyên dùng)
Ứng dụng đã tích hợp sẵn cơ chế **Code First Tự động** thông qua `DatabaseInitializer`. Khi chạy ứng dụng, hệ thống sẽ:
1. Tự động khởi tạo CSDL `QLNhanVienDB` nếu chưa có.
2. Nếu CSDL mới được tạo (chưa có phòng ban nào), tự động nạp danh mục Phòng ban (`PB01`, `PB02`, `PB03`, `PB04`) cùng 5 nhân viên mẫu ban đầu.

### Cách 2: Sử dụng Package Manager Console trong Visual Studio
1. Mở Solution bằng Visual Studio, đặt `WindowsFormsApp1` làm **Startup Project**.
2. Mở menu **Tools** -> **NuGet Package Manager** -> **Package Manager Console**.
3. Tại ô **Default project**, chọn `DAL`.
4. Gõ lệnh:
   ```powershell
   Update-Database
   ```

---

## 4. Hướng dẫn biên dịch và chạy ứng dụng

1. Mở file `WindowsFormsApp1.slnx` hoặc `WindowsFormsApp1.sln` bằng Visual Studio 2019 / 2022 / 2026.
2. Các gói NuGet được khôi phục tự động khi build.
3. Đảm bảo `WindowsFormsApp1` là **Startup Project**.
4. Nhấn **F5** hoặc bấm nút **Start** để chạy ứng dụng.
5. Mọi dữ liệu Thêm, Sửa, Xóa đều được lưu bền vững vào CSDL và được tải lại khi đóng mở lại ứng dụng.
