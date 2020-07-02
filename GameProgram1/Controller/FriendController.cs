using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol1;

namespace GameProgram1.Controller
{
    class FriendController : BaseController
    {
        public FriendController()
        {
            requestCode = RequestCode.Friend;
        }
        /// <summary>
        /// 获取好友表
        /// </summary>
        /// <returns></returns>
        public MainPack GetFriendData(Server server, Client client, MainPack pack)
        {
            return client.GetFriendData(pack);
        }
        /// <summary>
        /// 设置好友关系
        /// </summary>
        /// <returns></returns>
        public MainPack SetFriendship(Server server, Client client, MainPack pack)
        {
            return client.SetFriendship(pack);
        }
    }
}
