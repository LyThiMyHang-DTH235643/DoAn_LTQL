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
    public partial class frmKhachHang : Form
    {
        Database db;
        bool xuLyThem = false; // Biến kiểm tra đang bấm Thêm hay Sửa
        int id;
        public frmKhachHang()
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

            // RIÊNG Ô HỌ TÊN LUÔN MỞ ĐỂ GÕ TÌM KIẾM
            txtHovaTen.Enabled = true;

            txtSoDienThoai.Enabled = !val;
            txtDiaChi.Enabled = !val;
        }

        // HÀM LOAD DỮ LIỆU LÊN DATAGRIDVIEW
        private void LoadData()
        {
            BatTatChucNang(true);

            // Lấy dữ liệu từ bảng KHACH_HANG
            string sql = "SELECT MaKH AS ID, TenKH AS N'Họ và tên', SoDienThoai AS N'Số điện thoại', DiaChi AS N'Địa chỉ' FROM KHACH_HANG";
            DataTable dt = db.laydl(sql);
            dgvDSKH.DataSource = dt;
        }

        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            db = new Database(@"DESKTOP-MA5J22U\SQLSERVER2022", "QLSB");
            LoadData();
        }

        private void dgvDSKH_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDSKH.Rows[e.RowIndex];
                id = Convert.ToInt32(row.Cells["ID"].Value);

                txtHovaTen.Text = row.Cells["Họ và tên"].Value.ToString();
                txtSoDienThoai.Text = row.Cells["Số điện thoại"].Value.ToString();
                txtDiaChi.Text = row.Cells["Địa chỉ"].Value.ToString();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            xuLyThem = true;
            BatTatChucNang(false);

            txtHovaTen.Clear();
            txtSoDienThoai.Clear();
            txtDiaChi.Clear();

            txtHovaTen.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            xuLyThem = false;
            BatTatChucNang(false);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string sql = "DELETE FROM KHACH_HANG WHERE MaKH = @MaKH";
                SqlCommand cmd = new SqlCommand(sql, db.cn);
                cmd.Parameters.AddWithValue("@MaKH", id);

                try
                {
                    db.thucthi(cmd);
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: Không thể xóa do khách hàng này đã có dữ liệu Hóa Đơn hoặc Đặt Sân!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(txtHovaTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên khách hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string sql = "";
            if (xuLyThem)
            {
                sql = "INSERT INTO KHACH_HANG (TenKH, SoDienThoai, DiaChi) VALUES (@TenKH, @SoDienThoai, @DiaChi)";
            }
            else
            {
                sql = "UPDATE KHACH_HANG SET TenKH = @TenKH, SoDienThoai = @SoDienThoai, DiaChi = @DiaChi WHERE MaKH = @MaKH";
            }

            SqlCommand cmd = new SqlCommand(sql, db.cn);
            cmd.Parameters.AddWithValue("@TenKH", txtHovaTen.Text);
            cmd.Parameters.AddWithValue("@SoDienThoai", txtSoDienThoai.Text);
            cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);

            if (!xuLyThem)
            {
                cmd.Parameters.AddWithValue("@MaKH", id);
            }

            try
            {
                db.thucthi(cmd);
                MessageBox.Show("Lưu thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            string tuKhoa = txtHovaTen.Text.Trim();
            string sql = $"SELECT MaKH AS ID, TenKH AS N'Họ và tên', SoDienThoai AS N'Số điện thoại', DiaChi AS N'Địa chỉ' FROM KHACH_HANG WHERE TenKH LIKE N'%{tuKhoa}%'";

            DataTable dt = db.laydl(sql);
            dgvDSKH.DataSource = dt;
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

                    // Chèn dữ liệu vào bảng KHACH_HANG
                    foreach (DataRow r in dt.Rows)
                    {
                        string tenKH = r["Họ và tên"].ToString();
                        string sdt = r["Số điện thoại"].ToString();
                        string diachi = r["Địa chỉ"].ToString();

                        if (!string.IsNullOrWhiteSpace(tenKH))
                        {
                            string sql = "INSERT INTO KHACH_HANG (TenKH, SoDienThoai, DiaChi) VALUES (@TenKH, @SDT, @DC)";
                            SqlCommand cmd = new SqlCommand(sql, db.cn);
                            cmd.Parameters.AddWithValue("@TenKH", tenKH);
                            cmd.Parameters.AddWithValue("@SDT", sdt);
                            cmd.Parameters.AddWithValue("@DC", diachi);

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
            sfd.FileName = "DanhSachKhachHang_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable dt = (DataTable)dgvDSKH.DataSource;
                    XLWorkbook wb = new XLWorkbook();
                    var sheet = wb.Worksheets.Add(dt, "KhachHang");
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
