using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Linq;

namespace CodeCommander
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // création du fichier outils.VBS
                o2Mate.Compilateur comp = new o2Mate.Compilateur();
                o2Mate.Dictionnaire dict = new o2Mate.Dictionnaire();
                dict.AddString("dir", Documents.EditeurDirectory);
                string fileNameDict = Documents.TempDictFile;
                dict.Save(fileNameDict);
                comp.Compilation(Documents.OutilsVBSXML, fileNameDict, Documents.OutilsVBS, null);
                // le compilateur ne peut pas etre lancé 2 fois de suite
                comp = new o2Mate.Compilateur();
                comp.Compilation(Documents.LocalesVBSXML, fileNameDict, Documents.LocalesVBS, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            try
            {
                DirectoryInfo di = new DirectoryInfo(Documents.CodeCommanderDirectory);
                if (!di.Exists)
                {
                    di.Create();
                }
                di = new DirectoryInfo(Documents.SourcesDirectory);
                if (!di.Exists)
                {
                    di.Create();
                    di = new DirectoryInfo(Documents.ExamplesDirectory);
                    Program.CopyXML(di, Documents.SourcesDirectory);
                }
                di = new DirectoryInfo(Documents.TemplatesDirectory);
                if (!di.Exists)
                {
                    di.Create();
                    di = new DirectoryInfo(Documents.SrcTemplatesDirectory);
                    Program.CopyXML(di, Documents.TemplatesDirectory);
                }
                di = new DirectoryInfo(Documents.SkeletonsDirectory);
                if (!di.Exists)
                {
                    di.Create();
                    di = new DirectoryInfo(Documents.SrcSkeletonsDirectory);
                    Program.CopyXML(di, Documents.SkeletonsDirectory);
                }
                di = new DirectoryInfo(Documents.SyntaxDirectory);
                if (!di.Exists)
                {
                    di.Create();
                    di = new DirectoryInfo(Documents.SrcSyntaxDirectory);
                    Program.CopyXML(di, Documents.SyntaxDirectory);
                }
                di = new DirectoryInfo(Documents.LocalesDirectory);
                if (!di.Exists)
                {
                    di.Create();
                    di = new DirectoryInfo(Documents.SrcLocalesDirectory);
                    Program.CopyTSV(di, Documents.LocalesDirectory);
                }
                di = new DirectoryInfo(Documents.DictionariesDirectory);
                if (!di.Exists)
                {
                    di.Create();
                }
                di = new DirectoryInfo(Documents.BuildDirectory);
                if (!di.Exists)
                {
                    di.Create();
                }
                di = new DirectoryInfo(Documents.TempDirectory);
                if (!di.Exists)
                {
                    di.Create();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("L'application va s'arrêter parce qu'il est impossible de créer des fichiers dans le répertoire 'Mes documents'", "Impossible de continuer", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ComputerSettings computerSettings = new ComputerSettings();
            try
            {
                computerSettings.VerifyPowerShell();
                computerSettings.VerifyExecutingPowerShell();
                computerSettings.TestChangeParameterSettings();
                Application.Run(computerSettings);
                if (computerSettings.DialogResult == DialogResult.Cancel)
                {
                    // si cancel alors on stoppe l'application
                    return;
                }
                if (computerSettings.DialogResult == DialogResult.OK)
                {
                    Form1 form = new Form1(computerSettings);
                    try
                    {
                        // création du fichier editeur.HTM en fonction de la langue
                        o2Mate.Compilateur comp = new o2Mate.Compilateur();
                        o2Mate.Dictionnaire dict = new o2Mate.Dictionnaire();
                        string fileNameDict = Documents.TempDictFile;
                        dict.Save(fileNameDict);
                        comp.Compilation(Documents.EditorPageXML, fileNameDict, Documents.EditorPage, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    Application.Run(form);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            finally
            {
                computerSettings.RevertToSaved();
                // suppress all temporary files
                DirectoryInfo dir = new DirectoryInfo(Documents.TempDirectory);
                dir.EnumerateFiles().AsParallel().ForAll(a => { a.Delete(); });
            }
        }

        private static void CopyXML(DirectoryInfo di, string p)
        {
            foreach (FileInfo fi in di.GetFiles("*.xml"))
            {
                fi.CopyTo(p + fi.Name, true);
            }
        }

        private static void CopyTSV(DirectoryInfo di, string p)
        {
            foreach (FileInfo fi in di.GetFiles("*.tsv"))
            {
                fi.CopyTo(p + fi.Name, true);
            }
        }
    }
}