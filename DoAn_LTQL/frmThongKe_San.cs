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
    public partial class frmThongKe_San : Form
    {
        public frmThongKe_San()
        {
            InitializeComponent();
        }

        public void LoadReport(CrystalDecisions.CrystalReports.Engine.ReportDocument report)
        {
            crystalReportViewer1.ReportSource = report;
            crystalReportViewer1.RefreshReport();
        }
    }
}
