using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace o2Mate
{
    [ComVisible(false)]
    internal class Reference : IData
    {
        #region Private Fields
        private IScope scopeRef;
        private string name;
        private string value;
        private string belongsTo;
        private string realName;
        private EnumDataType type;
        private bool isDirty;
        #endregion

        #region Default Constructor
        public Reference(IScope innerScope, string belongsTo)
        {
            this.scopeRef = innerScope;
            this.name = "";
            this.value = "0";
            this.belongsTo = belongsTo;
            this.type = EnumDataType.E_ANY;
            this.isDirty = false;
        }
        #endregion

        #region IData Membres

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public string PrefixedName
        {
            get { return this.Prefix + this.Name; }
        }

        public bool IsSeparator
        {
            get { return false; }
        }

        public bool IsComputable
        {
            get { return false; }
            set { }
        }

        public string ValueString
        {
            get { return this.value; }
        }

        public int ValueInt
        {
            get { return 0; }
        }

        public string Value
        {
            set
            {
                this.value = value;
                this.isDirty = true;
            }
        }

        public string BelongsTo
        {
            get
            {
                return this.belongsTo;
            }
            set
            {
                this.belongsTo = value;
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
            get { return ""; }
        }

        public bool IsGlobal
        {
            get { return this.belongsTo == ScopeVariable.globalName; }
        }

        public bool IsDirty
        {
            get { return this.isDirty; }
        }

        public IScope InnerScope
        {
            get { return this.scopeRef; }
        }

        public IData CopyFrom(IData from, bool changeScope)
        {
            if (from is Reference)
            {
                Reference x = from as Reference;
                this.belongsTo = x.BelongsTo;
                this.type = x.DataType;
                this.name = x.Name;
                this.realName = x.realName;
                this.Value = x.ValueString;
                this.isDirty = true;
                if (changeScope) this.scopeRef = x.scopeRef;
                return this;
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public void Commit()
        {
            this.isDirty = false;
        }

        public void Set(string prefix, string belongsTo, string value, bool isComputable, EnumDataType dataType)
        {
            throw new NotSupportedException();
        }

        public void UseThis(EnumDataType dataType)
        {
            throw new NotSupportedException();
        }

        public bool TypeExists(EnumDataType dataType)
        {
            throw new NotSupportedException();
        }

        public PrefixInfo PrefixInfo(EnumDataType dataType)
        {
            throw new NotSupportedException();
        }

        #endregion

    }
}
