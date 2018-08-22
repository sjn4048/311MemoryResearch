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
                statusLabel.Text = @"游戏未启动";
                statusLabel.ForeColor = Color.Red;
                judgePOpenTimer.Interval = 100;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var script = scriptTextbox.Text;
            try
            {
                memManager.WriteMemoryValue(script.ToInjectData());
                MessageBox.Show("已完成修改", "修改成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("重要提示，请务必阅读。\n将本脚本应用到exe后，未来只需启动修改后的exe即可享受本脚本效果，无需每次启动后打开修改器修改。支持多次积累更新。\n请注意：**这将永久改变exe的内容且不可还原**，请务必做好版本备份", "重要提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Cancel)
                return;

            var ofDialog = new OpenFileDialog()
            {
                Filter = "可执行文件|san11pk.exe|所有文件|*.*"
            };
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("是否备份原EXE？", "备份提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string bakPath = ofDialog.FileName + ".bak";
                    try
                    {                      
                        File.Copy(ofDialog.FileName, bakPath);
                        MessageBox.Show("已备份至" + bakPath, "备份成功",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }
                    catch
                    {
                        if (MessageBox.Show("检测到了一个已有的备份，是否覆盖？", "覆盖备份", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            File.Copy(ofDialog.FileName, bakPath, true);
                            MessageBox.Show("已覆盖至" + bakPath, "备份成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                memManager.WriteToEXE(ofDialog.FileName, scriptTextbox.Text.ToInjectData());
                MessageBox.Show("已完成修改", "修改成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void 内存镜像写入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string readme = "这是一个高级功能，请务必仔细阅读本说明。\n内存写入指的是将已经做好的内存修改持久化写入EXE。" +
                "这不仅将写入当前的脚本，还将写入对内存的所有修改(SIRE、水军patch、PKME等)。\n在写入完成后，你将无需打开" +
                "任何内存修改器，只需打开修改后的游戏EXE即可。\n要注意的是，由于内存与exe之间的同步有一定复杂性，这是一个测试功能，" +
                "作者不保证修改的绝对稳定性，因此强烈建议备份最原始的游戏EXE\n" +
                "在继续写入前，请保证已经用各路修改器将游戏内存修改到你想持久化保存的状态。是否继续？";
            //if (MessageBox.Show(readme, "高级功能", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            //    return;
            //MessageBox.Show("请选择一个已有的游戏EXE作为模板", "高级功能", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var ofDialog = new OpenFileDialog()
            {
                Filter = "可执行文件|san11pk.exe|所有文件|*.*"
            };
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                judgePOpenTimer.Stop();
                // 将timer暂停
                statusLabel.Text = "写入中...这需要一段时间，请耐心等待，不要关闭程序。";
                statusLabel.ForeColor = Color.DarkBlue;
                int count = 0;
                Task task = new Task(() => { memManager.RecordOldMem(); MessageBox.Show(""); count = memManager.DumpToEXE(ofDialog.FileName); });
                task.Start();
                Task.WaitAll(task);
                // 恢复timer
                judgePOpenTimer.Start();
                MessageBox.Show(count.ToString());
                //MessageBox.Show("镜像写入成功！以后只需打开该EXE即可。如果遇到BUG，请务必联系作者反馈，十分感谢。", "写入成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void 联系作者CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:3160105216@zju.edu.cn?subject=[修改工具反馈]");
        }
    }
}
