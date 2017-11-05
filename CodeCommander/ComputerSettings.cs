using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Permissions;
using System.Security.Principal;
using System.Security;
using System.Security.AccessControl;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Resources;

namespace CodeCommander
{
    internal partial class ComputerSettings : Form
    {
        private bool powershellOk;
        private bool needToChange;
        private int? val1;
        private int? val2;
        private List<KeyValuePair<string, string>> coerce;

        public ComputerSettings()
        {
            InitializeComponent();
            this.coerce = new List<KeyValuePair<string, string>>();
            this.coerce.Add(new KeyValuePair<string, string>("English", "en-US"));
            this.coerce.Add(new KeyValuePair<string, string>("Français", "fr-FR"));
            this.val1 = null;
            this.val2 = null;
            this.needToChange = false;
            this.powershellOk = false;
        }

        public void VerifyPowerShell()
        {
            try
            {
                // pour savoir si powershell est installé, il suffit de le lancer
                Process proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.FileName = "powershell.exe";
                proc.StartInfo.Arguments = " -Version -NoLogo -NoInteractive -Command exit;";
                proc.Start();
                proc.WaitForExit();
                this.powershellOk = true;
            }
            catch
            {
                this.powershellOk = false;
            }
        }

        public void VerifyExecutingPowerShell()
        {
            try
            {
                RegistryKey key1 = Registry.ClassesRoot.OpenSubKey(@".ps1");
                if (key1 != null)
                {
                    string reference = (string)key1.GetValue("");
                    RegistryKey key2 = Registry.ClassesRoot.OpenSubKey(reference + @"\shell");
                    if (key2 != null)
                    {
                        foreach (string next in key2.GetSubKeyNames())
                        {
                            if (next.StartsWith("Ex"))
                            {
                                global::CodeCommander.Properties.Settings.Default.ExecPowerShellVerb = next;
                                return;
                            }
                        }
                    }
                    key1.Close();
                }
            }
            catch { }
        }

        public void TestChangeParameterSettings()
        {
            try
            {
                RegistryKey key1 = Registry.CurrentUser.OpenSubKey(@"SoftWare\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\0");
                if (key1 != null)
                {
                    this.val1 = (int)key1.GetValue("1200");
                    this.val2 = (int)key1.GetValue("1201");
                    if ((this.val1.HasValue && this.val1.Value != 0) || (this.val2.HasValue && this.val2.Value != 0))
                    {
                        this.needToChange = true;
                        this.checkBox1.Enabled = true;
                        this.label1.Visible = false;
                        this.label3.Visible = true;
                    }
                    else
                    {
                        this.needToChange = false;
                        this.checkBox1.Enabled = false;
                        this.label1.Visible = true;
                        this.label3.Visible = false;
                    }
                    key1.Close();
                }
            }
            catch
            {
                this.needToChange = true;
                this.checkBox1.Enabled = true;
                this.label1.Visible = false;
                this.label3.Visible = true;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.needToChange && this.checkBox1.Checked)
            {
                try
                {
                    RegistryKey key1 = Registry.CurrentUser.OpenSubKey(@"SoftWare\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\0", true);
                    if (key1 != null)
                    {
                        key1.SetValue("1200", 0);
                        key1.SetValue("1201", 0);
                        key1.Close();
                    }
                }
                catch { }
            }
            // verification présence PowerShell
            global::CodeCommander.Properties.Settings.Default.PowerShellInstalled = this.powershellOk;
            // langue choisie
            global::CodeCommander.Properties.Settings.Default.CurrentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            // exécution de scripts
            if (this.checkBox2.Checked)
            {
                global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms = true;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        internal void RevertToSaved()
        {
            // si on a accepté de lancer l'application et que la checkbox était cochée
            if (this.needToChange && this.DialogResult == DialogResult.OK && this.checkBox1.Checked)
            {
                if ((!this.val1.HasValue || this.val1.Value != 0) || (!this.val2.HasValue || this.val2.Value != 0))
                {
                    try
                    {
                        RegistryKey key1 = Registry.CurrentUser.OpenSubKey(@"SoftWare\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\0", true);
                        if (key1 != null)
                        {
                            if (!this.val1.HasValue)
                            {
                                key1.DeleteValue("1200");
                            }
                            else
                            {
                                key1.SetValue("1200", this.val1);
                            }
                            if (!this.val2.HasValue)
                            {
                                key1.DeleteValue("1201");
                            }
                            else
                            {
                                key1.SetValue("1201", this.val2);
                            }
                            key1.Close();
                        }
                    }
                    catch { }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindingContext[this].SuspendBinding();
            KeyValuePair<string, string> current = (KeyValuePair<string, string>)this.comboBox1.SelectedItem;
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(current.Value);
            this.Controls.Clear();
            InitializeComponent();
            if (this.needToChange)
            {
                this.checkBox1.Enabled = true;
                this.label1.Visible = true;
                this.label3.Visible = false;
            }
            else
            {
                this.checkBox1.Enabled = false;
                this.label1.Visible = false;
                this.label3.Visible = true;
            }
            if (this.powershellOk)
            {
                this.label3NotOk.Visible = false;
                this.label3Ok.Visible = true;
            }
            else
            {
                this.label3Ok.Visible = false;
                this.label3NotOk.Visible = true;
            }
            this.comboBox1.DisplayMember = "Key";
            this.comboBox1.ValueMember = "Value";
            this.comboBox1.DataSource = this.coerce;
            this.comboBox1.SelectedItem = current;
            this.comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
            this.BindingContext[this].ResumeBinding();
        }

        private void ComputerSettings_Load(object sender, EventArgs e)
        {
            if (this.powershellOk)
            {
                this.label3NotOk.Visible = false;
                this.label3Ok.Visible = true;
            }
            else
            {
                this.label3Ok.Visible = false;
                this.label3NotOk.Visible = true;
            }
            if (this.needToChange)
            {
                this.checkBox1.Enabled = true;
                this.label1.Visible = true;
                this.label3.Visible = false;
            }
            else
            {
                this.checkBox1.Enabled = false;
                this.label1.Visible = false;
                this.label3.Visible = true;
            }
            this.comboBox1.DisplayMember = "Key";
            this.comboBox1.ValueMember = "Value";
            this.comboBox1.DataSource = this.coerce;
            foreach (KeyValuePair<string, string> pair in this.coerce)
            {
                if (System.Threading.Thread.CurrentThread.CurrentUICulture.Name == pair.Value)
                {
                    this.comboBox1.SelectedItem = pair;
                }
            }
            this.comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
        }
    }
}