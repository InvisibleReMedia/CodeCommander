using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Parallel : ICompiler
    {
        #region Private Fields
        private XmlNode xmlCode;
        private int indent;
        #endregion

        #region Public Properties
        public XmlNode XmlCode
        {
            get { return this.xmlCode; }
        }
        #endregion

        #region Compiler Membres

        public string TypeName
        {
            get { return "Parallel"; }
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
                if (this.xmlCode.InnerXml.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans un programme parallèle", param.FormalParameter);
                        return false;
                    }
                    this.xmlCode.InnerXml = this.xmlCode.InnerXml.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.xmlCode = node.SelectSingleNode("code");
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayParallel", new object[] { this.indent.ToString(), false }));
            int subIndent = this.indent + 1;
            foreach (Node<ICompiler> child in node)
            {
                child.Object.Display(list, child, true, ref subIndent);
            }
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayParallel", new object[] { this.indent.ToString(), true }));
            int subIndent = this.indent + 1;
            foreach (Node<ICompiler> child in node)
            {
                child.Object.DisplayReadOnly(list, child, true, ref subIndent);
            }
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("parallel");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("code");
            // search next sibling child of code
            System.Windows.Forms.HtmlElement subElement = child.NextSibling;
            comp.Save(writer, ref subElement, this.indent + 1);
            writer.WriteEndElement();
            writer.WriteEndElement();
            child = subElement;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            comp.Parse(comp, node.SelectSingleNode("code"));
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
        }

        public void Convert(Converters.ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
        }

        #endregion

   }
}
