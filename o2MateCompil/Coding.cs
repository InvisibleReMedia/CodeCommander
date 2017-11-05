using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Coding : ICompiler
    {
        #region Private Fields
        private string uniqueContainer;
        private string codingName;
        private XmlNode xmlCode;
        private int indent;
        #endregion

        #region Public Properties
        public string CodingName
        {
            get
            {
                return this.codingName;
            }
        }

        public string UniqueCodingName
        {
            get { return this.uniqueContainer + "_" + this.codingName; }
        }

        public XmlNode XmlCode
        {
            get
            {
                return this.xmlCode;
            }
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
            get { return "Coding"; }
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
            list.Add(new DisplayElement(this.TypeName, "displayCoding", new object[] { this.codingName, this.indent.ToString(), false }));
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
            list.Add(new DisplayElement(this.TypeName, "displayCoding", new object[] { this.codingName, this.indent.ToString(), true }));
            int subIndent = this.indent + 1;
            foreach (Node<ICompiler> child in node)
            {
                child.Object.DisplayReadOnly(list, child, true, ref subIndent);
            }
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.codingName = this.GetElementByName(elem, "coding").InnerText;
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("coding");
            writer.WriteAttributeString("name", this.codingName);
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
            this.codingName = node.Attributes.GetNamedItem("name").Value;
            comp.Parse(comp, node.SelectSingleNode("code"));
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
                if (this.codingName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.codingName);
                        return false;
                    }
                    this.codingName = this.codingName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.uniqueContainer = comp.Unique.ComputeNewString();
            this.codingName = node.Attributes.GetNamedItem("name").Value;
            this.xmlCode = node.SelectSingleNode("code");
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
        }

        #endregion
    }
}
