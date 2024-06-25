using System;
using System.Collections.Generic;
using Lib_Test.Member;
using Lib_Test.Chat;
using Lib_Test.Class;

namespace Lib_Test.Network
{
    [Serializable]
    public class Login : Packet
    {
        public Dictionary<string, Member> members;
        public List<Room> rooms;
        public List<Schedule> schedules;

        public bool success = false;

        public Login (string id, string pw)
        {
            type = PacketType.Login;

            members = new Dictionary<string, Member>();
            members.Add(null, new Lib_Test.Member(id, pw));
        }
    }
}
