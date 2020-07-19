using System.Diagnostics;
using System.Windows.Forms;
using launcher.Properties;
using System;
using System.IO;
using System.Reflection;

namespace launcher {
    public class Param {
        public string Exe { get; set; }
        public string Text { get; set; }
        private string configPath;
        private string logPath;
        public string Port { get; set; }
        public string DocPort { get; set; }
        public Param(string[] args) {
            string[] param;
            string arg0 = args.Length > 0 ? args[0] : "";
            param = (arg0 + ";;;;").Split(';');

            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Exe = Path.Combine(path, "ziphttpd.exe");

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
        }
        public string arguments() {
            return string.Format(@"-config {0} -log {1} -port {2} -docport {3}", configPath, logPath, Port, DocPort);
        }
    }
    public class nicon {
        NotifyIcon icon = new NotifyIcon();
        Process process;
        Param param;
        nicon(string[] args) {
            param = new Param(args);
            icon.Text = param.Text;
            icon.Icon = Resources.favicon;
            icon.ContextMenu = new ContextMenu(new MenuItem[] {
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
        void stop() {
            if (process != null) {
                process.StandardInput.WriteLine("quit");
                process.WaitForExit();
                process = null;
            }
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
