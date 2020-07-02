using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol1;

namespace GameProgram1.Controller
{
    class BackageController:BaseController
    {
        public BackageController()
        {
            requestCode = RequestCode.Backpack;
        }

        public MainPack GetCharacterData(Server server, Client client, MainPack pack)
        {
            return client.GetCharacterData(pack);
        }
        public MainPack GetCard(Server server, Client client, MainPack pack)
        {
            return client.GetCard(pack);
        }
        public MainPack GetCpCharacterData(Server server, Client client, MainPack pack)
        {
            return client.GetCharacterData(pack);
        }
    }
}
