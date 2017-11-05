using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace o2Mate
{
    [ComVisible(false)]
    internal class Separator : IData
    {
        #region Private Fields
        private IScope scopeRef;
        #endregion

        #region Default Constructor
        public Separator(IScope innerScope)
        {
            this.scopeRef = innerScope;
        }
        #endregion

        #region IData Membres

        public string Name
        {
            get
            {
                return "Separator";
            }
            set
            {
            }
        }

        public string PrefixedName
        {
            get { return this.Prefix + this.Name; }
        }

        public bool IsSeparator
        {
            get { return true; }
        }

        public bool IsComputable
        {
            get { return false; }
            set { }
        }

        public string ValueString
        {
            get { return ""; }
        }

        public int ValueInt
        {
            get { return 0; }
        }

        public string Value
        {
            set { }
        }

        public string BelongsTo
        {
            get { return ""; }
            set { }
        }

        public string RealName
        {
            get { return ""; }
            set { }
        }

        public EnumDataType DataType
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public string Prefix
        {
            get { return ""; }
        }

        public bool IsGlobal
        {
            get { throw new NotSupportedException(); }
        }

        public bool IsDirty
        {
            get { return false; }
        }

        public IScope InnerScope
        {
            get { return this.scopeRef; }
        }

        public IData CopyFrom(IData from, bool changeScope)
        {
            throw new NotSupportedException("Un séparateur ne peut pas recopié l'argument passé en paramètre de CopyFrom()");
        }

        public void Commit()
        {
            throw new NotSupportedException();
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
