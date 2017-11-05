using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Converters
{
    /// <summary>
    /// VBScript converter
    /// </summary>
    public class VBScriptConverter : ICodeConverter
    {
        #region Private Static Constants
        private static readonly string IndentString = "    ";
        #endregion

        #region Public Static Constants
        /// <summary>
        /// Indicates the first structure type
        /// </summary>
        private static readonly string rootStructureType = "RootType";
        /// <summary>
        /// Indicates the first instance
        /// </summary>
        private static readonly string rootStructureInstance = "Root";
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
        public VBScriptConverter()
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
            get { return Languages.VBScript; }
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
            get { return VBScriptConverter.rootStructureType; }
        }

        /// <summary>
        /// Indicates the first instance
        /// </summary>
        public string RootStructureInstance
        {
            get { return VBScriptConverter.rootStructureInstance; }
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
            obj.WriteInVBScript(this);
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
                    output += VBScriptConverter.IndentString + m.Groups[1].Value + Environment.NewLine;
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
            StreamWriter sw = new StreamWriter(fileName, false, Encoding.GetEncoding(1252));
            sw.WriteLine(@"
' ########################################
' copyright @ 2014
' This file was generated
' by CodeCommander
'
' codecommander.codeplex.com
'
' ########################################

Const ForAppending = 8
Const ForReading = 1
Const ForWriting = 2
Const TriStateTrue = -1
Const SpaceChar = " + "\"" + " " + "\"" + @"
Const PlusChar = " + "\"" + "+" + "\"" + @"
Const MinusChar = " + "\"" + "-" + "\"" + @"
Const StarChar = " + "\"" + "*" + "\"" + @"
Const SlashChar = " + "\"" + "/" + "\"" + @"
Const ColonChar = " + "\"" + ":" + "\"" + @"
Const CommaChar = " + "\"" + "," + "\"" + @"
Const DotChar = " + "\"" + "." + "\"" + @"
Const ExChar = " + "\"" + "!" + "\"" + @"
Const IntChar = " + "\"" + "?" + "\"" + @"
Const EmptyChar = " + "\"" + "" + "\"" + @"
Const InfChar = " + "\"" + "<" + "\"" + @"
Const SupChar = " + "\"" + ">" + "\"" + @"
Const OpenParChar = " + "\"" + "(" + "\"" + @"
Const CloseParChar = " + "\"" + ")" + "\"" + @"
Const OpenBraChar = " + "\"" + "{" + "\"" + @"
Const CloseBraChar = " + "\"" + "}" + "\"" + @"
Const OpenSquaBraChar = " + "\"" + "[" + "\"" + @"
Const CloseSquaBraChar = " + "\"" + "]" + "\"" + @"
CrLf = " + "vbCrLf" + @"
Const trueValue = 1
Const falseValue = 0

Dim theDict
Set theDict = CreateObject(" + "\"" + "o2Mate.Dictionnaire" +  "\")" + @"
theDict.Load " + "\"" + fileDict + "\"" + @"


Sub CreateDirectoryIfNotExists(dir)
    Dim fso
    Dim textStream
    Set fso = CreateObject(" + "\"Scripting.FileSystemObject\"" + @")
    If Not fso.FolderExists(dir) Then
	    fso.CreateFolder dir
	End If
    Set fso = Nothing
End Sub

Sub EraseFile(file)
    Dim fso
    Dim textStream
    Set fso = CreateObject(" + "\"Scripting.FileSystemObject\"" + @")
    If fso.FileExists(file) Then
        fso.DeleteFile file
    End If
    Set fso = Nothing
End Sub

Function WriteIndent(writer, text)
    Dim output
    Dim reg
    Dim matches
    Set reg = New RegExp
    output = vbNullString
    reg.Pattern = " + "\"(((.*\" + crlf + \")+).*$|.*$)\"" + @"
    reg.MultiLine = true
    Set matches = reg.Execute(text)
    For Each match in matches
        If writer(2) Then
            If Len(match.Value) > 0 Then
                Dim n
                For n = 1 To writer(1)
                    output = output & " + "\"    \"" + @"
                Next
                If InStr(match.Value, crlf) = -1 Then
                    output = output & match.Value
                    writer(2) = false
                Else
                    output = output & match.Value
                End If
            End If
        Else
            If Len(match.Value) > 0 Then
                If InStr(match.Value, crlf) = -1 Then
                    output = output & match.Value
                    writer(2) = true
                Else
                    output = output & match.Value
                End If
            End If
        End If
    Next
    WriteIndent = output
End Function

Function IsBool(x)
    IsBool = (VarType(x) = 11)
End Function

Sub Indent(writer)
    writer(1) = writer(1) + 1
End Sub

Sub Unindent(writer)
    If writer(1) > 0 Then
        writer(1) = writer(1) - 1
    End If
End Sub

Sub WriteToFile(writer, text)
    Dim fso
    Dim textStream
    Set fso = CreateObject(" + "\"Scripting.FileSystemObject\"" + @")
    If fso.FileExists(writer(0)) Then
        Set textStream = fso.OpenTextFile(writer(0), ForAppending, False, TriStateTrue)
    Else
        Set textStream = fso.OpenTextFile(writer(0), ForWriting, True, TriStateTrue)
    End If
    If IsBool(text) Then
        If CBool(text) Then
            textStream.Write WriteIndent(writer, ""1"")
        else
            textStream.Write WriteIndent(writer, ""0"")
        End If
    Else
        textStream.Write WriteIndent(writer, text)
    End If
    textStream.Close

    Set textStream = Nothing
    Set fso = Nothing
End Sub

Function GetVar(name)
    ExecuteGlobal " + "\"Dim var:var = \" & name" + @"
    GetVar = var
End Function

Function IsDefined(var)
    If GetVar(var) <> " + "\"\"" + @" Then
        IsDefined = True
    Else
        IsDefined = False
    End If
End Function

defaultWriter = Array(" + "\"" + Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(fileName) + ".txt" + "\"" + @", 0, true)
EraseFile " + "\"" + Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(fileName) + ".txt" + "\"" + @"



");
            foreach (IFunction f in this.implementedFunctions)
            {
                sw.Write("Sub " + (f.IsMacro ? "macro_" : f.IsJob ? "job_" : "func_") + f.Name + "(");
                bool first = true;
                // recherche que les paramètres qui ne sont pas calculables
                List<string> pars = f.Parameters.FindAll(new Predicate<IParameter>(delegate(IParameter par) {
                    return !par.IsComputable;
                })).ConvertAll(new Converter<IParameter,string>(delegate(IParameter par) {
                    return par.VariableName;
                }));
                List<string> distincts = new List<string>();
                foreach (string p in pars)
                {
                    if (!distincts.Contains(p))
                    {
                        if (!first) { sw.Write(","); } else { first = false; }
                        sw.Write(p);
                        distincts.Add(p);
                    }
                }
                sw.WriteLine(")");
                sw.Write(this.IndentSource(f.Source));
                sw.WriteLine("End Sub");
                sw.WriteLine(@"


");
            }
            sw.WriteLine(@"


");
            sw.WriteLine((this.Main.IsMacro ? "macro_" : this.Main.IsJob ? "job_" : "func_") + this.Main.Name);
            sw.WriteLine(@"



");
            sw.Close();
            sw.Dispose();
        }
        #endregion
    }
}
