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
    public partial class frmChiTietHoaDon : Form
    {
        Database db;
        int _maHD;       
        int _maDatSan;
        public frmChiTietHoaDon(int maHD)
        {
            InitializeComponent();
            _maHD = maHD;
        }

        private void frmChiTietHoaDon_Load(object sender, EventArgs e)
        {
            db = new Database(@".\SQLEXPRESS", "QLSB");

            // Đổ dữ liệu vào ComboBox
            cboNhanVienLap.DataSource = db.laydl("SELECT MaNV, TenNV FROM NHAN_VIEN");
            cboNhanVienLap.DisplayMember = "TenNV";
            cboNhanVienLap.ValueMember = "MaNV";

            cboKhachHang.DataSource = db.laydl("SELECT MaKH, TenKH FROM KHACH_HANG");
            cboKhachHang.DisplayMember = "TenKH";
            cboKhachHang.ValueMember = "MaKH";

            cboTenSan.DataSource = db.laydl("SELECT MaSan, TenSan FROM SAN_BONG");
            cboTenSan.DisplayMember = "TenSan";
            cboTenSan.ValueMember = "MaSan";

            LoadChiTiet();
        }

        private void LoadChiTiet()
        {
            string sql = $@"
                SELECT h.MaNV, h.MaKH, h.NgayLapHD, h.TienThueSan, d.MaSan, d.MaDatSan 
                FROM HOA_DON h
                JOIN DAT_SAN d ON h.MaDatSan = d.MaDatSan
                WHERE h.MaHD = {_maHD}";

            DataTable dt = db.laydl(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                _maDatSan = Convert.ToInt32(r["MaDatSan"]);

                cboNhanVienLap.SelectedValue = r["MaNV"];
                cboKhachHang.SelectedValue = r["MaKH"];
                cboTenSan.SelectedValue = r["MaSan"];

                txtGhiChuHoaDon.Text = Convert.ToDateTime(r["NgayLapHD"]).ToString("dd/MM/yyyy HH:mm");
                txtGhiChuHoaDon.Enabled = false;

                numDonGia.Value = Convert.ToDecimal(r["TienThueSan"]);
                numSoLuong.Value = 1;
            }
        }

        private void btnXacNhanSua_Click(object sender, EventArgs e)
        {
            try
            {
                decimal tongTien = numDonGia.Value * numSoLuong.Value;

                string sqlHD = @"UPDATE HOA_DON SET MaNV = @MaNV, MaKH = @MaKH, TienThueSan = @TienThueSan WHERE MaHD = @MaHD";
                SqlCommand cmdHD = new SqlCommand(sqlHD, db.cn);
                cmdHD.Parameters.AddWithValue("@MaNV", cboNhanVienLap.SelectedValue);
                cmdHD.Parameters.AddWithValue("@MaKH", cboKhachHang.SelectedValue);
                cmdHD.Parameters.AddWithValue("@TienThueSan", tongTien);
                cmdHD.Parameters.AddWithValue("@MaHD", _maHD);
                db.thucthi(cmdHD);

                string sqlSan = @"UPDATE DAT_SAN SET MaSan = @MaSan WHERE MaDatSan = @MaDatSan";
                SqlCommand cmdSan = new SqlCommand(sqlSan, db.cn);
                cmdSan.Parameters.AddWithValue("@MaSan", cboTenSan.SelectedValue);
                cmdSan.Parameters.AddWithValue("@MaDatSan", _maDatSan);
                db.thucthi(cmdSan);

                MessageBox.Show("Sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            LoadChiTiet();
        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            frmHoaDonTinhTien frm = new frmHoaDonTinhTien(_maDatSan, true);
            frm.ShowDialog();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
