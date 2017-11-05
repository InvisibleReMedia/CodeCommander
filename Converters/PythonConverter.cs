using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Converters
{
    /// <summary>
    /// Python converter
    /// </summary>
    public class PythonConverter : ICodeConverter
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
        public PythonConverter()
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
            get { return Languages.Python; }
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
            get { return PythonConverter.rootStructureType; }
        }

        /// <summary>
        /// Indicates the first instance
        /// </summary>
        public string RootStructureInstance
        {
            get { return PythonConverter.rootStructureInstance; }
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
            return var.Name;
        }

        /// <summary>
        /// the principal method during conversion
        /// </summary>
        /// <param name="obj">object</param>
        public void Convert(ILanguageConverter obj)
        {
            obj.WriteInPython(this);
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
        public IStructure SearchVariable(string varName, Dictionary<string, string> instances, List<IParameter> pars, List<IStructure> declared)
        {
            throw new NotImplementedException();
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

        #region Private Methods
        private string IndentSource(string source)
        {
            string output = "";
            Regex reg = new Regex("(.*)" + Environment.NewLine);
            MatchCollection matches = reg.Matches(source);
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    output += PythonConverter.IndentString + m.Groups[1].Value + Environment.NewLine;
                }
            }
            return output;
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
            throw new NotImplementedException();
        }
        #endregion
    }
}
