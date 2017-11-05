using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class AffectationChaine : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string varName;
        private string stringName;
        private int indent;
        private ICompilateurInstance cachedComp;
        private IData value;
        #endregion

        #region Public Properties
        public string VariableName
        {
            get { return this.varName; }
        }
        #endregion

        #region Private Methods
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
        #endregion

        #region Compiler Membres

        public string TypeName
        {
            get { return "AffectationChaine"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
            if (!proc.CurrentDictionnaire.Exists(this.stringName))
            {
                proc.CurrentDictionnaire.AddString(this.stringName, "");
            }
            proc.CurrentScope.Add(this.varName, "", proc.Name, false);
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
                if (this.varName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.varName);
                        return false;
                    }
                    this.varName = this.varName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                if (this.stringName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.stringName);
                        return false;
                    }
                    this.stringName = this.stringName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.SelectSingleNode("variable").InnerText;
            this.stringName = node.SelectSingleNode("chaine").InnerText;
            // on en a besoin pour plus tard à la conversion
            this.cachedComp = comp;
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayAffectationChaine", new object[] { this.varName, this.stringName, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayAffectationChaine", new object[] { this.varName, this.stringName, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.varName = this.GetElementByName(elem, "varName").InnerText;
            this.stringName = this.GetElementByName(elem, "stringName").InnerText;

        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("affectationchaine");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("variable");
            writer.WriteString(this.varName);
            writer.WriteEndElement();
            writer.WriteStartElement("chaine");
            writer.WriteString(this.stringName);
            writer.WriteEndElement();
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.SelectSingleNode("variable").InnerText;
            this.stringName = node.SelectSingleNode("chaine").InnerText;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            if (proc.CurrentDictionnaire.IsString(this.stringName))
            {
                if (proc.CurrentScope.Exists(this.varName))
                {
                    IData d = proc.CurrentScope.GetVariable(this.varName);
                    d.Value = proc.CurrentDictionnaire.GetString(this.stringName);
                    d.IsComputable = false;
                }
                else
                {
                    proc.CurrentScope.Add(this.varName, proc.CurrentDictionnaire.GetString(this.stringName), proc.Name, false);
                    proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "dictionary string", this.varName, this.stringName));
                }
            }
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            this.value = Helper.ConvertNewDictionaryString(this.cachedComp, proc, converter, this.varName);
            converter.Convert(this);
        }

        #endregion

        #region ILanguageConverter Membres

        public void WriteInPerl(ICodeConverter converter)
        {
        }

        public void WriteInPython(ICodeConverter converter)
        {
        }

        public void WriteInUnixCPP(ICodeConverter converter)
        {
        }

        public void WriteInCSharp(ICodeConverter converter)
        {
        }

        public void WriteInJava(ICodeConverter converter)
        {
        }

        public void WriteInMacOSCPP(ICodeConverter converter)
        {
        }

        public void WriteInMicrosoftCPP(ICodeConverter converter)
        {
            if (!this.value.IsGlobal)
                converter.CurrentFunction.AddToSource("$[left:" + MicrosoftCPPConverter.Escape(value.PrefixedName) + "] = this->GetString(wstring(L\"" + MicrosoftCPPConverter.Escape(this.stringName) + "\"));" + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource(MicrosoftCPPConverter.Escape(value.PrefixedName) + " = this->GetString(wstring(L\"" + MicrosoftCPPConverter.Escape(this.stringName) + "\"));" + Environment.NewLine);
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource(this.varName + " = theDict.GetString(\"" + this.stringName + "\")" + Environment.NewLine);
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            if (!this.value.IsGlobal)
                converter.CurrentFunction.AddToSource("S[left:" + PowerShellConverter.Escape(this.varName) + "] = $theDict.GetString(\"" + PowerShellConverter.Escape(this.stringName) + "\")" + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource("$" + PowerShellConverter.Escape(this.varName) + " = $theDict.GetString(\"" + PowerShellConverter.Escape(this.stringName) + "\")" + Environment.NewLine);
        }

        #endregion
    }
}
