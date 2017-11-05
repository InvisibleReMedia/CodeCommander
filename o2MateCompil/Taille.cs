using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Taille : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string varName;
        private string tabName;
        private int indent;
        private ICompilateurInstance cachedComp;
        private IData value;
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
            get { return "Taille"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displaySize", new object[] { this.varName, this.tabName, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displaySize", new object[] { this.varName, this.tabName, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.varName = this.GetElementByName(elem, "varName").InnerText;
            this.tabName = this.GetElementByName(elem, "size").InnerText;
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("size");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("terme");
            writer.WriteString(this.varName);
            writer.WriteEndElement();
            writer.WriteStartElement("tableau");
            writer.WriteString(this.tabName);
            writer.WriteEndElement();
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.SelectSingleNode("terme").InnerText;
            this.tabName = node.SelectSingleNode("tableau").InnerText;
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
                if (this.tabName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.tabName);
                        return false;
                    }
                    this.tabName = this.tabName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.SelectSingleNode("terme").InnerText;
            this.tabName = node.SelectSingleNode("tableau").InnerText;
            this.cachedComp = comp;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            if (proc.CurrentDictionnaire.IsArray(this.tabName))
            {
                Object obj = proc.CurrentDictionnaire.GetArray(this.tabName);
                Array arr = obj as Array;
                int size = arr.Count;
                if (proc.CurrentScope.Exists(this.varName))
                {
                    IData d = proc.CurrentScope.GetVariable(this.varName);
                    d.Value = size.ToString();
                    d.IsComputable = false;
                    proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "size", this.varName, this.tabName));
                }
                else
                {
                    proc.CurrentScope.Add(this.varName, size.ToString(), proc.Name, false);
                    proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "size", this.varName, this.tabName));
                }
            }
            else
            {
                if (proc.CurrentScope.Exists(this.varName))
                {
                    IData d = proc.CurrentScope.GetVariable(this.varName);
                    d.Value = "0";
                    d.IsComputable = false;
                }
                else
                {
                    proc.CurrentScope.Add(this.varName, "0", proc.Name, true);
                }
            }
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            this.value = Helper.ConvertNewDictionarySize(this.cachedComp, proc, converter, this.varName);
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
            converter.CurrentFunction.AddToSource("$[left:" + MicrosoftCPPConverter.Escape(this.value.PrefixedName) + "] = this->GetArrayCount(wstring(L\"" + MicrosoftCPPConverter.Escape(this.tabName) + "\"));" + Environment.NewLine);
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource(this.value.Name + " = theDict.GetArray(\"" + this.tabName + "\").Count" + Environment.NewLine);
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource("S[left:" + PowerShellConverter.Escape(this.value.Name) + "] = $theDict.GetArray(\"" + PowerShellConverter.Escape(this.tabName) + "\").Count" + Environment.NewLine);
        }

        #endregion
    }
}
