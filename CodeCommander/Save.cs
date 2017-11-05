using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CodeCommander
{
    public partial class Save : Form
    {
        #region Private Fields
        private string _fileName;
        #endregion

        public Save()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get { return this._fileName; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.lvFiles.Items.Clear();
            DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\sources");
            foreach (FileInfo fi in di.GetFiles("*.xml"))
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = fi.Name;
                lvi.SubItems.Add("1/1/1");
                lvi.SubItems.Add("1/1/1");
                lvi.SubItems.Add("1");
                this.lvFiles.Items.Add(lvi);
            }
        }

        private void btnSuppr_Click(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            this._fileName = this.lvFiles.SelectedItems[0].Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lvFiles.SelectedIndices.Count > 0)
            {
                this.btnErase.Enabled = true;
                this.btnSuppr.Enabled = true;
            }
            else
            {
                this.btnErase.Enabled = false;
                this.btnSuppr.Enabled = false;
            }
        }

        private void Open_Load(object sender, EventArgs e)
        {
            this.btnRefresh_Click(sender, e);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this._fileName = "result3.xml";
        }
    }
}