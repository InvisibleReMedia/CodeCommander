using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class BeginSkeleton : ICompiler
    {
        #region Private Fields
        private string path;
        private string name;
        private int indent;
        #endregion

        #region Default Constructor
        public BeginSkeleton()
        {
            this.path = "";
            this.name = "";
        }
        #endregion

        #region Public Properties
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
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

        private string SpecialChars(string input)
        {
            const string availables = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJLMNOPQRSTUVWXYZ0123456789";
            string output = String.Empty;
            foreach (char c in input)
            {
                if (availables.IndexOf(c) != -1)
                {
                    output += c;
                }
                else
                {
                    output += "_";
                }
            }
            return output;
        }
        #endregion

        #region Compiler Membres

        public string TypeName
        {
            get { return "BeginSkeleton"; }
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
                if (this.path.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.path);
                        return false;
                    }
                    this.path = this.path.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                if (this.name.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.name);
                        return false;
                    }
                    this.name = this.name.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.path = node.Attributes.GetNamedItem("path").Value;
            this.name = node.Attributes.GetNamedItem("name").Value;
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayBeginSkeleton", new object[] { this.path + "/" + this.name, this.indent.ToString(), false}));
            int subIndent = this.indent;
            foreach (Node<ICompiler> child in node)
            {
                child.Object.Display(list, child, forcedIndent, ref subIndent);
            }
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayBeginSkeleton", new object[] { this.path + "/" + this.name, this.indent.ToString(), true }));
            int subIndent = this.indent;
            foreach (Node<ICompiler> child in node)
            {
                child.Object.Display(list, child, forcedIndent, ref subIndent);
            }
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            string path = this.GetElementByName(elem, "path").InnerText;
            if (String.IsNullOrEmpty(path)) path = "";
            int pos = path.LastIndexOf("/");
            if (pos != -1)
            {
                this.path = path.Substring(0, pos);
                this.name = path.Substring(pos + 1);
            }
            else
            {
                this.name = path;
                this.path = "";
            }
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("beginskeleton");
            writer.WriteAttributeString("path", this.path);
            writer.WriteAttributeString("name", this.name);
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.path = node.Attributes.GetNamedItem("path").Value;
            this.name = node.Attributes.GetNamedItem("name").Value;
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
