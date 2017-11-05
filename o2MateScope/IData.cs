using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace o2Mate
{
    /// <summary>
    /// Interface declaration of scope's variables
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IData
    {
        /// <summary>
        /// Gets or sets the name of the variable
        /// </summary>
        [DispId(1)]
        string Name { get; set; }
        /// <summary>
        /// True if it's a separator
        /// </summary>
        [DispId(2)]
        bool IsSeparator { get; }
        /// <summary>
        /// Gets the string value representation
        /// </summary>
        [DispId(3)]
        string ValueString { get; }
        /// <summary>
        /// Gets the int value representation
        /// </summary>
        [DispId(4)]
        int ValueInt { get; }
        /// <summary>
        /// Sets the string value representation
        /// </summary>
        [DispId(5)]
        string Value { set; }
        /// <summary>
        /// Gets or sets which process has declared this variable
        /// </summary>
        [DispId(6)]
        string BelongsTo { get; set; }
        /// <summary>
        /// Gets or sets the real variable name in an another programming language
        /// </summary>
        [DispId(7)]
        string RealName { get; set; }
        /// <summary>
        /// Gets or sets the computability of this variable
        /// </summary>
        [DispId(8)]
        bool IsComputable { get; set; }
        /// <summary>
        /// Gets or sets the data type of this variable
        /// </summary>
        [DispId(9)]
        EnumDataType DataType { get; set; }
        /// <summary>
        /// Says if this variable is global (known at root of the program)
        /// </summary>
        [DispId(10)]
        bool IsGlobal { get; }
        /// <summary>
        /// Copy from
        /// </summary>
        /// <param name="from">variable to read</param>
        /// <param name="changeScope">true if set the scope object</param>
        /// <returns>the modified variable</returns>
        [DispId(11)]
        IData CopyFrom(IData from, bool changeScope);
        /// <summary>
        /// Says if this variable has changed
        /// </summary>
        [DispId(12)]
        bool IsDirty { get; }
        /// <summary>
        /// Commit changes
        /// </summary>
        [DispId(13)]
        void Commit();
        /// <summary>
        /// Gets the prefix of this variable
        /// </summary>
        [DispId(14)]
        string Prefix { get; }
        /// <summary>
        /// Set infos to this variable
        /// </summary>
        /// <param name="prefix">prefix string form</param>
        /// <param name="belongsTo">process in which created</param>
        /// <param name="value">string value representation</param>
        /// <param name="isComputable">true if computable (gets a value)</param>
        /// <param name="dataType">data type</param>
        [DispId(15)]
        void Set(string prefix, string belongsTo, string value, bool isComputable, EnumDataType dataType);
        /// <summary>
        /// Use this data type as the current data type value
        /// </summary>
        /// <param name="dataType">current data type</param>
        [DispId(16)]
        void UseThis(EnumDataType dataType);
        /// <summary>
        /// Gets the complete name of this variable (with the string prefix)
        /// </summary>
        [DispId(17)]
        string PrefixedName { get; }
        /// <summary>
        /// Gets the complete infos of this variable, given a data type
        /// </summary>
        /// <param name="dataType">data type to get infos</param>
        /// <returns>prefix info</returns>
        [DispId(18)]
        PrefixInfo PrefixInfo(EnumDataType dataType);
        /// <summary>
        /// Returns true if this data type has been already created for this variable
        /// </summary>
        /// <param name="dataType">data type</param>
        /// <returns>true or false</returns>
        [DispId(19)]
        bool TypeExists(EnumDataType dataType);
        /// <summary>
        /// Gets the scope where resides this variable (throw an exception when this variable is free)
        /// </summary>
        [DispId(20)]
        IScope InnerScope { get; }
    }

    /// <summary>
    /// Interface to differenciate scope's variable or not
    /// </summary>
    public interface IDataNotInScope : IData
    {
        /// <summary>
        /// True if this variable resides in a particular scope
        /// </summary>
        bool IsInScope { get; }
    }

    /// <summary>
    /// Prefix info
    /// </summary>
    public struct PrefixInfo
    {
        /// <summary>
        /// Prefix value
        /// </summary>
        public string Prefix;
        /// <summary>
        /// Process name in which a variable is local
        /// </summary>
        public string BelongsTo;
        /// <summary>
        /// String value representation
        /// </summary>
        public string Value;
        /// <summary>
        /// True if computable
        /// </summary>
        public bool IsComputable;
        /// <summary>
        /// True if modified
        /// </summary>
        public bool IsDirty;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="prefix">prefix name</param>
        /// <param name="belongsTo">process name</param>
        /// <param name="value">string value representation</param>
        /// <param name="isComputable">true if computable</param>
        public PrefixInfo(string prefix, string belongsTo, string value, bool isComputable)
        {
            this.Prefix = prefix;
            this.BelongsTo = belongsTo;
            this.Value = value;
            this.IsComputable = isComputable;
            this.IsDirty = true;
        }

        /// <summary>
        /// Sets the dirty bit to 1
        /// </summary>
        public void SetDirty()
        {
            this.IsDirty = true;
        }

        /// <summary>
        /// Sets the dirty bit to 0
        /// </summary>
        public void Commit()
        {
            this.IsDirty = false;
        }
    }
}
