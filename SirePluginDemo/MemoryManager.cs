using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SirePluginDemo
{
    static class MemoryManager
    {
        /// <summary>
        /// 从kernel32.dll中引入的4个关于进程与内存的函数
        /// </summary>

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

        #region public interfaces

        /// <summary>
        /// 根据窗口标题查找窗口句柄
        /// </summary>
        /// <param name="title">窗口标题</param>
        /// <param name="allowDuplicate">是否允许存在多个同样进程，默认为false</param>
        /// <returns>窗口句柄</returns>
        public static IntPtr FindWindow(string title, bool allowDuplicate = false)
        {
            var handleList = Process.GetProcesses()
                .Where(x => x.MainWindowTitle.IndexOf(title) != -1)
                .Select(x => x.MainWindowHandle)
                .ToArray();
            // 未找到
            if (handleList.Length == 0)
                return IntPtr.Zero;
            // 找到了不允许的重复值
            if (handleList.Length > 1 && allowDuplicate == false)
                return IntPtr.Zero;
            // 返回第一个
            return handleList[0];
        }

        /// <summary>
        /// 读指定进程的4字节内存
        /// </summary>
        /// <param name="baseAddress">进程内存基地址</param>
        /// <param name="processName">进程名称</param>
        /// <param name="length">读取长度</param>
        /// <returns>指定字节内存</returns>
        public static int ReadMemoryValue(int baseAddress, string processName, int length = 4)
        {
            try
            {
                var buffer = new byte[length];
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0); //获取缓冲区地址
                IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName(processName));
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
        /// <param name="processName">进程名称</param>
        /// <param name="injectNodes">InjectData数组，存放了所有需要注入的代码以及相应的内容(参见InjectData类说明)</param>
        public static void WriteMemoryValue(string processName, InjectData[] injectNodes)
        {
            IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName(processName)); //0x1F0FFF 最高权限
            foreach (var injection in injectNodes)
            {
                WriteProcessMemory(hProcess, (IntPtr)injection.Address, new[] { BitConverter.ToInt32(injection.Value, 0) }, injection.Length, IntPtr.Zero);
            }
            CloseHandle(hProcess);
        }

        /// <summary>
        /// 判断是否打开311进程
        /// </summary>
        /// <param name="processName">进程名称</param>
        /// <returns>是否打开且无重复</returns>
        public static bool IsProcessOpen(string processName)
        {
            return GetPidByProcessName(processName) != -1;
        }
        #endregion

        #region private functions
        /// <summary>
        /// 由窗体名获取进程ID，暂未使用。
        /// </summary>
        /// <param name="windowTitle">窗体名</param>
        /// <returns>进程ID</returns>
        private static int GetPidByTitle(string windowTitle)
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
        /// <param name="processName">进程名，默认为"san11pk"</param>
        /// <param name="allowDuplicate">是否允许存在多个同样进程，默认为false</param>
        /// <returns>进程ID</returns>
        private static int GetPidByProcessName(string processName, bool allowDuplicate = false)
        {
            var processList = Process.GetProcesses().Where(x => x.ProcessName == processName).ToArray();
            // 未找到进程
            if (processList.Length == 0)
                return -1;
            // 找到了不允许的重复进程
            if (processList.Length > 1 && allowDuplicate == false)
                return -1;
            // 返回PID
            return processList[0].Id;
        }

        /// <summary>
        /// 由进程名获取进程ID
        /// </summary>
        /// <param name="processName">进程名</param>
        /// <param name="allowDuplicate">是否允许存在多个同样进程，默认为false</param>
        /// <returns>进程ID</returns>
        private static int GetPidByExeName(string processName, bool allowDuplicate = false)
        {
            var tmp = Process.GetProcesses();
            Process[] processList = Process.GetProcessesByName(processName);
            // 未找到
            if (processList.Length == 0)
                return -1;
            // 找到了不允许的重复值
            if (processList.Length > 1 && allowDuplicate == false)
                return -1;
            return processList[0].Id;
        }

        /// <summary>
        /// 写指定进程的4字节内存
        /// </summary>
        /// <param name="baseAddress">进程内存基地址</param>
        /// <param name="processName">进程名称</param>
        /// <param name="value">写入内容</param>
        private static void WriteMemoryValue(int baseAddress, string processName, int value)
        {
            IntPtr hProcess = OpenProcess(0x1F0FFF, false, GetPidByProcessName(processName)); //0x1F0FFF 最高权限
            WriteProcessMemory(hProcess, (IntPtr)baseAddress, new[] { value }, 4, IntPtr.Zero);
            CloseHandle(hProcess);
        }
        #endregion
    }
}
