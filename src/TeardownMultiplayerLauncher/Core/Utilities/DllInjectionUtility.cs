using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace TeardownMultiplayerLauncher.Core.Utilities
{
    // Modified example from guidedhacking.com | Credits: https://guidedhacking.com/threads/c-dll-injector-tutorial-how-to-inject-a-dll.14915/
    internal static class DllInjectionUtility
    {
        private class ghapi
        {
            public const int MAX_PATH = 260;
            private const int INVALID_HANDLE_VALUE = -1;

            [Flags]
            public enum ProcessAccessFlags : uint
            {
                All = 0x001F0FFF,
                Terminate = 0x00000001,
                CreateThread = 0x00000002,
                VirtualMemoryOperation = 0x00000008,
                VirtualMemoryRead = 0x00000010,
                VirtualMemoryWrite = 0x00000020,
                DuplicateHandle = 0x00000040,
                CreateProcess = 0x000000080,
                SetQuota = 0x00000100,
                SetInformation = 0x00000200,
                QueryInformation = 0x00000400,
                QueryLimitedInformation = 0x00001000,
                Synchronize = 0x00100000
            }
            [Flags]
            private enum SnapshotFlags : uint
            {
                HeapList = 0x00000001,
                Process = 0x00000002,
                Thread = 0x00000004,
                Module = 0x00000008,
                Module32 = 0x00000010,
                Inherit = 0x80000000,
                All = 0x0000001F,
                NoHeaps = 0x40000000
            }
            public struct PROCESS_INFORMATION
            {
                public IntPtr hProcess;
                public IntPtr hThread;
                public uint dwProcessId;
                public uint dwThreadId;
            }

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

            [DllImport("kernel32.dll")]
            public static extern int ResumeThread(IntPtr hThread);

            [DllImport("kernel32.dll")]
            public static extern IntPtr LoadLibraryA(string lpLibFileName);

            [DllImport("kernel32.dll")]
            public static extern uint GetThreadId(IntPtr hThread);

            [StructLayout(LayoutKind.Sequential)]
            public struct PROCESSENTRY32
            {
                public uint dwSize;
                public uint cntUsage;
                public uint th32ProcessID;
                public IntPtr th32DefaultHeapID;
                public uint th32ModuleID;
                public uint cntThreads;
                public uint th32ParentProcessID;
                public int pcPriClassBase;
                public uint dwFlags;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szExeFile;
            };

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public struct MODULEENTRY32
            {
                internal uint dwSize;
                internal uint th32ModuleID;
                internal uint th32ProcessID;
                internal uint GlblcntUsage;
                internal uint ProccntUsage;
                internal IntPtr modBaseAddr;
                internal uint modBaseSize;
                internal IntPtr hModule;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
                internal string szModule;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                internal string szExePath;
            }

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadProcessMemory(
            IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesRead);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool WriteProcessMemory(
            IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32.dll")]
            private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

            [DllImport("kernel32.dll")]
            private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

            [DllImport("kernel32.dll")]
            private static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

            [DllImport("kernel32.dll")]
            private static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool CloseHandle(IntPtr hHandle);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(string moduleName);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, int th32ProcessID);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32.dll")]
            public static extern IntPtr CreateRemoteThread(IntPtr hProcess,
               IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
               IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

            [Flags]
            public enum AllocationType
            {
                Commit = 0x1000,
                Reserve = 0x2000,
                Decommit = 0x4000,
                Release = 0x8000,
                Reset = 0x80000,
                Physical = 0x400000,
                TopDown = 0x100000,
                WriteWatch = 0x200000,
                LargePages = 0x20000000
            }

            [Flags]
            public enum MemoryProtection
            {
                Execute = 0x10,
                ExecuteRead = 0x20,
                ExecuteReadWrite = 0x40,
                ExecuteWriteCopy = 0x80,
                NoAccess = 0x01,
                ReadOnly = 0x02,
                ReadWrite = 0x04,
                WriteCopy = 0x08,
                GuardModifierflag = 0x100,
                NoCacheModifierflag = 0x200,
                WriteCombineModifierflag = 0x400
            }

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
                                uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

            public static IntPtr GetModuleBaseAddress(Process proc, string modName)
            {
                IntPtr addr = IntPtr.Zero;

                foreach (ProcessModule m in proc.Modules)
                {
                    if (m.ModuleName == modName)
                    {
                        addr = m.BaseAddress;
                        break;
                    }
                }
                return addr;
            }

            public static IntPtr GetModuleBaseAddress(int procId, string modName)
            {
                IntPtr modBaseAddr = IntPtr.Zero;

                IntPtr hSnap = CreateToolhelp32Snapshot(SnapshotFlags.Module | SnapshotFlags.Module32, procId);

                if (hSnap.ToInt64() != INVALID_HANDLE_VALUE)
                {
                    MODULEENTRY32 modEntry = new MODULEENTRY32();
                    modEntry.dwSize = (uint)Marshal.SizeOf(typeof(MODULEENTRY32));

                    if (Module32First(hSnap, ref modEntry))
                    {
                        do
                        {
                            if (modEntry.szModule.Equals(modName))
                            {
                                modBaseAddr = modEntry.modBaseAddr;
                                break;
                            }
                        } while (Module32Next(hSnap, ref modEntry));
                    }
                }

                CloseHandle(hSnap);
                return modBaseAddr;
            }

            public static int GetProcId(string procname)
            {
                int procid = 0;

                IntPtr hSnap = CreateToolhelp32Snapshot(SnapshotFlags.Process, 0);

                if (hSnap.ToInt64() != INVALID_HANDLE_VALUE)
                {
                    PROCESSENTRY32 procEntry = new PROCESSENTRY32();
                    procEntry.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));

                    if (Process32First(hSnap, ref procEntry))
                    {
                        do
                        {
                            if (procEntry.szExeFile.Equals(procname))
                            {
                                procid = (int)procEntry.th32ProcessID;
                                break;
                            }
                        } while (Process32Next(hSnap, ref procEntry));
                    }
                }

                CloseHandle(hSnap);
                return procid;
            }

            public static IntPtr FindDMAAddy(IntPtr hProc, IntPtr ptr, int[] offsets)
            {
                var buffer = new byte[IntPtr.Size];

                foreach (int i in offsets)
                {
                    ReadProcessMemory(hProc, ptr, buffer, buffer.Length, out
                    var read);
                    ptr = IntPtr.Size == 4 ? IntPtr.Add(new IntPtr(BitConverter.ToInt32(buffer, 0)), i) : ptr = IntPtr.Add(new IntPtr(BitConverter.ToInt64(buffer, 0)), i);
                }
                return ptr;
            }

            public static bool InjectDLL(string dllpath, string procname)
            {
                Process[] procs = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(procname));

                Debug.WriteLine(procs.Length);

                if (procs.Length == 0)
                {
                    return false;
                }

                Process proc = procs[0];

                //redundant native method example - GetProcessesByName will automatically open a handle
                int procid = GetProcId(procname);
                IntPtr hProc = OpenProcess(ProcessAccessFlags.All, false, proc.Id);
                //

                Debug.WriteLine(hProc);

                if (proc.Handle != IntPtr.Zero)
                {
                    //proc.Handle = managed
                    IntPtr loc = VirtualAllocEx(proc.Handle, IntPtr.Zero, MAX_PATH, AllocationType.Commit | AllocationType.Reserve,
                        MemoryProtection.ReadWrite);

                    if (loc.Equals(0))
                    {
                        return false;
                    }

                    IntPtr bytesRead = IntPtr.Zero;

                    bool result = WriteProcessMemory(proc.Handle, loc, dllpath.ToCharArray(), dllpath.Length, out bytesRead);

                    if (!result || bytesRead.Equals(0))
                    {
                        return false;
                    }

                    IntPtr loadlibAddy = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                    //redundant native method example - MUST BE CASE SENSITIVE CORRECT
                    loadlibAddy = GetProcAddress(GetModuleBaseAddress(proc.Id, "KERNEL32.DLL"), "LoadLibraryA");

                    IntPtr hThread = CreateRemoteThread(proc.Handle, IntPtr.Zero, 0, loadlibAddy, loc, 0, out _);

                    Debug.WriteLine("what");

                    if (!hThread.Equals(0))
                        //native method example
                        CloseHandle(hThread);
                    else
                    {
                        return false;
                    }
                }
                else return false;

                //this will CloseHandle automatically using the managed method
                proc.Dispose();
                return true;
            }
        }

        public static bool InjectDLL(string dllpath, Services.GameLaunchingService.PROCESS_INFORMATION ProcInfo)
        {
            const int PROCESS_CREATE_THREAD = 0x0002;
            const int PROCESS_QUERY_INFORMATION = 0x0400;
            const int PROCESS_VM_OPERATION = 0x0008;
            const int PROCESS_VM_WRITE = 0x0020;
            const int PROCESS_VM_READ = 0x0010;

            const uint MEM_COMMIT = 0x00001000;
            const uint MEM_RESERVE = 0x00002000;
            const uint PAGE_READWRITE = 4;
            // the target process - I'm using a dummy process for this
            // if you don't have one, open Task Manager and choose wisely
            Process targetProcess = Process.GetProcessById((int)ProcInfo.dwProcessId);

            // geting the handle of the process - with required privileges
            IntPtr procHandle = ghapi.OpenProcess((ghapi.ProcessAccessFlags)(PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ), false, targetProcess.Id);

            // searching for the address of LoadLibraryA and storing it in a pointer
            IntPtr loadLibraryAddr = ghapi.GetProcAddress(ghapi.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            // name of the dll we want to inject

            // alocating some memory on the target process - enough to store the name of the dll
            // and storing its address in a pointer
            IntPtr allocMemAddress = ghapi.VirtualAllocEx(procHandle, IntPtr.Zero, (uint)((dllpath.Length + 1) * Marshal.SizeOf(typeof(char))), (ghapi.AllocationType)(MEM_COMMIT | MEM_RESERVE), (ghapi.MemoryProtection)PAGE_READWRITE);

            // writing the name of the dll there
            IntPtr bytesWritten;
            ghapi.WriteProcessMemory(procHandle, allocMemAddress, Encoding.Default.GetBytes(dllpath), ((dllpath.Length + 1) * Marshal.SizeOf(typeof(char))), out bytesWritten);

            // creating a thread that will call LoadLibraryA with allocMemAddress as argument
            ghapi.CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, out _);

            ghapi.ResumeThread(ProcInfo.hThread);

            return true;


            //IntPtr loc = ghapi.VirtualAllocEx(ProcInfo.hProcess, IntPtr.Zero, ghapi.MAX_PATH, ghapi.AllocationType.Commit | ghapi.AllocationType.Reserve, ghapi.MemoryProtection.ReadWrite);
            //IntPtr loadlibAddy = ghapi.GetProcAddress(ghapi.GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            //if (ProcInfo.hThread == IntPtr.Zero)
            //{
            //    ProcInfo.hThread = ghapi.CreateRemoteThread(ProcInfo.hProcess, IntPtr.Zero, 0, loadlibAddy, loc, 0, out _);
            //    ProcInfo.dwThreadId = ghapi.GetThreadId(ProcInfo.hThread);
            //} 
            //else
            //{
            //    ProcInfo.hThread = ghapi.CreateRemoteThread(ProcInfo.hProcess, IntPtr.Zero, 0, loadlibAddy, loc, 0, out _);
            //}

            //if(ProcInfo.hThread == IntPtr.Zero)
            //{
            //    ghapi.CloseHandle(ProcInfo.hProcess);
            //    return false;
            //}

            //ghapi.ResumeThread(ProcInfo.hThread);

            //return true;

            ////redundant native method example - GetProcessesByName will automatically open a handle
            //int procid = process.Id;
            //IntPtr hProc = ghapi.OpenProcess(ghapi.ProcessAccessFlags.All, false, process.Id);

            //if (process.Handle != IntPtr.Zero)
            //{
            //    //proc.Handle = managed
            //    I

            //    if (loc.Equals(0))
            //    {
            //        return false;
            //    }

            //    IntPtr bytesRead = IntPtr.Zero;

            //    bool result = ghapi.WriteProcessMemory(process.Handle, loc, dllpath.ToCharArray(), dllpath.Length, out bytesRead);

            //    if (!result || bytesRead.Equals(0))
            //    {
            //        return false;
            //    }

            //    IntPtr loadlibAddy = ghapi.GetProcAddress(ghapi.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            //    //redundant native method example - MUST BE CASE SENSITIVE CORRECT


            //    IntPtr hThread = ghapi.CreateRemoteThread(process.Handle, IntPtr.Zero, 0, loadlibAddy, loc, 0, out _);

            //    if(hThread == IntPtr.Zero)
            //    {
            //        process.Dispose();
            //        return false;
            //    }

            //    ghapi.ResumeThread(hThread);                
            //}
            //else return false;

            ////this will CloseHandle automatically using the managed method
            //return true;
        }
    }
}
