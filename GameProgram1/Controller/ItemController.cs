using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol1;

namespace GameProgram1.Controller
{
    class ItemController:BaseController
    {
        public ItemController()
        {
            requestCode = RequestCode.Item;
        }

        public MainPack GetItemData(Server server, Client client, MainPack pack)
        {
            return client.SetFriendship(pack);
        }
    }
}
