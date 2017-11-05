using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.IO;
using Converters;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace o2Mate
{
    /// <summary>
    /// Delegate function
    /// </summary>
    /// <param name="comp">compiler instance object</param>
    public delegate void RunInnerInstanceCompiler(ICompilateurInstance comp);

    /// <summary>
    /// Interface of the compiler
    /// </summary>
    [CoClass(typeof(ICompilateur))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ICompilateur : ICloneable
    {
        /// <summary>
        /// Gets the unique system creation of a name
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        UniqueStrings Unique { get; }
        /// <summary>
        /// Gets summary
        /// </summary>
        ILegendeDict Legendes { get; }
        /// <summary>
        /// Gets or sets the programming language in conversion
        /// </summary>
        ICodeConverter ConvertedLanguage { get; set; }
        /// <summary>
        /// Gets or sets encoding file
        /// </summary>
        Encoding EncodedFile { get; set; }
        /// <summary>
        /// Gets or sets if a conversion is being processed
        /// </summary>
        bool UnderConversion { get; set; }
        /// <summary>
        /// Returns all processes (all types included)
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        List<IProcess> Processes { get; }
        /// <summary>
        /// Returns all known syntax
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        List<Syntax> Syntaxes { get; }
        /// <summary>
        /// Returns all known MOPs
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        List<CreateMOP> Mops { get; }
        /// <summary>
        /// Returns all loaded projects
        /// </summary>
        Projects Threads { get; }
        /// <summary>
        /// Returns all known MOPs
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        List<Injector> Injectors { get; }
        /// <summary>
        /// Load all files in a directory (assumes to contain templates)
        /// </summary>
        /// <param name="directory">directory path</param>
        void LoadTemplates(string directory);
        /// <summary>
        /// Load all files in a directory (assumes to contain skeletons)
        /// </summary>
        /// <param name="directory">directory path</param>
        void LoadSkeletons(string directory);
        /// <summary>
        /// Load all files in a directory (assumes to contain syntax)
        /// </summary>
        /// <param name="directory">directory path</param>
        void LoadSyntax(string directory);
        /// <summary>
        /// Load all files in a directory (assumes to contain MOPs)
        /// </summary>
        /// <param name="directory">directory path</param>
        void LoadMOP(string directory);
        /// <summary>
        /// Displays each statements stored in a string into the web navigator
        /// </summary>
        /// <param name="xmlString">xml string to display</param>
        /// <param name="list">list to store operation</param>
        /// <param name="forcedIndent">true if force a number at indentation</param>
        /// <param name="indent">number at indentation</param>
        /// <param name="writable">displays objects editable or readonly</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void DisplayXML(string xmlString, BindingList<DisplayElement> list, bool forcedIndent, int indent, bool writable, INotifyProgress wait);
        /// <summary>
        /// Displays a complete statements stored in a file into the web navigator
        /// </summary>
        /// <param name="fileSource">CodeCommander file name to display</param>
        /// <param name="list">list to store operation</param>
        /// <param name="writable">displays objects editable or readonly</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void Display(string fileSource, BindingList<DisplayElement> list, bool writable, INotifyProgress wait);
        /// <summary>
        /// The parse function is just like a limited read (to improve performance) - not a complete loading
        /// To call just before the display function
        /// </summary>
        /// <param name="comp">the compiler object</param>
        /// <param name="node">the xml node to read</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void Parse(ICompilateur comp, System.Xml.XmlNode node);
        /// <summary>
        /// Gets skeleton list
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        List<Skeleton> Skeletons { get; }
        /// <summary>
        /// Returns all known templates
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        List<Template> Templates { get; }
        /// <summary>
        /// Objects list (this is a tree for embedded objects)
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        Tree<ICompiler> Objects { get; }
        /// <summary>
        /// Gets or sets the current process
        /// </summary>
        IProcess CurrentProcess { get; set; }
        /// <summary>
        /// Returns the stack of loaded processes
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        Stack<IProcess> ProcessStack { get; }
        /// <summary>
        /// Says if it's a non recursive call
        /// </summary>
        /// <param name="processName">name of the process</param>
        /// <returns>true if is not a recursively call</returns>
        bool CheckNonRecursiveMode(string processName);
        /// <summary>
        /// Returns a process object identified by a name and a type 'coding'
        /// </summary>
        /// <param name="name">name of the process to find</param>
        /// <returns>process found or throw an exception</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        IProcess GetCodingProcess(string name);
        /// <summary>
        /// Returns a process object identified by a name and a type 'parallel'
        /// </summary>
        /// <param name="name">name of the process to find</param>
        /// <returns>process found or throw an exception</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        IProcess GetParallelProcess(string name);
        /// <summary>
        /// Returns a process object identified by a name and a type 'job'
        /// </summary>
        /// <param name="name">name of the process to find</param>
        /// <returns>process found or throw an exception</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        IProcess GetJobProcess(string name);
        /// <summary>
        /// Search for a registered skeleton
        /// </summary>
        /// <param name="name">name of the skeleton to find</param>
        /// <returns>skeleton found or throw an exception</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        Skeleton GetSkeleton(string name);
        /// <summary>
        /// Find a syntax by a name
        /// </summary>
        /// <param name="name">name of the syntax</param>
        /// <returns>syntax object or throw an exception</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        Syntax GetSyntax(string name);
        /// <summary>
        /// Remove a process identified by a name and a type 'coding'
        /// </summary>
        /// <param name="name">name of the process to remove</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void RemoveCodingProcess(string name);
        /// <summary>
        /// Returns a user process object identified by a name
        /// A user process has a colon on the first character name
        /// </summary>
        /// <param name="name">name of the process to find</param>
        /// <returns>process found or throw an exception</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        IProcess GetProcess(string name);
        /// <summary>
        /// Gets or sets the current using template
        /// </summary>
        UseTemplate CurrentUsingTemplate { get; set; }
        /// <summary>
        /// Assumes starting a template and do the good work
        /// </summary>
        /// <param name="curTemp">template object</param>
        void StartUsingTemplate(UseTemplate curTemp);
        /// <summary>
        /// Finalize a template
        /// </summary>
        /// <param name="curTemp">template object</param>
        void EndUsingTemplate(UseTemplate curTemp);
        /// <summary>
        /// Add a new process in the process list
        /// Pass a project interface reference and cast to instance of the implemented process class
        /// </summary>
        /// <param name="process">process object</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void AddProcess(IProcess process);
        /// <summary>
        /// Push a process on a stack
        /// It's required to loaded process
        /// </summary>
        /// <param name="process">process object</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void PushProcess(IProcess process);
        /// <summary>
        /// Pop a process on the top of the stack
        /// It's required to quit loaded process
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        void PopProcess();
        /// <summary>
        /// Function to construct a complete dictionary object
        /// by the CodeCommander file source
        /// </summary>
        /// <param name="fileSource">file name source</param>
        /// <returns>a dictionary object</returns>
        Dictionnaire OutputDictionary(string fileSource);
        /// <summary>
        /// Function to construct a list of summary object
        /// by the CodeCommander file source
        /// </summary>
        /// <param name="fileSource">file name source</param>
        /// <returns>a summary object</returns>
        ILegendeDict OutputLegendes(string fileSource);
        /// <summary>
        /// Summary has been modified from the web navigator
        /// This method keeps the summary in internal
        /// </summary>
        /// <param name="elem">html element</param>
        void InputLegendes(System.Windows.Forms.HtmlElement elem);
        /// <summary>
        /// The method compiler
        /// </summary>
        /// <param name="fileSource">CodeCommander file source</param>
        /// <param name="fileData">dictionary file</param>
        /// <param name="finalFile">final file</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        void Compilation(string fileSource, string fileData, string finalFile, INotifyProgress wait);
        /// <summary>
        /// The method compiler (use this method when the CodeCommander file source has been loaded either)
        /// </summary>
        /// <param name="fileData">dictionary file</param>
        /// <param name="finalFile">final file</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        void Compilation(string fileData, string finalFile, INotifyProgress wait);
        /// <summary>
        /// The defaut method compiler
        /// </summary>
        /// <param name="fileSource">CodeCommander file source</param>
        /// <param name="fileData">dictionary file</param>
        /// <param name="finalFile">final file</param>
        /// <param name="destEncoding">final file encoding</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        void Compilation(string fileSource, string fileData, string finalFile, string destEncoding, INotifyProgress wait);
        /// <summary>
        /// The method for debugging
        /// </summary>
        /// <param name="fileData">dictionary file</param>
        /// <param name="finalFile">final file</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        void Debug(string fileData, string finalFile, INotifyProgress wait);
        /// <summary>
        /// The method for debugging
        /// </summary>
        /// <param name="fileSource">CodeCommander file source</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        void Debug(string fileSource, INotifyProgress wait);
        /// <summary>
        /// Get header from a CodeCommander file source
        /// </summary>
        /// <param name="fileSource">CodeCommander file source</param>
        /// <param name="creationDate">returns the creation date</param>
        /// <param name="modificationDate">returns the modification date</param>
        /// <param name="revision">returns the revision date</param>
        void GetHeader(string fileSource, out string creationDate, out string modificationDate, out string revision);
        /// <summary>
        /// Converts a CodeCommander source file into a programming language
        /// </summary>
        /// <param name="converter">a converter object</param>
        /// <param name="fileSource">CodeCommander file source</param>
        /// <param name="fileData">dictionary file data</param>
        /// <param name="finalFile">final file (used to set the path output onto the converted source code)</param>
        void Convert(ICodeConverter converter, string fileSource, string fileData, string finalFile);
        /// <summary>
        /// Load an xml node
        /// </summary>
        /// <param name="comp">this compiler</param>
        /// <param name="node">xml node</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void Load(ICompilateurInstance comp, System.Xml.XmlNode node);
        /// <summary>
        /// Save objects from the web navigator
        /// </summary>
        /// <param name="elem">html element</param>
        /// <returns>the xml string</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        string SaveToString(System.Windows.Forms.HtmlElement elem);
        /// <summary>
        /// Save child objects from the web navigator
        /// </summary>
        /// <param name="elem">html element</param>
        /// <returns>the xml string</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        string SaveChildToString(System.Windows.Forms.HtmlElement elem);
        /// <summary>
        /// Save objects from the web navigator
        /// </summary>
        /// <param name="fileDestination">file where to write</param>
        /// <param name="elem">html element</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        /// <param name="infos">header infos</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void Save(string fileDestination, System.Windows.Forms.HtmlElement elem, INotifyProgress wait, params string[] infos);
        /// <summary>
        /// Save childs objects to an xml writer
        /// </summary>
        /// <param name="writer">xml writer object</param>
        /// <param name="elem">last html element to continue</param>
        /// <param name="currentIndent">current indentation</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void Save(XmlWriter writer, ref System.Windows.Forms.HtmlElement elem, int currentIndent);
        /// <summary>
        /// Save xml string into a CodeCommander file source (an xml file)
        /// </summary>
        /// <param name="fileDestination">destination file name</param>
        /// <param name="xmlCode">xml string to persist</param>
        /// <param name="legendes">summary data</param>
        void Save(string fileDestination, string xmlCode, ILegendeDict legendes);
        /// <summary>
        /// Find a template by a name
        /// </summary>
        /// <param name="name">template name to find</param>
        /// <returns>template object else throw an exception</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        Template GetTemplate(string name);
        /// <summary>
        /// Find a mop by a language and a name
        /// </summary>
        /// <param name="language">language</param>
        /// <param name="name">name of the mop</param>
        /// <returns>mop object or throw an exception</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        CreateMOP GetMOP(string language, string name);
        /// <summary>
        /// Get an already registered injector
        /// </summary>
        /// <param name="map">the name of the injector (can be in and out a skeleton)</param>
        /// <returns>injector object or throw an exception</returns>
        [System.Runtime.InteropServices.ComVisible(false)]
        Injector GetInjector(string map);
        /// <summary>
        /// Create a CodeCommander file source
        /// (overwrite if exists)
        /// </summary>
        /// <param name="fileName">file name to create</param>
        void CreateFile(string fileName);
        /// <summary>
        /// Method to update parameters of a function
        /// Make a backtrack on parent process to assume mutable parameters
        /// </summary>
        /// <param name="converter">language converter</param>
        /// <param name="proc">current process</param>
        /// <param name="varName">variable name to check</param>
        /// <param name="mutable">mutable switch</param>
        void UpdateParameters(ICodeConverter converter, IProcessInstance proc, string varName, bool mutable);
        /// <summary>
        /// Make a new instance of the compiler with internal values
        /// </summary>
        /// <param name="previous">previous instance</param>
        /// <param name="run">delegate</param>
        /// <returns>instance number</returns>
        int MakeNewInstance(ICompilateurInstance previous, RunInnerInstanceCompiler run);
    }

    /// <summary>
    /// Instance compiler interface
    /// </summary>
    public interface ICompilateurInstance : ICompilateur
    {
        /// <summary>
        /// Gets or sets the current dictionary
        /// </summary>
        o2Mate.Dictionnaire CurrentDictionnaire { get; set; }
        /// <summary>
        /// Gets or sets the current scope
        /// </summary>
        o2Mate.Scope CurrentScope { get; set; }
        /// <summary>
        /// Gets the current executed process
        /// </summary>
        IProcessInstance CurrentExecutedProcess { get; }
        /// <summary>
        /// Returns the stack of called instance processes
        /// </summary>
        [System.Runtime.InteropServices.ComVisible(false)]
        Stack<IProcessInstance> CalledProcesses { get; }
        /// <summary>
        /// Converts a template to a programming language destination
        /// </summary>
        /// <param name="converter">a converter object</param>
        /// <param name="templateName">template name</param>
        /// <param name="nc">struct contained the callback function as the request template</param>
        /// <param name="file">final file (used to set the path output onto the converted source code)</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void Convert(ICodeConverter converter, string templateName, NameConverter nc, FinalFile file);
        /// <summary>
        /// Converts a process to a programming language destination
        /// </summary>
        /// <param name="converter">a converter object</param>
        /// <param name="proc">current process to convert</param>
        /// <param name="file">final file (used to set the path output onto the converted source code)</param>
        void Convert(ICodeConverter converter, IProcess proc, FinalFile file);
        /// <summary>
        /// Summary persistence
        /// This method keeps the summary in internal
        /// </summary>
        /// <param name="legendes">summary object</param>
        void InputLegendes(ILegendeDict legendes);
        /// <summary>
        /// Internal starting function to execute statements from a next process
        /// Open for a recursive process
        /// </summary>
        /// <param name="proc">a process instance to execute</param>
        /// <param name="file">final file</param>
        void WriteToFile(IProcess proc, FinalFile file);
        /// <summary>
        /// Extracts an develop the dictionary based on the source code
        /// </summary>
        /// <param name="proc">process instance object</param>
        void ExtractDictionary(IProcess proc);
        /// <summary>
        /// The injection allows to inject code into a process
        /// </summary>
        /// <param name="injector">injector object</param>
        /// <param name="node">xml node</param>
        /// <param name="modifier">modifier after, before or replace</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void Injection(Injector injector, System.Xml.XmlNode node, string modifier);
    }

    /// <summary>
    /// Compiler of CodeCommander files (an xml format)
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE53")]
    public class Compilateur : ICompilateur
    {
        #region Private Delegate
        private delegate void OpenHeaderDelegate(BindingList<DisplayElement> list, string fileSource, string creationDate, string modificationDate, string revision);
        private delegate void DisplayDelegate(Node<ICompiler> node, BindingList<DisplayElement> list, bool writable);
        private delegate void DisplayEndDelegate(BindingList<DisplayElement> list);
        private delegate void DisplayXmlDelegate(Node<ICompiler> node, BindingList<DisplayElement> list, bool forcedIndent, int indent, bool writable);
        private delegate void DisplayObjectsDelegate(Node<ICompiler> objects, BindingList<DisplayElement> list, bool writable);
        #endregion

        #region Public Static Properties
        /// <summary>
        /// the only char prefix for a parameter identifier
        /// </summary>
        public static readonly char ParamIdentifier = '$';
        /// <summary>
        /// First process name
        /// </summary>
        public static readonly string RootProcessName = "principal";
        /// <summary>
        /// First writer name
        /// </summary>
        public static readonly string InitialWriter = "defaultWriter";
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Notify a progress bar to go to the next step
        /// </summary>
        /// <param name="count">count position</param>
        /// <param name="max">entire work numberous items</param>
        public static void NotifyProgress(int count, int max)
        {
            if (Compilateur.progress != null)
            {
                Compilateur.progress.SetCount(max);
                Compilateur.progress.Step(count);
            }
        }

        /// <summary>
        /// Set the progress text to display onto the window progress bar
        /// </summary>
        /// <param name="s">the text to display</param>
        public static void SetProgressText(string s)
        {
            if (Compilateur.progress != null)
            {
                Compilateur.progress.SetText(s);
            }
        }
        #endregion

        #region Private Static Methods
        private static void StartProgress(bool cancelable, INotifyProgress progress)
        {
            if (progress != null)
            {
                Compilateur.progress = progress;
                Compilateur.progress.Start(cancelable);
            }
        }
        private static void TerminateProgress(bool noError)
        {
            if (Compilateur.progress != null)
            {
                Compilateur.progress.IsFinished = true;
                Compilateur.progress = null;
            }
        }
        #endregion

        #region Private Static Objects
        private static INotifyProgress progress;
        #endregion

        #region Inner Class
        internal class CompilateurState : ICompilateurInstance
        {
            #region Private Fields
            private IProcessInstance currentExecutedProcess;
            private Stack<IProcessInstance> executedProcessStack;
            private o2Mate.Scope currentScope;
            private o2Mate.Dictionnaire currentDict;
            private ICompilateur staticCompiler;
            private int instanceNumber;
            private Object syncRoot;
            private bool inUse;
            #endregion

            #region Default Constructor
            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="comp"></param>
            /// <param name="number"></param>
            /// <param name="current">current instance</param>
            /// <param name="useImmediate">bloquer cette instance immediatement</param>
            public CompilateurState(ICompilateur comp, int number, ICompilateurInstance current, bool useImmediate)
            {
                this.staticCompiler = comp;
                this.instanceNumber = number;
                if (current != null)
                {
                    this.currentExecutedProcess = current.CurrentExecutedProcess;
                    this.executedProcessStack = new Stack<IProcessInstance>(current.CalledProcesses);
                    this.CurrentScope = current.CurrentScope;
                }
                else
                {
                    this.currentExecutedProcess = null;
                    this.executedProcessStack = new Stack<IProcessInstance>();
                    this.CurrentScope = new Scope();
                }
                this.syncRoot = new Object();
                this.inUse = useImmediate;
            }
            #endregion

            #region Public Methods

            /// <summary>
            /// Test and set lock
            /// </summary>
            /// <returns>true if active</returns>
            public bool TestAndSetLock()
            {
                bool allow = false;
                lock (this.syncRoot)
                {
                    if (!inUse) { inUse = true; allow = true; }
                }
                return allow;
            }

            /// <summary>
            /// This instance is released
            /// </summary>
            public void Unlock()
            {
                this.inUse = false;
            }

            #endregion

            #region ICompilateurInstance Members
            /// <summary>
            /// Returns all projects defined in a CodeCommander file source
            /// </summary>
            public Projects Threads
            {
                get { return this.staticCompiler.Threads; }
            }

            /// <summary>
            /// Gets summary
            /// </summary>
            public ILegendeDict Legendes
            {
                get { return this.staticCompiler.Legendes; }
            }

            /// <summary>
            /// Gets or sets the current scope
            /// </summary>
            public o2Mate.Scope CurrentScope
            {
                get
                {
                    return this.currentScope;
                }
                set
                {
                    this.currentScope = value;
                }
            }

            /// <summary>
            /// Returns the global current dictionary to propagate among each called processes
            /// </summary>
            public o2Mate.Dictionnaire CurrentDictionnaire
            {
                get
                {
                    return this.currentDict;
                }
                set
                {
                    this.currentDict = value;
                }
            }

            /// <summary>
            /// Returns the stack of loaded processes
            /// </summary>
            public Stack<IProcess> ProcessStack
            {
                get
                {
                    return this.staticCompiler.ProcessStack;
                }
            }

            /// <summary>
            /// Make a new instance of the compiler with internal values
            /// </summary>
            /// <param name="previous">previous instance</param>
            /// <param name="run">delegate</param>
            /// <returns>instance number</returns>
            public int MakeNewInstance(ICompilateurInstance previous, RunInnerInstanceCompiler run)
            {
                return this.staticCompiler.MakeNewInstance(previous, run);
            }

            /// <summary>
            /// Method to update parameters of a function
            /// Make a backtrack on parent process to assume mutable parameters
            /// </summary>
            /// <param name="converter">language converter</param>
            /// <param name="proc">current process</param>
            /// <param name="varName">variable name to check</param>
            /// <param name="mutable">mutable switch</param>
            public void UpdateParameters(ICodeConverter converter, IProcessInstance proc, string varName, bool mutable)
            {
                this.staticCompiler.UpdateParameters(converter, proc, varName, mutable);
            }

            /// <summary>
            /// Gets the current executed process
            /// </summary>
            public IProcessInstance CurrentExecutedProcess
            {
                get { return this.currentExecutedProcess; }
            }

            /// <summary>
            /// Returns the stack of called instance processes
            /// </summary>
            public Stack<IProcessInstance> CalledProcesses
            {
                get { return this.executedProcessStack; }
            }

            /// <summary>
            /// Returns the current using template
            /// </summary>
            public UseTemplate CurrentUsingTemplate
            {
                get
                {
                    return this.staticCompiler.CurrentUsingTemplate;
                }
                set
                {
                    this.staticCompiler.CurrentUsingTemplate = value;
                }
            }

            /// <summary>
            /// Assumes starting a template and do the good work
            /// </summary>
            /// <param name="curTemp">template object</param>
            public void StartUsingTemplate(UseTemplate curTemp)
            {
                this.staticCompiler.StartUsingTemplate(curTemp);
            }

            /// <summary>
            /// Finalize a template
            /// </summary>
            /// <param name="curTemp">template object</param>
            public void EndUsingTemplate(UseTemplate curTemp)
            {
                this.staticCompiler.EndUsingTemplate(curTemp);
            }

            /// <summary>
            /// Gets the unique system creation of a name
            /// </summary>
            public UniqueStrings Unique
            {
                get { return this.staticCompiler.Unique; }
            }

            /// <summary>
            /// Gets or sets the programming language in conversion
            /// </summary>
            public ICodeConverter ConvertedLanguage
            {
                get
                {
                    return this.staticCompiler.ConvertedLanguage;
                }
                set
                {
                    this.staticCompiler.ConvertedLanguage = value;
                }
            }

            /// <summary>
            /// Gets or sets encoding file
            /// </summary>
            public Encoding EncodedFile
            {
                get
                {
                    return this.staticCompiler.EncodedFile;
                }
                set
                {
                    this.staticCompiler.EncodedFile = value;
                }
            }

            /// <summary>
            /// Gets or sets if a conversion is being processed
            /// </summary>
            public bool UnderConversion
            {
                get
                {
                    return this.staticCompiler.UnderConversion;
                }
                set
                {
                    this.staticCompiler.UnderConversion = value;
                }
            }

            /// <summary>
            /// Returns all known templates
            /// </summary>
            public List<Template> Templates
            {
                get
                {
                    return this.staticCompiler.Templates;
                }
            }

            /// <summary>
            /// Returns all known skeletons
            /// </summary>
            public List<Skeleton> Skeletons
            {
                get
                {
                    return this.staticCompiler.Skeletons;
                }
            }

            /// <summary>
            /// Returns all known syntax
            /// </summary>
            public List<Syntax> Syntaxes
            {
                get
                {
                    return this.staticCompiler.Syntaxes;
                }
            }

            /// <summary>
            /// Returns all known MOPs
            /// </summary>
            public List<CreateMOP> Mops
            {
                get { return this.staticCompiler.Mops; }
            }

            /// <summary>
            /// Returns all known injectors
            /// </summary>
            public List<Injector> Injectors
            {
                get { return this.staticCompiler.Injectors; }
            }

            /// <summary>
            /// Load all files in a directory (assumes to contain templates)
            /// </summary>
            /// <param name="directory">directory path</param>
            public void LoadTemplates(string directory)
            {
                this.staticCompiler.LoadTemplates(directory);
            }

            /// <summary>
            /// Load all files in a directory (assumes to contain skeletons)
            /// </summary>
            /// <param name="directory">directory path</param>
            public void LoadSkeletons(string directory)
            {
                this.staticCompiler.LoadSkeletons(directory);
            }

            /// <summary>
            /// Load all files in a directory (assumes to contain syntax)
            /// </summary>
            /// <param name="directory">directory path</param>
            public void LoadSyntax(string directory)
            {
                this.staticCompiler.LoadSyntax(directory);
            }

            /// <summary>
            /// Load all files in a directory (assumes to contain MOPs)
            /// </summary>
            /// <param name="directory">directory path</param>
            public void LoadMOP(string directory)
            {
                this.staticCompiler.LoadMOP(directory);
            }

            /// <summary>
            /// Displays each statements stored in a string into the web navigator
            /// </summary>
            /// <param name="xmlString">xml string to display</param>
            /// <param name="list">list to store operation</param>
            /// <param name="forcedIndent">true if force a number at indentation</param>
            /// <param name="indent">number at indentation</param>
            /// <param name="writable">displays objects editable or readonly</param>
            /// <param name="wait">notification object (to display a progress bar)</param>
            public void DisplayXML(string xmlString, BindingList<DisplayElement> list, bool forcedIndent, int indent, bool writable, INotifyProgress wait)
            {
                this.staticCompiler.DisplayXML(xmlString, list, forcedIndent, indent, writable, wait);
            }

            /// <summary>
            /// Displays a complete statements stored in a file into the web navigator
            /// </summary>
            /// <param name="fileSource">CodeCommander file name to display</param>
            /// <param name="list">list to store operation</param>
            /// <param name="writable">displays objects editable or readonly</param>
            /// <param name="wait">notification object (to display a progress bar)</param>
            public void Display(string fileSource, BindingList<DisplayElement> list, bool writable, INotifyProgress wait)
            {
                this.staticCompiler.Display(fileSource, list, writable, wait);
            }

            /// <summary>
            /// The parse function is just like a limited read (to improve performance) - not a complete loading
            /// To call just before the display function
            /// </summary>
            /// <param name="comp">the compiler object</param>
            /// <param name="node">the xml node to read</param>
            public void Parse(ICompilateur comp, XmlNode node)
            {
                this.staticCompiler.Parse(comp, node);
            }

            /// <summary>
            /// Objects list (this is a tree for embedded objects)
            /// </summary>
            public Tree<ICompiler> Objects
            {
                get { return this.staticCompiler.Objects; }
            }

            /// <summary>
            /// Gets the current process
            /// </summary>
            public IProcess CurrentProcess
            {
                get { return this.staticCompiler.CurrentProcess; }
                set { this.staticCompiler.CurrentProcess = value; }
            }

            /// <summary>
            /// Returns all processes (all types included)
            /// </summary>
            public List<IProcess> Processes
            {
                get { return this.staticCompiler.Processes; }
            }
            /// <summary>
            /// Says if it's a non recursive call
            /// </summary>
            /// <param name="processName">name of the process</param>
            /// <returns>true if is not a recursively call</returns>
            public bool CheckNonRecursiveMode(string processName)
            {
                return this.staticCompiler.CheckNonRecursiveMode(processName);
            }

            /// <summary>
            /// Returns a process object identified by a name and a type 'coding'
            /// </summary>
            /// <param name="name">name of the process to find</param>
            /// <returns>process found or throw an exception</returns>
            public IProcess GetCodingProcess(string name)
            {
                return this.staticCompiler.GetCodingProcess(name);
            }

            /// <summary>
            /// Returns a process object identified by a name and a type 'parallel'
            /// </summary>
            /// <param name="name">name of the process to find</param>
            /// <returns>process found or throw an exception</returns>
            public IProcess GetParallelProcess(string name)
            {
                return this.staticCompiler.GetParallelProcess(name);
            }

            /// <summary>
            /// Returns a process object identified by a name and a type 'job'
            /// </summary>
            /// <param name="name">name of the process to find</param>
            /// <returns>process found or throw an exception</returns>
            public IProcess GetJobProcess(string name)
            {
                return this.staticCompiler.GetJobProcess(name);
            }

            /// <summary>
            /// Search for a registered skeleton
            /// </summary>
            /// <param name="name">name of the skeleton to find</param>
            /// <returns>skeleton found or throw an exception</returns>
            public Skeleton GetSkeleton(string name)
            {
                return this.staticCompiler.GetSkeleton(name);
            }

            /// <summary>
            /// Find a syntax by a name
            /// </summary>
            /// <param name="name">name of the syntax</param>
            /// <returns>syntax object or throw an exception</returns>
            public Syntax GetSyntax(string name)
            {
                return this.staticCompiler.GetSyntax(name);
            }

            /// <summary>
            /// Remove a process identified by a name and a type 'coding'
            /// </summary>
            /// <param name="name">name of the process to remove</param>
            public void RemoveCodingProcess(string name)
            {
                this.staticCompiler.RemoveCodingProcess(name);
            }

            /// <summary>
            /// Returns a user process object identified by a name
            /// A user process has a colon on the first character name
            /// </summary>
            /// <param name="name">name of the process to find</param>
            /// <returns>process found or throw an exception</returns>
            public IProcess GetProcess(string name)
            {
                return this.staticCompiler.GetProcess(name);
            }

            /// <summary>
            /// Add a new process in the process list
            /// Pass a project interface reference and cast to instance of the implemented process class
            /// </summary>
            /// <param name="process">process object</param>
            public void AddProcess(IProcess process)
            {
                this.staticCompiler.AddProcess(process);
            }

            /// <summary>
            /// Push a process on a stack
            /// It's required to loaded process
            /// </summary>
            /// <param name="process">process object</param>
            public void PushProcess(IProcess process)
            {
                this.staticCompiler.PushProcess(process);
            }

            /// <summary>
            /// Pop a process on the top of the stack
            /// It's required to quit loaded process
            /// </summary>
            public void PopProcess()
            {
                this.staticCompiler.PopProcess();
            }

            /// <summary>
            /// Push an instance process on the stack
            /// It's required to called process
            /// </summary>
            /// <param name="proc">process instance object</param>
            public void PushCalledProcess(IProcessInstance proc)
            {
                this.executedProcessStack.Push(proc);
            }

            /// <summary>
            /// Pop a process on the top of the stack
            /// It's required to quit called process
            /// </summary>
            public void PopCalledProcess()
            {
                this.executedProcessStack.Pop();
            }

            /// <summary>
            /// Function to construct a complete dictionary object
            /// by the CodeCommander file source
            /// </summary>
            /// <param name="fileSource">file name source</param>
            /// <returns>a dictionary object</returns>
            public Dictionnaire OutputDictionary(string fileSource)
            {
                return this.staticCompiler.OutputDictionary(fileSource);
            }

            /// <summary>
            /// Function to construct a list of summary object
            /// by the CodeCommander file source
            /// </summary>
            /// <param name="fileSource">file name source</param>
            /// <returns>a summary object</returns>
            public ILegendeDict OutputLegendes(string fileSource)
            {
                return this.staticCompiler.OutputLegendes(fileSource);
            }

            /// <summary>
            /// Summary has been modified from the web navigator
            /// This method keeps the summary in internal
            /// </summary>
            /// <param name="elem">html element</param>
            public void InputLegendes(System.Windows.Forms.HtmlElement elem)
            {
                this.staticCompiler.InputLegendes(elem);
            }

            /// <summary>
            /// Summary persistence
            /// This method keeps the summary in internal
            /// </summary>
            /// <param name="legendes">summary object</param>
            public void InputLegendes(ILegendeDict legendes)
            {
                this.currentDict.Legendes.CopyFrom(legendes);
            }

            /// <summary>
            /// The method compiler
            /// </summary>
            /// <param name="fileSource">CodeCommander file source</param>
            /// <param name="fileData">dictionary file</param>
            /// <param name="finalFile">final file</param>
            /// <param name="wait">notification object (to display a progress bar)</param>
            public void Compilation(string fileSource, string fileData, string finalFile, INotifyProgress wait)
            {
                this.staticCompiler.Compilation(fileSource, fileData, finalFile, wait);
            }

            /// <summary>
            /// The method compiler (use this method when the CodeCommander file source has been loaded either)
            /// </summary>
            /// <param name="fileData">dictionary file</param>
            /// <param name="finalFile">final file</param>
            /// <param name="wait">notification object (to display a progress bar)</param>
            public void Compilation(string fileData, string finalFile, INotifyProgress wait)
            {
                this.staticCompiler.Compilation(fileData, finalFile, wait);
            }

            /// <summary>
            /// The defaut method compiler
            /// </summary>
            /// <param name="fileSource">CodeCommander file source</param>
            /// <param name="fileData">dictionary file</param>
            /// <param name="finalFile">final file</param>
            /// <param name="destEncoding">final file encoding</param>
            /// <param name="wait">notification object (to display a progress bar)</param>
            public void Compilation(string fileSource, string fileData, string finalFile, string destEncoding, INotifyProgress wait)
            {
                this.staticCompiler.Compilation(fileSource, fileData, finalFile, destEncoding, wait);
            }

            /// <summary>
            /// The method for debugging
            /// </summary>
            /// <param name="fileData">dictionary file</param>
            /// <param name="finalFile">final file</param>
            /// <param name="wait">notification object (to display a progress bar)</param>
            public void Debug(string fileData, string finalFile, INotifyProgress wait)
            {
                this.staticCompiler.Debug(fileData, finalFile, wait);
            }

            /// <summary>
            /// The method for debugging
            /// </summary>
            /// <param name="fileSource">CodeCommander file source</param>
            /// <param name="wait">notification object (to display a progress bar)</param>
            public void Debug(string fileSource, INotifyProgress wait)
            {
                this.staticCompiler.Debug(fileSource, wait);
            }

            /// <summary>
            /// Get header from a CodeCommander file source
            /// </summary>
            /// <param name="fileSource">CodeCommander file source</param>
            /// <param name="creationDate">returns the creation date</param>
            /// <param name="modificationDate">returns the modification date</param>
            /// <param name="revision">returns the revision date</param>
            public void GetHeader(string fileSource, out string creationDate, out string modificationDate, out string revision)
            {
                this.staticCompiler.GetHeader(fileSource, out creationDate, out modificationDate, out revision);
            }

            /// <summary>
            /// Converts a CodeCommander source file into a programming language
            /// </summary>
            /// <param name="converter">a converter object</param>
            /// <param name="fileSource">CodeCommander file source</param>
            /// <param name="fileData">dictionary file data</param>
            /// <param name="finalFile">final file (used to set the path output onto the converted source code)</param>
            public void Convert(ICodeConverter converter, string fileSource, string fileData, string finalFile)
            {
                this.staticCompiler.Convert(converter, fileSource, fileData, finalFile);
            }

            /// <summary>
            /// Load an xml node
            /// </summary>
            /// <param name="comp">this compiler</param>
            /// <param name="node">xml node</param>
            public void Load(ICompilateurInstance comp, XmlNode node)
            {
                this.staticCompiler.Load(comp, node);
            }

            /// <summary>
            /// The injection allows to inject code into a process
            /// </summary>
            /// <param name="injector">injector object</param>
            /// <param name="node">xml node</param>
            /// <param name="modifier">modifier after, before or replace</param>
            public void Injection(Injector injector, System.Xml.XmlNode node, string modifier)
            {
                foreach (XmlNode elem in node.ChildNodes)
                {
                    switch (elem.Name.ToLower())
                    {
                        case "texte":
                            {
                                Texte t = new Texte();
                                t.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, t, modifier);
                                break;
                            }
                        case "br":
                            {
                                Br crLf = new Br();
                                crLf.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, crLf, modifier);
                                break;
                            }
                        case "variable":
                            {
                                Variabble var = new Variabble();
                                var.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, var, modifier);
                                break;
                            }
                        case "champ":
                            {
                                Champ ch = new Champ();
                                ch.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, ch, modifier);
                                break;
                            }
                        case "size":
                            {
                                Taille t = new Taille();
                                t.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, t, modifier);
                                break;
                            }
                        case "affectation":
                            {
                                Affectation aff = new Affectation();
                                aff.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, aff, modifier);
                                break;
                            }
                        case "affectationchaine":
                            {
                                AffectationChaine ach = new AffectationChaine();
                                ach.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, ach, modifier);
                                break;
                            }
                        case "affectationchamp":
                            {
                                AffectationChamp ac = new AffectationChamp();
                                ac.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, ac, modifier);
                                break;
                            }
                        case "condition":
                            {
                                Condition c = new Condition();
                                c.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, c, modifier);
                                break;
                            }
                        case "label":
                            {
                                Label l = new Label();
                                l.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, l, modifier);
                                break;
                            }
                        case "call":
                            {
                                Call c = new Call();
                                c.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, c, modifier);
                                break;
                            }
                        case "callskeleton":
                            {
                                CallSkeleton cs = new CallSkeleton();
                                cs.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, cs, modifier);
                                break;
                            }
                        case "beginprocess":
                            {
                                // empile le processus courant
                                injector.StackProcesses.Push(injector.InjectedProcess);
                                // démarre un nouveau processus
                                BeginProcess bp = new BeginProcess();
                                bp.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, bp, modifier);
                                Process p = new Process();
                                p.Name = ":" + bp.ProcessName;
                                injector.InjectedProcess = p;
                                IProcess old = this.Processes.Find(new Predicate<IProcess>(delegate(IProcess iter) { return iter.Name == p.Name; }));
                                if (old != null)
                                {
                                    this.Processes.Remove(old);
                                    old.Dispose();
                                }
                                this.staticCompiler.Processes.Add(injector.InjectedProcess);
                                break;
                            }
                        case "beginskeleton":
                            {
                                // empile le process courant
                                injector.StackProcesses.Push(injector.InjectedProcess as Process);
                                // démarre un nouveau processus
                                BeginSkeleton bs = new BeginSkeleton();
                                bs.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, bs, modifier);
                                Process p = new Process();
                                Skeleton skel = new Skeleton(bs.Path, bs.Name);
                                skel.Legendes = this.staticCompiler.Legendes;
                                skel.Process = p;
                                p.Name = "Skeleton:" + bs.Path + "/" + bs.Name;
                                injector.InjectedProcess = p;
                                Skeleton old = this.Skeletons.Find(new Predicate<Skeleton>(delegate(Skeleton iter) { return iter.Path + "/" + iter.Name == skel.Path + "/" + skel.Name; }));
                                if (old != null)
                                {
                                    this.Skeletons.Remove(old);
                                }
                                this.Skeletons.Add(skel);
                                break;
                            }
                        case "endprocess":
                            {
                                EndProcess ep = new EndProcess();
                                ep.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, ep, modifier);
                                if (injector.StackProcesses.Count > 0)
                                {
                                    IProcess previousProcess = injector.StackProcesses.Peek();
                                    if (injector.InjectedProcess.Name == ":" + ep.ProcessName)
                                    {
                                        injector.StackProcesses.Pop();
                                        injector.InjectedProcess = previousProcess;
                                    }
                                    else
                                        throw new Exception("Le processus actif (" + injector.InjectedProcess.Name + ") n'a pas le même nom que ':" + ep.ProcessName + "'");
                                }
                                else
                                    throw new Exception("Il n'y a pas de processus en cours pour terminer '" + ep.ProcessName + "'");
                                break;
                            }
                        case "endskeleton":
                            {
                                EndSkeleton es = new EndSkeleton();
                                es.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, es, modifier);
                                if (injector.StackProcesses.Count > 0)
                                {
                                    IProcess previousProcess = injector.StackProcesses.Peek();
                                    if (injector.InjectedProcess.Name == "Skeleton:" + es.Path + "/" + es.Name)
                                    {
                                        injector.StackProcesses.Pop();
                                        injector.InjectedProcess = previousProcess;
                                    }
                                    else
                                        throw new Exception("Le processus actif ('" + previousProcess.Name + "') n'a pas le même nom que '" + es.Path + "/" + es.Name + "'");
                                }
                                else
                                    throw new Exception("Il n'y a pas de squelette en cours pour terminer '" + es.Path + "/" + es.Name + "'");
                                break;
                            }
                        case "template":
                            {
                                Template temp = new Template();
                                temp.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, temp, modifier);
                                this.staticCompiler.Templates.Add(temp);
                                break;
                            }
                        case "usetemplate":
                            {
                                UseTemplate ut = new UseTemplate();
                                // uniquement pour l'extraction du dictionnaire et la conversion du code et avant de charger la suite
                                injector.InjectObject(injector.InjectedProcess, ut, modifier);
                                ut.Inject(injector, this, elem, modifier);
                                break;
                            }
                        case "handler":
                            {
                                Handler h = new Handler();
                                h.Inject(injector, this, elem, modifier);
                                if (this.CurrentUsingTemplate != null)
                                {
                                    Coding code = this.CurrentUsingTemplate.GetCoding(h.HandlerName);
                                    if (code != null)
                                        this.Injection(injector, code.XmlCode, modifier);
                                }
                                else
                                    throw new Exception("Aucune utilisation d'un template n'est en cours pour déclarer un événement");
                                break;
                            }
                        case "coding":
                            {
                                if (this.CurrentUsingTemplate != null)
                                {
                                    Coding code = new Coding();
                                    code.Inject(injector, this, elem, modifier);
                                    // nécessaire pour la conversion de code
                                    injector.InjectObject(injector.InjectedProcess, code, modifier);
                                    this.CurrentUsingTemplate.AddCoding(code);
                                    // ajouté pour convertir le code
                                    if (this.UnderConversion)
                                    {
                                        Process proc = new Process();
                                        proc.Name = "Coding:" + code.UniqueCodingName;
                                        this.Processes.Add(proc);
                                        injector.StackProcesses.Push(injector.InjectedProcess);
                                        injector.InjectedProcess = proc;
                                        this.Injection(injector, code.XmlCode, modifier);
                                        injector.InjectedProcess = injector.StackProcesses.Pop();
                                    }
                                }
                                else
                                    throw new Exception("Aucune utilisation d'un template n'est en cours pour déclarer un code");
                                break;
                            }
                        case "parallel":
                            {
                                Parallel par = new Parallel();
                                par.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, par, modifier);
                                // ajout pour convertir le code
                                if (this.UnderConversion)
                                {
                                    Process proc = new Process();
                                    proc.Name = "Parallel:" + injector.InjectedProcess.Name;
                                    this.Processes.Add(proc);
                                    injector.StackProcesses.Push(injector.InjectedProcess as Process);
                                    injector.InjectedProcess = proc;
                                    this.Injection(injector, par.XmlCode, modifier);
                                    injector.InjectedProcess = injector.StackProcesses.Pop();
                                }
                                break;
                            }
                        case "defaultwriter":
                            {
                                DefaultWriter dw = new DefaultWriter();
                                dw.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, dw, modifier);
                                break;
                            }
                        case "createwriter":
                            {
                                CreateWriter cw = new CreateWriter();
                                cw.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, cw, modifier);
                                break;
                            }
                        case "locale":
                            {
                                Locale loc = new Locale();
                                loc.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, loc, modifier);
                                break;
                            }
                        case "syntax":
                            {
                                Syntax syn = new Syntax();
                                syn.Inject(injector, this, elem, modifier);
                                // just one element with same name
                                Syntax old = this.Syntaxes.Find(new Predicate<Syntax>(delegate(Syntax s) { return s.Name == syn.Name; }));
                                if (old != null)
                                    this.Syntaxes.Remove(old);
                                this.Syntaxes.Add(syn);
                                break;
                            }
                        case "createmop":
                            {
                                CreateMOP mop = new CreateMOP();
                                mop.Inject(injector, this, elem, modifier);
                                // injection du mop pendant la conversion
                                if (this.UnderConversion)
                                {
                                    injector.Inject(injector, this, elem, modifier);
                                }
                                // add the mop ; replace if already exists
                                CreateMOP old = this.Mops.Find(new Predicate<CreateMOP>(delegate(CreateMOP m)
                                {
                                    return m.Language == mop.Language && m.Name == mop.Name;
                                }));
                                if (old != null)
                                    this.Mops.Remove(old);
                                this.Mops.Add(mop);
                                break;
                            }
                        case "injector":
                            {
                                Injector inj = new Injector();
                                // insertion de l'objet avant le load pour injector qui doit récupérer sa position
                                injector.InjectObject(injector.InjectedProcess, inj, modifier);
                                inj.Inject(injector, this, elem, modifier);
                                // just the last element with same name
                                Injector old = this.Injectors.Find(new Predicate<Injector>(delegate(Injector i) { return i.LongName == inj.LongName; }));
                                if (old != null)
                                    this.Injectors.Remove(old);
                                this.Injectors.Add(inj);
                                break;
                            }
                        case "usemop":
                            {
                                UseMOP um = new UseMOP();
                                um.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, um, modifier);
                                break;
                            }
                        case "indent":
                            {
                                Indent i = new Indent();
                                i.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, i, modifier);
                                break;
                            }
                        case "unindent":
                            {
                                Unindent ui = new Unindent();
                                ui.Inject(injector, this, elem, modifier);
                                injector.InjectObject(injector.InjectedProcess, ui, modifier);
                                break;
                            }
                    }
                }
            }

            /// <summary>
            /// Converts a template to a programming language destination
            /// </summary>
            /// <param name="converter">a converter object</param>
            /// <param name="templateName">template name</param>
            /// <param name="nc">struct contained the callback function as the request template</param>
            /// <param name="file">final file (used to set the path output onto the converted source code)</param>
            public void Convert(ICodeConverter converter, string templateName, NameConverter nc, FinalFile file)
            {
                if (templateName.StartsWith("/String/"))
                {
                    nc.del(this, this.CurrentProcess.GetInstance(0), converter, file);
                }
                else
                {
                    this.ProcessStack.Push(this.CurrentProcess);
                    Process p = new Process();
                    p.Name = "Template:" + this.Unique.ComputeNewString();
                    p.FunctionName = this.CurrentProcess.FunctionName;
                    p.MakeOneInstance(this.CurrentProcess.GetInstance(0), new RunInnerInstanceProcess(delegate(IProcessInstance prev, IProcessInstance sub)
                    {
                        sub.CurrentDictionnaire = prev.CurrentDictionnaire;
                        sub.CurrentScope = prev.CurrentScope;
                        sub.CurrentScope.Push();
                        sub.DefaultWriter = prev.DefaultWriter;
                        this.CurrentProcess = p;
                        nc.del(this, sub, converter, file);
                        sub.CurrentScope.Pop();
                    }));
                    this.CurrentProcess = this.ProcessStack.Pop();
                }
            }

            /// <summary>
            /// Conversion starting function with a next process
            /// </summary>
            /// <param name="converter">converter object</param>
            /// <param name="proc">process object to convert</param>
            /// <param name="file">final file</param>
            public void Convert(ICodeConverter converter, IProcess proc, FinalFile file)
            {
                IProcessInstance current = null;
                if (proc.Name != Compilateur.RootProcessName)
                {
                    this.ProcessStack.Push(this.CurrentProcess);
                    current = this.CurrentProcess.GetInstance(0);
                }
                proc.MakeOneInstance(current, new RunInnerInstanceProcess(delegate(IProcessInstance prev, IProcessInstance p)
                {
                    p.CurrentDictionnaire = (prev != null) ? prev.CurrentDictionnaire : this.CurrentDictionnaire;
                    p.CurrentScope = (prev != null) ? prev.CurrentScope : this.CurrentScope;
                    p.CurrentScope.Push();
                    p.DefaultWriter = (prev != null) ? prev.DefaultWriter : Compilateur.InitialWriter;
                    this.CurrentProcess = proc;
                    p.Convert(converter, this, file);
                    p.CurrentScope.Pop();
                }));
                if (this.ProcessStack.Count > 0)
                    this.CurrentProcess = this.ProcessStack.Pop();
                else
                    this.CurrentProcess = null;
            }

            /// <summary>
            /// Save objects from the web navigator
            /// </summary>
            /// <param name="elem">html element</param>
            /// <returns>the xml string</returns>
            public string SaveToString(System.Windows.Forms.HtmlElement elem)
            {
                return this.staticCompiler.SaveToString(elem);
            }

            /// <summary>
            /// Save child objects from the web navigator
            /// </summary>
            /// <param name="elem">html element</param>
            /// <returns>the xml string</returns>
            public string SaveChildToString(System.Windows.Forms.HtmlElement elem)
            {
                return this.staticCompiler.SaveChildToString(elem);
            }

            /// <summary>
            /// Save objects from the web navigator
            /// </summary>
            /// <param name="fileDestination">file where to write</param>
            /// <param name="elem">html element</param>
            /// <param name="wait">notification object (to display a progress bar)</param>
            /// <param name="infos">header infos</param>
            public void Save(string fileDestination, System.Windows.Forms.HtmlElement elem, INotifyProgress wait, params string[] infos)
            {
                this.staticCompiler.Save(fileDestination, elem, wait, infos);
            }

            /// <summary>
            /// Save childs objects to an xml writer
            /// </summary>
            /// <param name="writer">xml writer object</param>
            /// <param name="elem">last html element to continue</param>
            /// <param name="currentIndent">current indentation</param>
            public void Save(XmlWriter writer, ref System.Windows.Forms.HtmlElement elem, int currentIndent)
            {
                this.staticCompiler.Save(writer, ref elem, currentIndent);
            }

            /// <summary>
            /// Save xml string into a CodeCommander file source (an xml file)
            /// </summary>
            /// <param name="fileDestination">destination file name</param>
            /// <param name="xmlCode">xml string to persist</param>
            /// <param name="legendes">summary data</param>
            public void Save(string fileDestination, string xmlCode, ILegendeDict legendes)
            {
                this.staticCompiler.Save(fileDestination, xmlCode, legendes);
            }

            /// <summary>
            /// Find a template by a name
            /// </summary>
            /// <param name="name">template name to find</param>
            /// <returns>template object else throw an exception</returns>
            public Template GetTemplate(string name)
            {
                return this.staticCompiler.GetTemplate(name);
            }

            /// <summary>
            /// Find a mop by a language and a name
            /// </summary>
            /// <param name="language">language</param>
            /// <param name="name">name of the mop</param>
            /// <returns>mop object or throw an exception</returns>
            public CreateMOP GetMOP(string language, string name)
            {
                return this.staticCompiler.GetMOP(language, name);
            }

            /// <summary>
            /// Get an already registered injector
            /// </summary>
            /// <param name="map">the name of the injector (can be in and out a skeleton)</param>
            /// <returns>injector object or throw an exception</returns>
            public Injector GetInjector(string map)
            {
                return this.staticCompiler.GetInjector(map);
            }

            /// <summary>
            /// Create a CodeCommander file source
            /// (overwrite if exists)
            /// </summary>
            /// <param name="fileName">file name to create</param>
            public void CreateFile(string fileName)
            {
                this.staticCompiler.CreateFile(fileName);
            }

            public object Clone()
            {
                return this.staticCompiler.Clone();
            }

            /// <summary>
            /// Internal starting function to execute statements from a next process
            /// Open for a recursive process
            /// </summary>
            /// <param name="proc">a process instance to execute</param>
            /// <param name="file">final file</param>
            public void WriteToFile(IProcess proc, FinalFile file)
            {
                if (this.currentExecutedProcess != null)
                    this.CalledProcesses.Push(this.currentExecutedProcess);
                proc.MakeNewInstance(this.currentExecutedProcess, new RunInnerInstanceProcess(delegate(IProcessInstance prev, IProcessInstance i)
                {
                    i.DefaultWriter = (prev != null) ? prev.DefaultWriter : Compilateur.InitialWriter;
                    i.CurrentDictionnaire = (prev != null) ? prev.CurrentDictionnaire : this.CurrentDictionnaire;
                    // chaque processus en parallèle possède son propre Scope
                    // et le scope actuel devient un parent pour le nouveau Scope
                    if (prev != null && prev.Name.StartsWith("Parallel:"))
                    {
                        i.CurrentScope = new o2Mate.Scope();
                        i.CurrentScope.Parent = prev.CurrentScope;
                    }
                    else
                        i.CurrentScope = (prev != null) ? prev.CurrentScope : this.CurrentScope;
                    i.CurrentProject = (prev != null) ? prev.CurrentProject : this.Threads;
                    i.CurrentScope.Push();
                    this.currentExecutedProcess = i;
                    i.ExecuteProcess(this, file);
                    i.CurrentScope.Pop();
                }));
                if (this.CalledProcesses.Count > 0)
                    this.currentExecutedProcess = this.CalledProcesses.Pop();
                else
                    this.currentExecutedProcess = null;
            }

            /// <summary>
            /// Extracts dictionary names from a next process
            /// </summary>
            /// <param name="proc">process onto extract dictionary names</param>
            public void ExtractDictionary(IProcess proc)
            {
                IProcessInstance current = null;
                if (proc.Name != Compilateur.RootProcessName)
                {
                    this.PushProcess(this.CurrentProcess);
                    current = this.CurrentProcess.GetInstance(0);
                }
                this.CurrentProcess = proc;
                proc.MakeOneInstance(current, new RunInnerInstanceProcess(delegate(IProcessInstance prev, IProcessInstance i)
                {
                    i.CurrentDictionnaire = (prev != null) ? prev.CurrentDictionnaire : this.CurrentDictionnaire;
                    i.CurrentScope = (prev != null) ? prev.CurrentScope : this.CurrentScope;
                    i.ExtractDictionary(this);
                }));
                if (this.ProcessStack.Count > 0) {
                    this.CurrentProcess = this.ProcessStack.Peek();
                    this.PopProcess();
                }
                else
                    this.CurrentProcess = null;
            }

            #endregion
        }
        #endregion

        #region Private Fields
        private List<CompilateurState> instances;
        private UniqueStrings uniques;
        private ICodeConverter convertedLanguage;
        private List<Template> templates;
        private List<Syntax> syntaxes;
        private List<Injector> injectors;
        private List<CreateMOP> mops;
        private Process principal;
        private UseTemplate currentUsingTemplate;
        private Stack<UseTemplate> usingTemplateStack;
        private List<IProcess> processes;
        private List<Skeleton> skeletons;
        private ICompilateurInstance currentInstance;
        private IProcess currentProcess;
        private Process currentParseProcess;
        private Stack<IProcess> processStack;
        private Tree<ICompiler> objects;
        private Projects threads;
        private LegendeDict legendeDict;
        private bool copyElements;
        private bool startRecording;
        private string from;
        private string to;
        private bool underConversion;
        private Encoding enc;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Compilateur()
        {
            this.instances = new List<CompilateurState>();
            this.uniques = new UniqueStrings();
            this.convertedLanguage = null;
            this.objects = new Tree<ICompiler>();
            this.threads = new Projects();
            this.templates = new List<Template>();
            this.syntaxes = new List<Syntax>();
            this.injectors = new List<Injector>();
            this.mops = new List<CreateMOP>();
            this.currentUsingTemplate = null;
            this.usingTemplateStack = new Stack<UseTemplate>();
            this.principal = new Process();
            this.principal.Name = Compilateur.RootProcessName;
            this.processes = new List<IProcess>();
            this.skeletons = new List<Skeleton>();
            this.currentProcess = null;
            this.currentParseProcess = this.principal;
            this.processStack = new Stack<IProcess>();
            this.legendeDict = new LegendeDict();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets summary
        /// </summary>
        public ILegendeDict Legendes
        {
            get { return this.legendeDict; }
        }

        /// <summary>
        /// Gets or sets encoding file
        /// </summary>
        public Encoding EncodedFile
        {
            get
            {
                if (this.enc == null)
                    return Encoding.Unicode;
                else return this.enc;
            }
            set { this.enc = value; } 
        }

        /// <summary>
        /// Objects list (this is a tree for embedded objects)
        /// </summary>
        public Tree<ICompiler> Objects
        {
            get { return this.objects; }
        }

        /// <summary>
        /// Returns all projects defined in a CodeCommander file source
        /// </summary>
        public Projects Threads
        {
            get { return this.threads; }
        }

        /// <summary>
        /// Returns all known templates
        /// </summary>
        public List<Template> Templates
        {
            get
            {
                return this.templates;
            }
        }

        /// <summary>
        /// Returns all known skeletons
        /// </summary>
        public List<Skeleton> Skeletons
        {
            get
            {
                return this.skeletons;
            }
        }

        /// <summary>
        /// Returns all known syntax
        /// </summary>
        public List<Syntax> Syntaxes
        {
            get
            {
                return this.syntaxes;
            }
        }

        /// <summary>
        /// Returns all known MOPs
        /// </summary>
        public List<CreateMOP> Mops
        {
            get { return this.mops; }
        }

        /// <summary>
        /// Returns all known injectors
        /// </summary>
        public List<Injector> Injectors
        {
            get { return this.injectors; }
        }

        /// <summary>
        /// Returns the principal process
        /// </summary>
        internal Process Principal
        {
            get
            {
                return this.principal;
            }
        }

        /// <summary>
        /// Returns all processes (all types included)
        /// </summary>
        public List<IProcess> Processes
        {
            get
            {
                return this.processes;
            }
        }

        /// <summary>
        /// Gets or sets the current process (while loading)
        /// </summary>
        public IProcess CurrentProcess
        {
            get
            {
                return this.currentProcess;
            }
            set
            {
                this.currentProcess = value;
            }
        }

        /// <summary>
        /// Gets or sets the current instance
        /// </summary>
        public ICompilateurInstance CurrentInstance
        {
            get
            {
                return this.currentInstance;
            }
            set
            {
                this.currentInstance = value;
            }
        }

        /// <summary>
        /// Returns the current process during parsing
        /// </summary>
        internal Process CurrentParseProcess
        {
            get
            {
                return this.currentParseProcess;
            }
        }

        /// <summary>
        /// Returns the stack of loaded processes
        /// </summary>
        public Stack<IProcess> ProcessStack
        {
            get
            {
                return this.processStack;
            }
        }

        /// <summary>
        /// Gets or sets being in conversion to a programming language
        /// </summary>
        public bool UnderConversion
        {
            get { return this.underConversion; }
            set { this.underConversion = value; }
        }

        /// <summary>
        /// Gets or sets the programming language onto conversion
        /// </summary>
        public ICodeConverter ConvertedLanguage
        {
            get { return this.convertedLanguage; }
            set { this.convertedLanguage = value; }
        }


        /// <summary>
        /// Gets the unique system creation of a name
        /// </summary>
        public UniqueStrings Unique
        {
            get { return this.uniques; }
        }

        /// <summary>
        /// Returns the current using template
        /// </summary>
        public UseTemplate CurrentUsingTemplate
        {
            get
            {
                return this.currentUsingTemplate;
            }
            set
            {
                this.currentUsingTemplate = value;
            }
        }

        /// <summary>
        /// Assumes starting a template and do the good work
        /// </summary>
        /// <param name="curTemp">template object</param>
        public void StartUsingTemplate(UseTemplate curTemp)
        {
            this.currentUsingTemplate = curTemp;
            this.usingTemplateStack.Push(curTemp);
        }

        /// <summary>
        /// Finalize a template
        /// </summary>
        /// <param name="curTemp">template object</param>
        public void EndUsingTemplate(UseTemplate curTemp)
        {
            if (this.usingTemplateStack.Count > 0)
            {
                UseTemplate lastTemp = this.usingTemplateStack.Peek();
                if (lastTemp.Name == curTemp.Name)
                {
                    this.usingTemplateStack.Pop();
                    if (this.usingTemplateStack.Count > 0)
                    {
                        this.currentUsingTemplate = this.usingTemplateStack.Peek();
                    }
                    else
                    {
                        this.currentUsingTemplate = null;
                    }
                }
                else
                    throw new Exception("L'utilisation du template actif n'a pas le même nom");
            }
            else
                throw new Exception("Il n'y a pas d'utilisation d'un template qui soit activé");
        }

        #endregion

        #region Private methods
        private void BacktrackProcessesAndUpdateParameters(ICodeConverter converter, IProcessInstance proc, IData var, bool mutable)
        {
            // si elle est déjà dans une variable locale alors on considère qu'elle n'est pas en paramètre même si elle ne fait pas partie du processus courant
            // c'est pour les codings qui sont dans des processus mais ne crée pas de fonction dans la conversion
            IFunction f = converter.ImplementedFunctions.Find(new Predicate<IFunction>(delegate(IFunction func)
            {
                return func.Name == converter.ProcessAsFunction;
            }));
            if (!f.LocalVariables.Exists(new Predicate<IStructure>(delegate(IStructure st) { return (converter.IsStronglyTyped && st.PrefixedFieldName == var.PrefixedName) || (!converter.IsStronglyTyped && st.FieldName == var.Name); })))
            {
                IParameter p = f.Parameters.Find(new Predicate<IParameter>(delegate(IParameter param) { return (converter.IsStronglyTyped && param.FormalParameter == var.PrefixedName) || (!converter.IsStronglyTyped && param.VariableName == var.Name); }));
                if (p != null)
                {
                    // cas où la variable a été modifiée
                    if (!p.IsMutableParameter && mutable)
                    {
                        p.IsDirty = true;
                        p.IsMutableParameter = true;
                    }
                    // une variable mutable (en paramètre) ne peut pas être calculable dans une fonction
                    p.IsComputable = (p.IsMutableParameter ? false : p.IsComputable);

                    // recherche s'il y a déjà un paramètre d'un autre type pour cette variable
                    if (converter.IsStronglyTyped && f.Parameters.Exists(new Predicate<IParameter>(delegate(IParameter par)
                    {
                        return par.VariableName == var.Name && par.FormalParameter != var.PrefixedName && par.Order == EnumParameterOrder.E_NEW;
                    })) && p.IsMutableParameter)
                    {
                        // le dernier autre type à avoir été modifié
                        f.Parameters.ForEach(new Action<IParameter>(delegate(IParameter par)
                        {
                            if (par.VariableName == var.Name && par.Order == EnumParameterOrder.E_LAST)
                                par.Order = EnumParameterOrder.E_CURRENT;
                        }));
                        p.Order = EnumParameterOrder.E_LAST;
                    }
                }
                else
                {
                    Parameter newParam = new Parameter();
                    newParam.VariableName = var.Name;
                    newParam.FormalParameter = var.PrefixedName;
                    newParam.EffectiveParameter = var.PrefixedName;
                    newParam.DataType = var.DataType;
                    // une variable mutable (en paramètre) ne peut pas être calculable dans une fonction
                    newParam.IsMutableParameter = mutable;
                    // je préfère considérer les paramètres comme non calculables
                    //newParam.IsComputable = (mutable ? false : var.IsComputable);
                    newParam.IsComputable = false;
                    if (mutable) newParam.IsDirty = true;

                    // recherche s'il n'y a pas déjà un paramètre d'un autre type pour cette variable
                    if (converter.IsStronglyTyped && !f.Parameters.Exists(new Predicate<IParameter>(delegate(IParameter par)
                    {
                        return par.VariableName == var.Name && par.Order == EnumParameterOrder.E_NEW;
                    })))
                    {
                        newParam.Order = EnumParameterOrder.E_NEW;
                    }
                    else if (converter.IsStronglyTyped && mutable)   // le dernier type à avoir été modifié
                    {
                        // il faut conserver le paramètre inital ainsi que le dernier paramètre
                        // le dernier autre type à avoir été modifié
                        f.Parameters.ForEach(new Action<IParameter>(delegate(IParameter par)
                        {
                            if (par.VariableName == var.Name && par.Order == EnumParameterOrder.E_LAST)
                                par.Order = EnumParameterOrder.E_CURRENT;
                        }));
                        newParam.Order = EnumParameterOrder.E_LAST;
                    }
                    f.Parameters.Add(newParam);
                }
                IProcess[] list = this.ProcessStack.ToArray();
                for (int index = 0; index < list.GetLength(0); ++index)
                {
                    IProcess processInStack = list[index];
                    // ajouter le paramètre en référence dans les autres processus de la pile
                    f = converter.ImplementedFunctions.Find(new Predicate<IFunction>(delegate(IFunction func) { return func.Name == processInStack.FunctionName; }));
                    // à un processus, il existe toujours une fonction qui convertit ce processus
                    // Au cas où une fonction implémente plusieurs processus de manière séquentielle, il se peut que la variable soit locale à la fonction
                    // dans ce cas, on laisse tel quel
                    if (!f.LocalVariables.Exists(new Predicate<IStructure>(delegate(IStructure st) { return (converter.IsStronglyTyped && st.PrefixedFieldName == var.PrefixedName) || (!converter.IsStronglyTyped && st.FieldName == var.Name); })))
                    {
                        // si la variable n'appartient pas à ce processus
                        if (var.BelongsTo != processInStack.Name)
                        {
                            IParameter searchParam = f.Parameters.Find(new Predicate<IParameter>(delegate(IParameter param) { return (converter.IsStronglyTyped && param.FormalParameter == var.PrefixedName) || (!converter.IsStronglyTyped && param.VariableName == var.Name); }));
                            if (searchParam != null)
                            {
                                // cas où la variable a été modifiée
                                if (!searchParam.IsMutableParameter && mutable)
                                {
                                    searchParam.IsDirty = true;
                                    searchParam.IsMutableParameter = true;
                                }
                                // une variable mutable (en paramètre) ne peut pas être calculable dans une fonction
                                searchParam.IsComputable = (searchParam.IsMutableParameter ? false : searchParam.IsComputable);

                                // recherche s'il y a déjà un paramètre d'un autre type pour cette variable
                                if (converter.IsStronglyTyped && f.Parameters.Exists(new Predicate<IParameter>(delegate(IParameter par)
                                {
                                    return par.VariableName == var.Name && par.FormalParameter != var.PrefixedName && par.Order == EnumParameterOrder.E_NEW;
                                })) && searchParam.IsMutableParameter)
                                {
                                    // le dernier type à avoir été modifié
                                    f.Parameters.ForEach(new Action<IParameter>(delegate(IParameter par)
                                    {
                                        if (par.VariableName == var.Name && par.Order == EnumParameterOrder.E_LAST)
                                            par.Order = EnumParameterOrder.E_CURRENT;
                                    }));
                                    searchParam.Order = EnumParameterOrder.E_LAST;
                                }
                            }
                            else
                            {
                                Parameter newParam = new Parameter();
                                newParam.VariableName = var.Name;
                                newParam.FormalParameter = var.PrefixedName;
                                newParam.EffectiveParameter = var.PrefixedName;
                                newParam.DataType = var.DataType;
                                // une variable mutable (en paramètre) ne peut pas être calculable dans une fonction
                                newParam.IsMutableParameter = mutable;
                                // je préfère considérer les paramètres comme non calculables
                                //newParam.IsComputable = (mutable ? false : var.IsComputable);
                                newParam.IsComputable = false;
                                if (mutable) newParam.IsDirty = true;

                                // recherche s'il y a déjà un paramètre typé pour cette variable
                                if (converter.IsStronglyTyped && !f.Parameters.Exists(new Predicate<IParameter>(delegate(IParameter par)
                                {
                                    return par.VariableName == var.Name && par.Order == EnumParameterOrder.E_NEW;
                                })))
                                {
                                    newParam.Order = EnumParameterOrder.E_NEW;
                                }
                                else if (converter.IsStronglyTyped && mutable)
                                {
                                    // le dernier type à avoir été modifié
                                    f.Parameters.ForEach(new Action<IParameter>(delegate(IParameter par)
                                    {
                                        if (par.VariableName == var.Name && par.Order == EnumParameterOrder.E_LAST)
                                            par.Order = EnumParameterOrder.E_CURRENT;
                                    }));
                                    newParam.Order = EnumParameterOrder.E_LAST;
                                }
                                f.Parameters.Add(newParam);
                            }
                        }
                        else
                        {
                            if (!f.Parameters.Exists(new Predicate<IParameter>(delegate(IParameter param) { return (converter.IsStronglyTyped && param.FormalParameter == var.PrefixedName) || (!converter.IsStronglyTyped && param.VariableName == var.Name); })))
                            {
                                Helper.AddIntoLocal(converter, f, var);
                            }
                            // on s'arrête car la variable appartient à ce processus
                            break;
                        }
                    }
                    else
                    {
                        // on s'arrête car la variable appartient à cette fonction
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Load all files and subfolders from the template directory
        /// </summary>
        /// <param name="di">directory object</param>
        private void LoadTemplates(DirectoryInfo di)
        {
            foreach (FileInfo fi in di.GetFiles("*.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.Load(fi.FullName);
                Process p = new Process();
                p.Name = "Templates";
                this.CurrentProcess = p;
                this.MakeOneInstance(new RunInnerInstanceCompiler(delegate(ICompilateurInstance c)
                {
                    this.Load(c, doc.DocumentElement.SelectSingleNode("code"));
                }));
            }
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                this.LoadTemplates(subDi);
            }
        }

        /// <summary>
        /// Load all files and subfolders from the syntax directory
        /// </summary>
        /// <param name="di">directory object</param>
        private void LoadSyntax(DirectoryInfo di)
        {
            foreach (FileInfo fi in di.GetFiles("*.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.Load(fi.FullName);
                Process p = new Process();
                p.Name = "Syntaxes";
                this.currentProcess = p;
                this.MakeOneInstance(new RunInnerInstanceCompiler(delegate(ICompilateurInstance c)
                {
                    this.Load(c, doc.DocumentElement.SelectSingleNode("code"));
                }));
            }
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                this.LoadSyntax(subDi);
            }
        }

        /// <summary>
        /// Parse all files and subfolders from the skeletons directory
        /// a skeleton is declared with a starter and a closing statement
        /// </summary>
        /// <param name="di">directory object</param>
        private void ParseSkeletons(DirectoryInfo di)
        {
            foreach (FileInfo fi in di.GetFiles("*.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.Load(fi.FullName);
                this.Parse(this, doc.DocumentElement.SelectSingleNode("code"));
            }
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                this.ParseSkeletons(subDi);
            }
        }

        /// <summary>
        /// Load all files and subfolders from the skeletons directory
        /// </summary>
        /// <param name="di">directory object</param>
        private void LoadSkeletons(DirectoryInfo di)
        {
            foreach (FileInfo fi in di.GetFiles("*.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.Load(fi.FullName);
                Process p = new Process();
                p.Name = "Skeletons";
                this.currentProcess = p;
                this.MakeOneInstance(new RunInnerInstanceCompiler(delegate(ICompilateurInstance c)
                {
                    this.Load(c, doc.DocumentElement.SelectSingleNode("code"));
                }));
            }
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                this.LoadSkeletons(subDi);
            }
        }

        /// <summary>
        /// Load all files and subfolders from the MOP directory
        /// </summary>
        /// <param name="di">directory object</param>
        private void LoadMOP(DirectoryInfo di)
        {
            foreach (FileInfo fi in di.GetFiles("*.xml"))
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.Load(fi.FullName);
                Process p = new Process();
                p.Name = "MOPs";
                this.currentProcess = p;
                this.MakeOneInstance(new RunInnerInstanceCompiler(delegate(ICompilateurInstance c)
                {
                    this.Load(c, doc.DocumentElement.SelectSingleNode("code"));
                }));
            }
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                this.LoadMOP(subDi);
            }
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Find a user process by a name ; returns a process interface reference
        /// </summary>
        /// <param name="name">name of the process to find</param>
        /// <returns>process found or throw an exception</returns>
        public IProcess GetProcess(string name)
        {
            foreach (Process proc in this.processes)
            {
                if (proc.Name == ":" + name)
                {
                    return proc;
                }
            }
            throw new Exception("Processus utilisateur '" + name + "' non trouvé");
        }

        /// <summary>
        /// Find a parallel process by a name ; returns a process interface reference
        /// </summary>
        /// <param name="name">name of the parallel process</param>
        /// <returns>process found or throw an exception</returns>
        public IProcess GetParallelProcess(string name)
        {
            foreach (Process proc in this.processes)
            {
                if (proc.Name == "Parallel:" + name)
                {
                    return proc;
                }
            }
            throw new Exception("Processus parallèle '" + name + "' non trouvé");
        }

        /// <summary>
        /// Find a coding process by a name ; returns a process interface reference
        /// </summary>
        /// <param name="name">process name to find</param>
        /// <returns>process found or throw an exception</returns>
        public IProcess GetCodingProcess(string name)
        {
            IProcess found = this.Processes.Find(new Predicate<IProcess>(delegate(IProcess p) { return p.Name == "Coding:" + name; }));
            if (found != null)
            {
                return found;
            }
            throw new Exception("Processus coding '" + name + "' non trouvé");
        }

        /// <summary>
        /// Find a job process by a name ; returns a process interface reference
        /// </summary>
        /// <param name="name">process name to find</param>
        /// <returns>process found or throw an exception</returns>
        public IProcess GetJobProcess(string name)
        {
            IProcess found = this.Processes.Find(new Predicate<IProcess>(delegate(IProcess p) { return p.Name == "Job:" + name; }));
            if (found != null)
            {
                return found;
            }
            throw new Exception("Processus job '" + name + "' non trouvé");
        }

        /// <summary>
        /// Find a skeleton by a name
        /// </summary>
        /// <param name="name">skeleton name to find</param>
        /// <returns>skeleton found or throw an exception</returns>
        public Skeleton GetSkeleton(string name)
        {
            foreach (Skeleton skel in this.skeletons)
            {
                if (skel.Path + "/" + skel.Name == name)
                {
                    return skel;
                }
            }
            throw new Exception("Squelette '" + name + "' non trouvé");
        }

        /// <summary>
        /// Remove a process identified by a name and a type 'coding'
        /// </summary>
        /// <param name="name">name of the process to remove</param>
        public void RemoveCodingProcess(string name)
        {
            IProcess found = this.Processes.Find(new Predicate<IProcess>(delegate(IProcess p) { return p.Name == "Coding:" + name; }));
            if (found != null)
            {
                this.Processes.Remove(found);
                found.Dispose();
            }
        }

        /// <summary>
        /// Find a template by a name
        /// </summary>
        /// <param name="name">template name to find</param>
        /// <returns>template object else throw an exception</returns>
        public Template GetTemplate(string name)
        {
            foreach (Template temp in this.templates)
            {
                string path = temp.Path;
                if (!path.EndsWith("/")) path += "/";
                if (path + temp.Name == name)
                {
                    return temp;
                }
            }
            throw new Exception("Template '" + name + "' non trouvé");
        }

        /// <summary>
        /// Find a syntax by a name
        /// </summary>
        /// <param name="name">name of the syntax</param>
        /// <returns>syntax object or throw an exception</returns>
        public Syntax GetSyntax(string name)
        {
            foreach (Syntax syn in this.syntaxes)
            {
                if (syn.Name == name)
                {
                    return syn;
                }
            }
            throw new Exception("Syntaxe '" + name + "' non trouvée");
        }

        /// <summary>
        /// Find a mop by a language and a name
        /// </summary>
        /// <param name="language">language</param>
        /// <param name="name">name of the mop</param>
        /// <returns>mop object or throw an exception</returns>
        public CreateMOP GetMOP(string language, string name)
        {
            CreateMOP mop = this.mops.Find(new Predicate<CreateMOP>(delegate(CreateMOP c) { return c.Language == language && c.Name == name; }));
            if (mop != null)
                return mop;
            else
                throw new Exception("MOP '" + name + "' en '" + language + "' non trouvé");
        }

        /// <summary>
        /// Get an already registered injector
        /// </summary>
        /// <param name="map">the name of the injector (can be in and out a skeleton)</param>
        /// <returns>injector object or throw an exception</returns>
        public Injector GetInjector(string map)
        {
            Injector found = this.injectors.Find(new Predicate<Injector>(delegate(Injector i) { return i.LongName == map; }));
            if (found != null)
            {
                return found;
            }
            throw new Exception("Injecteur '" + map + "' non trouvé");
        }

        /// <summary>
        /// Push a process on a stack
        /// It's required for loaded process
        /// </summary>
        /// <param name="proc">process object</param>
        public void PushProcess(IProcess proc)
        {
            this.ProcessStack.Push(proc as Process);
        }

        /// <summary>
        /// Add a new process in the process list
        /// Pass a project interface reference and cast to instance of the implemented process class
        /// </summary>
        /// <param name="proc">process object</param>
        public void AddProcess(IProcess proc)
        {
            this.processes.Add(proc as Process);
        }

        /// <summary>
        /// Pop a process on the top of the stack
        /// It's required to quit loaded process
        /// </summary>
        public void PopProcess()
        {
            this.ProcessStack.Pop();
        }

        /// <summary>
        /// For conversion only
        /// Method to update parameters of a function
        /// Make a backtrack on parent process to assume mutable parameters
        /// </summary>
        /// <param name="converter">language converter</param>
        /// <param name="proc">current process instance</param>
        /// <param name="varName">variable name to check</param>
        /// <param name="mutable">mutable switch</param>
        public void UpdateParameters(ICodeConverter converter, IProcessInstance proc, string varName, bool mutable)
        {
            if (proc.CurrentScope.Exists(varName))
            {
                IData d = proc.CurrentScope.GetVariable(varName);
                if (!d.IsGlobal)
                {
                    if (d.BelongsTo != proc.Name)
                    {
                        this.BacktrackProcessesAndUpdateParameters(converter, proc, d, mutable);
                    }
                    else
                    {
                        Helper.AddIntoLocal(converter, d);
                    }
                }
            }
            // on ne traite aucune variable qui n'existe pas dans le scope
        }

        /// <summary>
        /// The parse function is just like a limited read (to improve performance) - not a complete loading
        /// To call just before the display function
        /// </summary>
        /// <param name="comp">the compiler object</param>
        /// <param name="node">the xml node to read</param>
        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            foreach (XmlNode elem in node.ChildNodes)
            {
                switch (elem.Name.ToLower())
                {
                    case "texte":
                        {
                            Texte t = new Texte();
                            t.Parse(comp, elem);
                            this.Objects.Add(t);
                            break;
                        }
                    case "br":
                        {
                            Br crLf = new Br();
                            crLf.Parse(comp, elem);
                            this.Objects.Add(crLf);
                            break;
                        }
                    case "variable":
                        {
                            Variabble var = new Variabble();
                            var.Parse(comp, elem);
                            this.Objects.Add(var);
                            break;
                        }
                    case "champ":
                        {
                            Champ ch = new Champ();
                            ch.Parse(comp, elem);
                            this.Objects.Add(ch);
                            break;
                        }
                    case "size":
                        {
                            Taille t = new Taille();
                            t.Parse(comp, elem);
                            this.Objects.Add(t);
                            break;
                        }
                    case "affectation":
                        {
                            Affectation aff = new Affectation();
                            aff.Parse(comp, elem);
                            this.Objects.Add(aff);
                            break;
                        }
                    case "affectationchaine":
                        {
                            AffectationChaine ach = new AffectationChaine();
                            ach.Parse(comp, elem);
                            this.Objects.Add(ach);
                            break;
                        }
                    case "affectationchamp":
                        {
                            AffectationChamp ac = new AffectationChamp();
                            ac.Parse(comp, elem);
                            this.Objects.Add(ac);
                            break;
                        }
                    case "condition":
                        {
                            Condition c = new Condition();
                            c.Parse(comp, elem);
                            this.Objects.Add(c);
                            break;
                        }
                    case "label":
                        {
                            Label l = new Label();
                            l.Parse(comp, elem);
                            this.Objects.Add(l);
                            break;
                        }
                    case "call":
                        {
                            Call c = new Call();
                            c.Parse(comp, elem);
                            this.Objects.Add(c);
                            break;
                        }
                    case "callskeleton":
                        {
                            CallSkeleton cs = new CallSkeleton();
                            cs.Parse(comp, elem);
                            this.Objects.Add(cs);
                            break;
                        }
                    case "beginprocess":
                        {
                            BeginProcess bp = new BeginProcess();
                            bp.Parse(comp, elem);
                            this.Objects.Add(bp);
                            break;
                        }
                    case "beginskeleton":
                        {
                            BeginSkeleton bs = new BeginSkeleton();
                            bs.Parse(comp, elem);
                            this.Objects.Add(bs);
                            this.Objects.Push();
                            Skeleton skel = new Skeleton(bs.Path, bs.Name);
                            skel.Objects = this.Objects.Current;
                            this.skeletons.Add(skel);
                            break;
                        }
                    case "endprocess":
                        {
                            EndProcess ep = new EndProcess();
                            ep.Parse(comp, elem);
                            this.Objects.Add(ep);
                            break;
                        }
                    case "endskeleton":
                        {
                            EndSkeleton es = new EndSkeleton();
                            es.Parse(comp, elem);
                            this.Objects.Pop();
                            this.Objects.Add(es);
                            break;
                        }
                    case "template":
                        {
                            Template temp = new Template();
                            temp.Parse(comp, elem);
                            this.Objects.Add(temp);
                            break;
                        }
                    case "usetemplate":
                        {
                            UseTemplate ut = new UseTemplate();
                            this.Objects.Add(ut);
                            this.Objects.Push();
                            ut.Parse(comp, elem);
                            this.Objects.Pop();
                            break;
                        }
                    case "handler":
                        {
                            Handler h = new Handler();
                            h.Parse(comp, elem);
                            this.Objects.Add(h);
                            break;
                        }
                    case "coding":
                        {
                            Coding code = new Coding();
                            this.Objects.Add(code);
                            this.Objects.Push();
                            code.Parse(comp, elem);
                            this.Objects.Pop();
                            break;
                        }
                    case "parallel":
                        {
                            Parallel par = new Parallel();
                            this.Objects.Add(par);
                            this.Objects.Push();
                            par.Parse(comp, elem);
                            this.Objects.Pop();
                            break;
                        }
                    case "defaultwriter":
                        {
                            DefaultWriter dw = new DefaultWriter();
                            this.Objects.Add(dw);
                            dw.Parse(comp, elem);
                            break;
                        }
                    case "createwriter":
                        {
                            CreateWriter cw = new CreateWriter();
                            this.Objects.Add(cw);
                            cw.Parse(comp, elem);
                            break;
                        }
                    case "syntax":
                        {
                            Syntax sy = new Syntax();
                            this.Objects.Add(sy);
                            sy.Parse(comp, elem);
                            break;
                        }
                    case "createmop":
                        {
                            CreateMOP mop = new CreateMOP();
                            this.Objects.Add(mop);
                            mop.Parse(comp, elem);
                            break;
                        }
                    case "injector":
                        {
                            Injector inj = new Injector();
                            this.Objects.Add(inj);
                            inj.Parse(comp, elem);
                            break;
                        }
                    case "usemop":
                        {
                            UseMOP um = new UseMOP();
                            this.Objects.Add(um);
                            um.Parse(comp, elem);
                            break;
                        }
                    case "indent":
                        {
                            Indent i = new Indent();
                            this.Objects.Add(i);
                            i.Parse(comp, elem);
                            break;
                        }
                    case "unindent":
                        {
                            Unindent ui = new Unindent();
                            this.Objects.Add(ui);
                            ui.Parse(comp, elem);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Debug function (not yet completly implemented)
        /// </summary>
        /// <param name="comp">this compiler</param>
        /// <param name="node">xml node</param>
        public void Debug(ICompilateur comp, System.Xml.XmlNode node)
        {
            foreach (XmlNode elem in node.ChildNodes)
            {
                // TODO : switch
            }
        }

        /// <summary>
        /// Loading xml node
        /// </summary>
        /// <param name="comp">this compiler</param>
        /// <param name="node">xml node</param>
        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            foreach (XmlNode elem in node.ChildNodes)
            {
                switch (elem.Name.ToLower())
                {
                    case "texte":
                        {
                            Texte t = new Texte();
                            t.Load(comp, elem);
                            this.CurrentProcess.AddObject(t);
                            break;
                        }
                    case "br":
                        {
                            Br crLf = new Br();
                            crLf.Load(comp, elem);
                            this.CurrentProcess.AddObject(crLf);
                            break;
                        }
                    case "variable":
                        {
                            Variabble var = new Variabble();
                            var.Load(comp, elem);
                            this.CurrentProcess.AddObject(var);
                            break;
                        }
                    case "champ":
                        {
                            Champ ch = new Champ();
                            ch.Load(comp, elem);
                            this.CurrentProcess.AddObject(ch);
                            break;
                        }
                    case "size":
                        {
                            Taille t = new Taille();
                            t.Load(comp, elem);
                            this.CurrentProcess.AddObject(t);
                            break;
                        }
                    case "affectation":
                        {
                            Affectation aff = new Affectation();
                            aff.Load(comp, elem);
                            this.CurrentProcess.AddObject(aff);
                            break;
                        }
                    case "affectationchaine":
                        {
                            AffectationChaine ach = new AffectationChaine();
                            ach.Load(comp, elem);
                            this.CurrentProcess.AddObject(ach);
                            break;
                        }
                    case "affectationchamp":
                        {
                            AffectationChamp ac = new AffectationChamp();
                            ac.Load(comp, elem);
                            this.CurrentProcess.AddObject(ac);
                            break;
                        }
                    case "condition":
                        {
                            Condition c = new Condition();
                            c.Load(comp, elem);
                            this.CurrentProcess.AddObject(c);
                            break;
                        }
                    case "label":
                        {
                            Label l = new Label();
                            l.Load(comp, elem);
                            this.CurrentProcess.AddObject(l);
                            break;
                        }
                    case "call":
                        {
                            Call c = new Call();
                            c.Load(comp, elem);
                            this.CurrentProcess.AddObject(c);
                            break;
                        }
                    case "callskeleton":
                        {
                            CallSkeleton cs = new CallSkeleton();
                            cs.Load(comp, elem);
                            this.CurrentProcess.AddObject(cs);
                            break;
                        }
                    case "beginprocess":
                        {
                            // empile le processus courant
                            this.ProcessStack.Push(this.CurrentProcess as Process);
                            // démarre un nouveau processus
                            BeginProcess bp = new BeginProcess();
                            bp.Load(comp, elem);
                            this.CurrentProcess.AddObject(bp);
                            Process p = new Process();
                            p.Name = ":" + bp.ProcessName;
                            this.CurrentProcess = p;
                            IProcess old = this.Processes.Find(new Predicate<IProcess>(delegate(IProcess iter) { return iter.Name == p.Name; }));
                            if (old != null) {
                                this.Processes.Remove(old);
                                old.Dispose();
                            }
                            this.Processes.Add(this.CurrentProcess as Process);
                            break;
                        }
                    case "beginskeleton":
                        {
                            // empile le process courant
                            this.ProcessStack.Push(this.CurrentProcess as Process);
                            // démarre un nouveau processus
                            BeginSkeleton bs = new BeginSkeleton();
                            bs.Load(comp, elem);
                            this.CurrentProcess.AddObject(bs);
                            Process p = new Process();
                            Skeleton skel = new Skeleton(bs.Path, bs.Name);
                            skel.Legendes = this.legendeDict;
                            skel.Process = p;
                            p.Name = "Skeleton:" + bs.Path + "/" + bs.Name;
                            this.CurrentProcess = p;
                            Skeleton old = this.Skeletons.Find(new Predicate<Skeleton>(delegate(Skeleton iter) { return iter.Path + "/" + iter.Name == skel.Path + "/" + skel.Name; }));
                            if (old != null)
                            {
                                this.Skeletons.Remove(old);
                            }
                            this.Skeletons.Add(skel);
                            break;
                        }
                    case "endprocess":
                        {
                            EndProcess ep = new EndProcess();
                            ep.Load(comp, elem);
                            this.CurrentProcess.AddObject(ep);
                            if (this.ProcessStack.Count > 0)
                            {
                                IProcess previousProcess = this.ProcessStack.Peek();
                                if (this.CurrentProcess.Name == ":" + ep.ProcessName)
                                {
                                    this.ProcessStack.Pop();
                                    this.CurrentProcess = previousProcess;
                                }
                                else
                                    throw new Exception("Le processus actif (" + this.CurrentProcess.Name + ") n'a pas le même nom que ':" + ep.ProcessName + "'");
                            }
                            else
                                throw new Exception("Il n'y a pas de processus en cours pour terminer '" + ep.ProcessName + "'");
                            break;
                        }
                    case "endskeleton":
                        {
                            EndSkeleton es = new EndSkeleton();
                            es.Load(comp, elem);
                            this.CurrentProcess.AddObject(es);
                            if (this.ProcessStack.Count > 0)
                            {
                                IProcess previousProcess = this.ProcessStack.Peek();
                                if (this.CurrentProcess.Name == "Skeleton:" + es.Path + "/" + es.Name)
                                {
                                    this.ProcessStack.Pop();
                                    this.CurrentProcess = previousProcess;
                                }
                                else
                                    throw new Exception("Le processus actif (" + previousProcess.Name + ") n'a pas le même nom que '" + es.Path + "/" + es.Name + "'");
                            }
                            else
                                throw new Exception("Il n'y a pas de squelette en cours pour terminer '" + es.Path + "/" + es.Name + "'");
                            break;
                        }
                    case "template":
                        {
                            Template temp = new Template();
                            temp.Load(comp, elem);
                            this.Templates.Add(temp);
                            break;
                        }
                    case "usetemplate":
                        {
                            UseTemplate ut = new UseTemplate();
                            // uniquement pour l'extraction du dictionnaire et la conversion du code et avant de charger la suite
                            this.CurrentProcess.AddObject(ut);
                            ut.Load(comp, elem);
                            break;
                        }
                    case "handler":
                        {
                            Handler h = new Handler();
                            h.Load(comp, elem);
                            if (this.CurrentUsingTemplate != null)
                            {
                                Coding code = this.CurrentUsingTemplate.GetCoding(h.HandlerName);
                                if (code != null)
                                    this.Load(comp, code.XmlCode);
                            }
                            else
                                throw new Exception("Aucune utilisation d'un template n'est en cours pour déclarer un événement");
                            break;
                        }
                    case "coding":
                        {
                            if (this.CurrentUsingTemplate != null)
                            {
                                Coding code = new Coding();
                                code.Load(comp, elem);
                                // nécessaire pour la conversion de code
                                this.CurrentProcess.AddObject(code);
                                this.CurrentUsingTemplate.AddCoding(code);
                                // ajouté pour convertir le code
                                if (this.UnderConversion)
                                {
                                    Process proc = new Process();
                                    proc.Name = "Coding:" + code.UniqueCodingName;
                                    this.Processes.Add(proc);
                                    this.ProcessStack.Push(this.CurrentProcess);
                                    this.CurrentProcess = proc;
                                    this.Load(comp, code.XmlCode);
                                    this.CurrentProcess = this.ProcessStack.Pop();
                                }
                            }
                            else
                                throw new Exception("Aucune utilisation d'un template n'est en cours pour déclarer un code");
                            break;
                        }
                    case "parallel":
                        {
                            Parallel par = new Parallel();
                            par.Load(comp, elem);
                            this.CurrentProcess.AddObject(par);
                            // ajout pour convertir le code
                            if (this.UnderConversion)
                            {
                                Process proc = new Process();
                                proc.Name = "Parallel:" + this.currentProcess.Name;
                                this.processes.Add(proc);
                                this.ProcessStack.Push(this.currentProcess);
                                this.CurrentProcess = proc;
                                this.Load(comp, par.XmlCode);
                                this.CurrentProcess = this.ProcessStack.Pop();
                            }
                            break;
                        }
                    case "defaultwriter":
                        {
                            DefaultWriter dw = new DefaultWriter();
                            dw.Load(comp, elem);
                            this.CurrentProcess.AddObject(dw);
                            break;
                        }
                    case "createwriter":
                        {
                            CreateWriter cw = new CreateWriter();
                            cw.Load(comp, elem);
                            this.CurrentProcess.AddObject(cw);
                            break;
                        }
                    case "locale":
                        {
                            Locale loc = new Locale();
                            loc.Load(comp, elem);
                            this.CurrentProcess.AddObject(loc);
                            break;
                        }
                    case "syntax":
                        {
                            Syntax syn = new Syntax();
                            syn.Load(comp, elem);
                            // just one element with same name
                            Syntax old = this.Syntaxes.Find(new Predicate<Syntax>(delegate(Syntax s) { return s.Name == syn.Name; }));
                            if (old != null)
                                this.Syntaxes.Remove(old);
                            this.Syntaxes.Add(syn);
                            break;
                        }
                    case "createmop":
                        {
                            CreateMOP mop = new CreateMOP();
                            mop.Load(comp, elem);
                            // injection du mop pendant la conversion
                            if (this.UnderConversion)
                            {
                                this.CurrentProcess.AddObject(mop);
                            }
                            // add the mop ; replace if already exists
                            CreateMOP old = this.Mops.Find(new Predicate<CreateMOP>(delegate(CreateMOP m)
                            {
                                return m.Language == mop.Language && m.Name == mop.Name;
                            }));
                            if (old != null)
                                this.Mops.Remove(old);
                            this.Mops.Add(mop);
                            break;
                        }
                    case "injector":
                        {
                            Injector inj = new Injector();
                            // insertion de l'objet avant le load pour injector qui doit récupérer sa position
                            this.CurrentProcess.AddObject(inj);
                            inj.Load(comp, elem);
                            // just the last element with same name
                            Injector old = this.Injectors.Find(new Predicate<Injector>(delegate(Injector i) { return i.LongName == inj.LongName; }));
                            if (old != null)
                                this.Injectors.Remove(old);
                            this.Injectors.Add(inj);
                            break;
                        }
                    case "usemop":
                        {
                            UseMOP um = new UseMOP();
                            um.Load(comp, elem);
                            this.CurrentProcess.AddObject(um);
                            break;
                        }
                    case "indent":
                        {
                            Indent i = new Indent();
                            i.Load(comp, elem);
                            this.CurrentProcess.AddObject(i);
                            break;
                        }
                    case "unindent":
                        {
                            Unindent ui = new Unindent();
                            ui.Load(comp, elem);
                            this.CurrentProcess.AddObject(ui);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Internal function saving html element from the web navigator
        /// </summary>
        /// <param name="writer">Xml writer</param>
        /// <param name="child">next child to save</param>
        internal void SaveElement(XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            string type = child.GetAttribute("type");
            switch (type)
            {
                case "texte":
                    {
                        Texte t = new Texte();
                        t.Update(child);
                        t.Save(this, writer, ref child);
                        break;
                    }
                case "affectation":
                    {
                        Affectation aff = new Affectation();
                        aff.Update(child);
                        aff.Save(this, writer, ref child);
                        break;
                    }
                case "affectationChaine":
                    {
                        AffectationChaine ach = new AffectationChaine();
                        ach.Update(child);
                        ach.Save(this, writer, ref child);
                        break;
                    }
                case "affectationChamp":
                    {
                        AffectationChamp affChamp = new AffectationChamp();
                        affChamp.Update(child);
                        affChamp.Save(this, writer, ref child);
                        break;
                    }
                case "beginProcess":
                    {
                        BeginProcess bp = new BeginProcess();
                        bp.Update(child);
                        bp.Save(this, writer, ref child);
                        break;
                    }
                case "beginSkeleton":
                    {
                        BeginSkeleton bs = new BeginSkeleton();
                        bs.Update(child);
                        bs.Save(this, writer, ref child);
                        break;
                    }
                case "call":
                    {
                        Call c = new Call();
                        c.Update(child);
                        c.Save(this, writer, ref child);
                        break;
                    }
                case "callSkeleton":
                    {
                        CallSkeleton cs = new CallSkeleton();
                        cs.Update(child);
                        cs.Save(this, writer, ref child);
                        break;
                    }
                case "champ":
                    {
                        Champ ch = new Champ();
                        ch.Update(child);
                        ch.Save(this, writer, ref child);
                        break;
                    }
                case "coding":
                    {
                        Coding cod = new Coding();
                        cod.Update(child);
                        cod.Save(this, writer, ref child);
                        break;
                    }
                case "condition":
                    {
                        Condition cond = new Condition();
                        cond.Update(child);
                        cond.Save(this, writer, ref child);
                        break;
                    }
                case "endProcess":
                    {
                        EndProcess ep = new EndProcess();
                        ep.Update(child);
                        ep.Save(this, writer, ref child);
                        break;
                    }
                case "endSkeleton":
                    {
                        EndSkeleton es = new EndSkeleton();
                        es.Update(child);
                        es.Save(this, writer, ref child);
                        break;
                    }
                case "handler":
                    {
                        Handler h = new Handler();
                        h.Update(child);
                        h.Save(this, writer, ref child);
                        break;
                    }
                case "label":
                    {
                        Label l = new Label();
                        l.Update(child);
                        l.Save(this, writer, ref child);
                        break;
                    }
                case "size":
                    {
                        Taille t = new Taille();
                        t.Update(child);
                        t.Save(this, writer, ref child);
                        break;
                    }
                case "template":
                    {
                        Template t = new Template();
                        t.Update(child);
                        t.Save(this, writer, ref child);
                        break;
                    }
                case "usetemplate":
                    {
                        UseTemplate ut = new UseTemplate();
                        ut.Update(child);
                        ut.Save(this, writer, ref child);
                        break;
                    }
                case "variable":
                    {
                        Variabble var = new Variabble();
                        var.Update(child);
                        var.Save(this, writer, ref child);
                        break;
                    }
                case "parallel":
                    {
                        Parallel par = new Parallel();
                        par.Update(child);
                        par.Save(this, writer, ref child);
                        break;
                    }
                case "useWriter":
                    {
                        DefaultWriter dw = new DefaultWriter();
                        dw.Update(child);
                        dw.Save(this, writer, ref child);
                        break;
                    }
                case "createWriter":
                    {
                        CreateWriter cw = new CreateWriter();
                        cw.Update(child);
                        cw.Save(this, writer, ref child);
                        break;
                    }
                case "syntax":
                    {
                        Syntax s = new Syntax();
                        s.Update(child);
                        s.Save(this, writer, ref child);
                        break;
                    }
                case "createmop":
                    {
                        CreateMOP mop = new CreateMOP();
                        mop.Update(child);
                        mop.Save(this, writer, ref child);
                        break;
                    }
                case "injector":
                    {
                        Injector inj = new Injector();
                        inj.Update(child);
                        inj.Save(this, writer, ref child);
                        break;
                    }
                case "usemop":
                    {
                        UseMOP um = new UseMOP();
                        um.Update(child);
                        um.Save(this, writer, ref child);
                        break;
                    }
                case "indent":
                    {
                        Indent i = new Indent();
                        i.Update(child);
                        i.Save(this, writer, ref child);
                        break;
                    }
                case "unindent":
                    {
                        Unindent ui = new Unindent();
                        ui.Update(child);
                        ui.Save(this, writer, ref child);
                        break;
                    }
                default:
                    {
                        child = child.NextSibling;
                        break;
                    }
            }
        }

        /// <summary>
        /// Search for one or more templates by name
        /// The search is intended for the template list box in the web navigator
        /// </summary>
        /// <param name="searchString">template name to search</param>
        /// <returns>a list of templates</returns>
        public List<Template> SearchTemplate(string searchString)
        {
            List<Template> list = new List<Template>();
            string searchLower = searchString.ToLower();
            foreach (Template t in this.templates)
            {
                string path = t.Path.ToLower();
                string name = t.Name.ToLower();
                if (path.Contains(searchLower) || name.Contains(searchLower) || (path + "/" + name).Contains(searchLower))
                {
                    list.Add(t);
                }
            }
            return list;
        }

        /// <summary>
        /// Search for one or more syntax by name
        /// The search is intended for the syntax list box in the web navigator
        /// </summary>
        /// <param name="searchString">syntax name to search</param>
        /// <returns>a list of syntax</returns>
        public List<Syntax> SearchSyntax(string searchString)
        {
            List<Syntax> list = new List<Syntax>();
            string searchLower = searchString.ToLower();
            string[] splitSearch = searchLower.Split(' ');
            string regExSearch = @".*";
            bool first = true;
            foreach (string search in splitSearch)
            {
                if (!String.IsNullOrEmpty(search))
                {
                    if (first)
                    {
                        regExSearch = Regex.Escape(search) + @"\s*";
                        first = false;
                    }
                    else
                    {
                        Regex reg = new Regex("(([a-zA-Z]+)[^a-zA-Z]*)+");
                        string value = reg.Match(search).Groups[2].Value;
                        regExSearch += Regex.Escape(value) + @"[^a-zA-Z]*";
                    }
                }
            }
            foreach (Syntax syn in this.syntaxes)
            {
                string name = syn.Name.ToLower();
                Regex reg = new Regex(regExSearch, RegexOptions.IgnoreCase);
                if (reg.IsMatch(name))
                {
                    list.Add(syn);
                }
            }
            return list;
        }

        /// <summary>
        /// Search for one or more skeletons by name
        /// The search is intended for the skeletons list box in the web navigator
        /// </summary>
        /// <param name="searchString">skeleton name to search</param>
        /// <returns>a list of skeletons</returns>
        public List<Skeleton> SearchSkeleton(string searchString)
        {
            List<Skeleton> list = new List<Skeleton>();
            string searchLower = searchString.ToLower();
            foreach (Skeleton skel in this.skeletons)
            {
                string path = skel.Path;
                string name = skel.Name;
                if (path.Contains(searchLower) || name.Contains(searchLower) || (path + "/" + name).Contains(searchLower))
                {
                    list.Add(skel);
                }
            }
            return list;
        }

        #endregion
 
        #region ICompilateur Membres
        /// <summary>
        /// Save childs objects to an xml writer
        /// </summary>
        /// <param name="writer">xml writer object</param>
        /// <param name="elem">last html element to continue</param>
        /// <param name="currentIndent">current indentation</param>
        public void Save(XmlWriter writer, ref System.Windows.Forms.HtmlElement elem, int currentIndent)
        {
            while (elem != null)
            {
                Compilateur.NotifyProgress(1, elem.Parent.Children.Count);
                Compilateur.SetProgressText(String.Format("Enregistrement {0}", elem.GetAttribute("type")));
                if (this.copyElements)
                {
                    if (elem.Id == this.from)
                    {
                        this.startRecording = true;
                    }
                    if (this.startRecording)
                    {
                        string type = elem.GetAttribute("type");
                        int indent = 0;
                        Int32.TryParse(elem.GetAttribute("indent"), out indent);
                        if (type != "paste")
                        {
                            if (currentIndent == 0 || (currentIndent > 0 && indent >= currentIndent))
                            {
                                this.SaveElement(writer, ref elem);
                                if (elem != null)
                                {
                                    if (elem.Id == this.to)
                                    {
                                        this.startRecording = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            elem = elem.NextSibling;
                        }
                    }
                    else
                    {
                        elem = elem.NextSibling;
                    }
                }
                else
                {
                    string type = elem.GetAttribute("type");
                    int indent = 0;
                    Int32.TryParse(elem.GetAttribute("indent"), out indent);
                    if (type != "paste")
                    {
                        if (currentIndent == 0 || (currentIndent > 0 && indent >= currentIndent))
                        {
                            this.SaveElement(writer, ref elem);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        elem = elem.NextSibling;
                    }
                }
            }
        }

        /// <summary>
        /// Save childs objects to an xml writer
        /// </summary>
        /// <param name="writer">xml writer object</param>
        /// <param name="elem">html element to save</param>
        /// <param name="from">html unique id to start saving</param>
        /// <param name="to">html unique id to stop saving</param>
        public void Save(XmlWriter writer, System.Windows.Forms.HtmlElement elem, string from, string to)
        {
            this.copyElements = true;
            this.startRecording = false;
            this.from = from;
            this.to = to;
            System.Windows.Forms.HtmlElement child = elem.FirstChild;
            this.Save(writer, ref child, 0);
            this.copyElements = false;
        }

        /// <summary>
        /// Save objects from the web navigator
        /// </summary>
        /// <param name="fileDestination">file where to write</param>
        /// <param name="elem">html element</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        /// <param name="infos">header infos</param>
        public void Save(string fileDestination, System.Windows.Forms.HtmlElement elem, INotifyProgress wait, params string[] infos)
        {
            Compilateur.StartProgress(false, wait);
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                XmlWriter xmlTW = XmlWriter.Create(fileDestination, settings);
                xmlTW.WriteComment("This is a CodeCommander source file");
                xmlTW.WriteStartElement("CodeCommander");
                xmlTW.WriteAttributeString("version", o2Mate.Version.GetSoftwareVersion().ToString());
                xmlTW.WriteStartElement("legendes");
                this.legendeDict.Save(xmlTW);
                xmlTW.WriteEndElement();
                xmlTW.WriteStartElement("infos");
                xmlTW.WriteElementString("creationDate", infos[1]);
                xmlTW.WriteElementString("modificationDate", infos[2]);
                xmlTW.WriteElementString("revision", infos[3]);
                xmlTW.WriteEndElement();
                xmlTW.WriteStartElement("code");
                this.copyElements = false;
                System.Windows.Forms.HtmlElement child = elem.FirstChild;
                this.Save(xmlTW, ref child, 0);
                xmlTW.WriteEndElement();
                xmlTW.WriteEndElement();
                xmlTW.Close();
                Compilateur.TerminateProgress(true);
            }
            catch (Exception ex)
            {
                if (progress != null)
                    Compilateur.progress.GiveException(ex);
                Compilateur.TerminateProgress(false);
            }
        }

        /// <summary>
        /// Save xml string into a CodeCommander file source (an xml file)
        /// </summary>
        /// <param name="fileDestination">destination file name</param>
        /// <param name="xmlCode">xml string to persist</param>
        /// <param name="legendes">summary data</param>
        public void Save(string fileDestination, string xmlCode, ILegendeDict legendes)
        {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                XmlWriter xmlTW = XmlWriter.Create(fileDestination, settings);
                xmlTW.WriteComment("This is a CodeCommander source file");
                xmlTW.WriteStartElement("CodeCommander");
                xmlTW.WriteAttributeString("version", o2Mate.Version.GetSoftwareVersion().ToString());
            xmlTW.WriteStartElement("legendes");
            legendes.Save(xmlTW);
            xmlTW.WriteEndElement();
            xmlTW.WriteStartElement("code");
            xmlTW.WriteRaw(xmlCode);
            xmlTW.WriteEndElement();
            xmlTW.WriteEndElement();
            xmlTW.Close();
        }

        /// <summary>
        /// Save objects from the web navigator
        /// </summary>
        /// <param name="elem">html element</param>
        /// <returns>the xml string</returns>
        public string SaveToString(System.Windows.Forms.HtmlElement elem)
        {
            byte[] dataUtf8 = null;
            using (StringWriter sw = new StringWriter())
            {
                XmlTextWriter xmlTW = new XmlTextWriter(sw);
                this.copyElements = false;
                System.Windows.Forms.HtmlElement child = elem.FirstChild;
                this.Save(xmlTW, ref child, 0);
                xmlTW.Close();
                // convert UTF-16 to UTF-8
                byte[] dataUnicode = Encoding.Unicode.GetBytes(sw.ToString());
                dataUtf8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, dataUnicode);
            }
            return Encoding.UTF8.GetString(dataUtf8);
        }

        /// <summary>
        /// Save child objects from the web navigator
        /// </summary>
        /// <param name="elem">html element</param>
        /// <returns>the xml string</returns>
        public string SaveChildToString(System.Windows.Forms.HtmlElement elem)
        {
            byte[] dataUtf8 = null;
            using (StringWriter sw = new StringWriter())
            {
                XmlTextWriter xmlTW = new XmlTextWriter(sw);
                this.SaveElement(xmlTW, ref elem);
                xmlTW.Close();
                // convert UTF-16 to UTF-8
                byte[] dataUnicode = Encoding.Unicode.GetBytes(sw.ToString());
                dataUtf8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, dataUnicode);
            }
            return Encoding.UTF8.GetString(dataUtf8);
        }

        private void ThreadSafeDisplayXml(Node<ICompiler> node, BindingList<DisplayElement> list, bool forcedIndent, int indent, bool writable)
        {
            if (writable)
            {
                node.Object.Display(list, node, forcedIndent, ref indent);
            }
            else
            {
                node.Object.DisplayReadOnly(list, node, forcedIndent, ref indent);
            }
        }

        /// <summary>
        /// Displays each statements stored in a string into the web navigator
        /// </summary>
        /// <param name="xmlString">xml string to display</param>
        /// <param name="list">list to store operation</param>
        /// <param name="forcedIndent">true if force a number at indentation</param>
        /// <param name="indent">number at indentation</param>
        /// <param name="writable">displays objects editable or readonly</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        public void DisplayXML(string xmlString, BindingList<DisplayElement> list, bool forcedIndent, int indent, bool writable, INotifyProgress wait)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml("<code>" + xmlString + "</code>");
            this.Parse(this, doc.DocumentElement);
            foreach (Node<ICompiler> node in this.Objects)
            {
                this.ThreadSafeDisplayXml(node, list, forcedIndent, indent, writable);
            }
            this.ThreadSafeEndDisplayXml(list);
        }

        /// <summary>
        /// Displays a skeleton (in read only html object)
        /// </summary>
        /// <param name="skeleton">skeleton to display</param>
        /// <param name="list">list of objects to display</param>
        public void DisplaySkeleton(Skeleton skeleton, BindingList<DisplayElement> list)
        {
            foreach (Node<ICompiler> node in skeleton.Objects)
            {
                this.ThreadSafeDisplayObjects(node, list, false);
            }
            this.ThreadSafeEndDisplay(list);
        }

        private void ThreadSafeDisplay(Node<ICompiler> node, BindingList<DisplayElement> list, bool writable)
        {
            int indent = 0;
            if (writable)
            {
                node.Object.Display(list, node, false, ref indent);
            }
            else
            {
                node.Object.DisplayReadOnly(list, node, false, ref indent);
            }
        }

        private void ThreadSafeEndDisplay(BindingList<DisplayElement> list)
        {
            list.Add(new DisplayElement("", "writePasteAtEnd"));
        }

        private void ThreadSafeEndDisplayXml(BindingList<DisplayElement> list)
        {
            list.Add(new DisplayElement("", "endWithPaste"));
        }

        private void ThreadSafeDisplayObjects(Node<ICompiler> node, BindingList<DisplayElement> list, bool writable)
        {
            int indent = 0;
            if (writable)
            {
                node.Object.Display(list, node, false, ref indent);
            }
            else
            {
                node.Object.DisplayReadOnly(list, node, false, ref indent);
            }
        }

        private void ThreadSafeDisplayHeader(BindingList<DisplayElement> list, string fileSource, string creationDate, string modificationDate, string revision)
        {
            list.Add(new DisplayElement("", "openHeader", new object[] { System.IO.Path.GetFileNameWithoutExtension(fileSource), 
                creationDate,
                modificationDate,
                revision}));
        }

        /// <summary>
        /// Displays a complete statements stored in a file into the web navigator
        /// </summary>
        /// <param name="fileSource">CodeCommander file name to display</param>
        /// <param name="list">list to store operation</param>
        /// <param name="writable">displays objects editable or readonly</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        public void Display(string fileSource, BindingList<DisplayElement> list, bool writable, INotifyProgress wait)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load(fileSource);
            this.ThreadSafeDisplayHeader(list, fileSource,
                doc.DocumentElement.SelectSingleNode("infos/creationDate").InnerText,
                doc.DocumentElement.SelectSingleNode("infos/modificationDate").InnerText,
                doc.DocumentElement.SelectSingleNode("infos/revision").InnerText);
            this.Parse(this, doc.DocumentElement.SelectSingleNode("code"));
            foreach (Node<ICompiler> node in this.Objects)
            {
                this.ThreadSafeDisplay(node, list, writable);
            }
            this.ThreadSafeEndDisplay(list);
        }

        /// <summary>
        /// Get header from a CodeCommander file source
        /// </summary>
        /// <param name="fileSource">CodeCommander file source</param>
        /// <param name="creationDate">returns the creation date</param>
        /// <param name="modificationDate">returns the modification date</param>
        /// <param name="revision">returns the revision date</param>
        public void GetHeader(string fileSource, out string creationDate, out string modificationDate, out string revision)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load(fileSource);
            creationDate = doc.DocumentElement.SelectSingleNode("infos/creationDate").InnerText;
            modificationDate = doc.DocumentElement.SelectSingleNode("infos/modificationDate").InnerText;
            revision = doc.DocumentElement.SelectSingleNode("infos/revision").InnerText;
        }

        /// <summary>
        /// Function to construct a complete dictionary object
        /// by the CodeCommander file source
        /// </summary>
        /// <param name="fileSource">file name source</param>
        /// <returns>a dictionary object</returns>
        public Dictionnaire OutputDictionary(string fileSource)
        {
            this.legendeDict = new LegendeDict();
            Dictionnaire dict = new Dictionnaire();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load(fileSource);
            this.MakeOneInstance(new RunInnerInstanceCompiler(delegate(ICompilateurInstance c)
            {
                c.CurrentDictionnaire = dict;
                this.CurrentProcess = this.principal;
                this.Load(c, doc.DocumentElement.SelectSingleNode("code"));
                c.CurrentDictionnaire.Legendes.Load(doc.DocumentElement.SelectSingleNode("legendes"));
                c.ExtractDictionary(this.principal);
            }));
            return dict;
        }

        /// <summary>
        /// Function to construct a list of summary object
        /// by the CodeCommander file source
        /// </summary>
        /// <param name="fileSource">file name source</param>
        /// <returns>a summary object</returns>
        public ILegendeDict OutputLegendes(string fileSource)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load(fileSource);
            this.legendeDict.Clear();
            this.legendeDict.Load(doc.DocumentElement.SelectSingleNode("legendes"));
            return this.legendeDict;
        }

        /// <summary>
        /// Summary has been modified from the web navigator
        /// This method keeps the summary in internal
        /// </summary>
        /// <param name="elem">html element</param>
        public void InputLegendes(System.Windows.Forms.HtmlElement elem)
        {
            this.legendeDict.Update(elem);
        }

        /// <summary>
        /// The method compiler
        /// </summary>
        /// <param name="fileSource">CodeCommander file source</param>
        /// <param name="fileData">dictionary file</param>
        /// <param name="finalFile">final file</param>
        /// <param name="destEncoding">final file encoding</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        public void Compilation(string fileSource, string fileData, string finalFile, string destEncoding, INotifyProgress wait)
        {
            try
            {
                this.EncodedFile = Encoding.GetEncoding(destEncoding);
            }
            catch { }
            this.Compilation(fileSource, fileData, finalFile, wait);
        }

        /// <summary>
        /// The method compiler
        /// </summary>
        /// <param name="fileSource">CodeCommander file source</param>
        /// <param name="fileData">dictionary file</param>
        /// <param name="finalFile">final file</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        public void Compilation(string fileSource, string fileData, string finalFile, INotifyProgress wait)
        {
            if (wait != null)
            {
                Compilateur.StartProgress(false, wait);
            }
            try
            {
                this.UnderConversion = false;
                Dictionnaire dict = new Dictionnaire();
                dict.Load(fileData);
                FinalFile final = new FinalFile();
                final.FileName = finalFile;
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.Load(fileSource);
                this.Threads.Add(new ProjectItem(Compilateur.RootProcessName, 0, "thread"));
                this.principal.CurrentProject = this.Threads;
                final.Start();
                this.MakeNewInstance(null, new RunInnerInstanceCompiler(delegate(ICompilateurInstance c)
                {
                    c.CurrentDictionnaire = dict;
                    c.CurrentProcess = this.principal;
                    this.Load(c, doc.DocumentElement.SelectSingleNode("code"));
                    IData writer = c.CurrentScope.Add(Compilateur.InitialWriter, "{\"" + finalFile + "\",0,true}", Variable.globalName, false, EnumDataType.E_WRITER);
                    c.WriteToFile(this.principal, final);

                }));
                if (wait != null)
                {
                    Compilateur.TerminateProgress(true);
                }
            }
            catch (Exception ex)
            {
                if (wait != null)
                {
                    Compilateur.progress.GiveException(ex);
                    Compilateur.TerminateProgress(false);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// The method for debugging
        /// </summary>
        /// <param name="fileSource">CodeCommander file source</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        public void Debug(string fileSource, INotifyProgress wait)
        {
            if (wait != null)
            {
                Compilateur.StartProgress(false, wait);
            }
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = false;
                doc.Load(fileSource);
                this.Threads.Add(new ProjectItem(Compilateur.RootProcessName, 0, "thread"));
                this.principal.CurrentProject = this.Threads;
                this.Debug(this, doc.DocumentElement.SelectSingleNode("code"));
                if (wait != null)
                {
                    Compilateur.TerminateProgress(true);
                }
            }
            catch (Exception ex)
            {
                if (wait != null)
                {
                    Compilateur.progress.GiveException(ex);
                    Compilateur.TerminateProgress(false);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// The method compiler (use this method when the CodeCommander file source has been loaded either)
        /// </summary>
        /// <param name="fileData">dictionary file</param>
        /// <param name="finalFile">final file</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        public void Compilation(string fileData, string finalFile, INotifyProgress wait)
        {
            if (wait != null)
            {
                Compilateur.StartProgress(false, wait);
            }
            try
            {
                this.UnderConversion = false;
                Dictionnaire dict = new Dictionnaire();
                dict.Load(fileData);
                FinalFile final = new FinalFile();
                final.FileName = finalFile;
                this.Threads.Add(new ProjectItem(Compilateur.RootProcessName, 0, "thread"));
                this.principal.CurrentProject = this.Threads;
                final.Start();
                this.MakeNewInstance(null, new RunInnerInstanceCompiler(delegate(ICompilateurInstance c)
                {
                    c.CurrentDictionnaire = dict;
                    c.CurrentProcess = this.principal;
                    IData writer = c.CurrentScope.Add(Compilateur.InitialWriter, "{\"" + finalFile + "\",0,true}", Variable.globalName, false, EnumDataType.E_WRITER);
                    c.WriteToFile(this.principal, final);

                }));
                if (wait != null)
                {
                    Compilateur.TerminateProgress(true);
                }
            }
            catch (Exception ex)
            {
                if (wait != null)
                {
                    Compilateur.progress.GiveException(ex);
                    Compilateur.TerminateProgress(false);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// The method for debugging
        /// </summary>
        /// <param name="fileData">dictionary file</param>
        /// <param name="finalFile">final file</param>
        /// <param name="wait">notification object (to display a progress bar)</param>
        public void Debug(string fileData, string finalFile, INotifyProgress wait)
        {
            if (wait != null)
            {
                Compilateur.StartProgress(false, wait);
            }
            try
            {
                this.UnderConversion = false;
                Dictionnaire dict = new Dictionnaire();
                dict.Load(fileData);
                FinalFile final = new FinalFile();
                final.FileName = finalFile;
                this.Threads.Add(new ProjectItem(Compilateur.RootProcessName, 0, "thread"));
                this.principal.CurrentProject = this.Threads;
                final.Start();
                this.MakeNewInstance(null, new RunInnerInstanceCompiler(delegate(ICompilateurInstance c)
                {
                    c.CurrentDictionnaire = dict;
                    c.CurrentProcess = this.principal;
                    IData writer = c.CurrentScope.Add(Compilateur.InitialWriter, "{\"" + finalFile + "\",0,true}", Variable.globalName, false, EnumDataType.E_WRITER);
                    c.WriteToFile(this.principal, final);
                }));
                if (wait != null)
                {
                    Compilateur.TerminateProgress(true);
                }
            }
            catch (Exception ex)
            {
                if (wait != null)
                {
                    Compilateur.progress.GiveException(ex);
                    Compilateur.TerminateProgress(false);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Converts a CodeCommander source file into a programming language
        /// </summary>
        /// <param name="converter">a converter object</param>
        /// <param name="fileSource">CodeCommander file source</param>
        /// <param name="fileData">dictionary file data</param>
        /// <param name="finalFile">final file (used to set the path output onto the converted source code)</param>
        public void Convert(ICodeConverter converter, string fileSource, string fileData, string finalFile)
        {
            this.UnderConversion = true;
            this.convertedLanguage = converter;
            Dictionnaire dict = new Dictionnaire();
            dict.Load(fileData);
            FinalFile final = new FinalFile();
            final.FileName = finalFile;
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load(fileSource);
            this.CurrentProcess = this.principal;
            converter.FunctionsFileName = "functions_" + Path.GetFileName(finalFile);
            converter.PrincipalFileName = finalFile;
            final.Start();
            this.principal.FunctionName = converter.ProcessAsFunction;
            // ajouter la fonction main dans les fonctions implémentées
            converter.ImplementedFunctions.Add(converter.Main);
            this.MakeOneInstance(new RunInnerInstanceCompiler(delegate(ICompilateurInstance c)
            {
                c.CurrentDictionnaire = dict;
                this.Load(c, doc.DocumentElement.SelectSingleNode("code"));
                IData writer = c.CurrentScope.Add(Compilateur.InitialWriter, "{\"" + finalFile + "\",0,true}", Variable.globalName, false, EnumDataType.E_WRITER);
                c.Convert(converter, this.principal, final);
            }));
            converter.WriteToFile(fileData, finalFile);
        }

        /// <summary>
        /// Says if it's a non recursive call
        /// </summary>
        /// <param name="processName">name of the process</param>
        /// <returns>true if is not a recursively call</returns>
        public bool CheckNonRecursiveMode(string processName)
        {
            int one = this.ProcessStack.Count(new Func<IProcess, bool>(delegate(IProcess p)
            {
                return p.Name == processName;
            }));
            return (one == 0);
        }

        /// <summary>
        /// Conversion mode
        /// </summary>
        /// <param name="run">delegate to launch</param>
        /// <returns>the instance number</returns>
        public int MakeOneInstance(RunInnerInstanceCompiler run)
        {
            int selectedInstance = this.instances.Count;
            if (selectedInstance == 0)
            {
                CompilateurState cs = new CompilateurState(this, selectedInstance, null, true);
                this.instances.Add(cs);
                try
                {
                    run(cs);
                }
                catch { throw; }
                finally { cs.Unlock(); }
            }
            else if (selectedInstance == 1)
            {
                try
                {
                    if (this.instances[selectedInstance - 1].TestAndSetLock())
                    {
                        run(this.instances[selectedInstance - 1]);
                    }
                    else
                        throw new InvalidProgramException("L'unique instance du compilateur est déjà en cours d'utilisation");
                }
                catch { throw; }
                finally { this.instances[selectedInstance - 1].Unlock(); }
            }
            else
            {
                throw new InvalidProgramException("Le nombre d'instances (" + selectedInstance.ToString() + ") du compilateur est supérieur à 1.");
            }
            return selectedInstance;
        }

        /// <summary>
        /// Make a new instance of the compiler with internal values
        /// </summary>
        /// <param name="previous">previous instance</param>
        /// <param name="run">delegate</param>
        /// <returns>instance number</returns>
        public int MakeNewInstance(ICompilateurInstance previous, RunInnerInstanceCompiler run)
        {
            int selectedInstance = -1;
            for (int number = this.instances.Count - 1; number >= 0; --number)
            {
                if (this.instances[number].TestAndSetLock())
                {
                    selectedInstance = number;
                    try
                    {
                        this.instances[selectedInstance] = new CompilateurState(this, selectedInstance, previous, true);
                        run(this.instances[selectedInstance]);
                    }
                    catch { throw; }
                    finally { this.instances[selectedInstance].Unlock(); }
                    break;
                }
            }
            if (selectedInstance == -1)
            {
                selectedInstance = this.instances.Count;
                CompilateurState cs = new CompilateurState(this, selectedInstance, previous, true);
                this.instances.Add(cs);
                try
                {
                    run(cs);
                }
                catch { throw; }
                finally { cs.Unlock(); }
            }
            return selectedInstance;
        }

        /// <summary>
        /// Load all files in a directory (assumes to contain templates)
        /// </summary>
        /// <param name="directory">directory path</param>
        public void LoadTemplates(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            if (di.Exists)
                this.LoadTemplates(di);
        }

        /// <summary>
        /// Load all files in a directory (assumes to contain syntax)
        /// </summary>
        /// <param name="directory">directory path</param>
        public void LoadSyntax(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            if (di.Exists)
                this.LoadSyntax(di);
        }

        /// <summary>
        /// Parse all files and subfolders from the skeletons directory
        /// a skeleton is declared with a starter and a closing statement
        /// </summary>
        /// <param name="directory">directory name</param>
        public void ParseSkeletons(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            if (di.Exists)
                this.ParseSkeletons(di);
        }

        /// <summary>
        /// Load all files in a directory (assumes to contain skeletons)
        /// </summary>
        /// <param name="directory">directory path</param>
        public void LoadSkeletons(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            if (di.Exists)
                this.LoadSkeletons(di);
        }

        /// <summary>
        /// Load all files in a directory (assumes to contain MOPs)
        /// </summary>
        /// <param name="directory">directory path</param>
        public void LoadMOP(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            if (di.Exists)
                this.LoadMOP(di);
        }

        /// <summary>
        /// Create a CodeCommander file source
        /// (overwrite if exists)
        /// </summary>
        /// <param name="fileName">file name to create</param>
        public void CreateFile(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            FileStream fs = fi.Create();
            o2Mate.ILocaleGroup locales = new o2Mate.LocaleGroup();
            o2Mate.ILocaleSet group = locales.Get("CodeCommander");
            string text = "Write something";
            if (group.ExistsOne("TextExample", System.Threading.Thread.CurrentThread.CurrentUICulture.Name))
                text = group.Get("TextExample", System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
            string xml = "<?xml version='1.0' encoding='utf-8'?><CodeCommander version='" + o2Mate.Version.GetSoftwareVersion().ToString() + "'><infos><creationDate>" + fi.CreationTime.ToString() + "</creationDate><modificationDate/><revision>1</revision></infos><legendes/><code><texte>" + text + "</texte></code></CodeCommander>";
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(xml);
            sw.Close();
        }

        /// <summary>
        /// Executes a multi-threading sequence
        /// </summary>
        /// <param name="parent">scope object</param>
        /// <param name="node">xml node data to execute</param>
        /// <param name="file">final file</param>
        /// <param name="proc">current process</param>
        public void StartParallel(o2Mate.Scope parent, System.Xml.XmlNode node, FinalFile file, IProcessInstance proc)
        {
            this.principal.Name = "Parallel";
            this.Threads.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "thread"));
            this.principal.CurrentProject = this.Threads;
            this.currentProcess = this.principal;
            this.MakeNewInstance(null, new RunInnerInstanceCompiler(delegate(ICompilateurInstance c)
            {
                c.CurrentScope.Parent = parent;
                this.Load(c, node);
                c.WriteToFile(this.principal, file);
            }));
        }

        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>Compiler object</returns>
        public object Clone()
        {
            Compilateur c = new Compilateur();
            c.uniques = this.uniques;
            c.convertedLanguage = this.convertedLanguage;
            c.templates = new List<Template>(this.templates);
            c.syntaxes = new List<Syntax>(this.syntaxes);
            c.mops = new List<CreateMOP>(this.mops);
            c.injectors = new List<Injector>(from Injector item in this.injectors select item.Clone() as Injector);
            c.principal = new Process();
            c.principal.Name = Compilateur.RootProcessName;
            c.processes = new List<IProcess>(from IProcess item in this.processes select item.Clone() as Process);
            c.skeletons = new List<Skeleton>(this.skeletons);
            c.currentProcess = this.currentProcess;
            c.currentParseProcess = this.currentParseProcess;
            c.processStack = new Stack<IProcess>();
            c.objects = new Tree<ICompiler>(this.objects.Root);
            c.threads = this.threads;
            c.legendeDict = this.legendeDict;
            c.underConversion = this.underConversion;
            c.enc = this.enc;
            return c;
        }
        #endregion
    }
}
