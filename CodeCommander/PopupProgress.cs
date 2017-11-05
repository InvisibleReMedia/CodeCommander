using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodeCommander
{
    internal partial class PopupProgress : Form, o2Mate.INotifyProgress
    {
        #region Public Events
        private event EventHandler cancel;
        #endregion

        #region Private Fields
        private int finished;
        private bool isCanceled;
        private delegate void SetCountDelegate(int count);
        private delegate void CloseDelegate();
        private delegate void SetProgressDelegate(int position);
        private delegate void SetTextDelegate(string t);
        private delegate void SetMarqueeDelegate();
        #endregion

        #region Public Properties
        public event EventHandler Cancel
        {
            add { this.cancel += new EventHandler(value); }
            remove { this.cancel -= value; }
        }
        #endregion

        #region Private Methods
        private void ThreadSafeClose()
        {
            this.Close();
        }

        private void ThreadSafeSetCount(int count)
        {
            this.progressBar1.Maximum = count;
        }

        private void ThreadSafeSetProgress(int position)
        {
            this.progressBar1.Value = position;
        }

        private void ThreadSafeStep(int inc)
        {
            this.progressBar1.Step = inc;
            this.progressBar1.PerformStep();
        }

        private void ThreadSafeSetText(string t)
        {
            this.lblAction.Text = t;
        }

        private void ThreadSafeMarquee()
        {
            this.progressBar1.Style = ProgressBarStyle.Marquee;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.isCanceled = true;
            if (this.cancel != null)
                this.cancel(sender, e);
            this.IsFinished = true;
        }
        #endregion

        #region Constructors
        public PopupProgress(bool cancelable)
        {
            InitializeComponent();
            this.progressBar1.Style = ProgressBarStyle.Blocks;
            this.progressBar1.Value = 0;
            this.lblAction.Text = "";
            this.btnCancel.Visible = cancelable;
        }

        public PopupProgress(bool cancelable, ProgressBarStyle style)
        {
            InitializeComponent();
            this.progressBar1.Style = style;
            this.progressBar1.Value = 0;
            this.lblAction.Text = "";
            this.btnCancel.Visible = cancelable;
        }
        #endregion

        #region INotifyProgress Membres

        public bool IsCanceled
        {
            get { return this.isCanceled; }
        }

        public bool IsFinished
        {
            get { return this.finished > 0; }
            set { if (value) { System.Threading.Interlocked.Increment(ref this.finished); this.Terminate(true); } }
        }

        public void Start(bool cancelable)
        {
            this.progressBar1.Value = 0;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = 100;
            this.lblAction.Text = "";
            this.btnCancel.Visible = cancelable;
            this.finished = 0;
        }

        public void SetMarquee()
        {
            if (this.IsHandleCreated)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new SetMarqueeDelegate(ThreadSafeMarquee));
                }
                else
                {
                    this.ThreadSafeMarquee();
                }
            }
        }

        public void SetText(string t)
        {
            if (!String.IsNullOrEmpty(t) && this.IsHandleCreated)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new SetTextDelegate(ThreadSafeSetText), t);
                }
                else
                {
                    this.ThreadSafeSetText(t);
                }
            }
        }

        public void SetCount(int count)
        {
            if (this.IsHandleCreated)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new SetCountDelegate(ThreadSafeSetCount), count);
                }
                else
                {
                    this.ThreadSafeSetCount(count);
                }
            }
        }

        public void SetProgress(int position)
        {
            if (this.IsHandleCreated)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new SetCountDelegate(ThreadSafeSetProgress), position);
                }
                else
                {
                    this.ThreadSafeSetProgress(position);
                }
            }
        }

        public void Step(int inc)
        {
            if (this.IsHandleCreated)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new SetCountDelegate(ThreadSafeStep), inc);
                }
                else
                {
                    this.ThreadSafeStep(inc);
                }
            }
        }

        public void Terminate(bool noError)
        {
            if (this.IsHandleCreated)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new CloseDelegate(ThreadSafeClose));
                }
                else
                {
                    this.ThreadSafeClose();
                }
            }
        }

        public void GiveException(Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Terminate(false);
        }

        #endregion

    }
}