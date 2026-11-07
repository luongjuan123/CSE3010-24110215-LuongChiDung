using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DTO;

namespace DAL
{
    public class NhanVienDAL
    {
        public List<NhanVien> GetAll()
        {
            using (var db = new QLNVContext())
            {
                return db.NhanViens
                         .Include(n => n.PhongBan)
                         .AsNoTracking()
                         .ToList();
            }
        }

        public NhanVien GetById(string maNV)
        {
            using (var db = new QLNVContext())
            {
                return db.NhanViens
                         .Include(n => n.PhongBan)
                         .AsNoTracking()
                         .FirstOrDefault(n => n.MaNV == maNV);
            }
        }

        public bool KiemTraTrungMa(string maNV)
        {
            using (var db = new QLNVContext())
            {
                return db.NhanViens.Any(n => n.MaNV == maNV);
            }
        }

        public void Add(NhanVien nv)
        {
            using (var db = new QLNVContext())
            {
                db.NhanViens.Add(nv);
                db.SaveChanges();
            }
        }

        public void Update(NhanVien nv)
        {
            using (var db = new QLNVContext())
            {
                var existing = db.NhanViens.Find(nv.MaNV);
                if (existing == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy nhân viên có mã: {nv.MaNV}");
                }

                existing.HoTen = nv.HoTen;
                existing.GioiTinh = nv.GioiTinh;
                existing.NgaySinh = nv.NgaySinh;
                existing.DiaChi = nv.DiaChi;
                existing.Luong = nv.Luong;
                existing.MaPB = nv.MaPB;

                db.SaveChanges();
            }
        }

        public void Delete(string maNV)
        {
            using (var db = new QLNVContext())
            {
                var existing = db.NhanViens.Find(maNV);
                if (existing == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy nhân viên có mã: {maNV}");
                }

                db.NhanViens.Remove(existing);
                db.SaveChanges();
            }
        }

        public List<NhanVien> Search(string keyword)
        {
            using (var db = new QLNVContext())
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return GetAll();
                }

                keyword = keyword.Trim().ToLower();
                return db.NhanViens
                         .Include(n => n.PhongBan)
                         .Where(n => n.MaNV.ToLower().Contains(keyword) ||
                                     n.HoTen.ToLower().Contains(keyword) ||
                                     (n.DiaChi != null && n.DiaChi.ToLower().Contains(keyword)) ||
                                     n.PhongBan.TenPB.ToLower().Contains(keyword))
                         .AsNoTracking()
                         .ToList();
            }
        }
    }
}
