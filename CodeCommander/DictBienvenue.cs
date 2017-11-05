using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CodeCommander
{
    internal class DictBienvenue : IDictProcess
    {
        #region Private Fields
        private System.Windows.Forms.WebBrowser web;
        private o2Mate.Dictionnaire inputDict, outputDict;
        private string fileNameFinal;
        private string fileNameSrc;
        private IDictProcess nextPage;
        #endregion

        #region Default Constructor
        public DictBienvenue(System.Windows.Forms.WebBrowser web, o2Mate.Dictionnaire inputDict, o2Mate.Dictionnaire outputDict, string fileName)
        {
            this.web = web;
            this.inputDict = inputDict;
            this.outputDict = outputDict;
            this.ParseExpressions();
            this.fileNameSrc = fileName;
            this.fileNameFinal = Path.Combine(Documents.TempDirectory, "bienvenue.htm");
        }
        #endregion

        #region Private Methods
        private void ParseExpressions()
        {
            foreach (string key in this.inputDict.StringKeys)
            {
                this.inputDict.ParseExpression(this.inputDict.Legendes.GetLegendeByName(key));
            }
            foreach (string tabKey in this.inputDict.ArrayKeys)
            {
                this.inputDict.ParseExpression(this.inputDict.Legendes.GetLegendeByName(tabKey));
                o2Mate.Array arr = this.inputDict.GetArray(tabKey) as o2Mate.Array;
                o2Mate.Fields fields = arr.Item(1) as o2Mate.Fields;
                foreach (string key in fields.Keys)
                {
                    this.inputDict.ParseExpression(this.inputDict.Legendes.GetLegendeByName(key, tabKey));
                }
            }
            // recopier toutes les legendes dans le dictionnaire de sortie
            this.outputDict.Legendes.CopyFrom(this.inputDict.Legendes as o2Mate.LegendeDict);
        }
        #endregion

        #region IDictProcess Membres
        public IDictProcess PreviousPage
        {
            get { return null; }
            set { }
        }

        public IDictProcess NextPage
        {
            get { return this.nextPage; }
            set { this.nextPage = value; }
        }

        public IDictProcess TablePage
        {
            get { return null; }
            set { }
        }

        public void Load()
        {
            o2Mate.Dictionnaire IHMdict = new o2Mate.Dictionnaire();
            string fileNameDict = Path.Combine(Documents.TempDirectory, Documents.BienvenuePage + ".xml");
            IHMdict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            comp.LoadTemplates(Documents.GeneratedDictionariesTemplatesDirectory);
            comp.Compilation(Documents.BienvenuePage, fileNameDict, this.fileNameFinal, null);
        }

        public void QuickLoad()
        {
            int countStrings = 0;
            int countTabs = 0;
            foreach (string key in this.inputDict.StringKeys) { ++countStrings; }
            foreach (string key in this.inputDict.ArrayKeys) { ++countTabs; }
            if (countTabs > 0)
            {
                if (countStrings > 0)
                {
                    DictSaisieLibre dsl = new DictSaisieLibre(null, null, this.web, this.inputDict, this.outputDict, this.fileNameSrc);
                    dsl.PreviousPage = this;
                    IDictProcess nextPage = dsl;
                    foreach (string key in this.inputDict.ArrayKeys)
                    {
                        bool makeTab = true;
                        o2Mate.ILegende legende = this.inputDict.Legendes.GetLegendeByName(key);
                        // si le tableau n'est pas libre alors on ne l'affiche pas
                        if (legende != null && !legende.Free)
                        {
                            makeTab = false;
                        }
                        if (makeTab)
                        {
                            DictSaisieTableau tab = new DictSaisieTableau(null, null, this.web, this.inputDict, this.outputDict, this.fileNameSrc, key);
                            tab.Load();
                            nextPage.NextPage = tab;
                            tab.PreviousPage = nextPage;
                            nextPage = tab;
                        }
                    }
                    dsl.Load();
                    dsl.Navigate();
                }
                else
                {
                    IDictProcess nextPage = this;
                    DictSaisieTableau tab = null;
                    foreach (string key in this.inputDict.ArrayKeys)
                    {
                        bool makeTab = true;
                        o2Mate.ILegende legende = this.inputDict.Legendes.GetLegendeByName(key);
                        // si le tableau n'est pas libre alors on ne l'affiche pas
                        if (legende != null && !legende.Free)
                        {
                            makeTab = false;
                        }
                        if (makeTab)
                        {
                            tab = new DictSaisieTableau(null, null, this.web, this.inputDict, this.outputDict, this.fileNameSrc, key);
                            tab.Load();
                            nextPage.NextPage = tab;
                            tab.PreviousPage = nextPage;
                            nextPage = tab;
                        }
                    }
                    if (tab != null) tab.Navigate();
                }
            }
            else if (countStrings > 0)
            {
                DictSaisieLibre dsl = new DictSaisieLibre(null, null, this.web, this.inputDict, this.outputDict, this.fileNameSrc);
                dsl.Load();
                dsl.PreviousPage = this;
                dsl.Navigate();
            }
        }

        public void Navigate()
        {
            this.web.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
            this.web.Navigate(this.fileNameFinal);
        }

        void web_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            this.web.Document.GetElementById("createDict").Click += new System.Windows.Forms.HtmlElementEventHandler(DictBienvenue_Create);
            this.web.Document.GetElementById("loadDict").Click += new System.Windows.Forms.HtmlElementEventHandler(DictBienvenue_Load);
            this.web.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
        }

        void DictBienvenue_Load(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
        }

        void DictBienvenue_Create(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            int countStrings = 0;
            int countTabs = 0;
            foreach (string key in this.inputDict.StringKeys) { ++countStrings; }
            foreach (string key in this.inputDict.ArrayKeys) { ++countTabs; }
            if (countTabs > 0)
            {
                if (countStrings > 0)
                {
                    DictSaisieLibre dsl = new DictSaisieLibre(null, null, this.web, this.inputDict, this.outputDict, this.fileNameSrc);
                    dsl.PreviousPage = this;
                    IDictProcess nextPage = dsl;
                    foreach (string key in this.inputDict.ArrayKeys)
                    {
                        bool makeTab = true;
                        o2Mate.ILegende legende = this.inputDict.Legendes.GetLegendeByName(key);
                        // si le tableau n'est pas libre alors on ne l'affiche pas
                        if (legende != null && !legende.Free)
                        {
                            makeTab = false;
                        }
                        if (makeTab)
                        {
                            DictSaisieTableau tab = new DictSaisieTableau(null, null, this.web, this.inputDict, this.outputDict, this.fileNameSrc, key);
                            tab.Load();
                            nextPage.NextPage = tab;
                            tab.PreviousPage = nextPage;
                            nextPage = tab;
                        }
                    }
                    nextPage.NextPage = new DictExecute(this.web, this.inputDict, this.outputDict, this.fileNameSrc);
                    nextPage.NextPage.Load();
                    dsl.Load();
                    dsl.Navigate();
                }
                else
                {
                    IDictProcess nextPage = this;
                    foreach (string key in this.inputDict.ArrayKeys)
                    {
                        bool makeTab = true;
                        o2Mate.ILegende legende = this.inputDict.Legendes.GetLegendeByName(key);
                        // si le tableau n'est pas libre alors on ne l'affiche pas
                        if (legende != null && !legende.Free)
                        {
                            makeTab = false;
                        }
                        if (makeTab)
                        {
                            DictSaisieTableau tab = new DictSaisieTableau(null, null, this.web, this.inputDict, this.outputDict, this.fileNameSrc, key);
                            tab.Load();
                            nextPage.NextPage = tab;
                            tab.PreviousPage = nextPage;
                            nextPage = tab;
                        }
                    }
                    nextPage.NextPage = new DictExecute(this.web, this.inputDict, this.outputDict, this.fileNameSrc);
                    nextPage.NextPage.Load();
                    this.NextPage.Navigate();
                }
            }
            else if (countStrings > 0)
            {
                DictSaisieLibre dsl = new DictSaisieLibre(null, null, this.web, this.inputDict, this.outputDict, this.fileNameSrc);
                dsl.PreviousPage = this;
                dsl.NextPage = new DictExecute(this.web, this.inputDict, this.outputDict, this.fileNameSrc);
                dsl.NextPage.Load();
                dsl.Load();
                dsl.Navigate();
            }
        }

        #endregion
    }
}
