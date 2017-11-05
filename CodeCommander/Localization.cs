using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodeCommander
{
    internal partial class Localization : Form
    {
        public Localization()
        {
            InitializeComponent();
        }

        private void Localization_Load(object sender, EventArgs e)
        {
            this.web.Navigate(Documents.LocalizationPage);
        }

        private void Localization_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}