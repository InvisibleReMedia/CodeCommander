using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace test_expr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            o2Mate.Expression expr = new o2Mate.Expression();
            o2Mate.Scope s = new o2Mate.Scope();
            s.Add("i", "1", "", true);
            o2Mate.IData data = expr.Evaluate("i+1", s);
            MessageBox.Show(data.ValueString);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            o2Mate.Dictionnaire dict = new o2Mate.Dictionnaire();
            dict.AddString("test", "test2");
            o2Mate.Array arr = new o2Mate.Array();
            o2Mate.Fields f = new o2Mate.Fields();
            f.AddString("value", "t");
            arr.Add(f);
            f = new o2Mate.Fields();
            f.AddString("value", "t2");
            arr.Add(f);
            f = new o2Mate.Fields();
            f.AddString("value", "t2");
            arr.Add(f);
            dict.AddArray("textes", arr);
            dict.Save("c:\\file.xml");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            o2Mate.Compilateur comp = new o2Mate.Compilateur();
            comp.Compilation(@"C:\Documents and Settings\Olivier\Mes documents\Visual Studio 2005\Projects\o2Mate\Editeur\outil_texte.xml",
                             @"C:\Documents and Settings\Olivier\Mes documents\Visual Studio 2005\Projects\o2Mate\Editeur\dicttest.xml",
                             @"C:\Documents and Settings\Olivier\Mes documents\Visual Studio 2005\Projects\o2Mate\Editeur\result.txt", null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            o2Mate.UseMOP mop = new o2Mate.UseMOP();
            mop.Command = "test";
            mop.Command = "test@x.c/d{before}";
            mop.Command = "test@a/b/c.m/d{before}";
            mop.Command = "test@/a/b/c.m/d{before}";
            mop.Command = "test@/a/b/c2/c.m/d{before}";
            mop.Command = "---test@/a/b/c.m/d{before}----";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Converters.VBScriptConverter vbscript = new Converters.VBScriptConverter();
            o2Mate.Scope scope = new o2Mate.Scope();
            o2Mate.Expression.Convert(vbscript, "$a + $b", scope, true);
            MessageBox.Show(vbscript.CurrentFunction.CacheSource);
            o2Mate.Expression.Convert(vbscript, "$$a + $$b", scope, true);
            MessageBox.Show(vbscript.CurrentFunction.CacheSource);

            Converters.PowerShellConverter powershell = new Converters.PowerShellConverter();
            o2Mate.Expression.Convert(powershell, "$a + $b", scope, true);
            MessageBox.Show(powershell.CurrentFunction.CacheSource);
            o2Mate.Expression.Convert(powershell, "$$a + $$b", scope, true);
            MessageBox.Show(powershell.CurrentFunction.CacheSource);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            o2Mate.UniqueStrings u = new o2Mate.UniqueStrings();
            for (int counter = 0; counter < 1000; ++counter)
            {
                Console.WriteLine(u.ComputeNewString());
            }
        }
    }
}