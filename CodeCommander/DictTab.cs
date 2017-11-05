using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CodeCommander
{
    public partial class DictTab : Form
    {
        #region Private Fields
        private o2Mate.Dictionnaire _dict;
        private string _fileName;
        #endregion

        #region Public Constructor
        public DictTab(string fileName, o2Mate.Dictionnaire dict)
        {
            this._fileName = fileName;
            this._dict = dict;
            InitializeComponent();
        }
        #endregion

        #region Private Methods
        private void Construct()
        {
            foreach (string key in this._dict.StringKeys)
            {
                Label l = new Label();
                l.Text = key;
                this.layout.Controls.Add(l);
                TextBox t = new TextBox();
                t.Name = key;
                t.Text = this._dict.GetString(key);
                t.TextChanged += new EventHandler(t_TextChanged);
                this.layout.Controls.Add(t);
            }
            foreach (string key in this._dict.ArrayKeys)
            {
                Label l = new Label();
                l.Text = key;
                this.layout.Controls.Add(l);
                TabUC uc = new TabUC(this._dict.GetArray(key) as o2Mate.Array);
                uc.Name = key;
                this.layout.Controls.Add(uc);
            }
        }

        void t_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox;
            this._dict.SetString(t.Name, t.Text);
        }
        #endregion

        private void DictTab_Load(object sender, EventArgs e)
        {
            this.Construct();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            this._dict = new o2Mate.Dictionnaire();
            this._dict.Load(AppDomain.CurrentDomain.BaseDirectory + "\\dict\\dict.xml");
            this.layout.Controls.Clear();
            this.Construct();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            this._dict = new o2Mate.Dictionnaire();
            this._dict.Load(AppDomain.CurrentDomain.BaseDirectory + "\\dict\\dict.xml");
            this.layout.Controls.Clear();
            this.Construct();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this._dict.Save(AppDomain.CurrentDomain.BaseDirectory + "\\dict\\dict.xml");
        }

        private void btnFermer_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            this._dict.Save(AppDomain.CurrentDomain.BaseDirectory + "\\dict\\" + this._fileName);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            try
            {
                comp.Compilation(AppDomain.CurrentDomain.BaseDirectory + "\\sources\\" + this._fileName, AppDomain.CurrentDomain.BaseDirectory + "\\dict\\" + this._fileName, AppDomain.CurrentDomain.BaseDirectory + "\\files\\" + System.IO.Path.GetFileNameWithoutExtension(this._fileName) + ".txt");
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
            Process proc = new Process();
            proc.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\files\\" + System.IO.Path.GetFileNameWithoutExtension(this._fileName) + ".txt";
            proc.Start();
            this.Close();
        }
    }
}