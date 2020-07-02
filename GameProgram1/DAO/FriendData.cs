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
    class FriendData
    {
        private MySqlConnection mysqlCon;
        ////IP地址、端口、用户、密码、数据库名称
        //private static string host = "localhost";
        //private static string port = "3306";
        //private static string userName = "root";
        //private static string password = "123456";
        //private static string database = "game1";

        public FriendData()
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
        int myFriend = 1;//id1和id2是朋友
        int noFriend = 2;//id1和id2不是朋友
        int applicationFriend = 3;//id2向id1申请好友
        private int TransformFriendType(FriendCode code)
        {
            switch (code)
            {
                case FriendCode.MyFriend:
                    return myFriend;
                    break;
                case FriendCode.AddFriend:
                    return noFriend;
                    break;
                case FriendCode.FriendApplication:
                    return applicationFriend;
                    break;
                default:
                    return 0;
                    break;
            }
        }
        public MainPack GetFriendata(MainPack pack)
        {
            try
            {
                //获取用户id
                string uid = pack.Loginpack.Uid;
                //转换好友Type
                int friendType = TransformFriendType(pack.Friendcode);
                //清空一下pack里面的好友内容
                pack.Characterdata.Clear();
                if (friendType==noFriend)
                {
                    //获取已经是好友的id列表
                    List<string> ids = new List<string>();
                    ids = GetListFriend(uid, myFriend);
                    //得到最小id
                    string sqlReadMin = string.Format("select min(uid) from userdata");
                    MySqlCommand comd = new MySqlCommand(sqlReadMin, mysqlCon);
                    MySqlDataReader read = comd.ExecuteReader();
                    read.Read();
                    long minId = (long)read[0];
                    read.Close();
                    //得到最大id
                    string sqlReadMax = string.Format("select max(uid) from userdata");
                    comd = new MySqlCommand(sqlReadMax, mysqlCon);
                    read = comd.ExecuteReader();
                    read.Read();
                    long maxId = (long)read[0];
                    read.Close();
                    //遍历好友列表的坐标
                    int j = 0, count = ids.Count;
                    Console.WriteLine(j + " " + count);
                    //遍历得到不是好友的列表
                    for (long i = minId; i <= maxId; i++)
                    {
                        //如果等于用户id
                        if (i.ToString() == uid)
                        {
                            //自己和自己是不能交朋友的
                            /*if (ids[j] == uid)
                            {
                                j++;
                            }*/
                            continue;
                        }
                        string checkId = i.ToString();
                        //已经当朋友的就不用管了
                        if(j<count&& i.ToString() == ids[j])
                        {
                            j++;
                            continue;
                        }
                        string sqlReadC = string.Format("select username from userdata where uid = '{0}'", checkId);
                        comd = new MySqlCommand(sqlReadC, mysqlCon);
                        read = comd.ExecuteReader();
                        read.Read();
                        SocketGameProtocol1.FriendData data = new SocketGameProtocol1.FriendData();
                        data.UID = checkId;
                        data.Kickname = read["username"].ToString();
                        pack.Frienddata.Add(data);
                        read.Close();
                    }
                    pack.Returncode = ReturnCode.Succeed;
                    return pack;
                }
                else
                {
                    List<string> ids = new List<string>();
                    ids=GetListFriend(uid, friendType);
                    for (int i = 0; i < ids.Count; i++)
                    {
                        string sqlReadC = string.Format("select username from userdata where uid = '{0}'", ids[i]);
                        MySqlCommand comd = new MySqlCommand(sqlReadC, mysqlCon);
                        MySqlDataReader read = comd.ExecuteReader();
                        read.Read();
                        SocketGameProtocol1.FriendData data = new SocketGameProtocol1.FriendData();
                        data.UID = ids[i];
                        data.Kickname = read["username"].ToString();
                        pack.Frienddata.Add(data);
                        read.Close();
                    }
                    pack.Returncode = ReturnCode.Succeed;
                    return pack;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }

        }
        //获取某个type的好友
        public List<string> GetListFriend(string uid, int state)
        {
            List<string> ids = new List<string>();
            string sqlCheck = string.Format("select * from frienddata where user='{0}'and type='{1}'", uid, state);//id和类型相同
            MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            try
            {
                while (read.Read())
                {
                    ids.Add(read[1].ToString());
                }
                read.Close();
                return ids;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public MainPack SetFriendship(MainPack pack)
        {
            string uid1 = pack.Loginpack.Uid;
            string uid2 = pack.Loginpack.Friendid;
            if (CheckUid(uid1) ==false||CheckUid(uid2)==false)
            {
                pack.Returncode = ReturnCode.Fail;
                Console.WriteLine(uid1+uid2+"用户不存在");
                return pack;
            }
            int friendType = TransformFriendType(pack.Friendcode);
            Console.WriteLine("Friendcode："+friendType);
            //删除好友关系
            if (friendType == myFriend)
            {
                if (ChekFriendShip(uid1, uid2) == myFriend)
                {
                    UpdateFriendShip(uid1, uid2, noFriend);
                    UpdateFriendShip(uid2, uid1, noFriend);
                    pack.Returncode = ReturnCode.Succeed;
                    return pack;
                }
                else
                {
                    pack.Returncode = ReturnCode.Fail;
                    return pack;
                }
            }
            //发送添加好友请求
            else if (friendType == noFriend)
            {
                //id1和id2不能已经是好友了
                if (ChekFriendShip(uid1, uid2) == myFriend)
                {
                    pack.Returncode = ReturnCode.Fail;
                    return pack;
                }
                //设置id2-id1关系为申请好友
                if(ChekFriendShip(uid2, uid1) == 0)
                {
                    InsertFriendShip(uid2, uid1, applicationFriend);
                }
                else
                {
                    UpdateFriendShip(uid2, uid1, applicationFriend);
                }
                pack.Returncode = ReturnCode.Succeed;
                return pack;
            }
            //接受好友申请
            else
            {
                //id1和id2不能已经是好友了
                if (ChekFriendShip(uid1, uid2) == myFriend)
                {
                    pack.Returncode = ReturnCode.Fail;
                    return pack;
                }
                //请求是否存在
                if(ChekFriendShip(uid1, uid2) != applicationFriend)
                {
                    pack.Returncode = ReturnCode.Fail;
                    return pack;
                }
                //更新id2-id1关系
                UpdateFriendShip(uid2, uid1, myFriend);
                //检查id1-id2关系存在性
                if (ChekFriendShip(uid1, uid2) == 0)
                {
                    InsertFriendShip(uid1, uid2, myFriend);
                }
                else
                {
                    UpdateFriendShip(uid1, uid2, myFriend);
                }
                pack.Returncode = ReturnCode.Succeed;
                return pack;
            }
        }
        //插入单向好友关系
        public bool InsertFriendShip(string uid1,string uid2,int state)
        {
            //插入数据
            string sqlInsert = string.Format("insert into frienddata(user,fid,type) values('{0}','{1}','{2}');", uid1, uid2, state);
            MySqlCommand comd = new MySqlCommand(sqlInsert, mysqlCon);
            try
            {
                comd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        //更新单向user fid好友关系
        public bool UpdateFriendShip(string uid1,string uid2,int state)
        {
            string sqlupdate = string.Format("update frienddata set type='{0}' where user='{1}'and fid='{2}';",state, uid1, uid2);
            MySqlCommand comd = new MySqlCommand(sqlupdate, mysqlCon);
            try
            {
                comd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        //单向检测user fid的好友关系
        public int ChekFriendShip(string uid1,string uid2)
        {
            string sqlCheck = string.Format("select * from frienddata where user='{0}'and fid='{1}'; ", uid1, uid2);
            MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            try
            {
                if (read.Read() == false)
                {
                    read.Close();
                    return 0;
                }
                else
                {
                    int num = (int)read["type"];
                    read.Close();
                    return num;
                }  
            }
            catch (Exception e)
            {
                read.Close();
                return 0;
            }
        }
        public bool CheckUid(string id)
        {
            string sqlCheck = string.Format("select * from game1.userdata where uid ='{0}'; ", id);
            MySqlCommand comd = new MySqlCommand(sqlCheck, mysqlCon);
            MySqlDataReader read = comd.ExecuteReader();
            if (read.Read() == true)
            {
                read.Close();
                return true;
            }
            read.Close();
            return false;
        }
    }
}
