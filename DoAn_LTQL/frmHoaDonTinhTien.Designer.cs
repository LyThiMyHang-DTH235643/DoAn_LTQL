namespace DoAn_LTQL
{
    partial class frmHoaDonTinhTien
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblHoaDonTinhTien = new System.Windows.Forms.Label();
            this.btnXacNhanThanhToan = new System.Windows.Forms.Button();
            this.btnHuyBo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblHoaDonTinhTien
            // 
            this.lblHoaDonTinhTien.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.lblHoaDonTinhTien.Location = new System.Drawing.Point(13, 9);
            this.lblHoaDonTinhTien.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHoaDonTinhTien.Name = "lblHoaDonTinhTien";
            this.lblHoaDonTinhTien.Size = new System.Drawing.Size(373, 457);
            this.lblHoaDonTinhTien.TabIndex = 0;
            this.lblHoaDonTinhTien.Text = " ";
            // 
            // btnXacNhanThanhToan
            // 
            this.btnXacNhanThanhToan.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnXacNhanThanhToan.Location = new System.Drawing.Point(147, 495);
            this.btnXacNhanThanhToan.Name = "btnXacNhanThanhToan";
            this.btnXacNhanThanhToan.Size = new System.Drawing.Size(241, 48);
            this.btnXacNhanThanhToan.TabIndex = 1;
            this.btnXacNhanThanhToan.Text = "Xác nhận thanh toán";
            this.btnXacNhanThanhToan.UseVisualStyleBackColor = true;
            this.btnXacNhanThanhToan.Click += new System.EventHandler(this.btnXacNhanThanhToan_Click);
            // 
            // btnHuyBo
            // 
            this.btnHuyBo.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnHuyBo.Location = new System.Drawing.Point(17, 495);
            this.btnHuyBo.Name = "btnHuyBo";
            this.btnHuyBo.Size = new System.Drawing.Size(118, 48);
            this.btnHuyBo.TabIndex = 1;
            this.btnHuyBo.Text = "Hủy bỏ";
            this.btnHuyBo.UseVisualStyleBackColor = true;
            this.btnHuyBo.Click += new System.EventHandler(this.btnHuyBo_Click);
            // 
            // frmHoaDonTinhTien
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 568);
            this.Controls.Add(this.btnHuyBo);
            this.Controls.Add(this.btnXacNhanThanhToan);
            this.Controls.Add(this.lblHoaDonTinhTien);
            this.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmHoaDonTinhTien";
            this.Text = "HÓA ĐƠN TÍNH TIỀN";
            this.Load += new System.EventHandler(this.frmHoaDonTinhTien_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblHoaDonTinhTien;
        private System.Windows.Forms.Button btnXacNhanThanhToan;
        private System.Windows.Forms.Button btnHuyBo;
    }
}