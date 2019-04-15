using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SirePluginDemo
{
    internal static class NativeMethods
    {
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
        public static extern void CloseHandle
        (
            IntPtr hObject
        );

        #endregion

        #region public interfaces

    }

    public class MemoryManager
    {
        /// <summary>
        /// 从kernel32.dll中引入的4个关于进程与内存的函数
        /// </summary>

        private static readonly int EXE_OFFSET = 0x400000;
        public struct Segment { public int start, end; public byte[] oldMem, newMem; }
        // 要扫描的代码区域
        private readonly Segment[] segs = {
            new Segment{ start = 0x401000, end = 0x74E000 },
            // RK 使用
            new Segment{ start = 0x8A5000, end = 0x90B000 },
            // txz_mk 使用
            new Segment{ start = 0x912000, end = 0x914000 },
            // sjn4048 使用
            new Segment{ start = 0x920000, end = 0x930000 }
        };

        /// <summary>
        /// 读指定进程的4字节内存
        /// </summary>
        /// <param name="baseAddress">进程内存基地址</param>
        /// <param name="length">读取长度</param>
        /// <returns>指定字节内存</returns>
        public byte[] ReadMemoryValue(int baseAddress, int length = 4)
        {
            int pid = GetPidByProcessName();
            if (pid == -1)
                throw new GameNotStartedException();
            try
            {
                var buffer = new byte[length];
                IntPtr byteAddress = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0); //获取缓冲区地址
                IntPtr hProcess = NativeMethods.OpenProcess(0x1F0FFF, false, pid);
                NativeMethods.ReadProcessMemory(hProcess, (IntPtr)baseAddress, byteAddress, length, IntPtr.Zero); //将制定内存中的值读入缓冲区
                NativeMethods.CloseHandle(hProcess);
                return buffer;
            }
            catch
            {
                throw new ReadMemoryException();
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
                throw new GameNotStartedException();
            IntPtr hProcess = NativeMethods.OpenProcess(0x1F0FFF, false, pid); //0x1F0FFF 最高权限
            foreach (var injection in injectNodes)
            {
                for (int i = 0; i < injection.Length; i = i + 4)
                {
                    // length为要注入的字节数
                    int length = (injection.Length - i) >= 4 ? 4 : injection.Length - i;
                    int[] content = { BitConverter.ToInt32(injection.Value.Skip(i).Take(4).ToArray(), 0) };
                    NativeMethods.WriteProcessMemory(hProcess, (IntPtr)(injection.Address + i), content, length, IntPtr.Zero);
                }
            }

            NativeMethods.CloseHandle(hProcess);
        }

        /// <summary>
        /// 将当前进程写入到exe中
        /// </summary>
        /// <param name="exePath">exe路径</param>
        /// <param name="injectNodes">注入的节点</param>
        public void WriteToEXE(string exePath, InjectData[] injectNodes)
        {
            using (var stream = new FileStream(exePath, FileMode.Open, FileAccess.ReadWrite))
            {
                foreach (var injection in injectNodes)
                {
                    stream.Position = injection.Address - EXE_OFFSET;
                    stream.Write(injection.Value, 0, injection.Length);
                }
            }
        }

        public void RecordOldMem()
        {
            for (int i = 0; i < segs.Length; i++)
            {
                int length = segs[i].end - segs[i].start;
                segs[i].oldMem = ReadMemoryValue(segs[i].start, length);
            }
        }

        /// <summary>
        /// 高级功能·将内存镜像写入至EXE
        /// </summary>
        /// <param name="exePath"></param>
        public int DumpToEXE(string exePath)
        {
            string newPath = exePath + "_NEW";
            File.Copy(exePath, newPath, true);
            int count = 0;
            using (var stream = new FileStream(newPath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (var sw = new StreamWriter(new FileStream("log.txt", FileMode.Create, FileAccess.ReadWrite)))
                {
                    // 遍历所有代码SEGMENT
                    for (int i = 0; i < segs.Length; i++)
                    {
                        int length = segs[i].end - segs[i].start;
                        segs[i].newMem = ReadMemoryValue(segs[i].start, length);
                        // 遍历新老内存，寻找差异
                        for (int j = 0; j < length; j++)
                        {
                            if (segs[i].oldMem[j] != segs[i].newMem[j])
                            {
                                // 定位到有差异的byte
                                stream.Position = segs[i].start + j;
                                // 写入新的
                                stream.WriteByte(segs[i].newMem[j]);
                                count++;
                                // 输出到log中，这个log可作为script直接使用。
                                sw.WriteLine($"{Resources.AddressSplitter} {stream.Position:X} {Resources.CodeSplitter} {segs[i].newMem[j]:X2}");
                            }
                        }
                    }
                }
            }
            return count;
        }

        public void DumpToEXE_old(string exePath)
        {
            string newPath = exePath + "_NEW";
            File.Copy(exePath, newPath, true);
            using (var stream = new FileStream(newPath, FileMode.Open, FileAccess.ReadWrite))
            {
                foreach (var seg in segs)
                {
                    int length = seg.end - seg.start + 1; // 要+1，因为要考虑末端的问题
                    byte[] content = ReadMemoryValue(seg.start, length);
                    stream.Position = seg.start - EXE_OFFSET;
                    stream.Write(content, 0, content.Length);
                }
            }
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
            var processList = Process.GetProcesses().Where(x => x.ProcessName == Resources.ProcessName).ToArray();
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
