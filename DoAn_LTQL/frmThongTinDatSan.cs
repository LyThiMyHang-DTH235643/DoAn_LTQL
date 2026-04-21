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
    public partial class frmThongTinDatSan : Form
    {
        Database db;
        int _maSan;
        string _tenSan;
        DateTime _thoiGianBatDau;
        DateTime _thoiGianKetThuc;
        public frmThongTinDatSan(int maSan, string tenSan, DateTime thoiGianBatDau)
        {
            InitializeComponent();
            _maSan = maSan;
            _tenSan = tenSan;
            _thoiGianBatDau = thoiGianBatDau;
            _thoiGianKetThuc = thoiGianBatDau.AddHours(1);
        }

        private void frmThongTinDatSan_Load(object sender, EventArgs e)
        {
            db = new Database(@"DESKTOP-MA5J22U\SQLSERVER2022", "QLSB");

            // 1. Set Tiêu đề hiển thị (Ví dụ: Sân A lúc 08:00-09:00, ngày 20/04/2026)
            lblTieuDe.Text = $"{_tenSan} lúc {_thoiGianBatDau:HH:mm}-{_thoiGianKetThuc:HH:mm}, ngày {_thoiGianBatDau:dd/MM/yyyy}";

            // 2. Load dữ liệu Khách hàng và Nhân viên
            LoadDuLieuComboBox();
        }
        private void LoadDuLieuComboBox()
        {
            // Đổ dữ liệu Khách hàng
            DataTable dtKH = db.laydl("SELECT MaKH, TenKH FROM KHACH_HANG");
            cboKhachHang.DataSource = dtKH;
            cboKhachHang.DisplayMember = "TenKH";
            cboKhachHang.ValueMember = "MaKH";

            // Đổ dữ liệu Nhân viên
            DataTable dtNV = db.laydl("SELECT MaNV, TenNV FROM NHAN_VIEN");
            cboNhanVien.DataSource = dtNV;
            cboNhanVien.DisplayMember = "TenNV";
            cboNhanVien.ValueMember = "MaNV";
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LuuThongTinDatSan(string trangThai)
        {
            if (cboKhachHang.SelectedValue == null || cboNhanVien.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Khách hàng và Nhân viên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int maKH = Convert.ToInt32(cboKhachHang.SelectedValue);
            int maNV = Convert.ToInt32(cboNhanVien.SelectedValue);
            string timeStart = _thoiGianBatDau.ToString("yyyy-MM-dd HH:mm:00");
            string timeEnd = _thoiGianKetThuc.ToString("yyyy-MM-dd HH:mm:00");

            try
            {
                // Insert vào CSDL
                string sql = $@"INSERT INTO DAT_SAN (MaKH, MaNV, MaSan, ThoiGianBatDau, ThoiGianKetThuc, TrangThaiDat) 
                                VALUES ({maKH}, {maNV}, {_maSan}, '{timeStart}', '{timeEnd}', N'{trangThai}')";

                SqlCommand cmd = new SqlCommand(sql, db.cn);
                db.thucthi(cmd);

                MessageBox.Show("Thao tác thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Lưu xong thì đóng form
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDatSan_Click(object sender, EventArgs e)
        {
            LuuThongTinDatSan("Đã đặt");
        }

        private void btnGiaoSan_Click(object sender, EventArgs e)
        {
            LuuThongTinDatSan("Đang sử dụng");
        }

        private void btnThemKhach_Click(object sender, EventArgs e)
        {
            frmKhachHang frm = new frmKhachHang();
            frm.ShowDialog();
            LoadDuLieuComboBox();
        }
    }
}
