using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DTO;

namespace DAL
{
    public static class DatabaseInitializer
    {
        private static readonly object _initLock = new object();

        public static void Initialize()
        {
            lock (_initLock)
            {
                using (var db = new QLNVContext())
                {
                    if (QLNVContext.CurrentProviderType == DatabaseProviderType.SqlServer)
                    {
                        try
                        {
                            db.Database.Migrate();
                        }
                        catch
                        {
                            // Nếu CSDL đã được tạo sẵn bằng script bên ngoài hoặc không dùng migration
                            db.Database.EnsureCreated();
                        }
                    }
                    else
                    {
                        db.Database.EnsureCreated();
                    }

                    // Khởi tạo danh mục phòng ban ban đầu để nạp vào ComboBox theo yêu cầu đề bài
                    if (!db.PhongBans.Any())
                    {
                        var danhSachPhongBan = new List<PhongBan>
                        {
                            new PhongBan { MaPB = "PB01", TenPB = "Phòng Kế toán" },
                            new PhongBan { MaPB = "PB02", TenPB = "Phòng Kỹ thuật" },
                            new PhongBan { MaPB = "PB03", TenPB = "Phòng Kinh doanh" },
                            new PhongBan { MaPB = "PB04", TenPB = "Phòng Nhân sự" }
                        };
                        db.PhongBans.AddRange(danhSachPhongBan);
                        db.SaveChanges();
                    }

                    // Khởi tạo dữ liệu mẫu nhân viên ban đầu khớp với giao diện đề bài
                    if (!db.NhanViens.Any())
                    {
                        var danhSachNhanVien = new List<NhanVien>
                        {
                            new NhanVien
                            {
                                MaNV = "NV001",
                                HoTen = "Nguyễn Văn An",
                                GioiTinh = true,
                                NgaySinh = new DateTime(1990, 5, 15),
                                DiaChi = "123 Lý Thường Kiệt, Hà Nội",
                                Luong = 12000000m,
                                MaPB = "PB01"
                            },
                            new NhanVien
                            {
                                MaNV = "NV002",
                                HoTen = "Trần Thị Bình",
                                GioiTinh = false,
                                NgaySinh = new DateTime(1992, 8, 20),
                                DiaChi = "45 Nguyễn Trãi, Hà Nội",
                                Luong = 11500000m,
                                MaPB = "PB03"
                            },
                            new NhanVien
                            {
                                MaNV = "NV003",
                                HoTen = "Lê Tấn Dũng",
                                GioiTinh = true,
                                NgaySinh = new DateTime(1988, 3, 10),
                                DiaChi = "78 Kim Mã, Hà Nội",
                                Luong = 15000000m,
                                MaPB = "PB02"
                            },
                            new NhanVien
                            {
                                MaNV = "NV004",
                                HoTen = "Phạm Thị Dung",
                                GioiTinh = false,
                                NgaySinh = new DateTime(1995, 11, 12),
                                DiaChi = "56 Cầu Giấy, Hà Nội",
                                Luong = 9800000m,
                                MaPB = "PB03"
                            },
                            new NhanVien
                            {
                                MaNV = "NV005",
                                HoTen = "Hoàng Văn Em",
                                GioiTinh = true,
                                NgaySinh = new DateTime(1991, 10, 5),
                                DiaChi = "89 Bạch Mai, Hà Nội",
                                Luong = 13200000m,
                                MaPB = "PB02"
                            }
                        };
                        db.NhanViens.AddRange(danhSachNhanVien);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
