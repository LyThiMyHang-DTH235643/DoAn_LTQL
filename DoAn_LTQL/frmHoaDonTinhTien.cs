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
    public partial class frmHoaDonTinhTien : Form
    {
        Database db;
        int _maDatSan;
        int _maKH;
        int _maNV;
        decimal _tienSan;
        bool _isXemLai;
        public frmHoaDonTinhTien(int maDatSan, bool isXemLai = false)
        {
            InitializeComponent();
            _maDatSan = maDatSan;
            _isXemLai = isXemLai;
        }

        private void frmHoaDonTinhTien_Load(object sender, EventArgs e)
        {
            db = new Database(@".\SQLEXPRESS", "QLSB");
            LoadChiTietHoaDon();

            if (_isXemLai == true)
            {
                btnXacNhanThanhToan.Enabled = false;
                btnXacNhanThanhToan.Visible = false;
                btnHuyBo.Text = "Đóng"; // Đổi chữ nút Hủy thành Đóng cho hợp lý
            }
        }

        private void LoadChiTietHoaDon()
        {
            // 1. Thêm JOIN với bảng NHAN_VIEN để lấy cột nv.TenNV
            string sql = $@"
        SELECT d.MaKH, d.MaNV, s.TenSan, s.GiaThueGio, k.TenKH, nv.TenNV 
        FROM DAT_SAN d
        JOIN SAN_BONG s ON d.MaSan = s.MaSan
        JOIN KHACH_HANG k ON d.MaKH = k.MaKH
        JOIN NHAN_VIEN nv ON d.MaNV = nv.MaNV
        WHERE d.MaDatSan = {_maDatSan}";

            DataTable dt = db.laydl(sql);

            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                _maKH = Convert.ToInt32(r["MaKH"]);
                _maNV = Convert.ToInt32(r["MaNV"]);
                _tienSan = Convert.ToDecimal(r["GiaThueGio"]);

                string tenKH = r["TenKH"].ToString();
                string tenNV = r["TenNV"].ToString(); // 2. Lấy tên nhân viên từ CSDL
                string tenSan = r["TenSan"].ToString();
                string ngayIn = DateTime.Now.ToString("dd/MM/yyyy");
                string bill = $@"
                         SÂN BÓNG TIẾN ĐẠT
        Trần Hưng Đạo - Phường Long Xuyên - An Giang
                         Điện thoại: 0359 299 545

                         HÓA ĐƠN TÍNH TIỀN

Mã HD: #{_maDatSan:000}                                       Ngày: {ngayIn}
Khách hàng: {tenKH}                           
Nhân viên: {tenNV}

Chi tiết hóa đơn:

Mã sân              Số giờ             Đơn giá             Thành tiền
{tenSan,-11}              1                {_tienSan,-12:N0}       {_tienSan,10:N0}
-----------------------------------------------------------
Tổng:                     1                                         {_tienSan,10:N0}
=======================================
Khách thanh toán:                                             {_tienSan,10:N0}

                                    Xin cảm ơn
                           Hẹn gặp lại Quý khách!
";
                lblHoaDonTinhTien.Text = bill;
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnXacNhanThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                // Ép kiểu tiền sân về số nguyên để SQL không bị lỗi dấu phẩy
                int tienSanSQL = Convert.ToInt32(_tienSan);

                // 1. Thêm dữ liệu vào bảng HOA_DON
                string sqlInsertHD = $@"
            INSERT INTO HOA_DON (MaDatSan, MaNV, MaKH, TienThueSan, GiamGia) 
            VALUES ({_maDatSan}, {_maNV}, {_maKH}, {tienSanSQL}, 0)";

                SqlCommand cmdInsert = new SqlCommand(sqlInsertHD, db.cn);
                db.thucthi(cmdInsert);

                // 2. Cập nhật trạng thái sân thành Hoàn thành trong bảng DAT_SAN
                string sqlUpdateSan = $"UPDATE DAT_SAN SET TrangThaiDat = N'Hoàn thành' WHERE MaDatSan = {_maDatSan}";
                SqlCommand cmdUpdate = new SqlCommand(sqlUpdateSan, db.cn);
                db.thucthi(cmdUpdate);

                MessageBox.Show("Thanh toán thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Đóng form hóa đơn lại
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thanh toán: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
