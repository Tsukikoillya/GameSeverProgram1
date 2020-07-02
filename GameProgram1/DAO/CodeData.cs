using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProgram1.Tool;
using MySql.Data.MySqlClient;
using SocketGameProtocol1;

namespace GameProgram1.DAO
{
    class CodeData
    {
        private MySqlConnection mysqlCon;
        ////IP地址、端口、用户、密码、数据库名称
        //private static string host = "localhost";
        //private static string port = "3306";
        //private static string userName = "root";
        //private static string password = "123456";
        //private static string database = "game1";
        public CodeData()
        {
            //ConnectMysql();
            mysqlCon = DatabaseConncetion.ConnectMysql();
        }
        //private void ConnectMysql()//连接数据库
        //{
        //    try
        //    {
        //        string mySqlString = string.Format("database={0};data source={1};user id={2};password={3};port={4};CharSet=utf8;",
        //       database, host, userName, password, port);
        //        mysqlCon = new MySqlConnection(mySqlString);
        //        mysqlCon.Open();
        //        Console.WriteLine("数据库连接成功！");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("连接数据库失败！");
        //    }
        //}
        public bool CheckCode(MainPack pack)
        {
            string email = pack.Loginpack.Email;
            string code = pack.Loginpack.Code;
            //检查邮箱是否存在
            string sqlCheck = string.Format("select code from codedata where email='{0}';", email);
            MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            if (read.Read() == false)
            {
                Console.WriteLine("邮箱不存在");
                read.Close();
                return false;
            }
            if (read.GetString(0) != code)
            {
                Console.WriteLine("验证码不一致");
                read.Close();
                return false;
            }
            read.Close();
            return true;
        }
        public bool NewCode(string email,string code)//更新新的code
        {
            //检查邮箱是否存在
            string sqlCheck = string.Format("select * from codedata where email='{0}';", email);
            MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            if (read.Read() == false)
            {
                Console.WriteLine("邮箱不存在");
                read.Close();
                string sqlInsert = string.Format("insert into codedata values('{0}','{1}');", email,code);
                comd = new MySqlCommand(sqlInsert, mysqlCon);
                try
                {
                    comd.ExecuteNonQuery();
                    Console.WriteLine("插入成功！");
                    read.Close();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    read.Close();
                    return false;
                }
            }
            read.Close();
            string sql = string.Format("update codedata set code='{0}' where email='{1}';", code, email);
            comd = new MySqlCommand(sql, mysqlCon);
            try
            {
                comd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
