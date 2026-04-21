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
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        Database db;

        // trả dữ liệu về frmMain
        public string Username { get; set; }
        public int VaiTro { get; set; }


        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string tk = txtDangNhap.Text.Trim();
            string mk = txtMatKhau.Text.Trim();

            if (string.IsNullOrEmpty(tk) || string.IsNullOrEmpty(mk))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                txtDangNhap.Focus();
                return;
            }

            try
            {
                string sql = "SELECT TenNV, VaiTro FROM NHAN_VIEN WHERE TaiKhoan=@tk AND MatKhau=@mk";

                SqlCommand cmd = new SqlCommand(sql, db.cn);
                cmd.Parameters.AddWithValue("@tk", tk);
                cmd.Parameters.AddWithValue("@mk", mk);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    this.Username = dt.Rows[0]["TenNV"].ToString();
                    this.VaiTro = Convert.ToInt32(dt.Rows[0]["VaiTro"]);

                    MessageBox.Show("Đăng nhập thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    txtDangNhap.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối database: " + ex.Message);
            }
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {
            db = new Database(@"DESKTOP-MA5J22U\SQLSERVER2022", "QLSB");
            txtMatKhau.PasswordChar = '*';
        }

        private void frmDangNhap_Shown(object sender, EventArgs e)
        {
            txtDangNhap.Focus();
        }
    }
}
