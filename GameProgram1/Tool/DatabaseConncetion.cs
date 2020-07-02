using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgram1.Tool
{
    static class DatabaseConncetion
    {
        private static MySqlConnection mysqlCon;
        //IP地址、端口、用户、密码、数据库名称
        private static string host = "localhost";
        private static string port = "3306";
        private static string userName = "root";
        private static string psd = "123456";
        private static string database = "game1";

        //public DatabaseConncetion()
        //{
        //    ConnectMysql();
        //}
        public static MySqlConnection ConnectMysql()//连接数据库
        {
            if (mysqlCon != null) return mysqlCon;
            else
            {
                try
                {
                    string mySqlString = string.Format("database={0};data source={1};user id={2};password={3};port={4};CharSet=utf8;",
                                  database, host, userName, psd, port);
                    mysqlCon = new MySqlConnection(mySqlString);
                    mysqlCon.Open();
                    Console.WriteLine("数据库连接成功！");
                    return mysqlCon;
                }
                catch (Exception e)
                {
                    Console.WriteLine("连接数据库失败！");
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
        }
    }
}
