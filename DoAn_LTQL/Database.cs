using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAn_LTQL
{
    internal class Database
    {
        public SqlConnection cn;
        SqlDataAdapter da;

        // Hàm khởi tạo: truyền vào tên Server và tên Database
        public Database(string srvname, string dbName)
        {
            string cnnstr = "Data source=" + srvname + "; Initial Catalog =" + dbName + "; Integrated Security = True";
            cn = new SqlConnection(cnnstr);
        }

        // Hàm lấy dữ liệu (Dùng cho lệnh SELECT) trả về DataTable
        public DataTable laydl(string sqlstr)
        {
            da = new SqlDataAdapter(sqlstr, cn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // Hàm thực thi lệnh (Dùng cho lệnh INSERT, UPDATE, DELETE)
        public void thucthi(SqlCommand sqlcmd)
        {
            cn.Open();
            sqlcmd.ExecuteNonQuery();
            cn.Close();
        }
    }
}
