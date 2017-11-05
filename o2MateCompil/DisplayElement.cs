using System;
using System.Collections.Generic;
using System.Text;

namespace o2Mate
{
    /// <summary>
    /// Display element
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DisplayElement
    {
        #region Private Delegate
        private delegate void MakeDisplay(System.Windows.Forms.WebBrowser web);
        #endregion

        #region Private Fields
        private string objectType;
        private string scriptName;
        private object[] datas;
        private int percent;
        private bool isLast;
        private MakeDisplay display;
        #endregion

        #region Public Properties
        /// <summary>
        /// Name of the object type
        /// </summary>
        public string ObjectType
        {
            get { return this.objectType; }
        }

        /// <summary>
        /// Gets or sets percentage of progression
        /// </summary>
        public int Percent
        {
            get { return this.percent; }
            set { this.percent = value; }
        }

        /// <summary>
        /// Gets or sets to be the last object
        /// </summary>
        public bool IsLast
        {
            get { return this.isLast; }
            set { this.isLast = value; }
        }
        #endregion

        #region Default Constructor
        /// <summary>
        /// Constructor for a script and some parameters
        /// </summary>
        /// <param name="objType">name of the object type</param>
        /// <param name="scriptName">name of the javascript function on the web browser</param>
        /// <param name="datas">datas infos</param>
        public DisplayElement(string objType, string scriptName, object[] datas)
        {
            this.objectType = objType;
            this.scriptName = scriptName;
            this.datas = datas;
            this.display = new MakeDisplay(BrowserWork);
        }

        /// <summary>
        /// Constructor for a script and no parameters
        /// </summary>
        /// <param name="objType">name of the object type</param>
        /// <param name="scriptName">name of the javascript function on the web browser</param>
        public DisplayElement(string objType, string scriptName)
        {
            this.objectType = objType;
            this.scriptName = scriptName;
            this.datas = null;
            this.display = new MakeDisplay(BrowserWork);
        }
        #endregion

        #region Private Methods
        private void BrowserWork(System.Windows.Forms.WebBrowser web)
        {
            if (this.datas != null)
                web.Document.InvokeScript(this.scriptName, this.datas);
            else
                web.Document.InvokeScript(this.scriptName);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Invoking web browser and call the internal delegate function
        /// </summary>
        /// <param name="web">web browser</param>
        public void InvokeBrowser(System.Windows.Forms.WebBrowser web)
        {
            if (web.IsHandleCreated)
            {
                if (web.InvokeRequired)
                {
                    web.BeginInvoke(this.display, web);
                }
                else
                {
                    this.BrowserWork(web);
                }
            }
        }
        #endregion
    }
}
