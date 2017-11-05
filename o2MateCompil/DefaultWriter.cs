using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class DefaultWriter : ICompiler
    {
        #region Private Fields
        private int indent;
        private string varName;
        private ICompilateurInstance cachedComp;
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
            get { return "DefaultWriter"; }
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
                if (this.varName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.varName);
                        return false;
                    }
                    this.varName = this.varName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.Attributes.GetNamedItem("name").Value;
            this.cachedComp = comp;
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayDefaultWriter", new object[] { this.varName, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayDefaultWriter", new object[] { this.varName, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            this.varName = this.GetElementByName(elem, "writer").InnerText;
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("defaultwriter");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteAttributeString("name", this.varName);
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.Attributes.GetNamedItem("name").Value;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            proc.DefaultWriter = this.varName;
            proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "default writer", this.varName));
        }

        public void Convert(Converters.ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            proc.DefaultWriter = this.varName;
        }

        #endregion
    }
}
