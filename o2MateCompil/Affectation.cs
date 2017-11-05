using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Affectation : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string varName;
        private string expression;
        private int indent;
        private ICompilateurInstance cachedComp;
        private IData result;
        #endregion

        #region Constructors
        public Affectation()
        {
        }

        public Affectation(ICompilateurInstance comp)
        {
            this.cachedComp = comp;
        }
        #endregion

        #region Public Properties
        public string VariableName
        {
            get { return this.varName; }
            set { this.varName = value; }
        }

        public string Expression
        {
            get { return this.expression; }
            set { this.expression = value; }
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
            get { return "Affectation"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
            // on veut savoir si l'expression est calculable ou pas
            o2Mate.Expression expr = new o2Mate.Expression();
            o2Mate.IData res = expr.Evaluate(this.expression, proc.CurrentScope);
            if (res.IsComputable)
                proc.CurrentScope.Add(this.varName, res.ValueString, proc.Name, true);
            else
                proc.CurrentScope.Add(this.varName, "", proc.Name, false);
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayAffectation", new object[] { this.varName, this.expression, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayAffectation", new object[] { this.varName, this.expression, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            this.varName = this.GetElementByName(elem, "varName").InnerText;
            this.expression = this.GetElementByName(elem, "expression").InnerText;
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("affectation");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("variable");
            writer.WriteString(this.varName);
            writer.WriteEndElement();
            writer.WriteStartElement("expression");
            writer.WriteString(this.expression);
            writer.WriteEndElement();
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.SelectSingleNode("variable").InnerText;
            this.expression = node.SelectSingleNode("expression").InnerText;
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
                if (this.expression.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (param.IsComputable)
                    {
                        this.expression = this.expression.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                    }
                    else
                    {
                        this.expression = this.expression.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.ReplacementParameter);
                    }
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.SelectSingleNode("variable").InnerText;
            this.expression = node.SelectSingleNode("expression").InnerText;
            // on en a besoin pour plus tard dans la conversion
            this.cachedComp = comp;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            o2Mate.Expression expr = new o2Mate.Expression();
            IData result = expr.Evaluate(this.expression, proc.CurrentScope);
            if (result != null)
            {
                if (proc.CurrentScope.Exists(this.varName))
                {
                    IData d = proc.CurrentScope.GetVariable(this.varName);
                    d.Value = result.ValueString;
                    d.IsComputable = result.IsComputable;
                }
                else
                {
                    proc.CurrentScope.Add(this.varName, result.ValueString, proc.Name, result.IsComputable);
                }
            }
            else
            {
                if (proc.CurrentScope.Exists(this.varName))
                {
                    IData d = proc.CurrentScope.GetVariable(this.varName);
                    d.Value = "0";
                    d.IsComputable = true;
                }
                else
                {
                    proc.CurrentScope.Add(this.varName, "0", proc.Name, true);
                }
            }
            proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "affectation", this.varName));
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            this.result = Helper.ConvertNewInferedVariable(this.cachedComp, proc, converter, this.expression, this.varName);
            converter.Convert(this);
        }

        #endregion

        #region ILanguageConverter Membres

        public void WriteInUnixCPP(ICodeConverter converter)
        {
        }

        public void WriteInMacOSCPP(ICodeConverter converter)
        {
        }

        public void WriteInCSharp(ICodeConverter converter)
        {
        }

        public void WriteInJava(ICodeConverter converter)
        {
        }

        public void WriteInPerl(ICodeConverter converter)
        {
        }

        public void WriteInPython(ICodeConverter converter)
        {
        }

        public void WriteInMicrosoftCPP(ICodeConverter converter)
        {
            if (!this.result.IsGlobal)
                converter.CurrentFunction.AddToSource("$[left:" + MicrosoftCPPConverter.Escape(this.result.PrefixedName) + "] = " + this.result.ValueString + ";" + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource(MicrosoftCPPConverter.Escape(this.result.PrefixedName) + " = " + this.result.ValueString + ";" + Environment.NewLine);
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource(this.varName + " = " + this.result.ValueString + Environment.NewLine);
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            if (!this.result.IsGlobal)
                converter.CurrentFunction.AddToSource("S[left:" + PowerShellConverter.Escape(this.varName) + "] = " + this.result.ValueString + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource("$" + PowerShellConverter.Escape(this.varName) + " = " + this.result.ValueString + Environment.NewLine);
        }

        #endregion
    }
}
