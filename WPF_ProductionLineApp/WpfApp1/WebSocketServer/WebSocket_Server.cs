using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Fleck;
using Microsoft.Extensions.DependencyInjection;
using WpfProductionLineApp.DobotCommunicationsFrameWork.Common;
using WpfProductionLineApp.Models;
using WpfProductionLineApp.ViewModels;

namespace WpfProductionLineApp.WebSocketServerModule
{
    public class WebSocket_Server
    {
        private List<IWebSocketConnection> allSockets;
        private WebSocketServer server;
        private bool isWebSocketServerClosed;//用于标识服务端关闭


        public void Open(string uri)
        {
            FleckLog.Level = LogLevel.Debug;
            allSockets = new List<IWebSocketConnection>();
            server = new WebSocketServer("ws://"+uri);
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    //Console.WriteLine("Open!");
                    MessageBox.Show("Open!");
                    allSockets.Add(socket);
                    StartToSendDobotNonRealTimeData(socket);
                    StartToSendDobotRealTimeData(socket);
                };
                socket.OnClose = () =>
                {
                    //Console.WriteLine("Close!");
                    MessageBox.Show("Close!");
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    //Console.WriteLine(message);
                    allSockets.ToList().ForEach(s => s.Send("Echo: " + message));
                };
            });
        }

        private void StartToSendDobotRealTimeData(IWebSocketConnection socket)
        {
            Task.Run(async () =>
            {
                DobotRealTimeData dobotData;
                RobotArmModel armModel = Locator.ServiceProvide.GetService<ConnectionViewModel>().robotModel;
                while (true)
                {
                    if (isWebSocketServerClosed) return;
                    //方案三：一个一个发送 + 数据打包发送
                    for (int i = 0; i < armModel.portLists.Count; i++)
                    {
                        dobotData = DataHelper.GetDobotRealTimeData(armModel.port2IndexDic[armModel.portLists[i]] - 1, armModel.portLists[i]);
                        Send(dobotData, socket);
                    }
                    await Task.Delay(50);//相当于发送时间(已经不等于发送时间了）
                }
            });
        }

        private void StartToSendDobotNonRealTimeData(IWebSocketConnection socket)
        {
            Task.Run(async () =>
            {
                DobotNonRealTimeData dobotData;
                RobotArmModel armModel = Locator.ServiceProvide.GetService<ConnectionViewModel>().robotModel;
                while (true)
                {
                    if (isWebSocketServerClosed) return;
                    //方案三：一个一个发送 + 数据打包发送
                    for (int i = 0; i < armModel.portLists.Count; i++)
                    {
                        dobotData = DataHelper.GetDobotNonRealTimeData(armModel.port2IndexDic[armModel.portLists[i]] - 1 +1000, armModel.portLists[i]);
                        Send(dobotData, socket);
                    }
                    await Task.Delay(200);//相当于发送时间(已经不等于发送时间了）
                }

            });
        }

        public void Close()
        {
            for (int i = 0; i < allSockets.Count; i++)
            {
                allSockets[i].Close();
            }
            isWebSocketServerClosed = true;
            server.Dispose();
        }


        public void SendAll(string msg)
        {
            for (int i = 0; i < allSockets.Count; i++)
            {
                allSockets[i].Send(msg);
            }
        }
        public void Send(byte[] buffer,IWebSocketConnection webSocket)
        {
            webSocket?.Send(buffer);
        }
        public void Send(DobotNonRealTimeData dobotData, IWebSocketConnection webSocket)
        {
            byte[] data = CommonHelper.Serialize(dobotData);
            byte[] lenArr = new byte[] { (byte)1, (byte)data.Length }; //前缀包括两个字节(数据类型和长度) 1--非实时
            byte[] res = lenArr.Concat(data).ToArray();
            webSocket?.Send(res);
        }
        public void Send(DobotRealTimeData dobotData, IWebSocketConnection webSocket)
        {
            byte[] data = CommonHelper.Serialize(dobotData);
            byte[] lenArr = new byte[] { (byte)0, (byte)data.Length }; //前缀包括两个字节(数据类型和长度) 0--实时
            byte[] res = lenArr.Concat(data).ToArray();
            webSocket?.Send(res);
        }
        public void Send(string msg, IWebSocketConnection webSocket)
        {
            webSocket?.Send(msg);
        }
        public void SendAll(byte[] buffer)
        {
            for (int i = 0; i < allSockets.Count; i++)
            {
                allSockets[i].Send(buffer);
            }
        }
    }



}