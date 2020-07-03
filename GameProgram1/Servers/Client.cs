using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GameProgram1.DAO;
using SocketGameProtocol1;
using GameProgram1.Tool;
using GameProgram1.Controller;

namespace GameProgram1
{
    class Client
    {
        private Socket socket;
        private Message message;
        private EmailCode emailCode;
        //private UserController userController;
        private UserData userData;
        private CodeData codeData;
        private DAO.CharacterData roleData;
        private DAO.ItemData itemData;
        private DAO.FriendData friendData;
        private DAO.CheckpointData checkpointData;
        private Server server;

        public UserData GetUserData
        {
            get{ return userData; }
        }
        public CodeData GetCodeData
        {
            get { return codeData; }
        }
        public DAO.CharacterData GetRoleData
        {
            get { return roleData; }
        }
        public DAO.ItemData GetItem
        {
            get { return itemData; }
        }
        public DAO.FriendData GetFriend
        {
            get { return friendData; }
        }
        public DAO.CheckpointData GetCheckpointData
        {
            get { return checkpointData; }
        }
        public Client(Socket socket,Server server)
        {
            message = new Message();
            userData = new UserData();
            emailCode = new EmailCode();
            //userController = new UserController();
            codeData = new CodeData();
            roleData = new DAO.CharacterData();
            itemData = new DAO.ItemData();
            friendData = new DAO.FriendData();
            checkpointData = new DAO.CheckpointData();
            

            this.socket = socket;
            this.server = server;

            StartReceive();
        }
        void StartReceive()
        {
            socket.BeginReceive(message.Buffer,message.StartIndex,message.Remsize,SocketFlags.None, ReceiveCallBack, null);
        }
        void ReceiveCallBack(IAsyncResult iar)
        {
            try//有时候客户端异常关闭程序
            {
                if (socket == null || socket.Connected == false)//socket是否为null，socket没有连接
                {
                    return;
                }
                int len = socket.EndReceive(iar);
                if (len == 0)
                {
                    return;
                }
                message.ReadBuffer(len,HandleRequest);
                StartReceive();
            }
            catch
            {

            }
            
        }
        public  void Send(MainPack pack)
        {
            socket.Send(Message.PackData(pack));
        }
        
        void HandleRequest(MainPack pack)
        {
            server.HandleRequest(pack,this);
        }
        public MainPack Logon(MainPack pack)
        {
            if (GetCodeData.CheckCode(pack))
            {
                return GetUserData.Logon(pack);
            }
            else
            {
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
        }
        public MainPack Login(MainPack pack)
        {
            return GetUserData.Login(pack);
        }
        public bool ChangePassword(MainPack pack)
        {
            if (GetCodeData.CheckCode(pack))
            {
                return GetUserData.ChangePassword(pack);
            }
            else
            {
                return false;
            }
            
        }
        public bool Code(MainPack pack)
        {
            Random r = new Random();
            string num = r.Next(100000, 999999).ToString();
            if(emailCode.Send(pack.Loginpack.Email, num))
            {
                return GetCodeData.NewCode(pack.Loginpack.Email, num);
            }
            else
            {
                return false;
            }

        }
        public MainPack GetCharacterData(MainPack pack)
        {
            return GetRoleData.GetCharacterData(pack);
        }
        public MainPack GetCard(MainPack pack)
        {
            return GetRoleData.GetCard(pack);
        }

        public MainPack GetItemData(MainPack pack)
        {
            return GetItem.GetItemData(pack);
        }
        public MainPack GetFriendData(MainPack pack)
        {
            return GetFriend.GetFriendata(pack);
        }
        public MainPack SetFriendship(MainPack pack)
        {
            return GetFriend.SetFriendship(pack);
        }

        public MainPack GetCheckpointList(MainPack pack)
        {
            return GetCheckpointData.GetCheckpointList(pack);
        }
        public bool SetUserMaxCpId(MainPack pack)
        {
            return GetUserData.SetUserMaxCpId(pack);
        }
    }
}
