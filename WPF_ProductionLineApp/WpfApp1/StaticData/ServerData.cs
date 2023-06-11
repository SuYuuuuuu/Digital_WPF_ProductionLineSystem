using System.Net.Sockets;

namespace WpfProductionLineApp.StaticData
{
    struct ReceiveState//用于传递信息，包括连接的socket对象和字节数组
    {
        public Socket cilentSocket;
        public byte[] buffer;
    }

}
