using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Converters;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace o2Mate
{
    /// <summary>
    /// Run against a process instance
    /// </summary>
    /// <param name="previous">previous instance</param>
    /// <param name="proc">process instance object</param>
    public delegate void RunInnerInstanceProcess(IProcessInstance previous, IProcessInstance proc);

    /// <summary>
    /// Interface declaration of a process
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public interface IProcess : ICloneable, IDisposable
    {
        /// <summary>
        /// Returns the current scope object from an existing instance
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <returns>current scope</returns>
        o2Mate.Scope GetCurrentScope(int instance);
        /// <summary>
        /// Sets the current scope for an existing instance
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <param name="scope">the scope value to set</param>
        void SetCurrentScope(int instance, o2Mate.Scope scope);
        /// <summary>
        /// Returns the current dictionary object from an existing instance
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <returns>the current scope</returns>
        o2Mate.Dictionnaire GetCurrentDictionnaire(int instance);
        /// <summary>
        /// Sets the current dictionary for an existing instance
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <param name="dict">the dictionary value to set</param>
        void SetCurrentDictionnaire(int instance, o2Mate.Dictionnaire dict);
        /// <summary>
        /// Returns the current execution position value from an existing instance
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <returns>the current execution position</returns>
        int GetCurrentPositionExecution(int instance);
        /// <summary>
        /// Sets the current execution position for an existing instance
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <param name="curPos">the execution position value to set</param>
        void SetCurrentPositionExecution(int instance, int curPos);
        /// <summary>
        /// Returns the default writer value from an existing instance
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <returns>the default writer</returns>
        string GetDefaultWriter(int instance);
        /// <summary>
        /// Sets the default writer for an existing instance
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <param name="writer">the default value to set</param>
        void SetDefaultWriter(int instance, string writer);
        /// <summary>
        /// Gets or sets the process name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the strict function name as the process name for conversion
        /// </summary>
        string FunctionName { get; set; }
        /// <summary>
        /// Gets or sets project informations
        /// </summary>
        Projects CurrentProject { get; set; }
        /// <summary>
        /// Gets the number of statements
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets or sets if the process has changed (change information to convert to an another instance function)
        /// </summary>
        bool HasChanged { get; set; }
        /// <summary>
        /// Gets the statement list
        /// </summary>
        List<ICompiler> Objects { get; }
        /// <summary>
        /// Gets the replacement parameters for mop
        /// </summary>
        List<IParameter> Replacements { get; }
        /// <summary>
        /// Adds a new statement
        /// </summary>
        /// <param name="obj">a statement object</param>
        void AddObject(ICompiler obj);
        /// <summary>
        /// Inserts a new statement
        /// </summary>
        /// <param name="index">index (zero-based) where to insert this statement</param>
        /// <param name="obj">statement object</param>
        void InsertObject(int index, ICompiler obj);
        /// <summary>
        /// Removes objects
        /// </summary>
        /// <param name="from">the start index</param>
        /// <param name="to">the last index (this included)</param>
        void Remove(int from, int to);
        /// <summary>
        /// Gets the execution position of a label identified by name
        /// </summary>
        /// <param name="name">name of the label</param>
        /// <returns>execution index (zero-based)</returns>
        int GetPositionLabel(string name);
        /// <summary>
        /// Converts this process to an another language
        /// There is only one instance of that process for the entire conversion
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <param name="converter">converter object</param>
        /// <param name="comp">compiler object</param>
        /// <param name="file">final file</param>
        void Convert(int instance, ICodeConverter converter, ICompilateurInstance comp, FinalFile file);
        /// <summary>
        /// Creates a new instance (allows recursive mode)
        /// </summary>
        /// <param name="previous">previous process</param>
        /// <param name="run">delegate function</param>
        /// <returns>a new instance number</returns>
        int MakeNewInstance(IProcessInstance previous, RunInnerInstanceProcess run);
        /// <summary>
        /// Creates a unique instance (doesn't allow recursive mode)
        /// for conversion only
        /// </summary>
        /// <param name="previous">previous instance process</param>
        /// <param name="run">delegate function</param>
        /// <returns>a unique instance number</returns>
        int MakeOneInstance(IProcessInstance previous, RunInnerInstanceProcess run);
        /// <summary>
        /// Get an instance of that process
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <returns>process instance</returns>
        IProcessInstance GetInstance(int instance);
        /// <summary>
        /// Execution starts immediately against this process
        /// Allows recursive calls
        /// </summary>
        /// <param name="instance">process instance number</param>
        /// <param name="comp">compiler instance object</param>
        /// <param name="file">final file writer</param>
        void ExecuteProcess(int instance, ICompilateurInstance comp, FinalFile file);
        /// <summary>
        /// Extracts the dictionary from reading the process statements
        /// </summary>
        /// <param name="instance">instance number</param>
        /// <param name="comp">compiler object</param>
        void ExtractDictionary(int instance, ICompilateurInstance comp);
    }

    /// <summary>
    /// An instance process
    /// </summary>
    public interface IProcessInstance : IProcess
    {
        /// <summary>
        /// Test and set lock
        /// </summary>
        bool TestAndSetLock();
        /// <summary>
        /// Gets this instance number
        /// </summary>
        int InstanceNumber { get; }
        /// <summary>
        /// Gets or sets the counter of threads
        /// </summary>
        int ParallelProcessCounter { get; set; }
        /// <summary>
        /// Gets or sets the current execution position
        /// </summary>
        int CurrentPositionExecution { get; set; }
        /// <summary>
        /// Gets or sets the current scope
        /// </summary>
        o2Mate.Scope CurrentScope { get; set; }
        /// <summary>
        /// Gets or sets the current dictionary
        /// </summary>
        Dictionnaire CurrentDictionnaire { get; set; }
        /// <summary>
        /// Gets or sets the default writer
        /// </summary>
        string DefaultWriter { get; set; }
        /// <summary>
        /// Shortcut method, execution starts immediately against this process
        /// Allows recursive calls
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="file">final file writer</param>
        void ExecuteProcess(ICompilateurInstance comp, FinalFile file);
        /// <summary>
        /// Extracts the dictionary from reading the process statements
        /// </summary>
        /// <param name="comp">compiler object</param>
        void ExtractDictionary(ICompilateurInstance comp);
        /// <summary>
        /// Converts this process to an another language
        /// There is only one instance of that process for the entire conversion
        /// </summary>
        /// <param name="converter">converter object</param>
        /// <param name="comp">compiler instance object</param>
        /// <param name="file">final file</param>
        void Convert(ICodeConverter converter, ICompilateurInstance comp, FinalFile file);
        /// <summary>
        /// Wait until all threads are terminated
        /// </summary>
        void Wait();
        /// <summary>
        /// Terminate sleeping...all threads are terminated
        /// </summary>
        void WakeUp();
    }
    
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Process : IProcess, ICompiler
    {
        #region Internal Classes
        internal class ProcessState : IProcessInstance, IDisposable
        {
            #region Private Fields
            private int instanceNumber;
            private int currentPosition;
            private o2Mate.Scope currentScope;
            private o2Mate.Dictionnaire currentDict;
            private string writer;
            internal int waitAll;
            private System.Threading.Timer tim;
            private ManualResetEvent wait;
            private IProcess staticProcess;
            private Object syncRoot;
            private bool inUse;
            #endregion

            #region Default Constructor
            public ProcessState(IProcess staticProcess, int number, bool useImmediate)
            {
                this.staticProcess = staticProcess;
                this.instanceNumber = number;
                this.currentScope = null;
                this.currentDict = null;
                this.currentPosition = 0;
                this.syncRoot = new Object();
                this.inUse = useImmediate;
            }
            #endregion

            #region Public Properties
            public int ParallelProcessCounter
            {
                get { return this.waitAll; }
                set { this.waitAll = value; }
            }

            public int InstanceNumber
            {
                get { return this.instanceNumber; }
            }

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

            public Dictionnaire CurrentDictionnaire
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

            public int CurrentPositionExecution
            {
                get
                {
                    return this.currentPosition;
                }
                set
                {
                    this.currentPosition = value;
                }
            }

            public string DefaultWriter
            {
                get { return this.writer; }
                set { this.writer = value; }
            }

            #endregion

            #region Private Methods

            private void Terminate(object obj)
            {
                if (this.ParallelProcessCounter == this.Count)
                {
                    this.WakeUp();
                }
            }

            #endregion

            #region Public Methods
            public bool TestAndSetLock()
            {
                bool allow = false;
                lock(this.syncRoot) {
                    if (!inUse) { inUse = true; allow = true; }
                }
                return allow;
            }

            public void Unlock()
            {
                this.inUse = false;
            }

            public void WakeUp()
            {
                this.tim.Dispose();
                this.tim = null;
                this.wait.Set();
            }

            public void Wait()
            {
                this.wait = new ManualResetEvent(false);
                this.tim = new System.Threading.Timer(new TimerCallback(Terminate), null, 1000, 2000);
                this.wait.WaitOne();
                this.wait.Close();
                this.wait.Dispose();
                this.wait = null;
            }
            #endregion

            #region IDisposable Members
            public void Dispose()
            {
                if (this.tim != null) { this.tim.Dispose(); this.tim = null; }
                if (this.wait != null) { this.wait.Set(); this.wait.Dispose(); this.wait = null; }
            }
            #endregion

            #region IProcessInstance Members
            public Scope GetCurrentScope(int instance)
            {
                return this.staticProcess.GetCurrentScope(instance);
            }

            public void SetCurrentScope(int instance, Scope scope)
            {
                this.staticProcess.SetCurrentScope(instance, scope);
            }

            public Dictionnaire GetCurrentDictionnaire(int instance)
            {
                return this.staticProcess.GetCurrentDictionnaire(instance);
            }

            public void SetCurrentDictionnaire(int instance, Dictionnaire dict)
            {
                this.staticProcess.SetCurrentDictionnaire(instance, dict);
            }

            public int GetCurrentPositionExecution(int instance)
            {
                return this.staticProcess.GetCurrentPositionExecution(instance);
            }

            public void SetCurrentPositionExecution(int instance, int curPos)
            {
                this.staticProcess.SetCurrentPositionExecution(instance, curPos);
            }

            public string GetDefaultWriter(int instance)
            {
                return this.staticProcess.GetDefaultWriter(instance);
            }

            public void SetDefaultWriter(int instance, string writer)
            {
                this.staticProcess.SetDefaultWriter(instance, writer);
            }

            public string Name
            {
                get
                {
                    return this.staticProcess.Name;
                }
                set
                {
                    this.staticProcess.Name = value;
                }
            }

            public string FunctionName
            {
                get
                {
                    return this.staticProcess.FunctionName;
                }
                set
                {
                    this.staticProcess.FunctionName = value;
                }
            }

            public Projects CurrentProject
            {
                get
                {
                    return this.staticProcess.CurrentProject;
                }
                set
                {
                    this.staticProcess.CurrentProject = value;
                }
            }

            public int Count
            {
                get { return this.staticProcess.Count; }
            }

            public bool HasChanged
            {
                get
                {
                    return this.staticProcess.HasChanged;
                }
                set
                {
                    this.staticProcess.HasChanged = value;
                }
            }

            public List<ICompiler> Objects
            {
                get { return this.staticProcess.Objects; }
            }

            public List<IParameter> Replacements
            {
                get { return this.staticProcess.Replacements; }
            }

            public void AddObject(ICompiler obj)
            {
                this.staticProcess.AddObject(obj);
            }

            public void InsertObject(int index, ICompiler obj)
            {
                this.staticProcess.InsertObject(index, obj);
            }

            public void Remove(int from, int to)
            {
                this.staticProcess.Remove(from, to);
            }

            public int GetPositionLabel(string name)
            {
                return this.staticProcess.GetPositionLabel(name);
            }

            public void Convert(ICodeConverter converter, ICompilateurInstance comp, FinalFile file)
            {
                this.staticProcess.Convert(this.instanceNumber, converter, comp, file);
            }

            public void Convert(int instance, ICodeConverter converter, ICompilateurInstance comp, FinalFile file)
            {
                this.staticProcess.Convert(instance, converter, comp, file);
            }

            public int MakeNewInstance(IProcessInstance previous, RunInnerInstanceProcess run)
            {
                return this.staticProcess.MakeNewInstance(previous, run);
            }

            public int MakeOneInstance(IProcessInstance previous, RunInnerInstanceProcess run)
            {
                return this.staticProcess.MakeOneInstance(previous, run);
            }

            public IProcessInstance GetInstance(int instance)
            {
                return this.staticProcess.GetInstance(instance);
            }

            public void ExecuteProcess(int instance, ICompilateurInstance comp, FinalFile file)
            {
                this.staticProcess.ExecuteProcess(instance, comp, file);
            }

            public void ExecuteProcess(ICompilateurInstance comp, FinalFile file)
            {
                this.staticProcess.ExecuteProcess(this.instanceNumber, comp, file);
            }

            public void ExtractDictionary(int instance, ICompilateurInstance comp)
            {
                this.staticProcess.ExtractDictionary(instance, comp);
            }

            public void ExtractDictionary(ICompilateurInstance comp)
            {
                this.staticProcess.ExtractDictionary(this.instanceNumber, comp);
            }

            #endregion

            #region ICloneable Member
            public object Clone()
            {
                ProcessState p = new ProcessState(this.staticProcess.Clone() as Process, this.instanceNumber, false);
                p.CurrentScope = this.CurrentScope;
                p.CurrentDictionnaire = this.CurrentDictionnaire;
                p.CurrentPositionExecution = this.CurrentPositionExecution;
                p.inUse = false;
                return p;
            }

            #endregion
        }

        internal class ThreadObject
        {
            #region Private Fields
            private IProcessInstance enclosureProc;
            private ICompilateurInstance comp;
            private ICompiler tache;
            private FinalFile file;
            private string errMessage;
            private bool errOccurred;
            #endregion

            #region Default Constructor
            public ThreadObject(IProcessInstance enclosureProc, ICompilateurInstance comp, ICompiler tache, FinalFile file)
            {
                this.enclosureProc = enclosureProc;
                this.comp = comp;
                this.tache = tache;
                this.file = file;
                this.errMessage = String.Empty;
                this.errOccurred = false;
            }
            #endregion

            #region Public Properties
            public bool HasError
            {
                get { return this.errOccurred; }
            }

            public string ErrorMessage
            {
                get { if (this.errOccurred) return this.errMessage; else return ""; }
                set { this.errMessage = value; this.errOccurred = true; }
            }
            #endregion

            #region Public Methods
            /// <summary>
            /// This function must be synchronized to run correctly
            /// </summary>
            [MethodImpl(MethodImplOptions.Synchronized)]
            public void Validate()
            {
                ++this.enclosureProc.ParallelProcessCounter;
            }

            public void Execute()
            {
                this.tache.WriteToFile(this.comp.CurrentExecutedProcess, this.file);
                if (this.tache.TypeName == "Call")
                {
                    Call appel = tache as Call;
                    this.comp.CurrentExecutedProcess.CurrentProject.Add(new ProjectItem(this.comp.CurrentExecutedProcess.Name, this.comp.CurrentExecutedProcess.CurrentPositionExecution, "call process", appel.ProcessName));
                    IProcess proc = comp.GetProcess(appel.ProcessName);
                    this.comp.CurrentExecutedProcess.CurrentProject.Add(new ProjectItem(proc.Name, 0, "begin process", appel.ProcessName));
                    comp.WriteToFile(proc, file);
                }
                else if (tache.TypeName == "CallSkeleton")
                {
                    CallSkeleton appel = tache as CallSkeleton;
                    this.comp.CurrentExecutedProcess.CurrentProject.Add(new ProjectItem(this.comp.CurrentExecutedProcess.Name, this.comp.CurrentExecutedProcess.CurrentPositionExecution, "call skeleton", appel.Name));
                    Skeleton skel = comp.GetSkeleton(appel.Name);
                    Process proc = skel.Process as Process;
                    this.comp.CurrentExecutedProcess.CurrentProject.Add(new ProjectItem(proc.Name, 0, "begin skeleton", appel.Name));
                    comp.WriteToFile(proc, file);
                }
                else if (tache.TypeName == "Parallel")
                {
                    Parallel par = tache as Parallel;
                    this.comp.CurrentExecutedProcess.CurrentProject.Add(new ProjectItem(this.comp.CurrentExecutedProcess.Name, this.comp.CurrentExecutedProcess.CurrentPositionExecution, "parallel"));
                    // créer un nouveau processus et installer les instructions dans ce processus
                    // pour ensuite traiter les instructions en parallèle
                    Process proc = new Process();
                    proc.Name = "Parallel:" + this.comp.Unique.ComputeNewString();
                    this.comp.PushProcess(this.comp.CurrentProcess);
                    this.comp.CurrentProcess = proc;
                    this.comp.Load(this.comp, par.XmlCode);
                    this.comp.WriteToFile(proc, file);
                    this.comp.CurrentProcess = this.comp.ProcessStack.Peek();
                    this.comp.PopProcess();
                }
            }
            #endregion
        }
        #endregion

        #region Private Fields
        private string name;
        private string funName;
        private List<ICompiler> list;
        private List<IParameter> replacements;
        private Projects currentProject;
        private bool hasChanged;
        private List<ProcessState> instances;
        private List<ThreadObject> threads;
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Returns the correct function name as a process
        /// </summary>
        /// <param name="processName">process name</param>
        /// <returns>a function name</returns>
        public static string ProcessAsFunction(string processName)
        {
            if (processName.StartsWith("Skeleton:"))
            {
                return CallSkeleton.SpecialChars(processName.Substring(9));
            }
            else if (processName.StartsWith("Coding:"))
            {
                return processName.Substring(7);
            }
            else if (processName.StartsWith("Parallel:"))
            {
                return processName.Substring(9);
            }
            else if (processName.StartsWith("Template:"))
            {
                return processName.Substring(9);
            }
            else if (processName.StartsWith("Job:"))
            {
                return processName.Substring(4);
            }
            else if (processName.StartsWith(":"))
            {
                return processName.Substring(1);
            }
            else if (processName == "principal")
            {
                return "Main";
            }
            else if (processName == "Templates")
            {
                return "Templates";
            }
            else if (processName == "Syntaxes")
            {
                return "Syntaxes";
            }
            else if (processName == "Skeletons")
            {
                return "Skeletons";
            }
            else if (processName == "MOPs")
            {
                return "MOPs";
            }
            else
            {
                throw new ArgumentException("Le nom du processus '" + processName + "' n'est pas bien formé");
            }
        }

        #endregion

        #region Default Constructor
        public Process()
        {
            this.name = "";
            this.hasChanged = false;
            this.instances = new List<ProcessState>();
            this.list = new List<ICompiler>();
            this.replacements = new List<IParameter>();
            this.currentProject = null;
            this.threads = new List<ThreadObject>();
        }
        #endregion

        #region Public Properties
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.FunctionName = Process.ProcessAsFunction(this.name);
            }
        }

        public string FunctionName
        {
            get
            {
                return this.funName;
            }
            set
            {
                this.funName = value;
            }
        }

        public Projects CurrentProject
        {
            get
            {
                return this.currentProject;
            }
            set
            {
                this.currentProject = value;
            }
        }

        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        public List<ICompiler> Objects
        {
            get
            {
                return this.list;
            }
        }
        #endregion

        #region Private Methods

        private void RunThread(object obj)
        {
            ThreadObject to = obj as ThreadObject;
            this.threads.Add(to);
            try
            {
                to.Execute();
            }
            catch (Exception ex) { to.ErrorMessage = ex.ToString(); }
            finally { to.Validate(); }
        }

        #endregion

        #region IProcess Membres
        public bool HasChanged
        {
            get
            {
                return this.hasChanged;
            }
            set
            {
                this.hasChanged = value;
            }
        }

        public List<IParameter> Replacements
        {
            get { return this.replacements; }
        }

        public object Clone()
        {
            Process p = new Process();
            p.Name = this.Name;
            p.FunctionName = this.FunctionName;
            ICompiler[] array = new ICompiler[this.list.Count];
            this.list.CopyTo(array);
            foreach (ICompiler c in array)
            {
                p.list.Add(c);
            }
            IParameter[] pars = new IParameter[this.replacements.Count];
            this.replacements.CopyTo(pars);
            foreach(IParameter pa in pars)
            {
                p.replacements.Add(pa);
            }
            return p;
        }

        public ICompiler Item(int index)
        {
            return this.list[index];
        }

        public void AddObject(ICompiler obj)
        {
            this.list.Add(obj);
        }

        public void InsertObject(int index, ICompiler obj)
        {
            foreach (ProcessState ps in this.instances)
            {
                if (ps.CurrentPositionExecution > index)
                    ++ps.CurrentPositionExecution;
            }
            this.list.Insert(index, obj);
        }

        public void Remove(int from, int to)
        {
            foreach (ProcessState ps in this.instances)
            {
                if (ps.CurrentPositionExecution > from)
                    if (ps.CurrentPositionExecution < to)
                        throw new InvalidProgramException("Une autre instance du processus '" + this.Name + "' est en cours d'exécution sur une autre thread et une injection de code endommage son exécution");
                    else
                        ps.CurrentPositionExecution -= to - from + 1;
            }
            this.list.RemoveRange(from, to - from + 1);
        }

        public int GetPositionLabel(string labName)
        {
            foreach (ICompiler tache in this.list)
            {
                if (tache.TypeName == "Label")
                {
                    Label lab = tache as Label;
                    if (lab.Name == labName)
                    {
                        return this.list.IndexOf(lab);
                    }
                }
            }
            throw new Exception("Label '" + labName + "' non trouvé");
        }

        public void ExtractDictionary(int instance, ICompilateurInstance comp)
        {
            IProcessInstance internalState = this.GetInstance(instance);
            internalState.CurrentPositionExecution = 0;
            while (internalState.CurrentPositionExecution < this.list.Count)
            {
                ICompiler tache = this.list[internalState.CurrentPositionExecution++];
                tache.ExtractDictionary(internalState);
                if (tache.TypeName == "Call")
                {
                    Call appel = tache as Call;
                    // non recursive mode
                    if (comp.CheckNonRecursiveMode(":" + appel.ProcessName))
                    {
                        internalState.CurrentScope.Push();
                        IProcess proc = comp.GetProcess(appel.ProcessName);
                        comp.ExtractDictionary(proc);
                        internalState.CurrentScope.Pop();
                    }
                }
                else if (tache.TypeName == "CallSkeleton")
                {
                    CallSkeleton appel = tache as CallSkeleton;
                    Skeleton skel = comp.GetSkeleton(appel.Name);
                    // non recursive mode
                    if (comp.CheckNonRecursiveMode(skel.Process.Name))
                    {
                        internalState.CurrentScope.Push();
                        comp.InputLegendes(skel.Legendes as ILegendeDict);
                        comp.ExtractDictionary(skel.Process as Process);
                        internalState.CurrentScope.Pop();
                    }
                }
            }
        }

        public int MakeNewInstance(IProcessInstance previous, RunInnerInstanceProcess run)
        {
            int selectedInstance = -1;
            for (int number = this.instances.Count - 1; number >= 0; --number)
            {
                if (this.instances[number].TestAndSetLock())
                {
                    selectedInstance = number;
                    try
                    {
                        this.instances[selectedInstance] = new ProcessState(this, selectedInstance, true);
                        run(previous, this.instances[selectedInstance]);
                    }
                    catch { throw; }
                    finally { this.instances[selectedInstance].Unlock(); }
                    break;
                }
            }
            if (selectedInstance == -1)
            {
                selectedInstance = this.instances.Count;
                ProcessState ps = new ProcessState(this, selectedInstance, true);
                this.instances.Add(ps);
                try
                {
                    run(previous, ps);
                }
                catch { throw; }
                finally { ps.Unlock(); }
            }
            return selectedInstance;
        }

        public int MakeOneInstance(IProcessInstance previous, RunInnerInstanceProcess run)
        {
            int selectedInstance = this.instances.Count;
            if (selectedInstance == 0)
            {
                ProcessState ps = new ProcessState(this, selectedInstance, true);
                this.instances.Add(ps);
                try
                {
                    run(previous, ps);
                }
                catch { throw; }
                finally { ps.Unlock(); }
            }
            else if (selectedInstance == 1)
            {
                try
                {
                    if (this.instances[selectedInstance - 1].TestAndSetLock())
                    {
                        this.instances[selectedInstance - 1] = new ProcessState(this, selectedInstance - 1, true);
                        run(previous, this.instances[selectedInstance - 1]);
                    }
                    else
                        throw new InvalidProgramException("L'unique instance du processus est déjà en cours d'utilisation");
                }
                catch { throw; }
                finally { this.instances[selectedInstance - 1].Unlock(); }
            }
            else
            {
                throw new InvalidProgramException("Le nombre d'instances (" + selectedInstance.ToString() + ") du processus '" + this.Name + "' est supérieur à 1.");
            }
            return selectedInstance;
        }

        public IProcessInstance GetInstance(int instance)
        {
            return this.instances[instance];
        }

        public o2Mate.Scope GetCurrentScope(int instance)
        {
            return this.instances[instance].CurrentScope;
        }

        public void SetCurrentScope(int instance, o2Mate.Scope scope)
        {
            this.instances[instance].CurrentScope = scope;
        }

        public o2Mate.Dictionnaire GetCurrentDictionnaire(int instance)
        {
            return this.instances[instance].CurrentDictionnaire;
        }

        public void SetCurrentDictionnaire(int instance, o2Mate.Dictionnaire dict)
        {
            this.instances[instance].CurrentDictionnaire = dict;
        }

        public int GetCurrentPositionExecution(int instance)
        {
            return this.instances[instance].CurrentPositionExecution;
        }

        public void SetCurrentPositionExecution(int instance, int curPos)
        {
            this.instances[instance].CurrentPositionExecution = curPos;
        }

        public string GetDefaultWriter(int instance)
        {
            return this.instances[instance].DefaultWriter;
        }

        public void SetDefaultWriter(int instance, string writer)
        {
            this.instances[instance].DefaultWriter = writer;
        }

        public void ExecuteProcess(int instance, ICompilateurInstance comp, FinalFile file)
        {
            if (this.Name.StartsWith("Parallel:"))
            {
                this.instances[instance].waitAll = 0;
                int workerCounter = 0, completionCounter = 0;
                ThreadPool.GetMaxThreads(out workerCounter, out completionCounter);
                if (!(this.Count < workerCounter || this.Count < completionCounter))
                    ThreadPool.SetMaxThreads(workerCounter + this.Count, completionCounter + this.Count);
            }
            this.SetCurrentPositionExecution(instance, 0);
            while (this.GetCurrentPositionExecution(instance) < this.list.Count)
            {
                int position = this.GetCurrentPositionExecution(instance);
                if (this.Name.StartsWith("Parallel:"))
                {
                    // créer une nouvelle instance du compilateur de manière à conserver
                    // l'enchainement indépendant des appels de processus
                    comp.MakeNewInstance(comp, new RunInnerInstanceCompiler(delegate(ICompilateurInstance newComp)
                    {
                        ThreadObject to = new ThreadObject(this.GetInstance(instance), newComp, this.list[position++], file);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(RunThread), to);
                    }));
                    this.SetCurrentPositionExecution(instance, position);
                }
                else
                {
                    ICompiler tache = this.list[position++];
                    this.SetCurrentPositionExecution(instance, position);
                    tache.WriteToFile(this.GetInstance(instance), file);
                    if (tache.TypeName == "Call")
                    {
                        Call appel = tache as Call;
                        this.CurrentProject.Add(new ProjectItem(this.Name, this.GetCurrentPositionExecution(instance), "call process", appel.ProcessName));
                        IProcess proc = comp.GetProcess(appel.ProcessName);
                        this.CurrentProject.Add(new ProjectItem(proc.Name, 0, "begin process", appel.ProcessName));
                        comp.WriteToFile(proc, file);
                    }
                    else if (tache.TypeName == "CallSkeleton")
                    {
                        CallSkeleton appel = tache as CallSkeleton;
                        this.CurrentProject.Add(new ProjectItem(this.Name, this.GetCurrentPositionExecution(instance), "call skeleton", appel.Name));
                        Skeleton skel = comp.GetSkeleton(appel.Name);
                        Process proc = skel.Process as Process;
                        this.CurrentProject.Add(new ProjectItem(proc.Name, 0, "begin skeleton", appel.Name));
                        comp.WriteToFile(proc, file);
                    }
                    else if (tache.TypeName == "Parallel")
                    {
                        Parallel par = tache as Parallel;
                        this.CurrentProject.Add(new ProjectItem(this.Name, this.GetCurrentPositionExecution(instance), "parallel"));
                        // créer un nouveau processus et installer les instructions dans ce processus
                        // pour ensuite traiter les instructions en parallèle
                        Process proc = new Process();
                        proc.Name = "Parallel:" + comp.Unique.ComputeNewString();
                        comp.PushProcess(comp.CurrentProcess);
                        comp.CurrentProcess = proc;
                        comp.Load(comp, par.XmlCode);
                        comp.WriteToFile(proc, file);
                        comp.CurrentProcess = comp.ProcessStack.Peek();
                        comp.PopProcess();
                    }
                }
            }
            if (this.Name.StartsWith("Parallel:"))
            {
                this.GetInstance(instance).Wait();
                string messages = String.Empty;
                foreach (ThreadObject to in this.threads)
                {
                    if (to.HasError) messages += to.ErrorMessage + Environment.NewLine;
                }
                if (!String.IsNullOrWhiteSpace(messages))
                {
                    MessageBox.Show(messages, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void Convert(int instance, ICodeConverter converter, ICompilateurInstance comp, FinalFile file)
        {
            // variable utilisée pour la conversion d'un processus d'execution en parallèle
            BeginJobs bj = new BeginJobs();

            this.SetCurrentPositionExecution(instance, 0);
            while (this.GetCurrentPositionExecution(instance) < this.list.Count)
            {
                int position = this.GetCurrentPositionExecution(instance);
                ICompiler tache = this.list[position++];
                this.SetCurrentPositionExecution(instance, position);
                if (this.Name.StartsWith("Parallel:"))
                {
                    bj.Statements.Add(tache);
                }
                else
                {
                    if (tache.TypeName == "Call")
                    {
                        Call c = tache as Call;

                        // recursive mode off
                        if (comp.CheckNonRecursiveMode(":" + c.ProcessName))
                        {
                            IProcess proc = comp.GetProcess(c.ProcessName);
                            // Ecriture du code de la fonction
                            IFunction newFunc = Helper.MakeNewMethod(converter, proc);
                            Helper.FillNewMethod(comp, proc, converter, newFunc, file);
                            proc.MakeOneInstance(this.GetInstance(instance), new RunInnerInstanceProcess(delegate(IProcessInstance prev, IProcessInstance i)
                            {
                                i.CurrentDictionnaire = prev.CurrentDictionnaire;
                                i.CurrentScope = prev.CurrentScope;
                                i.CurrentScope.Push();
                                i.DefaultWriter = prev.DefaultWriter;
                                c.Convert(converter, i, file);
                                i.CurrentScope.Pop();
                            }));
                            if (proc.HasChanged)
                            {
                                this.HasChanged = true;
                                proc.HasChanged = false;
                            }
                        }
                    }
                    else if (tache.TypeName == "CallSkeleton")
                    {
                        CallSkeleton c = tache as CallSkeleton;

                        if (comp.CheckNonRecursiveMode("Skeleton:" + CallSkeleton.SpecialChars(c.Name)))
                        {
                            Skeleton skel = comp.GetSkeleton(c.Name);
                            // Ecriture du code de la fonction
                            IFunction newFunc = Helper.MakeNewMethod(converter, skel.Process);
                            Helper.FillNewMethod(comp, skel.Process, converter, newFunc, file);
                            skel.Process.MakeOneInstance(this.GetInstance(instance), new RunInnerInstanceProcess(delegate(IProcessInstance prev, IProcessInstance i)
                            {
                                i.CurrentDictionnaire = prev.CurrentDictionnaire;
                                i.CurrentScope = prev.CurrentScope;
                                i.CurrentScope.Push();
                                i.DefaultWriter = prev.DefaultWriter;
                                c.Convert(converter, i, file);
                                i.CurrentScope.Pop();
                            }));
                            if (skel.Process.HasChanged)
                            {
                                this.HasChanged = true;
                                skel.Process.HasChanged = false;
                            }
                        }
                    }
                    else if (tache.TypeName == "BeginProcess")
                    {
                        BeginProcess bp = tache as BeginProcess;
                        bp.Convert(converter, this.GetInstance(instance), file);
                    }
                    else if (tache.TypeName == "BeginSkeleton")
                    {
                        BeginSkeleton bs = tache as BeginSkeleton;
                        bs.Convert(converter, this.GetInstance(instance), file);
                    }
                    else if (tache.TypeName == "EndProcess")
                    {
                        EndProcess ep = tache as EndProcess;
                        ep.Convert(converter, this.GetInstance(instance), file);
                    }
                    else if (tache.TypeName == "EndSkeleton")
                    {
                        EndSkeleton es = tache as EndSkeleton;
                        es.Convert(converter, this.GetInstance(instance), file);
                    }
                    else if (tache.TypeName == "Parallel")
                    {
                        this.GetCurrentScope(instance).Push();
                        // il faut faire une copie de ce processus car il est modifié
                        // au niveau de ses instructions
                        IProcess proc = comp.GetParallelProcess(this.Name).Clone() as Process;
                        proc.FunctionName = converter.ProcessAsFunction;
                        Parallel par = tache as Parallel;
                        // chaque ligne en parallèle crée un nouveau processus
                        // le nouveau processus reprend 1 seule ligne de la séquence
                        // un objet CallJob remplace cette ligne dans le code du processus
                        for (int index = 0; index < proc.Count; ++index)
                        {
                            CallJob cj = new CallJob(comp);
                            cj.ProcessName = comp.Unique.ComputeNewString();
                            Process np = new Process();
                            np.Name = "Job:" + cj.ProcessName;
                            np.AddObject(proc.Objects[index]);
                            proc.Objects.RemoveAt(index);
                            proc.Objects.Insert(index, cj);
                            // ajout du processus dans la liste des processus globaux
                            comp.AddProcess(np);
                        }


                        // on affecte un nouvel objet au début du processus
                        // qui sera une copie de la variable du fichier par défaut
                        Affectation affOld = new Affectation(comp);
                        affOld.VariableName = comp.Unique.ComputeNewString();
                        affOld.Expression = this.GetDefaultWriter(instance);
                        proc.InsertObject(0, affOld);

                        // j'ajoute la dernière instruction
                        Affectation affTemp = new Affectation(comp);
                        affTemp.VariableName = affOld.Expression;
                        affTemp.Expression = affOld.VariableName;
                        proc.AddObject(affTemp);

                        // indication du fichier de sortie pour cette fonction
                        this.SetDefaultWriter(instance, affOld.VariableName);

                        // conversion de la liste des call job
                        comp.Convert(converter, proc, file);

                        // réinitialiser le fichier de sortie précédent
                        this.SetDefaultWriter(instance, affOld.Expression);

                        this.GetCurrentScope(instance).Pop();
                    }
                    else
                    {
                        tache.Convert(converter, this.GetInstance(instance), file);
                    }
                }
            }
            if (this.Name.StartsWith("Parallel:"))
            {
                bj.ConvertToParallel(converter, comp, this.GetInstance(instance), file);
            }
        }
        #endregion

        #region Compiler Membres

        public string TypeName
        {
            get { return "Process"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
        }

        public void Inject(Injector injector, ICompilateurInstance comp, System.Xml.XmlNode node, string modifier)
        {
        }

        public bool IsComputable(IProcess proc, out string reason)
        {
            reason = null;
            return false;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
        }

        #endregion

        #region Dispose Method
        public void Dispose()
        {
            foreach (ProcessState p in this.instances)
            {
                p.Dispose();
            }
            this.instances.Clear();
            this.instances = null;
        }
        #endregion
    }
}
