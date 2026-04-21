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
    public partial class frmSanBong : Form
    {
        Database db;
        int id = -1;
        public frmSanBong()
        {
            InitializeComponent();
        }

        private void BatTatChucNang(bool val)
        {
            // Vì không có nút Thêm, ta chỉ quản lý nút Sửa và Xóa
            btnSua.Enabled = val;
            btnXoa.Enabled = val;
            btnThoat.Enabled = val;
            btnTimKiem.Enabled = val;
            btnNhap.Enabled = val;
            btnXuat.Enabled = val;

            btnLuu.Enabled = !val;
            btnHuyBo.Enabled = !val;

            // Ô Tên sân luôn mở để có thể gõ tìm kiếm
            txtTenSan.Enabled = true;

            txtGiaThue.Enabled = !val;
            cboLoaiSan.Enabled = !val;
            cboTrangThai.Enabled = !val;
        }

        private void LoadData()
        {
            BatTatChucNang(true);
            id = -1; // Reset lại ID mỗi khi load

            // Khởi tạo dữ liệu cho ComboBox nếu chưa có
            if (cboLoaiSan.Items.Count == 0)
            {
                cboLoaiSan.Items.Add("Sân 5");
                cboLoaiSan.Items.Add("Sân 7");
            }
            if (cboTrangThai.Items.Count == 0)
            {
                cboTrangThai.Items.Add("Bình thường");
                cboTrangThai.Items.Add("Đang bảo trì");
            }

            // Lấy dữ liệu từ bảng SAN_BONG
            string sql = "SELECT MaSan AS ID, TenSan AS N'Tên sân', LoaiSan AS N'Loại sân', GiaThueGio AS N'Giá thuê', TrangThai AS N'Trạng thái' FROM SAN_BONG";
            DataTable dt = db.laydl(sql);
            dgvDSSan.DataSource = dt;
        }

        private void frmSanBong_Load(object sender, EventArgs e)
        {
            db = new Database(@".\SQLEXPRESS", "QLSB");
            LoadData();
        }

        private void dgvDSSan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDSSan.Rows[e.RowIndex];
                id = Convert.ToInt32(row.Cells["ID"].Value);

                txtTenSan.Text = row.Cells["Tên sân"].Value.ToString();
                // Ép kiểu Decimal rồi ToString để bỏ bớt phần thập phân dư thừa (,0000)
                txtGiaThue.Text = Convert.ToDecimal(row.Cells["Giá thuê"].Value).ToString("0");
                cboLoaiSan.Text = row.Cells["Loại sân"].Value.ToString();
                cboTrangThai.Text = row.Cells["Trạng thái"].Value.ToString();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (id == -1)
            {
                MessageBox.Show("Vui lòng chọn một sân bóng trên danh sách để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            BatTatChucNang(false);
            txtTenSan.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (id == -1)
            {
                MessageBox.Show("Vui lòng chọn sân bóng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa sân bóng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string sql = "DELETE FROM SAN_BONG WHERE MaSan = @MaSan";
                SqlCommand cmd = new SqlCommand(sql, db.cn);
                cmd.Parameters.AddWithValue("@MaSan", id);

                try
                {
                    db.thucthi(cmd);
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xóa sân này vì đã có dữ liệu Đặt Sân liên quan!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenSan.Text) || string.IsNullOrWhiteSpace(txtGiaThue.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên sân và Giá thuê!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra giá thuê có phải là số không
            decimal giaThue;
            if (!decimal.TryParse(txtGiaThue.Text, out giaThue))
            {
                MessageBox.Show("Giá thuê phải là một con số hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sql = "UPDATE SAN_BONG SET TenSan = @TenSan, LoaiSan = @LoaiSan, GiaThueGio = @GiaThue, TrangThai = @TrangThai WHERE MaSan = @MaSan";

            SqlCommand cmd = new SqlCommand(sql, db.cn);
            cmd.Parameters.AddWithValue("@TenSan", txtTenSan.Text);
            cmd.Parameters.AddWithValue("@LoaiSan", cboLoaiSan.Text);
            cmd.Parameters.AddWithValue("@GiaThue", giaThue);
            cmd.Parameters.AddWithValue("@TrangThai", cboTrangThai.Text);
            cmd.Parameters.AddWithValue("@MaSan", id);

            try
            {
                db.thucthi(cmd);
                MessageBox.Show("Cập nhật thông tin sân thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
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
            string tuKhoa = txtTenSan.Text.Trim();
            string sql = $"SELECT MaSan AS ID, TenSan AS N'Tên sân', LoaiSan AS N'Loại sân', GiaThueGio AS N'Giá thuê', TrangThai AS N'Trạng thái' FROM SAN_BONG WHERE TenSan LIKE N'%{tuKhoa}%'";

            DataTable dt = db.laydl(sql);
            dgvDSSan.DataSource = dt;
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

                    // Chèn dữ liệu sân bóng mới từ Excel
                    foreach (DataRow r in dt.Rows)
                    {
                        string tenSan = r["Tên sân"].ToString();
                        string loaiSan = r["Loại sân"].ToString();
                        string giaThueStr = r["Giá thuê"].ToString();
                        string trangThai = r["Trạng thái"].ToString();

                        if (!string.IsNullOrWhiteSpace(tenSan))
                        {
                            decimal giaThue = 0;
                            decimal.TryParse(giaThueStr, out giaThue);

                            string sql = "INSERT INTO SAN_BONG (TenSan, LoaiSan, GiaThueGio, TrangThai) VALUES (@TenSan, @LoaiSan, @GiaThue, @TrangThai)";
                            SqlCommand cmd = new SqlCommand(sql, db.cn);
                            cmd.Parameters.AddWithValue("@TenSan", tenSan);
                            cmd.Parameters.AddWithValue("@LoaiSan", loaiSan);
                            cmd.Parameters.AddWithValue("@GiaThue", giaThue);
                            cmd.Parameters.AddWithValue("@TrangThai", string.IsNullOrWhiteSpace(trangThai) ? "Bình thường" : trangThai);

                            db.thucthi(cmd);
                        }
                    }

                    MessageBox.Show("Đã import dữ liệu Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
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
            sfd.FileName = "DanhSachSanBong_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable dt = (DataTable)dgvDSSan.DataSource;
                    XLWorkbook wb = new XLWorkbook();
                    var sheet = wb.Worksheets.Add(dt, "SanBong");
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
