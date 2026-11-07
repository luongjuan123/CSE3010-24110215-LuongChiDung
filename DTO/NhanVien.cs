using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key]
        [StringLength(20)]
        public string MaNV { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        public bool GioiTinh { get; set; } // true là Nam, false là Nữ

        public DateTime NgaySinh { get; set; }

        [StringLength(200)]
        public string DiaChi { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Luong { get; set; }

        [Required]
        [StringLength(10)]
        [ForeignKey("PhongBan")]
        public string MaPB { get; set; }

        // Navigation property: Mỗi nhân viên thuộc một phòng ban
        public virtual PhongBan PhongBan { get; set; }
    }
}
