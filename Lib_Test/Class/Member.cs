using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_Test.Member
{
    [Serializable]
    public class Member
    {
        public Image profile;
        public string user_id, user_pw, user_name, user_mobile, user_center, user_team, user_rank;

        public Member(string user_id, string user_pw)
        {
            this.user_id = user_id;
            this.user_pw = user_pw;
        }

        public Member(Image profile, string user_id, string user_pw, string user_name, string user_mobile, string user_center, string user_team, string user_rank)
        {
            this.profile = profile;
            this.user_id = user_id;
            this.user_pw = user_pw;
            this.user_name = user_name;
            this.user_mobile = user_mobile;
            this.user_center = user_center;
            this.user_team = user_team;
            this.user_rank = user_rank;
        }
    }
}
