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
    class CheckpointData
    {
        private MySqlConnection mysqlCon;
        ////IP地址、端口、用户、密码、数据库名称
        //private static string host = "localhost";
        //private static string port = "3306";
        //private static string userName = "root";
        //private static string password = "123456";
        //private static string database = "game1";
        //关卡起始id
        private int startId = 1;
        public CheckpointData()
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
        public MainPack GetCheckpointList(MainPack pack)
        {
            string uid = pack.Loginpack.Uid;
            Console.WriteLine("checkpoint" + 1);
            int max_user_cp = GetUserMaxCheckpointId(uid);
            if (max_user_cp == -1)
            {
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
            int max_cp = GetMaxCheckpointId();
            Console.WriteLine("checkpoint" + 2);
            if (max_cp == -1)
            {
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
            Console.WriteLine("checkpoint" + 2);
            List<string> names = GetCheckpointName();
            if (names == null)
            {
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
            Console.WriteLine("checkpoint" + 3);
            for (int i = 0; i < names.Count; i++)
            {
                SocketGameProtocol1.CheckpointData data = new SocketGameProtocol1.CheckpointData();
                data.Id = i + 1;
                data.Name = names[i];
                if (i + 1 < max_user_cp)
                {
                    data.State = CheckpointCode.Pass;
                }
                else if (i + 1 == max_user_cp)
                {
                    data.State = CheckpointCode.UnlockFail;
                }
                else
                {
                    data.State = CheckpointCode.Lock;
                }
                pack.Checkpointdata.Add(data);
            }
            pack.Returncode = ReturnCode.Succeed;
            return pack;
        }
        public List<string> GetCheckpointName()
        {
            List<string> names = new List<string>();
            string sqlCheck = string.Format("select * from checkpoint;");//id和类型相同
            MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            try
            {
                while (read.Read())
                {
                    names.Add(read[0].ToString());
                }
                read.Close();
                return names;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        private int GetUserMaxCheckpointId(string uid)
        {
            //检查邮箱是否存在
            string sqlCheck = string.Format("select * from userdata where uid='{0}';", uid);
            MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            try
            {
                read.Read();
                int num = int.Parse(read["new_checkpoint"].ToString());
                read.Close();
                return num;
            }
            catch (Exception e)
            {
                read.Close();
                Console.WriteLine(e.Message);
                return -1;
            }
        }
        private int GetMaxCheckpointId()
        {
            //得到最大id
            string sqlReadMax = string.Format("select max(gid) from checkpoint");
            MySqlCommand comd = new MySqlCommand(sqlReadMax, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            try
            {
                read.Read();
                int maxId = (int)read[0];
                read.Close();
                return maxId;
            }
            catch (Exception e)
            {
                read.Close();
                Console.WriteLine(e.Message);
                return -1;
            }
        }
    }
}
