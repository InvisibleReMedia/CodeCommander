using System;
using System.Collections.Generic;
using System.Text;

namespace Converters
{
    /// <summary>
    /// List of all supported language converter
    /// </summary>
    public static class Languages
    {
        /// <summary>
        /// Language name of PowerShell
        /// </summary>
        public const string PowerShell = "PowerShell";
        /// <summary>
        /// Language name of Microsoft C# .NET
        /// </summary>
        public const string MicrosoftCSharp = "Microsoft C# .NET (with Visual Studio 12)";
        /// <summary>
        /// Language name of Java
        /// </summary>
        public const string Java = "Java";
        /// <summary>
        /// Language name of Mac OS C++
        /// </summary>
        public const string MacOSCPP = "MacOS C/C++";
        /// <summary>
        /// Language name of Microsoft C++ .NET
        /// </summary>
        public const string MicrosoftCPP = "Microsoft C++ .NET (with Visual Studion 2012)";
        /// <summary>
        /// Language name of Perl
        /// </summary>
        public const string Perl = "Perl";
        /// <summary>
        /// Language name Python
        /// </summary>
        public const string Python = "Python";
        /// <summary>
        /// Language name Unix C++
        /// </summary>
        public const string UnixCPP = "Unix GCC C/C++";
        /// <summary>
        /// Language name of VBScript
        /// </summary>
        public const string VBScript = "VBScript";

    }

    /// <summary>
    /// In a strongly name language, some variables
    /// could have the same value in multiple different types.
    /// The typed variable name with a specific type is new, current or last
    /// </summary>
    public enum EnumParameterOrder
    {
        /// <summary>
        /// NEW VARIABLE TYPE
        /// </summary>
        E_NEW,
        /// <summary>
        /// CURRENT VARIABLE TYPE
        /// </summary>
        E_CURRENT,
        /// <summary>
        /// LAST VARIABLE TYPE
        /// </summary>
        E_LAST
    }

    /// <summary>
    /// Hold a parameter in a function
    /// </summary>
    public interface IParameter : ICloneable
    {
        /// <summary>
        /// Gets or sets the formal parameter name
        /// </summary>
        string FormalParameter { get; set; }
        /// <summary>
        /// Gets or sets the replacement parameter name (from a mop)
        /// </summary>
        string ReplacementParameter { get; set; }
        /// <summary>
        /// Gets or sets the effective parameter name (a data from scope)
        /// </summary>
        string EffectiveParameter { get; set; }
        /// <summary>
        /// Gets or sets the encapsulated structure reference of this parameter
        /// </summary>
        List<IStructure> StructureReferences { get; }
        /// <summary>
        /// Gets or sets the variable name
        /// Exact name (associated on a variable from scope) of the effective parameter
        /// </summary>
        string VariableName { get; set; }
        /// <summary>
        /// Gets or sets if parameter has changed
        /// </summary>
        bool IsDirty { get; set; }
        /// <summary>
        /// Gets or sets if the parameter is mutable
        /// </summary>
        bool IsMutableParameter { get; set; }
        /// <summary>
        /// Gets or sets if the parameter is computable
        /// </summary>
        bool IsComputable { get; set; }
        /// <summary>
        /// Gets or sets the simple data type (in any language)
        /// </summary>
        o2Mate.EnumDataType DataType { get; set; }
        /// <summary>
        /// Gets or sets the parameter order
        /// </summary>
        EnumParameterOrder Order { get; set; }
        /// <summary>
        /// Commit
        /// </summary>
        void Commit();
    }

    /// <summary>
    /// an interface for structures on any programming language interface
    /// </summary>
    public interface IStructure : ICloneable
    {
        /// <summary>
        /// Gets the instance name
        /// </summary>
        string InstanceName { get; }
        /// <summary>
        /// Gets the field name in a structure
        /// </summary>
        string FieldName { get; }
        /// <summary>
        /// Gets the prefixed field name
        /// </summary>
        string PrefixedFieldName { get; }
        /// <summary>
        /// Says if it's a global field
        /// </summary>
        bool IsGlobal { get; }
        /// <summary>
        /// Gets the data type field
        /// </summary>
        o2Mate.EnumDataType DataType { get; }
        /// <summary>
        /// Gets the structure string data type as a field
        /// </summary>
        string StructureType { get; }
        /// <summary>
        /// Says if this field is mutable
        /// </summary>
        bool IsMutable { get; }
        /// <summary>
        /// Says if this field is a structure itself
        /// </summary>
        bool IsItself { get; }
    }

    /// <summary>
    /// an interface for functions on any programming language interface
    /// </summary>
    public interface IFunction : IEquatable<IFunction>
    {
        /// <summary>
        /// Gets the name of the function
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets or sets strictly the name
        /// </summary>
        string StrictName { get; set; }
        /// <summary>
        /// Gets or sets if function as a job
        /// </summary>
        bool IsJob { get; set; }
        /// <summary>
        /// Gets or sets if function as a macro
        /// </summary>
        bool IsMacro { get; set; }
        /// <summary>
        /// Gets or sets when a function is visible
        /// </summary>
        bool IsVisible { get; set; }
        /// <summary>
        /// Gets or sets an instance number
        /// This counts the number of functions with the same name
        /// </summary>
        int InstanceNumber { get; set; }
        /// <summary>
        /// Gets the function parameters
        /// </summary>
        List<IParameter> Parameters { get; }
        /// <summary>
        /// Gets local variables
        /// </summary>
        List<IStructure> LocalVariables { get; }
        /// <summary>
        /// Gets instance structures (instance/structure name)
        /// </summary>
        Dictionary<string, string> InstancesStructure { get; }
        /// <summary>
        /// Gives the current control flow
        /// </summary>
        EnumControlFlow ControlFlow { get; }
        /// <summary>
        /// Is true when the current control flow
        /// is none or at the root of a function
        /// </summary>
        bool IsStaticControlFlow { get; }
        /// <summary>
        /// Gets or sets the numerous of private variables
        /// This allows to create automatic variable
        /// that are different of user variables
        /// </summary>
        int PrivateVariableCounter { get; set; }
        /// <summary>
        /// Number of indentation in the source code
        /// </summary>
        int IndentSize { get; set; }
        /// <summary>
        /// Gets or sets the entire function body
        /// </summary>
        string Source { get; set; }
        /// <summary>
        /// Gets or sets the expression string converted on the fly
        /// </summary>
        string CacheSource { get; set; }
        /// <summary>
        /// Gets or sets known data type (of any programming language)
        /// </summary>
        o2Mate.EnumDataType DataTypeResult { get; set; }
        /// <summary>
        /// Gets or sets if the expression must return by reference
        /// </summary>
        bool IsByReferenceReturn { get; set; }
        /// <summary>
        /// Gets or sets if the current evaluated expression is computable
        /// </summary>
        bool IsComputableExpression { get; set; }
        /// <summary>
        /// Gets or sets intermediate statements
        /// </summary>
        string AdditionalSource { get; set; }
        /// <summary>
        /// Gets used variables
        /// This tells what variables are used
        /// </summary>
        List<string> UsedVariables { get; }
        /// <summary>
        /// Make function info empty
        /// Ideal for renewing the function
        /// </summary>
        void Clear();
        /// <summary>
        /// Inserts a new statement in the function body
        /// </summary>
        /// <param name="source">source</param>
        void AddToSource(string source);
        /// <summary>
        /// Keeps control flow among multiple functions
        /// </summary>
        /// <param name="f">current function</param>
        void PropagateControlFlow(IFunction f);
        /// <summary>
        /// Assumes to start a function
        /// </summary>
        void ForwardControlFlowSub();
        /// <summary>
        /// Assumes to be in a loop
        /// </summary>
        void ForwardControlFlowInLoop();
        /// <summary>
        /// Assumes to be after a loop
        /// </summary>
        void ForwardControlFlowAfterLoop();
        /// <summary>
        /// Assumes to be in a condition
        /// </summary>
        void ForwardControlFlowInIf();
        /// <summary>
        /// Assumes to be after a condition
        /// </summary>
        void ForwardControlFlowAfterIf();
        /// <summary>
        /// Assumes to end a function
        /// </summary>
        void BackwardControlFlowSub();
    }

    /// <summary>
    /// Converter to a programming language
    /// </summary>
    public interface ICodeConverter
    {
        /// <summary>
        /// Language name
        /// </summary>
        string LanguageName { get; }
        /// <summary>
        /// True if strongly typed language
        /// </summary>
        bool IsStronglyTyped { get; }
        /// <summary>
        /// Gets or sets the output file name where to write all functions
        /// </summary>
        string FunctionsFileName { get; set; }
        /// <summary>
        /// Get or sets the output file name where to write the main function
        /// </summary>
        string PrincipalFileName { get; set; }
        /// <summary>
        /// Indicates the first structure type
        /// </summary>
        string RootStructureType { get; }
        /// <summary>
        /// Indicates the first instance
        /// </summary>
        string RootStructureInstance { get; }
        /// <summary>
        /// List of implemented functions
        /// </summary>
        List<IFunction> ImplementedFunctions { get; }
        /// <summary>
        /// List of calling functions
        /// </summary>
        List<IFunction> CallingFunctions { get; }
        /// <summary>
        /// main Function
        /// </summary>
        IFunction Main { get; }
        /// <summary>
        /// Gets the list of all structure names
        /// </summary>
        Dictionary<string, List<IStructure>> StructureNames { get; }
        /// <summary>
        /// Current function during conversion
        /// </summary>
        IFunction CurrentFunction { get; }
        /// <summary>
        /// Change the current function
        /// (if the function is blinding then keeps the current function name)
        /// </summary>
        /// <param name="newFunc">function</param>
        /// <returns>the previous function to store</returns>
        IFunction SetCurrentFunction(IFunction newFunc);
        /// <summary>
        /// Gives the appropriate latest function name
        /// </summary>
        string ProcessAsFunction { get; }
        /// <summary>
        /// the principal method during conversion
        /// </summary>
        /// <param name="obj">object</param>
        void Convert(ILanguageConverter obj);
        /// <summary>
        /// This function serves to return the name of a variable
        /// dependent if the language is a strongly typed language
        /// </summary>
        /// <param name="var">the variable object</param>
        /// <returns>the adequate name of the variable</returns>
        string ReturnVarName(o2Mate.IData var);
        /// <summary>
        /// Returns a structure info found in parameters or local variables
        /// </summary>
        /// <param name="varName">variable name to search</param>
        /// <param name="instances">structure instance list</param>
        /// <param name="pars">parameters list</param>
        /// <param name="declared">local variables list</param>
        /// <returns>a structure or null if not found</returns>
        IStructure SearchVariable(string varName, Dictionary<string, string> instances, List<IParameter> pars, List<IStructure> declared);
        /// <summary>
        /// Compile the final converted files and run the executable file
        /// </summary>
        /// <param name="solutionFileName">file name of the solution</param>
        /// <param name="projectFileName">file name of the project</param>
        /// <param name="executableFileName">file name of the executable</param>
        /// <param name="executingAuthorization">true if accepts the execution, else false</param>
        /// <returns>true if succeeded</returns>
        bool CompileAndExecute(string solutionFileName, string projectFileName, string executableFileName, bool executingAuthorization);
        /// <summary>
        /// Push a function in a stack
        /// </summary>
        /// <param name="function">given function</param>
        void PushFunction(IFunction function);
        /// <summary>
        /// Pop the function on the top of the stack
        /// </summary>
        /// <returns>function on the top of the stack</returns>
        IFunction PopFunction();
        /// <summary>
        /// Create a new structure name with a structure data type
        /// </summary>
        /// <param name="structDataType">a structure data type name</param>
        /// <param name="structName">a structure name</param>
        /// <param name="isMutable">mutable switch</param>
        /// <returns>a structure object</returns>
        IStructure CreateNewField(string structDataType, string structName, bool isMutable);
        /// <summary>
        /// Create a new sub field and alias the data type, the prefix and the name
        /// </summary>
        /// <param name="instanceName">name of the structure instance (throw exception if not exists)</param>
        /// <param name="dataType">data type</param>
        /// <param name="name">variable name</param>
        /// <param name="isMutable">true if mutable</param>
        /// <returns>a structure object</returns>
        IStructure CreateNewField(string instanceName, o2Mate.EnumDataType dataType, string name, bool isMutable);
        /// <summary>
        /// Create a new sub field and alias a variable in scope
        /// </summary>
        /// <param name="instanceName">name of the structure instance (throw exception if not exists)</param>
        /// <param name="field">an existing variable object (from scope)</param>
        /// <param name="isMutable">true if mutable</param>
        /// <returns>a structure object</returns>
        IStructure CreateNewField(string instanceName, o2Mate.IData field, bool isMutable);
        /// <summary>
        /// Principal method for writing all files
        /// </summary>
        /// <param name="fileDict">dictionary file</param>
        /// <param name="fileName">final file</param>
        void WriteToFile(string fileDict, string fileName);
    }
}
