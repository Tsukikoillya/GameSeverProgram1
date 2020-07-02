using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol1;


namespace GameProgram1
{
    abstract class BaseController
    {
        protected RequestCode requestCode=RequestCode.RequestNone;//只有子类才能访问
        public RequestCode GetRequestCode
        {
            get { return requestCode; }
        }
    }
}
