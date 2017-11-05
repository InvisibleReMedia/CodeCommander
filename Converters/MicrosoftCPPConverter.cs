using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;

namespace Converters
{
    /// <summary>
    /// Microsoft CPP converter
    /// </summary>
    public class MicrosoftCPPConverter : ICodeConverter
    {
        #region Private Static Constants
        private static readonly string IndentString = "    ";
        private static readonly string[] tabTypeName = { "bool", "int", "wstring", "const wstring", "wchar_t", "wchar_t*", "writer", "SimpleType", "void", "int" };
        #endregion

        #region Public Static Constants
        /// <summary>
        /// Indicates the first structure type
        /// </summary>
        public static readonly string rootStructureType = "me";
        /// <summary>
        /// Indicates the first instance
        /// </summary>
        public static readonly string rootStructureInstance = "this";
        #endregion

        #region Private Fields
        private string functionsFileName;
        private string principalFileName;
        private string lastFunctionName;
        private List<IFunction> implementedFunctions;
        private List<IFunction> callingFunctions;
        private Dictionary<string, List<IStructure>> structures;
        private IFunction main;
        private IFunction currentFunction;
        private Stack<IFunction> stack;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public MicrosoftCPPConverter()
        {
            this.callingFunctions = new List<IFunction>();
            this.implementedFunctions = new List<IFunction>();
            this.structures = new Dictionary<string, List<IStructure>>();
            this.stack = new Stack<IFunction>();
            this.main = new Function();
            this.main.StrictName = "main";
            this.CallingFunctions.Add(this.main);
            this.SetCurrentFunction(this.main);
        }
        #endregion

        #region ICodeConverter Membres

        /// <summary>
        /// Language name
        /// </summary>
        public string LanguageName
        {
            get { return Languages.MicrosoftCPP; }
        }

        /// <summary>
        /// True if strongly typed language
        /// </summary>
        public bool IsStronglyTyped
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the output file name where to write all functions
        /// </summary>
        public string FunctionsFileName
        {
            get { return this.functionsFileName; }
            set { this.functionsFileName = value; }
        }

        /// <summary>
        /// Get or sets the output file name where to write the main function
        /// </summary>
        public string PrincipalFileName
        {
            get { return this.principalFileName; }
            set { this.principalFileName = value; }
        }

        /// <summary>
        /// Indicates the first structure type
        /// </summary>
        public string RootStructureType
        {
            get { return MicrosoftCPPConverter.rootStructureType; }
        }

        /// <summary>
        /// Indicates the first instance
        /// </summary>
        public string RootStructureInstance
        {
            get { return MicrosoftCPPConverter.rootStructureInstance; }
        }

        /// <summary>
        /// List of implemented functions
        /// </summary>
        public List<IFunction> ImplementedFunctions
        {
            get { return this.implementedFunctions; }
        }

        /// <summary>
        /// List of calling functions
        /// </summary>
        public List<IFunction> CallingFunctions
        {
            get { return this.callingFunctions; }
        }

        /// <summary>
        /// Gets the list of all structure names
        /// </summary>
        public Dictionary<string, List<IStructure>> StructureNames
        {
            get { return this.structures; }
        }

        /// <summary>
        /// main Function
        /// </summary>
        public IFunction Main
        {
            get { return this.main; }
        }

        /// <summary>
        /// Current function during conversion
        /// </summary>
        public IFunction CurrentFunction
        {
            get
            {
                return this.currentFunction;
            }
        }

        /// <summary>
        /// Change the current function
        /// (if the function is blinding then keeps the current function name)
        /// </summary>
        /// <param name="func">function</param>
        /// <returns>the previous function to store</returns>
        public IFunction SetCurrentFunction(IFunction func)
        {
            if (func.IsVisible) this.lastFunctionName = func.Name;
            IFunction previous = this.currentFunction;
            this.currentFunction = func;
            this.CreateNewField(this.RootStructureType, this.RootStructureInstance, false);
            return previous;
        }

        /// <summary>
        /// Gives the appropriate latest function name
        /// </summary>
        public string ProcessAsFunction
        {
            get
            {
                return (this.currentFunction.IsVisible) ? this.currentFunction.Name : this.lastFunctionName;
            }
        }

        /// <summary>
        /// This function serves to return the name of a variable
        /// dependent if the language is a strongly typed language
        /// </summary>
        /// <param name="var">the variable object</param>
        /// <returns>the adequate name of the variable</returns>
        public string ReturnVarName(o2Mate.IData var)
        {
            return MicrosoftCPPConverter.Escape(var.PrefixedName);
        }

        /// <summary>
        /// the principal method during conversion
        /// </summary>
        /// <param name="obj">object</param>
        public void Convert(ILanguageConverter obj)
        {
            obj.WriteInMicrosoftCPP(this);
        }

        /// <summary>
        /// Compile the final converted files and run the executable file
        /// </summary>
        /// <param name="solutionFileName">file name of the solution</param>
        /// <param name="projectFileName">file name of the project</param>
        /// <param name="executableFileName">file name of the executable</param>
        /// <param name="executingAuthorization">true if accepts the execution, else false</param>
        /// <returns>true if succeeded</returns>
        public bool CompileAndExecute(string solutionFileName, string projectFileName, string executableFileName, bool executingAuthorization)
        {
            bool done = false;

            #region Search MSBuild exe
            if (String.IsNullOrEmpty(global::Converters.Properties.Settings.Default.MSBuildPath))
            {
                // search the path of MSBuild program
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.CreateNoWindow = true;
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processInfo.UseShellExecute = false;
                processInfo.FileName = "cmd.exe";
                processInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                processInfo.Arguments = "/c dir /B /s MSBuild.exe > \"" + CodeCommander.Documents.TempDirectory + "msbuild_found.txt\"";
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;
                Process p = Process.Start(processInfo);
                p.WaitForExit();
                Console.WriteLine(p.StandardOutput.ReadToEnd());
                Console.WriteLine(p.StandardError.ReadToEnd());
                p.Dispose();

                // load the msbuild output file
                FileInfo fi = new FileInfo(CodeCommander.Documents.TempDirectory + "msbuild_found.txt");
                if (fi.Exists)
                {
                    StreamReader sr = fi.OpenText();
                    while (!sr.EndOfStream)
                    {
                        string path = sr.ReadLine();
                        if (path.Contains("\\amd64\\"))
                        {
                            global::Converters.Properties.Settings.Default.MSBuildPath = path;
                            break;
                        }
                    }
                    sr.Close();
                    sr.Dispose();
                }

            }
            #endregion

            #region Search VCBuild exe
            if (!global::Converters.Properties.Settings.Default.VCBuildNotExists && String.IsNullOrEmpty(global::Converters.Properties.Settings.Default.VCBuildPath))
            {
                // search the path of VCBuild program
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.CreateNoWindow = true;
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processInfo.UseShellExecute = false;
                processInfo.FileName = "cmd.exe";
                // system is c:\windows or an another disk unit
                processInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                processInfo.Arguments = "/c dir /B /s vcbuild.exe > \"" + CodeCommander.Documents.TempDirectory + "vcbuild_found.txt\"";
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;
                Process p = Process.Start(processInfo);
                p.WaitForExit();
                Console.WriteLine(p.StandardOutput.ReadToEnd());
                Console.WriteLine(p.StandardError.ReadToEnd());
                p.Dispose();

                global::Converters.Properties.Settings.Default.VCBuildNotExists = true;
                // load the vcbuild output file
                FileInfo fi = new FileInfo(CodeCommander.Documents.TempDirectory + "vcbuild_found.txt");
                if (fi.Exists)
                {
                    StreamReader sr = fi.OpenText();
                    if (!sr.EndOfStream)
                    {
                        global::Converters.Properties.Settings.Default.VCBuildNotExists = false;
                        global::Converters.Properties.Settings.Default.VCBuildPath = sr.ReadLine();
                    }
                    sr.Close();
                    sr.Dispose();
                }
            }
            #endregion

            #region Execute MSbuild against the project
            if (!String.IsNullOrEmpty(global::Converters.Properties.Settings.Default.MSBuildPath))
            {
                // launch msbuild against the solution (.sln)
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processInfo.FileName = global::Converters.Properties.Settings.Default.MSBuildPath;
                processInfo.WorkingDirectory = Path.GetDirectoryName(solutionFileName);
                processInfo.Arguments = "/t:Rebuild /p:Configuration=Debug;Platform=x64 " + Path.GetFileName(solutionFileName);
                processInfo.RedirectStandardError = false;
                processInfo.RedirectStandardOutput = false;
                Process p = Process.Start(processInfo);
                p.WaitForExit();
                p.Dispose();

                FileInfo fiExe = new FileInfo(executableFileName);
                if (fiExe.Exists)
                {
                    done = true;
                }
            }
            #endregion

            #region Execute VCBuild against the project
            if (!done && !global::Converters.Properties.Settings.Default.VCBuildNotExists)
            {
                // launch vcbuild against the project (.vcxproj)
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processInfo.FileName = global::Converters.Properties.Settings.Default.VCBuildPath;
                processInfo.WorkingDirectory = Path.GetDirectoryName(solutionFileName);
                processInfo.Arguments = "/t:Rebuild /p:Configuration=Debug;Platform=x64 " + Path.GetFileName(solutionFileName);
                processInfo.RedirectStandardError = true;
                processInfo.RedirectStandardOutput = true;
                Process p = Process.Start(processInfo);
                p.WaitForExit();
                Console.WriteLine(p.StandardOutput.ReadToEnd());
                Console.WriteLine(p.StandardError.ReadToEnd());
                p.Dispose();

                FileInfo fiExe = new FileInfo(executableFileName);
                if (fiExe.Exists)
                {
                    done = true;
                }
            }
            #endregion

            #region execute if authorized
            if (done && executingAuthorization)
            {
                // launch executable of the project (.vcxproj)
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.CreateNoWindow = true;
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processInfo.FileName = executableFileName;
                processInfo.WorkingDirectory = Path.GetDirectoryName(executableFileName);
                processInfo.Arguments = "";
                Process p = Process.Start(processInfo);
                p.WaitForExit();
                p.Dispose();
            }
            #endregion

            #region save settings
            global::Converters.Properties.Settings.Default.Save();
            #endregion

            return done;
        }

        /// <summary>
        /// Push a function in a stack
        /// </summary>
        /// <param name="function">given function</param>
        public void PushFunction(IFunction function)
        {
            this.stack.Push(function);
        }

        /// <summary>
        /// Pop the function on the top of the stack
        /// </summary>
        /// <returns>function on the top of the stack</returns>
        public IFunction PopFunction()
        {
            return this.stack.Pop();
        }

        /// <summary>
        /// Returns a structure info found in parameters or local variables
        /// </summary>
        /// <param name="varName">variable name to search</param>
        /// <param name="instances">structure instance list</param>
        /// <param name="pars">parameters list</param>
        /// <param name="declared">local variables list</param>
        /// <returns>a structure or null if not found</returns>
        public IStructure SearchVariable(string varName, Dictionary<string, string> instances, List<IParameter> pars, List<IStructure> declared)
        {
            IParameter currentParam = null, currentStructure = null;
            IStructure res = null;
            foreach (IParameter p in pars)
            {
                if (p.DataType == o2Mate.EnumDataType.E_STRUCTURE)
                {
                    if (p.VariableName == varName)
                    {
                        currentStructure = p;
                    }
                    else
                    {
                        res = p.StructureReferences.Find(new Predicate<IStructure>(delegate(IStructure st)
                        {
                            return st.PrefixedFieldName == varName;
                        }));
                    }
                }
                else
                {
                    if (p.FormalParameter == varName)
                    {
                        if (currentParam != null)
                        {
                            if (!p.IsMutableParameter && currentParam.IsMutableParameter)
                                currentParam.IsMutableParameter = false;
                        }
                        else
                        {
                            currentParam = p;
                        }
                    }
                }
            }
            if (currentStructure != null)
            {
                if (instances.ContainsKey(currentStructure.FormalParameter))
                {
                    string structTypeName = instances[currentStructure.FormalParameter];
                    res = this.CreateNewField(structTypeName, currentStructure.FormalParameter, currentStructure.IsMutableParameter);
                }
                else
                {
                    res = this.CreateNewField(MicrosoftCPPConverter.rootStructureInstance, currentStructure.FormalParameter, currentStructure.IsMutableParameter);
                }
            }
            else if (currentParam != null)
            {
                res = this.CreateNewField(MicrosoftCPPConverter.rootStructureInstance, currentParam.DataType, currentParam.FormalParameter, currentParam.IsMutableParameter);
            }
            else if (res == null)
            {
                // recherche dans les variables locales
                res = declared.Find(new Predicate<IStructure>(delegate(IStructure st)
                {
                    return st.PrefixedFieldName == varName;
                }));
            }
            return res;
        }

        /// <summary>
        /// Create a new sub field and alias the data type, the prefix and the name
        /// </summary>
        /// <param name="instanceName">name of the structure instance (throw exception if not exists)</param>
        /// <param name="dataType">data type</param>
        /// <param name="fieldName">variable name</param>
        /// <param name="isMutable">mutable parameter</param>
        /// <returns>a structure object</returns>
        public IStructure CreateNewField(string instanceName, o2Mate.EnumDataType dataType, string fieldName, bool isMutable)
        {
            string typeName = String.Empty;
            if (!this.CurrentFunction.InstancesStructure.ContainsKey(instanceName))
            {
                throw new ArgumentException("L'instance '" + instanceName + "' n'existait pas dans la fonction en cours.");
            }
            else
                typeName = this.CurrentFunction.InstancesStructure[instanceName];
            IStructure st = new Structure(instanceName, fieldName, dataType, isMutable);
            if (this.StructureNames.ContainsKey(typeName))
            {
                if (!this.StructureNames[typeName].Exists(new Predicate<IStructure>(delegate(IStructure search)
                {
                    return search.PrefixedFieldName == st.PrefixedFieldName;
                })))
                    this.StructureNames[typeName].Add(st);
            }
            else
                throw new ArgumentException("Le type '" + typeName + "' n'avait pas été déclaré.");
            return st;
        }

        /// <summary>
        /// Create a new sub field and alias a variable in scope
        /// </summary>
        /// <param name="instanceName">name of the structure instance (throw exception if not exists)</param>
        /// <param name="field">an existing variable object (from scope)</param>
        /// <param name="isMutable">mutable switch</param>
        /// <returns>a structure object</returns>
        public IStructure CreateNewField(string instanceName, o2Mate.IData field, bool isMutable)
        {
            string typeName = String.Empty;
            if (!this.CurrentFunction.InstancesStructure.ContainsKey(instanceName))
            {
                throw new ArgumentException("L'instance '" + instanceName + "' n'existait pas dans la fonction en cours.");
            }
            else
                typeName = this.CurrentFunction.InstancesStructure[instanceName];
            IStructure st = new Structure(instanceName, field, isMutable);
            if (this.StructureNames.ContainsKey(typeName))
            {
                if (!this.StructureNames[typeName].Exists(new Predicate<IStructure>(delegate(IStructure search)
                {
                    return search.PrefixedFieldName == st.PrefixedFieldName;
                })))
                    this.StructureNames[typeName].Add(st);
            } else
                throw new ArgumentException("Le type '" + typeName + "' n'avait pas été déclaré.");

            return st;
        }

        /// <summary>
        /// Create a new structure name with a structure data type
        /// </summary>
        /// <param name="structDataType">a structure data type name</param>
        /// <param name="structName">a structure name</param>
        /// <param name="isMutable">mutable switch</param>
        /// <returns>a structure object</returns>
        public IStructure CreateNewField(string structDataType, string structName, bool isMutable)
        {
            Structure s = new Structure(this.RootStructureInstance, structDataType, structName, isMutable);
            if (!this.CurrentFunction.InstancesStructure.ContainsKey(this.RootStructureInstance))
            {
                this.CurrentFunction.InstancesStructure.Add(this.RootStructureInstance, this.RootStructureType);
            }
            if (!this.StructureNames.ContainsKey(structDataType))
            {
                this.StructureNames.Add(structDataType, new List<IStructure>());
            }
            if (!this.CurrentFunction.InstancesStructure.ContainsKey(structName))
            {
                this.CurrentFunction.InstancesStructure.Add(structName, structDataType);
            }
            if (!this.StructureNames.ContainsKey(this.RootStructureType))
            {
                this.StructureNames.Add(this.RootStructureType, new List<IStructure>());
            }
            if (this.RootStructureInstance != s.FieldName && !this.StructureNames[this.RootStructureType].Exists(new Predicate<IStructure>(delegate(IStructure search)
            {
                return search.PrefixedFieldName == s.PrefixedFieldName;
            })))
                this.StructureNames[this.RootStructureType].Add(s);
            return s;
        }


        #endregion

        #region Public Static Methods
        /// <summary>
        /// Output escaped string
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>escaped string</returns>
        public static string Escape(string input)
        {
            return input.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("$[", "\\$[").Replace("]", "\\]");
        }
        #endregion

        #region Private Methods
        private void RecursiveCopy(DirectoryInfo src, DirectoryInfo dest)
        {
            foreach (FileInfo fi in src.GetFiles())
            {
                File.Copy(fi.FullName, Path.Combine(dest.FullName, fi.Name), true);
            }
            foreach (DirectoryInfo di in src.GetDirectories())
            {
                DirectoryInfo newDi = new DirectoryInfo(Path.Combine(dest.FullName, di.Name));
                if (!newDi.Exists)
                {
                    newDi.Create();
                }
                this.RecursiveCopy(di, newDi);
            }
        }

        private string IndentSource(string source)
        {
            string output = "";
            Regex reg = new Regex("(.*)" + Environment.NewLine);
            MatchCollection matches = reg.Matches(source);
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    output += MicrosoftCPPConverter.IndentString + m.Groups[1].Value + Environment.NewLine;
                }
            }
            return output;
        }

        private string TypeToString(o2Mate.EnumDataType t)
        {
            string type;
            switch (t)
            {
                case o2Mate.EnumDataType.E_BOOL:
                    type = tabTypeName[0]; break;
                case o2Mate.EnumDataType.E_NUMBER:
                    type = tabTypeName[1]; break;
                case o2Mate.EnumDataType.E_STRING_OBJECT:
                    type = tabTypeName[2]; break;
                case o2Mate.EnumDataType.E_CONST_STRING_OBJECT:
                    type = tabTypeName[3]; break;
                case o2Mate.EnumDataType.E_WCHAR:
                    type = tabTypeName[4]; break;
                case o2Mate.EnumDataType.E_STRING:
                    type = tabTypeName[5]; break;
                case o2Mate.EnumDataType.E_WRITER:
                    type = tabTypeName[6]; break;
                case o2Mate.EnumDataType.E_SIMPLETYPE:
                    type = tabTypeName[7]; break;
                case o2Mate.EnumDataType.E_VOID:
                    type = tabTypeName[8]; break;
                default:
                    type = tabTypeName[9]; break;
            }
            return type;
        }

        /// <summary>
        /// Turns the variable name on a value or a reference.
        /// OnLeft means that the variable is placed at the left of an assertion
        /// if the variable is a reference, dereferences it. Other, leaves as it
        /// </summary>
        /// <param name="var">var structure</param>
        /// <param name="instances">structure instance list</param>
        /// <param name="pars">parameter list</param>
        /// <param name="declared">local variable list</param>
        /// <returns>the truth expression</returns>
        private string InterpreteOnLeft(IStructure var, Dictionary<string, string> instances, List<IParameter> pars, List<IStructure> declared)
        {
            string expr = String.Empty;
            if (var.DataType != o2Mate.EnumDataType.E_STRUCTURE)
            {
                if (var.InstanceName != PowerShellConverter.rootStructureInstance)
                {
                    // retrouver l'instance de la structure pour savoir si elle est en référence ou non
                    IStructure cap = this.SearchVariable(var.InstanceName, instances, pars, declared);
                    if (cap != null)
                    {
                        expr += ((cap.IsMutable) ? cap.PrefixedFieldName + "->" : cap.PrefixedFieldName + ".");
                        expr += var.PrefixedFieldName;
                        if (var.IsMutable)
                            expr = "(*" + expr + ")";
                    }
                    else
                        throw new ArgumentException("La structure '" + var.InstanceName + "' n'a pas été transmise à la fonction");
                }
                else
                    expr += (var.IsMutable) ? "(*" + var.PrefixedFieldName + ")" : var.PrefixedFieldName;
            }
            else
                throw new ArgumentException("la variable '" + var.FieldName + "' est une structure et il est impossible d'affecter la valeur d'une structure");
            return expr;
        }

        /// <summary>
        /// Turns the variable name on a reference or a value.
        /// OnPtr means that the variable is a pointer or a value
        /// </summary>
        /// <param name="var">var structure</param>
        /// <param name="instances">structure instance list</param>
        /// <param name="pars">parameter list</param>
        /// <param name="declared">local variable list</param>
        /// <returns>the truth expression</returns>
        private string InterpreteOnPtr(IStructure var, Dictionary<string, string> instances, List<IParameter> pars, List<IStructure> declared)
        {
            string expr = String.Empty;
            if (var.DataType != o2Mate.EnumDataType.E_STRUCTURE)
            {
                if (var.InstanceName != PowerShellConverter.rootStructureInstance)
                {
                    // retrouver l'instance de la structure pour savoir si elle est en référence ou non
                    IStructure cap = this.SearchVariable(var.InstanceName, instances, pars, declared);
                    if (cap != null)
                    {
                        expr += ((cap.IsMutable) ? cap.PrefixedFieldName + "->" : cap.PrefixedFieldName + ".");
                        expr += var.PrefixedFieldName;
                    }
                    else
                        throw new ArgumentException("La structure '" + var.InstanceName + "' n'a pas été transmise à la fonction");
                }
                else
                    expr += var.PrefixedFieldName;
                if (var.IsMutable)
                {
                    expr += "->";
                }
                else
                {
                    expr += ".";
                }
            }
            else
                throw new ArgumentException("la variable '" + var.PrefixedFieldName + "' est une structure et il est impossible de recopier l'ensemble de la structure");
            return expr;
        }

        /// <summary>
        /// Turns the variable name on a reference.
        /// OnRef means that the variable is passed by reference to a function
        /// always references it if variable is a value
        /// </summary>
        /// <param name="var">var structure</param>
        /// <param name="instances">structure instance list</param>
        /// <param name="pars">parameter list</param>
        /// <param name="declared">local variable list</param>
        /// <returns>the truth expression</returns>
        private string InterpreteOnRef(IStructure var, Dictionary<string, string> instances, List<IParameter> pars, List<IStructure> declared)
        {
            string expr = String.Empty;
            if (var.DataType != o2Mate.EnumDataType.E_STRUCTURE)
            {
                if (var.InstanceName != PowerShellConverter.rootStructureInstance)
                {
                    // retrouver l'instance de la structure pour savoir si elle est en référence ou non
                    IStructure cap = this.SearchVariable(var.InstanceName, instances, pars, declared);
                    if (cap != null)
                    {
                        expr += ((cap.IsMutable) ? cap.PrefixedFieldName + "->" : cap.PrefixedFieldName + ".");
                        expr += var.PrefixedFieldName;
                    }
                    else
                        throw new ArgumentException("La structure '" + var.InstanceName + "' n'a pas été transmise à la fonction");
                }
                else
                    expr += var.PrefixedFieldName;
                if (!var.IsMutable)
                {
                    expr = "&(" + expr + ")";
                }
            }
            else
                throw new ArgumentException("la variable '" + var.PrefixedFieldName + "' est une structure et il est impossible de recopier l'ensemble de la structure");
            return expr;
        }

        /// <summary>
        /// Turns the variable name on a value.
        /// OnValue means that the variable is placed at the right of an assertion
        /// or the variable is passed by value to a function
        /// always dereferences it
        /// </summary>
        /// <param name="var">var structure</param>
        /// <param name="instances">structure instance list</param>
        /// <param name="pars">parameter list</param>
        /// <param name="declared">local variable list</param>
        /// <returns>the truth expression</returns>
        private string InterpreteOnValue(IStructure var, Dictionary<string, string> instances, List<IParameter> pars, List<IStructure> declared)
        {
            string expr = String.Empty;
            if (var.DataType != o2Mate.EnumDataType.E_STRUCTURE)
            {
                if (var.InstanceName != PowerShellConverter.rootStructureInstance)
                {
                    // retrouver l'instance de la structure pour savoir si elle est en référence ou non
                    IStructure cap = this.SearchVariable(var.InstanceName, instances, pars, declared);
                    if (cap != null)
                    {
                        expr += ((cap.IsMutable) ? cap.PrefixedFieldName + "->" : cap.PrefixedFieldName + ".");
                        expr += var.PrefixedFieldName;
                        if (var.IsMutable)
                            expr = "(*" + expr + ")";
                    }
                    else
                        throw new ArgumentException("La structure '" + var.InstanceName + "' n'a pas été transmise à la fonction");
                }
                else
                    expr += (var.IsMutable) ? "(*" + var.PrefixedFieldName + ")" : var.PrefixedFieldName;
            }
            else
                throw new ArgumentException("la variable '" + var.PrefixedFieldName + "' est une structure et il est impossible de recopier l'ensemble de la structure");
            return expr;
        }

        /// <summary>
        /// Do not interprete the variable, leaves as it
        /// default means that the variable is presented as it
        /// </summary>
        /// <param name="var">var structure</param>
        /// <param name="instances">structure instance list</param>
        /// <param name="pars">parameter list</param>
        /// <param name="declared">local variable list</param>
        /// <returns>the truth expression</returns>
        private string InterpreteDefault(IStructure var, Dictionary<string, string> instances, List<IParameter> pars, List<IStructure> declared)
        {
            string expr = String.Empty;
            if (var.DataType != o2Mate.EnumDataType.E_STRUCTURE)
            {
                if (var.InstanceName != PowerShellConverter.rootStructureInstance)
                {
                    // retrouver l'instance de la structure pour savoir si elle est en référence ou non
                    IStructure cap = this.SearchVariable(var.InstanceName, instances, pars, declared);
                    if (cap != null)
                    {
                        expr += ((cap.IsMutable) ? cap.PrefixedFieldName + "->" : cap.PrefixedFieldName + ".");
                        expr += var.PrefixedFieldName;
                    }
                    else
                        throw new ArgumentException("La structure '" + var.InstanceName + "' n'a pas été transmise à la fonction");
                }
                else
                    expr += var.PrefixedFieldName;
            }
            else
                throw new ArgumentException("la variable '" + var.PrefixedFieldName + "' est une structure et il est impossible de recopier l'ensemble de la structure");
            return expr;
        }

        private string InterpreteVariable(IStructure var, string type, Dictionary<string, string> instances, List<IParameter> pars, List<IStructure> declared)
        {
            string expr = String.Empty;
            switch (type)
            {
                case "left":
                    expr = this.InterpreteOnLeft(var, instances, pars, declared);
                    break;
                case "byvalue":
                    expr = this.InterpreteOnValue(var, instances, pars, declared);
                    break;
                case "byref":
                    expr = this.InterpreteOnRef(var, instances, pars, declared);
                    break;
                case "ifptr":
                    expr = this.InterpreteOnPtr(var, instances, pars, declared);
                    break;
                default:
                    expr = this.InterpreteDefault(var, instances, pars, declared);
                    break;
            }
            return expr;
        }

        private string ReplaceVars(string source, Dictionary<string, string> instances, List<IParameter> pars, List<IStructure> declared)
        {
            string varName = String.Empty;
            string output = String.Empty;
            int state = 0;
            for (int index = 0; index < source.Length; ++index)
            {
                char c = source[index];
                if (state == 1)
                {
                    if (c == '$' || c == '\\' || c == ']')
                    {
                        output += c.ToString();
                    }
                    else
                    {
                        output += "\\" + c.ToString();
                    }
                    state = 0;
                }
                else if (state == 0)
                {
                    if (c == '\\')
                    {
                        state = 1;
                    }
                    else if (c == '$')
                    {
                        varName = String.Empty;
                        state = 2;
                    }
                    else
                    {
                        output += c.ToString();
                    }
                }
                else if (state == 2)
                {
                    if (c == '[') state = 3;
                    else
                    {
                        output += "$" + c.ToString();
                        state = 0;
                    }
                }
                else if (state == 3)
                {
                    if (c == ']')
                    {
                        // extract x:varName
                        int posColon = varName.IndexOf(':');
                        string type = String.Empty;
                        string name = String.Empty;
                        if (posColon != -1) // avec ou sans type
                        {
                            if (posColon > 0)
                            {
                                type = varName.Substring(0, posColon);
                                name = varName.Substring(posColon + 1);
                            }
                            else
                                name = varName.Substring(1);
                        }
                        else
                            name = varName;
                        IStructure found = this.SearchVariable(name, instances, pars, declared);
                        if (found != null)
                        {
                            output += this.InterpreteVariable(found, type, instances, pars, declared);
                        }
                        else
                            throw new InvalidDataException("La variable '" + name + "' n'avait pas été déclarée");
                        state = 0;
                    }
                    else if (c == '\\')
                    {
                        state = 4;
                    }
                    else
                    {
                        varName += c.ToString();
                    }
                }
                else if (state == 4)
                {
                    if (c == ']' || c == '$' || c == '\\' || c == '"')
                    {
                        varName += c.ToString();
                    }
                    else
                    {
                        varName += "\\" + c.ToString();
                    }
                    state = 3;
                }

            }
            return output;
        }

        private void Compilation(FileInfo fiSource, FileInfo fiDict, FileInfo fiResult)
        {
            // I must use reflection : if not, this is a circular project
            Type tComp = Type.GetTypeFromProgID("o2Mate.Compilateur");
            if (tComp != null)
            {
                object oComp = Activator.CreateInstance(tComp);
                MethodInfo mComp = null;
                // aller la chercher la méthode Compilation avec 5 paramètres
                foreach (MethodInfo mi in tComp.GetMethods())
                {
                    if (mi.Name == "Compilation" && mi.GetParameters().Length == 5)
                    {
                        mComp = mi;
                        break;
                    }
                }
                if (mComp != null)
                {
                    mComp.Invoke(oComp, new object[] { fiSource.FullName, fiDict.FullName, fiResult.FullName, "Windows-1252", null });
                }
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Principal method for writing all files
        /// </summary>
        /// <param name="fileDict">dictionary file</param>
        /// <param name="fileName">final file</param>
        public void WriteToFile(string fileDict, string fileName)
        {

            #region Common Files copy
            string projectName = Path.GetFileNameWithoutExtension(fileName);
            string solutionDir = projectName + "_wincpp";
            DirectoryInfo di = new DirectoryInfo(CodeCommander.Documents.LanguageConvertersDirectory("wincpp"));
            DirectoryInfo build = new DirectoryInfo(CodeCommander.Documents.BuildDirectory);
            if (!build.Exists)
            {
                build.Create();
            }
            DirectoryInfo dest = new DirectoryInfo(Path.Combine(build.FullName, solutionDir));
            if (!dest.Exists)
            {
                dest.Create();
            }
            else
            {
                dest.Delete(true);
                dest.Create();
            }
            this.RecursiveCopy(di, dest);
            #endregion

            #region Compilation des fichiers de configuration
            o2Mate.Dictionnaire dict = new o2Mate.Dictionnaire();
            dict.AddString("SolutionGuid", Guid.NewGuid().ToString());
            dict.AddString("ProjectGuid", Guid.NewGuid().ToString());
            dict.AddString("SolutionName", projectName);
            dict.AddString("ProjectPath", projectName);
            dict.AddString("ProjectNamespace", projectName);
            dict.AddString("ProjectName", projectName);
            dict.Save(CodeCommander.Documents.TempDictFile);

            FileInfo fiSource = new FileInfo(Path.Combine(dest.FullName, "ConsoleCpp.sln.xml"));
            FileInfo fiDict = new FileInfo(CodeCommander.Documents.TempDictFile);
            FileInfo fiResult = new FileInfo(Path.Combine(dest.FullName, projectName + ".sln"));
            this.Compilation(fiSource, fiDict, fiResult);

            // renommer le sous répertoire
            DirectoryInfo subDi = new DirectoryInfo(Path.Combine(dest.FullName, "ConsoleCpp"));
            subDi.MoveTo(Path.Combine(dest.FullName, projectName));
            fiSource = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "ConsoleCpp.vcxproj.xml")));
            fiResult = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, projectName + ".vcxproj")));
            this.Compilation(fiSource, fiDict, fiResult);

            fiSource = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "AssemblyInfo.cpp.xml")));
            fiResult = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "AssemblyInfo.cpp")));
            this.Compilation(fiSource, fiDict, fiResult);

            fiSource = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "ReadMe.txt.xml")));
            fiResult = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "ReadMe.txt")));
            this.Compilation(fiSource, fiDict, fiResult);

            #endregion

            #region Copy DLL features
            FileInfo fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "o2MateDict.dll"));
            fi.CopyTo(Path.Combine(dest.FullName, Path.Combine(projectName, "o2MateDict.dll")), true);
            fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "o2MateLegende.dll"));
            fi.CopyTo(Path.Combine(dest.FullName, Path.Combine(projectName, "o2MateLegende.dll")), true);
            fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "o2MateLocale.dll"));
            fi.CopyTo(Path.Combine(dest.FullName, Path.Combine(projectName, "o2MateLocale.dll")), true);
            #endregion

            #region Initialize three files : one for .h and for .cpp and for main.cpp
            FileInfo headerFile = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "compiled.h")));
            FileInfo sourceFile = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "compiled.cpp")));
            FileInfo mainFile = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "main.cpp")));
            #endregion

            #region header Construct

            // declare typedef (iteration à l'envers)
            string structures = String.Empty;
            foreach (KeyValuePair<string, List<IStructure>> kv in this.StructureNames.Reverse())
            {
                structures += "typedef struct tag_" + kv.Key + " {" + Environment.NewLine;
                foreach (IStructure field in kv.Value)
                {
                    if (field.IsMutable)
                    {
                        structures += MicrosoftCPPConverter.IndentString;
                        if (field.DataType == o2Mate.EnumDataType.E_STRUCTURE)
                        {
                            structures += field.StructureType;
                        }
                        else
                        {
                            structures += this.TypeToString(field.DataType);
                        }
                        structures += "* " + field.PrefixedFieldName + ";" + Environment.NewLine;
                    }
                    else
                    {
                        if (field.DataType == o2Mate.EnumDataType.E_STRUCTURE)
                        {
                            structures += field.StructureType;
                        }
                        else
                        {
                            structures += this.TypeToString(field.DataType);
                        }
                        structures += " " + field.PrefixedFieldName + ";" + Environment.NewLine;
                    }
                }
                structures += "} " + kv.Key + ";" + Environment.NewLine;
            }
            // declare prototypes of all private called functions
            string prototypes = String.Empty;
            foreach (IFunction f in this.CallingFunctions)
            {
                prototypes += "void " + (f.IsMacro ? "macro_" : f.IsJob ? "job_" : "func_") + f.Name + "(";
                bool first = true;
                // tous les paramètres sont indiqués
                foreach (IParameter p in f.Parameters)
                {
                    if (!first) { prototypes += ", "; } else { first = false; }
                    if (p.IsMutableParameter)
                    {
                        if (p.DataType != o2Mate.EnumDataType.E_STRUCTURE)
                            prototypes += this.TypeToString(p.DataType) + "* " + p.FormalParameter;
                        else
                            if (f.InstancesStructure.ContainsKey(p.FormalParameter))
                                prototypes += f.InstancesStructure[p.FormalParameter] + "* " + p.FormalParameter;
                            else
                                throw new ArrayTypeMismatchException("La structure '" + p.FormalParameter + "' n'avait pas été déclarée");
                    }
                    else
                    {
                        if (p.DataType == o2Mate.EnumDataType.E_CONST_STRING_OBJECT ||
                            p.DataType == o2Mate.EnumDataType.E_SIMPLETYPE ||
                            p.DataType == o2Mate.EnumDataType.E_STRING ||
                            p.DataType == o2Mate.EnumDataType.E_STRING_OBJECT)
                            prototypes += this.TypeToString(p.DataType) + "& " + p.FormalParameter;
                        else
                            prototypes += this.TypeToString(p.DataType) + " " + p.FormalParameter;
                    }
                }
                prototypes += ");" + Environment.NewLine;
            }
            prototypes += Environment.NewLine;
            prototypes += Environment.NewLine;
            dict.AddString("structures", structures);
            dict.AddString("prototypes", prototypes);
            #endregion

            #region implementation Construct

            dict.AddString("DictFileName", (Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + solutionDir + Path.DirectorySeparatorChar + projectName).Replace("\\", "\\\\"));
            dict.AddString("FinalFileName", (Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + projectName).Replace("\\", "\\\\"));


            // implements all functions
            string functions = String.Empty;
            foreach (IFunction f in this.implementedFunctions)
            {
                functions += "void Compiled::" + (f.IsMacro ? "macro_" : f.IsJob ? "job_" : "func_") + f.Name + "(";
                bool first = true;
                // les paramètres calculables ne sont pas des paramètres
                foreach (IParameter p in f.Parameters)
                {
                    if (!first) { functions += ", "; } else { first = false; }
                    if (p.IsMutableParameter)
                    {
                        if (p.DataType != o2Mate.EnumDataType.E_STRUCTURE)
                            functions += this.TypeToString(p.DataType) + "* " + p.FormalParameter;
                        else
                            if (f.InstancesStructure.ContainsKey(p.FormalParameter))
                                functions += f.InstancesStructure[p.FormalParameter] + "* " + p.FormalParameter;
                            else
                                throw new ArrayTypeMismatchException("La structure '" + p.FormalParameter + "' n'avait pas été déclarée");
                    }
                    else
                    {
                        if (p.DataType == o2Mate.EnumDataType.E_CONST_STRING_OBJECT ||
                            p.DataType == o2Mate.EnumDataType.E_SIMPLETYPE ||
                            p.DataType == o2Mate.EnumDataType.E_STRING ||
                            p.DataType == o2Mate.EnumDataType.E_STRING_OBJECT)
                            functions += this.TypeToString(p.DataType) + "& " + p.FormalParameter;
                        else
                            functions += this.TypeToString(p.DataType) + " " + p.FormalParameter;
                    }
                }
                functions += ") {" + Environment.NewLine;

                // declare all local data(s)
                foreach (IStructure tvar in f.LocalVariables)
                {
                    if (!tvar.IsItself && tvar.InstanceName == MicrosoftCPPConverter.rootStructureInstance)
                            functions += MicrosoftCPPConverter.IndentString + this.TypeToString(tvar.DataType) + " " + tvar.PrefixedFieldName + ";" + Environment.NewLine;
                }

                functions += this.IndentSource(this.ReplaceVars(f.Source, f.InstancesStructure, f.Parameters, f.LocalVariables)) + Environment.NewLine + "}" + Environment.NewLine + Environment.NewLine + Environment.NewLine;
            }

            // writing main function
            string mainCode = (this.Main.IsMacro ? "macro_" : this.Main.IsJob ? "job_" : "func_") + this.main.Name + "();";
            dict.AddString("functions", functions);
            dict.AddString("mainCode", mainCode);

            #endregion

            #region Write files

            dict.Save(CodeCommander.Documents.TempDictFile);

            // main.cpp
            fiSource = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "main.cpp.xml")));
            fiDict = new FileInfo(CodeCommander.Documents.TempDictFile);
            fiResult = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "main.cpp")));
            this.Compilation(fiSource, fiDict, fiResult);

            // compiled.cpp
            fiSource = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "compiled.cpp.xml")));
            fiDict = new FileInfo(CodeCommander.Documents.TempDictFile);
            fiResult = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "compiled.cpp")));
            this.Compilation(fiSource, fiDict, fiResult);

            // compiled.h
            fiSource = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "compiled.h.xml")));
            fiDict = new FileInfo(CodeCommander.Documents.TempDictFile);
            fiResult = new FileInfo(Path.Combine(dest.FullName, Path.Combine(projectName, "compiled.h")));
            this.Compilation(fiSource, fiDict, fiResult);
            
            #endregion

            #region Remove XML Files
            foreach (FileInfo fiXml in dest.GetFiles("*.xml"))
            {
                fiXml.Delete();
            }
            foreach (FileInfo fiXml in dest.GetDirectories(projectName)[0].GetFiles("*.xml"))
            {
                fiXml.Delete();
            }
            #endregion

        }
        #endregion
    }
}
