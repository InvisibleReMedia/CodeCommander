using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Converters
{
    internal class Structure : IStructure
    {
        #region Private Fields
        private string instanceName;
        private string prefixName;
        private string fieldName;
        private bool isGlobal;
        private string structDataType;
        private o2Mate.EnumDataType dataType;
        private bool isMutable;
        private bool asStructure;
        #endregion

        #region Public Constructors
        /// <summary>
        /// Creates a new field into a structure to hold a variable in a scope with its current data type
        /// </summary>
        /// <param name="name">name of the instance structure</param>
        /// <param name="d">variable object</param>
        /// <param name="isMutable">mutable switch</param>
        public Structure(string name, o2Mate.IData d, bool isMutable)
        {
            this.instanceName = name;
            this.asStructure = false;
            if (d != null)
            {
                this.fieldName = d.Name;
                this.prefixName = d.Prefix;
                this.dataType = d.DataType;
                this.isMutable = isMutable;
                this.isGlobal = d.IsGlobal;
            }
            else throw new ArgumentException("Field object cannot be null");
        }

        /// <summary>
        /// Creates a new field into a structure to hold
        /// </summary>
        /// <param name="name">name of the instance structure</param>
        /// <param name="fieldName">field name</param>
        /// <param name="dataType">data type</param>
        /// <param name="isMutable">mutable switch</param>
        public Structure(string name, string fieldName, o2Mate.EnumDataType dataType, bool isMutable)
        {
            this.instanceName = name;
            this.asStructure = false;
            int posUnderscore = fieldName.IndexOf('_');
            if (posUnderscore != -1)
            {
                this.prefixName = fieldName.Substring(0, posUnderscore + 1);
                this.fieldName = fieldName.Substring(posUnderscore + 1);
            }
            else
            {
                this.prefixName = o2Mate.Scope.StandardPrefix(dataType);
                this.fieldName = fieldName;
            }
            this.dataType = dataType;
            this.isMutable = isMutable;
            this.isGlobal = false;
        }

        /// <summary>
        /// Creates a new field into a structure with a structure data type
        /// </summary>
        /// <param name="name">name of the instance structure</param>
        /// <param name="structDataType">structure data type name</param>
        /// <param name="fieldName">field name</param>
        /// <param name="isMutable">mutable switch</param>
        public Structure(string name, string structDataType, string fieldName, bool isMutable)
        {
            this.instanceName = name;
            this.asStructure = true;
            int posUnderscore = fieldName.IndexOf('_');
            if (posUnderscore != -1)
            {
                this.prefixName = fieldName.Substring(0, posUnderscore + 1);
                this.fieldName = fieldName.Substring(posUnderscore + 1);
            }
            else
            {
                this.prefixName = o2Mate.Scope.StandardPrefix(o2Mate.EnumDataType.E_STRUCTURE);
                this.fieldName = fieldName;
            }
            this.structDataType = structDataType;
            this.dataType = o2Mate.EnumDataType.E_STRUCTURE;
            this.isMutable = isMutable;
            this.isGlobal = false;
        }

        #endregion

        #region IStructure Members
        /// <summary>
        /// Gets the instance name
        /// </summary>
        public string InstanceName
        {
            get
            {
                return this.instanceName;
            }
        }

        /// <summary>
        /// Gets the field name in a structure
        /// </summary>
        public string FieldName
        {
            get
            {
                return this.fieldName;
            }
        }

        /// <summary>
        /// Gets the prefixed field name
        /// </summary>
        public string PrefixedFieldName
        {
            get
            {
                return this.prefixName + this.fieldName;
            }
        }

        /// <summary>
        /// Gets the data type field
        /// </summary>
        public o2Mate.EnumDataType DataType
        {
            get
            {
                return this.dataType;
            }
        }

        /// <summary>
        /// Gets the structure string data type as a field
        /// </summary>
        public string StructureType
        {
            get
            {
                if (this.asStructure)
                    return this.structDataType;
                else
                    return this.DataType.ToString();
            }
        }

        /// <summary>
        /// Says if it's a global field
        /// </summary>
        public bool IsGlobal
        {
            get
            {
                if (!this.asStructure)
                    return this.isGlobal;
                else
                    return false;
            }
        }

        /// <summary>
        /// Says if this field is mutable
        /// </summary>
        public bool IsMutable
        {
            get { return this.isMutable; }
        }

        /// <summary>
        /// Says if this field is a structure itself
        /// </summary>
        public bool IsItself
        {
            get { return this.asStructure; }
        }

        #endregion

        #region ICloneable Member

        public object Clone()
        {
            Structure s;
            if (!this.asStructure)
            {
                s = new Structure(this.instanceName, this.structDataType, this.fieldName, this.isMutable);
                s.isGlobal = this.isGlobal;
            }
            else
            {
                s = new Structure(this.instanceName, this.fieldName, this.dataType, this.isMutable);
            }
            return s;
        }

        #endregion
    }
}
