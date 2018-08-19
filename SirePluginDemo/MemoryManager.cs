using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SirePluginDemo
{
    class MemoryManager
    {
        /// <summary>
        /// 从kernel32.dll中引入的4个关于进程与内存的函数
        /// </summary>

        static readonly string ProcessName = "san11pk";

        #region imported dll functions
        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory
            (
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            IntPtr lpBuffer,
            int nSize,
            IntPtr lpNumberOfBytesRead
            );

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory
            (
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            int[] lpBuffer,
            int nSize,
            IntPtr lpNumberOfBytesWritten
            );

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess
            (
            int dwDesiredAccess,
            bool bInheritHandle,
            int dwProcessId
            );

        [DllImport("kernel32.dll")]
        private static extern void CloseHandle
            (
            IntPtr hObject
            );
        #endregion

        #region public interfaces
        /// <summary>
        /// 读指定进程的4字节内存
        /// </summary>
        /// <param name="baseAddress">进程内存基地址</param>
        /// <param name="length">读取长度</param>
        /// <returns>指定字节内存</returns>
        public int ReadMemoryValue(int baseAddress, int length = 4)
        {
            try
            {
                var buffer = new byte[length];
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0); //获取缓冲区地址
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName());
                ReadProcessMemory(hProcess, (IntPtr)baseAddress, byteAddress, length, IntPtr.Zero); //将制定内存中的值读入缓冲区
                CloseHandle(hProcess);
                return Marshal.ReadInt32(byteAddress);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 在任意多的位置，写指定进程的任意多内存
        /// </summary>
        /// <param name="injectNodes">InjectData数组，存放了所有需要注入的代码以及相应的内容(参见InjectData类说明)</param>
        public void WriteMemoryValue(InjectData[] injectNodes)
        {
            int pid = GetPidByProcessName();
            if (pid == -1)
                throw new NullReferenceException("未打开311pk.exe");
            IntPtr hProcess = OpenProcess(0x1F0FFF, false, pid); //0x1F0FFF 最高权限
            foreach (var injection in injectNodes)
            {
                for (int i = 0; i < injection.Length; i = i + 4)
                {
                    // length为要注入的字节数
                    int length = (injection.Length - i) >= 4 ? 4 : injection.Length - i;
                    int[] content = new[] { BitConverter.ToInt32(injection.Value.Skip(i).Take(4).ToArray(), 0) };
                    WriteProcessMemory(hProcess, (IntPtr)(injection.Address + i),content, length, IntPtr.Zero);
                }
            }
            CloseHandle(hProcess);
        }

        /// <summary>
        /// 判断是否打开311进程
        /// </summary>
        /// <returns>是否打开且无重复</returns>
        public bool IsProcessOpen()
        {
            return GetPidByProcessName() != -1;
        }
        #endregion

        #region private functions
        /// <summary>
        /// 由窗体名获取进程ID，暂未使用。
        /// </summary>
        /// <param name="windowTitle">窗体名</param>
        /// <returns>进程ID</returns>
        private int GetPidByTitle(string windowTitle)
        {
            int result = 0;
            Process[] processList = Process.GetProcesses();
            foreach (Process p in processList)
            {
                if (p.MainWindowTitle.IndexOf(windowTitle) != -1)
                {
                    result = p.Id;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 由exe名获取进程ID
        /// </summary>
        /// <param name="allowDuplicate">是否允许存在多个同样进程，默认为false</param>
        /// <returns>进程ID</returns>
        private int GetPidByProcessName(bool allowDuplicate = false)
        {
            var processList = Process.GetProcesses().Where(x => x.ProcessName == ProcessName).ToArray();
            // 未找到进程
            if (processList.Length == 0)
                return -1;
            // 找到了不允许的重复进程
            if (processList.Length > 1 && allowDuplicate == false)
                return -1;
            // 返回PID
            return processList[0].Id;
        }
        #endregion
    }
}
