using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using Converters;

namespace o2Mate
{
    /// <summary>
    /// Converter a template name with a specific program at a particular programming language
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public struct NameConverter
    {
        /// <summary>
        /// Delegate for conversion
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="proc">process object</param>
        /// <param name="converter">converter object</param>
        /// <param name="file">final file</param>
        public delegate void Conversion(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file);
        /// <summary>
        /// Name of the template to use
        /// </summary>
        public string name;
        /// <summary>
        /// Delegate instance for conversion
        /// </summary>
        public Conversion del;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">name of the template to use</param>
        /// <param name="conv">delegate conversion</param>
        public NameConverter(string name, Conversion conv)
        {
            this.name = name;
            this.del = conv;
        }
    }

    /// <summary>
    /// Use a template statement
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class UseTemplate : ICompiler, ILanguageConverter
    {
        #region Private Delegate
        private delegate void ImplementsTemplate();
        #endregion

        #region Private Fields
        private string name;
        private Dictionary<string, string> defParams;
        private List<Coding> codings;
        private Template template;
        private int indent;
        private ICompilateurInstance cachedComp;
        private IProcessInstance cachedProc;
        private FinalFile cachedFile;
        private NameConverter[] namePowerShellConverters, nameVBScriptConverters, nameMicrosoftCPPConverters;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public UseTemplate()
        {
            this.name = "";
            this.codings = new List<Coding>();
            this.namePowerShellConverters = new NameConverter[] {
                new NameConverter("/CodeCommander/Condition", new NameConverter.Conversion(this.ConditionToPowerShell)),
                new NameConverter("/CodeCommander/WithCondition", new NameConverter.Conversion(this.WithConditionToPowerShell)),
                new NameConverter("/CodeCommander/LoopWithWhile", new NameConverter.Conversion(this.LoopWithWhileToPowerShell)),
                new NameConverter("/CodeCommander/LoopWithConditionalWhile", new NameConverter.Conversion(this.LoopWithConditionalWhileToPowerShell)),
                new NameConverter("/CodeCommander/LoopWhile", new NameConverter.Conversion(this.LoopWhileToPowerShell)),
                new NameConverter("/CodeCommander/LoopConditionalWhile", new NameConverter.Conversion(this.LoopConditionalWhileToPowerShell)),
                new NameConverter("/CodeCommander/LoopWithUntil", new NameConverter.Conversion(this.LoopWithUntilToPowerShell)),
                new NameConverter("/CodeCommander/LoopWithConditionalUntil", new NameConverter.Conversion(this.LoopWithConditionalUntilToPowerShell)),
                new NameConverter("/CodeCommander/LoopUntil", new NameConverter.Conversion(this.LoopUntilToPowerShell)),
                new NameConverter("/CodeCommander/LoopConditionalUntil", new NameConverter.Conversion(this.LoopConditionalUntilToPowerShell)),
                new NameConverter("/CodeCommander/ForEach", new NameConverter.Conversion(this.LoopForEachToPowerShell)),
                new NameConverter("/CodeCommander/ForEachConditional", new NameConverter.Conversion(this.LoopConditionalForEachToPowerShell)),
                new NameConverter("/CodeCommander/Loop", new NameConverter.Conversion(this.LoopToPowerShell)),
                new NameConverter("/CodeCommander/LoopInverse", new NameConverter.Conversion(this.LoopInverseToPowerShell)),
                new NameConverter("/String/length", new NameConverter.Conversion(this.StringLengthToPowerShell)),
                new NameConverter("/String/substring", new NameConverter.Conversion(this.StringSubstringToPowerShell)),
                new NameConverter("/String/replace", new NameConverter.Conversion(this.StringReplaceToPowerShell))
            };

            this.nameVBScriptConverters = new NameConverter[] {
                new NameConverter("/CodeCommander/Condition", new NameConverter.Conversion(this.ConditionToVBScript)),
                new NameConverter("/CodeCommander/WithCondition", new NameConverter.Conversion(this.WithConditionToVBScript)),
                new NameConverter("/CodeCommander/LoopWithWhile", new NameConverter.Conversion(this.LoopWithWhileToVBScript)),
                new NameConverter("/CodeCommander/LoopWithConditionalWhile", new NameConverter.Conversion(this.LoopWithConditionalWhileToVBScript)),
                new NameConverter("/CodeCommander/LoopWhile", new NameConverter.Conversion(this.LoopWhileToVBScript)),
                new NameConverter("/CodeCommander/LoopConditionalWhile", new NameConverter.Conversion(this.LoopConditionalWhileToVBScript)),
                new NameConverter("/CodeCommander/LoopWithUntil", new NameConverter.Conversion(this.LoopWithUntilToVBScript)),
                new NameConverter("/CodeCommander/LoopWithConditionalUntil", new NameConverter.Conversion(this.LoopWithConditionalUntilToVBScript)),
                new NameConverter("/CodeCommander/LoopUntil", new NameConverter.Conversion(this.LoopUntilToVBScript)),
                new NameConverter("/CodeCommander/LoopConditionalUntil", new NameConverter.Conversion(this.LoopConditionalUntilToVBScript)),
                new NameConverter("/CodeCommander/ForEach", new NameConverter.Conversion(this.LoopForEachToVBScript)),
                new NameConverter("/CodeCommander/ForEachConditional", new NameConverter.Conversion(this.LoopConditionalWhileToVBScript)),
                new NameConverter("/CodeCommander/Loop", new NameConverter.Conversion(this.LoopToVBScript)),
                new NameConverter("/CodeCommander/LoopInverse", new NameConverter.Conversion(this.LoopInverseToVBScript)),
                new NameConverter("/String/length", new NameConverter.Conversion(this.StringLengthToVBScript)),
                new NameConverter("/String/substring", new NameConverter.Conversion(this.StringSubstringToVBScript)),
                new NameConverter("/String/replace", new NameConverter.Conversion(this.StringReplaceToVBScript))
            };

            this.nameMicrosoftCPPConverters = new NameConverter[] {
                new NameConverter("/CodeCommander/Condition", new NameConverter.Conversion(this.ConditionToMicrosoftCPP)),
                new NameConverter("/CodeCommander/WithCondition", new NameConverter.Conversion(this.WithConditionToMicrosoftCPP)),
                new NameConverter("/CodeCommander/LoopWithWhile", new NameConverter.Conversion(this.LoopWithWhileToMicrosoftCPP)),
                new NameConverter("/CodeCommander/LoopWithConditionalWhile", new NameConverter.Conversion(this.LoopWithConditionalWhileToMicrosoftCPP)),
                new NameConverter("/CodeCommander/LoopWhile", new NameConverter.Conversion(this.LoopWhileToMicrosoftCPP)),
                new NameConverter("/CodeCommander/LoopConditionalWhile", new NameConverter.Conversion(this.LoopConditionalWhileToMicrosoftCPP)),
                new NameConverter("/CodeCommander/LoopWithUntil", new NameConverter.Conversion(this.LoopWithUntilToMicrosoftCPP)),
                new NameConverter("/CodeCommander/LoopWithConditionalUntil", new NameConverter.Conversion(this.LoopWithConditionalUntilToMicrosoftCPP)),
                new NameConverter("/CodeCommander/LoopUntil", new NameConverter.Conversion(this.LoopUntilToMicrosoftCPP)),
                new NameConverter("/CodeCommander/LoopConditionalUntil", new NameConverter.Conversion(this.LoopConditionalUntilToMicrosoftCPP)),
                new NameConverter("/CodeCommander/ForEach", new NameConverter.Conversion(this.LoopForEachToMicrosoftCPP)),
                new NameConverter("/CodeCommander/ForEachConditional", new NameConverter.Conversion(this.LoopConditionalWhileToMicrosoftCPP)),
                new NameConverter("/CodeCommander/Loop", new NameConverter.Conversion(this.LoopToMicrosoftCPP)),
                new NameConverter("/CodeCommander/LoopInverse", new NameConverter.Conversion(this.LoopInverseToMicrosoftCPP)),
                new NameConverter("/String/length", new NameConverter.Conversion(this.StringLengthToMicrosoftCPP)),
                new NameConverter("/String/substring", new NameConverter.Conversion(this.StringSubstringToMicrosoftCPP)),
                new NameConverter("/String/replace", new NameConverter.Conversion(this.StringReplaceToMicrosoftCPP))
            };
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the name of the template to use
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
        /// Gets parameters (name/value) for the template to use
        /// </summary>
        public Dictionary<string, string> Parameters
        {
            get { return this.defParams; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a coding by name
        /// </summary>
        /// <param name="name">coding name</param>
        /// <returns>a coding object</returns>
        internal Coding GetCoding(string name)
        {
            foreach (Coding c in this.codings)
            {
                if (c.CodingName == name)
                {
                    return c;
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a new coding object
        /// </summary>
        /// <param name="code">coding object</param>
        internal void AddCoding(Coding code)
        {
            this.codings.Add(code);
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

        private bool IsConvertible(ICodeConverter converter, string name)
        {
            bool isConvertible = true;
            switch (converter.LanguageName)
            {
                case Languages.PowerShell:
                    foreach (NameConverter conv in this.namePowerShellConverters)
                    {
                        if (conv.name == name)
                        {
                            isConvertible = false;
                        }
                    }
                    break;
                case Languages.VBScript:
                    foreach (NameConverter conv in this.nameVBScriptConverters)
                    {
                        if (conv.name == name)
                        {
                            isConvertible = false;
                        }
                    }
                    break;
                case Languages.MicrosoftCPP:
                    foreach (NameConverter conv in this.nameMicrosoftCPPConverters)
                    {
                        if (conv.name == name)
                        {
                            isConvertible = false;
                        }
                    }
                    break;
            }
            return isConvertible;
        }
        
        private void NewFunctionForMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, NameConverter conv, FinalFile file)
        {
            Helper.IsolatedFunction(comp, proc, converter, new Helper.IsolatedFunctionDelegate(delegate()
            {
                comp.Convert(converter, this.Name, conv, file);
            }));
        }

        #endregion

        #region Compiler Membres

        /// <summary>
        /// Gets the type name of an instance of this class
        /// </summary>
        public string TypeName
        {
            get { return "UseTemplate"; }
        }

        /// <summary>
        /// Extracts dictionary keys
        /// </summary>
        /// <param name="proc">current process</param>
        public void ExtractDictionary(IProcessInstance proc)
        {
            proc.CurrentDictionnaire.Legendes.CopyFrom(this.template.Legendes);
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
            foreach (KeyValuePair<string,string> kv in this.defParams)
            {
                if (!first) { strParams += " "; } else { first = false; }
                strParams += "@" + kv.Key + "=" + kv.Value;
            }
            list.Add(new DisplayElement(this.TypeName, "displayUseTemplate", new object[] { this.name, strParams, this.indent.ToString(), false }));
            int subIndent = this.indent + 1;
            foreach (Node<ICompiler> child in node)
            {
                child.Object.Display(list, child, true, ref subIndent);
            }
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
            foreach (KeyValuePair<string, string> kv in this.defParams)
            {
                if (!first) { strParams += " "; } else { first = false; }
                strParams += "@" + kv.Key + "=" + kv.Value;
            }
            list.Add(new DisplayElement(this.TypeName, "displayUseTemplate", new object[] { this.name, strParams, this.indent.ToString(), true }));
            int subIndent = this.indent + 1;
            foreach (Node<ICompiler> child in node)
            {
                child.Object.DisplayReadOnly(list, child, true, ref subIndent);
            }
        }

        /// <summary>
        /// Updates data from the web browser (before save)
        /// </summary>
        /// <param name="elem">html element</param>
        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.name = this.GetElementByName(elem, "path").InnerText;
            if (String.IsNullOrEmpty(this.name)) this.name = "";
            string strParams = this.GetElementByName(elem, "params").InnerText + " ";
            this.defParams = new Dictionary<string, string>();
            // on peut ajouter des parentheses pour utiliser le caractère =
            Regex reg = new Regex(@"@([a-zA-Z_0-9]+)=(?<sentence>(?<word>\w|[^@]|\s)+)");
            if (!String.IsNullOrEmpty(strParams))
            {
                MatchCollection mColl = reg.Matches(strParams);
                foreach (Match m in mColl)
                {
                    if (m.Success)
                    {
                        string value = m.Groups[2].Value.Trim();
                        value = value.Replace("&", "&amp;");
                        value = value.Replace("<", "&lt;");
                        value = value.Replace(">", "&gt;");
                        if (!this.defParams.ContainsKey(m.Groups[1].Value))
                            this.defParams.Add(m.Groups[1].Value, value);
                    }
                }
            }
        }

        /// <summary>
        /// Save this statement into an xml file
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="writer">xml writer object</param>
        /// <param name="child">html child to save</param>
        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("usetemplate");
            writer.WriteAttributeString("name", this.name);
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("params");
            foreach (KeyValuePair<string, string> keyValue in this.defParams)
            {
                writer.WriteStartElement("param");
                writer.WriteAttributeString("name", keyValue.Key);
                writer.WriteString(keyValue.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("codings");
            System.Windows.Forms.HtmlElement subElement = child.NextSibling;
            comp.Save(writer, ref subElement, this.indent + 1);
            writer.WriteEndElement();
            writer.WriteEndElement();
            child = subElement;
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
            this.defParams = new Dictionary<string, string>();
            foreach (XmlNode paramNode in node.SelectNodes("params/param"))
            {
                string paramName = paramNode.Attributes.GetNamedItem("name").Value;
                string paramValue = paramNode.InnerText;
                if (!this.defParams.ContainsKey(paramName))
                    this.defParams.Add(paramName, paramValue);
            }
            XmlNode nodeCodings = node.SelectSingleNode("codings");
            comp.Parse(comp, nodeCodings);
        }

        /// <summary>
        /// Injects a use template
        /// </summary>
        /// <param name="injector">injector where to inject</param>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node object</param>
        /// <param name="modifier">modifier switch</param>
        public void Inject(Injector injector, ICompilateurInstance comp, System.Xml.XmlNode node, string modifier)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.name = node.Attributes.GetNamedItem("name").Value;
            this.defParams = new Dictionary<string, string>();
            if (node.SelectNodes("params")[0].FirstChild != null)
            {
                foreach (XmlNode paramNode in node.SelectNodes("params/param"))
                {
                    string paramName = paramNode.Attributes.GetNamedItem("name").Value;
                    string paramValue = paramNode.InnerText;
                    if (!this.defParams.ContainsKey(paramName))
                        this.defParams.Add(paramName, paramValue);
                }
            }
            if (comp.UnderConversion)
            {
                string s = String.Empty;
                if (!this.IsComputable(injector.InjectedProcess, out s))
                {
                    throw new Exception(s);
                }
            }
            // démarre un using template
            comp.StartUsingTemplate(this);
            // itère les extraits de code
            XmlNode nodeCodings = node.SelectSingleNode("codings");
            comp.Injection(injector, nodeCodings, modifier);
            // on ne charge pas les données si le template est déjà développé
            if (!comp.UnderConversion || this.IsConvertible(comp.ConvertedLanguage, this.Name))
            {
                this.template = comp.GetTemplate(this.name);
                string code = this.template.ReplaceParams(this.defParams);
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                // charge dans le compilateur l'extrait de code du template
                doc.LoadXml("<code>" + code + "</code>");
                comp.Injection(injector, doc.DocumentElement, modifier);
            }
            else
            {
                // on a besoin de récupérer le compilateur pour plus tard
                this.cachedComp = comp;
            }
            // termine un using template
            comp.EndUsingTemplate(this);
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
                if (this.name.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.name);
                        return false;
                    }
                    this.name = this.name.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                string[] keys = new string[this.defParams.Keys.Count];
                this.defParams.Keys.CopyTo(keys, 0);
                foreach(string key in keys)
                {
                    if (this.defParams[key].Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                    {
                        if (!param.IsComputable)
                        {
                            reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.defParams[key]);
                            return false;
                        }
                        this.defParams[key] = this.defParams[key].Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Load statement data
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node object</param>
        public void Load(ICompilateurInstance comp, XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.name = node.Attributes.GetNamedItem("name").Value;
            this.defParams = new Dictionary<string,string>();
            if (node.SelectNodes("params")[0].FirstChild != null)
            {
                foreach (XmlNode paramNode in node.SelectNodes("params/param"))
                {
                    string paramName = paramNode.Attributes.GetNamedItem("name").Value;
                    string paramValue = paramNode.InnerText;
                    if (!this.defParams.ContainsKey(paramName))
                        this.defParams.Add(paramName, paramValue);
                }
            }
            // démarre un using template
            comp.StartUsingTemplate(this);
            // itère les extraits de code
            XmlNode nodeCodings = node.SelectSingleNode("codings");
            comp.Load(comp, nodeCodings);
            // on ne charge pas les données si le template est déjà développé
            if (!comp.UnderConversion || this.IsConvertible(comp.ConvertedLanguage, this.Name))
            {
                this.template = comp.GetTemplate(this.name);
                string code = this.template.ReplaceParams(this.defParams);
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                // charge dans le compilateur l'extrait de code du template
                doc.LoadXml("<code>" + code + "</code>");
                comp.Load(comp, doc.DocumentElement);
            }
            else
            {
                // on a besoin de récupérer le compilateur pour plus tard
                this.cachedComp = comp;
            }
            // termine un using template
            comp.EndUsingTemplate(this);
        }

        /// <summary>
        /// Process this statement and writes results in a file
        /// </summary>
        /// <param name="proc">process object</param>
        /// <param name="file">final file object</param>
        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "use template", this.Name));
        }

        /// <summary>
        /// Converts this statement for a particular programming language destination
        /// </summary>
        /// <param name="converter">converter object</param>
        /// <param name="proc">process object</param>
        /// <param name="file">final file</param>
        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            // nous avons besoin de stocker ces données pour les réutiliser
            // en dehors de la fonction (notamment quand on effectue la conversion)
            this.cachedFile = file;
            this.cachedProc = proc;
            converter.Convert(this);
        }

        #endregion

        #region ILanguageConverter Membres

        private void LoopToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "0", EnumDataType.E_NUMBER);

            converter.CurrentFunction.AddToSource("for($[left:" + counter + "] = 1; $[byvalue:" + counter + "] <= " + value + "; ++$[byvalue:" + counter + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            Helper.CallCoding(comp, converter, this, "body", file);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "0", EnumDataType.E_NUMBER);

            converter.CurrentFunction.AddToSource("For " + counter + " = 1 To " + value + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Next" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "0", EnumDataType.E_NUMBER);

            converter.CurrentFunction.AddToSource("for(S[left:" + counter + "] = 1;S[byvalue:" + counter + "] -le " + value + ";++S[byvalue:" + counter + "])" + Environment.NewLine + "{" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopInverseToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "0", EnumDataType.E_NUMBER);

            converter.CurrentFunction.AddToSource("for($[left:" + counter + "] = " + value + "; $[byvalue:" + counter + "] > 0; --$[byvalue:" + counter + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            Helper.CallCoding(comp, converter, this, "body", file);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopInverseToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "0", EnumDataType.E_NUMBER);

            converter.CurrentFunction.AddToSource("For " + counter + " = " + value + " To 1 Step -1" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Next" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopInverseToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "0", EnumDataType.E_NUMBER);

            converter.CurrentFunction.AddToSource("for(S[left:" + counter + "] = " + value + ";S[byvalue:" + counter + "] -ge 1;--S[byvalue:" + counter + "])" + Environment.NewLine + "{" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWhileToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string prefixedFirst = o2Mate.Scope.ConstructPrefixedName(o2Mate.EnumDataType.E_BOOL, first);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("$[left:" + counter + "] = " + init + ";" + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("while (" + limit + ") {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!$[byvalue:" + prefixedFirst + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.AddToSource("++$[byvalue:" + counter + "];" + Environment.NewLine);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = false;" + Environment.NewLine);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWhileToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource(counter + " = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("While " + limit + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("If Not " + name + "First Then" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.AddToSource(counter + " = " + counter + " + 1" + Environment.NewLine);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = False" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Wend" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWhileToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("S[left:" + counter + "] = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("while (" + limit + ") {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!S[byvalue:" + first + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.AddToSource("++S[byvalue:" + counter + "]" + Environment.NewLine);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $false;" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithWhileToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string prefixedFirst = o2Mate.Scope.ConstructPrefixedName(o2Mate.EnumDataType.E_BOOL, first);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("$[left:" + counter + "] = " + init + ";" + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("$[left:" + limit + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("while ($[byvalue:" + limit + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.AddToSource("if ($[byvalue:" + limit + "]) {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!$[byvalue:" + prefixedFirst + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            Helper.CallCoding(comp, converter, this, "increment", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = false;" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithWhileToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource(counter + " = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource(limit + " = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("While " + limit + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.AddToSource("If " + limit + " Then" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("If Not " + name + "First Then" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            Helper.CallCoding(comp, converter, this, "increment", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = False" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Wend" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithWhileToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("S[left:" + counter + "] = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("S[left:" + limit + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("while (S[byvalue:" + limit + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.AddToSource("if (S[byvalue:" + limit + "]) {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!S[byvalue:" + first + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            Helper.CallCoding(comp, converter, this, "increment", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $false" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopUntilToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string prefixedFirst = o2Mate.Scope.ConstructPrefixedName(o2Mate.EnumDataType.E_BOOL, first);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("$[left:" + counter + "] = " + init + ";" + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("do {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!$[byvalue:" + prefixedFirst + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.AddToSource("++$[byvalue:" + counter + "];" + Environment.NewLine);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = false;" + Environment.NewLine);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("} while(!(" + limit + "));" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopUntilToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource(counter + " = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("Do Until " + limit + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("If Not" + name + "First Then" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.AddToSource(counter + " = " + counter + " + 1" + Environment.NewLine);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = False" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Loop" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopUntilToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("S[left:" + counter + "] = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("do {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!S[byvalue:" + first + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.AddToSource("++S[byvalue:" + counter + "]" + Environment.NewLine);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $false" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("} while (!(" + limit + "))" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithUntilToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string prefixedFirst = o2Mate.Scope.ConstructPrefixedName(o2Mate.EnumDataType.E_BOOL, first);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("$[left:" + counter + "] = " + init + ";" + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("do {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!$[byvalue:" + prefixedFirst + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            Helper.CallCoding(comp, converter, this, "increment", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = false;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("$[left:" + limit + "] = true;" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "limit", file);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("} while(!$[byvalue:" + limit + "]);" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithUntilToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource(counter + " = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource(limit + " = False" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("Do Until " + limit + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("If Not " + name + "First Then" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            Helper.CallCoding(comp, converter, this, "increment", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = False" + Environment.NewLine);
            converter.CurrentFunction.AddToSource(limit + " = False" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Loop" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithUntilToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("S[left:" + counter + "] = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("do {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!S[byvalue:" + first + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            Helper.CallCoding(comp, converter, this, "increment", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $false" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("S[left:" + limit + "] = $false" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("} while(!S[byvalue:" + limit + "])" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopForEachToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string prefixedFirst = o2Mate.Scope.ConstructPrefixedName(o2Mate.EnumDataType.E_BOOL, first);
            string tabName = Helper.CreateValue(this, "tabName");
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("for($[left:" + counter + "] = " + init + "; $[byvalue:" + counter + "] <= this->GetArrayCount(wstring(L\"" + tabName + "\")); ++$[byvalue:" + counter + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!$[byvalue:" + prefixedFirst + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = false;" + Environment.NewLine);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopForEachToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string tabName = Helper.CreateValue(this, "tabName");
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("For " + counter + " = (" + init + ") To (theDict.GetArray(\"" + tabName + "\").Count)" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("If Not " + name + "First Then" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = False" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Next" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopForEachToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string tabName = Helper.CreateValue(this, "tabName");
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("for(S[left:" + counter + "] = " + init + ";S[byvalue:" + counter + "] -le $theDict.GetArray(\"" + tabName + "\").Count;++S[byvalue:" + counter + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!S[byvalue:" + first + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $false" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopConditionalForEachToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string prefixedFirst = o2Mate.Scope.ConstructPrefixedName(o2Mate.EnumDataType.E_BOOL, first);
            string tabName = Helper.CreateValue(this, "tabName");
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("for($[left:" + counter + "] = " + init + "; $[byvalue:" + counter + "] <= this->GetArrayCount(\"" + tabName + "\"); ++$[byvalue:" + counter + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource("$[left:" + condition + "] = true;" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if ($[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.ForwardControlFlowInIf();

            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!$[byvalue:" + prefixedFirst + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = false;" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopConditionalForEachToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string tabName = Helper.CreateValue(this, "tabName");
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("For " + counter + " = (" + init + ") To (theDict.GetArray(\"" + tabName + "\").Count)" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource(condition + " = True" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("If " + condition + " Then" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.ForwardControlFlowInIf();
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("If Not " + name + "First Then" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = False" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Next" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopConditionalForEachToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string tabName = Helper.CreateValue(this, "tabName");
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("for(S[left:" + counter + "] = " + init + ";S[byvalue:" + counter + "] -le $theDict.GetArray(\"" + tabName + "\").Count;++S[byvalue:" + counter + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource("S[left:" + condition + "] = $true" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if (S[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.ForwardControlFlowInIf();
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!S[byvalue:" + first + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $false" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopConditionalWhileToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string prefixedFirst = o2Mate.Scope.ConstructPrefixedName(o2Mate.EnumDataType.E_BOOL, first);
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[byvalue:" + prefixedFirst + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("$[left:" + counter + "] = " + init + ";" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("while(" + limit + ") {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            converter.CurrentFunction.AddToSource("$[left:" + condition + "] = true;" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if ($[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.ForwardControlFlowInIf();
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!$[byvalue:" + prefixedFirst + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = false;" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("++$[byvalue:" + counter + "];" + Environment.NewLine);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopConditionalWhileToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource(counter + " = " + init + Environment.NewLine);
            converter.CurrentFunction.AddToSource("While " + limit + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource(condition + " = True" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("If " + condition + " Then" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.ForwardControlFlowInIf();
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("If Not " + name + "First Then" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = False" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            converter.CurrentFunction.AddToSource(counter + " = " + counter + " + 1" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Wend" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopConditionalWhileToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("S[left:" + counter + "] = " + init + Environment.NewLine);
            converter.CurrentFunction.AddToSource("while (" + limit + ") {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource("S[left:" + condition + "] = $true" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if (S[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.ForwardControlFlowInIf();
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!S[byvalue:" + first + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $false" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("++S[byvalue:" + counter + "]" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithConditionalWhileToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string prefixedFirst = o2Mate.Scope.ConstructPrefixedName(o2Mate.EnumDataType.E_BOOL, first);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("$[left:" + counter + "] = " + init + ";" + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("$[left;" + limit + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("while ($[byvalue:" + limit + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.AddToSource("if ($[byvalue:" + limit + "]) {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource("$[left:" + condition + "] = true;" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if ($[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!$[left:" + prefixedFirst + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = false;" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "increment", file);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithConditionalWhileToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource(counter + " = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource(limit + " = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("While " + limit + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.AddToSource("If " + limit + " Then" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource(condition + " = True" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("If " + condition + " Then" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("If Not " + name + "First Then" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = False" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            Helper.CallCoding(comp, converter, this, "increment", file);
            converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Wend" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithConditionalWhileToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("S[left:" + counter + "] = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("S[left:" + limit + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("while (S[byvalue:" + limit + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.AddToSource("if (S[byvalue:" + limit + "]) {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource("S[left:" + condition + "] = $true" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if (S[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!S[byvalue:" + first + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $false" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "increment", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopConditionalUntilToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string prefixedFirst = o2Mate.Scope.ConstructPrefixedName(o2Mate.EnumDataType.E_BOOL, first);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("$[left:" + counter + "] = " + init + ";" + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("do {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            converter.CurrentFunction.AddToSource("$[left:" + condition + "] = true;" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if ($[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!$[byvalue:" + prefixedFirst + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = false;" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("++$[byvalue:" + counter + "];" + Environment.NewLine);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("} while(!(" + limit + "));" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopConditionalUntilToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource(counter + " = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("Do Until " + limit + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource(condition + " = True" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("If " + condition + " Then" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("If Not " + name + "First Then" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = False" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            converter.CurrentFunction.AddToSource(counter + " = " + counter + " + 1" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Loop" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopConditionalUntilToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateExprVariable(converter, comp, proc, this, "limit", "0", EnumDataType.E_BOOL);
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("S[left:" + counter + "] = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("do {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource("S[left:" + condition + "] = $true" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if (S[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!S[byvalue:" + first + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $false" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("++S[byvalue:" + counter + "]" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("} while(!(" + limit + "))" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithConditionalUntilToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = MicrosoftCPPConverter.Escape(name) + "First";
            string prefixedFirst = o2Mate.Scope.ConstructPrefixedName(o2Mate.EnumDataType.E_BOOL, first);
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("$[left:" + counter + "] = " + init + ";" + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = true;" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("do {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;

            converter.CurrentFunction.AddToSource("$[left:" + limit + "] = false;" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.AddToSource("$[left:" + condition + "] = true;" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if ($[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!$[byvalue:" + prefixedFirst + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, first, EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("$[left:" + prefixedFirst + "] = false;" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "increment", file);

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("} while(!$[byvalue:" + limit + "]);" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithConditionalUntilToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name");
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource(counter + " = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = True" + Environment.NewLine);
            converter.CurrentFunction.AddToSource(limit + " = False" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("Do Until " + limit + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource(condition + " = True" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("If " + condition + " Then" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("If Not " + name + "First Then" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource(name + "First = False" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "increment", file);
            converter.CurrentFunction.AddToSource(limit + " = False" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("Loop" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void LoopWithConditionalUntilToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            converter.CurrentFunction.ForwardControlFlowInLoop();
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string counter = Helper.CreateDependantVariable(comp, converter, proc, this, "counter", EnumDataType.E_NUMBER);
            string init = Helper.CreateExprVariable(converter, comp, proc, this, "init", "0", EnumDataType.E_NUMBER);
            string limit = Helper.CreateDependantVariable(comp, converter, proc, this, "limit", EnumDataType.E_BOOL);
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);
            converter.CurrentFunction.AddToSource("S[left:" + counter + "] = " + init + Environment.NewLine);
            Coding codingNext = this.GetCoding("next");
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $true" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("do {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource("S[left:" + limit + "] = $false" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "limit", file);
            converter.CurrentFunction.AddToSource("S[left:" + condition + "] = $true" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if (S[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            if (codingNext != null)
            {
                converter.CurrentFunction.AddToSource("if (!S[byvalue:" + first + "]) {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, proc, converter, codingNext, name + "First", EnumDataType.E_BOOL, file);
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            }
            Helper.CallCoding(comp, converter, this, "body", file);
            if (codingNext != null)
                converter.CurrentFunction.AddToSource("S[left:" + first + "] = $false" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "increment", file);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("} while(!S[byvalue:" + limit + "])" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowAfterLoop();
        }

        private void ConditionToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string condition = Helper.CreateExprVariable(converter, comp, proc, this, "condition", "0", EnumDataType.E_BOOL);

            converter.CurrentFunction.AddToSource("if (" + condition + ") {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;

            Helper.IsolatedFunction(comp, proc, converter, new Helper.IsolatedFunctionDelegate(delegate()
            {
                Helper.CallCoding(comp, converter, this, "true", file);

                if (this.GetCoding("false") != null)
                {
                    converter.CurrentFunction.IndentSize -= 1;
                    converter.CurrentFunction.ForwardControlFlowAfterIf();
                    converter.CurrentFunction.AddToSource("} else {" + Environment.NewLine);
                    converter.CurrentFunction.ForwardControlFlowInIf();
                    converter.CurrentFunction.IndentSize += 1;

                    Helper.CallCoding(comp, converter, this, "false", file);
                }

            }));

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
        }

        private void ConditionToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string name = Helper.CreateValue(this, "name");
            string condition = Helper.CreateExprVariable(converter, comp, proc, this, "condition", "0", EnumDataType.E_BOOL);

            converter.CurrentFunction.AddToSource("If " + condition + " Then" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "true", file);
            Coding codingElse = this.GetCoding("false");
            if (codingElse != null)
            {
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("Else" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, converter, this, "false", file);
            }
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
        }

        private void ConditionToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string condition = Helper.CreateExprVariable(converter, comp, proc, this, "condition", "0", EnumDataType.E_BOOL);

            converter.CurrentFunction.AddToSource("if (" + condition + ") {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "true", file);
            Coding codingElse = this.GetCoding("false");
            if (codingElse != null)
            {
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("} else {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, converter, this, "false", file);
            }
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
        }

        private void WithConditionToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);

            converter.CurrentFunction.AddToSource("$[left:" + condition + "] = true;" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);

            converter.CurrentFunction.AddToSource("if ($[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;

            Helper.IsolatedFunction(comp, proc, converter, new Helper.IsolatedFunctionDelegate(delegate()
            {
                Helper.CallCoding(comp, converter, this, "true", file);
            }));

            if (this.GetCoding("false") != null)
            {
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("} else {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;

                Helper.IsolatedFunction(comp, proc, converter, new Helper.IsolatedFunctionDelegate(delegate()
                {
                    Helper.CallCoding(comp, converter, this, "false", file);
                }));
            }

            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
        }

        private void WithConditionToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string name = Helper.CreateValue(this, "name");
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);

            converter.CurrentFunction.AddToSource(condition + " = True" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("If " + condition + " Then" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "true", file);
            Coding codingElse = this.GetCoding("false");
            if (codingElse != null)
            {
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("Else" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, converter, this, "false", file);
            }
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("End If" + Environment.NewLine);
        }

        private void WithConditionToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string name = Helper.CreateValue(this, "name"); string first = PowerShellConverter.Escape(name) + "First";
            string condition = Helper.CreateDependantVariable(comp, converter, proc, this, "condition", EnumDataType.E_BOOL);

            converter.CurrentFunction.AddToSource("S[left:" + condition + "] = $true" + Environment.NewLine);
            Helper.CallCoding(comp, converter, this, "condition", file);
            converter.CurrentFunction.AddToSource("if (S[byvalue:" + condition + "]) {" + Environment.NewLine);
            converter.CurrentFunction.ForwardControlFlowInIf();
            converter.CurrentFunction.IndentSize += 1;
            Helper.CallCoding(comp, converter, this, "true", file);
            Coding codingElse = this.GetCoding("false");
            if (codingElse != null)
            {
                converter.CurrentFunction.IndentSize -= 1;
                converter.CurrentFunction.ForwardControlFlowAfterIf();
                converter.CurrentFunction.AddToSource("} else {" + Environment.NewLine);
                converter.CurrentFunction.ForwardControlFlowInIf();
                converter.CurrentFunction.IndentSize += 1;
                Helper.CallCoding(comp, converter, this, "false", file);
            }
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.ForwardControlFlowAfterIf();
            converter.CurrentFunction.AddToSource("}" + Environment.NewLine);
        }

        private void StringLengthToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "wstring(L\"\")", EnumDataType.E_STRING_OBJECT);
            string result = Helper.CreateDependantVariable(comp, converter, proc, this, "out", EnumDataType.E_NUMBER);
            converter.CurrentFunction.AddToSource("$[left:" + result + "] = " + value + ".length();" + Environment.NewLine);
        }

        private void StringLengthToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "\"\"", EnumDataType.E_STRING);
            string result = Helper.CreateDependantVariable(comp, converter, proc, this, "out", EnumDataType.E_NUMBER);
            converter.CurrentFunction.AddToSource(result + " = Len(" + value + ")" + Environment.NewLine);
        }

        private void StringLengthToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "\"\"", EnumDataType.E_STRING);
            string result = Helper.CreateDependantVariable(comp, converter, proc, this, "out", EnumDataType.E_NUMBER);
            converter.CurrentFunction.AddToSource("S[left:" + result + "] = (" + value + ").Length" + Environment.NewLine);
        }

        private void StringSubstringToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "wstring(L\"\")", EnumDataType.E_STRING_OBJECT);
            string start = Helper.CreateExprVariable(converter, comp, proc, this, "start", "0", EnumDataType.E_NUMBER);
            string length = Helper.CreateExprVariable(converter, comp, proc, this, "length", "0", EnumDataType.E_NUMBER);
            string result = Helper.CreateDependantVariable(comp, converter, proc, this, "out", EnumDataType.E_STRING_OBJECT);
            converter.CurrentFunction.AddToSource("$[left:" + result + "] = " + value + ".substr((" + start + ") - 1, " + length + ");" + Environment.NewLine);
        }

        private void StringSubstringToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "\"\"", EnumDataType.E_STRING);
            string start = Helper.CreateExprVariable(converter, comp, proc, this, "start", "0", EnumDataType.E_NUMBER);
            string length = Helper.CreateExprVariable(converter, comp, proc, this, "length", "0", EnumDataType.E_NUMBER);
            string result = Helper.CreateDependantVariable(comp, converter, proc, this, "out", EnumDataType.E_STRING);
            converter.CurrentFunction.AddToSource(result + " = Mid(" + value + ", " + start + ", " + length + ")" + Environment.NewLine);
        }

        private void StringSubstringToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "\"\"", EnumDataType.E_STRING);
            string start = Helper.CreateExprVariable(converter, comp, proc, this, "start", "0", EnumDataType.E_NUMBER);
            string length = Helper.CreateExprVariable(converter, comp, proc, this, "length", "0", EnumDataType.E_NUMBER);
            string result = Helper.CreateDependantVariable(comp, converter, proc, this, "out", EnumDataType.E_STRING);
            converter.CurrentFunction.AddToSource("S[left:" + result + "] = (" + value + ").SubString(" + start + " - 1, " + length + ")" + Environment.NewLine);
        }

        private void StringReplaceToMicrosoftCPP(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "L\"\"", EnumDataType.E_STRING);
            string search = Helper.CreateExprVariable(converter, comp, proc, this, "search", "L\"\"", EnumDataType.E_STRING);
            string replacement = Helper.CreateExprVariable(converter, comp, proc, this, "replacement", "L\"\"", EnumDataType.E_STRING);
            string result = Helper.CreateDependantVariable(comp, converter, proc, this, "out", EnumDataType.E_STRING_OBJECT);
            converter.CurrentFunction.AddToSource("$[left:" + result + "] = this->ReplaceString(" + value + ", " + search + ", " + replacement + ");" + Environment.NewLine);
        }

        private void StringReplaceToVBScript(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "\"\"", EnumDataType.E_STRING);
            string search = Helper.CreateExprVariable(converter, comp, proc, this, "search", "\"\"", EnumDataType.E_STRING);
            string replacement = Helper.CreateExprVariable(converter, comp, proc, this, "replacement", "\"\"", EnumDataType.E_STRING);
            string result = Helper.CreateDependantVariable(comp, converter, proc, this, "out", EnumDataType.E_STRING);
            converter.CurrentFunction.AddToSource(result + " = Replace(" + value + ", " + search + ", " + replacement + ")" + Environment.NewLine);
        }

        private void StringReplaceToPowerShell(ICompilateurInstance comp, IProcessInstance proc, ICodeConverter converter, FinalFile file)
        {
            string value = Helper.CreateExprVariable(converter, comp, proc, this, "value", "\"\"", EnumDataType.E_STRING);
            string search = Helper.CreateExprVariable(converter, comp, proc, this, "search", "\"\"", EnumDataType.E_STRING);
            string replacement = Helper.CreateExprVariable(converter, comp, proc, this, "replacement", "\"\"", EnumDataType.E_STRING);
            string result = Helper.CreateDependantVariable(comp, converter, proc, this, "out", EnumDataType.E_STRING);
            converter.CurrentFunction.AddToSource("S[left:" + result + "] = (" + value + ") -replace " + search + ", " + replacement + Environment.NewLine);
        }

        /// <summary>
        /// Write in perl
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInPerl(ICodeConverter converter)
        {
        }

        /// <summary>
        /// Write in python
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInPython(ICodeConverter converter)
        {
        }

        /// <summary>
        /// Write in unix cpp
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInUnixCPP(ICodeConverter converter)
        {
        }

        /// <summary>
        /// Write in Microsoft C# .NET (Visual Studio 2012)
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInCSharp(ICodeConverter converter)
        {
        }

        /// <summary>
        /// Write in java
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInJava(ICodeConverter converter)
        {
        }

        /// <summary>
        /// Write in Mac OS C++
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInMacOSCPP(ICodeConverter converter)
        {
        }

        /// <summary>
        /// Write in Microsoft C++ .NET (Visual Studio 2012)
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInMicrosoftCPP(ICodeConverter converter)
        {
            foreach (NameConverter conv in this.nameMicrosoftCPPConverters)
            {
                if (conv.name == this.Name)
                {
                    this.NewFunctionForMicrosoftCPP(this.cachedComp, this.cachedProc, converter, conv, this.cachedFile);
                }
            }
        }

        /// <summary>
        /// Write in VB Script
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInVBScript(ICodeConverter converter)
        {
            foreach (NameConverter conv in this.nameVBScriptConverters)
            {
                if (conv.name == this.Name)
                {
                    this.cachedComp.Convert(converter, this.Name, conv, this.cachedFile);
                }
            }
        }

        /// <summary>
        /// Write in Microsoft PowerShell
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInPowerShell(ICodeConverter converter)
        {
            foreach (NameConverter conv in this.namePowerShellConverters)
            {
                if (conv.name == this.Name)
                {
                    this.cachedComp.Convert(converter, this.Name, conv, this.cachedFile);
                }
            }
        }

        #endregion
    }
}
