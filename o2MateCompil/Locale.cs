using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Locale : ICompiler
    {
        #region Private Fields
        private string localeName;
        private Encoding enc;
        #endregion

        #region Compiler Membres

        public string TypeName
        {
            get { return "Locale"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
        }

        public void Inject(Injector injector, ICompilateurInstance comp, System.Xml.XmlNode node, string modifier)
        {
            this.Load(comp, node);
            if (comp.UnderConversion)
            {
                string s = String.Empty;
                if (!this.IsComputable(injector.InjectedProcess, out s))
                {
                    throw new Exception(s);
                }
            }
        }

        public bool IsComputable(IProcess proc, out string reason)
        {
            reason = null;
            // les noms des paramètres sont triés par ordre du plus long au plus court
            foreach (IParameter param in proc.Replacements)
            {
                if (this.localeName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.localeName);
                        return false;
                    }
                    this.localeName = this.localeName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            this.localeName = node.InnerText;
            this.enc = comp.EncodedFile;
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
        }

        public void Update(System.Windows.Forms.HtmlElement doc)
        {
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("locale");
            writer.WriteString(this.localeName);
            writer.WriteEndElement();
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            this.localeName = node.InnerText;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            if (proc.CurrentScope.Exists(proc.DefaultWriter))
            {
                string writer = proc.CurrentScope.GetVariable(proc.DefaultWriter).ValueString;
                o2Mate.LocaleGroup lg = new LocaleGroup();
                string groupName;
                string title;
                if (lg.ExtractGroupAndName(this.localeName, out groupName, out title))
                {
                    ILocaleSet set = lg.Get(groupName) as ILocaleSet;
                    string language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                    try
                    {
                        // lève une exception si l'élement n'existe pas
                        FinalFile.WriteToFile(ref writer, set.Get(title, language), this.enc);
                    }
                    catch
                    {
                        FinalFile.WriteToFile(ref writer, this.localeName, this.enc);
                    }
                }
                proc.CurrentScope.GetVariable(proc.DefaultWriter).Value = writer;
            }

        }

        public void Convert(Converters.ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
        }

        #endregion
    }
}
