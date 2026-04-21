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
using System.IO;

namespace DoAn_LTQL
{
    public partial class frmDatSan : Form
    {
        public frmDatSan()
        {
            InitializeComponent();
        }
        Database db;

        // Class phụ để lưu trữ thông tin của sân vào thuộc tính Tag của Label
        class ThongTinSan
        {
            public int MaSan { get; set; }
            public int MaDatSan { get; set; }
            public string TrangThai { get; set; }
            public string TrangThaiBaoTri { get; set; }
        }

        private void frmDatSan_Load(object sender, EventArgs e)
        {
            // 1. Khởi tạo kết nối DB 
            // (Nhớ đổi "TEN_MAY_TINH\\SQLEXPRESS" thành tên Server SQL của bạn)
            db = new Database(@"(localdb)\MSSQLLocalDB", "QLSB");

            // 2. Mặc định chọn ngày hiện tại cho DateTimePicker
            dtpChonNgay.Value = DateTime.Now;

            // 3. Load danh sách khung giờ vào ComboBox (08:00 đến 21:00)
            cboThoiGian.Items.Clear();
            for (int i = 8; i < 21; i++)
            {
                string gioBatDau = i.ToString("00") + ":00";
                string gioKetThuc = (i + 1).ToString("00") + ":00";
                cboThoiGian.Items.Add($"{gioBatDau} - {gioKetThuc}");
            }
            // Chọn mặc định mục đầu tiên là "08:00 - 09:00"
            cboThoiGian.SelectedIndex = 0;

            // 4. Khởi tạo Tag cho các Label sân (Gắn cứng Mã Sân từ 1 đến 5 theo CSDL)
            lblSanA.Tag = new ThongTinSan { MaSan = 1, TrangThai = "Trống" };
            lblSanB.Tag = new ThongTinSan { MaSan = 2, TrangThai = "Trống" };
            lblSanC.Tag = new ThongTinSan { MaSan = 3, TrangThai = "Trống" };
            lblSanD.Tag = new ThongTinSan { MaSan = 4, TrangThai = "Trống" };
            lblSanE.Tag = new ThongTinSan { MaSan = 5, TrangThai = "Trống" };

            // 5. TỰ ĐỘNG TẢI DỮ LIỆU NGAY KHI MỞ FORM
            // Không cần gọi XoaRongSan nữa vì hàm LoadTrangThaiSan sẽ tự xử lý việc này
            LoadTrangThaiSan();
        }

        private DateTime LayThoiGianBatDau()
        {
            string gioBatDauStr = cboThoiGian.Text.Substring(0, 5); // Cắt lấy "08:00" từ chuỗi "08:00 - 09:00"
            TimeSpan thoiGian = TimeSpan.Parse(gioBatDauStr);
            return dtpChonNgay.Value.Date + thoiGian; // Kết hợp ngày và giờ
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadTrangThaiSan();
        }

        private void LoadTrangThaiSan()
        {
            DateTime thoiGianBatDau = LayThoiGianBatDau();
            string thoiGianSQL = thoiGianBatDau.ToString("yyyy-MM-dd HH:mm:00");

            string sql = $@"
            SELECT d.MaDatSan, s.MaSan, s.TenSan, s.LoaiSan, s.TrangThai AS TrangThaiBaoTri, k.TenKH, k.SoDienThoai, d.TrangThaiDat
            FROM SAN_BONG s
            LEFT JOIN DAT_SAN d ON s.MaSan = d.MaSan AND d.ThoiGianBatDau = '{thoiGianSQL}'
            LEFT JOIN KHACH_HANG k ON d.MaKH = k.MaKH";

            DataTable dt = db.laydl(sql);

            // Truyền thẳng Label vào hàm, thống nhất Sân A đến Sân E
            CapNhatUI_San(lblSanA, 1, "SÂN A", "Sân 5 người", dt);
            CapNhatUI_San(lblSanB, 2, "SÂN B", "Sân 5 người", dt);
            CapNhatUI_San(lblSanC, 3, "SÂN C", "Sân 7 người", dt);
            CapNhatUI_San(lblSanD, 4, "SÂN D", "Sân 7 người", dt);
            CapNhatUI_San(lblSanE, 5, "SÂN E", "Sân 5 người", dt);
        }

        private void CapNhatUI_San(Label lbl, int maSan, string tenSan, string loaiSan, DataTable dtBookings)
        {
            ThongTinSan info = (ThongTinSan)lbl.Tag;
            DataRow[] rows = dtBookings.Select($"MaSan = {maSan}");

            string ngayChon = dtpChonNgay.Value.ToString("dd/MM/yyyy");
            string gioChon = cboThoiGian.Text.Replace(" ", "");

            string khoangTrongChoAnh = "\n\n\n\n\n\n\n";
            string phanDauText = $"{khoangTrongChoAnh}               {tenSan}\n           ({loaiSan})\n ----------------------------\n";

            // Lưu trạng thái bảo trì vào info
            info.TrangThaiBaoTri = rows.Length > 0 ? rows[0]["TrangThaiBaoTri"].ToString() : "Bình thường";

            // NẾU SÂN ĐANG BẢO TRÌ
            if (info.TrangThaiBaoTri == "Đang bảo trì")
            {
                info.MaDatSan = 0;
                info.TrangThai = "Trống";

                lbl.Text = phanDauText + "\n\n      (ĐANG BẢO TRÌ)";
                lbl.BackColor = Color.Salmon; // Màu đỏ nhạt cảnh báo
            }
            else if (rows.Length > 0 && rows[0]["MaDatSan"] != DBNull.Value)
            {
                // CÓ NGƯỜI ĐẶT
                DataRow r = rows[0];
                info.MaDatSan = Convert.ToInt32(r["MaDatSan"]);
                info.TrangThai = r["TrangThaiDat"].ToString();

                lbl.Text = phanDauText +
                           $" Mã đặt: {info.MaDatSan}\n" +
                           $" Ngày: {ngayChon}\n" +
                           $" Giờ: {gioChon}\n" +
                           $" Tên: {r["TenKH"]}\n" +
                           $" SĐT: {r["SoDienThoai"]}\n" +
                           $" Trạng thái: {info.TrangThai}";

                if (info.TrangThai == "Đã đặt") lbl.BackColor = Color.Gold;
                else if (info.TrangThai == "Đang sử dụng") lbl.BackColor = Color.LimeGreen;
                else if (info.TrangThai == "Hoàn thành") lbl.BackColor = Color.DeepSkyBlue;
            }
            else
            {
                // SÂN TRỐNG
                info.MaDatSan = 0;
                info.TrangThai = "Trống";

                lbl.Text = phanDauText +
                           $" Mã đặt: ---\n" +
                           $" Ngày: {ngayChon}\n" +
                           $" Giờ: {gioChon}\n" +
                           $" Tên: ---\n" +
                           $" SĐT: ---\n" +
                           $" Trạng thái: Trống";

                lbl.BackColor = Color.LightGray;
            }
        }

        private void XoaRongSan(Label lbl, string tenSan)
        {
            lbl.Text = $"{tenSan}\n\n(Sân đang trống)";
            lbl.BackColor = Color.LightGray; // Màu xám
        }

        // --- XỬ LÝ SỰ KIỆN CLICK VÀO SÂN ---
        // (Nhớ gán sự kiện này cho cả 5 Label ở màn hình Design)

        private void lblSan_Click(object sender, EventArgs e)
        {
            Label lbl = sender as Label;
            ThongTinSan info = lbl.Tag as ThongTinSan;
            DateTime thoiGianChon = LayThoiGianBatDau();

            // CHẶN ĐẶT SÂN NẾU ĐANG BẢO TRÌ
            if (info.TrangThaiBaoTri == "Đang bảo trì")
            {
                MessageBox.Show("Sân bóng này đang được bảo trì, không thể đặt lúc này!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Kết thúc luôn, không chạy code phía dưới
            }

            // 1. TRƯỜNG HỢP: SÂN TRỐNG -> ĐẶT SÂN
            if (info.TrangThai == "Trống")
            {
                if (thoiGianChon < DateTime.Now)
                {
                    MessageBox.Show("Khung giờ này đã qua, không thể đặt sân!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Sân đang trống, bạn có muốn đặt không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Tạo một mảng chứa tên sân (Vị trí 1 tương ứng SÂN A, 2 tương ứng SÂN B...)
                    string[] danhSachTenSan = { "", "SÂN A", "SÂN B", "SÂN C", "SÂN D", "SÂN E" };

                    // Lấy tên sân ra dựa vào Mã Sân
                    string tenSan = danhSachTenSan[info.MaSan];

                    // Mở Form Đặt sân và truyền Mã Sân, Tên Sân, Thời gian bắt đầu sang
                    frmThongTinDatSan frm = new frmThongTinDatSan(info.MaSan, tenSan, thoiGianChon);
                    frm.ShowDialog();
                    LoadTrangThaiSan();
                }
            }
            // 2. TRƯỜNG HỢP: ĐÃ ĐẶT -> CHUYỂN SANG ĐANG SỬ DỤNG
            else if (info.TrangThai == "Đã đặt")
            {
                if (MessageBox.Show("Bạn có muốn giao sân không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sql = $"UPDATE DAT_SAN SET TrangThaiDat = N'Đang sử dụng' WHERE MaDatSan = {info.MaDatSan}";
                    SqlCommand cmd = new SqlCommand(sql, db.cn);
                    db.thucthi(cmd);
                    LoadTrangThaiSan();
                }
            }
            // 3. TRƯỜNG HỢP: ĐANG SỬ DỤNG -> THANH TOÁN (HOÀN THÀNH)
            else if (info.TrangThai == "Đang sử dụng")
            {
                if (MessageBox.Show("Xác nhận thanh toán và trả sân?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Gọi form Hóa Đơn và truyền Mã Đặt Sân sang
                    frmHoaDonTinhTien frm = new frmHoaDonTinhTien(info.MaDatSan);
                    frm.ShowDialog();

                    // Sau khi tắt form Hóa đơn, load lại để sân chuyển sang màu xanh biển
                    LoadTrangThaiSan();
                }
            }
            // 4. TRƯỜNG HỢP: HOÀN THÀNH
            else if (info.TrangThai == "Hoàn thành")
            {
                MessageBox.Show("Khung giờ này đã thanh toán xong!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lblSanA_Click(object sender, EventArgs e)
        {
            lblSan_Click(sender, e);
        }

        private void lblSanB_Click(object sender, EventArgs e)
        {
            lblSan_Click(sender, e);
        }

        private void lblSanC_Click(object sender, EventArgs e)
        {
            lblSan_Click(sender, e);
        }

        private void lblSanD_Click(object sender, EventArgs e)
        {
            lblSan_Click(sender, e);
        }

        private void lblSanE_Click(object sender, EventArgs e)
        {
            lblSan_Click(sender, e);
        }

        private void btnTraCuu_Click(object sender, EventArgs e)
        {
            string input = txtTraCuu.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Vui lòng nhập Mã đặt sân để tra cứu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra xem người dùng có nhập đúng định dạng số không
            int maDatSan;
            if (!int.TryParse(input, out maDatSan))
            {
                MessageBox.Show("Mã đặt sân phải là một con số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. Truy vấn xem mã đặt này thuộc Sân nào và Khung giờ nào
            string sql = $"SELECT MaSan, ThoiGianBatDau FROM DAT_SAN WHERE MaDatSan = {maDatSan}";
            DataTable dt = db.laydl(sql);

            if (dt.Rows.Count > 0)
            {
                int maSan = Convert.ToInt32(dt.Rows[0]["MaSan"]);
                DateTime thoiGianBatDau = Convert.ToDateTime(dt.Rows[0]["ThoiGianBatDau"]);

                // 2. Tự động đổi ngày trên giao diện về đúng ngày của mã đặt
                dtpChonNgay.Value = thoiGianBatDau.Date;

                // 3. Tự động đổi giờ trên ComboBox
                // Vì giờ của bạn bắt đầu từ 8h (nghĩa là Index 0 -> 8h, Index 1 -> 9h,...)
                int gioBatDau = thoiGianBatDau.Hour;
                if (gioBatDau >= 8 && gioBatDau <= 20)
                {
                    cboThoiGian.SelectedIndex = gioBatDau - 8;
                }

                // 4. Gọi hàm load dữ liệu để cập nhật thông tin sân theo ngày giờ vừa đổi
                LoadTrangThaiSan();

                // 5. Ẩn các sân không liên quan, chỉ hiện sân có mã đặt vừa tìm
                lblSanA.Visible = (maSan == 1);
                lblSanB.Visible = (maSan == 2);
                lblSanC.Visible = (maSan == 3);
                lblSanD.Visible = (maSan == 4);
                lblSanE.Visible = (maSan == 5);
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin cho mã đặt sân này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            // 1. Xóa chữ trong ô nhập
            txtTraCuu.Clear();

            // 2. Hiện lại toàn bộ 5 sân
            lblSanA.Visible = true;
            lblSanB.Visible = true;
            lblSanC.Visible = true;
            lblSanD.Visible = true;
            lblSanE.Visible = true;

            // 3. Load lại trạng thái để đảm bảo dữ liệu hiển thị đúng
            LoadTrangThaiSan();
        }
    }
}
