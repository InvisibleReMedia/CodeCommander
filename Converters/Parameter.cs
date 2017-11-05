using System;
using System.Collections.Generic;
using System.Text;

namespace Converters
{
    /// <summary>
    /// Parameter class implementation
    /// </summary>
    public class Parameter : IParameter
    {
        #region Private Fields
        private string formalParameter;
        private string replacementParameter;
        private string effectiveParameter;
        private string variableName;
        private bool isDirty;
        private o2Mate.EnumDataType dataType;
        private EnumParameterOrder order = EnumParameterOrder.E_NEW;
        private bool isMutableParameter;
        private bool isComputable;
        private List<IStructure> structRef = new List<IStructure>();
        #endregion

        #region IParameter Membres

        /// <summary>
        /// Gets or sets the variable name
        /// Exact name (associated on a variable) of the effective parameter
        /// </summary>
        public string VariableName
        {
            get
            {
                return this.variableName;
            }
            set
            {
                this.variableName = value;
            }
        }

        /// <summary>
        /// Gets or sets the formal parameter name
        /// </summary>
        public string FormalParameter
        {
            get
            {
                return this.formalParameter;
            }
            set
            {
                this.formalParameter = value;
            }
        }

        /// <summary>
        /// Gets or sets the effective parameter name (from a template)
        /// </summary>
        public string EffectiveParameter
        {
            get
            {
                return this.effectiveParameter;
            }
            set
            {
                this.effectiveParameter = value;
            }
        }

        /// <summary>
        /// Gets or sets the replacement parameter name (from a mop)
        /// </summary>
        public string ReplacementParameter
        {
            get
            {
                return this.replacementParameter;
            }
            set
            {
                this.replacementParameter = value;
            }
        }

        /// <summary>
        /// Gets or sets if parameter has changed
        /// </summary>
        public bool IsDirty
        {
            get { return this.isDirty; }
            set { this.isDirty = value; }
        }

        /// <summary>
        /// Gets or sets the simple data type (in any language)
        /// </summary>
        public o2Mate.EnumDataType DataType
        {
            get
            {
                return this.dataType;
            }
            set
            {
                this.dataType = value;
            }
        }

        /// <summary>
        /// Gets or sets the parameter order
        /// </summary>
        public EnumParameterOrder Order
        {
            get
            {
                return this.order;
            }
            set
            {
                this.order = value;
            }
        }

        /// <summary>
        /// Gets or sets if the parameter is mutable
        /// </summary>
        public bool IsMutableParameter
        {
            get
            {
                return this.isMutableParameter;
            }
            set
            {
                this.isMutableParameter = value;
            }
        }

        /// <summary>
        /// Gets or sets if the parameter is computable
        /// </summary>
        public bool IsComputable
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
        /// Gets the encapsulated structure reference of this parameter
        /// </summary>
        public List<IStructure> StructureReferences
        {
            get
            {
                return this.structRef;
            }
        }

        /// <summary>
        /// Commit
        /// </summary>
        public void Commit()
        {
            this.isDirty = false;
        }

        #endregion

        #region ICloneable Member
        /// <summary>
        /// Clone it
        /// </summary>
        /// <returns>cloned object</returns>
        public object Clone()
        {
            Parameter p = new Parameter();
            p.dataType = this.dataType;
            p.effectiveParameter = this.effectiveParameter;
            p.formalParameter = this.formalParameter;
            p.isComputable = this.isComputable;
            p.isDirty = this.isDirty;
            p.isMutableParameter = this.isMutableParameter;
            p.order = this.order;
            p.replacementParameter = this.replacementParameter;
            p.structRef = new List<IStructure>();
            foreach (IStructure s in this.structRef)
            {
                p.structRef.Add(s.Clone() as IStructure);
            }
            return p;
        }
        #endregion
    }
}
