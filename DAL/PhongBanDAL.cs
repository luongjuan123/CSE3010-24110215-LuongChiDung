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
                return db.PhongBans.AsNoTracking().ToList();
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
