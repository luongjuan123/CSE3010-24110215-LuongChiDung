using System;
using System.Windows.Forms;
using BUS;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Tự động khởi tạo cơ sở dữ liệu và nạp dữ liệu danh mục ban đầu nếu CSDL chưa tồn tại
            try
            {
                new PhongBanBUS().KhoiTaoDuLieu();
            }
            catch (Exception ex)
            {
                // Nếu chưa bật SQL Server, ứng dụng vẫn tiếp tục mở Form và thông báo lỗi rõ ràng khi thao tác
                MessageBox.Show("Không thể khởi tạo cơ sở dữ liệu. Vui lòng kiểm tra SQL Server và chuỗi kết nối trong App.config.\n\nChi tiết: " + ex.GetBaseException().Message, "Lỗi cơ sở dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.Run(new Form1());
        }
    }
}
