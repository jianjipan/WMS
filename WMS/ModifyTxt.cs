using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WMS
{
    class ModifyTxt
    {
        public static int printCount = 1;    //控制命令的打印次数
        public static void PrintTxt(byte[] sendbyte)
        {                       
            File.AppendAllText(@"C:\Users\jjp-god\Desktop\send.txt",string.Concat(sendbyte.Select(b=>b.ToString("X02")+" ").ToArray()));
            File.AppendAllText(@"C:\Users\jjp-god\Desktop\send.txt", "\r\n");          
        }
        public static void PrintReceiveTxt(byte[] receivebyte)
        {
            File.AppendAllText(@"C:\Users\jjp-god\Desktop\receive.txt", string.Concat(receivebyte.Select(b => b.ToString("X02") + " ").ToArray()));
            File.AppendAllText(@"C:\Users\jjp-god\Desktop\receive.txt", "\r\n"); 
        }
        public static void PrintlogTxt(string log)
        {
            if (MainWindow.PrintLog)
            {
                string[] srr = System.IO.File.ReadAllLines(@"C:\Users\LJD\Desktop\log.txt");
                if (srr.Length != 0)
                {
                    if (log != srr[srr.Length - 1])
                    {
                        File.AppendAllText(@"C:\Users\LJD\Desktop\log.txt", log);
                        File.AppendAllText(@"C:\Users\LJD\Desktop\log.txt", "\r\n");
                    }
                }
                else
                {
                    File.AppendAllText(@"C:\Users\LJD\Desktop\log.txt", log);
                    File.AppendAllText(@"C:\Users\LJD\Desktop\log.txt", "\r\n");
                }
            }
        }
        
    }
}
