using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoAn_LTQL
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }



        private void frmMain_Load(object sender, EventArgs e)
        {
            ResetGiaoDien();
        }

        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {

        }

        private void mnuDangNhap_Click(object sender, EventArgs e)
        {
            this.Hide();

            frmDangNhap f = new frmDangNhap();

            if (f.ShowDialog() == DialogResult.OK)
            {
                toolStripStatusLabel1.Text = "Xin chào, " + f.Username;
            }
            PhanQuyen(f.VaiTro);

            this.Show();
        }
        private void PhanQuyen(int vaiTro)
        {
            if (vaiTro == 1)
            {
                mnuHeThong.Enabled = true;
                mnuQuanLy.Enabled = true;
                mnuBaoCaoThongKe.Enabled = true;
                mnuSanBong.Enabled = true;
                mnuDatSan.Enabled = true;
                mnuKhachHang.Enabled = true;
                mnuNhanVien.Enabled = true;
                mnuHoaDon.Enabled = true;
                mnuThongKeDoanhThu.Enabled = true;
                mnuHuongDanSuDung.Enabled = true;
                mnuHoTroDatSan.Enabled = true;
                mnuDangXuat.Enabled = true;
                mnuDangNhap.Enabled = false;
            }
            else
            {
                // NHÂN VIÊN: giới hạn quyền

                mnuHeThong.Enabled = true;
                mnuKhachHang.Enabled = true;
                mnuQuanLy.Enabled = true;
                mnuDatSan.Enabled = true;
                mnuDangXuat .Enabled = true;
                mnuHuongDanSuDung.Enabled = true;
                mnuTroGiup.Enabled = true;
                mnuHoTroDatSan.Enabled = true;
                mnuDangNhap.Enabled = false;
            }
        }
        private void mnuThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát chương trình không?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void ResetGiaoDien()
        {
            toolStripStatusLabel1.Text = "Chưa đăng nhập";
            toolStripStatusLabel2.Spring = true;
            toolStripStatusLabel3.Text = "© 2026 FIT";

            mnuSanBong.Enabled = false;
            mnuDatSan.Enabled = false;
            mnuKhachHang.Enabled = false;
            mnuNhanVien.Enabled = false;
            mnuHoaDon.Enabled = false;
            mnuDangXuat.Enabled = false;
            mnuThongKeDoanhThu.Enabled = false;
            mnuHuongDanSuDung.Enabled = false;
            mnuHoTroDatSan.Enabled = false;
        }
        private void mnuDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
        "Bạn có chắc chắn muốn đăng xuất không?",
        "Xác nhận",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    );

            if (result == DialogResult.Yes)
            {
                ResetGiaoDien();   // 👈 quay về trạng thái ban đầu

                MessageBox.Show("Đã đăng xuất!");
            }
        }

        private void mnuSanBong_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmSanBong f = new frmSanBong();
            f.ShowDialog();

        }

        private void mnuDatSan_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmDatSan f = new frmDatSan();
            f.ShowDialog();
        }

        private void mnuKhachHang_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmKhachHang f = new frmKhachHang();
            f.ShowDialog();
        }

        private void mnuNhanVien_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmNhanVien f = new frmNhanVien();
            f.ShowDialog();
        }

        private void mnuHoaDon_Click(object sender, EventArgs e)
        {
            
        }
    }
}
