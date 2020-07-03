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
    class UserData
    {
        private MySqlConnection mysqlCon;
        ////IP地址、端口、用户、密码、数据库名称
        //private static string host = "localhost";
        //private static string port = "3306";
        //private static string userName = "root";
        //private static string psd = "123456";
        //private static string database = "game1";
        public UserData()
        {
            //ConnectMysql();
            mysqlCon = DatabaseConncetion.ConnectMysql();
        }
        //private void ConnectMysql()//连接数据库
        //{
        //    try
        //    {
        //        string mySqlString = string.Format("database={0};data source={1};user id={2};password={3};port={4};CharSet=utf8;",
        //       database, host, userName, psd, port);
        //        mysqlCon = new MySqlConnection(mySqlString);
        //        mysqlCon.Open();
        //        Console.WriteLine("数据库连接成功！");
        //    }
        //    catch(Exception e){
        //        Console.WriteLine("连接数据库失败！");
        //    }
        //}
        public MainPack Logon(MainPack pack)//注册
        {
            string username = pack.Loginpack.Username;
            string password = pack.Loginpack.Password;
            string email = pack.Loginpack.Email;
            //检查邮箱有没有重复
            string sqlCheck = string.Format("select * from game1.userdata where email ='{0}'; ", email);
            MySqlCommand comd = new MySqlCommand(sqlCheck,mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            if (read.Read() == true)
            {
                Console.WriteLine("邮箱已经被使用了");
                read.Close();
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
            read.Close();
            //插入数据
            string sqlInsert = string.Format("insert into userdata(email,username,password,diamonds,support_role_id,ban_flag,new_checkpoint) values('{0}','{1}','{2}',3000,1,false,1);",email,username,password);
            Console.WriteLine(sqlInsert);
            comd = new MySqlCommand(sqlInsert, mysqlCon);
            try
            {
                comd.ExecuteNonQuery();
                Console.WriteLine("插入成功！");
                pack.Returncode = ReturnCode.Succeed;
                return pack;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
        }
        public MainPack Login(MainPack pack)
        {
            string password = pack.Loginpack.Password;
            string email = pack.Loginpack.Email;
            //检查邮箱是否存在
            string sqlCheck = string.Format("select * from userdata where email='{0}';", email);
            MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            if (read.Read() == false)
            {
                Console.WriteLine("邮箱不存在");
                read.Close();
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
            if (read["password"].ToString() != password)
            {
                Console.WriteLine("密码不一致");
                read.Close();
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
            pack.Loginpack.Uid = read["uid"].ToString();
            pack.Loginpack.Password = read["password"].ToString();
            pack.Loginpack.Username = read["username"].ToString();
            pack.Loginpack.Diamonds= read["diamonds"].ToString();
            pack.Returncode = ReturnCode.Succeed;
            read.Close();
            return pack;
        }
        public bool ChangePassword(MainPack pack)
        {
            string password = pack.Loginpack.Password;
            string email = pack.Loginpack.Email;
            string sql = string.Format("update userdata set password='{0}' where email='{1}';",password,email);
            MySqlCommand comd = new MySqlCommand(sql, mysqlCon);
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
        public bool SetUserMaxCpId(MainPack pack)
        {
            string uid = pack.Loginpack.Uid;
            string maxId = pack.Loginpack.Maxcpid;
            string sql = string.Format("update userdata set new_checkpoint='{0}' where uid='{1}';", maxId, uid);
            MySqlCommand comd = new MySqlCommand(sql, mysqlCon);
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
