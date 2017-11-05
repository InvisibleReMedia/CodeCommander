using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace CodeCommander
{
    internal class DisplayWorker
    {
        #region Private Delegate
        private delegate void AsyncDisplay(System.Windows.Forms.WebBrowser web);
        #endregion

        #region Private Fields
        private o2Mate.INotifyProgress pp;
        private Queue<o2Mate.DisplayElement> buffer;
        private AsyncDisplay display;
        #endregion

        #region Default Constructor
        public DisplayWorker(o2Mate.INotifyProgress pp)
        {
            this.pp = pp;
            this.display = new AsyncDisplay(Consume);
            this.buffer = new Queue<o2Mate.DisplayElement>();
        }
        #endregion

        #region Private Methods
        private void Consume(System.Windows.Forms.WebBrowser web)
        {
            while (this.buffer.Count > 0)
            {
                if (this.pp.IsCanceled) break;
                o2Mate.DisplayElement de = this.buffer.Dequeue();
                this.pp.SetProgress(de.Percent);
                this.pp.SetText(String.Format("Traitement {0}", de.ObjectType));
                de.InvokeBrowser(web);
                System.Windows.Forms.Application.DoEvents();
            }
            if (this.pp.IsCanceled)
            {
                this.Cloture(web);
            }
        }
        #endregion

        #region Public Methods
        public void InvokeBrowser(o2Mate.DisplayElement de, System.Windows.Forms.WebBrowser web)
        {
            try
            {
                this.buffer.Enqueue(de);
                if (System.Threading.Monitor.TryEnter(web))
                {
                    web.Invoke(this.display, web);
                    System.Threading.Monitor.Exit(web);
                }
            }
            catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public void Cloture(System.Windows.Forms.WebBrowser web)
        {
            this.pp.IsFinished = true;
        }
        #endregion
    }
}
