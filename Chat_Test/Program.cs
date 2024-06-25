using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Lib_Test.Member;


namespace Chat_Test
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        /// 
        public static object locker = new object();
        public static TcpClient client;
        public static NetworkStream ns;
        public static string hostname;
        public static readonly ushort port = 8000;
        public static Thread thread;

        public static Member member; // 사원정보
        public static Dictionary<string, Member> members; // 사원들정보
        

        [STAThread]
        static void Main()
        {
            client = new TcpClient();

            string[] args = Environment.GetCommandLineArgs();
            hostname = args.Length > 1 ? args[1] : "localhost";
            
            

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
