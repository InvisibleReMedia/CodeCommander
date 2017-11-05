using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace o2Mate
{
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE48")]
    [ClassInterface(ClassInterfaceType.None)]
    internal class ScopeVariable : IData
    {
        #region Public Static Constants
        public static string globalName = "global";
        #endregion

        #region Private or Internal Fields
        private IScope scopeRef;
        private string name;
        internal string realName;
        private EnumDataType type;
        internal Dictionary<EnumDataType, PrefixInfo> prefixes;
        #endregion

        #region Constructors
        internal ScopeVariable(IScope innerScope, string belongsTo, bool isComputable)
        {
            this.scopeRef = innerScope;
            this.name = "";
            this.prefixes = new Dictionary<EnumDataType, PrefixInfo>();
            this.Set(Scope.StandardPrefix(EnumDataType.E_STRING), belongsTo, "0", isComputable, EnumDataType.E_STRING);
            this.UseThis(EnumDataType.E_STRING);
        }

        internal ScopeVariable(IScope innerScope, string belongsTo, bool isComputable, EnumDataType type)
        {
            this.scopeRef = innerScope;
            this.name = "";
            this.prefixes = new Dictionary<EnumDataType, PrefixInfo>();
            this.Set(Scope.StandardPrefix(type), belongsTo, "0", isComputable, type);
            this.UseThis(type);
        }

        internal ScopeVariable(IScope innerScope, string prefix, string belongsTo, bool isComputable, EnumDataType type)
        {
            this.scopeRef = innerScope;
            this.name = "";
            this.prefixes = new Dictionary<EnumDataType, PrefixInfo>();
            this.Set(prefix, belongsTo, "0", isComputable, type);
            this.UseThis(type);
        }
        #endregion

        #region IData Membres

        public string Name
        {
            set { this.name = value; }
            get { return this.name; }
        }

        public string PrefixedName
        {
            get { return this.prefixes[this.type].Prefix + this.RealName; }
        }

        public bool IsSeparator
        {
            get { return false; }
        }

        public bool IsComputable
        {
            get { return this.prefixes[this.type].IsComputable; }
            set
            {
                PrefixInfo cur = this.prefixes[this.type];
                this.prefixes[this.type] = new PrefixInfo(cur.Prefix, cur.BelongsTo, cur.Value, value);
            }
        }

        public string ValueString
        {
            get
            {
                return this.prefixes[this.type].Value;
            }
        }

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

        public string Value
        {
            set
            {
                PrefixInfo cur = this.prefixes[this.type];
                this.prefixes[this.type] = new PrefixInfo(cur.Prefix, cur.BelongsTo, value, cur.IsComputable);
                this.prefixes[this.type].SetDirty();
            }
        }

        public string BelongsTo
        {
            get { return this.prefixes[this.type].BelongsTo; }
            set
            {
                PrefixInfo cur = this.prefixes[this.type];
                this.prefixes[this.type] = new PrefixInfo(cur.Prefix, value, cur.Value, cur.IsComputable);
            }
        }

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

        public EnumDataType DataType
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public string Prefix
        {
            get { return this.prefixes[this.type].Prefix; }
        }

        public bool IsGlobal
        {
            get { return this.prefixes[this.type].BelongsTo == ScopeVariable.globalName; }
        }

        public bool IsDirty
        {
            get { return this.prefixes[this.type].IsDirty; }
        }

        public IScope InnerScope
        {
            get { return this.scopeRef; }
        }

        public IData CopyFrom(IData from, bool changeScope)
        {
            if (from is ScopeVariable)
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
                if (changeScope)
                    this.scopeRef = x.scopeRef;
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

        public void Commit()
        {
            this.prefixes[this.type].Commit();
        }

        public void Set(string prefix, string belongsTo, string value, bool isComputable, EnumDataType dataType)
        {
            if (!this.prefixes.ContainsKey(dataType))
                this.prefixes.Add(dataType, new PrefixInfo(prefix, belongsTo, value, isComputable));
            else
                this.prefixes[dataType] = new PrefixInfo(prefix, belongsTo, value, isComputable);
        }

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

        public bool TypeExists(EnumDataType dataType)
        {
            return this.prefixes.ContainsKey(dataType);
        }

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

    }
}
