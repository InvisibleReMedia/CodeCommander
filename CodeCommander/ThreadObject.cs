using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CodeCommander
{
    internal class ThreadObject : IDisposable
    {
        #region Private Fields
        private object[] datas;
        private ManualResetEvent wait;
        private Timer tim;
        #endregion

        #region Public Constructor
        public ThreadObject(object[] datas)
        {
            this.datas = datas;
            this.wait = new ManualResetEvent(false);
            this.tim = new Timer(new TimerCallback(TimerElapsed), null, 500, 0);
        }
        #endregion

        #region Private Methods
        private void TimerElapsed(object obj)
        {
            this.wait.Set();
        }
        #endregion

        #region Public Properties
        public object[] Datas
        {
            get { return this.datas; }
        }

        public ManualResetEvent Wait
        {
            get { return this.wait; }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            this.wait.Dispose();
            this.tim.Dispose();
        }
        #endregion
    }
}
