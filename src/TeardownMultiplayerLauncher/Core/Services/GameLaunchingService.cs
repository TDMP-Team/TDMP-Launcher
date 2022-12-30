using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TeardownMultiplayerLauncher.Core.Models.State;
using TeardownMultiplayerLauncher.Core.Utilities;

namespace TeardownMultiplayerLauncher.Core.Services
{
    internal class GameLaunchingService
    {
        private static readonly int MaxProcessSearchAttempts = 30;
        private static readonly TimeSpan ProcessSearchInterval = TimeSpan.FromSeconds(1);
        private static readonly string TeardownProcessName = "teardown";
        private readonly LauncherState _state;

        public GameLaunchingService(LauncherState state)
        {
            _state = state;
        }

        public Task LaunchTeardownMultiplayerAsync()
        {
            return Task.Run(() =>
            {
                LaunchTeardown();
                //WaitForGameAndInject();
            }).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    throw task.Exception;
                }
            });
        }

        private void LaunchTeardown()
        {
            //Process.Start(
            //    new ProcessStartInfo("steam://rungameid/1167630")
            //    {
            //        UseShellExecute = true,
            //        Verb = "open",
            //    }
            //);
            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
            bool success = CreateProcess(_state.TeardownExePath, null,
                IntPtr.Zero, IntPtr.Zero, true,
                ProcessCreationFlags.CREATE_DEFAULT_ERROR_MODE | ProcessCreationFlags.CREATE_SUSPENDED,
                IntPtr.Zero, PathUtility.GetTeardownDirectory(_state.TeardownExePath), ref si, out pi);
            //Thread.Sleep(ProcessSearchInterval*10);
            //Process teardownProcess = Process.GetProcessById((int)pi.dwProcessId);
            if (!InjectTeardownMultiplayer(pi))
            {
                throw new Exception("Failed to inject TDMP");
            }
            Thread.Sleep(2000);
            //teardownProcess = Process.GetProcessById((int)pi.dwProcessId);
            //ResumeThread(teardownProcess.Threads[0].Id);
            //teardownProcess = Process.GetProcessById((int)pi.dwProcessId);
            //ResumeProcess(teardownProcess);
            Debug.WriteLine(success);
            //teardownProcess.WaitForExit();
        }

        //private void WaitForGameAndInject()
        //{
        //    for (var processSearchAttempt = 1; processSearchAttempt <= MaxProcessSearchAttempts; ++processSearchAttempt)
        //    {
        //        Thread.Sleep(ProcessSearchInterval); // Search interval.

        //        var teardownProcess = Process.GetProcessesByName(TeardownProcessName).FirstOrDefault();
        //        if (teardownProcess == null)
        //        {
        //            if (processSearchAttempt < MaxProcessSearchAttempts)
        //            {
        //                continue;
        //            }
        //            throw new Exception("Could not find running Teardown process");
        //        }

        //        //Thread.Sleep(_state.InjectionDelay); // TODO: Reliably check memory if Teardown Game object is initialized. Slower PCs might take longer for Teardown to fully initialize and be ready to inject into.
        //        //SuspendProcess(teardownProcess);
        //        if (!InjectTeardownMultiplayer(teardownProcess))
        //        {
        //            throw new Exception("Failed to inject TDMP");
        //        }
        //        teardownProcess = Process.GetProcessesByName(TeardownProcessName).FirstOrDefault(); // Search for teardown process again after injection because the old Process object gets corrupted.
        //        if (teardownProcess != null)
        //        {
        //            //ResumeProcess(teardownProcess);
        //            teardownProcess.WaitForExit();
        //        }
        //        return;
        //    }
        //}

        private bool InjectTeardownMultiplayer(PROCESS_INFORMATION pi)
        {
            try
            {
                return DllInjectionUtility.InjectDLL(PathUtility.GetTeardownMultiplayerDllPath(_state.TeardownExePath), pi);
            }
            catch
            {
                try
                {
                    //teardownProcess.Kill();
                }
                catch
                {
                    MessageBox.Show(
                        "Teardown could not shut down and is still running in the background. Please wait a few minutes for your OS to clear it",
                        "TDMP Launcher"
                    );
                    return false;
                }
                return false;
            }
        }

        [Flags]
        public enum ProcessCreationFlags : uint
        {
            ZERO_FLAG = 0x00000000,
            CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
            CREATE_DEFAULT_ERROR_MODE = 0x04000000,
            CREATE_NEW_CONSOLE = 0x00000010,
            CREATE_NEW_PROCESS_GROUP = 0x00000200,
            CREATE_NO_WINDOW = 0x08000000,
            CREATE_PROTECTED_PROCESS = 0x00040000,
            CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
            CREATE_SEPARATE_WOW_VDM = 0x00001000,
            CREATE_SHARED_WOW_VDM = 0x00001000,
            CREATE_SUSPENDED = 0x00000004,
            CREATE_UNICODE_ENVIRONMENT = 0x00000400,
            DEBUG_ONLY_THIS_PROCESS = 0x00000002,
            DEBUG_PROCESS = 0x00000001,
            DETACHED_PROCESS = 0x00000008,
            EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
            INHERIT_PARENT_AFFINITY = 0x00010000
        }

        public struct STARTUPINFO
        {
            public uint cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(string lpApplicationName,
           string lpCommandLine, IntPtr lpProcessAttributes,
           IntPtr lpThreadAttributes,
           bool bInheritHandles, ProcessCreationFlags dwCreationFlags,
           IntPtr lpEnvironment, string lpCurrentDirectory,
           ref STARTUPINFO lpStartupInfo,
           out PROCESS_INFORMATION lpProcessInformation);

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);


        private static void SuspendProcess(Process process)
        {
            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        public static void ResumeProcess(Process process)
        {
            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                CloseHandle(pOpenThread);
            }
        }

    }
}
