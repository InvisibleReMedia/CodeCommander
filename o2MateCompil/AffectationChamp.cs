using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class AffectationChamp : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string varName;
        private string tabName;
        private string expression;
        private string fieldName;
        private int indent;
        private ICompilateurInstance cachedComp;
        private string index;
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
            get { return "AffectationChamp"; }
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
            proc.CurrentScope.Add(this.varName, "", proc.Name, false);
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayAffectationChamp", new object[] { this.varName, this.tabName, this.expression, this.fieldName, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayAffectationChamp", new object[] { this.varName, this.tabName, this.expression, this.fieldName, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.varName = this.GetElementByName(elem, "varName").InnerText;
            this.tabName = this.GetElementByName(elem, "tableau").InnerText;
            this.expression = this.GetElementByName(elem, "expression").InnerText;
            this.fieldName = this.GetElementByName(elem, "champ").InnerText;
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("affectationchamp");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("variable");
            writer.WriteString(this.varName);
            writer.WriteEndElement();
            writer.WriteStartElement("champ");
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
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.SelectSingleNode("variable").InnerText;
            this.tabName = node.SelectSingleNode("champ/tableau").InnerText;
            this.expression = node.SelectSingleNode("champ/expression").InnerText;
            this.fieldName = node.SelectSingleNode("champ/variable").InnerText;
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
            this.varName = node.SelectSingleNode("variable").InnerText;
            this.tabName = node.SelectSingleNode("champ/tableau").InnerText;
            this.expression = node.SelectSingleNode("champ/expression").InnerText;
            this.fieldName = node.SelectSingleNode("champ/variable").InnerText;
            // on en a besoin pour plus tard dans la conversion
            this.cachedComp = comp;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            if (proc.CurrentDictionnaire.IsArray(this.tabName))
            {
                Array arr = proc.CurrentDictionnaire.GetArray(this.tabName) as Array;
                o2Mate.Expression expr = new o2Mate.Expression();
                IData result = expr.Evaluate(this.expression, proc.CurrentScope);
                Fields f = null;
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
                    if (proc.CurrentScope.Exists(this.varName))
                    {
                        IData d = proc.CurrentScope.GetVariable(this.varName);
                        d.Value = f.GetString(this.fieldName);
                        d.IsComputable = false;
                    }
                    else
                    {
                        proc.CurrentScope.Add(this.varName, f.GetString(this.fieldName), proc.Name, false);
                        proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "dictionary field", this.varName, this.tabName, this.fieldName));
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
                        proc.CurrentScope.Add(this.varName, "0", proc.Name, false);
                    }
                }
            }
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            // pas de test sur l'existence du tableau dans le dictionnaire
            IData res = Helper.ConvertNewExpression(this.cachedComp, proc, converter, this.expression);
            this.index = res.ValueString;
            // add a new variable or change an existing variable
            this.value = Helper.ConvertNewDictionaryArray(this.cachedComp, proc, converter, this.varName);
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
                converter.CurrentFunction.AddToSource("$[left:" + MicrosoftCPPConverter.Escape(this.value.PrefixedName) + "] = this->GetField(wstring(L\"" + MicrosoftCPPConverter.Escape(this.tabName) + "\"), " + this.index + ", wstring(L\"" + MicrosoftCPPConverter.Escape(this.fieldName) + "\"));" + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource(MicrosoftCPPConverter.Escape(this.value.PrefixedName) + " = this->GetField(wstring(L\"" + MicrosoftCPPConverter.Escape(this.tabName) + "\"), " + this.index + ", wstring(L\"" + MicrosoftCPPConverter.Escape(this.fieldName) + "\"));" + Environment.NewLine);
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource(this.varName + " = theDict.GetArray(\"" + this.tabName + "\").Item(" + this.index + ").GetString(\"" + this.fieldName + "\")" + Environment.NewLine);
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            if (!this.value.IsGlobal)
                converter.CurrentFunction.AddToSource("S[left:" + PowerShellConverter.Escape(this.varName) + "] = $theDict.GetArray(\"" + PowerShellConverter.Escape(this.tabName) + "\").Item(" + this.index + ").GetString(\"" + PowerShellConverter.Escape(this.fieldName) + "\")" + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource("$" + PowerShellConverter.Escape(this.varName) + " = $theDict.GetArray(\"" + PowerShellConverter.Escape(this.tabName) + "\").Item(" + this.index + ").GetString(\"" + PowerShellConverter.Escape(this.fieldName) + "\")" + Environment.NewLine);
        }

        #endregion
    }
}
