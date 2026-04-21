using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace DoAn_LTQL
{
    public partial class frmThongKeDoanhThu : Form
    {
        Database db;
        public frmThongKeDoanhThu()
        {
            InitializeComponent();
        }

        private void frmThongKeDoanhThu_Load(object sender, EventArgs e)
        {
            db = new Database(@".\SQLEXPRESS", "QLSB"); // Đổi tên server SQL của bạn cho đúng

            // 1. Mặc định ngày
            dtpTuNgay.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpDenNgay.Value = DateTime.Now;

            // 2. Tải danh sách sân vào ComboBox
            LoadComboBoxSan();
        }

        private void LoadComboBoxSan()
        {
            DataTable dtSan = db.laydl("SELECT MaSan, TenSan FROM SAN_BONG");

            DataRow row = dtSan.NewRow();
            row["MaSan"] = 0;
            row["TenSan"] = "Tất cả các sân";
            dtSan.Rows.InsertAt(row, 0);

            cboChonSan.DataSource = dtSan;
            cboChonSan.DisplayMember = "TenSan";
            cboChonSan.ValueMember = "MaSan";
            cboChonSan.SelectedIndex = 0;
        }

        // --- HÀM TẠO LỆNH SQL CHUNG ĐỂ LỌC THEO NGÀY VÀ SÂN ---
        private string TaoCauLenhSQL()
        {
            string tuNgay = dtpTuNgay.Value.ToString("yyyyMMdd 00:00:00");
            string denNgay = dtpDenNgay.Value.ToString("yyyyMMdd 23:59:59");

            // SỬA LỖI: Bỏ hết các chữ "as 'Nhân viên'", "as 'Sân'". Phải giữ đúng tên cột gốc.
            // THÊM: 1 AS SoGio để in ra báo cáo
            string sql = $@"
        SELECT h.MaHD, nv.TenNV, kh.TenKH, s.TenSan, h.NgayLapHD, 
               1 AS SoGio, 
               h.ThanhTien
        FROM HOA_DON h
        JOIN NHAN_VIEN nv ON h.MaNV = nv.MaNV
        JOIN KHACH_HANG kh ON h.MaKH = kh.MaKH
        JOIN DAT_SAN ds ON h.MaDatSan = ds.MaDatSan
        JOIN SAN_BONG s ON ds.MaSan = s.MaSan
        WHERE h.NgayLapHD >= '{tuNgay}' AND h.NgayLapHD <= '{denNgay}'";

            if (Convert.ToInt32(cboChonSan.SelectedValue) > 0)
            {
                sql += $" AND s.MaSan = {cboChonSan.SelectedValue}";
            }
            return sql;
        }

        private void dgvDSTK_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            try
            {
                // Đổ vào Lưới dgvDSTK
                string sql = TaoCauLenhSQL();
                DataTable dt = db.laydl(sql);
                dgvDSTK.DataSource = dt;

                // Đổ vào Biểu đồ chartBieuDo
                string tuNgay = dtpTuNgay.Value.ToString("yyyyMMdd 00:00:00");
                string denNgay = dtpDenNgay.Value.ToString("yyyyMMdd 23:59:59");
                string sqlChart = $@"
                    SELECT s.TenSan, SUM(h.ThanhTien) AS TongTien
                    FROM HOA_DON h
                    JOIN DAT_SAN ds ON h.MaDatSan = ds.MaDatSan
                    JOIN SAN_BONG s ON ds.MaSan = s.MaSan
                    WHERE h.NgayLapHD >= '{tuNgay}' AND h.NgayLapHD <= '{denNgay}'";

                if (Convert.ToInt32(cboChonSan.SelectedValue) > 0)
                    sqlChart += $" AND s.MaSan = {cboChonSan.SelectedValue}";

                sqlChart += " GROUP BY s.TenSan";

                DataTable dtChart = db.laydl(sqlChart);

                chartBieuDo.Series.Clear();
                chartBieuDo.Series.Add("DoanhThu");
                chartBieuDo.Series["DoanhThu"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
                chartBieuDo.Series["DoanhThu"].IsValueShownAsLabel = true;

                foreach (DataRow r in dtChart.Rows)
                {
                    chartBieuDo.Series["DoanhThu"].Points.AddXY(r["TenSan"].ToString(), Convert.ToDecimal(r["TongTien"]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            if (dgvDSTK.Rows.Count == 0 || dgvDSTK.DataSource == null) return;
            SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Files|*.xlsx", FileName = "DoanhThu.xlsx" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataTable dtExcel = (DataTable)dgvDSTK.DataSource;
                XLWorkbook wb = new XLWorkbook();
                wb.Worksheets.Add(dtExcel, "DoanhThu");
                wb.SaveAs(sfd.FileName);
                MessageBox.Show("Xuất Excel xong!");
            }
        }

        private void btnInBaoCao_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = TaoCauLenhSQL();
                DataTable dt = db.laydl(sql);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để in!");
                    return;
                }

                // Gọi tờ giấy rptDoanhThu
                rptDoanhThu_San cry = new rptDoanhThu_San();
                cry.SetDataSource(dt); // Bơm Data

                // Bơm 2 tham số ngày vào
                cry.SetParameterValue("pTuNgay", dtpTuNgay.Value.ToString("dd/MM/yyyy"));
                cry.SetParameterValue("pDenNgay", dtpDenNgay.Value.ToString("dd/MM/yyyy"));

                // Chiếu lên Form
                frmThongKe_San frm = new frmThongKe_San();
                frm.LoadReport(cry);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi in: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
