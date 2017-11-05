using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace CodeCommander
{
    internal class DictWizard : IDictProcess
    {
        #region Public Events
        public event EventHandler Previous;
        public event EventHandler Next;
        #endregion

        #region Private Fields
        private IDictProcess previousPage;
        private IDictProcess nextPage;
        private IDictProcess tablePage;
        #endregion

        #region Protected Fields
        protected IDictProcess rootPage;
        protected Dictionary<string, IDictProcess> pages;
        protected System.Windows.Forms.WebBrowser web;
        protected o2Mate.Dictionnaire inputDict, outputDict;
        protected string fileNameFinal;
        protected string fileNameSrc;
        protected int preferredHeight;
        #endregion

        #region Default Constructor
        public DictWizard(IDictProcess rootPage, Dictionary<string, IDictProcess> pages, System.Windows.Forms.WebBrowser web, o2Mate.Dictionnaire inputDict, o2Mate.Dictionnaire outputDict, string fileName)
        {
            this.rootPage = rootPage;
            this.pages = pages;
            this.web = web;
            this.inputDict = inputDict;
            this.outputDict = outputDict;
            this.fileNameSrc = fileName;
            this.fileNameFinal = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(this.fileNameFinal) + ".htm");
        }
        #endregion

        #region Protected Methods

        protected void Validate()
        {
            IDictProcess currentPage = rootPage;
            foreach (string tabKey in this.inputDict.ArrayKeys)
            {
                if (this.outputDict.TestDependsOn(this.inputDict.Legendes.GetLegendeByName(tabKey)))
                {
                    IDictProcess temp = currentPage.NextPage;
                    currentPage.NextPage = this.pages[tabKey];
                    currentPage.NextPage.PreviousPage = currentPage;
                    currentPage.NextPage.NextPage = temp;
                    if (temp != null)
                    {
                        temp.PreviousPage = temp;
                    }
                }
            }
        }
        #endregion

        #region IDictProcess Membres
        public IDictProcess PreviousPage
        {
            get { return this.previousPage; }
            set { this.previousPage = value;  }
        }

        public IDictProcess NextPage
        {
            get { return this.nextPage; }
            set { this.nextPage = value;  }
        }

        public IDictProcess TablePage
        {
            get { return this.tablePage; }
            set { this.tablePage = value; }
        }

        public int PreferredHeight
        {
            get { return this.preferredHeight; }
            set { this.preferredHeight = value; }
        }

        public void Load()
        {
        }

        protected void DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.preferredHeight < this.web.Parent.MaximumSize.Height)
                this.web.Parent.Size = new Size(this.web.Parent.Size.Width, this.preferredHeight);
            else
                this.web.Parent.Size = new Size(this.web.Parent.Size.Width, this.web.Parent.MaximumSize.Height);
            this.web.Document.GetElementById("previousTop").Click += new System.Windows.Forms.HtmlElementEventHandler(DictWizard_Previous);
            this.web.Document.GetElementById("previousBottom").Click += new System.Windows.Forms.HtmlElementEventHandler(DictWizard_Previous);
            this.web.Document.GetElementById("nextTop").Click += new System.Windows.Forms.HtmlElementEventHandler(DictWizard_Next);
            this.web.Document.GetElementById("nextBottom").Click += new System.Windows.Forms.HtmlElementEventHandler(DictWizard_Next);
        }

        void DictWizard_Next(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            if (this.Next != null)
            {
                this.Next(sender, new EventArgs());
            }
        }

        void DictWizard_Previous(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            if (this.Previous != null)
            {
                this.Previous(sender, new EventArgs());
            }
        }

        public void Navigate()
        {
            this.web.Navigate(this.fileNameFinal);
        }

        #endregion
    }
}
