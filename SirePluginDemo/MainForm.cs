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
    public partial class MainForm : Form
    {
        MemoryManager memManager;

        string CurrentFileName = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            memManager = new MemoryManager();
            judgePOpenTimer.Start();
        }

        private void judgePOpenTimer_Tick(object sender, EventArgs e)
        {
            if (memManager.IsProcessOpen())
            {
                statusLabel.Text = Resources.GameStartedStr;
                statusLabel.ForeColor = Color.DarkGreen;
                judgePOpenTimer.Interval = 2000;
            }
            else
            {
                statusLabel.Text = Resources.GameNotStartedStr;
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
                MessageBox.Show(Resources.WriteToMemSuccessTextStr, Resources.WriteToMemSuccessTitleStr, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(ex.Message, Resources.WriteToMemFailStr, MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
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
            MessageBox.Show(Resources.AboutAuthorStr);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void 帮助HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.HelpStr);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            const string initial = "Initial.txt";
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
            DialogResult result = MessageBox.Show(Resources.ModifyToExeStr, Resources.ImportantTipStr, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Cancel)
                return;

            var ofDialog = new OpenFileDialog()
            {
                Filter = "可执行文件|san11pk.exe|所有文件|*.*"
            };
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show(Resources.AskForBakStr, Resources.AskForBakTitleStr, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string bakPath = ofDialog.FileName + ".bak";
                    try
                    {                      
                        File.Copy(ofDialog.FileName, bakPath);
                        MessageBox.Show(Resources.BakSuccessStr + bakPath, Resources.BakSuccessTitleStr, MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }
                    catch
                    {
                        if (MessageBox.Show(Resources.FoundExistBakStr, Resources.OverwriteBakStr, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            File.Copy(ofDialog.FileName, bakPath, true);
                            MessageBox.Show(Resources.BakSuccessStr + bakPath, Resources.BakSuccessTitleStr, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                memManager.WriteToEXE(ofDialog.FileName, scriptTextbox.Text.ToInjectData());
                MessageBox.Show(Resources.WriteToExeSuccessTextStr, Resources.WriteToExeSuccessTitleStr, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void 内存镜像写入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.WriteToMirrorTipStr, Resources.ImportantTipStr, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                return;
            MessageBox.Show(Resources.ChooseExeFileStr, Resources.ProFeature, MessageBoxButtons.OK, MessageBoxIcon.Information);
            var ofDialog = new OpenFileDialog()
            {
                Filter = "可执行文件|san11pk.exe|所有文件|*.*"
            };
            if (ofDialog.ShowDialog() == DialogResult.OK)
            {
                judgePOpenTimer.Stop();
                // 将timer暂停
                statusLabel.Text = Resources.DuringWaitingStr;
                statusLabel.ForeColor = Color.DarkBlue;
                int count = 0;
                Task task = new Task(() => { memManager.RecordOldMem(); MessageBox.Show(""); count = memManager.DumpToEXE(ofDialog.FileName); });
                task.Start();
                Task.WaitAll(task);
                // 恢复timer
                judgePOpenTimer.Start();
                MessageBox.Show(count.ToString());
                MessageBox.Show(Resources.WriteToExeSuccessTextStr, Resources.WriteToExeSuccessTitleStr, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void 联系作者CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Resources.ContactAuthorStr);
        }

        private void 高级ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
