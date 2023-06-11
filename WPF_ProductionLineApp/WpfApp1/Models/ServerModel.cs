using CommunityToolkit.Mvvm.ComponentModel;
using HslCommunication.Profinet.Siemens;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace WpfProductionLineApp.Models
{
    public partial class ServerModel : ObservableObject
    {
        /*--------------TCP服务端----------------------*/
        [ObservableProperty]
        private Socket _serverSocket;//服务端用于监听的socket

        [ObservableProperty]
        private ObservableCollection<Socket> _socketLists;//已连接的客户端socket列表

        [ObservableProperty]
        private Dictionary<Socket, byte[]> _clientBufferDic;//存储客户端发送的数据

        [ObservableProperty]
        private string _ipAddress;//服务端ip地址

        [ObservableProperty]
        private int _port;//服务端端口号

        [ObservableProperty]
        private ConnectState _tcpServerState;

        /*-------------------------机械臂----------------------*/
        [ObservableProperty]
        private Dictionary<string, Socket> _socketDic;//串口号+机械臂

        /*------------------------plc服务端------------------------*/
        [ObservableProperty]
        private SiemensS7Net _plcServer;//plc服务端

        [ObservableProperty]
        private string _pLCServerConnectText;

        [ObservableProperty]
        private ConnectState _pLCServerState;

        /*------------------------WebSocket服务端------------------------*/
        [ObservableProperty]
        private ConnectState _webSocketState;

    }
}
