using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAn_LTQL
{
    public partial class frmHoaDon : Form
    {
        Database db;
        int idHD = -1;
        public frmHoaDon()
        {
            InitializeComponent();
        }

        private void frmHoaDon_Load(object sender, EventArgs e)
        {
            db = new Database(@".\SQLEXPRESS", "QLSB");
            LoadData();
        }
        private void LoadData()
        {
            string sql = @"
                SELECT h.MaHD AS N'Mã hóa đơn', nv.TenNV AS N'Nhân viên', 
                       kh.TenKH AS N'Khách hàng', h.NgayLapHD AS N'Ngày lập', h.ThanhTien AS N'Thành tiền' 
                FROM HOA_DON h 
                JOIN NHAN_VIEN nv ON h.MaNV = nv.MaNV 
                JOIN KHACH_HANG kh ON h.MaKH = kh.MaKH";

            DataTable dt = db.laydl(sql);
            dgvDSHD.DataSource = dt;

            // Thêm cột nút bấm "Xem chi tiết" vào cuối lưới nếu chưa có
            if (!dgvDSHD.Columns.Contains("ChiTiet"))
            {
                DataGridViewButtonColumn btnChiTiet = new DataGridViewButtonColumn();
                btnChiTiet.Name = "ChiTiet";
                btnChiTiet.HeaderText = "Chi tiết";
                btnChiTiet.Text = "Xem";
                btnChiTiet.UseColumnTextForButtonValue = true;
                dgvDSHD.Columns.Add(btnChiTiet);
            }
        }

        private void dgvDSHD_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                idHD = Convert.ToInt32(dgvDSHD.Rows[e.RowIndex].Cells["Mã hóa đơn"].Value);

                // Nếu click trúng cột "Chi tiết"
                if (dgvDSHD.Columns[e.ColumnIndex].Name == "ChiTiet")
                {
                    // Truyền mã hóa đơn qua form Chi Tiết
                    frmChiTietHoaDon frm = new frmChiTietHoaDon(idHD);
                    frm.ShowDialog();
                    LoadData(); // Load lại lưới khi tắt form Chi Tiết
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtTimKiem.Text.Trim();
            string sql = $@"
                SELECT h.MaHD AS N'Mã hóa đơn', nv.TenNV AS N'Nhân viên', 
                       kh.TenKH AS N'Khách hàng', h.NgayLapHD AS N'Ngày lập', h.ThanhTien AS N'Thành tiền' 
                FROM HOA_DON h 
                JOIN NHAN_VIEN nv ON h.MaNV = nv.MaNV 
                JOIN KHACH_HANG kh ON h.MaKH = kh.MaKH
                WHERE nv.TenNV LIKE N'%{tuKhoa}%' OR kh.TenKH LIKE N'%{tuKhoa}%'";

            dgvDSHD.DataSource = db.laydl(sql);
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            idHD = -1;
            LoadData();
        }

        private void btnXoaHoaDon_Click(object sender, EventArgs e)
        {
            if (idHD == -1)
            {
                MessageBox.Show("Vui lòng chọn Hóa đơn cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Xóa Hóa đơn này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string sql = $"DELETE FROM HOA_DON WHERE MaHD = {idHD}";
                    SqlCommand cmd = new SqlCommand(sql, db.cn);
                    db.thucthi(cmd);

                    MessageBox.Show("Đã xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    idHD = -1;
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Files|*.xlsx";
            sfd.FileName = "DS_HoaDon_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataTable dtExcel = (DataTable)dgvDSHD.DataSource;
                XLWorkbook wb = new XLWorkbook();
                wb.Worksheets.Add(dtExcel, "HoaDon");
                wb.SaveAs(sfd.FileName);
                MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
