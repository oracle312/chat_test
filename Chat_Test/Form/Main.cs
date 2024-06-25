using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lib_Test.Member;

namespace Chat_Test
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        
        public Form1()
        {
            InitializeComponent();
            Opacity = 1;
        }

        private void Main_Shown(object send, EventArgs e)
        {
            Hide();
            Opacity = 1;

            foreach (Member mem in Program.members.Values)
            {
                TreeNode tn;

                if (!treeMem.Nodes.ContainsKey(mem.user_center))
                {
                    treeMem.Nodes.Add(mem.user_center, mem.user_center);
                    tn = treeMem.Nodes[mem.user_center];

                    
                }

                if (!treeMem.Nodes[mem.user_center].Nodes.ContainsKey(mem.user_team))
                {
                    treeMem.Nodes[mem.user_center].Nodes.Add(mem.user_team, mem.user_team);
                    tn = treeMem.Nodes[mem.user_center].Nodes[mem.user_team];
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
