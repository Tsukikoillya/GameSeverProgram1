using GameProgram1.Tool;
using MySql.Data.MySqlClient;
using SocketGameProtocol1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgram1.DAO
{
    class ItemData
    {
        private MySqlConnection mysqlCon;
        ////IP地址、端口、用户、密码、数据库名称
        //private static string host = "localhost";
        //private static string port = "3306";
        //private static string userName = "root";
        //private static string password = "123456";
        //private static string database = "game1";



        public ItemData()
        {
            //ConnectMysql();
            mysqlCon = DatabaseConncetion.ConnectMysql();
        }
        private void ConnectMysql()//连接数据库
        {
            //try
            //{
            //    string mySqlString = string.Format("database={0};data source={1};user id={2};password={3};port={4};CharSet=utf8;",
            //   database, host, userName, password, port);
            //    mysqlCon = new MySqlConnection(mySqlString);
            //    mysqlCon.Open();
            //    Console.WriteLine("数据库连接成功！");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("连接数据库失败！");
            //}
        }

        public MainPack GetItemData(MainPack pack)
        {
            try
            {
                string userName = pack.Loginpack.Username;
                List<string> IIDs = new List<string>();
                List<int> count = new List<int>();
                Console.WriteLine(userName);
                string sqlCheck = string.Format("select * from uitem where UID={0}", userName);
                MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
                MySqlDataReader read = comd.ExecuteReader();
                while (read.Read())
                {
                    IIDs.Add(read[1].ToString());
                    Console.WriteLine(read[1].ToString());

                    count.Add(Convert.ToInt32(read[2]));
                    pack.Returncode = ReturnCode.Succeed;
                }
                read.Close();


                
                //List<Character> characters = new List<Character>();
                pack.Characterdata.Clear();
                for (int i = 0; i < IIDs.Count; i++)
                {
                    string sqlReadC = string.Format("select * from item where IID = {0}", IIDs[i]);
                    Console.WriteLine(sqlReadC);
                    comd = new MySqlCommand(sqlReadC, mysqlCon);
                    Console.WriteLine("reading");
                    read = comd.ExecuteReader();
                    Console.WriteLine("reading");
                    read.Read();
                    SocketGameProtocol1.ItemData data = new SocketGameProtocol1.ItemData();
                    data.IID = Convert.ToInt32(read["IID"]);
                    data.Type = Convert.ToInt32(read["type"]);
                    data.Value = Convert.ToInt32(read["value"]);
                    data.Detail = read["detail"].ToString();
                    data.Count = count[i];
                    data.Name = read["name"].ToString();
                    pack.Itemdata.Add(data);
                    read.Close();
                }
                return pack;
            }
            catch
            {
                Console.WriteLine("读取物品数据失败");
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }

        }
    }
}
