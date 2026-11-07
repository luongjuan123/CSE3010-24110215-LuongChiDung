using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using DTO;

namespace BUS
{
    public class NhanVienBUS
    {
        private readonly NhanVienDAL _nhanVienDAL = new NhanVienDAL();

        /// <summary>
        /// Lấy toàn bộ danh sách nhân viên và ánh xạ sang ViewModel để hiển thị trên DataGridView
        /// </summary>
        public List<NhanVienViewModel> GetAllViewModels()
        {
            var list = _nhanVienDAL.GetAll();
            return list.Select(n => ToViewModel(n)).ToList();
        }

        /// <summary>
        /// Tìm kiếm nhân viên theo từ khóa (Mã NV, Họ tên, Địa chỉ, Tên phòng ban)
        /// </summary>
        public List<NhanVienViewModel> Search(string keyword)
        {
            var list = _nhanVienDAL.Search(keyword);
            return list.Select(n => ToViewModel(n)).ToList();
        }

        /// <summary>
        /// Lấy nhân viên theo mã
        /// </summary>
        public NhanVien GetById(string maNV)
        {
            if (string.IsNullOrWhiteSpace(maNV))
            {
                return null;
            }
            return _nhanVienDAL.GetById(maNV.Trim());
        }

        /// <summary>
        /// Thêm nhân viên mới có kiểm tra quy tắc nghiệp vụ
        /// </summary>
        public void Add(NhanVien nv)
        {
            // Kiểm tra quy tắc nghiệp vụ cho thao tác Thêm
            ValidateBusinessRules(nv, isAdding: true);

            _nhanVienDAL.Add(nv);
        }

        /// <summary>
        /// Sửa thông tin nhân viên (giữ nguyên khóa chính)
        /// </summary>
        public void Update(NhanVien nv)
        {
            // Kiểm tra quy tắc nghiệp vụ cho thao tác Sửa
            ValidateBusinessRules(nv, isAdding: false);

            _nhanVienDAL.Update(nv);
        }

        /// <summary>
        /// Xóa nhân viên theo mã
        /// </summary>
        public void Delete(string maNV)
        {
            if (string.IsNullOrWhiteSpace(maNV))
            {
                throw new ArgumentException("Mã nhân viên không hợp lệ!");
            }

            maNV = maNV.Trim();
            if (!_nhanVienDAL.KiemTraTrungMa(maNV))
            {
                throw new KeyNotFoundException($"Không tìm thấy nhân viên có mã '{maNV}' để xóa!");
            }

            _nhanVienDAL.Delete(maNV);
        }

        /// <summary>
        /// Kiểm tra các ràng buộc nghiệp vụ theo yêu cầu Câu 4c của đề thi
        /// </summary>
        private void ValidateBusinessRules(NhanVien nv, bool isAdding)
        {
            if (nv == null)
            {
                throw new ArgumentNullException(nameof(nv), "Dữ liệu nhân viên không được để trống!");
            }

            // 1. Mã không được trống sau khi loại bỏ khoảng trắng đầu/cuối
            if (string.IsNullOrWhiteSpace(nv.MaNV))
            {
                throw new ArgumentException("Mã nhân viên không được để trống!");
            }
            nv.MaNV = nv.MaNV.Trim();

            // 2. Họ tên không được trống sau khi loại bỏ khoảng trắng đầu/cuối
            if (string.IsNullOrWhiteSpace(nv.HoTen))
            {
                throw new ArgumentException("Họ và tên không được để trống!");
            }
            nv.HoTen = nv.HoTen.Trim();

            // 3. Phải chọn phòng ban (khóa ngoại bắt buộc)
            if (string.IsNullOrWhiteSpace(nv.MaPB))
            {
                throw new ArgumentException("Vui lòng chọn phòng ban cho nhân viên!");
            }
            nv.MaPB = nv.MaPB.Trim();

            // 4. Ngày sinh không được lớn hơn ngày hiện tại
            if (nv.NgaySinh.Date > DateTime.Today)
            {
                throw new ArgumentException("Ngày sinh không được lớn hơn ngày hiện tại!");
            }

            // 5. Lương là số lớn hơn 0
            if (nv.Luong <= 0)
            {
                throw new ArgumentException("Lương phải là số lớn hơn 0!");
            }

            if (!string.IsNullOrEmpty(nv.DiaChi))
            {
                nv.DiaChi = nv.DiaChi.Trim();
            }

            // 6. Kiểm tra mã trùng khi thêm mới
            if (isAdding)
            {
                if (_nhanVienDAL.KiemTraTrungMa(nv.MaNV))
                {
                    throw new InvalidOperationException($"Mã nhân viên '{nv.MaNV}' đã tồn tại trong hệ thống! Vui lòng nhập mã khác.");
                }
            }
            else
            {
                // Khi sửa: kiểm tra nhân viên có tồn tại trong CSDL không
                if (!_nhanVienDAL.KiemTraTrungMa(nv.MaNV))
                {
                    throw new KeyNotFoundException($"Không tìm thấy nhân viên có mã '{nv.MaNV}' để sửa!");
                }
            }
        }

        private static NhanVienViewModel ToViewModel(NhanVien n)
        {
            return new NhanVienViewModel
            {
                MaNV = n.MaNV,
                HoTen = n.HoTen,
                GioiTinh = n.GioiTinh,
                NgaySinh = n.NgaySinh,
                DiaChi = n.DiaChi,
                Luong = n.Luong,
                MaPB = n.MaPB,
                TenPB = n.PhongBan != null ? n.PhongBan.TenPB : ""
            };
        }
    }
}
