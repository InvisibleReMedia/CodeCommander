using System;
using System.Collections.Generic;
using System.Text;

namespace Converters
{
    /// <summary>
    /// Control flow
    /// This enum specifies the meaning of using variable
    /// </summary>
    public enum EnumControlFlow
    {
        /// <summary>
        /// NONE
        /// </summary>
        E_NONE,
        /// <summary>
        /// IN A FUNCTION CONTROL FLOW
        /// </summary>
        E_FUNC,
        /// <summary>
        /// IN A LOOP CONTROL FLOW
        /// </summary>
        E_IN_LOOP,
        /// <summary>
        /// IN CONDITION CONTROL FLOW
        /// </summary>
        E_IN_CONDITION,
        /// <summary>
        /// AFTER A LOOP
        /// </summary>
        E_AFTER_LOOP,
        /// <summary>
        /// AFTER A CONDITION
        /// </summary>
        E_AFTER_CONDITION
    }
}
