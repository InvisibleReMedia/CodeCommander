using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CodeCommander
{
    public partial class TabUC : UserControl
    {
        #region Private Fields
        private o2Mate.Array _arr;
        #endregion

        public TabUC(o2Mate.Array arr)
        {
            this._arr = arr;
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnSuppr_Click(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }

        private void btnUp_Click(object sender, EventArgs e)
        {

        }

        private void btnDown_Click(object sender, EventArgs e)
        {

        }

        private void tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tab.SelectedIndex != -1)
            {
                this.btnDown.Enabled = true;
                this.btnEdit.Enabled = true;
                this.btnSuppr.Enabled = true;
                this.btnUp.Enabled = true;
            }
            else
            {
                this.btnDown.Enabled = false;
                this.btnEdit.Enabled = false;
                this.btnSuppr.Enabled = false;
                this.btnUp.Enabled = false;
            }
        }
    }
}
