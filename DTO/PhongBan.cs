using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO
{
    [Table("PhongBan")]
    public class PhongBan
    {
        [Key]
        [StringLength(10)]
        public string MaPB { get; set; }

        [Required]
        [StringLength(100)]
        public string TenPB { get; set; }

        // Navigation property: Một phòng ban có nhiều nhân viên
        public virtual ICollection<NhanVien> NhanViens { get; set; } = new HashSet<NhanVien>();
    }
}
