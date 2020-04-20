using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace WMS
{
    class DB
    {
       
        public static string connectionString="Data Source=.;Initial Catalog=test2;Integrated Security=true";
        public static SqlConnection con = new SqlConnection(connectionString);
        //将用户选择的库位插入到数据库中，供立库软件使用
        public static void InsertKuwei(string Part1,string Part2,string Part3,string Part4,string Assembly)   
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("UPDATE ZPkuwei SET kuweiPostion='" + Part1 + "',kuweiOutlib='undo',kuweiInlib='undo' WHERE kuweiStyle=1", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("UPDATE ZPkuwei SET kuweiPostion='" + Part2 + "',kuweiOutlib='undo',kuweiInlib='undo' WHERE kuweiStyle=2", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("UPDATE ZPkuwei SET kuweiPostion='" + Part3 + "',kuweiOutlib='undo',kuweiInlib='undo' WHERE kuweiStyle=3", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("UPDATE ZPkuwei SET kuweiPostion='" + Part4 + "',kuweiOutlib='undo',kuweiInlib='undo' WHERE kuweiStyle=4", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("UPDATE ZPkuwei SET kuweiPostion='" + Assembly + "',kuweiOutlib='undo',kuweiInlib='undo' WHERE kuweiStyle=5", con))
            {
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
        public static void InsertKuwei1(string Part1, string Part2, string Part3, string Part4, string Assembly)
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("UPDATE CJkuwei SET kuweiPostion='" + Part1 + "',kuweiOutlib='undo',kuweiInlib='undo' WHERE kuweiStyle=1", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("UPDATE CJkuwei SET kuweiPostion='" + Part2 + "',kuweiOutlib='undo',kuweiInlib='undo' WHERE kuweiStyle=2", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("UPDATE CJkuwei SET kuweiPostion='" + Part3 + "',kuweiOutlib='undo',kuweiInlib='undo' WHERE kuweiStyle=3", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("UPDATE CJkuwei SET kuweiPostion='" + Part4 + "',kuweiOutlib='undo',kuweiInlib='undo' WHERE kuweiStyle=4", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("UPDATE CJkuwei SET kuweiPostion='" + Assembly + "',kuweiOutlib='undo',kuweiInlib='undo' WHERE kuweiStyle=5", con))
            {
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        public static void ResetDB_cmd()      //清理数据库指令表中【正在执行....】
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("DELETE FROM ComponentInlibcmd WHERE cmdState='正在执行....'", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("DELETE FROM ComponentOutlibcmd WHERE cmdState='正在执行....'", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("DELETE FROM AssemblyInlibcmd WHERE cmdState='正在执行....'", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("DELETE FROM AssemblyOutlibcmd WHERE cmdState='正在执行....'", con))
            {
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
        public static void ResetDB_Stock()    //更新数据库指令表中的库位
        {
            con.Open();
            if (MainWindow.ZPMode == true)
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE ComponentStock SET proNumber= '2'", con))
                {
                    cmd.ExecuteNonQuery();
                }
                using (SqlCommand cmd = new SqlCommand("UPDATE AssemblyStock SET proNumber='0'", con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE ComponentStock SET proNumber= '1'", con))
                {
                    cmd.ExecuteNonQuery();
                }
                using (SqlCommand cmd = new SqlCommand("UPDATE AssemblyStock SET proNumber='1'", con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            con.Close();
        }
        public static void ResetDB_kuwei()   //更新库位表中的出入库状态
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("UPDATE ZPkuwei SET kuweiOutlib='undo', kuweiInlib='undo'", con))
            {
                cmd.ExecuteNonQuery();
            }
            using (SqlCommand cmd = new SqlCommand("UPDATE CJkuwei SET kuweiOutlib='undo',kuweiInlib='undo'", con))
            {
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
    }
}
