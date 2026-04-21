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
using ClosedXML.Excel;

namespace DoAn_LTQL
{
    public partial class frmNhanVien : Form
    {
        Database db;
        bool xuLyThem = false; 
        int id;
        public frmNhanVien()
        {
            InitializeComponent();
        }
        private void BatTatChucNang(bool val)
        {
            btnThem.Enabled = val;
            btnSua.Enabled = val;
            btnXoa.Enabled = val;
            btnThoat.Enabled = val;
            btnTimKiem.Enabled = val;
            btnNhap.Enabled = val;
            btnXuat.Enabled = val;

            btnLuu.Enabled = !val;
            btnHuyBo.Enabled = !val;

            txtHovaTen.Enabled = true;
            txtSoDienThoai.Enabled = !val;
            txtDiaChi.Enabled = !val;
            txtTenDangNhap.Enabled = !val;
            txtMatKhau.Enabled = !val;
            cboQuyenHan.Enabled = !val;
        }

        // HÀM LOAD DỮ LIỆU LÊN DATAGRIDVIEW
        private void LoadData()
        {
            BatTatChucNang(true);

            if (cboQuyenHan.Items.Count == 0)
            {
                cboQuyenHan.Items.Add("Nhân viên");
                cboQuyenHan.Items.Add("Quản lý");
            }

            // Dùng CASE WHEN để biến 1 thành Quản lý, 0 thành Nhân viên
            string sql = @"SELECT MaNV AS ID, TenNV AS N'Họ và tên', SoDienThoai AS N'Điện thoại', 
                          DiaChi AS N'Địa chỉ', TaiKhoan AS N'Tên đăng nhập', MatKhau AS N'Mật khẩu', 
                          CASE WHEN VaiTro = 1 THEN N'Quản lý' ELSE N'Nhân viên' END AS N'Vai trò' 
                   FROM NHAN_VIEN";
            DataTable dt = db.laydl(sql);
            dgvDSNV.DataSource = dt;
            dgvDSNV.Columns["Mật khẩu"].Visible = false;
        }

        private void NhanVien_Load(object sender, EventArgs e)
        {
            db = new Database(@"DESKTOP-MA5J22U\SQLSERVER2022", "QLSB");
            txtMatKhau.PasswordChar = '*';
            LoadData();
        }

        private void dgvDSNV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDSNV.Rows[e.RowIndex];
                id = Convert.ToInt32(row.Cells["ID"].Value);

                txtHovaTen.Text = row.Cells["Họ và tên"].Value.ToString();
                txtSoDienThoai.Text = row.Cells["Điện thoại"].Value.ToString();
                txtDiaChi.Text = row.Cells["Địa chỉ"].Value.ToString();
                txtTenDangNhap.Text = row.Cells["Tên đăng nhập"].Value.ToString();
                txtMatKhau.Text = row.Cells["Mật khẩu"].Value.ToString();

                // So sánh chữ để chọn đúng ComboBox
                string vaiTroStr = row.Cells["Vai trò"].Value.ToString();
                if (vaiTroStr == "Quản lý")
                {
                    cboQuyenHan.SelectedIndex = 1;
                }
                else
                {
                    cboQuyenHan.SelectedIndex = 0;
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            xuLyThem = true;
            BatTatChucNang(false);

            // Xóa trắng các ô nhập liệu
            txtHovaTen.Clear();
            txtSoDienThoai.Clear();
            txtDiaChi.Clear();
            txtTenDangNhap.Clear();
            txtMatKhau.Clear();
            cboQuyenHan.SelectedIndex = 0; // Mặc định là Nhân viên

            txtHovaTen.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            xuLyThem = false;
            BatTatChucNang(false);
            txtTenDangNhap.Enabled = false;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string sql = "DELETE FROM NHAN_VIEN WHERE MaNV = @MaNV";
                SqlCommand cmd = new SqlCommand(sql, db.cn);
                cmd.Parameters.AddWithValue("@MaNV", id);

                try
                {
                    db.thucthi(cmd);
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: Không thể xóa do nhân viên này đã có dữ liệu Hóa Đơn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(txtHovaTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên nhân viên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sql = "";
            if (xuLyThem)
            {
                // Câu lệnh INSERT
                sql = "INSERT INTO NHAN_VIEN (TenNV, SoDienThoai, DiaChi, TaiKhoan, MatKhau, VaiTro) VALUES (@TenNV, @SoDienThoai, @DiaChi, @TaiKhoan, @MatKhau, @VaiTro)";
            }
            else
            {
                // Câu lệnh UPDATE
                sql = "UPDATE NHAN_VIEN SET TenNV = @TenNV, SoDienThoai = @SoDienThoai, DiaChi = @DiaChi, MatKhau = @MatKhau, VaiTro = @VaiTro WHERE MaNV = @MaNV";
            }

            SqlCommand cmd = new SqlCommand(sql, db.cn);
            cmd.Parameters.AddWithValue("@TenNV", txtHovaTen.Text);
            cmd.Parameters.AddWithValue("@SoDienThoai", txtSoDienThoai.Text);
            cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
            cmd.Parameters.AddWithValue("@MatKhau", txtMatKhau.Text);
            cmd.Parameters.AddWithValue("@VaiTro", cboQuyenHan.SelectedIndex); // 0 hoặc 1

            if (xuLyThem)
            {
                cmd.Parameters.AddWithValue("@TaiKhoan", txtTenDangNhap.Text);
            }
            else
            {
                cmd.Parameters.AddWithValue("@MaNV", id);
            }

            try
            {
                db.thucthi(cmd);
                MessageBox.Show("Lưu thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); // Tải lại form
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string tuKhoa = txtHovaTen.Text.Trim();

            // Câu lệnh SQL đã được thêm CASE WHEN giống hàm LoadData và giữ nguyên điều kiện tìm kiếm WHERE
            string sql = $@"SELECT MaNV AS ID, TenNV AS N'Họ và tên', SoDienThoai AS N'Điện thoại', 
                           DiaChi AS N'Địa chỉ', TaiKhoan AS N'Tên đăng nhập', MatKhau AS N'Mật khẩu', 
                           CASE WHEN VaiTro = 1 THEN N'Quản lý' ELSE N'Nhân viên' END AS N'Vai trò' 
                    FROM NHAN_VIEN 
                    WHERE TenNV LIKE N'%{tuKhoa}%'";

            DataTable dt = db.laydl(sql);
            dgvDSNV.DataSource = dt;
            dgvDSNV.Columns["Mật khẩu"].Visible = false;
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Nhập dữ liệu từ file Excel";
            ofd.Filter = "Tập tin Excel|*.xls;*.xlsx";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable dt = new DataTable();
                    XLWorkbook workbook = new XLWorkbook(ofd.FileName);
                    IXLWorksheet worksheet = workbook.Worksheet(1);
                    bool firstRow = true;

                    // Đọc dữ liệu từ file Excel vào DataTable tạm
                    foreach (IXLRow row in worksheet.RowsUsed())
                    {
                        if (firstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                                dt.Columns.Add(cell.Value.ToString());
                            firstRow = false;
                        }
                        else
                        {
                            dt.Rows.Add();
                            int i = 0;
                            foreach (IXLCell cell in row.Cells(1, dt.Columns.Count))
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                        }
                    }

                    // Chèn dữ liệu từ DataTable tạm vào SQL Server
                    foreach (DataRow r in dt.Rows)
                    {
                        string tenNV = r["Họ và tên"].ToString();
                        string sdt = r["Điện thoại"].ToString();
                        string diachi = r["Địa chỉ"].ToString();
                        string taikhoan = r["Tên đăng nhập"].ToString();
                        string matkhau = r["Mật khẩu"].ToString();
                        string vaitro = r["Vai trò"].ToString();

                        if (!string.IsNullOrWhiteSpace(tenNV) && !string.IsNullOrWhiteSpace(taikhoan))
                        {
                            string sql = "INSERT INTO NHAN_VIEN (TenNV, SoDienThoai, DiaChi, TaiKhoan, MatKhau, VaiTro) VALUES (@TenNV, @SDT, @DC, @TK, @MK, @VT)";
                            SqlCommand cmd = new SqlCommand(sql, db.cn);
                            cmd.Parameters.AddWithValue("@TenNV", tenNV);
                            cmd.Parameters.AddWithValue("@SDT", sdt);
                            cmd.Parameters.AddWithValue("@DC", diachi);
                            cmd.Parameters.AddWithValue("@TK", taikhoan);
                            cmd.Parameters.AddWithValue("@MK", matkhau);
                            cmd.Parameters.AddWithValue("@VT", vaitro);

                            db.thucthi(cmd);
                        }
                    }

                    MessageBox.Show("Đã import dữ liệu Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData(); // Load lại form sau khi import
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi nhập file Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Xuất dữ liệu ra file Excel";
            sfd.Filter = "Tập tin Excel |*.xlsx";
            sfd.FileName = "DanhSachNhanVien_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable dt = (DataTable)dgvDSNV.DataSource;
                    XLWorkbook wb = new XLWorkbook();
                    var sheet = wb.Worksheets.Add(dt, "NhanVien");
                    sheet.Columns().AdjustToContents();
                    wb.SaveAs(sfd.FileName);
                    MessageBox.Show("Đã xuất dữ liệu ra file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất file Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
