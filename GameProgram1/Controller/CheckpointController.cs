using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol1;

namespace GameProgram1.Controller
{
    class CheckpointController : BaseController
    {
        public CheckpointController()
        {
            requestCode = RequestCode.Checkpoint;
        }
        /// <summary>
        /// 获取关卡表
        /// </summary>
        /// <returns></returns>
        public MainPack GetCheckpointList(Server server, Client client, MainPack pack)
        {
            return client.GetCheckpointList(pack);
        }
    }
}
