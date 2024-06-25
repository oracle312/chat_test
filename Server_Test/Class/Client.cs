using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib_Test.Member;
using Lib_Test.Network;
using Lib_Test.Secure;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Server_Test.Class
{
    public class Client
    {
        public TcpClient socket;
        NetworkStream ns;
        public Thread thread;
        public Member member;

        public Client(TcpClient socket)
        {
            this.socket = socket;
            ns = socket.GetStream();
            member = new Member(string.Empty, string.Empty);
            thread = new Thread(new ThreadStart(Recieve));
            thread.Start();

            public void Recieve()
            {
                byte[] buffer = null, bufferLen = new byte[4];
                while (true)
                {
                    try
                    {
                        if (ns.Read(bufferLen, 0, 4) < 4)
                        {
                            Log("리시브", "패킷 길이를 읽지 못했습니다.\n");

                            while (ns.ReadByte() != 1) ;
                        }
                        buffer = new byte[BitConverter.ToInt32(bufferLen, 0)];
                        int pos = 0;

                        while (pos < buffer.Length)
                            pos += ns.Read(buffer, pos, buffer.Length - pos);
                    }
                    catch (IOException socketExcept)
                    {
                        Log("IOException", socketExcept.Message);
                        Program.RemoveClient(this);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log("Exception", ex.ToString());
                        Program.RemoveClient(this);
                        break;
                    }

                    object obj = null;
                    try
                    {
                        obj = Packet.Deserialize(AES256.Decrypt(buffer));
                    }
                    catch (Exception except)
                    {
                        Log("Deserialize", ex.ToString());

                        // 수신 버퍼 리셋
                        while (ns.ReadByte() != -1) ;
                    }
                    if (obj == null) continue;

                    Packet packet = obj as Packet;

                    if (packet.type == PacketType.Header)
                    {
                        Log("위험", "Receieved no length HeaderPacket");
                    }
                    else if (packet.type == PacketType.Close)
                    {
                        Log("닫힘", "클라이언트와의 연결이 끊어졌습니다.");
                        socket.Close();
                        break;
                    }

                    // 로그인

                    else if (packet.type == PacketType.Login)
                    {
                        
                    }

                }
            }
        }
    }
}
