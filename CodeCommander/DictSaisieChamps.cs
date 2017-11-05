using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace CodeCommander
{
    internal class DictSaisieChamps : IDictProcess
    {
        #region Private Fields
        private IDictProcess previousPage;
        private IDictProcess rootPage;
        private Dictionary<string, IDictProcess> pages;
        private System.Windows.Forms.WebBrowser web;
        private o2Mate.Dictionnaire inputDict, outputDict;
        private string fileNameFinal;
        private string fileNameSrc;
        private int preferredHeight;
        private Dictionary<string, string> keys;
        private string keyTab;
        private int index;
        #endregion

        #region Default Constructor
        public DictSaisieChamps(IDictProcess previousPage, IDictProcess rootPage, Dictionary<string, IDictProcess> pages, System.Windows.Forms.WebBrowser web, o2Mate.Dictionnaire inputDict, o2Mate.Dictionnaire outputDict, string fileName, string key, int index)
        {
            this.previousPage = previousPage;
            this.rootPage = rootPage;
            this.pages = pages;
            this.web = web;
            this.inputDict = inputDict;
            this.outputDict = outputDict;
            this.fileNameSrc = fileName;
            this.keyTab = key;
            this.index = index;
            this.keys = new Dictionary<string, string>();
            this.fileNameFinal = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(new FileInfo(fileName).Name) + ".htm");
        }
        #endregion

        #region IDictProcess Membres

        public void Load()
        {
            o2Mate.Dictionnaire IHMdict = new o2Mate.Dictionnaire();
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(new FileInfo(this.fileNameSrc).Name) + ".xml"); ;
            this.preferredHeight = 270;
            this.keys.Clear();
            o2Mate.Array tabInput = this.inputDict.GetArray(this.keyTab) as o2Mate.Array;
            o2Mate.Fields fieldsInput = tabInput.Item(1) as o2Mate.Fields;
            o2Mate.Array tabDefaultFields = new o2Mate.Array();
            int fieldIndex = 0;
            if (!this.outputDict.IsArray(this.keyTab))
            {
                this.outputDict.AddArray(this.keyTab, new o2Mate.Array());
            }
            o2Mate.Array arrRows = this.outputDict.GetArray(this.keyTab) as o2Mate.Array;
            o2Mate.Fields rowFields = null;
            if (index > 0)
            {
                rowFields = arrRows.Item(this.index) as o2Mate.Fields;
            }
            o2Mate.Array refArrConnections = new o2Mate.Array();
            foreach (string key in fieldsInput.Keys)
            {
                o2Mate.Fields fieldsDefault = new o2Mate.Fields();
                ++fieldIndex;
                o2Mate.ILegende leg = this.inputDict.Legendes.GetLegendeByName(key, this.keyTab);
                this.keys.Add("field" + fieldIndex.ToString(), key);
                fieldsDefault.AddString("nom", "field" + fieldIndex.ToString());
                string description = key;
                if (leg != null)
                {
                    description = leg.Description;
                }
                fieldsDefault.AddString("description", description);
                string commentaire = "";
                if (leg != null)
                {
                    commentaire = leg.Commentaire;
                }
                fieldsDefault.AddString("commentaire", commentaire);
                string myType = "String";
                if (leg != null)
                {
                    myType = leg.Type;
                    if (leg.Observe != "")
                    {
                        fieldsDefault.AddString("observe", leg.Observe);
                    }
                }
                fieldsDefault.AddString("type", myType);
                string arrName, field;
                if (this.outputDict.GetInConnection(leg, this.keyTab, this.index, out arrName, out field))
                {
                    fieldsDefault.AddString("arrayReference", arrName);
                    o2Mate.Array arrConnection = this.outputDict.GetArray(arrName) as o2Mate.Array;
                    o2Mate.Fields refFieldsConnection = new o2Mate.Fields();
                    refFieldsConnection.AddString("name", "field" + index.ToString());
                    refFieldsConnection.AddString("value", "");
                    refArrConnections.Add(refFieldsConnection);
                    for (int indexConnection = 1; indexConnection <= arrConnection.Count; ++indexConnection)
                    {
                        o2Mate.Fields f = arrConnection.Item(indexConnection) as o2Mate.Fields;
                        if (f.Exists(field))
                        {
                            o2Mate.Fields refFields = new o2Mate.Fields();
                            refFields.AddString("name", "field" + index.ToString());
                            refFields.AddString("value", f.GetString(field));
                            refArrConnections.Add(refFields);
                        }
                    }
                }
                if (this.index > 0)
                {
                    if (rowFields.Exists(key))
                    {
                        fieldsDefault.AddString("valeur", rowFields.GetString(key));
                    }
                    else
                    {
                        rowFields.AddString(key, "");
                        fieldsDefault.AddString("valeur", "");
                    }
                }
                tabDefaultFields.Add(fieldsDefault);
                this.preferredHeight += 40;
            }
            IHMdict.AddArray("keyTab", tabDefaultFields);
            IHMdict.AddArray("connections", refArrConnections);
            if (this.index > 0)
                IHMdict.AddString("title", "Nouvel élément du tableau '" + this.keyTab + "'");
            else
                IHMdict.AddString("title", "Edition d'un élément du tableau '" + this.keyTab + "'");
            o2Mate.ILegende legTab = this.inputDict.Legendes.GetLegendeByName(this.keyTab);
            string descriptionTab = this.keyTab;
            if (legTab != null)
            {
                    descriptionTab = legTab.Description;
            }
            IHMdict.AddString("description", descriptionTab);
            string commentaireTab = "";
            if (legTab != null)
            {
                commentaireTab = legTab.Commentaire;
            }
            IHMdict.AddString("commentaire", commentaireTab);
            IHMdict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            comp.LoadTemplates(Documents.GeneratedDictionariesTemplatesDirectory);
            try
            {
                comp.Compilation(Documents.SaisieChampsPage, fileNameDict, this.fileNameFinal, null);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        void web_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.preferredHeight < this.web.Parent.MaximumSize.Height)
                this.web.Parent.Size = new Size(this.web.Parent.Size.Width, this.preferredHeight);
            else
                this.web.Parent.Size = new Size(this.web.Parent.Size.Width, this.web.Parent.MaximumSize.Height);
            this.web.Document.GetElementById("returnTop").Click += new System.Windows.Forms.HtmlElementEventHandler(Return_Click);
            this.web.Document.GetElementById("returnBottom").Click += new System.Windows.Forms.HtmlElementEventHandler(Return_Click);
            this.web.Document.GetElementById("callback").Click += new System.Windows.Forms.HtmlElementEventHandler(Callback_Click);
            this.web.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
        }

        void Return_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            this.previousPage.Navigate();
        }

        void Callback_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            string action = this.web.Document.GetElementById("callback").GetAttribute("action");
            if (action == "validate")
            {
                int rowId = this.index;
                if (rowId > 0)
                {
                    o2Mate.Array outputTab = this.outputDict.GetArray(this.keyTab) as o2Mate.Array;
                    o2Mate.Fields outputFields = outputTab.Item(rowId) as o2Mate.Fields;
                    foreach (KeyValuePair<string, string> kv in this.keys)
                    {
                        string value = this.web.Document.GetElementById(kv.Key).GetAttribute("value");
                        outputFields.AddString(kv.Value, value);
                    }
                }
                else
                {
                    o2Mate.Array outputTab = this.outputDict.GetArray(this.keyTab) as o2Mate.Array;
                    o2Mate.Fields outputFields = new o2Mate.Fields();
                    foreach (KeyValuePair<string, string> kv in this.keys)
                    {
                        string value = this.web.Document.GetElementById(kv.Key).GetAttribute("value");
                        outputFields.AddString(kv.Value, value);
                    }
                    outputTab.Add(outputFields);
                }
                this.Return_Click(sender, e);
            }
            else if (action == "add")
            {
                string arrayRef = this.web.Document.GetElementById("callback").GetAttribute("reference");
                DictSaisieChamps dsc = new DictSaisieChamps(this, null, null, this.web, this.inputDict, this.outputDict, this.fileNameSrc, arrayRef, 0);
                dsc.Load();
                dsc.Navigate();
            }
            else if (action == "edit")
            {
                string arrayRef = this.web.Document.GetElementById("callback").GetAttribute("reference");
                int indexRef = 0;
                Int32.TryParse(this.web.Document.GetElementById("callback").GetAttribute("index"), out indexRef);
                DictSaisieChamps dsc = new DictSaisieChamps(this, null, null, this.web, this.inputDict, this.outputDict, this.fileNameSrc, arrayRef, indexRef);
                dsc.Load();
                dsc.Navigate();
            }

        }

        public void Navigate()
        {
            this.Load();
            this.web.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
            this.web.Navigate(this.fileNameFinal);
        }

        public IDictProcess PreviousPage
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IDictProcess NextPage
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IDictProcess TablePage
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}
