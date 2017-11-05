using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace CodeCommander
{
    internal partial class Form1 : Form
    {
        #region Constants
        private static readonly string YES = "YES";
        private static readonly string NO = "NO";
        private static readonly int[] intervalsOpenView = new int[] { 10, 50, 100, 110, 200, 280, 360, 450, 510, 520, 530, 540, 550 };
        private static readonly int[] intervalsTimer = new int[] { 100, 100, 100, 70, 50, 20, 20, 20, 20, 20, 50, 70, 100 };
        #endregion

        #region Private Types
        private enum ViewType
        {
            None,
            Template,
            Skeleton,
            Syntax
        }
        #endregion

        #region Private Fields
        private string directorySource;
        private string _fileName;
        private o2Mate.Compilateur templates;
        private Dictionary<string, o2Mate.Template> listTemplateRef;
        private Dictionary<string, o2Mate.Skeleton> listSkeletonRef;
        private Dictionary<string, o2Mate.Syntax> listSyntaxRef;
        private o2Mate.Template currentTemplateView;
        private o2Mate.Skeleton currentSkeletonView;
        private o2Mate.Syntax currentSyntaxView;
        ViewType viewType;
        private bool openView;
        private int currentCursorView;
        private ComputerSettings cs;
        private PopupProgress pp;
        private Localization locales;
        private List<string> cutter;
        private Projects pageProjects;
        private BackgroundWorker bw;
        #endregion

        public Form1(ComputerSettings cs)
        {
            this.cs = cs;
            this.directorySource = Documents.SourcesDirectory;
            this.KeyPreview = true;
            this.templates = new o2Mate.Compilateur();
            this.listTemplateRef = new Dictionary<string, o2Mate.Template>();
            this.listSkeletonRef = new Dictionary<string, o2Mate.Skeleton>();
            this.listSyntaxRef = new Dictionary<string, o2Mate.Syntax>();
            this.viewType = ViewType.None;
            this.currentTemplateView = null;
            this.currentSkeletonView = null;
            this.currentSyntaxView = null;
            this.openView = false;
            this.locales = null;
            this.cutter = new List<string>();
            InitializeComponent();
        }

        public ComputerSettings ComputerSettings
        {
            get { return this.cs; }
        }

        private System.Windows.Forms.HtmlElement GetElementByName(System.Windows.Forms.HtmlElement from, string name)
        {
            if (from.Name == name)
            {
                return from;
            }
            else
            {
                foreach (System.Windows.Forms.HtmlElement child in from.Children)
                {
                    try
                    {
                        return GetElementByName(child, name);
                    }
                    catch { }
                }
                throw new Exception("this sub-element does not contain that name element");
            }
        }

        private ToolStripItem GetModelesMenuItem(string path)
        {
            foreach (ToolStripItem tsi in this.modèlesToolStripMenuItem.DropDownItems)
            {
                if (tsi.Text == path)
                {
                    return tsi;
                }
            }
            return null;
        }

        private ToolStripItem GetSkeletonsMenuItem(string path)
        {
            foreach (ToolStripItem tsi in this.skeletonsToolStripMenuItem.DropDownItems)
            {
                if (tsi.Text == path)
                {
                    return tsi;
                }
            }
            return null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int nbItems = 0;
            this.templates.LoadTemplates(Documents.TemplatesDirectory);
            foreach (o2Mate.Template t in this.templates.Templates)
            {
                string path = t.Path;
                if (path.StartsWith("/"))
                {
                    path = path.Substring(1);
                }
                ToolStripItem tsi = this.GetModelesMenuItem(path);
                if (tsi != null)
                {
                    ToolStripMenuItem menu = tsi as ToolStripMenuItem;
                    ToolStripMenuItem subItem = new ToolStripMenuItem(t.Name);
                    ++nbItems;
                    this.listTemplateRef.Add("modele" + nbItems.ToString(), t);
                    subItem.Name = "modele" + nbItems.ToString();
                    subItem.Click += new EventHandler(template_Click);
                    menu.DropDownItems.Add(subItem);
                }
                else
                {
                    tsi = this.modèlesToolStripMenuItem.DropDownItems.Add(path);
                    ToolStripMenuItem menu = tsi as ToolStripMenuItem;
                    ToolStripMenuItem subItem = new ToolStripMenuItem(t.Name);
                    ++nbItems;
                    this.listTemplateRef.Add("modele" + nbItems.ToString(), t);
                    subItem.Name = "modele" + nbItems.ToString();
                    subItem.Click += new EventHandler(template_Click);
                    menu.DropDownItems.Add(subItem);
                }

            }
            nbItems = 0;
            this.templates.ParseSkeletons(Documents.SkeletonsDirectory);
            foreach (o2Mate.Skeleton skel in this.templates.Skeletons)
            {
                string path = skel.Path;
                if (path.StartsWith("/"))
                {
                    path = path.Substring(1);
                }
                ToolStripItem tsi = this.GetSkeletonsMenuItem(path);
                if (tsi != null)
                {
                    ToolStripMenuItem menu = tsi as ToolStripMenuItem;
                    ToolStripMenuItem subItem = new ToolStripMenuItem(skel.Name);
                    ++nbItems;
                    this.listSkeletonRef.Add("skeleton" + nbItems.ToString(), skel);
                    subItem.Name = "skeleton" + nbItems.ToString();
                    subItem.Click += new EventHandler(skeleton_Click);
                    menu.DropDownItems.Add(subItem);
                }
                else
                {
                    tsi = this.skeletonsToolStripMenuItem.DropDownItems.Add(path);
                    ToolStripMenuItem menu = tsi as ToolStripMenuItem;
                    ToolStripMenuItem subItem = new ToolStripMenuItem(skel.Name);
                    ++nbItems;
                    this.listSkeletonRef.Add("skeleton" + nbItems.ToString(), skel);
                    subItem.Name = "skeleton" + nbItems.ToString();
                    subItem.Click += new EventHandler(skeleton_Click);
                    menu.DropDownItems.Add(subItem);
                }
            }
            nbItems = 0;
            this.templates.LoadSyntax(Documents.SyntaxDirectory);
            foreach (o2Mate.Syntax syn in this.templates.Syntaxes)
            {
                ToolStripItem tsi = this.syntaxesToolStripMenuItem.DropDownItems.Add(syn.Name);
                ToolStripMenuItem menu = tsi as ToolStripMenuItem;
                ++nbItems;
                this.listSyntaxRef.Add("syntax" + nbItems.ToString(), syn);
                menu.Name = "syntax" + nbItems.ToString();
                menu.Click += new EventHandler(syntax_Click);
            }
            this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
            web.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
            web.Navigate(Documents.EditorPage);
            this.webBrowser1.Navigate(Documents.ReadOnlyViewPage);
        }

        void syntax_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            this.currentSyntaxView = this.listSyntaxRef[item.Name];
            this.viewType = ViewType.Syntax;
            if (!this.openView)
            {
                this.timView.Start();
            }
            else
            {
                this.InitializeSyntax();
            }
        }

        void skeleton_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            this.currentSkeletonView = this.listSkeletonRef[item.Name];
            this.viewType = ViewType.Skeleton;
            if (!this.openView)
            {
                this.timView.Start();
            }
            else
            {
                this.InitializeSkeleton();
            }
        }

        void template_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            this.currentTemplateView = this.listTemplateRef[item.Name];
            this.viewType = ViewType.Template;
            if (!this.openView)
            {
                this.timView.Start();
            }
            else
            {
                this.InitializeTemplate();
            }
        }

        void web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if ((sender as WebBrowser).ReadyState == WebBrowserReadyState.Complete && (sender as WebBrowser).Url == new Uri(Documents.EditorPage))
            {
                web.Document.GetElementById("documentName").KeyUp += new HtmlElementEventHandler(Form1_KeyUp);
                web.Document.GetElementById("callback").Click += new HtmlElementEventHandler(callback_Click);
            }
        }

        void Form1_KeyUp(object sender, HtmlElementEventArgs e)
        {
            if (e.KeyPressedCode == 13)
            {
                // changer le focus
                HtmlElement body = web.Document.Body;
                body.Focus();
                HtmlElement elem = sender as HtmlElement;
                if (String.IsNullOrEmpty(elem.GetAttribute("value")))
                {
                    web.Document.InvokeScript("errorFileNameExists", new object[] { "not empty" });
                    this.sauvegarderToolStripMenuItem.Enabled = false;
                    this.btnSave.Enabled = false;
                    this.générerToolStripMenuItem.Enabled = false;
                    this.btnGenerate.Enabled = false;
                }
                else
                {
                    if (Path.GetFileNameWithoutExtension(this._fileName) != elem.GetAttribute("value"))
                    {
                        FileInfo fi = new FileInfo(this.directorySource + "\\" + elem.GetAttribute("value") + ".xml");
                        if (fi.Exists)
                        {
                            web.Document.InvokeScript("errorFileNameExists", new object[] { "already exists" });
                            this.sauvegarderToolStripMenuItem.Enabled = false;
                            this.btnSave.Enabled = false;
                            this.générerToolStripMenuItem.Enabled = false;
                            this.btnGenerate.Enabled = false;
                        }
                        else
                        {
                            web.Document.InvokeScript("noErrorFileName");
                            FileInfo fiSrc = new FileInfo(this.directorySource + "\\" + this._fileName);
                            fiSrc.MoveTo(this.directorySource + "\\" + elem.GetAttribute("value") + ".xml");
                            this._fileName = elem.GetAttribute("value") + ".xml";
                            this.Text = CodeCommander.Documents.ProgramName + " - " + this._fileName;
                            this.sauvegarderToolStripMenuItem.Enabled = true;
                            this.btnSave.Enabled = true;
                            // impossible de générer un template
                            if (this.directorySource == Documents.SourcesDirectory)
                            {
                                this.générerToolStripMenuItem.Enabled = true;
                                this.btnGenerate.Enabled = true;
                            }
                            else
                            {
                                this.générerToolStripMenuItem.Enabled = false;
                                this.btnGenerate.Enabled = false;
                            }
                        }
                    }
                }
            }
        }

        void callback_Click(object sender, HtmlElementEventArgs e)
        {
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            string action = web.Document.GetElementById("callback").GetAttribute("action");
            if (action == "open")
            {
                // to get the data, we use an attribute (because the innerHTML parses the xml and return null)
                this.pp = new PopupProgress(true);
                this.bw = new BackgroundWorker();
                this.bw.WorkerReportsProgress = true;
                this.bw.WorkerSupportsCancellation = true;
                this.bw.DoWork += new DoWorkEventHandler(RunThreadDisplayXml);
                this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunThreadDisplayCompleted);
                this.pp.Cancel += new EventHandler(delegate(object s, EventArgs ea) { this.bw.CancelAsync(); });

                ThreadObject to = new ThreadObject(new object[] { comp, web.Document.GetElementById("callback").GetAttribute("data"), web, this.pp, 0, false, true, SynchronizationContext.Current });
                this.bw.RunWorkerAsync(to);

                to.Wait.WaitOne();
                if (!this.pp.IsFinished)
                    this.pp.ShowDialog();
            }
            else if (action == "close")
            {
                string window = web.Document.GetElementById("callback").GetAttribute("window");
                // we save the data template into the xml property of the template
                string xmlData = comp.SaveToString(web.Document.GetElementById(window));
                string template = web.Document.GetElementById("callback").GetAttribute("template");
                HtmlElement obj = web.Document.GetElementById(template);
                HtmlElement xmlContainer = this.GetElementByName(obj, "xml");
                // xmlData is already in HTML encoded format
                xmlContainer.InnerHtml = xmlData;
            }
            else if (action == "copy")
            {
                this.copierToolStripMenuItem_Click(sender, new EventArgs());
            }
            else if (action == "paste")
            {
                if (Clipboard.ContainsData("CodeCommander-src"))
                {
                    int indent = 0;

                    this.pp = new PopupProgress(true);
                    this.bw = new BackgroundWorker();
                    this.bw.WorkerReportsProgress = true;
                    this.bw.WorkerSupportsCancellation = true;
                    this.bw.DoWork += new DoWorkEventHandler(RunThreadDisplayXml);
                    this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunThreadDisplayCompleted);
                    this.pp.Cancel += new EventHandler(delegate(object s, EventArgs ea) { this.bw.CancelAsync(); });

                    Int32.TryParse(web.Document.GetElementById("callback").GetAttribute("indent"), out indent);
                    ThreadObject to = new ThreadObject(new object[] { comp, Clipboard.GetData("CodeCommander-src").ToString(), web, this.pp, indent, true, true, SynchronizationContext.Current });
                    this.bw.RunWorkerAsync(to);

                    to.Wait.WaitOne();
                    if (!this.pp.IsFinished)
                        this.pp.ShowDialog();
                }
            }
            else if (action == "search")
            {
                string searchString = web.Document.GetElementById("callback").GetAttribute("searchString");
                string repositoryId = web.Document.GetElementById("callback").GetAttribute("repositoryId");
                string repositoryType = web.Document.GetElementById("callback").GetAttribute("repositoryType");
                o2Mate.Dictionnaire dict = new o2Mate.Dictionnaire();
                dict.AddString("searchString", searchString);
                o2Mate.Array arr = new o2Mate.Array();
                if (repositoryType == "template")
                {
                    List<o2Mate.Template> tps = this.templates.SearchTemplate(searchString);
                    foreach (o2Mate.Template t in tps)
                    {
                        o2Mate.Fields fields = new o2Mate.Fields();
                        fields.AddString("path", t.Path);
                        fields.AddString("name", t.Name);
                        fields.AddString("data", t.Path + "/" + t.Name);
                        arr.Add(fields);
                    }
                    dict.AddArray("templates", arr);
                    dict.Save(Documents.DictionaryTemplate);
                    comp.Compilation(Documents.OutilSearchTemplate, Documents.DictionaryTemplate, Documents.SearchResult, null);
                }
                else if (repositoryType == "syntax")
                {
                    List<o2Mate.Syntax> syns = this.templates.SearchSyntax(searchString);
                    foreach (o2Mate.Syntax s in syns)
                    {
                        o2Mate.Fields fields = new o2Mate.Fields();
                        fields.AddString("name", s.Name);
                        arr.Add(fields);
                    }
                    dict.AddArray("syntax", arr);
                    dict.Save(Documents.DictionarySyntax);
                    comp.Compilation(Documents.OutilSearchSyntax, Documents.DictionarySyntax, Documents.SearchResult, null);
                }
                StreamReader sr = new StreamReader(Documents.SearchResult);
                string result = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                try
                {
                    web.Document.GetElementById(repositoryId).InnerHtml = result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else if (action == "reload")
            {
                if (this.sauvegarderToolStripMenuItem.Enabled)
                    this.sauvegarderToolStripMenuItem_Click(sender, new EventArgs());
                if (!this.web.IsDisposed)
                    this.web.Navigate(Documents.EditorPage);
            }
            else if (action == "runSyntax")
            {
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                comp.LoadSyntax(Documents.SyntaxDirectory);
                string syntaxName = web.Document.GetElementById("callback").GetAttribute("syntaxName");
                o2Mate.Syntax syn = comp.GetSyntax(syntaxName);
                comp.Save(Documents.TempDirectory + "\\syntax.xml", syn.XmlCode, syn.Legendes);
                o2Mate.Dictionnaire dict = comp.OutputDictionary(Documents.TempDirectory + "\\syntax.xml");
                SyntaxForm synForm = new SyntaxForm(Documents.TempDirectory + "\\syntax.xml", dict);
                string value = synForm.Compile().Replace("\r\n", "¶\r\n").Replace(' ', '·').Replace('\t', '¬');
                this.web.Document.InvokeScript("writeSyntax", new object[] { value });
            }
            else if (action == "store")
            {
                string repositoryId = web.Document.GetElementById("callback").GetAttribute("repositoryId");
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                o2Mate.Dictionnaire dict = new o2Mate.Dictionnaire();
                o2Mate.Array arr = new o2Mate.Array();
                foreach(string s in this.cutter)
                {
                    o2Mate.Fields fields = new o2Mate.Fields();
                    fields.AddString("string", s);
                    arr.Add(fields);
                }
                dict.AddArray("cutter", arr);
                dict.Save(Documents.DictionaryTemplate);
                comp.Compilation(Documents.OutilCutter, Documents.DictionaryTemplate, Documents.SearchResult, null);
                StreamReader sr = new StreamReader(Documents.SearchResult);
                string result = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                try
                {
                    web.Document.GetElementById(repositoryId).InnerHtml = result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else if (action == "cutAndStore")
            {
                string text = web.Document.GetElementById("callback").GetAttribute("text");
                this.cutter.Add(text);
            }
            else if (action == "initDnD")
            {
                string repositoryType = web.Document.GetElementById("callback").GetAttribute("repositoryType");
                string target = web.Document.GetElementById("callback").GetAttribute("target");
                if (repositoryType == "paste")
                {
                    string text = web.Document.GetElementById(target).InnerText;
                    web.DoDragDrop(text, DragDropEffects.Copy);
                }
                else if (repositoryType == "syntax")
                {
                    comp.LoadTemplates(Documents.TemplatesDirectory);
                    comp.LoadSkeletons(Documents.SkeletonsDirectory);
                    comp.LoadSyntax(Documents.SyntaxDirectory);
                    string syntaxName = web.Document.GetElementById(target).InnerText;
                    o2Mate.Syntax syn = comp.GetSyntax(syntaxName);
                    comp.Save(Documents.TempDirectory + "\\syntax.xml", syn.XmlCode, syn.Legendes);
                    o2Mate.Dictionnaire dict = comp.OutputDictionary(Documents.TempDirectory + "\\syntax.xml");
                    foreach (string s in dict.StringKeys)
                    {
                        dict.SetString(s, s);
                    }
                    foreach (string s in dict.ArrayKeys)
                    {
                        o2Mate.Array arr = dict.GetArray(s) as o2Mate.Array;
                        o2Mate.Fields f = arr.Item(1) as o2Mate.Fields;
                        System.Collections.IEnumerator en = f.Keys.GetEnumerator();
                        while(en.MoveNext())
                        {
                            f.AddString(en.Current.ToString(), en.Current.ToString());
                        }
                    }
                    string fileNameDict = Documents.TempDirectory + "\\dict.xml";
                    dict.Save(fileNameDict);
                    string fileNameSrc = Documents.TempDirectory + "\\syntax.xml";
                    FileInfo fi = new FileInfo(fileNameSrc);
                    string outputFinalFile = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".txt";
                    string result = String.Empty;
                    try
                    {
                        // le script est déjà chargé
                        comp.Compilation(fileNameDict, outputFinalFile, null);
                        StreamReader sr = new StreamReader(outputFinalFile);
                        result = sr.ReadToEnd();
                        sr.Close();
                        sr.Dispose();
                    }
                    catch { }
                    result = result.Replace("\r\n", "¶\r\n").Replace(' ', '·').Replace('\t', '¬');
                    web.DoDragDrop(result, DragDropEffects.Copy);
                }
            }
            else if (action == "Dirty")
            {
                this.Text = (this.Text + " *").Substring(0, this.Text.Length + 2);
            }
        }

        private void copyBtn_Click(object sender, EventArgs e)
        {
            this.copierToolStripMenuItem_Click(sender, e);
        }

        private void cutBtn_Click(object sender, EventArgs e)
        {
            HtmlElement clip = web.Document.GetElementById("copyToClipboard");
            if (clip.GetAttribute("ready") == YES)
            {
                string from = clip.GetAttribute("from");
                string to = clip.GetAttribute("to");
                HtmlElement cutted = web.Document.CreateElement("srcCut");
                HtmlElement src = web.Document.GetElementById("src");
                bool startingCut = false;
                foreach (HtmlElement child in src.Children)
                {
                    if (child.Id == from)
                    {
                        startingCut = true;
                    }
                    if (!startingCut)
                    {
                        cutted.AppendChild(child);
                    }
                    if (child.Id == to)
                    {
                        startingCut = false;
                    }
                }
                src.InnerHtml = cutted.InnerHtml;
            }
        }

        private void pasteBtn_Click(object sender, EventArgs e)
        {
            web.Document.InvokeScript("paste");
        }

        private void sauvegarderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int countPopup = (int)web.Document.InvokeScript("getCountPopup");
            bool hasUndoInPopup = (bool)web.Document.InvokeScript("hasUndoInPopup");
            bool isHiddenClose = (bool)web.Document.InvokeScript("isHiddenClose");
            if (countPopup > 0 && (hasUndoInPopup || isHiddenClose))
            {
                MessageBox.Show("Fermez d'abord la popup", "Popup en cours modifiée", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            HtmlElement body = web.Document.Body;
            // on donne le focus pour que l'undo fonctionne correctement
            body.Focus();
            HtmlElement src = web.Document.GetElementById("src");
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            this._fileName = web.Document.GetElementById("documentName").GetAttribute("value") + ".xml";
            bool isDirty = (bool)web.Document.InvokeScript("getDirty");
            int rev = 0;
            Int32.TryParse(web.Document.GetElementById("revision").GetAttribute("value"), out rev);
            comp.InputLegendes(web.Document.GetElementById("legendes0"));
            this.pp = new PopupProgress(false);
            ThreadObject to = new ThreadObject(new object[] { comp, this.directorySource + "\\" + web.Document.GetElementById("documentName").GetAttribute("value") + ".xml",
                                                src, this.pp, web.Document.GetElementById("creationDate").InnerText, web.Document.GetElementById("modifiedDate").InnerText,
                                                rev > 0 && isDirty ? (rev+1).ToString() : web.Document.GetElementById("revision").GetAttribute("value")});
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunThreadSave), to);
            to.Wait.WaitOne(2000);
            if (!this.pp.IsFinished)
                this.pp.ShowDialog();
            if (rev > 0 && isDirty)
                web.Document.GetElementById("revision").SetAttribute("value", (rev+1).ToString());
            web.Document.InvokeScript("clearDirty");
            this.Text = CodeCommander.Documents.ProgramName + " - " + this._fileName;
            // impossible de générer un template
            if (this.directorySource == Documents.SourcesDirectory)
            {
                this.générerToolStripMenuItem.Enabled = true;
                this.btnGenerate.Enabled = true;
            }
            else
            {
                this.générerToolStripMenuItem.Enabled = false;
                this.btnGenerate.Enabled = false;
            }
        }

        private void nouveauToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HtmlElement body = web.Document.Body;
            // on donne le focus pour que l'undo fonctionne correctement
            body.Focus();
            int countPopup = (int)web.Document.InvokeScript("getCountPopup");
            bool hasUndoInPopup = (bool)web.Document.InvokeScript("hasUndoInPopup");
            bool isHiddenClose = (bool)web.Document.InvokeScript("isHiddenClose");
            if (countPopup > 0 && (hasUndoInPopup || isHiddenClose))
            {
                MessageBox.Show("Fermez d'abord la popup", "Popup en cours modifiée", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            bool isDirty = (bool)web.Document.InvokeScript("getDirty");
            if (isDirty)
            {
                DialogResult dr = MessageBox.Show("Save the current document before ?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    this.sauvegarderToolStripMenuItem_Click(sender, e);
                }
                else if (dr == DialogResult.Cancel)
                {
                    return;
                }
                else
                {
                    web.Document.InvokeScript("clearDirty");
                }
            }
            web.Document.InvokeScript("CloseDocument");
            web.Document.GetElementById("src").InnerHtml = "";
            web.Document.GetElementById("selectionArea").InnerHtml = "";
            string fileName = this.SearchNewFileName();
            this._fileName = fileName + ".xml";
            this.directorySource = Documents.SourcesDirectory;
            FileInfo fi = new FileInfo(this.directorySource + "\\" + this._fileName);
            // we create the file now with the correct format
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            web.Document.InvokeScript("clearUndo");
            String date = DateTime.Now.ToString();
            web.Document.InvokeScript("openHeader", new object[] { fileName, date, "", "1" });
            web.Document.InvokeScript("newDocument", new object[] { "Write anything" });
            comp.Save(fi.FullName, web.Document.GetElementById("src"), null, fileName, date, "", "1");
            o2Mate.ILegendeDict dict = comp.OutputLegendes(fi.FullName);
            web.Document.InvokeScript("LoadMasterLegendes", new object[] { dict });
            this.Text = CodeCommander.Documents.ProgramName + " - " + this._fileName;
            this.sauvegarderToolStripMenuItem.Enabled = true;
            this.btnSave.Enabled = true;
            this.générerToolStripMenuItem.Enabled = false;
            this.btnGenerate.Enabled = false;
        }

        private string SearchNewFileName()
        {
            DirectoryInfo di = new DirectoryInfo(Documents.SourcesDirectory);
            return "Unknown-" + (di.GetFiles("Unknown-*.xml").Length + 1).ToString();
        }

        private void ouvrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HtmlElement body = web.Document.Body;
            // on donne le focus pour que l'undo fonctionne correctement
            body.Focus();
            int countPopup = (int)web.Document.InvokeScript("getCountPopup");
            bool hasUndoInPopup = (bool)web.Document.InvokeScript("hasUndoInPopup");
            bool isHiddenClose = (bool)web.Document.InvokeScript("isHiddenClose");
            if (countPopup > 0 && (hasUndoInPopup || isHiddenClose))
            {
                MessageBox.Show("Fermez d'abord la popup", "Popup en cours modifiée", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            Open open = new Open(Documents.SourcesDirectory);
            DialogResult dr = open.ShowDialog();
            if (dr == DialogResult.OK)
            {
                bool isDirty = (bool)web.Document.InvokeScript("getDirty");
                if (isDirty)
                {
                    DialogResult drSave = MessageBox.Show("Sauvegarder le fichier en cours ?", "Sauvegarder", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (drSave == DialogResult.Yes)
                    {
                        this.sauvegarderToolStripMenuItem_Click(sender, e);
                    }
                    else if (drSave == DialogResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        web.Document.InvokeScript("clearDirty");
                    }
                }
                this._fileName = open.FileName;
                web.Document.InvokeScript("clearUndo");
                web.Document.InvokeScript("CloseDocument");
                web.Document.GetElementById("src").InnerHtml = "";
                web.Document.GetElementById("selectionArea").InnerHtml = "";
                o2Mate.Compilateur comp = new o2Mate.Compilateur();
                try
                {
                    this.directorySource = Documents.SourcesDirectory;
                    o2Mate.ILegendeDict dict = comp.OutputLegendes(this.directorySource + "\\" + this._fileName);
                    web.Document.InvokeScript("LoadMasterLegendes", new object[] { dict });

                    this.pp = new PopupProgress(true);
                    this.bw = new BackgroundWorker();
                    this.bw.WorkerReportsProgress = true;
                    this.bw.WorkerSupportsCancellation = true;
                    this.bw.DoWork += new DoWorkEventHandler(RunThreadDisplay);
                    this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunThreadDisplayCompleted);
                    this.pp.Cancel += new EventHandler(delegate(object s, EventArgs ea) { this.bw.CancelAsync(); });

                    ThreadObject to = new ThreadObject(new object[] { comp, this.directorySource + "\\" + this._fileName, web, true, this.pp, SynchronizationContext.Current });
                    this.bw.RunWorkerAsync(to);

                    to.Wait.WaitOne();
                    if (!this.pp.IsFinished)
                        this.pp.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                this.Text = CodeCommander.Documents.ProgramName + " - " + this._fileName;
                this.sauvegarderToolStripMenuItem.Enabled = true;
                this.btnSave.Enabled = true;
                this.générerToolStripMenuItem.Enabled = true;
                this.btnGenerate.Enabled = true;
            }
        }

        private void générerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HtmlElement body = web.Document.Body;
            // on donne le focus pour que l'undo fonctionne correctement
            body.Focus();
            int countPopup = (int)web.Document.InvokeScript("getCountPopup");
            bool hasUndoInPopup = (bool)web.Document.InvokeScript("hasUndoInPopup");
            bool isHiddenClose = (bool)web.Document.InvokeScript("isHiddenClose");
            if (countPopup > 0 && (hasUndoInPopup || isHiddenClose))
            {
                MessageBox.Show("Fermez d'abord la popup", "Popup en cours modifiée", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            bool isDirty = (bool)web.Document.InvokeScript("getDirty");
            if (isDirty)
            {
                // generate needs to save the file before
                this.sauvegarderToolStripMenuItem_Click(sender, e);
            }
            try
            {
                o2Mate.Compilateur comp = new o2Mate.Compilateur();
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                o2Mate.Dictionnaire dict = comp.OutputDictionary(Documents.SourcesDirectory + this._fileName);
                DictForm dt = new DictForm(Documents.SourcesDirectory + this._fileName, dict);
                dt.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    this.copierToolStripMenuItem_Click(sender, new EventArgs());
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Debug.Assert(!this.web.IsBusy);
            HtmlElement body = web.Document.Body;
            // on donne le focus pour que l'undo fonctionne correctement
            body.Focus();
            int countPopup = (int)web.Document.InvokeScript("getCountPopup");
            bool hasUndoInPopup = (bool)web.Document.InvokeScript("hasUndoInPopup");
            bool isHiddenClose = (bool)web.Document.InvokeScript("isHiddenClose");
            if (countPopup > 0 && (hasUndoInPopup || isHiddenClose))
            {
                MessageBox.Show("Fermez d'abord la popup", "Popup en cours modifiée", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                e.Cancel = true;
                return;
            }
            bool isDirty = (bool)web.Document.InvokeScript("getDirty");
            if (isDirty && this.btnSave.Enabled)
            {
                DialogResult drSave = MessageBox.Show("Sauvegarder le fichier en cours ?", "Quitter", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (drSave == DialogResult.Yes)
                {
                    this.sauvegarderToolStripMenuItem_Click(sender, e);
                    // on désactive l'autorisation de sauvegarder au moment du unload du navigateur
                    this.sauvegarderToolStripMenuItem.Enabled = false;
                    // dispose web
                    this.web.Dispose();
                }
                else if (drSave == DialogResult.No)
                {
                    // on désactive l'autorisation de sauvegarder au moment du unload du navigateur
                    this.sauvegarderToolStripMenuItem.Enabled = false;
                    // dispose web
                    this.web.Dispose();
                }
                else if (drSave == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            else
            {
                // on désactive l'autorisation de sauvegarder au moment du unload du navigateur
                this.sauvegarderToolStripMenuItem.Enabled = false;
                // dispose web
                this.web.Dispose();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.nouveauToolStripMenuItem_Click(sender, e);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            this.ouvrirToolStripMenuItem_Click(sender, e);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.sauvegarderToolStripMenuItem_Click(sender, e);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            web.Document.InvokeScript("undo");
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            web.Document.InvokeScript("redo");
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            this.générerToolStripMenuItem_Click(sender, e);
        }

        private void annulerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            web.Document.InvokeScript("undo");
        }

        private void rétablirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            web.Document.InvokeScript("redo");
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int countPopup = (int)web.Document.InvokeScript("getCountPopup");
            bool hasUndoInPopup = (bool)web.Document.InvokeScript("hasUndoInPopup");
            bool isHiddenClose = (bool)web.Document.InvokeScript("isHiddenClose");
            if (countPopup > 0 && (hasUndoInPopup || isHiddenClose))
            {
                MessageBox.Show("Fermez d'abord la popup", "Popup en cours modifiée", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            HtmlElement body = web.Document.Body;
            body.Focus();
            Open open = new Open(Documents.TemplatesDirectory);
            DialogResult dr = open.ShowDialog();
            if (dr == DialogResult.OK)
            {
                bool isDirty = (bool)web.Document.InvokeScript("getDirty");
                if (isDirty)
                {
                    DialogResult drSave = MessageBox.Show("Sauvegarder le fichier en cours ?", "Sauvegarder", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (drSave == DialogResult.Yes)
                    {
                        this.sauvegarderToolStripMenuItem_Click(sender, e);
                    }
                    else
                    {
                        web.Document.InvokeScript("clearDirty");
                    }
                }
                this._fileName = open.FileName;
                web.Document.InvokeScript("CloseDocument");
                web.Document.GetElementById("src").InnerHtml = "";
                web.Document.GetElementById("selectionArea").InnerHtml = "";
                o2Mate.Compilateur comp = new o2Mate.Compilateur();
                try
                {
                    this.directorySource = Documents.TemplatesDirectory;
                    o2Mate.ILegendeDict dict = comp.OutputLegendes(this.directorySource + "\\" + this._fileName);
                    web.Document.InvokeScript("LoadMasterLegendes", new object[] { dict });

                    this.pp = new PopupProgress(true);
                    this.bw = new BackgroundWorker();
                    this.bw.WorkerReportsProgress = true;
                    this.bw.WorkerSupportsCancellation = true;
                    this.bw.DoWork += new DoWorkEventHandler(RunThreadDisplay);
                    this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunThreadDisplayCompleted);
                    this.pp.Cancel += new EventHandler(delegate(object s, EventArgs ea) { this.bw.CancelAsync(); });

                    ThreadObject to = new ThreadObject(new object[] { comp, this.directorySource + "\\" + this._fileName, web, true, this.pp, SynchronizationContext.Current });
                    this.bw.RunWorkerAsync(to);

                    to.Wait.WaitOne();
                    if (!this.pp.IsFinished)
                        this.pp.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                this.Text = CodeCommander.Documents.ProgramName + " - " + this._fileName;
                this.sauvegarderToolStripMenuItem.Enabled = true;
                this.btnSave.Enabled = true;
                this.générerToolStripMenuItem.Enabled = false;
                this.btnGenerate.Enabled = true;
                web.Document.InvokeScript("clearUndo");
            }
        }

        private void aProposDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }

        private void RunThreadSave(object infos)
        {
            ThreadObject datas = infos as ThreadObject;
            o2Mate.Compilateur comp = datas.Datas[0] as o2Mate.Compilateur;
            string filename = (string)datas.Datas[1];
            HtmlElement src = datas.Datas[2] as HtmlElement;
            o2Mate.INotifyProgress pp = datas.Datas[3] as o2Mate.INotifyProgress;
            string creationDate = (string)datas.Datas[4];
            string modifiedDate = (string)datas.Datas[5];
            string revision = (string)datas.Datas[6];
            comp.Save(filename, src,
                pp,
                "",
                creationDate,
                modifiedDate,
                revision);
        }

        private void RunThreadDisplay(object sender, DoWorkEventArgs e)
        {
            ThreadObject datas = e.Argument as ThreadObject;
            o2Mate.Compilateur comp = datas.Datas[0] as o2Mate.Compilateur;
            string filename = (string)datas.Datas[1];
            WebBrowser webSrc = datas.Datas[2] as WebBrowser;
            bool writable = (bool)datas.Datas[3];
            o2Mate.INotifyProgress pp = datas.Datas[4] as o2Mate.INotifyProgress;
            SynchronizationContext synchro = datas.Datas[5] as SynchronizationContext;

            DisplayWorker dw = new DisplayWorker(pp);
            BackgroundWorker worker = sender as BackgroundWorker;

            pp.Start(true);
            BindingList<o2Mate.DisplayElement> webContent = new BindingList<o2Mate.DisplayElement>();
            comp.Display(filename, webContent, writable, pp);
            int index = 1;
            foreach (o2Mate.DisplayElement de in webContent)
            {
                if (pp.IsCanceled) break;
                de.Percent = (int)(100 * (index / (double)webContent.Count));
                dw.InvokeBrowser(de, webSrc);
                ++index;
            }
            pp.IsFinished = true;
        }

        private void RunThreadDisplayXml(object sender, DoWorkEventArgs e)
        {
            ThreadObject datas = e.Argument as ThreadObject;
            o2Mate.Compilateur comp = datas.Datas[0] as o2Mate.Compilateur;
            string xmlString = (string)datas.Datas[1];
            WebBrowser webSrc = datas.Datas[2] as WebBrowser;
            o2Mate.INotifyProgress pp = datas.Datas[3] as o2Mate.INotifyProgress;
            int indent = (int)datas.Datas[4];
            bool forceIndent = (bool)datas.Datas[5];
            bool writable = (bool)datas.Datas[6];
            SynchronizationContext synchro = datas.Datas[7] as SynchronizationContext;

            BackgroundWorker worker = sender as BackgroundWorker;
            DisplayWorker dw = new DisplayWorker(pp);

            pp.Start(true);
            BindingList<o2Mate.DisplayElement> webContent = new BindingList<o2Mate.DisplayElement>();
            comp.DisplayXML(xmlString, webContent, forceIndent, indent, writable, this.pp);
            int index = 1;
            foreach (o2Mate.DisplayElement de in webContent)
            {
                if (pp.IsCanceled) break;
                de.Percent = (int)(100 * (index / (double)webContent.Count));
                dw.InvokeBrowser(de, webSrc);
                ++index;
            }
            pp.IsFinished = true;
        }

        private void RunThreadDisplaySkeleton(object sender, DoWorkEventArgs e)
        {
            ThreadObject datas = e.Argument as ThreadObject;
            o2Mate.Compilateur comp = datas.Datas[0] as o2Mate.Compilateur;
            o2Mate.Skeleton skel = datas.Datas[1] as o2Mate.Skeleton;
            WebBrowser webSrc = datas.Datas[2] as WebBrowser;
            o2Mate.INotifyProgress pp = datas.Datas[3] as o2Mate.INotifyProgress;
            SynchronizationContext synchro = datas.Datas[4] as SynchronizationContext;

            BackgroundWorker worker = sender as BackgroundWorker;
            DisplayWorker dw = new DisplayWorker(pp);

            pp.Start(true);
            BindingList<o2Mate.DisplayElement> webContent = new BindingList<o2Mate.DisplayElement>();
            comp.DisplaySkeleton(skel, webContent);
            int index = 1;
            foreach (o2Mate.DisplayElement de in webContent)
            {
                if (pp.IsCanceled) break;
                de.Percent = (int)(100 * (index / (double)webContent.Count));
                dw.InvokeBrowser(de, webSrc);
                ++index;
            }
            pp.IsFinished = true;
        }

        void RunThreadDisplayCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.pp.GiveException(e.Error);
            }
        }

        private void copierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            HtmlElement clip = web.Document.GetElementById("copyToClipboard");
            if (clip.GetAttribute("ready") == YES)
            {
                string content = (string)web.Document.InvokeScript("getCurrentContent");
                HtmlElement src = web.Document.GetElementById(content);
                string data = "";
                string objects = (string)clip.GetAttribute("objects");
                HtmlElement selObjects = web.Document.GetElementById(objects);
                foreach (HtmlElement elem in selObjects.Children)
                {
                    string toolId = (string)elem.GetAttribute("tool");
                    HtmlElement tool = web.Document.GetElementById(toolId);
                    data += comp.SaveChildToString(tool);
                }
                Clipboard.SetData("CodeCommander-src", data);
                clip.SetAttribute("ready", NO);
                web.Document.InvokeScript("endCopy");
            }
        }

        private void couperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            web.Document.InvokeScript("InitNames");
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            HtmlElement clip = web.Document.GetElementById("copyToClipboard");
            if (clip.GetAttribute("ready") == YES)
            {
                string content = (string)web.Document.InvokeScript("getCurrentContent");
                HtmlElement src = web.Document.GetElementById(content);
                string data = "";
                string objects = (string)clip.GetAttribute("objects");
                HtmlElement selObjects = web.Document.GetElementById(objects);
                foreach (HtmlElement elem in selObjects.Children)
                {
                    string toolId = (string)elem.GetAttribute("tool");
                    HtmlElement tool = web.Document.GetElementById(toolId);
                    data += comp.SaveChildToString(tool);
                }
                Clipboard.SetData("CodeCommander-src", data);
                clip.SetAttribute("ready", NO);
                web.Document.InvokeScript("endCut");
            }
        }

        private void collerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsData("CodeCommander-src"))
            {
                this.web.Document.InvokeScript("InitNames");
                o2Mate.Compilateur comp = new o2Mate.Compilateur();
                // il faut supprimer le dernier paste
                this.web.Document.InvokeScript("suppressEndPaste");

                this.pp = new PopupProgress(true);
                this.bw = new BackgroundWorker();
                this.bw.WorkerReportsProgress = true;
                this.bw.WorkerSupportsCancellation = true;
                this.bw.DoWork += new DoWorkEventHandler(RunThreadDisplayXml);
                this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunThreadDisplayCompleted);
                this.pp.Cancel += new EventHandler(delegate(object s, EventArgs ea) { this.bw.CancelAsync(); });

                ThreadObject to = new ThreadObject(new object[] { comp, Clipboard.GetData("CodeCommander-src").ToString(), web, this.pp, 0, true, true, SynchronizationContext.Current });
                this.bw.RunWorkerAsync(to);

                to.Wait.WaitOne();
                if (!this.pp.IsFinished)
                    this.pp.ShowDialog();

                this.web.Document.InvokeScript("endWithPaste");
                this.web.Document.InvokeScript("endPaste");
            }
        }

        private void InitializeTemplate()
        {
            this.webBrowser1.Document.GetElementById("title").InnerText = "Nom du template";
            this.webBrowser1.Document.GetElementById("documentName").InnerText = this.currentTemplateView.Path + "/" + this.currentTemplateView.Name;
            this.webBrowser1.Document.GetElementById("parameters").InnerText = this.currentTemplateView.Parameters;
            this.webBrowser1.Document.GetElementById("src").InnerHtml = "";

            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            this.pp = new PopupProgress(true);
            this.bw = new BackgroundWorker();
            this.bw.WorkerReportsProgress = true;
            this.bw.WorkerSupportsCancellation = true;
            this.bw.DoWork += new DoWorkEventHandler(RunThreadDisplayXml);
            this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunThreadDisplayCompleted);
            this.pp.Cancel += new EventHandler(delegate(object s, EventArgs ea) { this.bw.CancelAsync(); });

            ThreadObject to = new ThreadObject(new object[] { comp, this.currentTemplateView.XmlCode, this.webBrowser1, this.pp, 0, false, false, SynchronizationContext.Current });
            this.bw.RunWorkerAsync(to);

            to.Wait.WaitOne();
            if (!this.pp.IsFinished)
                this.pp.ShowDialog();
        }

        private void InitializeSkeleton()
        {
            this.webBrowser1.Document.GetElementById("title").InnerText = "Nom du squelette";
            this.webBrowser1.Document.GetElementById("documentName").InnerText = this.currentSkeletonView.Path + "/" + this.currentSkeletonView.Name;
//            this.webBrowser1.Document.GetElementById("parameters").InnerText = this.currentTemplateView.Parameters;
            this.webBrowser1.Document.GetElementById("src").InnerHtml = "";

            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            this.pp = new PopupProgress(true);
            this.bw = new BackgroundWorker();
            this.bw.WorkerReportsProgress = true;
            this.bw.WorkerSupportsCancellation = true;
            this.bw.DoWork += new DoWorkEventHandler(RunThreadDisplaySkeleton);
            this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunThreadDisplayCompleted);
            this.pp.Cancel += new EventHandler(delegate(object s, EventArgs ea) { this.bw.CancelAsync(); });

            ThreadObject to = new ThreadObject(new object[] { comp, this.currentSkeletonView, this.webBrowser1, this.pp, SynchronizationContext.Current });
            this.bw.RunWorkerAsync(to);

            to.Wait.WaitOne();
            if (!this.pp.IsFinished)
                this.pp.ShowDialog();
        }

        private void InitializeSyntax()
        {
            this.webBrowser1.Document.GetElementById("title").InnerText = "Nom de syntaxe";
            this.webBrowser1.Document.GetElementById("documentName").InnerText = this.currentSyntaxView.Name;
            this.webBrowser1.Document.GetElementById("src").InnerHtml = "";

            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            this.pp = new PopupProgress(true);
            this.bw = new BackgroundWorker();
            this.bw.WorkerReportsProgress = true;
            this.bw.WorkerSupportsCancellation = true;
            this.bw.DoWork += new DoWorkEventHandler(RunThreadDisplayXml);
            this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunThreadDisplayCompleted);
            this.pp.Cancel += new EventHandler(delegate(object s, EventArgs ea) { this.bw.CancelAsync(); });

            ThreadObject to = new ThreadObject(new object[] { comp, this.currentSyntaxView.XmlCode, this.webBrowser1, this.pp, 0, false, false, SynchronizationContext.Current });
            this.bw.RunWorkerAsync(to);

            to.Wait.WaitOne();
            if (!this.pp.IsFinished)
                this.pp.ShowDialog();
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if ((sender as WebBrowser).ReadyState == WebBrowserReadyState.Complete && (sender as WebBrowser).Url == new Uri(Documents.ReadOnlyViewPage))
            {
                this.webBrowser1.Document.GetElementById("btnClose").Click += new HtmlElementEventHandler(closeTemplateView_Click);
            }
        }

        void closeTemplateView_Click(object sender, HtmlElementEventArgs e)
        {
            if (this.openView)
            {
                this.timView.Start();
            }
        }

        private void timView_Tick(object sender, EventArgs e)
        {
            if (!this.openView)
            {
                this.webBrowser1.Visible = true;
                if (this.currentCursorView < Form1.intervalsOpenView.Length)
                {
                    this.webBrowser1.MaximumSize = new Size(this.webBrowser1.MaximumSize.Width, this.Height);
                    this.webBrowser1.Size = new Size(Form1.intervalsOpenView[this.currentCursorView], this.Height);
                    this.timView.Interval = Form1.intervalsTimer[this.currentCursorView];
                    ++this.currentCursorView;
                }
                else
                {
                    this.timView.Stop();
                    this.openView = true;
                    if (this.viewType == ViewType.Template)
                        this.InitializeTemplate();
                    else if (this.viewType == ViewType.Skeleton)
                        this.InitializeSkeleton();
                    else if (this.viewType == ViewType.Syntax)
                        this.InitializeSyntax();
                }
            }
            else
            {
                if (this.currentCursorView > 0)
                {
                    --this.currentCursorView;
                    this.webBrowser1.MaximumSize = new Size(this.webBrowser1.MaximumSize.Width, this.Height);
                    this.webBrowser1.Size = new Size(Form1.intervalsOpenView[this.currentCursorView], this.Height);
                    this.timView.Interval = Form1.intervalsTimer[this.currentCursorView];
                }
                else
                {
                    this.timView.Stop();
                    this.openView = false;
                    this.webBrowser1.Visible = false;
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.webBrowser1.MaximumSize = new Size(this.webBrowser1.MaximumSize.Width, this.Height);
            this.webBrowser1.Size = new Size(this.webBrowser1.Width, this.Height);
        }

        private void languesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.locales == null)
            {
                this.locales = new Localization();
                this.locales.Location = new Point(this.Location.X + this.Width - this.locales.Width, this.Location.Y);
            }
            this.locales.Show();
        }

        private void rechercherToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ajouterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void didactitielVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VideosHelp vh = new VideosHelp();
            vh.Location = new Point(this.Location.X + this.Width - vh.Width, 0);
            vh.Show();
        }

        private void ouvrirSkelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int countPopup = (int)web.Document.InvokeScript("getCountPopup");
            bool hasUndoInPopup = (bool)web.Document.InvokeScript("hasUndoInPopup");
            bool isHiddenClose = (bool)web.Document.InvokeScript("isHiddenClose");
            if (countPopup > 0 && (hasUndoInPopup || isHiddenClose))
            {
                MessageBox.Show("Fermez d'abord la popup", "Popup en cours modifiée", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            HtmlElement body = web.Document.Body;
            body.Focus();
            Open open = new Open(Documents.SkeletonsDirectory);
            DialogResult dr = open.ShowDialog();
            if (dr == DialogResult.OK)
            {
                bool isDirty = (bool)web.Document.InvokeScript("getDirty");
                if (isDirty)
                {
                    DialogResult drSave = MessageBox.Show("Sauvegarder le fichier en cours ?", "Sauvegarder", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (drSave == DialogResult.Yes)
                    {
                        this.sauvegarderToolStripMenuItem_Click(sender, e);
                    }
                    else
                    {
                        web.Document.InvokeScript("clearDirty");
                    }
                }
                this._fileName = open.FileName;
                web.Document.InvokeScript("CloseDocument");
                web.Document.GetElementById("src").InnerHtml = "";
                web.Document.GetElementById("selectionArea").InnerHtml = "";
                o2Mate.Compilateur comp = new o2Mate.Compilateur();
                try
                {
                    this.directorySource = Documents.SkeletonsDirectory;
                    o2Mate.ILegendeDict dict = comp.OutputLegendes(this.directorySource + "\\" + this._fileName);
                    web.Document.InvokeScript("LoadMasterLegendes", new object[] { dict });

                    this.pp = new PopupProgress(true);
                    this.bw = new BackgroundWorker();
                    this.bw.WorkerReportsProgress = true;
                    this.bw.WorkerSupportsCancellation = true;
                    this.bw.DoWork += new DoWorkEventHandler(RunThreadDisplay);
                    this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunThreadDisplayCompleted);
                    this.pp.Cancel += new EventHandler(delegate(object s, EventArgs ea) { this.bw.CancelAsync(); });

                    ThreadObject to = new ThreadObject(new object[] { comp, this.directorySource + "\\" + this._fileName, web, true, this.pp, SynchronizationContext.Current });
                    this.bw.RunWorkerAsync(to);

                    to.Wait.WaitOne();
                    if (!this.pp.IsFinished)
                        this.pp.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                this.Text = CodeCommander.Documents.ProgramName + " - " + this._fileName;
                this.sauvegarderToolStripMenuItem.Enabled = true;
                this.btnSave.Enabled = true;
                this.générerToolStripMenuItem.Enabled = false;
                this.btnGenerate.Enabled = true;
                web.Document.InvokeScript("clearUndo");
            }
        }

        private void OuvrirSyntaxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int countPopup = (int)web.Document.InvokeScript("getCountPopup");
            bool hasUndoInPopup = (bool)web.Document.InvokeScript("hasUndoInPopup");
            bool isHiddenClose = (bool)web.Document.InvokeScript("isHiddenClose");
            if (countPopup > 0 && (hasUndoInPopup || isHiddenClose))
            {
                MessageBox.Show("Fermez d'abord la popup", "Popup en cours modifiée", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            HtmlElement body = web.Document.Body;
            body.Focus();
            Open open = new Open(Documents.SyntaxDirectory);
            DialogResult dr = open.ShowDialog();
            if (dr == DialogResult.OK)
            {
                bool isDirty = (bool)web.Document.InvokeScript("getDirty");
                if (isDirty)
                {
                    DialogResult drSave = MessageBox.Show("Sauvegarder le fichier en cours ?", "Sauvegarder", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (drSave == DialogResult.Yes)
                    {
                        this.sauvegarderToolStripMenuItem_Click(sender, e);
                    }
                    else
                    {
                        web.Document.InvokeScript("clearDirty");
                    }
                }
                this._fileName = open.FileName;
                web.Document.InvokeScript("CloseDocument");
                web.Document.GetElementById("src").InnerHtml = "";
                web.Document.GetElementById("selectionArea").InnerHtml = "";
                o2Mate.Compilateur comp = new o2Mate.Compilateur();
                try
                {
                    this.directorySource = Documents.SyntaxDirectory;
                    o2Mate.ILegendeDict dict = comp.OutputLegendes(this.directorySource + "\\" + this._fileName);
                    web.Document.InvokeScript("LoadMasterLegendes", new object[] { dict });

                    this.pp = new PopupProgress(true);
                    this.bw = new BackgroundWorker();
                    this.bw.WorkerReportsProgress = true;
                    this.bw.WorkerSupportsCancellation = true;
                    this.bw.DoWork += new DoWorkEventHandler(RunThreadDisplay);
                    this.bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunThreadDisplayCompleted);
                    this.pp.Cancel += new EventHandler(delegate(object s, EventArgs ea) { this.bw.CancelAsync(); });

                    ThreadObject to = new ThreadObject(new object[] { comp, this.directorySource + "\\" + this._fileName, web, true, this.pp, SynchronizationContext.Current });
                    this.bw.RunWorkerAsync(to);

                    to.Wait.WaitOne();
                    if (!this.pp.IsFinished)
                        this.pp.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                this.Text = CodeCommander.Documents.ProgramName + " - " + this._fileName;
                this.sauvegarderToolStripMenuItem.Enabled = true;
                this.btnSave.Enabled = true;
                this.générerToolStripMenuItem.Enabled = false;
                this.btnGenerate.Enabled = true;
                web.Document.InvokeScript("clearUndo");
            }
        }

        private void ImporterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int countPopup = (int)web.Document.InvokeScript("getCountPopup");
            bool hasUndoInPopup = (bool)web.Document.InvokeScript("hasUndoInPopup");
            bool isHiddenClose = (bool)web.Document.InvokeScript("isHiddenClose");
            if (countPopup > 0 && (hasUndoInPopup || isHiddenClose))
            {
                MessageBox.Show("Fermez d'abord la popup", "Popup en cours modifiée", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            HtmlElement body = web.Document.Body;
            body.Focus();
            DialogResult dr = this.ofs.ShowDialog();
            if (dr == DialogResult.OK)
            {
                bool isDirty = (bool)web.Document.InvokeScript("getDirty");
                if (isDirty)
                {
                    DialogResult drSave = MessageBox.Show("Sauvegarder le fichier en cours ?", "Sauvegarder", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (drSave == DialogResult.Yes)
                    {
                        this.sauvegarderToolStripMenuItem_Click(sender, e);
                    }
                    else
                    {
                        web.Document.InvokeScript("clearDirty");
                    }
                }
                string fileName = this.SearchNewFileName();
                this._fileName = fileName + ".xml";
                web.Document.InvokeScript("CloseDocument");
                web.Document.GetElementById("src").InnerHtml = "";
                web.Document.GetElementById("selectionArea").InnerHtml = "";
                this.directorySource = Documents.SourcesDirectory;
                FileInfo fi = new FileInfo(this.directorySource + "\\" + this._fileName);
                // we create the file now with the correct format
                o2Mate.Compilateur comp = new o2Mate.Compilateur();
                String date = DateTime.Now.ToString();
                web.Document.InvokeScript("openHeader", new object[] { fileName, date, "", "1" });
                try
                {
                    StreamReader sr = new StreamReader(this.ofs.FileName, Encoding.GetEncoding(1252));
                    string text = sr.ReadToEnd();
                    text = text.Replace("\r\n", "¶\r\n").Replace(' ', '·').Replace('\t', '¬');
                    web.Document.InvokeScript("newDocument", new object[] { text });
                    web.Document.InvokeScript("setDirty");
                    sr.Close();
                    sr.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                comp.Save(fi.FullName, web.Document.GetElementById("src"), null, fileName, date, "", "1");
                this.Text = CodeCommander.Documents.ProgramName + " - " + this._fileName;
                this.sauvegarderToolStripMenuItem.Enabled = true;
                this.btnSave.Enabled = true;
                this.générerToolStripMenuItem.Enabled = false;
                this.btnGenerate.Enabled = true;
                web.Document.InvokeScript("clearUndo");
            }
        }

        private void projetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this._fileName))
            {
                this.sauvegarderToolStripMenuItem_Click(sender, e);
                if (this.pageProjects == null)
                {
                    this.pageProjects = new Projects();
                    this.pageProjects.Compilation(this.directorySource + "\\" + this._fileName);
                }
                if (!this.pageProjects.IsHandleCreated)
                {
                    // on recrée la fenêtre
                    this.pageProjects = new Projects();
                    Rectangle bounds = Screen.GetBounds(this);
                    this.pageProjects.Left = bounds.Right - (this.pageProjects.Width + 70);
                    this.pageProjects.Top = 0;
                    this.pageProjects.Height = bounds.Top + bounds.Height - 20;
                    this.pageProjects.Show();
                }
                this.pageProjects.Activate();
            }
        }

        private void btnAplus_Click(object sender, EventArgs e)
        {
            this.web.Document.InvokeScript("increaseFontSize");
        }

        private void btnAmoins_Click(object sender, EventArgs e)
        {
            this.web.Document.InvokeScript("decreaseFontSize");
        }
    }
}