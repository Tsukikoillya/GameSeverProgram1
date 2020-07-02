using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol1;
using System.Reflection;

namespace GameProgram1.Controller
{
    class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> controlDic = new Dictionary<RequestCode, BaseController>();
        private Server server;
        public ControllerManager(Server server)
        {
            this.server = server;
            UserController userController = new UserController();
            BackageController backageController = new BackageController();
            ItemController itemController = new ItemController();
            FriendController friendController = new FriendController();
            CheckpointController checkpointController = new CheckpointController();
            controlDic.Add(userController.GetRequestCode,userController);
            controlDic.Add(backageController.GetRequestCode,backageController);
            controlDic.Add(itemController.GetRequestCode, itemController);
            controlDic.Add(friendController.GetRequestCode,friendController);
            controlDic.Add(checkpointController.GetRequestCode, checkpointController);
        }
        public void HandleRequest(MainPack pack,Client client)
        {
            Console.WriteLine(pack.Requestcode);
            //找出对应的处理方法
            if(controlDic.TryGetValue(pack.Requestcode, out BaseController controller))
            {
                string methodName = pack.Actioncode.ToString();//得到名字
                MethodInfo method = controller.GetType().GetMethod(methodName);//方法
                if (method == null)
                {
                    Console.WriteLine("没有找到对应的处理方法！");
                    return;
                }
                object[] obj = new object[] { server,client,pack};
                Object ret = method.Invoke(controller, obj);//发射调用方法
                Console.WriteLine("返回"+ret);
                if (ret != null)
                {
                    client.Send(ret as MainPack);
                }
            }
            else
            {
                Console.WriteLine("没有找到对应controller的处理方法！");
            }
        }
    }
}
