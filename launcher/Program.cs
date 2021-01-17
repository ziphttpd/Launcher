using System.Diagnostics;
using System.Windows.Forms;
using launcher.Properties;
using System;
using System.IO;
using System.Reflection;

namespace launcher {
    public class Param {
        public string[] Args { get; set; }
        public string Exe { get; set; }
        public string Updater { get; set; }
        public string Zhget { get; set; }
        public string Launcher { get; set; }
        public string Selector { get; set; }
        public string Text { get; set; }
        private string configPath;
        private string logPath;
        public string Port { get; set; }
        public string DocPort { get; set; }
        public string SelPort { get; set; }
        public Param(string[] args) {
            string[] param;
            Args = args;
            string arg0 = args.Length > 0 ? args[0] : "";
            param = (arg0 + ";;;;;").Split(';');

            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Exe = Path.Combine(path, "ziphttpd.exe");
            Updater = Path.Combine(path, "updater.exe");
            Zhget = Path.Combine(path, "zhget.exe");
            Launcher = Path.Combine(path, "launcher.exe");
            Selector = Path.Combine(path, "selector.exe");

            //            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            Text = param[0].Trim();
            if (Text == "") { Text = @"ZipHttpd"; }

            configPath = Environment.ExpandEnvironmentVariables(param[1].Trim());
            if (configPath == "") { configPath = path; }

            logPath = Environment.ExpandEnvironmentVariables(param[2].Trim());
            if (logPath == "") { logPath = Path.Combine(configPath, "log"); }

            Port = Environment.ExpandEnvironmentVariables(param[3].Trim());
            if (Port == "") { Port = "8823"; }

            DocPort = Environment.ExpandEnvironmentVariables(param[4].Trim());
            if (DocPort == "") { DocPort = "58823"; }

            SelPort = Environment.ExpandEnvironmentVariables(param[5].Trim());
            if (SelPort == "") { SelPort = "8822"; }
        }
        public string arguments() {
            return string.Format(@"-config {0} -log {1} -port {2} -docport {3}", configPath, logPath, Port, DocPort);
        }
        public string selarguments() {
            return string.Format(@"-config {0} -port {1}", configPath, SelPort);
        }
    }
    public class nicon {
        NotifyIcon icon = new NotifyIcon();
        Process process;
        Process selprocess;
        Param param;
        nicon(string[] args) {
            param = new Param(args);
            icon.Text = param.Text;
            icon.Icon = Resources.favicon;
            icon.ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Update", delegate { update(); } ),
                new MenuItem("Selector", delegate { select(); } ),
                new MenuItem("Show", delegate { show(); } ),
                new MenuItem("Restart", delegate { restart(); } ),
                new MenuItem("Exit", delegate { exit(); } ),
            });
            icon.DoubleClick += delegate { show(); };
            start();
            icon.Visible = true;
        }
        void show() {
            System.Diagnostics.Process.Start("http://localhost:" + param.Port + "/");
        }
        void start() {
            if (process == null) {
                string nowExe = param.Exe + ".now";
                try {
                    File.Delete(nowExe);
                    File.Copy(param.Exe, nowExe);
                } catch (Exception) { /* noop */}
                process = new Process();
                process.StartInfo.FileName = nowExe;
                process.StartInfo.Arguments = param.arguments();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.OutputDataReceived += OutputDataReceived;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
            }
        }
        bool stop() {
            var running = process != null;
            if (running) {
                process.StandardInput.WriteLine("quit");
                process.WaitForExit();
                process = null;
            }
            return running;
        }
        void update() {
            stop();

            // ZipHttpd の更新プログラムのダウンロード
            Process zhget = new Process();
            zhget.StartInfo.FileName = param.Zhget;
            zhget.StartInfo.Arguments = "-host ziphttpd.com -group windows";
            zhget.StartInfo.UseShellExecute = false;
            zhget.StartInfo.RedirectStandardInput = false;
            zhget.StartInfo.CreateNoWindow = true;
            zhget.Start();
            zhget.WaitForExit();

            // ZipHttpd v1 自体のドキュメントのダウンロード
            zhget = new Process();
            zhget.StartInfo.FileName = param.Zhget;
            zhget.StartInfo.Arguments = "-host ziphttpd.com -group ziphttpd-V1";
            zhget.StartInfo.UseShellExecute = false;
            zhget.StartInfo.RedirectStandardInput = false;
            zhget.StartInfo.CreateNoWindow = true;
            zhget.Start();
            zhget.WaitForExit();

            // 更新ドキュメントのダウンロード
            zhget = new Process();
            zhget.StartInfo.FileName = param.Zhget;
            zhget.StartInfo.UseShellExecute = false;
            zhget.StartInfo.RedirectStandardInput = false;
            zhget.StartInfo.CreateNoWindow = true;
            zhget.Start();
            zhget.WaitForExit();

            // プログラムのアップデート
            Process updater = new Process();
            updater.StartInfo.FileName = param.Updater;
            updater.StartInfo.UseShellExecute = false;
            updater.StartInfo.RedirectStandardInput = false;
            updater.StartInfo.CreateNoWindow = true;
            updater.Start();
            updater.WaitForExit();

            // 自身を再起動
            Process mine = new Process();
            mine.StartInfo.FileName = param.Launcher;
            mine.StartInfo.Arguments = String.Join(" ", param.Args);
            mine.StartInfo.UseShellExecute = false;
            mine.Start();

            Application.Exit();
        }
        void select() {
            if (selprocess == null) {
                string nowExe = param.Selector + ".now";
                try {
                    File.Delete(nowExe);
                    File.Copy(param.Selector, nowExe);
                } catch (Exception) { /* noop */}
                selprocess = new Process();
                selprocess.StartInfo.FileName = nowExe;
                selprocess.StartInfo.Arguments = param.selarguments();
                selprocess.StartInfo.UseShellExecute = false;
                selprocess.StartInfo.RedirectStandardInput = true;
                selprocess.OutputDataReceived += OutputDataReceived;
                selprocess.StartInfo.CreateNoWindow = true;
                selprocess.Start();
            }
            System.Diagnostics.Process.Start("http://localhost:" + param.SelPort + "/");
        }
        void restart() {
            stop();
            start();
        }
        void exit() {
            stop();
            Application.Exit();
        }
        void OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e) {
            // 読み捨て
        }
        [System.STAThread]
        public static void Main(string[] args) {
            nicon inst = new nicon(args);
            Application.Run();
            inst.icon.Dispose();
        }
    }
}
