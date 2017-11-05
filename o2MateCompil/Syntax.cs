using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Converters;

namespace o2Mate
{
    /// <summary>
    /// A syntax class
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class Syntax : ICompiler
    {
        #region Private Fields
        private string name;
        private string xmlCode;
        private int indent;
        private ILegendeDict legendes;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Syntax()
        {
            this.name = "";
            this.legendes = new LegendeDict();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the syntax's name
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets the inner XML string in this syntax
        /// </summary>
        public string XmlCode
        {
            get { return this.xmlCode; }
        }

        /// <summary>
        /// Gets the summary
        /// </summary>
        public ILegendeDict Legendes
        {
            get { return this.legendes; }
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
        /// Gets the type name of an instance of this class
        /// </summary>
        public string TypeName
        {
            get { return "Syntax"; }
        }

        /// <summary>
        /// Extracts dictionary keys
        /// </summary>
        /// <param name="proc">current process</param>
        public void ExtractDictionary(IProcessInstance proc)
        {
        }

        /// <summary>
        /// Injects a syntax
        /// </summary>
        /// <param name="injector">injector where to inject</param>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node object</param>
        /// <param name="modifier">modifier switch</param>
        public void Inject(Injector injector, ICompilateurInstance comp, System.Xml.XmlNode node, string modifier)
        {
            this.Load(comp, node);
        }

        /// <summary>
        /// Says if this statement is computable when it's injected
        /// </summary>
        /// <param name="proc">process to test</param>
        /// <param name="reason">error text</param>
        /// <returns>true if succeeded else false (reason parameter contains explanation)</returns>
        public bool IsComputable(IProcess proc, out string reason)
        {
            reason = null;
            return true;
        }

        /// <summary>
        /// Load template data
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node object</param>
        public void Load(ICompilateurInstance comp, XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.name = node.Attributes.GetNamedItem("name").Value;
            this.legendes.Load(node.SelectSingleNode("legendes"));
            this.xmlCode = node.SelectSingleNode("code").InnerXml;
        }

        /// <summary>
        /// Displays this statement into the web browser
        /// </summary>
        /// <param name="list">binding data to insert</param>
        /// <param name="node">node tree of statements</param>
        /// <param name="forcedIndent">true if sets at a particular indentation</param>
        /// <param name="indent">a particular indentation</param>
        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displaySyntax", new object[] { this.name, this.xmlCode, this.legendes, this.indent.ToString(), false }));
        }

        /// <summary>
        /// Displays this statement into the web browser in a read only mode
        /// </summary>
        /// <param name="list">binding data to insert</param>
        /// <param name="node">node tree of statements</param>
        /// <param name="forcedIndent">true if sets at a particular indentation</param>
        /// <param name="indent">a particular indentation</param>
        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displaySyntax", new object[] { this.name, this.xmlCode, this.legendes, this.indent.ToString(), true }));
        }

        /// <summary>
        /// Updates data from the web browser (before save)
        /// </summary>
        /// <param name="elem">html element</param>
        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            string name = this.GetElementByName(elem, "name").InnerText;
            if (String.IsNullOrEmpty(name)) name = "";
            this.name = name;
            // mise à jour des légendes
            this.legendes.Update(this.GetElementByName(elem, "myLegendes"));
            this.xmlCode = this.GetElementByName(elem, "xml").InnerHtml;
        }

        /// <summary>
        /// Save the statement into an xml file
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="writer">xml writer object</param>
        /// <param name="child">html child to save</param>
        public void Save(ICompilateur comp, XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("syntax");
            writer.WriteAttributeString("name", this.name);
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("legendes");
            this.legendes.Save(writer);
            writer.WriteEndElement();
            writer.WriteStartElement("code");
            writer.WriteRaw(this.xmlCode);
            writer.WriteEndElement();
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        /// <summary>
        /// Parse is not load
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node from reading</param>
        public void Parse(ICompilateur comp, XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.name = node.Attributes.GetNamedItem("name").Value;
            this.legendes.Load(node.SelectSingleNode("legendes"));
            this.xmlCode = node.SelectSingleNode("code").InnerXml;
        }

        /// <summary>
        /// Process the statement and writes results in a file
        /// </summary>
        /// <param name="proc">process object</param>
        /// <param name="file">final file object</param>
        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
        }

        /// <summary>
        /// Converts this statement for a particular programming language destination
        /// </summary>
        /// <param name="converter">converter object</param>
        /// <param name="proc">process object</param>
        /// <param name="file">final file</param>
        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
        }

        #endregion
    }
}
