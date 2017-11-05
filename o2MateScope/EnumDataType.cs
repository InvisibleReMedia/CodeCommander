using System;
using System.Collections.Generic;
using System.Text;

namespace o2Mate
{
    /// <summary>
    /// Standard data type
    /// </summary>
    public enum EnumDataType
    {
        /// <summary>
        /// accepte tout type
        /// </summary>
        E_ANY,
        /// <summary>
        /// wchar_t type
        /// </summary>
        E_WCHAR,
        /// <summary>
        /// wchar_t* type
        /// </summary>
        E_STRING,
        /// <summary>
        /// int type
        /// </summary>
        E_NUMBER,
        /// <summary>
        /// bool type
        /// </summary>
        E_BOOL,
        /// <summary>
        /// wstring type
        /// </summary>
        E_STRING_OBJECT,
        /// <summary>
        /// const wstring type
        /// </summary>
        E_CONST_STRING_OBJECT,
        /// <summary>
        /// for writing into a file
        /// </summary>
        E_WRITER,
        /// <summary>
        /// SimpleType type (encapsulates a simple data type : bool,int,char_t,wchar_t* and wstring)
        /// </summary>
        E_SIMPLETYPE,
        /// <summary>
        /// The data type is a structure
        /// </summary>
        E_STRUCTURE,
        /// <summary>
        /// Void data type (C++)
        /// </summary>
        E_VOID
    }

}
