using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Converters
{
    /// <summary>
    /// PowerShell converter
    /// </summary>
    public class PowerShellConverter : ICodeConverter
    {
        #region Private Static Constants
        private static readonly string IndentString = "    ";
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
        public PowerShellConverter()
        {
            this.callingFunctions = new List<IFunction>();
            this.implementedFunctions = new List<IFunction>();
            this.structures = new Dictionary<string, List<IStructure>>();
            this.stack = new Stack<IFunction>();
            this.main = new Function();
            this.main.StrictName = "main";
            this.callingFunctions.Add(this.main);
            this.SetCurrentFunction(this.main);
        }
        #endregion

        #region ICodeConverter Membres

        /// <summary>
        /// Language name
        /// </summary>
        public string LanguageName
        {
            get { return Languages.PowerShell; }
        }

        /// <summary>
        /// True if strongly typed language
        /// </summary>
        public bool IsStronglyTyped
        {
            get { return false; }
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
            get { return PowerShellConverter.rootStructureType; }
        }

        /// <summary>
        /// Indicates the first instance
        /// </summary>
        public string RootStructureInstance
        {
            get { return PowerShellConverter.rootStructureInstance; }
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
            return PowerShellConverter.Escape(var.Name);
        }

        /// <summary>
        /// the principal method during conversion
        /// </summary>
        /// <param name="obj">object</param>
        public void Convert(ILanguageConverter obj)
        {
            obj.WriteInPowerShell(this);
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
            throw new NotImplementedException();
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
        public IStructure SearchVariable(string varName, Dictionary<string,string> instances, List<IParameter> pars, List<IStructure> declared)
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
                        foreach (IStructure st in p.StructureReferences)
                        {
                            if (st.FieldName == varName)
                            {
                                res = st;
                            }
                        }
                    }
                }
                else
                {
                    if (p.VariableName == varName)
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
                if (instances.ContainsKey(currentStructure.VariableName))
                {
                    string structTypeName = instances[currentStructure.VariableName];
                    res = this.CreateNewField(structTypeName, currentStructure.VariableName, currentStructure.IsMutableParameter);
                } else
                {
                    res = this.CreateNewField("", currentStructure.VariableName, currentStructure.IsMutableParameter);
                }
            }
            else if (currentParam != null)
            {
                res = this.CreateNewField(PowerShellConverter.rootStructureInstance, currentParam.DataType, currentParam.VariableName, currentParam.IsMutableParameter);
            }
            else if (res == null)
            {
                // recherche dans les variables locales
                res = declared.Find(new Predicate<IStructure>(delegate(IStructure st)
                {
                    return st.FieldName == varName;
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
            if (!this.StructureNames.ContainsKey(typeName))
            {
                this.StructureNames[typeName].Add(st);
            }
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
            if (!this.StructureNames.ContainsKey(typeName))
            {
                this.StructureNames[typeName].Add(st);
            }
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
            if (!this.structures.ContainsKey(structDataType))
            {
                this.structures.Add(structDataType, new List<IStructure>());
            }
            if (!this.CurrentFunction.InstancesStructure.ContainsKey(structName))
            {
                this.CurrentFunction.InstancesStructure.Add(structName, structDataType);
            }
            if (!this.structures.ContainsKey(this.RootStructureType))
            {
                this.structures.Add(this.RootStructureType, new List<IStructure>());
            }
            this.structures[this.RootStructureType].Add(s);
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
            return input.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("S[", "\\S[").Replace("]", "\\]");
        }
        #endregion

        #region Private Methods
        private string IndentSource(int n, string source)
        {
            string output = "";
            string indent = String.Empty;
            for (int index = 0; index < n; ++index) indent += PowerShellConverter.IndentString;
            Regex reg = new Regex("(.*)" + Environment.NewLine);
            MatchCollection matches = reg.Matches(source);
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    output += indent + m.Groups[1].Value + Environment.NewLine;
                }
            }
            return output;
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
        private string InterpreteOnLeft(IStructure var, Dictionary<string,string> instances, List<IParameter> pars, List<IStructure> declared)
        {
            string expr = String.Empty;
            if (var.DataType != o2Mate.EnumDataType.E_STRUCTURE)
            {
                expr += "$";
                if (var.InstanceName != PowerShellConverter.rootStructureInstance)
                {
                    // retrouver l'instance de la structure pour savoir si elle est en référence ou non
                    IStructure cap = this.SearchVariable(var.InstanceName, instances, pars, declared);
                    if (cap != null)
                    {
                        expr += ((cap.IsMutable) ? cap.FieldName + ".value" : cap.FieldName);
                        expr += ".Item(\"" + var.FieldName + "\")";
                        expr += (var.IsMutable) ? ".value" : "";
                    }
                    else
                        throw new ArgumentException("La structure '" + var.InstanceName + "' n'a pas été transmise à la fonction");
                }
                else
                    expr += (var.IsMutable) ? var.FieldName + ".value" : var.FieldName;
            }
            else
                throw new ArgumentException("la variable '" + var.FieldName + "' est une structure et il est impossible d'affecter la valeur d'une structure");
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
                expr += "$";
                if (var.InstanceName != PowerShellConverter.rootStructureInstance)
                {
                    // retrouver l'instance de la structure pour savoir si elle est en référence ou non
                    IStructure cap = this.SearchVariable(var.InstanceName, instances, pars, declared);
                    if (cap != null)
                    {
                        expr += ((cap.IsMutable) ? cap.FieldName + ".value" : cap.FieldName);
                        expr += ".Item(\"" + var.FieldName + "\")";
                    }
                    else
                        throw new ArgumentException("La structure '" + var.InstanceName + "' n'a pas été transmise à la fonction");
                }
                else
                    expr += var.FieldName;
                if (!var.IsMutable)
                {
                    expr = "([ref] " + expr + ")";
                }
            }
            else
                throw new ArgumentException("la variable '" + var.FieldName + "' est une structure et il est impossible de recopier l'ensemble de la structure");
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
                expr += "$";
                if (var.InstanceName != PowerShellConverter.rootStructureInstance)
                {
                    // retrouver l'instance de la structure pour savoir si elle est en référence ou non
                    IStructure cap = this.SearchVariable(var.InstanceName, instances, pars, declared);
                    if (cap != null)
                    {
                        expr += ((cap.IsMutable) ? cap.FieldName + ".value" : cap.FieldName);
                        expr += ".Item(\"" + var.FieldName + "\")";
                        expr += (var.IsMutable) ? ".value" : "";
                    }
                    else
                        throw new ArgumentException("La structure '" + var.InstanceName + "' n'a pas été transmise à la fonction");
                }
                else
                    expr += (var.IsMutable) ? var.FieldName + ".value" : var.FieldName;
            }
            else
                throw new ArgumentException("la variable '" + var.FieldName + "' est une structure et il est impossible de recopier l'ensemble de la structure");
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
                expr += "$";
                if (var.InstanceName != PowerShellConverter.rootStructureInstance)
                {
                    // retrouver l'instance de la structure pour savoir si elle est en référence ou non
                    IStructure cap = this.SearchVariable(var.InstanceName, instances, pars, declared);
                    if (cap != null)
                    {
                        expr += ((cap.IsMutable) ? cap.FieldName + ".value" : cap.FieldName);
                        expr += ".Item(\"" + var.FieldName + "\")";
                    }
                    else
                        throw new ArgumentException("La structure '" + var.InstanceName + "' n'a pas été transmise à la fonction");
                }
                else
                    expr += var.FieldName;
            }
            else
                throw new ArgumentException("la variable '" + var.FieldName + "' est une structure et il est impossible de recopier l'ensemble de la structure");
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
                    if (c == 'S' || c == '\\' || c == ']')
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
                    else if (c == 'S')
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
                        output += "S" + c.ToString();
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
                    if (c == ']' || c == 'S' || c == '\\' || c == '"')
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
            string sourceName = Path.GetFileNameWithoutExtension(fileName);
            DirectoryInfo di = new DirectoryInfo(CodeCommander.Documents.LanguageConvertersDirectory("powershell"));
            DirectoryInfo dest = new DirectoryInfo(CodeCommander.Documents.BuildDirectory);
            if (!dest.Exists)
            {
                dest.Create();
            }

            // Copying files
            FileInfo fiMain = new FileInfo(Path.Combine(di.FullName, "main.ps1.xml"));
            FileInfo fiScript = new FileInfo(Path.Combine(di.FullName, "functions.ps1.xml"));

            fiMain.CopyTo(Path.Combine(dest.FullName, "main" + sourceName + ".ps1.xml"), true);
            fiScript.CopyTo(Path.Combine(dest.FullName, "functions" + sourceName + ".ps1.xml"), true);

            #endregion

            #region Open dictionary

            o2Mate.Dictionnaire dict = new o2Mate.Dictionnaire();
            dict.AddString("DictionaryFileName", fileDict);
            
            #endregion

            #region main file script
            string defaultFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".txt");
            dict.AddString("DefaultFileName", defaultFileName);
            string mainFunction = (this.Main.IsMacro ? "macro_" : this.Main.IsJob ? "job_" : "func_") + this.Main.Name;
            dict.AddString("MainFunction", mainFunction);
            dict.AddString("FunctionsFileName", "functions_" + sourceName + ".ps1");
            #endregion

            #region implementation Construct

            // implements all functions
            string functions = String.Empty;
            foreach (IFunction f in this.implementedFunctions)
            {
                functions += "function " + (f.IsMacro ? "macro_" : f.IsJob ? "job_" : "func_") + f.Name + "(";
                bool first = true;
                // ne choisis que les paramètres inconnus
                // recherche que les paramètres qui ne sont pas calculables
                List<string> distincts = new List<string>();
                foreach (IParameter p in f.Parameters.FindAll(new Predicate<IParameter>(delegate(IParameter par)
                {
                    return !par.IsComputable;
                })))
                {
                    if (!distincts.Contains(p.VariableName))
                    {
                        if (!first) { functions += ", "; } else { first = false; }
                        if (p.IsMutableParameter) { functions += "[ref] "; }
                        functions += "$" + p.VariableName;
                        distincts.Add(p.VariableName);
                    }
                }
                functions += ") {" + Environment.NewLine + this.IndentSource(1, this.ReplaceVars(f.Source, f.InstancesStructure, f.Parameters, f.LocalVariables)) + "}" + Environment.NewLine + Environment.NewLine + Environment.NewLine;
            }

            dict.AddString("ImplementedFunctions", this.IndentSource(2, functions));

            #endregion

            #region Write files

            dict.Save(CodeCommander.Documents.TempDictFile);

            // main.ps1
            FileInfo fiSource = new FileInfo(Path.Combine(dest.FullName, "main" + sourceName + ".ps1.xml"));
            FileInfo fiDict = new FileInfo(CodeCommander.Documents.TempDictFile);
            FileInfo fiResult = new FileInfo(Path.Combine(dest.FullName, sourceName + ".ps1"));
            this.Compilation(fiSource, fiDict, fiResult);

            // functions.ps1
            fiSource = new FileInfo(Path.Combine(dest.FullName, "functions" + sourceName + ".ps1.xml"));
            fiResult = new FileInfo(Path.Combine(dest.FullName, "functions_" + sourceName + ".ps1"));
            this.Compilation(fiSource, fiDict, fiResult);

            #endregion
        }
        #endregion
    }
}
