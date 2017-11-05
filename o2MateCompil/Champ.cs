using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Champ : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string tabName;
        private string expression;
        private string fieldName;
        private int indent;
        private ICompilateurInstance cachedComp;
        private IData defaultWriter;
        private Encoding enc;
        private string index;
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
            get { return "Champ"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
            if (proc.CurrentDictionnaire.Exists(this.tabName))
            {
                if (proc.CurrentDictionnaire.IsArray(this.tabName))
                {
                    Array arr = proc.CurrentDictionnaire.GetArray(this.tabName) as Array;
                    Fields f = arr.Item(1) as Fields;
                    if (!f.Exists(this.fieldName))
                    {
                        f.AddString(this.fieldName, "");
                    }
                }
            }
            else
            {
                Array arr = new Array();
                Fields f = new Fields();
                f.AddString(this.fieldName, "");
                arr.Add(f);
                proc.CurrentDictionnaire.AddArray(this.tabName, arr);
            }
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayChamp", new object[] { this.tabName, this.expression, this.fieldName, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayChamp", new object[] { this.tabName, this.expression, this.fieldName, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.tabName = this.GetElementByName(elem, "tableau").InnerText;
            this.expression = this.GetElementByName(elem, "expression").InnerText;
            this.fieldName = this.GetElementByName(elem, "champ").InnerText;
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("champ");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("tableau");
            writer.WriteString(this.tabName);
            writer.WriteEndElement();
            writer.WriteStartElement("expression");
            writer.WriteString(this.expression);
            writer.WriteEndElement();
            writer.WriteStartElement("variable");
            writer.WriteString(this.fieldName);
            writer.WriteEndElement();
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.tabName = node.SelectSingleNode("tableau").InnerText;
            this.expression = node.SelectSingleNode("expression").InnerText;
            this.fieldName = node.SelectSingleNode("variable").InnerText;
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
                if (this.tabName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.tabName);
                        return false;
                    }
                    this.tabName = this.tabName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                if (this.fieldName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.fieldName);
                        return false;
                    }
                    this.fieldName = this.fieldName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                if (this.expression.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    this.expression = this.expression.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.tabName = node.SelectSingleNode("tableau").InnerText;
            this.expression = node.SelectSingleNode("expression").InnerText;
            this.fieldName = node.SelectSingleNode("variable").InnerText;
            this.enc = comp.EncodedFile;
            this.cachedComp = comp;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            if (proc.CurrentDictionnaire.IsArray(this.tabName))
            {
                Array arr = proc.CurrentDictionnaire.GetArray(this.tabName) as Array;
                Fields f = null;
                o2Mate.Expression expr = new o2Mate.Expression();
                IData result = expr.Evaluate(this.expression, proc.CurrentScope);
                if (result != null)
                {
                    if (result.ValueInt <= arr.Count)
                    {
                        f = arr.Item(result.ValueInt) as Fields;
                    }
                }
                else
                {
                    if (arr.Count > 0)
                    {
                        f = arr.Item(1) as Fields;
                    }
                }
                if (f != null && f.Exists(this.fieldName))
                {
                    if (proc.CurrentScope.Exists(proc.DefaultWriter))
                    {
                        string writer = proc.CurrentScope.GetVariable(proc.DefaultWriter).ValueString;
                        FinalFile.WriteToFile(ref writer, f.GetString(this.fieldName), this.enc);
                        proc.CurrentScope.GetVariable(proc.DefaultWriter).Value = writer;
                        proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "print field", this.tabName, this.fieldName));
                    }
                }
            }
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            IData res = Helper.ConvertNewExpression(this.cachedComp, proc, converter, this.expression);
            this.index = res.ValueString;
            if (proc.CurrentScope.Exists(proc.DefaultWriter))
            {
                this.defaultWriter = proc.CurrentScope.GetVariable(proc.DefaultWriter);
                if (!this.defaultWriter.IsGlobal)
                {
                    this.cachedComp.UpdateParameters(converter, proc, this.defaultWriter.Name, true);
                }
                converter.Convert(this);
            }
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
            if (!this.defaultWriter.IsGlobal)
                converter.CurrentFunction.AddToSource("$[ifptr:" + MicrosoftCPPConverter.Escape(this.defaultWriter.PrefixedName) + "]WriteToFile(this->GetField(wstring(L\"" + MicrosoftCPPConverter.Escape(this.tabName) + "\"), " + this.index + ", wstring(L\"" + MicrosoftCPPConverter.Escape(this.fieldName) + "\")));" + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource(MicrosoftCPPConverter.Escape(this.defaultWriter.PrefixedName) + ".WriteToFile(this->GetField(wstring(L\"" + MicrosoftCPPConverter.Escape(this.tabName) + "\"), " + this.index + ", wstring(L\"" + MicrosoftCPPConverter.Escape(this.fieldName) + "\")));" + Environment.NewLine);
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource("WriteToFile " + this.defaultWriter.Name + ", theDict.GetArray(\"" + this.tabName + "\").Item(" + this.index + ").GetString(\"" + this.fieldName + "\")" + Environment.NewLine);
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            if (!this.defaultWriter.IsGlobal)
                converter.CurrentFunction.AddToSource("WriteToFile S[byvalue:" + PowerShellConverter.Escape(this.defaultWriter.Name) + "] $theDict.GetArray(\"" + PowerShellConverter.Escape(this.tabName) + "\").Item(" + this.index + ").GetString(\"" + PowerShellConverter.Escape(this.fieldName) + "\")" + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource("WriteToFile $" + PowerShellConverter.Escape(this.defaultWriter.Name) + " $theDict.GetArray(\"" + PowerShellConverter.Escape(this.tabName) + "\").Item(" + this.index + ").GetString(\"" + PowerShellConverter.Escape(this.fieldName) + "\")" + Environment.NewLine);
        }

        #endregion
    }
}
