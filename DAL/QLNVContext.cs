using System.Configuration;
using Microsoft.EntityFrameworkCore;
using DTO;

namespace DAL
{
    public class QLNVContext : DbContext
    {
        public DbSet<PhongBan> PhongBans { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Chuỗi kết nối được lưu và đọc từ tệp App.config với tên 'QLNVContext'
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["QLNVContext"].ConnectionString);
            }
        }
    }
}
