using System.Collections.Generic;
using DAL;
using DTO;

namespace BUS
{
    public class PhongBanBUS
    {
        private readonly PhongBanDAL _phongBanDAL = new PhongBanDAL();

        public void KhoiTaoDuLieu()
        {
            DatabaseInitializer.Initialize();
        }

        public List<PhongBan> GetAll()
        {
            return _phongBanDAL.GetAll();
        }

        public PhongBan GetById(string maPB)
        {
            if (string.IsNullOrWhiteSpace(maPB))
            {
                return null;
            }
            return _phongBanDAL.GetById(maPB.Trim());
        }
    }
}
