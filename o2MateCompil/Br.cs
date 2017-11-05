using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Br : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private int indent;
        private IData defaultWriter;
        private IData crlf;
        private Encoding enc;
        private ICompilateurInstance cachedComp;
        #endregion

        #region Compiler Membres

        public string TypeName
        {
            get { return "Br"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("br");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
        }

        public void Inject(Injector injector, ICompilateurInstance comp, System.Xml.XmlNode node, string modifier)
        {
            this.Load(comp, node);
        }

        public bool IsComputable(IProcess proc, out string reason)
        {
            reason = null;
            return false;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.enc = comp.EncodedFile;
            this.cachedComp = comp;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            file.WriteToFile(Environment.NewLine, this.enc);
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            if (proc.CurrentScope.Exists("CrLf"))
            {
                this.crlf = proc.CurrentScope.GetVariable("CrLf");
            }
            else
            {
                this.crlf = new Variable(proc.Name, true);
                this.crlf.Value = @"\r\n";
            }
            if (proc.CurrentScope.Exists(proc.DefaultWriter))
            {
                this.defaultWriter = proc.CurrentScope.GetVariable(proc.DefaultWriter);
                if (!this.defaultWriter.IsGlobal)
                {
                    this.cachedComp.UpdateParameters(converter, proc, this.defaultWriter.Name, true);
                }
                converter.Convert(this);
            }
        }

        #endregion

        #region ILanguageConverter Membres

        public void WriteInPerl(ICodeConverter converter)
        {
        }

        public void WriteInPython(ICodeConverter converter)
        {
        }

        public void WriteInUnixCPP(ICodeConverter converter)
        {
        }

        public void WriteInCSharp(ICodeConverter converter)
        {
        }

        public void WriteInJava(ICodeConverter converter)
        {
        }

        public void WriteInMacOSCPP(ICodeConverter converter)
        {
        }

        public void WriteInMicrosoftCPP(ICodeConverter converter)
        {
            if (!this.defaultWriter.IsGlobal)
                converter.CurrentFunction.AddToSource("$[ifptr:" + MicrosoftCPPConverter.Escape(this.defaultWriter.PrefixedName) + "]WriteToFile(" + MicrosoftCPPConverter.Escape(this.crlf.PrefixedName) + ");" + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource(MicrosoftCPPConverter.Escape(this.defaultWriter.PrefixedName) + ".WriteToFile(" + MicrosoftCPPConverter.Escape(this.crlf.PrefixedName) + ");" + Environment.NewLine);
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource("WriteToFile " + this.defaultWriter + ", " + this.crlf.Name + Environment.NewLine);
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            if (!this.defaultWriter.IsGlobal)
                converter.CurrentFunction.AddToSource("WriteToFile S[byvalue:" + PowerShellConverter.Escape(this.defaultWriter.Name) + "] $" + PowerShellConverter.Escape(this.crlf.Name) + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource("WriteToFile $" + PowerShellConverter.Escape(this.defaultWriter.Name) + " $" + PowerShellConverter.Escape(this.crlf.Name) + Environment.NewLine);
        }

        #endregion
    }
}
