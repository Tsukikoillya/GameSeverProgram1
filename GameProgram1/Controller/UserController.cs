using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProgram1.DAO;
using SocketGameProtocol1;

namespace GameProgram1.Controller
{
    class UserController:BaseController
    {
        private UserData userData;
        public UserController()
        {
            requestCode = RequestCode.User;
            userData = new UserData();
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public MainPack Logon(Server server,Client client,MainPack pack)
        {
            return client.Logon(pack);
        }
        /// <summary>
        /// 登入
        /// </summary>
        /// <returns></returns>
        public MainPack Login(Server server, Client client, MainPack pack)
        {
            Console.WriteLine("登入操作");
            return client.Login(pack);
        }
        ///<summary>
        ///修改密码
        /// </summary>
        public MainPack ChangePassword(Server server, Client client, MainPack pack)
        {
            if (client.ChangePassword(pack))
            {
                pack.Returncode = ReturnCode.Succeed;
            }
            else
            {
                pack.Returncode = ReturnCode.Fail;
            }
            return pack;
        }
        ///<summary>
        ///发送验证码
        /// </summary>
        public MainPack Code(Server server, Client client, MainPack pack)
        {
            if (client.Code(pack))
            {
                pack.Returncode = ReturnCode.Succeed;
            }
            else
            {
                pack.Returncode = ReturnCode.Fail;
            }
            return pack;
        }
        public MainPack SetUserMaxCpId(Server server, Client client, MainPack pack)
        {
            if (client.SetUserMaxCpId(pack))
            {
                pack.Returncode = ReturnCode.Succeed;
            }
            else
            {
                pack.Returncode = ReturnCode.Fail;
            }
            return pack;
        }
    }
}
