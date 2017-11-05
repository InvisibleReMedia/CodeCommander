using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodeCommander
{
    internal partial class SyntaxForm : Form
    {
        #region Private Fields
        private string fileName;
        private o2Mate.Dictionnaire dict, outputDict;
        private string result;
        #endregion

        public SyntaxForm(string fileName, o2Mate.Dictionnaire dict)
        {
            this.fileName = fileName;
            this.dict = dict;
            this.outputDict = new o2Mate.Dictionnaire();
            InitializeComponent();
        }

        public string Compile()
        {
            if (this.dict.IsEmpty())
            {
                DictExecute de = new DictExecute(this.web, this.dict, this.outputDict, this.fileName);
                this.result = de.ExecuteAndRead();
            }
            else
            {
                this.ShowDialog();
            }
            return this.result;
        }

        private void DictForm_Load(object sender, EventArgs e)
        {
            // le dictionnaire d'entrée sert pour connaitre les champs
            // le dictionnaire de sortie permet de stocker les valeurs effectives
            DictBienvenue db = new DictBienvenue(this.web, this.dict, this.outputDict, this.fileName);
            db.QuickLoad();
        }

        private void SyntaxForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DictExecute exec = new DictExecute(this.web, this.dict, this.outputDict, this.fileName);
            result = exec.ExecuteAndRead();
        }
    }
}