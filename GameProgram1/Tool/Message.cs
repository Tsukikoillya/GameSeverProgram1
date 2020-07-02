using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol1;
using Google.Protobuf;

namespace GameProgram1
{
    class Message
    {
        private byte[] buffer = new byte[1024];
        private int startindex;
        public byte[]Buffer
        {
            get
            {
                return buffer;
            }
        }
        public int StartIndex
        {
            get
            {
                return startindex;
            }
        }
        public int Remsize
        {
            get
            {
                return buffer.Length - startindex;
            }
        }
        public void ReadBuffer(int len,Action<MainPack> HandleRequest)//len为接受的数据长度
        {
            //解析消息
            //消息buffer=1*包头（int 4个字节）+n*包体
            startindex += len;
            if (startindex <= 4)//包头int四个字节，小于等于4肯定是不完整的
            {
                return;
            }
            //count计算的包头包含的包体的长度
            int count = BitConverter.ToInt32(buffer, 0);//返回转换的字节数组指定位置处四个字节（32位）有符号整数
            while(true)
            {
                if (startindex >= (count + 4))//当前长度大于包头+包体
                {
                    MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer,4,count);//处理包体
                    HandleRequest(pack);
                    Array.Copy(buffer, count + 4, buffer, 0, startindex - count - 4);//把后面的消息覆盖到前面
                    startindex -= (count + 4);//剩余长度计算
                }
                else
                {
                    break;
                }
            }
        }
        public static byte[] PackData(MainPack pack)//数据封装
        {
            byte[] data = pack.ToByteArray();//包体
            byte[] head = BitConverter.GetBytes(data.Length);//包头
            return head.Concat(data).ToArray();
        }
    }
}
