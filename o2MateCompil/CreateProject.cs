using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    /// <summary>
    /// Create a new project (a specific statement)
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class CreateProject : ICompiler
    {
        #region Private Fields
        private string name;
        private int indent;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the name of the project
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
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

        /// <summary>
        /// Type name to identifying the object 
        /// </summary>
        public string TypeName
        {
            get { return "CreateProject"; }
        }

        /// <summary>
        /// Extract dictionary onto this object
        /// </summary>
        /// <param name="proc">process</param>
        public void ExtractDictionary(IProcessInstance proc)
        {
        }

        /// <summary>
        /// Inject this object onto the injector
        /// </summary>
        /// <param name="injector">injector object</param>
        /// <param name="comp">the compiler object</param>
        /// <param name="node">xml node object</param>
        /// <param name="modifier">modifier after, before or replace</param>
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

        /// <summary>
        /// Computes if this object instance can be injected onto a destination process
        /// Terms of each object information can be uncomputable except when it's a direct computation indeterminisme
        /// </summary>
        /// <param name="proc">process destination</param>
        /// <param name="reason">reason of uncomputable and indeterminist</param>
        /// <returns>true if ok else false (and then the reason is set)</returns>
        public bool IsComputable(IProcess proc, out string reason)
        {
            reason = null;
            // les noms des paramètres sont triés par ordre du plus long au plus court
            foreach (IParameter param in proc.Replacements)
            {
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

        /// <summary>
        /// Load object from xml node
        /// </summary>
        /// <param name="comp">this compiler</param>
        /// <param name="node">xml node object</param>
        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.name = node.Attributes.GetNamedItem("name").Value;
        }

        /// <summary>
        /// Displays this object onto the web navigator instance
        /// A javascript function is invoked with the web navigator
        /// </summary>
        /// <param name="list">list of binding elements</param>
        /// <param name="node">node object (Tree class)</param>
        /// <param name="forcedIndent">force a new numberous indentation</param>
        /// <param name="indent">number of indentation</param>
        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayCreateProject", new object[] { this.name, this.indent.ToString(), false }));
        }

        /// <summary>
        /// Displays read only object onto the web navigator instance
        /// A javascript function is invoked with the web navigator
        /// </summary>
        /// <param name="list">list of binding elements</param>
        /// <param name="node">node object (Tree class)</param>
        /// <param name="forcedIndent">force a new numberous indentation</param>
        /// <param name="indent">number of indentation</param>
        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayCreateProject", new object[] { this.name, this.indent.ToString(), true }));
        }

        /// <summary>
        /// When saving the CodeCommander file source, receipts and updates infos from the tml element of the web navigator
        /// </summary>
        /// <param name="elem">html element</param>
        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.name = this.GetElementByName(elem, "name").InnerText;
        }

        /// <summary>
        /// Saving the object into an xml node
        /// </summary>
        /// <param name="comp">this compiler</param>
        /// <param name="writer">Xml writer object</param>
        /// <param name="child">html child for nested objects</param>
        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("createproject");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteAttributeString("name", this.name);
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        /// <summary>
        /// Parse but not load
        /// </summary>
        /// <param name="comp">this compiler</param>
        /// <param name="node">xml node to read</param>
        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.name = node.Attributes.GetNamedItem("name").Value;
        }

        /// <summary>
        /// Execute the object and write results onto the final file
        /// </summary>
        /// <param name="proc">current process</param>
        /// <param name="file">final file</param>
        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "project", this.name));
        }

        /// <summary>
        /// Converts the object and write specific statements for an another programming language
        /// </summary>
        /// <param name="converter">the language converter</param>
        /// <param name="proc">current process</param>
        /// <param name="file">final file</param>
        public void Convert(Converters.ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
        }

        #endregion
    }
}
