using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using Converters;
using System.Drawing;

namespace CodeCommander
{
    internal class DictExecute : IDictProcess
    {
        #region Public Event
        public event EventHandler Close;
        #endregion

        #region Private Fields
        private int preferredHeight;
        private System.Windows.Forms.WebBrowser web;
        private o2Mate.Dictionnaire inputDict, outputDict;
        private string fileNameFinal;
        private string fileNameSrc;
        #endregion

        #region Default Constructor
        public DictExecute(System.Windows.Forms.WebBrowser web, o2Mate.Dictionnaire inputDict, o2Mate.Dictionnaire outputDict, string fileName)
        {
            this.web = web;
            this.inputDict = inputDict;
            this.outputDict = outputDict;
            this.fileNameSrc = fileName;
            this.fileNameFinal = Path.Combine(Documents.TempDirectory, "execute.htm");
        }
        #endregion

        #region IDictProcess Membres

        public void Load()
        {
            o2Mate.Dictionnaire IHMdict = new o2Mate.Dictionnaire();
            string fileNameDict = Path.Combine(Documents.TempDirectory, Documents.ExecutePage + ".xml");
            IHMdict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            comp.LoadTemplates(Documents.GeneratedDictionariesTemplatesDirectory);
            comp.Compilation(Documents.ExecutePage, fileNameDict, this.fileNameFinal, null);
            this.preferredHeight = 400;
        }

        public string ExecuteAndRead()
        {
            string result = String.Empty;
            FileInfo fi = new FileInfo(this.fileNameSrc);
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(fi.Name) + ".xml");
            this.outputDict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            comp.LoadTemplates(Documents.TemplatesDirectory);
            comp.LoadSkeletons(Documents.SkeletonsDirectory);
            string outputFinalFile = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".txt";
            try
            {
                comp.Compilation(this.fileNameSrc, fileNameDict, outputFinalFile, null);
                StreamReader sr = new StreamReader(outputFinalFile);
                result = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (this.Close != null)
                this.Close(this, new EventArgs());
            return result;
        }

        public void Navigate()
        {
            this.web.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
            this.web.Navigate(this.fileNameFinal);
        }

        void web_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if (this.preferredHeight < this.web.Parent.MaximumSize.Height)
                this.web.Parent.Size = new Size(this.web.Parent.Size.Width, this.preferredHeight);
            else
                this.web.Parent.Size = new Size(this.web.Parent.Size.Width, this.web.Parent.MaximumSize.Height);
            this.web.Document.GetElementById("exec").Click += new System.Windows.Forms.HtmlElementEventHandler(DictExecute_Exec);
            this.web.Document.GetElementById("vbscript").Click += new HtmlElementEventHandler(VBScript_Click);
            this.web.Document.GetElementById("powershell").Click += new HtmlElementEventHandler(PowerShell_Click);
            this.web.Document.GetElementById("perl").Click += new HtmlElementEventHandler(Perl_Click);
            this.web.Document.GetElementById("csharp").Click += new HtmlElementEventHandler(CSharp_Click);
            this.web.Document.GetElementById("java").Click += new HtmlElementEventHandler(Java_Click);
            this.web.Document.GetElementById("wincpp").Click += new HtmlElementEventHandler(WindowsCPP_Click);
            this.web.Document.GetElementById("unixcpp").Click += new HtmlElementEventHandler(unixCPP_Click);
            this.web.Document.GetElementById("maccpp").Click += new HtmlElementEventHandler(macCPP_Click);

            this.web.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
        }

        void Perl_Click(object sender, HtmlElementEventArgs e)
        {
            FileInfo fi = new FileInfo(this.fileNameSrc);
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(fi.Name) + ".xml");
            this.outputDict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            string outputFinalFile = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".pl";
            try
            {
                comp.UnderConversion = true;
                comp.ConvertedLanguage = new Converters.PerlConverter();
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                comp.Convert(comp.ConvertedLanguage, this.fileNameSrc, fileNameDict, outputFinalFile);
                Process proc = new Process();
                if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms)
                {
                    proc.StartInfo.Verb = "Open";
                }
                else
                {
                    proc.StartInfo.Verb = "Edit";
                }
                proc.StartInfo.FileName = outputFinalFile;
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (this.Close != null)
                this.Close(this, new EventArgs());
        }

        void CSharp_Click(object sender, HtmlElementEventArgs e)
        {
            FileInfo fi = new FileInfo(this.fileNameSrc);
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(fi.Name) + ".xml");
            this.outputDict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            string outputFinalFile = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".vbs";
            try
            {
                comp.UnderConversion = true;
                comp.ConvertedLanguage = new Converters.CSharpConverter();
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                comp.Convert(comp.ConvertedLanguage, this.fileNameSrc, fileNameDict, outputFinalFile);
                Process proc = new Process();
                if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms)
                {
                    proc.StartInfo.Verb = "Open";
                }
                else
                {
                    proc.StartInfo.Verb = "Edit";
                }
                proc.StartInfo.FileName = outputFinalFile;
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (this.Close != null)
                this.Close(this, new EventArgs());
        }

        void Java_Click(object sender, HtmlElementEventArgs e)
        {
            FileInfo fi = new FileInfo(this.fileNameSrc);
            string pathExecutable = Path.Combine(Documents.BuildDirectory,
                                    Path.Combine(Path.GetFileNameWithoutExtension(fi.Name),
                                    Path.Combine(Path.GetFileNameWithoutExtension(fi.Name), "bin\\Debug")));
            string fileNameDict = Documents.TempDictFile;
            this.outputDict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            string outputFinalFile = Path.Combine(pathExecutable, Path.GetFileNameWithoutExtension(fi.Name) + ".exe");
            try
            {
                comp.UnderConversion = true;
                comp.ConvertedLanguage = new Converters.JavaConverter();
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                comp.Convert(comp.ConvertedLanguage, this.fileNameSrc, fileNameDict, outputFinalFile);
                Process proc = new Process();
                if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms)
                {
                    proc.StartInfo.Verb = "Open";
                }
                else
                {
                    proc.StartInfo.Verb = "Edit";
                }
                proc.StartInfo.FileName = outputFinalFile;
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (this.Close != null)
                this.Close(this, new EventArgs());
        }

        void WindowsCPP_Click(object sender, HtmlElementEventArgs e)
        {
            // fermer la dialog box pour lancer le message d'attente
            if (this.Close != null)
                this.Close(this, new EventArgs());
            FileInfo fi = new FileInfo(this.fileNameSrc);
            string solutionPath = Path.Combine(Documents.BuildDirectory, Path.GetFileNameWithoutExtension(fi.Name) + "_wincpp");
            string projectPath = Path.Combine(solutionPath, Path.GetFileNameWithoutExtension(fi.Name));
            string executablePath = Path.Combine(solutionPath, @"x64\Debug");
            string fileNameDict = Path.Combine(Documents.DictionariesDirectory, Path.GetFileNameWithoutExtension(fi.Name) + ".xml");
            this.outputDict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            string outputFinalFile = Path.Combine(Documents.BuildDirectory, Path.GetFileNameWithoutExtension(fi.Name) + ".txt");
            try
            {
                comp.UnderConversion = true;
                comp.ConvertedLanguage = new Converters.MicrosoftCPPConverter();
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                comp.Convert(comp.ConvertedLanguage, this.fileNameSrc, fileNameDict, outputFinalFile);

                #region Start Progress Bar
                PopupProgress pp = new PopupProgress(false, ProgressBarStyle.Marquee);
                pp.Start(false);
                #endregion

                #region compile and execute C++ Windows generated app
                bool compiled = false;
                ThreadObject to = new ThreadObject(new object[] { comp, pp });
                if (System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(delegate(object param)
                {
                    ThreadObject d = param as ThreadObject;
                    try
                    {
                        compiled = comp.ConvertedLanguage.CompileAndExecute(Path.Combine(solutionPath, Path.GetFileNameWithoutExtension(fi.Name) + ".sln"),
                                                Path.Combine(projectPath, Path.GetFileNameWithoutExtension(fi.Name) + ".vcxproj"),
                                                Path.Combine(executablePath, Path.GetFileNameWithoutExtension(fi.Name) + ".exe"),
                                                global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms);
                        (d.Datas[1] as PopupProgress).IsFinished = true;
                    }
                    catch(Exception ex)
                    {
                        (d.Datas[1] as PopupProgress).GiveException(ex);
                        (d.Datas[1] as PopupProgress).IsFinished = true;
                    }


                }), to))
                {
                    to.Wait.WaitOne(2000);
                    if (!pp.IsFinished)
                        pp.ShowDialog();

                    #region Copy dict file into the source directory
                    if (compiled)
                    {
                        File.Copy(fileNameDict, Path.Combine(solutionPath, Path.GetFileName(fileNameDict)), true);

                        #region Open result file
                        Process proc = new Process();
                        if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms)
                        {
                            proc.StartInfo.Verb = "Open";
                            proc.StartInfo.FileName = outputFinalFile;
                        }
                        else
                        {
                            proc.StartInfo.Verb = "Edit";
                            proc.StartInfo.FileName = Path.Combine(projectPath, "compiled.cpp");
                        }
                        proc.Start();
                        #endregion

                    }
                    else { MessageBox.Show("La compilation a échouée.", "Erreur à la compilation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void unixCPP_Click(object sender, HtmlElementEventArgs e)
        {
            FileInfo fi = new FileInfo(this.fileNameSrc);
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(fi.Name) + ".xml");
            this.outputDict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            string outputFinalFile = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".cpp";
            try
            {
                comp.UnderConversion = true;
                comp.ConvertedLanguage = new Converters.UnixCPPConverter();
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                comp.Convert(comp.ConvertedLanguage, this.fileNameSrc, fileNameDict, outputFinalFile);
                Process proc = new Process();
                if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms)
                {
                    proc.StartInfo.Verb = "Open";
                }
                else
                {
                    proc.StartInfo.Verb = "Edit";
                }
                proc.StartInfo.FileName = outputFinalFile;
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (this.Close != null)
                this.Close(this, new EventArgs());
        }

        void macCPP_Click(object sender, HtmlElementEventArgs e)
        {
            FileInfo fi = new FileInfo(this.fileNameSrc);
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(fi.Name) + ".xml");
            this.outputDict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            string outputFinalFile = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".cpp";
            try
            {
                comp.UnderConversion = true;
                comp.ConvertedLanguage = new Converters.MacOSCPPConverter();
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                comp.Convert(comp.ConvertedLanguage, this.fileNameSrc, fileNameDict, outputFinalFile);
                Process proc = new Process();
                if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms)
                {
                    proc.StartInfo.Verb = "Open";
                }
                else
                {
                    proc.StartInfo.Verb = "Edit";
                }
                proc.StartInfo.FileName = outputFinalFile;
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (this.Close != null)
                this.Close(this, new EventArgs());
        }

        void PowerShell_Click(object sender, HtmlElementEventArgs e)
        {
            FileInfo fi = new FileInfo(this.fileNameSrc);
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(fi.Name) + ".xml");
            this.outputDict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            string outputFinalFile = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".ps1";
            try
            {
                comp.UnderConversion = true;
                comp.ConvertedLanguage = new Converters.PowerShellConverter();
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                comp.Convert(comp.ConvertedLanguage, this.fileNameSrc, fileNameDict, outputFinalFile);
                Process proc = new Process();
                if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms && global::CodeCommander.Properties.Settings.Default.PowerShellInstalled)
                {
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.StartInfo.Verb = global::CodeCommander.Properties.Settings.Default.ExecPowerShellVerb;
                }
                else
                {
                    proc.StartInfo.Verb = "Open";
                }
                proc.StartInfo.FileName = outputFinalFile;
                proc.Start();
                if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms && global::CodeCommander.Properties.Settings.Default.PowerShellInstalled)
                {
                    proc.WaitForExit();
                }

                // ouvrir le document résultat
                if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms && global::CodeCommander.Properties.Settings.Default.PowerShellInstalled)
                {
                    proc = new Process();
                    proc.StartInfo.Verb = "Open";
                    proc.StartInfo.FileName = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".txt";
                    proc.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (this.Close != null)
                this.Close(this, new EventArgs());
        }

        void VBScript_Click(object sender, HtmlElementEventArgs e)
        {
            FileInfo fi = new FileInfo(this.fileNameSrc);
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(fi.Name) + ".xml");
            this.outputDict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            string outputFinalFile = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".vbs";
            try
            {
                comp.UnderConversion = true;
                comp.ConvertedLanguage = new Converters.VBScriptConverter();
                comp.LoadTemplates(Documents.TemplatesDirectory);
                comp.LoadSkeletons(Documents.SkeletonsDirectory);
                comp.Convert(comp.ConvertedLanguage, this.fileNameSrc, fileNameDict, outputFinalFile);

                Process proc = new Process();
                if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms)
                {
                    proc.StartInfo.Verb = "Open";
                }
                else
                {
                    proc.StartInfo.Verb = "Edit";
                }
                proc.StartInfo.FileName = outputFinalFile;
                proc.Start();
                proc.WaitForExit();

                // ouvrir le document résultat
                if (global::CodeCommander.Properties.Settings.Default.AcceptExecutingPrograms)
                {
                    proc = new Process();
                    proc.StartInfo.Verb = "Open";
                    proc.StartInfo.FileName = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".txt";
                    proc.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (this.Close != null)
                this.Close(this, new EventArgs());
        }

        void DictExecute_Exec(object sender, System.Windows.Forms.HtmlElementEventArgs e)
        {
            FileInfo fi = new FileInfo(this.fileNameSrc);
            string fileNameDict = Path.Combine(Documents.TempDirectory, Path.GetFileNameWithoutExtension(fi.Name) + ".xml");
            this.outputDict.Save(fileNameDict);
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            comp.LoadTemplates(Documents.TemplatesDirectory);
            comp.LoadSkeletons(Documents.SkeletonsDirectory);
            string outputFinalFile = Documents.BuildDirectory + Path.GetFileNameWithoutExtension(fi.Name) + ".txt";
            try
            {
                comp.Compilation(this.fileNameSrc, fileNameDict, outputFinalFile, null);
                Process proc = new Process();
                proc.StartInfo.FileName = outputFinalFile;
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (this.Close != null)
                this.Close(this, new EventArgs());
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
