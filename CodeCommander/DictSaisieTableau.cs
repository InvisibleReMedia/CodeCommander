using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace CodeCommander
{
    internal class DictSaisieTableau : DictWizard, IDictProcess
    {
        #region Private Fields
        private Dictionary<string, string> keys;
        private string keyTab;
        #endregion

        #region Default Constructor
        public DictSaisieTableau(IDictProcess rootPage, Dictionary<string, IDictProcess> pages, System.Windows.Forms.WebBrowser web, o2Mate.Dictionnaire inputDict, o2Mate.Dictionnaire outputDict, string fileName, string key) : base(rootPage, pages, web, inputDict, outputDict, fileName)
        {
            this.keyTab = key;
            this.keys = new Dictionary<string, string>();
            base.Next += new EventHandler(DictSaisieTableau_Next);
            base.Previous += new EventHandler(DictSaisieTableau_Previous);
        }
        #endregion

        #region IDictProcess Membres

        public new void Load()
        {
            o2Mate.Dictionnaire IHMdict = new o2Mate.Dictionnaire();
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(new FileInfo(this.fileNameSrc).Name) + ".xml");
            this.PreferredHeight = 270;
            this.keys.Clear();
            o2Mate.Array tabInput = this.inputDict.GetArray(this.keyTab) as o2Mate.Array;
            o2Mate.Fields fieldsInput = tabInput.Item(1) as o2Mate.Fields;
            o2Mate.Array tabDefaultFields = new o2Mate.Array();
            int fieldIndex = 0;
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
                    if (leg.Expression != "")
                    {
                        fieldsDefault.AddString("observe", leg.Expression);
                    }
                }
                fieldsDefault.AddString("type", myType);
                tabDefaultFields.Add(fieldsDefault);
            }
            IHMdict.AddArray("keyTab", tabDefaultFields);
            IHMdict.AddString("title", "Saisie du tableau '" + this.keyTab + "'");
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
            if (!this.outputDict.IsArray(this.keyTab))
            {
                this.outputDict.AddArray(this.keyTab, new o2Mate.Array());
            }
            o2Mate.Array tabOutput = this.outputDict.GetArray(this.keyTab) as o2Mate.Array;
            o2Mate.Array arrRows = new o2Mate.Array();
            o2Mate.Array arrFields = new o2Mate.Array();
            for (int index = 1; index <= tabOutput.Count; ++index)
            {
                o2Mate.Fields tabRows = new o2Mate.Fields();
                tabRows.AddString("index", index.ToString());
                arrRows.Add(tabRows);
                o2Mate.Fields tabFieldsInput = tabInput.Item(1) as o2Mate.Fields;
                o2Mate.Fields fields = tabOutput.Item(index) as o2Mate.Fields;
                fieldIndex = 0;
                foreach (string key in tabFieldsInput.Keys)
                {
                    o2Mate.Fields tabFields = new o2Mate.Fields();
                    tabFields.AddString("rowId", index.ToString());
                    ++fieldIndex;
                    o2Mate.ILegende leg = this.inputDict.Legendes.GetLegendeByName(key, this.keyTab);
                    this.keys.Add("rowId" + index.ToString() + "_field" + fieldIndex.ToString(), key);
                    tabFields.AddString("nom", "rowId" + index.ToString() + "_field" + fieldIndex.ToString());
                    string description = key;
                    if (leg != null)
                    {
                        description = leg.Description;
                    }
                    tabFields.AddString("description", description);
                    string commentaire = "";
                    if (leg != null)
                    {
                        commentaire = leg.Commentaire;
                    }
                    tabFields.AddString("commentaire", commentaire);
                    string myType = "String";
                    if (leg != null)
                    {
                        myType = leg.Type;
                        if (leg.Expression != "")
                        {
                            tabFields.AddString("observe", leg.Expression);
                        }
                    }
                    tabFields.AddString("type", myType);
                    if (fields.Exists(key))
                        tabFields.AddString("value", fields.GetString(key));
                    else
                        tabFields.AddString("value", "");
                    arrFields.Add(tabFields);
                    this.PreferredHeight += 40;
                }
                this.PreferredHeight += 10;
            }
            IHMdict.AddArray("items", arrRows);
            IHMdict.AddArray("fields", arrFields);
            IHMdict.Save(fileNameDict);
            try
            {
                o2Mate.Compilateur comp = new o2Mate.Compilateur();
                comp.LoadTemplates(Documents.GeneratedDictionariesTemplatesDirectory);
                comp.Compilation(Documents.SaisieTableauPage, fileNameDict, this.fileNameFinal, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            FileInfo fi = new FileInfo(Documents.GeneratedDictionariesDirectory + Documents.ImageTabUp);
            fi.CopyTo(Path.GetDirectoryName(this.fileNameFinal) + Documents.ImageTabUp, true);
            fi = new FileInfo(Documents.GeneratedDictionariesDirectory + Documents.ImageTabDown);
            fi.CopyTo(Path.GetDirectoryName(this.fileNameFinal) + Documents.ImageTabDown, true);
        }

        void web_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            this.web.Document.GetElementById("callback").Click += new System.Windows.Forms.HtmlElementEventHandler(Callback_Click);
            this.DocumentCompleted(sender, e);
            this.web.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
        }

        void Callback_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            string action = this.web.Document.GetElementById("callback").GetAttribute("action");
            if (action == "addRow")
            {
                int rowId = 0;
                Int32.TryParse(this.web.Document.GetElementById("callback").GetAttribute("rowId"), out rowId);
                o2Mate.Array arr = this.outputDict.GetArray(this.keyTab) as o2Mate.Array;
                o2Mate.Fields fields = new o2Mate.Fields();
                o2Mate.Fields inputFields = (this.inputDict.GetArray(this.keyTab) as o2Mate.Array).Item(1) as o2Mate.Fields;
                int fieldIndex = 0;
                foreach (string key in inputFields.Keys)
                {
                    ++fieldIndex;
                    this.keys.Add("rowId" + rowId.ToString() + "_field" + fieldIndex.ToString(), key);
                    fields.AddString(key, "");
                    this.PreferredHeight += 40;
                }
                arr.Add(fields);
                this.PreferredHeight += 10;
                if (this.preferredHeight < this.web.Parent.MaximumSize.Height)
                    this.web.Parent.Size = new System.Drawing.Size(this.web.Parent.Size.Width, this.preferredHeight);
                else
                    this.web.Parent.Size = new System.Drawing.Size(this.web.Parent.Size.Width, this.web.Parent.MaximumSize.Height);
            }
            else if (action == "onchange")
            {
                int rowId = 0;
                Int32.TryParse(this.web.Document.GetElementById("callback").GetAttribute("rowId"), out rowId);
                if (rowId > 0)
                {
                    string value = this.web.Document.GetElementById("callback").GetAttribute("value");
                    string fieldName = this.web.Document.GetElementById("callback").GetAttribute("fieldId");
                    o2Mate.Array outputTab = this.outputDict.GetArray(this.keyTab) as o2Mate.Array;
                    o2Mate.Fields outputFields = outputTab.Item(rowId) as o2Mate.Fields;
                    outputFields.AddString(this.keys[fieldName], value);
                }
            }
        }

        void DictSaisieTableau_Previous(object sender, EventArgs e)
        {
            if (this.PreviousPage != null)
            {
                this.PreviousPage.Navigate();
            }
            else
                this.web.FindForm().Close();
        }

        void DictSaisieTableau_Next(object sender, EventArgs e)
        {
            if (this.NextPage != null)
            {
                this.NextPage.Navigate();
            }
            else
                this.web.FindForm().Close();
        }

        public new void Navigate()
        {
            this.Load();
            this.web.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
            base.Navigate();
        }

        #endregion
    }
}
