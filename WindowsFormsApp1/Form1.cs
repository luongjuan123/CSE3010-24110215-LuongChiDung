using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BUS;
using DTO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly NhanVienBUS _nhanVienBUS = new NhanVienBUS();
        private readonly PhongBanBUS _phongBanBUS = new PhongBanBUS();

        // Biến theo dõi nhân viên đang được chọn trên DataGridView
        private string _selectedMaNV = null;

        public Form1()
        {
            InitializeComponent();
        }

        #region Form Events

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Nạp danh sách phòng ban vào ComboBox
                LoadDanhSachPhongBan();

                // Nạp danh sách nhân viên vào DataGridView
                LoadDanhSachNhanVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không tải được dữ liệu từ cơ sở dữ liệu: " + ex.GetBaseException().Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Đưa form về trạng thái ban đầu theo yêu cầu Câu 1c
            ResetInputForm();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Yêu cầu Câu 4d: Xác nhận khi đóng bằng nút Thoát, nút X hoặc Esc.
            // Xử lý tập trung tại đây để tránh hiển thị 2 lần hộp thoại đóng Form.
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát chương trình?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
            {
                // Từ chối xác nhận -> hủy thao tác đóng
                e.Cancel = true;
            }
        }

        #endregion

        #region Load Data Methods

        /// <summary>
        /// Nạp danh sách phòng ban vào cboPhongBan theo yêu cầu Câu 4a
        /// Thiết lập DisplayMember và ValueMember trước DataSource để đảm bảo ComboBox hiển thị chính xác
        /// </summary>
        private void LoadDanhSachPhongBan()
        {
            var danhSach = _phongBanBUS.GetAll();
            cboPhongBan.DisplayMember = "TenPB";
            cboPhongBan.ValueMember = "MaPB";
            cboPhongBan.DataSource = danhSach;
            cboPhongBan.SelectedIndex = -1;
        }

        /// <summary>
        /// Nạp danh sách nhân viên vào dgvNhanVien theo yêu cầu Câu 4a
        /// </summary>
        private void LoadDanhSachNhanVien()
        {
            dgvNhanVien.DataSource = _nhanVienBUS.GetAllViewModels();
        }

        #endregion

        #region UI State Helpers

        /// <summary>
        /// Yêu cầu Câu 1c:
        /// Khi Form Load hoặc nhấn Mới:
        /// - Làm sạch dữ liệu nhập
        /// - Bỏ chọn dòng hiện tại
        /// - Đưa con trỏ về txtMaNV
        /// - Bật nút Thêm, tắt Sửa và Xóa
        /// - ComboBox ở trạng thái chưa chọn (SelectedIndex = -1)
        /// </summary>
        private void ResetInputForm()
        {
            txtMaNV.Clear();
            txtHoTen.Clear();
            txtDiaChi.Clear();
            txtLuong.Clear();

            rdoNam.Checked = true;
            rdoNu.Checked = false;
            dtpNgaySinh.Value = DateTime.Today;

            cboPhongBan.SelectedIndex = -1;

            _selectedMaNV = null;
            txtMaNV.ReadOnly = false;

            btnThem.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            dgvNhanVien.ClearSelection();
            txtMaNV.Focus();
        }

        #endregion

        #region Grid Selection Event

        /// <summary>
        /// Yêu cầu Câu 1c & Câu 4a:
        /// Khi chọn dòng hợp lệ:
        /// - Điền đúng toàn bộ thông tin và chọn ComboBox theo khóa ngoại
        /// - Tắt Thêm, bật Sửa/Xóa và không cho sửa mã
        /// </summary>
        private void dgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvNhanVien.Rows.Count)
            {
                return;
            }

            try
            {
                var selectedRow = dgvNhanVien.Rows[e.RowIndex];
                var vm = selectedRow.DataBoundItem as NhanVienViewModel;
                if (vm == null)
                {
                    return;
                }

                _selectedMaNV = vm.MaNV;

                txtMaNV.Text = vm.MaNV;
                txtHoTen.Text = vm.HoTen;

                if (vm.GioiTinh)
                {
                    rdoNam.Checked = true;
                }
                else
                {
                    rdoNu.Checked = true;
                }

                if (vm.NgaySinh > dtpNgaySinh.MinDate && vm.NgaySinh < dtpNgaySinh.MaxDate)
                {
                    dtpNgaySinh.Value = vm.NgaySinh;
                }

                txtDiaChi.Text = vm.DiaChi ?? "";
                txtLuong.Text = vm.Luong.ToString("0.##");

                // Chọn ComboBox theo khóa ngoại MaPB
                if (!string.IsNullOrEmpty(vm.MaPB))
                {
                    cboPhongBan.SelectedValue = vm.MaPB;
                    if (cboPhongBan.SelectedIndex == -1)
                    {
                        for (int i = 0; i < cboPhongBan.Items.Count; i++)
                        {
                            var pb = cboPhongBan.Items[i] as PhongBan;
                            if (pb != null && (pb.MaPB == vm.MaPB || pb.TenPB == vm.TenPB))
                            {
                                cboPhongBan.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    cboPhongBan.SelectedIndex = -1;
                }

                // Cập nhật trạng thái điều khiển
                txtMaNV.ReadOnly = true; // Không cho sửa mã
                btnThem.Enabled = false; // Tắt Thêm
                btnSua.Enabled = true;   // Bật Sửa
                btnXoa.Enabled = true;   // Bật Xóa
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị dữ liệu dòng chọn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        /// Nút Mới: Làm sạch dữ liệu và đưa form về trạng thái thêm mới
        /// </summary>
        private void btnMoi_Click(object sender, EventArgs e)
        {
            ResetInputForm();
        }

        /// <summary>
        /// Nút Thêm (Câu 4b, 4c): Thêm nhân viên qua BUS và DAL
        /// </summary>
        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra sơ bộ định dạng lương tại GUI để hỗ trợ người dùng nhập liệu
                if (string.IsNullOrWhiteSpace(txtLuong.Text))
                {
                    MessageBox.Show("Vui lòng nhập lương!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLuong.Focus();
                    return;
                }

                if (!decimal.TryParse(txtLuong.Text.Trim(), out decimal luong))
                {
                    MessageBox.Show("Lương phải là số hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLuong.Focus();
                    return;
                }

                string maPB = null;
                if (cboPhongBan.SelectedItem is PhongBan pb)
                {
                    maPB = pb.MaPB;
                }
                else if (cboPhongBan.SelectedValue != null)
                {
                    maPB = cboPhongBan.SelectedValue.ToString();
                }

                var nhanVien = new NhanVien
                {
                    MaNV = txtMaNV.Text,
                    HoTen = txtHoTen.Text,
                    GioiTinh = rdoNam.Checked,
                    NgaySinh = dtpNgaySinh.Value,
                    DiaChi = txtDiaChi.Text,
                    Luong = luong,
                    MaPB = maPB
                };

                // Gọi xử lý nghiệp vụ ở tầng BUS
                _nhanVienBUS.Add(nhanVien);

                // Thông báo thành công và nạp lại DataGridView
                MessageBox.Show("Thêm nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDanhSachNhanVien();
                ResetInputForm();
            }
            catch (Exception ex)
            {
                // Không thông báo thành công khi lưu thất bại (Câu 3c)
                MessageBox.Show(ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Nút Sửa (Câu 4b, 4c): Sửa thông tin nhân viên qua BUS và DAL (giữ nguyên khóa chính)
        /// </summary>
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaNV))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(txtLuong.Text))
                {
                    MessageBox.Show("Vui lòng nhập lương!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLuong.Focus();
                    return;
                }

                if (!decimal.TryParse(txtLuong.Text.Trim(), out decimal luong))
                {
                    MessageBox.Show("Lương phải là số hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLuong.Focus();
                    return;
                }

                string maPB = null;
                if (cboPhongBan.SelectedItem is PhongBan pb)
                {
                    maPB = pb.MaPB;
                }
                else if (cboPhongBan.SelectedValue != null)
                {
                    maPB = cboPhongBan.SelectedValue.ToString();
                }

                var nhanVien = new NhanVien
                {
                    MaNV = _selectedMaNV, // Giữ nguyên khóa chính theo yêu cầu
                    HoTen = txtHoTen.Text,
                    GioiTinh = rdoNam.Checked,
                    NgaySinh = dtpNgaySinh.Value,
                    DiaChi = txtDiaChi.Text,
                    Luong = luong,
                    MaPB = maPB
                };

                // Gọi xử lý nghiệp vụ ở tầng BUS
                _nhanVienBUS.Update(nhanVien);

                // Thông báo thành công và nạp lại DataGridView
                MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDanhSachNhanVien();
                ResetInputForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Nút Xóa (Câu 4b, 4d): Xác nhận trước khi xóa, xóa qua BUS và DAL
        /// </summary>
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaNV))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Câu 4d: Xác nhận trước khi xóa. Từ chối xác nhận phải hủy thao tác.
            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa nhân viên '{txtHoTen.Text.Trim()}' (Mã: {_selectedMaNV}) không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
            {
                return; // Từ chối xác nhận -> hủy thao tác
            }

            try
            {
                // Gọi xử lý nghiệp vụ ở tầng BUS
                _nhanVienBUS.Delete(_selectedMaNV);

                // Thông báo thành công và nạp lại DataGridView
                MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDanhSachNhanVien();
                ResetInputForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Nút Tìm kiếm: Hỗ trợ tìm kiếm nhân viên theo từ khóa
        /// </summary>
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtMaNV.Text.Trim();
                if (string.IsNullOrEmpty(keyword))
                {
                    keyword = txtHoTen.Text.Trim();
                }

                if (string.IsNullOrEmpty(keyword))
                {
                    // Nạp lại toàn bộ nếu không có từ khóa
                    LoadDanhSachNhanVien();
                    ResetInputForm();
                    return;
                }

                var ketQua = _nhanVienBUS.Search(keyword);
                dgvNhanVien.DataSource = ketQua;

                if (ketQua.Count == 0)
                {
                    MessageBox.Show($"Không tìm thấy nhân viên nào với từ khóa '{keyword}'!", "Kết quả tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Nút Thoát: Đóng form, sự kiện FormClosing sẽ kích hoạt để hiển thị hộp thoại xác nhận
        /// </summary>
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
