using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DTO;

namespace DAL
{
    public class PhongBanDAL
    {
        public List<PhongBan> GetAll()
        {
            using (var db = new QLNVContext())
            {
                var list = db.PhongBans.AsNoTracking().ToList();
                if (list.Count == 0)
                {
                    // Tự động khởi tạo dữ liệu mẫu nếu danh mục phòng ban chưa có
                    DatabaseInitializer.Initialize();
                    list = db.PhongBans.AsNoTracking().ToList();
                }
                return list;
            }
        }

        public PhongBan GetById(string maPB)
        {
            using (var db = new QLNVContext())
            {
                return db.PhongBans.AsNoTracking().FirstOrDefault(p => p.MaPB == maPB);
            }
        }
    }
}
