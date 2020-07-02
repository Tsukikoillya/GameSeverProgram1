using MySql.Data.MySqlClient;
using SocketGameProtocol1;
using GameProgram1.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProgram1.DAO
{
    class CharacterData
    {
        private MySqlConnection mysqlCon;
        ////IP地址、端口、用户、密码、数据库名称
        //private static string host = "localhost";
        //private static string port = "3306";
        //private static string userName = "root";
        //private static string password = "123456";
        //private static string database = "game1";
        //角色的cid循环周期，也就是最高等级
        private int maxLevel = 1;
        //随机数种子
        Random random = new Random();
        

        public CharacterData()
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

        public MainPack GetCharacterData(MainPack pack)
        {
            try
            {
                string userName = pack.Loginpack.Username;
                List<string> CIDs = new List<string>();
                List<int> exps = new List<int>();
                Console.WriteLine(userName);
                string sqlCheck = string.Format("select * from ucpackage where UID={0}",userName);
                MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
                MySqlDataReader read = comd.ExecuteReader();
                while (read.Read())
                {
                    CIDs.Add(read[1].ToString());
                    Console.WriteLine(read[1].ToString());

                    exps.Add(Convert.ToInt32(read[2]));
                    pack.Returncode = ReturnCode.Succeed;
                }
                read.Close();
                

                //读取角色数据
                //List<Character> characters = new List<Character>();
                pack.Characterdata.Clear();
                for (int i = 0; i < CIDs.Count; i++)
                {
                    string sqlReadC = string.Format("select * from role where CID = {0}", CIDs[i]);
                    Console.WriteLine(sqlReadC);
                    comd = new MySqlCommand(sqlReadC, mysqlCon);
                    Console.WriteLine("reading");
                    read = comd.ExecuteReader();
                    Console.WriteLine("reading");
                    read.Read();
                    SocketGameProtocol1.CharacterData data = new SocketGameProtocol1.CharacterData();
                    data.CID = Convert.ToInt32(read["CID"]);
                    data.Level = Convert.ToInt32(read["level"]);
                    data.Exp = Convert.ToInt32(read["exp"]);
                    data.Cost = Convert.ToInt32(read["cost"]);
                    data.Life = Convert.ToInt32(read["life"]);
                    data.Name = read["name"].ToString();
                    data.Type = Convert.ToInt32(read["type"]);
                    data.AttackPow = Convert.ToInt32(read["attack_pow"]);
                    data.Defend = Convert.ToInt32(read["defend"]);
                    data.AttackArea = Convert.ToSingle(read["attack_area"]);
                    data.AttackInterval = Convert.ToSingle(read["attack_interval"]);
                    data.RoleExp = exps[i];
                    //Character character = new Character(data);
                    pack.Characterdata.Add(data);
                    read.Close();
                }
                pack.Returncode = ReturnCode.Succeed;
                return pack;
            }
            catch
            {
                Console.WriteLine("读取角色数据失败");
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }

        }
        public MainPack GetCard(MainPack pack)
        {
            try
            {
                string uid = pack.Loginpack.Uid;
                Console.WriteLine("玩家id" + uid);
                int count = 0, needDiamonds = 0;
                if (pack.Loginpack.Cardnum == "1")
                {
                    count = 1;
                    needDiamonds = 100;
                }
                else
                {
                    count = 10;
                    needDiamonds = 900;
                }
                int newDiamonds = IsEnoughDiamonds(needDiamonds, uid);
                if (newDiamonds == -1)
                {
                    pack.Returncode = ReturnCode.Fail;
                    return pack;
                }
                Console.WriteLine("抽卡数额"+count);
                pack.Characterdata.Clear();
                for (int i = 0; i < count; i++)
                {
                    int num = RandomCard();
                    Console.WriteLine("随机抽卡id"+num);
                    InsertCard(uid, num.ToString());
                    string sqlReadC = string.Format("select * from role where CID = {0}", num);
                    MySqlCommand comd = new MySqlCommand(sqlReadC, mysqlCon);
                    MySqlDataReader read = comd.ExecuteReader();
                    read.Read();
                    SocketGameProtocol1.CharacterData data = new SocketGameProtocol1.CharacterData();
                    data.Name = read["name"].ToString();
                    data.Type = Convert.ToInt32(read["type"]);
                    pack.Characterdata.Add(data);
                    read.Close();
                }
                pack.Loginpack.Diamonds = newDiamonds.ToString();
                pack.Returncode = ReturnCode.Succeed;
                return pack;
            }
            catch
            {
                Console.WriteLine("抽卡数据失败");
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
        }
        private int RandomCard()
        {
            int count = MaxCountRole();
            int num = random.Next(1,count+1);
            num = num / (maxLevel + 1) + 1;
            num *= maxLevel;
            return num;
        }
        private int MaxCountRole()
        {
            string sqlReadMin = string.Format("select max(cid) from ucpackage");
            MySqlCommand comd = new MySqlCommand(sqlReadMin, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            try
            {
                read.Read();
                int maxCid = (int)read[0];
                read.Close();
                return maxCid;
            }
            catch (Exception e)
            {
                read.Close();
                return 0;
            }
        }
        private bool InsertCard(string uid,string cid)
        {
            //插入数据
            //string sqlInsert = string.Format("insert into frienddata(user,fid,type) values('{0}','{1}','{2}');", uid1, uid2, state);
            string sqlInsert = string.Format("insert into ucpackage(uid,cid,exp) values('{0}','{1}','0');", uid,cid);
            MySqlCommand comd = new MySqlCommand(sqlInsert, mysqlCon);
            try
            {
                comd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("插入卡牌数据失败"+e);
                return false;
            }
        }
        private int IsEnoughDiamonds(int needDiamonds,string uid)
        {
            string sqlCheck = string.Format("select diamonds from game1.userdata where uid ='{0}'; ", uid);
            MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            try
            {
                int nowDiamonds=0;
                if (read.Read() == true)
                {
                    nowDiamonds= int.Parse(read["diamonds"].ToString());
                }
                read.Close();
                if (needDiamonds <= nowDiamonds)
                {
                    if(SetDiamonds(nowDiamonds - needDiamonds, uid))
                    {
                        return nowDiamonds - needDiamonds;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }
        private bool SetDiamonds(int newDiamonds,string uid)
        {
            string sql = string.Format("update userdata set diamonds='{0}' where uid='{1}';", newDiamonds, uid);
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
