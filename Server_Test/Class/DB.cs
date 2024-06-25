using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Lib_Test.Member;
using Lib_Test.Chat;
using Lib_Test.Secure;
using Lib_Test.Class;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;


namespace Server_Test.Class
{
    static class DB
    {
        public static MySqlConnection con;
        public static readonly string host = "localhost";
        public static readonly string id = "root";
        public static readonly string pw = "0000";
        public static readonly string db = "testdb";

        static string Filter(string str)
        {
            return Regex.Replace(str, @"[-<>()'""\;=+|&#.]", "");
        }

        public static bool Connect()
        {
            con = new MySqlConnection(string.Format("Server={0};Database={1};Uid={2};Pwd={3};",
                host, db, id, pw));
            try
            {
                con.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Dictionary<string, Member> GetMember()
        {
            string sql = $"SELECT user_id, user_name, user_mobile, user_center, user_team, `user_rank`, user_profile, user_profileLen FROM member";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            Dictionary<string, Member> members = new Dictionary<string, Member>();

            using (MySqlDataReader dr = cmd.ExecuteReader())
                while(dr.Read())
                {
                    byte[] profileBytes = new byte[dr.GetUInt32("user_profileLen")];
                    dr.GetBytes(dr.GetOrdinal("user_profile"), 0, profileBytes, 0, profileBytes.Length);

                    using (MemoryStream ms = new MemoryStream(profileBytes))
                        members.Add(dr.GetString("user_id"), new Member(
                            Image.FromStream(ms),
                            String.Empty,
                            dr.GetString("user_id"),
                            dr.GetString("user_name"),
                            dr.GetString("user_mobile"),
                            dr.GetString("user_center"),
                            dr.GetString("user_team"),
                            dr.GetString("user_rank")));
                }
            return members;
        }

        public static Member Login(string id, string pw)
        {
            string sql = $"SELECT user_id, user_name, user_mobile, user_center, user_team, `user_rank`, user_profile, user_profileLen FROM member";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            using (MySqlDataReader dr = cmd.ExecuteReader())
                while (dr.Read())
                {
                    byte[] profileBytes = new byte[dr.GetUInt32("user_profileLen")];
                    dr.GetBytes(dr.GetOrdinal("user_profile"), 0, profileBytes, 0, profileBytes.Length);

                    using (MemoryStream ms = new MemoryStream(profileBytes))
                        return new Member(
                        
                            Image.FromStream(ms),
                            String.Empty,
                            dr.GetString("user_id"),
                            dr.GetString("user_name"),
                            dr.GetString("user_mobile"),
                            dr.GetString("user_center"),
                            dr.GetString("user_team"),
                            dr.GetString("user_rank"));
                }
            return null; 
        }

        public static bool Register(Member member)
        {
            MySqlCommand cmd = new MySqlCommand(
                "INSERT INTO member VALUES (@user_id, @user_pw, @user_name, @user_mobile, @user_center, @user_team, @user_rank, @user_profile, @user_profileLen);",
                con);

            using(MemoryStream ms = new MemoryStream())
            {
                member.profile.Save(ms, ImageFormat.Png);
                MySqlParameter blob = new MySqlParameter("@user_profile", MySqlDbType.LongBlob, (int)ms.Length);
                blob.Value = ms.ToArray();
                cmd.Parameters.AddWithValue("@user_id", member.user_id);
                cmd.Parameters.AddWithValue("@user_pw", member.user_pw);
                cmd.Parameters.AddWithValue("@user_name", member.user_name);
                cmd.Parameters.AddWithValue("@user_mobile", member.user_mobile);
                cmd.Parameters.AddWithValue("@user_center", member.user_center);
                cmd.Parameters.AddWithValue("@user_team", member.user_team);
                cmd.Parameters.AddWithValue("@user_rank", member.user_rank);
                cmd.Parameters.Add(blob);
                cmd.Parameters.AddWithValue("@user_profileLen", (int)ms.Length);

                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (MySqlException except)
                {
                    Program.Log("DB", except.ToString());
                    return false;
                }
            }
      
        }

        public static bool AddSchedule(Schedule schedule)
        {

            MySqlCommand cmd = new MySqlCommand(
                "INSERT INTO schedule VALUES (@author, @title, @start, @end, @scope, @contents, @target);",
                con);
            using (MemoryStream ms = new MemoryStream())
            {
                MySqlParameter start = new MySqlParameter("@start", MySqlDbType.DateTime, (int)ms.Length);
                MySqlParameter end = new MySqlParameter("@end", MySqlDbType.DateTime, (int)ms.Length);
                MySqlParameter contents = new MySqlParameter("@contents", MySqlDbType.LongText, (int)ms.Length);
                start.Value = schedule.start; // MetroDateTime 연결할 것
                end.Value = schedule.end; // MetroDateTime 연결할 것
                contents.Value = schedule.contents;

                cmd.Parameters.AddWithValue("@author", schedule.author);
                cmd.Parameters.AddWithValue("@title", schedule.title);
                cmd.Parameters.Add(start);
                cmd.Parameters.Add(end);
                cmd.Parameters.AddWithValue("@scope", schedule.scope);
                cmd.Parameters.Add(contents);
                cmd.Parameters.AddWithValue("@target", schedule.target); ;
            }
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException e)
            {
                Program.Log("DB", e.ToString());
                return false;
            }
        }
        public static List<Schedule> GetSchedule(Member mem)
        {
            List<Schedule> schedules = new List<Schedule>();

            string sql = $"(SELECT * FROM schedule WHERE scope = '회사 전체')"
                + $" UNION (SELECT * FROM schedule WHERE scope = '본부 전체' AND target='{Filter(mem.user_center)}')"
                + $" UNION (SELECT * FROM schedule WHERE scope = '팀 전체' AND target='{Filter(mem.user_team)}')"
                + $" UNION (SELECT * FROM schedule WHERE scope = '개인')";
            MySqlCommand cmd = new MySqlCommand(sql, con);

            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    using (MemoryStream ms = new MemoryStream())
                        schedules.Add(new Schedule(
                            rdr.GetInt32("author"),
                            rdr.GetString("title"),
                            rdr.GetDateTime("start"),
                            rdr.GetDateTime("end"),
                            rdr.GetString("scope"),
                            rdr.GetString("contents"),
                            rdr.GetString("target")));
                }
            }
            return schedules;

        }

        public static bool AddRoom(Room room)
        {
            MySqlCommand cmd = new MySqlCommand(
                   "INSERT INTO room VALUES (@id, @scope, @target);",
                   con);

            cmd.Parameters.AddWithValue("@id", room.id);
            cmd.Parameters.AddWithValue("@scope", room.scopeIdx);
            cmd.Parameters.AddWithValue("@target", room.target);
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (MySqlException e)
            {
                Program.Log("DB", e.ToString());
                return false;
            }
        }

        public static bool CreateOfficeRoom()
        {
            // 회사 전체 톡방이 없을 경우 만듦
            MySqlCommand cmd = new MySqlCommand(
                "SELECT * FROM room WHERE scope=0;", con);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (!rdr.HasRows)
            {
                rdr.Close();
                cmd = new MySqlCommand(
                    "INSERT INTO room VALUES (@id, @scope, @target);",
                    con);

                cmd.Parameters.AddWithValue("@id", MD5.NextRandom());
                cmd.Parameters.AddWithValue("@scope", 0);
                cmd.Parameters.AddWithValue("@target", Room.Scope[0]);
                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            rdr.Close();
            return false;
        }
        public static List<Room> GetRooms(Member mem)
        {
            List<Room> rooms = new List<Room>();

            string sql =
                $"(SELECT * FROM room WHERE scope=0)" +
                $" UNION (SELECT * FROM room WHERE scope=1 AND target='{Filter(mem.user_center)}')" +
                $" UNION (SELECT * FROM room WHERE scope=2 AND target='{Filter(mem.user_team)}')" +
                $" UNION (SELECT * FROM room WHERE scope=3 AND target LIKE '%{mem.user_id}%')";

            using (MySqlDataReader rdr = new MySqlCommand(sql, con).ExecuteReader())
                while (rdr.Read())
                    rooms.Add(new Room(rdr.GetString("id"), rdr.GetInt32("scope"), rdr.GetString("target")));

            return rooms;
        }

        public static bool AddChat(Chat chat)
        {
            MySqlCommand cmd = new MySqlCommand(
                "INSERT INTO chat VALUES (@room_id, @chat_date, @employee_id, @data, @data_length, @data_type);",
                con);

            using (MemoryStream ms = new MemoryStream())
            {
                cmd.Parameters.AddWithValue("@room_id", chat.room.id);
                cmd.Parameters.AddWithValue("@chat_date", chat.date);
                cmd.Parameters.AddWithValue("@employee_id", chat.empId);

                // 데이터 byte[]로 만들어서 sql에 집어넣기
                if (chat.type == ChatType.Image)
                {
                    chat.image.Save(ms, ImageFormat.Png);
                    cmd.Parameters.Add(new MySqlParameter("@data", MySqlDbType.LongBlob, (int)ms.Length))
                        .Value = ms.ToArray();
                    cmd.Parameters.AddWithValue("@data_length", (int)ms.Length);
                }
                else
                {
                    byte[] textBytes = Encoding.UTF8.GetBytes(chat.text);
                    cmd.Parameters.Add(new MySqlParameter("@data", MySqlDbType.LongBlob, textBytes.Length))
                        .Value = textBytes;
                    cmd.Parameters.AddWithValue("@data_length", textBytes.Length);
                }
                cmd.Parameters.AddWithValue("@data_type", chat.type);

                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (MySqlException e)
                {
                    Program.Log("DB", e.ToString());
                    return false;
                }
            }
        }
        public static Chat GetLastChat(Room room)
        {
            MySqlCommand cmd = new MySqlCommand(
                $"SELECT * FROM chat WHERE room_id='{Filter(room.id)}' ORDER BY chat_date DESC LIMIT 1",
                con);

            using (MySqlDataReader rdr = cmd.ExecuteReader())
                if (rdr.Read())
                {
                    ChatType ct = (ChatType)rdr.GetInt32("data_type");
                    byte[] dataBytes = new byte[rdr.GetInt32("data_length")];
                    rdr.GetBytes(rdr.GetOrdinal("data"), 0, dataBytes, 0, dataBytes.Length);

                    room.id = rdr.GetString("room_id");
                    Chat chat = new Chat(ct, rdr.GetDateTime("chat_date"), room,
                        rdr.GetInt32("employee_id"), "");

                    if (ct == ChatType.Image)
                        using (MemoryStream ms = new MemoryStream(dataBytes))
                            chat.image = Image.FromStream(ms);
                    else
                        chat.text = Encoding.UTF8.GetString(dataBytes);

                    return chat;
                }
            return null;
        }
        public static List<Chat> GetChats(Room room, DateTime until)
        {
            List<Chat> chats = new List<Chat>();
            MySqlCommand cmd = new MySqlCommand(
                $"SELECT * FROM chat WHERE room_id='{Filter(room.id)}'"
                + "AND chat_date < @chat_date_until LIMIT 100",
                con);
            cmd.Parameters.AddWithValue("@chat_date_until", until);

            using (MySqlDataReader rdr = cmd.ExecuteReader())
                while (rdr.Read())
                {
                    ChatType ct = (ChatType)rdr.GetInt32("data_type");
                    byte[] dataBytes = new byte[rdr.GetInt32("data_length")];
                    rdr.GetBytes(rdr.GetOrdinal("data"), 0, dataBytes, 0, dataBytes.Length);

                    room.id = rdr.GetString("room_id");
                    Chat chat = new Chat(ct, rdr.GetDateTime("chat_date"), room,
                        rdr.GetInt32("employee_id"), "");

                    if (ct == ChatType.Image)
                        using (MemoryStream ms = new MemoryStream(dataBytes))
                            chat.image = Image.FromStream(ms);
                    else
                        chat.text = Encoding.UTF8.GetString(dataBytes);

                    chats.Add(chat);
                }
            return chats;
        }



    }
}
