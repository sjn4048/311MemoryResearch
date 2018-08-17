using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SirePluginDemo
{
    public partial class Form1 : Form
    {
        Modifier modifier;

        public Form1()
        {
            InitializeComponent();
            modifier = new Modifier();
            judgePOpenTimer.Start();
        }

        private void judgePOpenTimer_Tick(object sender, EventArgs e)
        {
            if (modifier.IsProcessOpen())
            {
                statusLabel.Text = "游戏已启动";
                statusLabel.ForeColor = Color.DarkGreen;
                judgePOpenTimer.Interval = 2000;
            }
            else
            {
                statusLabel.Text = "游戏未启动";
                statusLabel.ForeColor = Color.Red;
                judgePOpenTimer.Interval = 100;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var script = scriptTextbox.Text;
#if DEBUG
            script = UnitTest.beforeComment;
#endif
            var res = InjectData.ReadScript(script);
        }
    }
}
