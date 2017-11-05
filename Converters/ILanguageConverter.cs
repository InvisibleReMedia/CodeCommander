using System;
using System.Collections.Generic;
using System.Text;

namespace Converters
{
    /// <summary>
    /// Callback functions to redirect for specific language
    /// Any CodeCommander's statement have to implement these methods
    /// </summary>
    public interface ILanguageConverter
    {
        /// <summary>
        /// VBScript converter
        /// </summary>
        /// <param name="converter">converter object</param>
        void WriteInVBScript(ICodeConverter converter);
        /// <summary>
        /// PowerShell converter
        /// </summary>
        /// <param name="converter">converter object</param>
        void WriteInPowerShell(ICodeConverter converter);
        /// <summary>
        /// Perl converter
        /// </summary>
        /// <param name="converter">converter object</param>
        void WriteInPerl(ICodeConverter converter);
        /// <summary>
        /// Python converter
        /// </summary>
        /// <param name="converter">converter object</param>
        void WriteInPython(ICodeConverter converter);
        /// <summary>
        /// Microsoft CPP converter
        /// </summary>
        /// <param name="converter">converter object</param>
        void WriteInMicrosoftCPP(ICodeConverter converter);
        /// <summary>
        /// Unix CPP converter
        /// </summary>
        /// <param name="converter">converter object</param>
        void WriteInUnixCPP(ICodeConverter converter);
        /// <summary>
        /// Microsoft CSharp converter
        /// </summary>
        /// <param name="converter">converter object</param>
        void WriteInCSharp(ICodeConverter converter);
        /// <summary>
        /// Java converter
        /// </summary>
        /// <param name="converter">converter object</param>
        void WriteInJava(ICodeConverter converter);
        /// <summary>
        /// Mac OS CPP converter
        /// </summary>
        /// <param name="converter">converter object</param>
        void WriteInMacOSCPP(ICodeConverter converter);
    }
}
