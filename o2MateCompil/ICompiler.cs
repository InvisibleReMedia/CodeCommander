using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using Converters;
using System.ComponentModel;

namespace o2Mate
{
    /// <summary>
    /// Interface declaration of a statement
    /// </summary>
    public interface ICompiler
    {
        /// <summary>
        /// Gets the type name of an instance of this class
        /// </summary>
        string TypeName { get; }
        /// <summary>
        /// Extracts dictionary keys
        /// </summary>
        /// <param name="proc">current instance process</param>
        void ExtractDictionary(IProcessInstance proc);
        /// <summary>
        /// Load statement data
        /// </summary>
        /// <param name="comp">compiler instance object</param>
        /// <param name="node">xml node object</param>
        void Load(ICompilateurInstance comp, XmlNode node);
        /// <summary>
        /// Injects a statement
        /// </summary>
        /// <param name="injector">injector where to inject</param>
        /// <param name="comp">compiler instance object</param>
        /// <param name="node">xml node object</param>
        /// <param name="modifier">modifier switch</param>
        void Inject(Injector injector, ICompilateurInstance comp, XmlNode node, string modifier);
        /// <summary>
        /// Says if a statement is computable when it's injected
        /// </summary>
        /// <param name="proc">process to test</param>
        /// <param name="reason">error text</param>
        /// <returns>true if succeeded else false (reason parameter contains explanation)</returns>
        bool IsComputable(IProcess proc, out string reason);
        /// <summary>
        /// Displays a statement into the web browser
        /// </summary>
        /// <param name="list">binding data to insert</param>
        /// <param name="node">node tree of statements</param>
        /// <param name="forcedIndent">true if sets at a particular indentation</param>
        /// <param name="indent">a particular indentation</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent);
        /// <summary>
        /// Displays a statement into the web browser in a read only mode
        /// </summary>
        /// <param name="list">binding data to insert</param>
        /// <param name="node">node tree of statements</param>
        /// <param name="forcedIndent">true if sets at a particular indentation</param>
        /// <param name="indent">a particular indentation</param>
        [System.Runtime.InteropServices.ComVisible(false)]
        void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent);
        /// <summary>
        /// Updates data from the web browser (before save)
        /// </summary>
        /// <param name="elem">html element</param>
        void Update(HtmlElement elem);
        /// <summary>
        /// Save a statement into an xml file
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="writer">xml writer object</param>
        /// <param name="child">html child to save</param>
        void Save(ICompilateur comp, XmlWriter writer, ref HtmlElement child);
        /// <summary>
        /// Parse is not load
        /// </summary>
        /// <param name="comp">compiler object</param>
        /// <param name="node">xml node from reading</param>
        void Parse(ICompilateur comp, XmlNode node);
        /// <summary>
        /// Process the statement and writes results in a file
        /// </summary>
        /// <param name="proc">process instance object</param>
        /// <param name="file">final file object</param>
        void WriteToFile(IProcessInstance proc, FinalFile file);
        /// <summary>
        /// Converts a statement for a particular programming language destination
        /// </summary>
        /// <param name="converter">converter object</param>
        /// <param name="proc">process instance object</param>
        /// <param name="file">final file</param>
        void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file);
    }
}
