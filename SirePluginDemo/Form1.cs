using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace SirePluginDemo
{
    public partial class Form1 : Form
    {
        MemoryManager memManager;

        string CurrentFileName = string.Empty;

        public Form1()
        {
            InitializeComponent();
            memManager = new MemoryManager();
            judgePOpenTimer.Start();
        }

        private void judgePOpenTimer_Tick(object sender, EventArgs e)
        {
            if (memManager.IsProcessOpen())
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
            if (script == string.Empty)
            script = UnitTest.sample;
#endif
            // 将脚本转化为InjectData[]并写入
            try
            {
                memManager.WriteMemoryValue(script.ToInjectData());
                MessageBox.Show("Finished.", "修改成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(ex.Message, "修改失败", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                {
                    button1.PerformClick();
                }
            }
        }

        private void 载入脚本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofDialog = new OpenFileDialog()
            {
                // InitialDirectory = "",
                Filter = "所有文件|*.*"
            };
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentFileName = ofDialog.FileName;
                scriptTextbox.Text = File.ReadAllText(CurrentFileName);
                //return true;
            }
        }

        private void 保存脚本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentFileName != string.Empty)
            {
                File.WriteAllText(CurrentFileName, scriptTextbox.Text);
                //return true;
            }
            else
            {
                另存为ToolStripMenuItem.PerformClick();
            }
            //return false;
        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfdialog = new SaveFileDialog()
            {
                Filter = "所有文件|*.*",
            };
            if (sfdialog.ShowDialog() == DialogResult.OK)
            {
                CurrentFileName = sfdialog.FileName;
                File.WriteAllText(CurrentFileName, scriptTextbox.Text);
                //return true;
            }
            //return false;
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Written By sjn4048@ZJU");
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void 帮助HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("在与EXE同目录下新建Initial.txt，存放默认加载的脚本。");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string initial = "Initial.txt";
            if (!File.Exists(initial))
                File.Create(initial);
            else
            {
                string content = File.ReadAllText(initial);
                if (content != string.Empty)
                    scriptTextbox.Text = content;
            }
        }
    }
}
