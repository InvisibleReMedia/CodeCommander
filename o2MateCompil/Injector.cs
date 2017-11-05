using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    /// <summary>
    /// An injector class
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class Injector : ICompiler, ICloneable
    {
        #region Private Fields
        private string skeleton;
        private string name;
        private int indent;
        private int current, before, after;
        IProcess injectedProcess;
        private Stack<IProcess> processStack;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Injector()
        {
            this.skeleton = "";
            this.name = "";
            this.indent = 0;
            this.current = this.before = this.after = 0;
            this.injectedProcess = null;
            this.processStack = new Stack<IProcess>();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the name of the injector
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets the skeleton path (not null if this injector resided in a skeleton)
        /// </summary>
        public string Skeleton
        {
            get { return this.skeleton; }
        }

        /// <summary>
        /// Gets the long name of this injector
        /// </summary>
        public string LongName
        {
            get { return this.skeleton + this.name; }
        }

        /// <summary>
        /// Gets or sets the process in which this injector resides
        /// </summary>
        public IProcess InjectedProcess
        {
            get { return this.injectedProcess; }
            set { this.injectedProcess = value; }
        }

        /// <summary>
        /// Gets the process stack
        /// </summary>
        internal Stack<IProcess> StackProcesses
        {
            get
            {
                return this.processStack;
            }
        }

        /// <summary>
        /// Gets or sets the position of the first element of this injector
        /// </summary>
        public int Before
        {
            get { return this.before; }
            set { this.before = value; }
        }

        /// <summary>
        /// Gets or sets the position of the latest element of this injector
        /// </summary>
        public int After
        {
            get { return this.after; }
            set { this.after = value; }
        }

        /// <summary>
        /// Gets or sets the position of the injector statement
        /// </summary>
        public int Current
        {
            get { return this.current; }
            set { this.current = value; }
        }

        /// <summary>
        /// Returns true if the injector has no statement
        /// </summary>
        public bool IsEmpty
        {
            get { return this.StackProcesses.Count == 0 && this.current == this.after && this.current == this.before; }
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

        #region Public Methods
        /// <summary>
        /// Inject a statement
        /// </summary>
        /// <param name="proc">the process where to add the statement</param>
        /// <param name="obj">the statement to add</param>
        /// <param name="modifier">modifier switch (after, before or replace)</param>
        public void InjectObject(IProcess proc, ICompiler obj, string modifier)
        {
            // injecter dans le processus initial mais pas dans les nouveaux processus
            if (this.StackProcesses.Count == 0)
            {
                if (modifier == "after" || modifier == "replace")
                {
                    if (this.after < proc.Objects.Count)
                    {
                        proc.InsertObject(this.after, obj);
                        ++this.after;
                    }
                    else
                    {
                        proc.AddObject(obj);
                        ++this.after;
                    }
                }
                else if (modifier == "before")
                {
                    proc.InsertObject(this.current, obj);
                    ++this.current;
                    // les objets ajoutés en before déplacent les positions inférieures
                    ++this.after;
                }
            }
            else
            {
                // on n'est plus dans le processus injecté mais dans une subroutine
                proc.AddObject(obj);
            }
        }
        #endregion

        #region Compiler Membres

        /// <summary>
        /// Gets the type name of an instance of this class
        /// </summary>
        public string TypeName
        {
            get { return "Injector"; }
        }

        /// <summary>
        /// Extracts dictionary keys
        /// </summary>
        /// <param name="proc">current process</param>
        public void ExtractDictionary(IProcessInstance proc)
        {
        }

        /// <summary>
        /// Injects an injector
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
        /// Load injector data
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node object</param>
        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.name = node.Attributes.GetNamedItem("name").Value;
            this.injectedProcess = comp.CurrentProcess;
            this.current = this.injectedProcess.Objects.IndexOf(this);
            this.before = this.after = this.current;
            // si je suis dans un squelette
            if (this.injectedProcess.Name.StartsWith("Skeleton:"))
            {
                this.skeleton = this.injectedProcess.Name.Substring(9) + "/";
            }
            else
            {
                this.skeleton = "";
            }
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
            list.Add(new DisplayElement(this.TypeName, "displayInjector", new object[] { this.name, this.indent.ToString(), false }));
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
            list.Add(new DisplayElement(this.TypeName, "displayInjector", new object[] { this.name, this.indent.ToString(), true }));
        }

        /// <summary>
        /// Updates data from the web browser (before save)
        /// </summary>
        /// <param name="elem">html element</param>
        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.name = this.GetElementByName(elem, "name").InnerText;
        }

        /// <summary>
        /// Save the statement into an xml file
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="writer">xml writer object</param>
        /// <param name="child">html child to save</param>
        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("injector");
            writer.WriteAttributeString("name", this.name);
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
            this.name = node.Attributes.GetNamedItem("name").Value;
        }

        /// <summary>
        /// Process the statement and writes results in a file
        /// </summary>
        /// <param name="proc">process object</param>
        /// <param name="file">final file object</param>
        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "injector", this.Name));
        }

        /// <summary>
        /// Converts this statement for a particular programming language destination
        /// </summary>
        /// <param name="converter">converter object</param>
        /// <param name="proc">process object</param>
        /// <param name="file">final file</param>
        public void Convert(Converters.ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
        }

        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>injector object</returns>
        public object Clone()
        {
            Injector i = new Injector();
            i.skeleton = this.skeleton;
            i.name = this.name;
            i.indent = this.indent;
            i.current = this.current;
            i.before = this.before;
            i.after = this.after;
            i.injectedProcess = this.injectedProcess;
            i.processStack = new Stack<IProcess>(this.processStack);
            return i;
        }
        #endregion
    }
}
