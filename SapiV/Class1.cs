using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using VelocityAPI;

/* 
Made by Sonar.

Join the Saturn X discord server! discord.gg/PHWymfWpr4
 */

namespace SapiV
{
    [SupportedOSPlatform("windows")]
    public static class Api
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();
        private static readonly VelAPI velApi = new VelAPI();
        private static bool _isInjected;
        private static string? LuaScriptNotification;
        private static string? LuaScriptUserAgent;
        private static string? LuaScriptNameExecutor;
        private static string? Velocity_Version;
        private static readonly Regex KickRegex = new Regex(
            @"(\w+)\s*[:\.]\s*Kick\s*\(([^)]*)\)|" +
            @"(\w+)\s*\[\s*[""']Kick[""']\s*\]\s*\(([^)]*)\)|" +
            @"(\w+)\s*\.\s*Kick\s*\=\s*function|" +
            @"kick\s*\=\s*function",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );
        private const string Msg = @" __           
(_  _  _ .\  /
__)(_||_)| \/ 
      |       


discord.gg/bDMtpCnx3K";
        public struct ClientInfo
        {
            public int Id;
            public string Name;
            public string Version;
            public int State;
        }


        //⬇️⬇️⬇️⬇️------------------------------ PUBLIC FUNCTIONS ------------------------------⬇️⬇️⬇️⬇️


        public static bool EnableConsole { get; set; } = true;
        private static void Output(string output)
        {
            if (EnableConsole)
            {
                Console.Write(output);
            }
        }
        public static async Task Inject()
        {
            var robloxProcs = Process.GetProcessesByName("RobloxPlayerBeta");
            if (robloxProcs.Length == 0)
            {
                _isInjected = false;
                return;
            }
            var proc = robloxProcs.FirstOrDefault();
            if (proc == null)
            {
                _isInjected = false;
                return;
            }
            await FileManager();
            string autoExecPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoExec");
            string filePath = Path.Combine(autoExecPath, "CustomNotification.lua");
            File.WriteAllText(filePath, LuaScriptNotification);
            Thread.Sleep(500);
            var status = await velApi.Attach(proc.Id);
            try
            {
                if (status == VelocityStates.Attached)
                {
                    _isInjected = true;
                    velApi.Execute(LuaScriptNotification);
                    await Task.Delay(5000).ContinueWith(_ =>
                    {
                        try
                        {
                            if (status == VelocityStates.Attached)
                                File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            AllocConsole();
                            Output($"==> Cannot delete the file: {ex.Message}");
                            Thread.Sleep(5000);
                            FreeConsole();
                        }
                    });
                }
                else
                {
                    _isInjected = false;
                    velApi.StopCommunication();
                    await Task.Delay(5000).ContinueWith(_ =>
                    {
                        try
                        {
                            if (status == VelocityStates.Attached)
                                File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                            AllocConsole();
                            Output($"==> Cannot delete the file: {ex.Message}");
                            Thread.Sleep(5000);
                            FreeConsole();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _isInjected = false;
                velApi.StopCommunication();
                await Task.Delay(5000).ContinueWith(_ =>
                {
                    try
                    {
                        if (status == VelocityStates.Attached)
                            File.Delete(filePath);
                    }
                    catch
                    {
                        AllocConsole();
                        Output($"==> Cannot delete the file: {ex.Message}");
                        Thread.Sleep(5000);
                        FreeConsole();
                    }
                });
                AllocConsole();
                Output($"==> Injection failed: {ex.Message}");
                Thread.Sleep(5000);
                FreeConsole();
            }
        }

        public static bool IsInjected() => _isInjected && velApi.VelocityStatus == VelocityStates.Attached;

        public static bool IsRobloxOpen()
        {
            return Process.GetProcessesByName("RobloxPlayerBeta").Any();
        }

        public static void KillRoblox()
        {
            foreach (var proc in Process.GetProcessesByName("RobloxPlayerBeta"))
            {
                try
                {
                    proc.Kill();
                }
                catch (Exception ex)
                {
                    AllocConsole();
                    Output($"==> Kill process failed: {ex.Message}");
                    Thread.Sleep(5000);
                    FreeConsole();
                }
            }
            velApi.StopCommunication();
        }

        public static void Execute(string scriptSource)
        {
            if (velApi.injected_pids.Count == 0)
            {
                return;
            }
            string sanitized = AntiKick(scriptSource);
            StringBuilder combined = new StringBuilder();
            if (!string.IsNullOrEmpty(LuaScriptUserAgent)) { combined.AppendLine(LuaScriptUserAgent); combined.AppendLine(); }
            if (!string.IsNullOrEmpty(LuaScriptNameExecutor)) { combined.AppendLine(LuaScriptNameExecutor); combined.AppendLine(); }
            combined.AppendLine(sanitized);
            string fullScript = combined.ToString();
            try
            {
                var status = velApi.Execute(fullScript);
            }
            catch (Exception ex)
            {
                AllocConsole();
                Output($"==> Execution failed: {ex.Message}");
                Output("==> Press any key to continue...");
                Console.ReadKey();
                FreeConsole();
            }
        }

        public static void SetCustomInjectionNotification(string title, string text, string idIcon, string duration)
        {
            if (string.IsNullOrEmpty(title)) title = "Saturn Api";
            if (string.IsNullOrEmpty(text)) text = "Injected Successfully!";
            if (string.IsNullOrEmpty(idIcon)) idIcon = "";
            if (string.IsNullOrEmpty(duration)) duration = "5";
            LuaScriptNotification = $@"
            game.StarterGui:SetCore(""SendNotification"", {{
                Title = ""{title}"",
                Text = ""{text}"",
                Icon = ""rbxassetid://{idIcon}"",
                Duration = {duration}
            }})";
        }

        public static void SetCustomUserAgent(string Name)
        {
            if (string.IsNullOrEmpty(Name)) Name = "Saturn Api";
            LuaScriptUserAgent = $@"oldr = request
            getgenv().request = function(options)
                if options.Headers then
                    options.Headers[""User-Agent""] = ""{Name}""
                else
                    options.Headers = {{[""User-Agent""] = ""{Name}""}}
                end
                local response = oldr(options)
                return response
            end
            request = getgenv().request";
        }

        public static void SetCustomNameExecutor(string Name, string Version)
        {
            if (string.IsNullOrEmpty(Name)) Name = "Saturn Api";
            if (string.IsNullOrEmpty(Version)) Version = "v1.0.0";
            LuaScriptNameExecutor = $@"local Name = ""{Name}""
            local Version = ""{Version}""
            getgenv().identifyexecutor = function()
                return Name, Version
            end
            getgenv().getexecutorname = function()
                return Name
            end
            getgenv().getexploitname = function()
                return Name
            end";
        }

        public static List<ClientInfo> GetClientsList()
        {
            var clients = new List<ClientInfo>();
            foreach (int pid in velApi.injected_pids)
            {
                if (IsProcessRunning(pid))
                {
                    var proc = Process.GetProcessById(pid);
                    clients.Add(new ClientInfo
                    {
                        Id = pid,
                        Name = proc.ProcessName,
                        Version = "Unknown",
                        State = 3
                    });
                }
            }
            return clients;
        }


        //⬇️⬇️⬇️⬇️------------------------------ PRIVATE FUNCTIONS ------------------------------⬇️⬇️⬇️⬇️


        private static async Task<bool> CheckUpdate()
        {
            try
            {
                string versionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "current_version.txt");
                string localVersion = "";
                if (File.Exists(versionPath))
                {
                    localVersion = (await File.ReadAllTextAsync(versionPath)).Trim();
                }
                else
                {
                    return true;
                }

                using HttpClient client = new HttpClient();
                string remoteVersion = (await client.GetStringAsync("https://realvelocity.xyz/assets/current_version.txt")).Trim();
                Velocity_Version = remoteVersion;
                bool isUpdateNeeded = localVersion != remoteVersion;
                return isUpdateNeeded;
            }
            catch (Exception ex)
            {
                Output($"\n==> Error checking for updates: {ex.Message}");
                return true;
            }
        }

        private static bool CheckFiles()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string autoExecPath = Path.Combine(basePath, "AutoExec");
                string binPath = Path.Combine(basePath, "Bin");
                string scriptsPath = Path.Combine(basePath, "Scripts");
                string workspacePath = Path.Combine(basePath, "Workspace");
                string decompilerPath = Path.Combine(basePath, "Bin", "Decompiler.exe");
                string injectorPath = Path.Combine(basePath, "Bin", "erto3e4rortoergn.exe");
                string versionPath = Path.Combine(basePath, "Bin", "current_version.txt");

                bool filesMissing = !Directory.Exists(autoExecPath) ||
                                   !Directory.Exists(binPath) ||
                                   !Directory.Exists(scriptsPath) ||
                                   !Directory.Exists(workspacePath) ||
                                   !File.Exists(decompilerPath) ||
                                   !File.Exists(injectorPath) ||
                                   !File.Exists(versionPath);
                return filesMissing;
            }
            catch (Exception ex)
            {
                AllocConsole();
                Output($"==> Error in checkfiles: {ex.Message}");
                Thread.Sleep(5000);
                FreeConsole();
                return true;
            }
        }
        private static bool IsProcessRunning(int pid)
        {
            try
            {
                Process.GetProcessById(pid); return true;
            }
            catch (Exception ex)
            {
                AllocConsole();
                Output($"==> error in IsProcessRunning: {ex.Message}");
                Thread.Sleep(5000);
                FreeConsole();
                return false;
            }
        }

        static Api()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            SetCustomInjectionNotification("Saturn Api", "Injected Successfully!", "", "5");
        }

        private static void OnProcessExit(object? sender, EventArgs e)
        {
            velApi.StopCommunication();
            string Notification = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutoExec", "CustomNotification.lua");
            File.Delete(Notification);
        }

        private static string AntiKick(string source)
        {
            return KickRegex.Replace(source, "print('[SapiV] Kick function blocked.')");
        }

        private static async Task FileManager()
        {
            bool needsDownload = CheckFiles();
            bool needsUpdate = false;

            if (!needsDownload)
            {
                needsUpdate = await CheckUpdate();
            }

            if (needsDownload)
            {
                if (EnableConsole)
                {
                    AllocConsole();
                    Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                    Console.Title = "SapiV - Velocity files missed";
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Output(Msg);
                    Output("\n==> We are downloading the latest files for Velocity...");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Output("\n==> Disable the antivirus, otherwise the files may not download correctly!!\n==> DO NOT CLOSE THIS WINDOW!");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }

                try
                {
                    await Task.Run(() => velApi.StartCommunication());
                    int maxWaitSeconds = 120;
                    int waited = 0;
                    while (CheckFiles() && waited < maxWaitSeconds)
                    {
                        await Task.Delay(1000);
                        waited++;
                    }

                    if (CheckFiles())
                    {
                        Output("\n!!! Files not downloaded correctly !!!");
                        Output("\nPress any key to close...");
                        Console.ResetColor();
                        await Task.Delay(5000);
                        FreeConsole();
                    }
                    else
                    {
                        Output("\n==> Files downloaded successfully!");
                        if (EnableConsole)
                        {
                            Console.ResetColor();
                            await Task.Delay(1000);
                            FreeConsole();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Output($"\n==> Error during file download: {ex.Message}");
                    Output("\n!!! Files not downloaded correctly due to error !!!");
                    await Task.Delay(5000);
                    FreeConsole();
                }
            }
            else if (needsUpdate)
            {
                if (EnableConsole)
                {
                    AllocConsole();
                    Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                    Console.Title = "SapiV - Velocity update";
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Output(Msg);
                    Output($"\n==> A new update of Velocity has been detected! [{Velocity_Version}]");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Output("\n==> Disable the antivirus, otherwise the files may not download correctly!!\n==> DO NOT CLOSE THIS WINDOW!");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                }

                try
                {
                    await Task.Run(() => velApi.StartCommunication());
                    int maxWaitSeconds = 120;
                    int waited = 0;
                    while ((CheckFiles() || await CheckUpdate()) && waited < maxWaitSeconds)
                    {
                        await Task.Delay(1000);
                        waited++;
                    }

                    if (CheckFiles() || await CheckUpdate())
                    {
                        Output("\n!!! Files not updated correctly !!!");
                        await Task.Delay(5000);
                        FreeConsole();
                    }
                    else
                    {
                        Output("\n==> Update completed successfully!");
                        if (EnableConsole)
                        {
                            Console.ResetColor();
                            await Task.Delay(1000);
                            FreeConsole();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Output($"\n==> Error during update: {ex.Message}");
                    Output("\n!!! Files not updated correctly due to error !!!");
                    await Task.Delay(5000);
                    FreeConsole();
                }
            }
            else
            {
                if (EnableConsole)
                {
                    FreeConsole();
                }
            }
        }
    }
}