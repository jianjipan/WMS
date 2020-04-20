using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS
{
    class DataCheck
    {
        private static readonly object obj = new object();
        public static bool CheckData(byte[] Rec_Data)    //校验数据
        {
            lock (obj)
            {
                bool betrue = false;
                byte a = 0, b = 0;
                for (int i = 0; i < 19; i++)
                {
                    a ^= Rec_Data[i];
                }
                if (a == Rec_Data[19])
                    betrue = true;
                if (betrue)
                {
                    betrue = false;
                    for (int j = 0; j < Rec_Data[9]; j++)
                    {
                        b ^= Rec_Data[20 + j];
                    }
                    if (b == Rec_Data[14])
                    {
                        betrue = true;                      
                    }
                }
                return betrue;
            }
        }
        public static bool CheckDataE1(byte[] Rec_Data)
        {
            lock (obj)
            {
                bool betrue = false;
                byte b=0;
                for (int j = 0; j < 8; j++)
                {
                    b += Rec_Data[j];
                    if (b > 255)
                        b -= 255;
                }
                if (b == Rec_Data[8])
                {
                    betrue = true;
                }
                return betrue;

            }
        }
        public static bool CheckDataE2(byte[] Rec_Data)
        {
            lock (obj)
            {
                bool betrue = false;
                byte b = 0;
                for (int j = 0; j < 12; j++)
                {
                    b += Rec_Data[j];
                    if (b > 255)
                        b -= 255;
                }
                if (b == Rec_Data[12])
                {
                    betrue = true;
                }
                return betrue;

            }
        }
        public static bool CheckDataE3(byte[] Rec_Data)
        {
            lock (obj)
            {
                bool betrue = false;
                byte b = 0;
                for (int j = 0; j < 2; j++)
                {
                    b += Rec_Data[j];
                    if (b > 255)
                        b -= 255;
                }
                if (b == Rec_Data[2])
                {
                    betrue = true;
                }
                return betrue;

            }
        }
    }
}
