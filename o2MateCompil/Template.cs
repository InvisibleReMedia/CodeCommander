using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Converters;

namespace o2Mate
{
    /// <summary>
    /// Template class
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class Template : ICompiler
    {
        #region Public Constants
        /// <summary>
        /// Parameter identifier
        /// </summary>
        public static readonly char ParamIdentifier = Compilateur.ParamIdentifier;
        #endregion

        #region Private Fields
        private string path;
        private string name;
        private Dictionary<string, string> pars;
        private string xmlCode;
        private int indent;
        private LegendeDict legendes;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Template()
        {
            this.path = "";
            this.name = "";
            this.legendes = new LegendeDict();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the template path
        /// </summary>
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

        /// <summary>
        /// Gets or sets the name of the template
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
        /// Gets the inner XML string embedded for a template
        /// </summary>
        public string XmlCode
        {
            get
            {
                return this.xmlCode;
            }
        }

        /// <summary>
        /// Gets summary
        /// </summary>
        public ILegendeDict Legendes
        {
            get { return this.legendes; }
        }

        /// <summary>
        /// Gets the parameter list in a string representation
        /// </summary>
        public string Parameters
        {
            get
            {
                string output = "";
                if (this.pars != null)
                {
                    foreach (KeyValuePair<string, string> kv in this.pars)
                    {
                        output += kv.Key + " ";
                    }
                }
                return output;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Replace all params with values from the inner XML string
        /// </summary>
        /// <param name="values">dictionary values</param>
        /// <returns>the correct inner XML string without parameters</returns>
        public string ReplaceParams(Dictionary<string, string> values)
        {
            string output = "";
            string xmlCode = this.XmlCode;
            for (int strIndex = 0; strIndex < xmlCode.Length; ++strIndex)
            {
                char c = this.XmlCode[strIndex];
                if (c == Template.ParamIdentifier && (strIndex + 1) < xmlCode.Length)
                {
                    bool found = false;
                    Dictionary<string, string> sortedDict = new Dictionary<string, string>();
                    int maxLength = 0;
                    // rechercher parmi les paramètres celui qui a le nom le plus long
                    foreach (string paramName in this.pars.Keys)
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
                        output += Template.ParamIdentifier;
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
        /// Gets the type name of an instance of this class
        /// </summary>
        public string TypeName
        {
            get { return "Template"; }
        }

        /// <summary>
        /// Extracts dictionary keys
        /// </summary>
        /// <param name="proc">current process</param>
        public void ExtractDictionary(IProcessInstance proc)
        {
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
            string strParams = String.Empty;
            bool first = true;
            foreach (string param in this.pars.Keys)
            {
                if (!first) strParams += ", "; else first = false; 
                strParams += Compilateur.ParamIdentifier.ToString() + param;
            }
            list.Add(new DisplayElement(this.TypeName, "displayTemplate", new object[] { this.path + "/" + this.name, strParams, this.xmlCode, this.legendes, this.indent.ToString(), false }));
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
            string strParams = String.Empty;
            bool first = true;
            foreach (string param in this.pars.Keys)
            {
                if (!first) strParams += ", "; else first = false;
                strParams += Compilateur.ParamIdentifier.ToString() + param;
            }
            list.Add(new DisplayElement(this.TypeName, "displayTemplate", new object[] { this.path + "/" + this.name, strParams, this.xmlCode, this.legendes, this.indent.ToString(), true }));
        }

        /// <summary>
        /// Updates data from the web browser (before save)
        /// </summary>
        /// <param name="elem">html element</param>
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
            string strParams = this.GetElementByName(elem, "params").InnerText;
            this.pars = new Dictionary<string, string>();
            if (!String.IsNullOrEmpty(strParams))
            {
                string[] tab = strParams.Split(',');
                foreach (string param in tab)
                {
                    string name = String.Empty;
                    if (param.Trim().StartsWith(Template.ParamIdentifier.ToString()))
                    {
                        name = param.Trim().Substring(1).Trim();
                    }
                    else
                    {
                        name = param.Trim();
                    }
                    if (!this.pars.ContainsKey(name))
                        this.pars.Add(name, "");
                }
            }
            // mise à jour des legendes
            this.legendes.Update(this.GetElementByName(elem, "myLegendes"));
            this.xmlCode = this.GetElementByName(elem, "xml").InnerHtml;
        }

        /// <summary>
        /// Save the statement into an xml file
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="writer">xml writer object</param>
        /// <param name="child">html child to save</param>
        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("template");
            writer.WriteAttributeString("path", this.path);
            writer.WriteAttributeString("name", this.name);
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("legendes");
            this.legendes.Save(writer);
            writer.WriteEndElement();
            writer.WriteStartElement("params");
            foreach (string param in this.pars.Keys)
            {
                writer.WriteStartElement("param");
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
        /// Parse is not load
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node from reading</param>
        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.path = node.Attributes.GetNamedItem("path").Value;
            this.name = node.Attributes.GetNamedItem("name").Value;
            this.legendes.Load(node.SelectSingleNode("legendes"));
            this.pars = new Dictionary<string, string>();
            foreach (XmlNode paramNode in node.SelectNodes("params/param"))
            {
                if (!this.pars.ContainsKey(paramNode.InnerText))
                    this.pars.Add(paramNode.InnerText, "");
            }
            this.xmlCode = node.SelectSingleNode("code").InnerXml;
        }

        /// <summary>
        /// Injects a template
        /// </summary>
        /// <param name="injector">injector where to inject</param>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node object</param>
        /// <param name="modifier">modifier switch</param>
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
        /// Says if this statement is computable when it's injected
        /// </summary>
        /// <param name="proc">process to test</param>
        /// <param name="reason">error text</param>
        /// <returns>true if succeeded else false (reason parameter contains explanation)</returns>
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
                if (this.xmlCode.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans un programme", param.FormalParameter);
                        return false;
                    }
                    this.xmlCode = this.xmlCode.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                foreach(string key in this.pars.Keys)
                {
                    if (this.pars[key].Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                    {
                        if (!param.IsComputable)
                        {
                            reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.pars[key]);
                            return false;
                        }
                        this.pars[key] = this.pars[key].Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Load template data
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node object</param>
        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.path = node.Attributes.GetNamedItem("path").Value;
            this.name = node.Attributes.GetNamedItem("name").Value;
            this.legendes.Load(node.SelectSingleNode("legendes"));
            this.pars = new Dictionary<string, string>();
            foreach (XmlNode paramNode in node.SelectNodes("params/param"))
            {
                if (!this.pars.ContainsKey(paramNode.InnerText))
                    this.pars.Add(paramNode.InnerText, "");
            }
            this.xmlCode = node.SelectSingleNode("code").InnerXml;
        }

        /// <summary>
        /// Process the statement and writes results in a file
        /// </summary>
        /// <param name="proc">process object</param>
        /// <param name="file">final file object</param>
        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "declare template", this.Path, this.Name, this.Parameters));
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
