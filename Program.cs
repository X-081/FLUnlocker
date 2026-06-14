using System.Diagnostics;

namespace FLUnlocker
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length != 0)
                {
                    string file = args[0];
                    byte[] bytes = File.ReadAllBytes(file);
                    byte[] change = [0x46, 0x4C, 0x68, 0x64, 0x06, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x49, 0x00, 0x60, 0x00, 0x46, 0x4C,
                        0x64, 0x74, 0xA8, 0x82, 0x0B, 0x00, 0xC7, 0x05,
                        0x32, 0x30, 0x2E, 0x30, 0x00, 0x9F, 0x00, 0x00,
                        0x00, 0x00, 0x1C, 0x03, 0x25, 0x01, 0xC8, 0x24];
                    if (bytes.Length >= change.Length)
                    {
                        for (int i = 0;i < change.Length;i++) bytes[i] = change[i];
                        File.WriteAllBytes(file, bytes);
                    }
                    string flpath = GetFLPath();
                    var psi = new ProcessStartInfo(flpath, file);
                    Process.Start(psi);
                    return;
                }
                else
                {
                    Console.WriteLine("输入你的FL版本（如20）");
                    string version = Console.ReadLine()!;
                    Console.WriteLine("[0] 解锁FL");
                    Console.WriteLine("[其他键] 取消解锁FL");
                    if (Console.ReadKey().Key == ConsoleKey.D0)
                    {
                        Console.WriteLine("\n正在解锁");
                        SetReg(version, Environment.ProcessPath!);
                        Console.WriteLine("已写入注册表");
                        Console.WriteLine("解锁完成");
                    }
                    else
                    {
                        Console.WriteLine("\n正在取消解锁");
                        SetReg(version, GetFLPath());
                        Console.WriteLine("取消解锁完成");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("程序出现错误");
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("按任意键退出");
            Console.ReadKey();
        }
        public static void SetReg(string version,string path)
        {
            string command = $"\"{path}\" \"%1\"";
            SetRegisterKey($"HKCR\\FL32.flp.{version}\\shell\\open\\command", command);
            SetRegisterKey($"HKCR\\FL64.flp.{version}\\shell\\open\\command", command);
        }
        public static string GetFLPath()
        {
            string tmppath = "C:\\Program Files\\FLPath.txt";
            string path;
            if (!File.Exists(tmppath))
            {
                Console.WriteLine("输入你的FL的程序路径（如D;/FLStudio/FL64.exe）");
                path = Console.ReadLine()!;
                if (!File.Exists(path))
                {
                    Console.WriteLine("文件不存在");
                    return GetFLPath();
                }
                File.WriteAllText(tmppath, path);
                return path;
            }
            else
            {
                path = File.ReadAllText(tmppath);
                if (!File.Exists(path)) return GetFLPath();
                else return path;
            }
        }
        public static void SetRegisterKey(string path, string value)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c reg add \"{path}\" /ve /d \"{value.Replace("\"", "\\\"")}\" /f",
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true
            };
            Process.Start(psi)?.WaitForExit();
        }
    }
}