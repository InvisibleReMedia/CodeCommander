using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodeCommander
{
    internal partial class DictForm : Form
    {
        #region Private Fields
        private string fileName;
        private o2Mate.Dictionnaire dict;
        #endregion

        public DictForm(string fileName, o2Mate.Dictionnaire dict)
        {
            this.fileName = fileName;
            this.dict = dict;
            InitializeComponent();
        }

        private void DictForm_Load(object sender, EventArgs e)
        {
            if (this.dict.IsEmpty())
            {
                DictExecute de = new DictExecute(this.web, this.dict, new o2Mate.Dictionnaire(), this.fileName);
                de.Load();
                de.Navigate();
            }
            else
            {
                // le dictionnaire d'entrée sert pour connaitre les champs
                // le dictionnaire de sortie permet de stocker les valeurs effectives
                DictBienvenue db = new DictBienvenue(this.web, this.dict, new o2Mate.Dictionnaire(), this.fileName);
                db.Load();
                db.Navigate();
            }
        }
    }
}