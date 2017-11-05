using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace o2Mate
{
    /// <summary>
    /// Interface declaration of a scope
    /// </summary>
    [CoClass(typeof(IScope))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IScope
    {
        /// <summary>
        /// Add a new variable in this scope
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="value">string value representation</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <returns>a reference to the newly created variable</returns>
        IData Add(string name, string value, string belongsTo, bool isComputable);
        /// <summary>
        /// Add a new variable in this scope
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="value">string value representation</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <param name="dataType">desired data type</param>
        /// <returns>a reference to the newly created variable</returns>
        IData Add(string name, string value, string belongsTo, bool isComputable, EnumDataType dataType);

        /// <summary>
        /// Add a new variable in this scope
        /// </summary>
        /// <param name="prefix">prefix name</param>
        /// <param name="name">variable name</param>
        /// <param name="value">string value representation</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <param name="dataType">desired data type</param>
        /// <returns>a reference to the newly created variable</returns>
        IData Add(string prefix, string name, string value, string belongsTo, bool isComputable, EnumDataType dataType);

        /// <summary>
        /// imports a scope variable
        /// </summary>
        /// <param name="from">variable object to import</param>
        /// <returns>the new added variable</returns>
        IData Add(IData from);

        /// <summary>
        /// Update variable infos
        /// </summary>
        /// <param name="name">current name of the variable</param>
        /// <param name="value">string value representation</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <param name="dataType">desired data type</param>
        /// <returns>a reference to the updated variable</returns>
        IData Update(string name, string value, string belongsTo, bool isComputable, EnumDataType dataType);

        /// <summary>
        /// Update variable infos
        /// </summary>
        /// <param name="prefix">prefix name</param>
        /// <param name="name">current name of the variable</param>
        /// <param name="value">string value representation</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <param name="dataType">desired data type</param>
        /// <returns>a reference to the updated variable</returns>
        IData Update(string prefix, string name, string value, string belongsTo, bool isComputable, EnumDataType dataType);

        /// <summary>
        /// Copy infos from a variable to an another
        /// </summary>
        /// <param name="name">current variable name to modify</param>
        /// <param name="from">variable object to copy</param>
        /// <param name="makeInScope">true if modify scope's variable</param>
        /// <returns>the modified variable</returns>
        IData CopyFrom(string name, IData from, bool makeInScope);

        /// <summary>
        /// Get the nth-item variable
        /// </summary>
        /// <param name="index">index number</param>
        /// <returns>variable data or throw an exception</returns>
        IData Item(int index);

        /// <summary>
        /// Returns an iterator for reading all variables
        /// </summary>
        /// <returns>iterator</returns>
        IEnumerator GetEnumerator();

        /// <summary>
        /// Returns true if the variable name exists in this scope
        /// </summary>
        /// <param name="name">name of the variable to search</param>
        /// <returns>true or false</returns>
        bool Exists(string name);

        /// <summary>
        /// Returns a variable object from the scope by this name
        /// </summary>
        /// <param name="name">name of the variable to search</param>
        /// <returns>variable object or throw an exception</returns>
        IData GetVariable(string name);

        /// <summary>
        /// Remove a variable from the scope
        /// </summary>
        /// <param name="name">variable name</param>
        void Remove(string name);

        /// <summary>
        /// This function enqueue a new separator which means to
        /// start a new process
        /// </summary>
        void Push();

        /// <summary>
        /// This function dequeues all variable object until the first separator (and deletes it)
        /// </summary>
        void Pop();

        /// <summary>
        /// Gets or sets the parent's scope (in multi-threading model)
        /// </summary>
        IScope Parent { get; set; }
    }

    /// <summary>
    /// Implementation of a scope
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE49")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Scope : IScope, IEnumerable, ICloneable
    {
        #region Public Static Constants
        /// <summary>
        /// a standard prefix
        /// </summary>
        public static string GeneralPrefix = "local_";
        /// <summary>
        /// a prefix which serves for template's variable
        /// </summary>
        public static string TemplatePrefix = "auto_";
        #endregion

        #region Private Fields
        private List<IData> scope;
        private IScope parent;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Declares a new scope
        /// </summary>
        public Scope()
        {
            this.parent = null;
            this.scope = new List<IData>();
            this.Add("x_", "true", "1", ScopeVariable.globalName, false, EnumDataType.E_BOOL);
            this.scope[0].RealName = "trueValue";
            this.Add("x_", "false", "0", ScopeVariable.globalName, false, EnumDataType.E_BOOL);
            this.scope[1].RealName = "falseValue";
            this.Add("x_", "SpaceChar", " ", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "PlusChar", "+", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "MinusChar", "-", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "StarChar", "*", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "SlashChar", "/", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "ColonChar", ":", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "CommaChar", ",", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "DotChar", ".", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "ExChar", "!", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "IntChar", "?", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "EmptyChar", "", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "InfChar", "<", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "SupChar", ">", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "OpenParChar", "(", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "CloseParChar", ")", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "OpenBraChar", "{", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "CloseBraChar", "}", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "OpenSquaBraChar", "[", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "CloseSquaBraChar", "]", ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
            this.Add("x_", "CrLf", Environment.NewLine, ScopeVariable.globalName, false, EnumDataType.E_WCHAR);
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Constructs a new string accordingly with standard prefix in a particular type
        /// </summary>
        /// <param name="type">data type</param>
        /// <param name="varName">variable name</param>
        /// <returns></returns>
        public static string ConstructPrefixedName(EnumDataType type, string varName)
        {
            return Scope.StandardPrefix(type) + varName;
        }
        /// <summary>
        /// Returns a standard prefix by a data type
        /// </summary>
        /// <param name="dataType">data type</param>
        /// <returns>a specific prefix for this data type</returns>
        public static string StandardPrefix(EnumDataType dataType)
        {
            string prefix = String.Empty;
            switch (dataType)
            {
                case EnumDataType.E_ANY:
                    prefix = "x_"; break;
                case EnumDataType.E_BOOL:
                    prefix = "b_"; break;
                case EnumDataType.E_NUMBER:
                    prefix = "n_"; break;
                case EnumDataType.E_WCHAR:
                    prefix = "wch_"; break;
                case EnumDataType.E_STRING:
                    prefix = "s_"; break;
                case EnumDataType.E_STRING_OBJECT:
                    prefix = "ws_"; break;
                case EnumDataType.E_CONST_STRING_OBJECT:
                    prefix = "cws_"; break;
                case EnumDataType.E_WRITER:
                    prefix = "w_"; break;
                case EnumDataType.E_SIMPLETYPE:
                    prefix = "st_"; break;
                case EnumDataType.E_STRUCTURE:
                    prefix = "m_"; break;
            }
            return prefix;
        }
        #endregion

        #region Private Methods
        private string Search(string name)
        {
            for (int index = this.scope.Count - 1; index >= 0; --index)
            {
                IData data = this.scope[index];
                if (!data.IsSeparator)
                {
                    if (data.Name == name)
                    {
                        if (data is Reference)
                        {
                            // on change le nom de recherche et on continue la recherche en remontant
                            name = data.ValueString;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
            }
            if (this.parent != null)
            {
                return ("+" + (this.parent as Scope).Search(name));
            }
            return "";
        }

        private List<IData> IndexOf(string name)
        {
            IScope current = this;
            string result = this.Search(name);
            while (result.StartsWith("+"))
            {
                current = current.Parent;
                result = result.Substring(1);
            }
            return (current as Scope).scope;
        }
        #endregion

        #region IScope Membres

        /// <summary>
        /// Gets or sets the parent's scope (in multi-threading model)
        /// </summary>
        public IScope Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        /// <summary>
        /// Adds a new variable reference
        /// </summary>
        /// <param name="name">name of the variable</param>
        /// <param name="reference">name of the variable to reference</param>
        /// <param name="belongsTo">current process name</param>
        public void AddReference(string name, string reference, string belongsTo)
        {
            Reference r = new Reference(this, belongsTo);
            r.Name = name;
            r.Value = reference;
            this.scope.Add(r);
        }

        /// <summary>
        /// Add a new variable in this scope
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="value">string value representation</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <returns>a reference to the newly created variable</returns>
        public IData Add(string name, string value, string belongsTo, bool isComputable)
        {
            ScopeVariable var = new ScopeVariable(this, belongsTo, isComputable, EnumDataType.E_ANY);
            var.Name = name;
            var.Value = value;
            this.scope.Add(var);
            return var;
        }

        /// <summary>
        /// Add a new variable in this scope
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="value">string value representation</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <param name="dataType">desired data type</param>
        /// <returns>a reference to the newly created variable</returns>
        public IData Add(string name, string value, string belongsTo, bool isComputable, EnumDataType dataType)
        {
            ScopeVariable var = new ScopeVariable(this, belongsTo, isComputable, dataType);
            var.Name = name;
            var.Value = value;
            this.scope.Add(var);
            return var;
        }

        /// <summary>
        /// Add a new variable in this scope
        /// </summary>
        /// <param name="prefix">prefix name</param>
        /// <param name="name">variable name</param>
        /// <param name="value">string value representation</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <param name="dataType">desired data type</param>
        /// <returns>a reference to the newly created variable</returns>
        public IData Add(string prefix, string name, string value, string belongsTo, bool isComputable, EnumDataType dataType)
        {
            ScopeVariable var = new ScopeVariable(this, prefix, belongsTo, isComputable, dataType);
            var.Name = name;
            var.Value = value;
            this.scope.Add(var);
            return var;
        }

        /// <summary>
        /// imports a scope variable
        /// </summary>
        /// <param name="from">variable object to import</param>
        /// <returns>the new added variable</returns>
        public IData Add(IData from)
        {
            this.Add(from.Name, from.ValueString, from.BelongsTo, from.IsComputable);
            return this.CopyFrom(from.Name, from, false);
        }

        /// <summary>
        /// Update variable infos
        /// </summary>
        /// <param name="name">current name of the variable</param>
        /// <param name="value">string value representation</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <param name="dataType">desired data type</param>
        /// <returns>a reference to the updated variable</returns>
        public IData Update(string name, string value, string belongsTo, bool isComputable, EnumDataType dataType)
        {
            List<IData> list = this.IndexOf(name);
            int index = list.FindIndex(new Predicate<IData>(delegate(IData d)
            {
                return d.Name == name;
            }));
            if (index != -1)
            {
                ScopeVariable v = new ScopeVariable(this, belongsTo, isComputable, dataType);
                v.Name = name;
                v.Value = value;
                list[index].CopyFrom(v, true);
                return list[index];
            }
            else
            {
                throw new ArgumentException("La variable '" + name +"' n'existe pas");
            }
        }

        /// <summary>
        /// Update variable infos
        /// </summary>
        /// <param name="prefix">prefix name</param>
        /// <param name="name">current name of the variable</param>
        /// <param name="value">string value representation</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <param name="dataType">desired data type</param>
        /// <returns>a reference to the updated variable</returns>
        public IData Update(string prefix, string name, string value, string belongsTo, bool isComputable, EnumDataType dataType)
        {
            List<IData> list = this.IndexOf(name);
            int index = list.FindIndex(new Predicate<IData>(delegate(IData d)
            {
                return d.Name == name;
            }));
            if (index != -1)
            {
                ScopeVariable v = new ScopeVariable(this, prefix, belongsTo, isComputable, dataType);
                v.Name = name;
                v.Value = value;
                list[index].CopyFrom(v, true);
                return list[index];
            }
            else
            {
                throw new ArgumentException("La variable '" + name + "' n'existe pas");
            }
        }

        /// <summary>
        /// Copy infos from a variable to an another
        /// </summary>
        /// <param name="name">current variable name to modify</param>
        /// <param name="from">variable object to copy</param>
        /// <param name="makeInScope">true if modify scope's variable</param>
        /// <returns>the modified variable</returns>
        public IData CopyFrom(string name, IData from, bool makeInScope)
        {
            List<IData> list = this.IndexOf(name);
            int index = list.FindIndex(new Predicate<IData>(delegate(IData d)
            {
                return d.Name == name;
            }));
            if (index != -1)
            {
                list[index].CopyFrom(from, makeInScope);
                return list[index];
            }
            else
            {
                throw new ArgumentException("La variable '" + name + "' n'existe pas");
            }
        }

        /// <summary>
        /// Get the nth-item variable
        /// </summary>
        /// <param name="index">index number</param>
        /// <returns>variable data or throw an exception</returns>
        public IData Item(int index)
        {
            return this.scope[index];
        }

        /// <summary>
        /// Returns true if the variable name exists in this scope
        /// </summary>
        /// <param name="name">name of the variable to search</param>
        /// <returns>true or false</returns>
        public bool Exists(string name)
        {
            for (int index = this.scope.Count - 1; index >= 0; --index)
            {
                IData data = this.scope[index];
                if (!data.IsSeparator)
                {
                    if (data.Name == name)
                    {
                        return true;
                    }
                }
            }
            if (this.parent != null)
            {
                return this.parent.Exists(name);
            }
            return false;
        }

        /// <summary>
        /// Returns a variable object from the scope by this name
        /// </summary>
        /// <param name="name">name of the variable to search</param>
        /// <returns>variable object or throw an exception</returns>
        public IData GetVariable(string name)
        {
            for (int index = this.scope.Count - 1; index >= 0; --index)
            {
                IData data = this.scope[index];
                if (!data.IsSeparator)
                {
                    if (data.Name == name)
                    {
                        if (data is Reference)
                        {
                            // on change le nom de recherche et on continue la recherche en remontant
                            name = data.ValueString;
                        }
                        else
                        {
                            return data;
                        }
                    }
                }
            }
            if (this.parent != null)
            {
                return this.parent.GetVariable(name);
            }
            return null;
        }

        /// <summary>
        /// Remove a variable from the scope
        /// </summary>
        /// <param name="name">variable name</param>
        public void Remove(string name)
        {
            for (int index = this.scope.Count - 1; index >= 0; --index)
            {
                IData data = this.scope[index];
                if (!data.IsSeparator)
                {
                    if (data.Name == name)
                    {
                        this.scope.Remove(data);
                        return;
                    }
                }
            }
            if (this.parent != null)
            {
                this.parent.Remove(name);
            }
        }

        /// <summary>
        /// This function enqueue a new separator which means to
        /// start a new process
        /// </summary>
        public void Push()
        {
            Separator sep = new Separator(this);
            this.scope.Add(sep);
        }

        /// <summary>
        /// This function dequeues all variable object until the first separator (and deletes it)
        /// </summary>
        public void Pop()
        {
            for (int index = this.scope.Count - 1; index >= 0; --index)
            {
                IData data = this.scope[index];
                this.scope.RemoveAt(index);
                if (data.IsSeparator)
                {
                    return;
                }
            }
        }

        #endregion

        #region IEnumerable Membres

        /// <summary>
        /// Returns an iterator for reading all variables
        /// </summary>
        /// <returns>iterator</returns>
        public IEnumerator GetEnumerator()
        {
            return this.scope.GetEnumerator();
        }

        #endregion

        #region ICloneable Member
        /// <summary>
        /// Clone object
        /// </summary>
        /// <returns>a new scope</returns>
        public object Clone()
        {
            Scope s = new Scope();
            s.scope.Clear();
            foreach (IData d in this)
            {
                s.Add(d);
            }
            s.parent = this.parent;
            return s;
        }
        #endregion
    }
}
