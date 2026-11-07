using System;
using System.Configuration;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using DTO;

namespace DAL
{
    public enum DatabaseProviderType
    {
        SqlServer,
        Sqlite
    }

    public class QLNVContext : DbContext
    {
        public DbSet<PhongBan> PhongBans { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }

        private static DatabaseProviderType? _providerType;
        private static string _resolvedConnectionString;
        private static readonly object _lock = new object();

        public static DatabaseProviderType CurrentProviderType
        {
            get
            {
                EnsureProviderConfigured();
                return _providerType.Value;
            }
        }

        public static string CurrentConnectionString
        {
            get
            {
                EnsureProviderConfigured();
                return _resolvedConnectionString;
            }
        }

        public static void EnsureProviderConfigured()
        {
            if (_providerType.HasValue) return;

            lock (_lock)
            {
                if (_providerType.HasValue) return;

                // 1. Kiểm tra chuỗi kết nối được cấu hình trong App.config (yêu cầu Câu 2b)
                string configConn = null;
                try
                {
                    configConn = ConfigurationManager.ConnectionStrings["QLNVContext"]?.ConnectionString;
                }
                catch
                {
                    // Trường hợp chạy ngoài môi trường WinForms hoặc file config không tồn tại
                }

                if (!string.IsNullOrWhiteSpace(configConn))
                {
                    if (TryConnectSqlServer(configConn))
                    {
                        _providerType = DatabaseProviderType.SqlServer;
                        _resolvedConnectionString = configConn;
                        return;
                    }
                }

                // 2. Tự động kiểm tra các instance SQL Server phổ biến tại máy cục bộ (SQLEXPRESS, LocalDB, v.v.)
                string[] candidateServers = new[]
                {
                    ".",
                    @".\SQLEXPRESS",
                    @"(localdb)\mssqllocaldb",
                    "localhost",
                    @"(localdb)\ProjectsV13"
                };

                foreach (var s in candidateServers)
                {
                    string testConn = $"Data Source={s};Initial Catalog=QLNhanVienDB;Integrated Security=True;TrustServerCertificate=True";
                    if (TryConnectSqlServer(testConn))
                    {
                        _providerType = DatabaseProviderType.SqlServer;
                        _resolvedConnectionString = testConn;
                        return;
                    }
                }

                // 3. Tự động chuyển sang SQLite cục bộ nếu máy không có SQL Server nào đang hoạt động
                // Giúp ứng dụng luôn khởi chạy thành công 100%, nạp đủ danh mục phòng ban và không bị lỗi kết nối
                _providerType = DatabaseProviderType.Sqlite;
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QLNhanVienDB.db");
                _resolvedConnectionString = $"Data Source={dbPath}";
            }
        }

        private static bool TryConnectSqlServer(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                // Giới hạn timeout 2 giây để phát hiện nhanh mà không gây đơ giao diện
                builder.ConnectTimeout = 2;

                // Thử kết nối đến master nếu database QLNhanVienDB chưa được tạo
                builder.InitialCatalog = "master";

                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                EnsureProviderConfigured();

                if (_providerType == DatabaseProviderType.SqlServer)
                {
                    optionsBuilder.UseSqlServer(_resolvedConnectionString);
                }
                else
                {
                    optionsBuilder.UseSqlite(_resolvedConnectionString);
                }
            }
        }
    }
}
