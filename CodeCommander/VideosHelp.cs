using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodeCommander
{
    internal partial class VideosHelp : Form
    {
        public VideosHelp()
        {
            InitializeComponent();
        }

        private void VideosHelp_Load(object sender, EventArgs e)
        {
            this.videoWeb.Navigate(Documents.VideoPage);
        }
    }
}