using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YeelightCMD
{
    class Yeelight
    {
        // result正则
        private static Regex RESULT_REGEX = new Regex("\"result\":" + @"\[" + "\"(.+)\"" + @"\]");

        // 设备ID
        private string id;
        // 设备连接对象
        private TcpClient tcpClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <param name="ip">设备IP</param>
        /// <param name="port">设备端口</param>
        public Yeelight(string id, string ip, int port)
        {
            this.id = id;
            this.tcpClient = new TcpClient(ip, port);
        }

        public bool Toggle()
        {
            // 命令
            byte[] buf = Encoding.Default.GetBytes("{\"id\": " + this.id + ", \"method\": \"toggle\", \"params\":[]}\r\n");
            // 发送
            this.tcpClient.Client.Send(buf);
            // 接收回应
            byte[] recv = new byte[1000];
            int len = this.tcpClient.Client.Receive(recv);

            // 转换
            string msg = Encoding.Default.GetString(recv.ToList().GetRange(0, len).ToArray());

            Console.WriteLine(msg);

            // 处理
            var resultGroups = RESULT_REGEX.Match(msg).Groups;
            string result = resultGroups[1].ToString();

            return result == "ok" ? true : false;
        }

        public bool set_bright(int brightness)
        {
            // 命令
            byte[] buf = Encoding.Default.GetBytes("{\"id\": " + this.id + ", \"method\": \"set_bright\", \"params\":["+brightness.ToString()+",\"smooth\",500]}\r\n");
            // 发送
            this.tcpClient.Client.Send(buf);
            // 接收回应
            byte[] recv = new byte[1000];
            int len = this.tcpClient.Client.Receive(recv);

            // 转换
            string msg = Encoding.Default.GetString(recv.ToList().GetRange(0, len).ToArray());

            Console.WriteLine(msg);

            // 处理
            var resultGroups = RESULT_REGEX.Match(msg).Groups;
            string result = resultGroups[1].ToString();

            return result == "ok" ? true : false;
        }

    }
}
