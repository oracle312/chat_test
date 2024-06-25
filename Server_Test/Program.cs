using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lib_Test.Member;
using System.Net;
using System.Net.Sockets;
using Server_Test.Class;



namespace Server_Test
{
    class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        static ushort port = 3306;
        static TcpListener listener;
        static List<Client> unloginedClients;

        public static Dictionary<string, Client> clients;
        public static Dictionary<string, Member> members;
        public static Dictionary<string, List<string>> roomMem;
        static void Main(string[] args)
        {
            if (args.Length > 0) ushort.TryParse(args[0], out port);

            clients = new Dictionary<string, Client>();
            unloginedClients = new List<Client>();
            roomMem = new Dictionary<string, List<string>>();

            // 데이터 베이스 접속
            if (DB.Connect())
            {
                Log("DB", $"Server {DB.host} is connected");
                if (DB.CreateOfficeRoom())
                    Log("DB", "Office room is created");
                members = DB.GetMember();
            }
            else
            {
                Log("DB", $"Server {DB.host} connect failed.");
                return;
            }

            // 테스트

            // 서버 시작
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Log("System", $"Server is opend with port {port}");

            // 접속 받기
            while (true)
            {
                Client client = new Client(listener.AcceptTcpClient());
                if (!client.socket.Connected) continue;

                unloginedClients.Add(client);
                Log("Connect", "클라이언트 접속");
            }


        }
        public static void Log(int empId, string type, string content)
        {
            Console.WriteLine(string.Format("[{0}] {1} | {2} | {3}",
                DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"),
                empId, type, content));
        }

        public static void Log(string type, string content)
        {
            Console.WriteLine(string.Format("[{0}] {1} | {2}",
                DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"),
                type, content));
        }

        public static bool MoveLoginClient(Client c)
        {
            if (c.member.user_id == null)
            {
                Log("System", "사원 정보가 없는 클라이언트가 로그인을 했다고 함");
                return false;
            }

            if (clients.ContainsKey(c.member.user_id))
                return false;

            clients.Add(c.member.user_id, c);
            unloginedClients.Remove(c);
            return true;
        }
        public static void MoveLogoutClient(Client c)
        {
            if (c.member.user_id == null)
            {
                Log("System", "사원 정보가 없는 클라이언트가 로그아웃을 했다고 함");
                return;
            }

            unloginedClients.Add(c);
            clients.Remove(c.member.user_id);
        }
        public static void RemoveClient(Client c)
        {
            if (c.member.user_id == null)
                unloginedClients.Remove(c);
            else
                clients.Remove(c.member.user_id);
        }

    }
}
