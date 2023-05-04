using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace SimpleDebugger
{
    internal static class Program
    {
        private const string TargetProcessName = "LeagueClientUx.exe";

        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Program.Menu();
            }
            else
            {
                Program.AllocConsole();
                string lpApplicationName = args[0];
                string input = "\"" + ((IEnumerable<string>)args).Skip<string>(1).Aggregate<string>((Func<string, string, string>)((x, y) => x + "\" \"" + y)) + "\"";
                string str = Regex.Match(input, "(\"--remoting-auth-token=)([^\"]*)(\")").Groups[2].Value;
                Console.WriteLine("Using Port: " + int.Parse(Regex.Match(input, "(\"--app-port=)([^\"]*)(\")").Groups[2].Value).ToString());
                Console.WriteLine("Using Auth: " + str);
                Console.WriteLine("CurrentDir: " + Environment.CurrentDirectory);
                Console.WriteLine(new string('-', Console.BufferWidth - 1));
                Console.WriteLine("App: " + lpApplicationName);
                Console.WriteLine("Args: " + input);
                string lpCommandLine = input.Replace("\"--no-proxy-server\"", "");
                Console.WriteLine(new string('-', Console.BufferWidth - 1));
                Console.WriteLine("App: " + lpApplicationName);
                Console.WriteLine("Args: " + lpCommandLine);
                Program.STARTUPINFO lpStartupInfo = new Program.STARTUPINFO();
                Program.PROCESS_INFORMATION lpProcessInformation;


                //if (!Program.CreateProcess(lpApplicationName, lpCommandLine, IntPtr.Zero, IntPtr.Zero, false, 2U, IntPtr.Zero, (string) null, ref lpStartupInfo, out lpProcessInformation))
                //    throw new Win32Exception();


                try
                {
                    Program.CreateProcess(lpApplicationName, lpCommandLine, IntPtr.Zero, IntPtr.Zero, true, 1U, IntPtr.Zero, (string)null, ref lpStartupInfo, out lpProcessInformation);
                    Program.DebugActiveProcessStop(lpProcessInformation.dwProcessId);
                    Process.GetProcessById(lpProcessInformation.dwProcessId).WaitForExit();
                }
                catch (Exception ex)
                { 
                    
                    Console.WriteLine(ex.ToString()); 
                
                }


                Console.WriteLine("LeagueClientUx.exe quit.");
                Thread.Sleep(3000);
            }
        }

        private static void Menu()
        {
            Program.AllocConsole();
            RegistryKey registryKey = (RegistryKey)null;
            string location = Assembly.GetExecutingAssembly().Location;
            try
            {
                registryKey = Registry.LocalMachine.CreateSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\LeagueClientUx.exe");
            }
            catch (UnauthorizedAccessException ex)
            {
                try
                {
                    new Process()
                    {
                        StartInfo = {
              FileName = Assembly.GetExecutingAssembly().Location,
              UseShellExecute = true,
              Verb = "runas"
            }
                    }.Start();
                    Environment.Exit(0);
                }
                catch
                {
                    Console.WriteLine("Access denied.");
                    Thread.Sleep(1000);
                    Environment.Exit(1);
                }
            }
            int num = 0;
        label_31:
            Console.Clear();
            Console.ResetColor();
            Console.CursorVisible = false;
            Console.Write("Currently hooked to: ");
            string str = (registryKey?.GetValue("debugger") ?? (object)"Nothing.").ToString().Replace(location, "This program.");
            if (str == "This program.")
                Console.ForegroundColor = ConsoleColor.Green;
            else if (str != "Nothing.")
                Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            if (num == 0)
                Console.ForegroundColor = ConsoleColor.White;
            else
                Console.ResetColor();
            Console.WriteLine((num == 0 ? "-->" : "   ") + " Register LeagueClientUx.exe debugger IEFO.");
            if (num == 1)
                Console.ForegroundColor = ConsoleColor.White;
            else
                Console.ResetColor();
            Console.WriteLine((num == 1 ? "-->" : "   ") + " Unregister LeagueClientUx.exe debugger IEFO.");
            if (num == 2)
                Console.ForegroundColor = ConsoleColor.White;
            else
                Console.ResetColor();
            Console.WriteLine((num == 2 ? "-->" : "   ") + " Exit.");
            bool flag = false;
            while (!flag)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Enter:
                        flag = true;
                        switch (num)
                        {
                            case 0:
                                if (registryKey != null)
                                {
                                    registryKey.SetValue("debugger", (object)location);
                                    continue;
                                }
                                continue;
                            case 1:
                                if (registryKey != null)
                                {
                                    registryKey.DeleteValue("debugger");
                                    continue;
                                }
                                continue;
                            case 2:
                                Environment.Exit(0);
                                continue;
                            default:
                                continue;
                        }
                    case ConsoleKey.UpArrow:
                        if (num != (num = Math.Max(num - 1, 0)))
                        {
                            flag = true;
                            continue;
                        }
                        continue;
                    case ConsoleKey.DownArrow:
                        if (num != (num = Math.Min(num + 1, 2)))
                        {
                            flag = true;
                            continue;
                        }
                        continue;
                    default:
                        continue;
                }
            }
            goto label_31;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DebugActiveProcessStop([In] int Pid);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CreateProcess(
          string lpApplicationName,
          string lpCommandLine,
          IntPtr lpProcessAttributes,
          IntPtr lpThreadAttributes,
          bool bInheritHandles,
          uint dwCreationFlags,
          IntPtr lpEnvironment,
          string lpCurrentDirectory,
          [In] ref Program.STARTUPINFO lpStartupInfo,
          out Program.PROCESS_INFORMATION lpProcessInformation);

        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        private struct STARTUPINFO
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
    }
}
