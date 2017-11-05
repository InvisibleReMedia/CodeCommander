using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace CodeCommander
{
    internal class DictSaisieLibre : DictWizard, IDictProcess
    {
        #region Private Fields
        private Dictionary<string, string> keys;
        #endregion

        #region Default Constructor
        public DictSaisieLibre(IDictProcess rootPage, Dictionary<string, IDictProcess> pages, System.Windows.Forms.WebBrowser web, o2Mate.Dictionnaire inputDict, o2Mate.Dictionnaire outputDict, string fileName) : base(rootPage, pages, web, inputDict, outputDict, fileName)
        {
            this.keys = new Dictionary<string, string>();
            base.Next += new EventHandler(DictSaisieLibre_Next);
            base.Previous += new EventHandler(DictSaisieLibre_Previous);
        }
        #endregion

        #region IDictProcess Membres

        public new void Load()
        {
            o2Mate.Dictionnaire IHMdict = new o2Mate.Dictionnaire();
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(new FileInfo(this.fileNameSrc).Name) + ".xml");
            o2Mate.Array arr = new o2Mate.Array();
            this.PreferredHeight = 258;
            this.keys.Clear();
            int index = 1;
            IHMdict.AddString("title", "Saisie des champs libres");
            o2Mate.Array refArrConnections = new o2Mate.Array();
            foreach (string key in this.inputDict.StringKeys)
            {
                o2Mate.Fields fields = new o2Mate.Fields();
                this.keys.Add("field" + index.ToString(), key);
                fields.AddString("nom", "field" + index.ToString());
                o2Mate.ILegende leg = this.inputDict.Legendes.GetLegendeByName(key);
                string description = key;
                if (leg != null)
                    description = leg.Description;
                fields.AddString("description", description);
                string commentaire = "";
                if (leg != null)
                    commentaire = leg.Commentaire;
                fields.AddString("commentaire", commentaire);
                string myType = "String";
                if (leg != null)
                {
                    myType = leg.Type;
                    if (leg.Expression != "")
                    {
                        fields.AddString("observe", leg.Expression);
                    }
                }
                fields.AddString("type", myType);
                string arrName, field;
                if (this.outputDict.GetInConnection(leg, "", 0, out arrName, out field))
                {
                    fields.AddString("arrayReference", arrName);
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
                if (this.outputDict.IsString(key))
                {
                    fields.AddString("valeur", this.outputDict.GetString(key));
                }
                else
                {
                    fields.AddString("valeur", "");
                }
                arr.Add(fields);
                this.PreferredHeight += 40;
                ++index;
            }
            IHMdict.AddArray("connections", refArrConnections);
            IHMdict.AddArray("champs", arr);
            IHMdict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            comp.LoadTemplates(Documents.GeneratedDictionariesTemplatesDirectory);
            comp.Compilation(Documents.SaisieLibrePage, fileNameDict, this.fileNameFinal, null);
            FileInfo fi = new FileInfo(Documents.GeneratedDictionariesDirectory + Documents.ImageAdd);
            fi.CopyTo(Path.GetDirectoryName(this.fileNameFinal) + Documents.ImageAdd, true);
            fi = new FileInfo(Documents.GeneratedDictionariesDirectory + Documents.ImageEdit);
            fi.CopyTo(Path.GetDirectoryName(this.fileNameFinal) + Documents.ImageEdit, true);
        }

        void web_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            this.web.Document.GetElementById("callback").Click += new System.Windows.Forms.HtmlElementEventHandler(Callback_Click);
            foreach (string fieldName in this.keys.Keys)
            {
                if (this.outputDict.IsString(this.keys[fieldName]))
                {
                    this.web.Document.GetElementById(fieldName).SetAttribute("value", this.outputDict.GetString(this.keys[fieldName]));
                }
                this.web.Document.GetElementById(fieldName).LostFocus += new HtmlElementEventHandler(DictSaisieLibre_LostFocus);
            }
            this.DocumentCompleted(sender, e);
            this.web.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
        }

        void Callback_Click(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            string action = this.web.Document.GetElementById("callback").GetAttribute("action");
            if (action == "add")
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

        void DictSaisieLibre_LostFocus(object sender, HtmlElementEventArgs e)
        {
            foreach (string fieldName in this.keys.Keys)
            {
                if (this.outputDict.IsString(this.keys[fieldName]))
                {
                    this.outputDict.SetString(this.keys[fieldName], this.web.Document.GetElementById(fieldName).GetAttribute("value"));
                }
                else
                {
                    this.outputDict.AddString(this.keys[fieldName], this.web.Document.GetElementById(fieldName).GetAttribute("value"));
                }
            }
        }

        void DictSaisieLibre_Previous(object sender, EventArgs e)
        {
            if (this.PreviousPage != null)
            {
                this.PreviousPage.Navigate();
            }
            else
                this.web.FindForm().Close();
        }

        void DictSaisieLibre_Next(object sender, EventArgs e)
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
