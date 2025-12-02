using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace DAL
{
    public class DatabaseConnect
    {
        private static string connectionString = @"Server=C3DACC548CE34C4\SQLEXPRESS;Database=QLKhachSan1234;Trusted_Connection=True;";
        private static SqlConnection conn;

        public static SqlConnection Conn
        {
            get
            {
                if (conn == null)
                {
                    conn = new SqlConnection(connectionString);
                }
                return conn;
            }
        }

        public static string OpenDatabase()
        {
            try
            {
                if (Conn.State == ConnectionState.Closed)
                {
                    Conn.Open();
                    return "Ket Noi Thanh Cong";
                }
                return "Ket Noi Da Duoc Mo";
            }
            catch (Exception ex)
            {
                return "Ket Noi That Bai: " + ex.Message;
            }
        }

        public static string CloseDatabase()
        {
            try
            {
                if (Conn.State == ConnectionState.Open)
                {
                    Conn.Close();
                    return "Dong Thanh Cong";
                }
                return "Ket Noi Da Duoc Dong";
            }
            catch (Exception ex)
            {
                return "Dong That Bai: " + ex.Message;
            }
        }
    }

    public class OtherSystem
    {
        public static string Createcode(string table)
        {
            DateTime now = DateTime.Now;
            string formattedTime = now.ToString("yyMMddHHmmss");
            return table + formattedTime + Randomcode();
        }

        public static string GetDate()
        {
            DateTime now = DateTime.Now;
            string formattedDate = now.ToString("yyyy-MM-dd");
            return formattedDate;
        }

        private static char Randomcode()
        {
            Random random = new Random();
            char randomChar;

            while (true)
            {
                int randomNumber = random.Next(49, 123);
                randomChar = (char)randomNumber;

                if ((randomChar >= '1' && randomChar <= '9') || (randomChar >= 'a' && randomChar <= 'z'))
                {
                    break;
                }
            }
            return randomChar;
        }
    }
}