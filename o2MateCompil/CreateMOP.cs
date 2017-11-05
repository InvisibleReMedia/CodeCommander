using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Converters;

namespace o2Mate
{
    /// <summary>
    /// Contains a new MOP (a specific statement)
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class CreateMOP : ICompiler
    {
        #region Private Constants
        private static readonly char ParamIdentifier = Compilateur.ParamIdentifier;
        #endregion

        #region Private Fields
        private string language;
        private string name;
        private List<string> refs;
        private string xmlCode;
        private int indent;
        private LegendeDict legendes;
        private ICompilateurInstance cachedComp;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public CreateMOP()
        {
            this.language = "";
            this.name = "";
            this.legendes = new LegendeDict();
            this.xmlCode = "";
            this.refs = new List<string>();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the programming language
        /// </summary>
        public string Language
        {
            get
            {
                return this.language;
            }
            set
            {
                this.language = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the MOP
        /// </summary>
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

        /// <summary>
        /// Gets the inner xml source code
        /// </summary>
        public string XmlCode
        {
            get
            {
                return this.xmlCode;
            }
        }

        /// <summary>
        /// Gets the summary object
        /// </summary>
        public LegendeDict Legendes
        {
            get
            {
                return this.legendes;
            }
        }

        /// <summary>
        /// Gets the parameter list in a separated comma parameter name
        /// </summary>
        public string ReferenceString
        {
            get
            {
                string output = "";
                bool first = true;
                foreach (string key in this.refs)
                {
                    if (!first) output += ", "; else first = false;
                    output += CreateMOP.ParamIdentifier + key;
                }
                return output;
            }
        }

        /// <summary>
        /// Gets the list of parameters name
        /// </summary>
        public List<string> References
        {
            get
            {
                return this.refs;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Search and replace parameters name in direct xml source
        /// </summary>
        /// <param name="values">a dictionary container parameters name and values</param>
        /// <returns>xml code change</returns>
        public string ReplaceParams(Dictionary<string, string> values)
        {
            string output = "";
            string xmlCode = this.XmlCode;
            for (int strIndex = 0; strIndex < xmlCode.Length; ++strIndex)
            {
                char c = this.XmlCode[strIndex];
                if (c == CreateMOP.ParamIdentifier && (strIndex + 1) < xmlCode.Length)
                {
                    bool found = false;
                    Dictionary<string, string> sortedDict = new Dictionary<string, string>();
                    int maxLength = 0;
                    // rechercher parmi les paramètres celui qui a le nom le plus long
                    foreach (string paramName in this.refs)
                    {
                        if ((strIndex + 1 + paramName.Length) < xmlCode.Length)
                        {
                            if (xmlCode.Substring(strIndex + 1, paramName.Length) == paramName)
                            {
                                if (values.ContainsKey(paramName))
                                {
                                    sortedDict.Add(paramName, values[paramName]);
                                    if (paramName.Length > maxLength)
                                    {
                                        maxLength = paramName.Length;
                                    }
                                }
                            }
                        }
                    }
                    // itérer tous les paramètres qui correspondent et prendre le plus long
                    foreach (string paramName in sortedDict.Keys)
                    {
                        if (paramName.Length == maxLength)
                        {
                            output += sortedDict[paramName];
                            found = true;
                            strIndex += paramName.Length;
                        }
                    }
                    if (!found)
                    {
                        output += CreateMOP.ParamIdentifier;
                    }
                }
                else
                {
                    output += c;
                }
            }
            return output;
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
            get { return "CreateMOP"; }
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
                if (this.language.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.language);
                        return false;
                    }
                    this.language = this.language.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
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
                if (this.xmlCode.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans un programme", param.FormalParameter);
                        return false;
                    }
                    this.xmlCode = this.xmlCode.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                for(int index = 0; index < this.refs.Count; ++index)
                {
                    if (this.refs[index].Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                    {
                        if (!param.IsComputable)
                        {
                            reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.refs[index]);
                            return false;
                        }
                        this.refs[index] = this.refs[index].Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                    }
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
            this.language = node.Attributes.GetNamedItem("language").Value;
            this.name = node.Attributes.GetNamedItem("name").Value;
            this.legendes.Load(node.SelectSingleNode("legendes"));
            this.refs.Clear();
            foreach (XmlNode paramNode in node.SelectNodes("references/ref"))
            {
                if (!this.refs.Contains(paramNode.InnerText))
                    this.refs.Add(paramNode.InnerText);
            }
            this.xmlCode = node.SelectSingleNode("code").InnerXml;
            // on en a besoin pour la conversion
            this.cachedComp = comp;
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
            list.Add(new DisplayElement(this.TypeName, "displayCreateMOP", new object[] { this.language, this.name, this.ReferenceString, this.xmlCode, this.legendes, this.indent.ToString(), false }));
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
            list.Add(new DisplayElement(this.TypeName, "displayCreateMOP", new object[] { this.language, this.name, this.ReferenceString, this.xmlCode, this.legendes, this.indent.ToString(), true }));
        }

        /// <summary>
        /// When saving the CodeCommander file source, receipts and updates infos from the tml element of the web navigator
        /// </summary>
        /// <param name="elem">html element</param>
        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.language = this.GetElementByName(elem, "language").InnerText;
            this.name = this.GetElementByName(elem, "name").InnerText;
            string strParams = this.GetElementByName(elem, "params").InnerText;
            this.refs.Clear();
            if (!String.IsNullOrEmpty(strParams))
            {
                string[] tab = strParams.Split(',');
                foreach (string param in tab)
                {
                    string name = param.Trim();
                    if (name.StartsWith(CreateMOP.ParamIdentifier.ToString()))
                        name = name.Substring(1).Trim();
                    if (!String.IsNullOrEmpty(name))
                    {
                        if (!this.refs.Contains(name))
                            this.refs.Add(name);
                    }
                }
            }
            // mise à jour des legendes
            this.legendes.Update(this.GetElementByName(elem, "myLegendes"));
            this.xmlCode = this.GetElementByName(elem, "xml").InnerHtml;
        }

        /// <summary>
        /// Saving the object into an xml node
        /// </summary>
        /// <param name="comp">this compiler</param>
        /// <param name="writer">Xml writer object</param>
        /// <param name="child">html child for nested objects</param>
        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("createmop");
            writer.WriteAttributeString("language", this.language);
            writer.WriteAttributeString("name", this.name);
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("legendes");
            this.legendes.Save(writer);
            writer.WriteEndElement();
            writer.WriteStartElement("references");
            foreach (string param in this.refs)
            {
                writer.WriteStartElement("ref");
                writer.WriteString(param);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("code");
            writer.WriteRaw(this.xmlCode);
            writer.WriteEndElement();
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
            this.language = node.Attributes.GetNamedItem("language").Value;
            this.name = node.Attributes.GetNamedItem("name").Value;
            this.legendes.Load(node.SelectSingleNode("legendes"));
            this.refs.Clear();
            foreach (XmlNode paramNode in node.SelectNodes("references/ref"))
            {
                if (!this.refs.Contains(paramNode.InnerText))
                    this.refs.Add(paramNode.InnerText);
            }
            this.xmlCode = node.SelectSingleNode("code").InnerXml;
        }

        /// <summary>
        /// Execute the object and write results onto the final file
        /// </summary>
        /// <param name="proc">current process</param>
        /// <param name="file">final file</param>
        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
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
