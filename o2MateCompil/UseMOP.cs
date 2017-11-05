using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Converters;

namespace o2Mate
{
    /// <summary>
    /// Use a mop statement class
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class UseMOP : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string language;
        private string command;
        private string expression;
        private int indent;
        private Injector localInjector;
        private ICompilateurInstance cachedComp;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public UseMOP()
        {
            this.Language = "";
            this.command = "";
            this.expression = "";
            this.localInjector = null;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the language name
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
        /// Gets the injector string
        /// </summary>
        public string Injector
        {
            get
            {
                string name = null;
                List<string> skeletonPath = null;
                string modifier = null;
                if (this.ParseCommand(this.command, out name, out skeletonPath, out modifier))
                {
                    string insert = String.Empty;
                    bool first = true;
                    foreach (string path in skeletonPath)
                    {
                        if (!first) insert += "/"; else first = false;
                        insert += path;
                    }
                    return insert;
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets or sets the command string
        /// </summary>
        public string Command
        {
            get
            {
                return this.command;
            }
            set
            {
                this.command = value;
            }
        }

        /// <summary>
        /// Gets the expression parameter list
        /// </summary>
        public string Expression
        {
            get { return this.expression; }
        }


        /// <summary>
        /// Says if this mop is intended to inject at the place of an injector
        /// </summary>
        public bool HasInjector
        {
            get
            {
                string name = null;
                List<string> skeletonPath = null;
                string modifier = null;
                if (this.ParseCommand(this.command, out name, out skeletonPath, out modifier))
                {
                    return !(skeletonPath.Count == 0 || String.IsNullOrEmpty(modifier));
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parses the command string and gets separated results
        /// </summary>
        /// <param name="command">the command string</param>
        /// <param name="name">the resolved mop name</param>
        /// <param name="path">the resolved path</param>
        /// <param name="modifier">the resolved modifier switch</param>
        /// <returns></returns>
        public bool ParseCommand(string command, out string name, out List<string> path, out string modifier)
        {
            Regex reg = new Regex(@"^([^@]+)([@](/?(([^/]+)/)*([^/]+))\{(after|before|replace)\})?$", RegexOptions.IgnoreCase);
            Match m = reg.Match(command);
            if (m.Success)
            {
                name = m.Groups[1].Value;
                string file = m.Groups[3].Value;
                path = new List<string>();
                string[] tab = file.Split('/');
                foreach (string p in tab)
                {
                    if (!String.IsNullOrEmpty(p))
                        path.Add(p);
                }
                modifier = m.Groups[7].Value.ToLower();
                return true;
            }
            else
            {
                name = "";
                path = new List<string>();
                modifier = "";
                return false;
            }
        }
        #endregion

        #region Private Methods
        private string MOPToFunctionName(string mopName)
        {
            string value = mopName;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                int position = -1;
                if ((position = value.IndexOf(c)) != -1)
                    value = value.Substring(0, position) + "_" + value.Substring(position + 1);
            }
            return value;
        }

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
            get { return "UseMOP"; }
        }

        /// <summary>
        /// Extracts dictionary keys
        /// </summary>
        /// <param name="proc">current process</param>
        public void ExtractDictionary(IProcessInstance proc)
        {
            string name = null;
            List<string> skeletonPath = null;
            string modifier = null;
            if (this.ParseCommand(this.command, out name, out skeletonPath, out modifier))
            {
                // rechercher le MOP source
                CreateMOP mopSrc = this.cachedComp.GetMOP(this.language, name);
                // évaluation de l'expression (liste)
                string[] tab;
                if (!String.IsNullOrEmpty(this.expression))
                {
                    o2Mate.Expression expr = new o2Mate.Expression();
                    IData result = expr.Evaluate("array(" + this.expression + ")", proc.CurrentScope);
                    tab = result.ValueString.Split(',');
                }
                else
                {
                    tab = new string[0];
                }
                // remplacement des valeurs
                int index = 0;
                Dictionary<string, string> values = new Dictionary<string, string>();
                while (index < mopSrc.References.Count)
                {
                    if (index < tab.Length)
                    {
                        if (!String.IsNullOrEmpty(tab[index]))
                            values.Add(mopSrc.References[index], tab[index]);
                        else
                            values.Add(mopSrc.References[index], "");
                    }
                    ++index;
                }
                if (index < tab.Length && mopSrc.References.Count > 0)
                {
                    values[mopSrc.References[mopSrc.References.Count - 1]] += String.Join(",", tab, index, tab.Length - index);
                }
                string code = mopSrc.ReplaceParams(values);
                // charge dans le compilateur l'extrait de code du mop
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.LoadXml("<code>" + code + "</code>");
                if (skeletonPath.Count == 0 || String.IsNullOrEmpty(modifier))
                {
                    if (this.localInjector != null)
                    {
                        // on recherche le début de l'injection, i.e. UseMOP
                        int current = proc.Objects.IndexOf(this);
                        // on supprime l'injection précédente à partir du point de départ
                        proc.Remove(this.localInjector.Current + 1, this.localInjector.After);
                        proc.Remove(this.localInjector.Before, this.localInjector.Current - 1);
                        this.localInjector.Current = this.localInjector.Before = current;
                        this.localInjector.After = this.localInjector.Before + 1;
                    }
                    else
                    {
                        this.localInjector = new Injector();
                        this.localInjector.Current = this.localInjector.Before = proc.Objects.IndexOf(this);
                        this.localInjector.After = this.localInjector.Before + 1;
                        this.localInjector.InjectedProcess = proc;
                    }
                    this.cachedComp.Injection(this.localInjector, doc.DocumentElement, "after");
                }
                else
                {
                    string insert = String.Empty;
                    bool first = true;
                    foreach (string path in skeletonPath)
                    {
                        if (!first) insert += "/"; else first = false;
                        insert += path;
                    }
                    Injector j = this.cachedComp.GetInjector(insert);
                    this.cachedComp.Injection(j, doc.DocumentElement, modifier);
                }
            }
            else
            {
                throw new FormatException("Format de la commande '" + this.command + "' inattendue");
            }
        }

        /// <summary>
        /// Injects a use mop
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
                if (this.language.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.language);
                        return false;
                    }
                    this.language = this.language.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                if (this.command.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.command);
                        return false;
                    }
                    this.command = this.command.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                if (this.expression.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    this.expression = this.expression.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        /// <summary>
        /// Load statement data
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node object</param>
        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.language = node.Attributes.GetNamedItem("language").Value;
            this.command = node.Attributes.GetNamedItem("command").Value;
            this.expression = node.Attributes.GetNamedItem("expression").Value;
            this.cachedComp = comp;
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
            list.Add(new DisplayElement(this.TypeName, "displayUseMOP", new object[] { this.language, this.command, this.expression, this.indent.ToString(), false }));
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
            list.Add(new DisplayElement(this.TypeName, "displayUseMOP", new object[] { this.language, this.command, this.expression, this.indent.ToString(), true }));
        }

        /// <summary>
        /// Updates data from the web browser (before save)
        /// </summary>
        /// <param name="elem">html element</param>
        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.language = this.GetElementByName(elem, "language").InnerText;
            if (String.IsNullOrEmpty(this.language)) this.language = "";
            this.command = this.GetElementByName(elem, "command").InnerText;
            this.expression = this.GetElementByName(elem, "expression").InnerText;
        }

        /// <summary>
        /// Save this statement into an xml file
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="writer">xml writer object</param>
        /// <param name="child">html child to save</param>
        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("usemop");
            writer.WriteAttributeString("language", this.language);
            writer.WriteAttributeString("command", this.command);
            writer.WriteAttributeString("expression", this.expression);
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
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
            this.language = node.Attributes.GetNamedItem("language").Value;
            this.command = node.Attributes.GetNamedItem("command").Value;
            this.expression = node.Attributes.GetNamedItem("expression").Value;
        }

        /// <summary>
        /// Process this statement and writes results in a file
        /// </summary>
        /// <param name="proc">process object</param>
        /// <param name="file">final file object</param>
        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "use mop", this.Language, this.Command));
            string name = null;
            List<string> skeletonPath = null;
            string modifier = null;
            if (this.ParseCommand(this.command, out name, out skeletonPath, out modifier))
            {
                // rechercher le MOP source
                CreateMOP mopSrc = this.cachedComp.GetMOP(this.language, name);
                // évaluation de l'expression (liste)
                string[] tab;
                if (!String.IsNullOrEmpty(this.expression))
                {
                    o2Mate.Expression expr = new o2Mate.Expression();
                    IData result = expr.Evaluate("array(" + this.expression + ")", proc.CurrentScope);
                    tab = result.ValueString.Split(',');
                }
                else
                {
                    tab = new string[0];
                }
                // remplacement des valeurs
                int index = 0;
                Dictionary<string, string> values = new Dictionary<string, string>();
                while (index < mopSrc.References.Count)
                {
                    if (index < tab.Length)
                    {
                        values.Add(mopSrc.References[index], tab[index]);
                    }
                    ++index;
                }
                if (index < tab.Length && mopSrc.References.Count > 0)
                {
                    values[mopSrc.References[mopSrc.References.Count - 1]] += String.Join(",", tab, index, tab.Length - index);
                }
                string code = mopSrc.ReplaceParams(values);
                // charge dans le compilateur l'extrait de code du mop
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.LoadXml("<code>" + code + "</code>");
                if (skeletonPath.Count == 0 || String.IsNullOrEmpty(modifier))
                {
                    if (this.localInjector != null)
                    {
                        // on recherche le début de l'injection
                        this.localInjector.InjectedProcess.Remove(this.localInjector.Current, this.localInjector.After - 1);
                        this.localInjector.InjectedProcess.Remove(this.localInjector.Before, this.localInjector.Current - 1);
                        this.localInjector.Current = this.localInjector.Before;
                        this.localInjector.After = this.localInjector.Before + 1;
                    }
                    else
                    {
                        this.localInjector = new Injector();
                        this.localInjector.Current = this.localInjector.Before = proc.Objects.IndexOf(this);
                        this.localInjector.After = this.localInjector.Before + 1;
                        this.localInjector.InjectedProcess = proc;
                    }
                    this.cachedComp.Injection(this.localInjector, doc.DocumentElement, "after");
                }
                else
                {
                    string insert = String.Empty;
                    bool first = true;
                    foreach (string path in skeletonPath)
                    {
                        if (!first) insert += "/"; else first = false;
                        insert += path;
                    }
                    Injector j = this.cachedComp.GetInjector(insert);
                    if (modifier == "replace")
                    {
                        // on recherche le début de l'injection
                        j.InjectedProcess.Remove(j.Current, j.After - 1);
                        j.InjectedProcess.Remove(j.Before, j.Current - 1);
                        j.Current = j.Before;
                        j.After = j.Before + 1;
                    }
                    this.cachedComp.Injection(j, doc.DocumentElement, modifier);
                }
            }
            else
            {
                throw new FormatException("Format de la commande '" + this.command + "' inattendue");
            }
        }

        /// <summary>
        /// Converts this statement for a particular programming language destination
        /// </summary>
        /// <param name="converter">converter object</param>
        /// <param name="proc">process object</param>
        /// <param name="file">final file</param>
        public void Convert(Converters.ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            string name = null;
            List<string> skeletonPath = null;
            string modifier = null;
            if (this.ParseCommand(this.command, out name, out skeletonPath, out modifier))
            {
                // rechercher le MOP source
                CreateMOP mopSrc = this.cachedComp.GetMOP(this.language, name);

                // charge l'extrait de code du mop
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.LoadXml("<code>" + mopSrc.XmlCode + "</code>");

                if (skeletonPath.Count == 0 || String.IsNullOrEmpty(modifier))
                {
                    // cas lorsque l'on insère dans le processus en cours

                    // fabrication d'un processus
                    Process subProcess = new Process();
                    subProcess.Name = this.MOPToFunctionName(mopSrc.Language + "_" + mopSrc.Name);

                    // ajouter les affectations pour chaque paramètre
                    string[] tab;
                    if (!String.IsNullOrEmpty(this.expression))
                    {
                        tab = this.expression.Split(',');
                    }
                    else
                    {
                        tab = new string[0];
                    }
                    int index = 0;
                    Dictionary<string, string> values = new Dictionary<string, string>();
                    while (index < mopSrc.References.Count)
                    {
                        if (index < tab.Length)
                        {
                            values.Add(mopSrc.References[index], tab[index]);
                        }
                        ++index;
                    }
                    if (index < tab.Length && mopSrc.References.Count > 0)
                    {
                        values[mopSrc.References[mopSrc.References.Count - 1]] += "array(" + String.Join(",", tab, index, tab.Length - index) + ")";
                    }
                    foreach (KeyValuePair<string, string> kv in values)
                    {
                        Converters.Parameter param = new Parameter();
                        param.FormalParameter = kv.Key.Trim();
                        param.ReplacementParameter = kv.Value.Trim();
                        o2Mate.Expression expr = new o2Mate.Expression();
                        o2Mate.IData res = expr.Evaluate(kv.Value.Trim(), proc.CurrentScope);
                        param.EffectiveParameter = res.ValueString;
                        param.IsComputable = res.IsComputable;
                        subProcess.Replacements.Add(param);
                    }

                    if (this.localInjector != null)
                    {
                        // on recherche le début de l'injection
                        this.localInjector.InjectedProcess.Remove(this.localInjector.Current, this.localInjector.After - 1);
                        this.localInjector.InjectedProcess.Remove(this.localInjector.Before, this.localInjector.Current - 1);
                        this.localInjector.Current = this.localInjector.Before;
                        this.localInjector.After = this.localInjector.Before + 1;
                    }
                    else
                    {
                        this.localInjector = new Injector();
                        this.localInjector.Current = this.localInjector.Before = proc.Objects.IndexOf(this);
                        this.localInjector.After = this.localInjector.Before + 1;
                        this.localInjector.InjectedProcess = proc;
                    }
                    this.cachedComp.Injection(this.localInjector, doc.DocumentElement, "after");

                    // on implémente une fonction avec ce processus
                    Call c = new Call(this.cachedComp, proc);
                    c.ProcessName = subProcess.Name;
                    c.Convert(converter, proc, file);
                }
                else
                {
                    // cas d'utilisation d'un inserteur

                    string insert = String.Empty;
                    bool first = true;
                    foreach (string path in skeletonPath)
                    {
                        if (!first) insert += "/"; else first = false;
                        insert += path;
                    }
                    Injector j = this.cachedComp.GetInjector(insert);

                    // lorsque l'injecteur a déjà été utilisé
                    if (!j.IsEmpty && modifier == "replace")
                    {
                        // on efface tout
                        // on recherche le début de l'injection
                        j.InjectedProcess.Remove(j.Current, j.After - 1);
                        j.InjectedProcess.Remove(j.Before, j.Current - 1);
                        j.Current = j.Before;
                        j.After = j.Before + 1;
                    }

                    // ajouter les affectations pour chaque paramètre
                    string[] tab;
                    if (!String.IsNullOrEmpty(this.expression))
                    {
                        tab = this.expression.Split(',');
                    }
                    else
                    {
                        tab = new string[0];
                    }
                    int index = 0;
                    Dictionary<string, string> values = new Dictionary<string, string>();
                    while (index < mopSrc.References.Count)
                    {
                        if (index < tab.Length)
                        {
                            values.Add(mopSrc.References[index], tab[index]);
                        }
                        ++index;
                    }
                    if (index < tab.Length && mopSrc.References.Count > 0)
                    {
                        values[mopSrc.References[mopSrc.References.Count - 1]] += "array(" + String.Join(",", tab, index, tab.Length - index) + ")";
                    }

                    j.InjectedProcess.Replacements.Clear();
                    foreach (KeyValuePair<string,string> kv in values)
                    {
                        Converters.Parameter param = new Parameter();
                        param.FormalParameter = kv.Key.Trim();
                        param.ReplacementParameter = kv.Value.Trim();
                        o2Mate.Expression expr = new o2Mate.Expression();
                        o2Mate.IData res = expr.Evaluate(kv.Value.Trim(), proc.CurrentScope);
                        param.EffectiveParameter = res.ValueString;
                        param.IsComputable = res.IsComputable;
                        j.InjectedProcess.Replacements.Add(param);
                    }

                    this.cachedComp.Injection(j, doc.DocumentElement, modifier);
                    // le processus a été modifié
                    j.InjectedProcess.HasChanged = true;
                }
            }
            else
            {
                throw new FormatException("Format de la commande '" + this.command + "' inattendue");
            }
        }

        #endregion

        #region ILanguageConverter Membres

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
        }

        /// <summary>
        /// Write in VB Script
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInVBScript(ICodeConverter converter)
        {
        }

        /// <summary>
        /// Write in Microsoft PowerShell
        /// </summary>
        /// <param name="converter">converter object</param>
        public void WriteInPowerShell(ICodeConverter converter)
        {
        }

        #endregion
    }
}
