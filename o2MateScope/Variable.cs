using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace o2Mate
{
    /// <summary>
    /// Implementation of a free variable (does not resides in a scope)
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE48")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Variable : IDataNotInScope
    {

        #region Public Static Constants
        /// <summary>
        /// Says it's a global variable
        /// </summary>
        public static string globalName = ScopeVariable.globalName;
        #endregion

        #region Private Fields
        private string name;
        private string realName;
        private EnumDataType type;
        private Dictionary<EnumDataType, PrefixInfo> prefixes;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor (the data type is a string by default)
        /// </summary>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        public Variable(string belongsTo, bool isComputable)
        {
            this.name = "";
            this.prefixes = new Dictionary<EnumDataType, PrefixInfo>();
            this.Set(Scope.StandardPrefix(EnumDataType.E_STRING), belongsTo, "0", isComputable, EnumDataType.E_STRING);
            this.UseThis(EnumDataType.E_STRING);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <param name="type">data type</param>
        public Variable(string belongsTo, bool isComputable, EnumDataType type)
        {
            this.name = "";
            this.prefixes = new Dictionary<EnumDataType, PrefixInfo>();
            this.Set(Scope.StandardPrefix(type), belongsTo, "0", isComputable, type);
            this.UseThis(type);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix">prefix name</param>
        /// <param name="belongsTo">current process name</param>
        /// <param name="isComputable">true if computable</param>
        /// <param name="type">data type</param>
        public Variable(string prefix, string belongsTo, bool isComputable, EnumDataType type)
        {
            this.name = "";
            this.prefixes = new Dictionary<EnumDataType, PrefixInfo>();
            this.Set(prefix, belongsTo, "0", isComputable, type);
            this.UseThis(type);
        }
        #endregion

        #region IData Membres

        /// <summary>
        /// Gets or sets the name of the variable
        /// </summary>
        public string Name
        {
            set { this.name = value; }
            get { return this.name; }
        }

        /// <summary>
        /// Gets the complete name of this variable (with the string prefix)
        /// </summary>
        public string PrefixedName
        {
            get { return this.prefixes[this.type].Prefix + this.RealName; }
        }

        /// <summary>
        /// True if it's a separator
        /// </summary>
        public bool IsSeparator
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the computability of this variable
        /// </summary>
        public bool IsComputable
        {
            get { return this.prefixes[this.type].IsComputable; }
            set
            {
                PrefixInfo cur = this.prefixes[this.type];
                this.prefixes[this.type] = new PrefixInfo(cur.Prefix, cur.BelongsTo, cur.Value, value);
            }
        }

        /// <summary>
        /// Gets the string value representation
        /// </summary>
        public string ValueString
        {
            get
            {
                return this.prefixes[this.type].Value;
            }
        }

        /// <summary>
        /// Gets the int value representation
        /// </summary>
        public int ValueInt
        {
            get
            {
                try
                {
                    int val = Int32.Parse(this.prefixes[this.type].Value);
                    return val;
                }
                catch { return 0; }
            }
        }

        /// <summary>
        /// Sets the string value representation
        /// </summary>
        public string Value
        {
            set
            {
                PrefixInfo cur = this.prefixes[this.type];
                this.prefixes[this.type] = new PrefixInfo(cur.Prefix, cur.BelongsTo, value, cur.IsComputable);
                this.prefixes[this.type].SetDirty();
            }
        }

        /// <summary>
        /// Gets or sets which process has declared this variable
        /// </summary>
        public string BelongsTo
        {
            get { return this.prefixes[this.type].BelongsTo; }
            set
            {
                PrefixInfo cur = this.prefixes[this.type];
                this.prefixes[this.type] = new PrefixInfo(cur.Prefix, value, cur.Value, cur.IsComputable);
            }
        }

        /// <summary>
        /// Gets or sets the real variable name in an another programming language
        /// </summary>
        public string RealName
        {
            get
            {
                if (String.IsNullOrEmpty(this.realName))
                {
                    return this.name;
                }
                else
                {
                    return this.realName;
                }
            }
            set { this.realName = value; }
        }

        /// <summary>
        /// Gets or sets the data type of this variable
        /// </summary>
        public EnumDataType DataType
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// Gets the prefix of this variable
        /// </summary>
        public string Prefix
        {
            get { return this.prefixes[this.type].Prefix; }
        }

        /// <summary>
        /// Says if this variable is global (known at root of the program)
        /// </summary>
        public bool IsGlobal
        {
            get { return this.prefixes[this.type].BelongsTo == ScopeVariable.globalName; }
        }

        /// <summary>
        /// Says if this variable has changed
        /// </summary>
        public bool IsDirty
        {
            get { return this.prefixes[this.type].IsDirty; }
        }

        /// <summary>
        /// Gets the scope where resides this variable (throw an exception when this variable is free)
        /// </summary>
        public IScope InnerScope
        {
            get { throw new InvalidCastException("L'objet référencé par l'interface IData n'appartient pas à un scope"); }
        }

        /// <summary>
        /// Copy from
        /// </summary>
        /// <param name="from">variable to read</param>
        /// <param name="changeScope">true if set the scope object</param>
        /// <returns>the modified variable</returns>
        public IData CopyFrom(IData from, bool changeScope)
        {
            if (from is Variable)
            {
                Variable x = from as Variable;
                if (!String.IsNullOrEmpty(x.realName))
                {
                    this.realName = x.realName;
                }
                else
                {
                    this.realName = null;
                }
                this.name = x.Name;
                this.type = x.DataType;
                foreach (KeyValuePair<EnumDataType, PrefixInfo> kv in x.prefixes)
                {
                    if (this.prefixes.ContainsKey(kv.Key))
                        this.prefixes[kv.Key] = new PrefixInfo(kv.Value.Prefix, kv.Value.BelongsTo, kv.Value.Value, kv.Value.IsComputable);
                    else
                        this.prefixes.Add(kv.Key, new PrefixInfo(kv.Value.Prefix, kv.Value.BelongsTo, kv.Value.Value, kv.Value.IsComputable));
                }
                this.UseThis(this.type);
                return this;
            }
            else if (from is ScopeVariable)
            {
                ScopeVariable x = from as ScopeVariable;
                if (!String.IsNullOrEmpty(x.realName))
                {
                    this.realName = x.realName;
                }
                else
                {
                    this.realName = null;
                }
                this.name = x.Name;
                this.type = x.DataType;
                foreach (KeyValuePair<EnumDataType, PrefixInfo> kv in x.prefixes)
                {
                    if (this.prefixes.ContainsKey(kv.Key))
                        this.prefixes[kv.Key] = new PrefixInfo(kv.Value.Prefix, kv.Value.BelongsTo, kv.Value.Value, kv.Value.IsComputable);
                    else
                        this.prefixes.Add(kv.Key, new PrefixInfo(kv.Value.Prefix, kv.Value.BelongsTo, kv.Value.Value, kv.Value.IsComputable));
                }
                this.UseThis(this.type);
                return this;                
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        /// <summary>
        /// Commit changes
        /// </summary>
        public void Commit()
        {
            this.prefixes[this.type].Commit();
        }

        /// <summary>
        /// Set infos to this variable
        /// </summary>
        /// <param name="prefix">prefix string form</param>
        /// <param name="belongsTo">process in which created</param>
        /// <param name="value">string value representation</param>
        /// <param name="isComputable">true if computable (gets a value)</param>
        /// <param name="dataType">data type</param>
        public void Set(string prefix, string belongsTo, string value, bool isComputable, EnumDataType dataType)
        {
            if (!this.prefixes.ContainsKey(dataType))
                this.prefixes.Add(dataType, new PrefixInfo(prefix, belongsTo, value, isComputable));
            else
                this.prefixes[dataType] = new PrefixInfo(prefix, belongsTo, value, isComputable);
        }

        /// <summary>
        /// Use this data type as the current data type value
        /// </summary>
        /// <param name="dataType">current data type</param>
        public void UseThis(EnumDataType dataType)
        {
            if (this.prefixes.ContainsKey(dataType))
            {
                this.type = dataType;
            }
            else
            {
                throw new ArgumentException("Le type de données '" + dataType.ToString() + "' demandé n'existe pas pour la variable '" + this.Name + "'");
            }
        }

        /// <summary>
        /// Returns true if this data type has been already created for this variable
        /// </summary>
        /// <param name="dataType">data type</param>
        /// <returns>true or false</returns>
        public bool TypeExists(EnumDataType dataType)
        {
            return this.prefixes.ContainsKey(dataType);
        }

        /// <summary>
        /// Gets the complete infos of this variable, given a data type
        /// </summary>
        /// <param name="dataType">data type to get infos</param>
        /// <returns>prefix info</returns>
        public PrefixInfo PrefixInfo(EnumDataType dataType)
        {
            if (this.prefixes.ContainsKey(dataType))
            {
                return this.prefixes[dataType];
            }
            else
            {
                throw new ArgumentException("Le type de données '" + dataType.ToString() + "' demandé n'existe pas pour la variable '" + this.Name + "'");
            }
        }

        #endregion

        #region IDataNotInScope Members
        /// <summary>
        /// True if this variable resides in a particular scope
        /// </summary>
        public bool IsInScope
        {
            get { return false; }
        }
        #endregion

    }
}
