using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodeCommander
{
    internal partial class Projects : Form
    {
        public Projects()
        {
            InitializeComponent();
        }

        private void Projects_Load(object sender, EventArgs e)
        {
            this.web.Navigate(Documents.TempDirectory + "project.htm");
        }

        internal void Compilation(string fileName)
        {
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            comp.LoadTemplates(Documents.TemplatesDirectory);
            comp.LoadSkeletons(Documents.SkeletonsDirectory);
            o2Mate.Dictionnaire dict = comp.OutputDictionary(fileName);
            dict.Save(Documents.TempDictFile);
            // le fichier de sortie n'est pas important
            comp.Debug(Documents.TempDictFile, Documents.UnusedFile, null);
            o2Mate.Compilateur compProject = new o2Mate.Compilateur();
            compProject.LoadTemplates(Documents.TemplatesDirectory);
            o2Mate.Dictionnaire dictProject = comp.Threads.Dictionary;
            dictProject.Save(Documents.TempDictFile);
            compProject.Compilation(Documents.ProjectSourceCode, Documents.TempDictFile, Documents.ProjectPage, null);
        }
    }
}