using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Condition : ICompiler
    {
        #region Private Fields
        private string expression;
        private string labelTrue;
        private string labelFalse;
        private int indent;
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
            get { return "Condition"; }
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
            list.Add(new DisplayElement(this.TypeName, "displayCondition", new object[] { this.expression, this.labelTrue, this.labelFalse, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayCondition", new object[] { this.expression, this.labelTrue, this.labelFalse, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.expression = this.GetElementByName(elem, "expression").InnerText;
            this.labelTrue = this.GetElementByName(elem, "labelTrue").InnerText;
            this.labelFalse = this.GetElementByName(elem, "labelFalse").InnerText;
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("condition");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("expression");
            writer.WriteString(this.expression);
            writer.WriteEndElement();
            writer.WriteStartElement("iftrue");
            writer.WriteString(this.labelTrue);
            writer.WriteEndElement();
            writer.WriteStartElement("iffalse");
            writer.WriteString(this.labelFalse);
            writer.WriteEndElement();
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.expression = node.SelectSingleNode("expression").InnerText;
            this.labelTrue = node.SelectSingleNode("iftrue").InnerText;
            this.labelFalse = node.SelectSingleNode("iffalse").InnerText;
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
                if (this.expression.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    this.expression = this.expression.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                if (this.labelTrue.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.labelTrue);
                        return false;
                    }
                    this.labelTrue = this.labelTrue.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                if (this.labelFalse.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.labelFalse);
                        return false;
                    }
                    this.labelFalse = this.labelFalse.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.expression = node.SelectSingleNode("expression").InnerText;
            this.labelTrue = node.SelectSingleNode("iftrue").InnerText;
            this.labelFalse = node.SelectSingleNode("iffalse").InnerText;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            int nextPosition;
            o2Mate.Expression expr = new o2Mate.Expression();
            IData result = expr.Evaluate(this.expression, proc.CurrentScope);
            if (result != null)
            {
                if (result.ValueInt == 1)
                {
                    nextPosition = proc.GetPositionLabel(this.labelTrue);
                    proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "goto", this.labelTrue));
                }
                else
                {
                    nextPosition = proc.GetPositionLabel(this.labelFalse);
                    proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "goto", this.labelFalse));
                }
                proc.CurrentPositionExecution = nextPosition;
            }
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            throw new Exception("Impossible de convertir une instruction de branchement. Utilisez plutôt le modèle '/CodeCommander/Condition'");
        }

        #endregion
    }
}
