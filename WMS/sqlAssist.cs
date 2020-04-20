using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Data;
using System.Data.SqlClient;

namespace WMS
{
    class sqlAssist
    {
        //////获取数据表中的命令\
        public static SqlConnection con;
        public static string connectionString = "Data Source=.;Initial Catalog=test3;Integrated Security=true";
            
        public static void AcquireSqlcmd(string table,int n)
        {
            con = new SqlConnection(connectionString);
            con.Open();
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT OperationCmd,TargetMachine FROM " + table + " WHERE OperationId='" + n + "'", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    
                }
            }
        }
    }
}
