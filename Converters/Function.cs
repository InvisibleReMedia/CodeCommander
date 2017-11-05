using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Converters
{
    /// <summary>
    /// any function from a language
    /// </summary>
    public class Function : IFunction, IEquatable<IFunction>
    {
        #region Private Constants
        private readonly string IndentString = "    ";
        #endregion

        #region Private Fields
        private string name;
        private bool isJob;
        private bool isMacro;
        private bool isVisible;
        private int instanceNumber;
        private string source;
        private string cacheSource;
        private o2Mate.EnumDataType dataTypeResult;
        private bool isComputable;
        private bool isByRef;
        private string additionalSource;
        private List<IParameter> parameters;
        private List<IStructure> locals;
        private Dictionary<string, string> instancesStructure;
        private List<string> used;
        private int indentSize;
        private int variableCounter;
        private bool cr;
        private Stack<EnumControlFlow> stackControlFlow;
        #endregion

        #region Default Constructor
        /// <summary>
        /// construct a new function
        /// </summary>
        public Function()
        {
            this.parameters = new List<IParameter>();
            this.locals = new List<IStructure>();
            this.instancesStructure = new Dictionary<string, string>();
            this.used = new List<string>();
            this.stackControlFlow = new Stack<EnumControlFlow>();
            this.source = "";
            this.cacheSource = "";
            this.additionalSource = "";
            this.cr = true;
            this.isJob = false;
            this.isMacro = false;
            this.isVisible = true;
            this.isComputable = true;
            this.instanceNumber = 0;
            this.variableCounter = 0;
            this.indentSize = 0;
        }
        #endregion

        #region Private Methods
        private string Repeat(string input, int round)
        {
            string output = "";
            for (int counter = 0; counter < round; ++counter)
            {
                output += input;
            }
            return output;
        }

        private string IndentSource(int indent, string source)
        {
            string output = "";
            Regex reg = new Regex("((.*" + Environment.NewLine + ")|.*$)");
            MatchCollection matches = reg.Matches(source);
            foreach (Match m in matches)
            {
                if (m.Success)
                {
                    string v = m.Groups[1].Value;
                    if (!String.IsNullOrEmpty(v))
                    {
                        if (this.cr)
                        {
                            output += this.Repeat(this.IndentString, indent) + v;
                        }
                        else
                        {
                            output += v;
                        }
                        this.cr = v.EndsWith(Environment.NewLine);
                    }
                }
            }
            return output;
        }
        #endregion

        #region IFunction Membres

        /// <summary>
        /// Number of indentation in the source code
        /// </summary>
        public int IndentSize
        {
            get { return this.indentSize; }
            set { this.indentSize = value; }
        }

        /// <summary>
        /// Gives the current control flow
        /// </summary>
        public EnumControlFlow ControlFlow
        {
            get
            {
                if (this.stackControlFlow.Count > 0)
                    return this.stackControlFlow.Peek();
                else
                    return EnumControlFlow.E_NONE;
            }
        }

        /// <summary>
        /// Is true when the current control flow
        /// is none or at the root of a function
        /// </summary>
        public bool IsStaticControlFlow
        {
            get
            {
                bool result = true;
                foreach (EnumControlFlow cf in this.stackControlFlow)
                {
                    if (cf != EnumControlFlow.E_NONE && cf != EnumControlFlow.E_FUNC)
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public string Name
        {
            get
            {
                return this.name + (this.InstanceNumber > 0 ? "_" + this.InstanceNumber : "");
            }
        }

        /// <summary>
        /// Gets or sets strictly the name
        /// </summary>
        public string StrictName
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets or sets if function as a job
        /// </summary>
        public bool IsJob
        {
            get { return this.isJob; }
            set { this.isJob = value; }
        }

        /// <summary>
        /// Gets or sets if function as a macro
        /// </summary>
        public bool IsMacro
        {
            get { return this.isMacro; }
            set { this.isMacro = value; }
        }

        /// <summary>
        /// Gets or sets when a function is visible
        /// </summary>
        public bool IsVisible
        {
            get { return this.isVisible; }
            set { this.isVisible = value; }
        }

        /// <summary>
        /// Gets or sets an instance number
        /// This counts the number of functions with the same name
        /// </summary>
        public int InstanceNumber
        {
            get { return this.instanceNumber; }
            set { this.instanceNumber = value; }
        }

        /// <summary>
        /// Gets or sets the numerous of private variables
        /// This allows to create automatic variable
        /// that are different of user variables
        /// </summary>
        public int PrivateVariableCounter
        {
            get { return this.variableCounter; }
            set { this.variableCounter = value; }
        }

        /// <summary>
        /// Gets the function parameters
        /// </summary>
        public List<IParameter> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Gets local variables
        /// </summary>
        public List<IStructure> LocalVariables
        {
            get { return this.locals; }
        }

        /// <summary>
        /// Gets local variables
        /// </summary>
        public Dictionary<string,string> InstancesStructure
        {
            get { return this.instancesStructure; }
        }

        /// <summary>
        /// Gets used variables
        /// This tells what variables are used
        /// </summary>
        public List<string> UsedVariables
        {
            get { return this.used; }
        }

        /// <summary>
        /// Gets or sets the entire function body
        /// </summary>
        public string Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
            }
        }

        /// <summary>
        /// Gets or sets the expression string converted on the fly
        /// </summary>
        public string CacheSource
        {
            get
            {
                return this.cacheSource;
            }
            set
            {
                this.cacheSource = value;
            }
        }

        /// <summary>
        /// Gets or sets known data type (of any programming language)
        /// </summary>
        public o2Mate.EnumDataType DataTypeResult
        {
            get
            {
                return this.dataTypeResult;
            }
            set
            {
                this.dataTypeResult = value;
            }
        }

        /// <summary>
        /// Gets or sets if the expression must return by reference
        /// </summary>
        public bool IsByReferenceReturn
        {
            get
            {
                return this.isByRef;
            }
            set
            {
                this.isByRef = value;
            }
        }

        /// <summary>
        /// Gets or sets if the current evaluated expression
        /// is computable
        /// </summary>
        public bool IsComputableExpression
        {
            get
            {
                return this.isComputable;
            }
            set
            {
                this.isComputable = value;
            }
        }

        /// <summary>
        /// Gets or sets intermediate statements
        /// </summary>
        public string AdditionalSource
        {
            get
            {
                return this.additionalSource;
            }
            set
            {
                this.additionalSource = value;
            }
        }

        /// <summary>
        /// Make function info empty
        /// Ideal for renewing the function
        /// </summary>
        public void Clear()
        {
            this.parameters.Clear();
            this.locals.Clear();
            this.used.Clear();
            this.stackControlFlow.Clear();
            this.source = "";
            this.cacheSource = "";
            this.additionalSource = "";
            this.cr = true;
            this.isMacro = false;
            this.isComputable = true;
            this.variableCounter = 0;
            this.indentSize = 0;
        }
        
        /// <summary>
        /// Inserts a new statement in the function body
        /// </summary>
        /// <param name="source">source</param>
        public void AddToSource(string source)
        {
            this.Source += this.IndentSource(this.IndentSize, source);
        }

        /// <summary>
        /// Keeps control flow among multiple functions
        /// </summary>
        /// <param name="f">current function</param>
        public void PropagateControlFlow(IFunction f)
        {
            if (f is Function)
            {
                Function obj = f as Function;
                this.stackControlFlow = new Stack<EnumControlFlow>(obj.stackControlFlow);
            }
        }

        /// <summary>
        /// Assumes to be in a loop
        /// </summary>
        public void ForwardControlFlowInLoop()
        {
            this.stackControlFlow.Push(EnumControlFlow.E_IN_LOOP);
        }

        /// <summary>
        /// Assumes to be after a loop
        /// </summary>
        public void ForwardControlFlowAfterLoop()
        {
            while (this.ControlFlow != EnumControlFlow.E_IN_LOOP) this.stackControlFlow.Pop();
            this.stackControlFlow.Push(EnumControlFlow.E_AFTER_LOOP);
        }

        /// <summary>
        /// Assumes to start a function
        /// </summary>
        public void ForwardControlFlowSub()
        {
            this.stackControlFlow.Push(EnumControlFlow.E_FUNC);
        }

        /// <summary>
        /// Assumes to end a function
        /// </summary>
        public void BackwardControlFlowSub()
        {
            while (this.ControlFlow != EnumControlFlow.E_FUNC)
                this.stackControlFlow.Pop();
        }

        /// <summary>
        /// Assumes to be in a condition
        /// </summary>
        public void ForwardControlFlowInIf()
        {
            this.stackControlFlow.Push(EnumControlFlow.E_IN_CONDITION);
        }

        /// <summary>
        /// Assumes to be after a condition
        /// </summary>
        public void ForwardControlFlowAfterIf()
        {
            while (this.ControlFlow != EnumControlFlow.E_IN_CONDITION) this.stackControlFlow.Pop();
            this.stackControlFlow.Push(EnumControlFlow.E_AFTER_CONDITION);
        }
        #endregion

        #region IEquatable<IFunction> Membres

        /// <summary>
        /// Check equality between 2 functions
        /// Must have same parameters
        /// Must have same local variables
        /// </summary>
        /// <param name="other">other function</param>
        /// <returns>true or false</returns>
        public bool Equals(IFunction other)
        {
            if (this.StrictName == other.StrictName)
            {
                if (this.Parameters.Count == other.Parameters.Count && this.LocalVariables.Count == other.LocalVariables.Count)
                {
                    // les nombres de paramètres sont égaux dans les deux objets
                    if (this.Parameters.Except(other.Parameters).Count() == 0 && this.LocalVariables.Except(other.LocalVariables).Count() == 0)
                        return true;
                    else return false;
                }
                else return false;
            }
            else return false;
        }

        #endregion
    }
}
