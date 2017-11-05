using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Variabble : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string varName;
        private IData value;
        private bool isFromDict;
        private int indent;
        private ICompilateurInstance cachedComp;
        private IProcessInstance cachedProc;
        private IData defaultWriter;
        private Encoding enc;
        #endregion

        #region Constructors
        public Variabble()
        {
        }

        public Variabble(ICompilateurInstance comp, IProcessInstance proc)
        {
            this.cachedComp = comp;
            this.cachedProc = proc;
        }
        #endregion

        #region Public Properties
        public string VariableName
        {
            get { return this.varName; }
            set { this.varName = value; }
        }
        #endregion

        #region Private Methods
        private System.Windows.Forms.HtmlElement GetElementByName(System.Windows.Forms.HtmlElement from, string name)
        {
            if (from.Name == name)
            {
                return from;
            }
            else
            {
                foreach (System.Windows.Forms.HtmlElement child in from.Children)
                {
                    try
                    {
                        return GetElementByName(child, name);
                    }
                    catch { }
                }
                throw new Exception("this sub-element does not contain that name element");
            }
        }
        #endregion

        #region Compiler Membres

        public string TypeName
        {
            get { return "Variable"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
            if (!proc.CurrentScope.Exists(this.varName))
            {
                proc.CurrentDictionnaire.AddString(this.varName, "");
            }
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayVariable", new object[] { this.varName, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayVariable", new object[] { this.varName, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.varName = this.GetElementByName(elem, "variable").InnerText;
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("variable");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteString(this.varName);
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.InnerText;
        }

        public void Inject(Injector injector, ICompilateurInstance comp, System.Xml.XmlNode node, string modifier)
        {
            this.Load(comp, node);
            if (comp.UnderConversion)
            {
                string s = String.Empty;
                if (!this.IsComputable(injector.InjectedProcess, out s))
                {
                    throw new Exception(s);
                }
            }
        }

        public bool IsComputable(IProcess proc, out string reason)
        {
            reason = null;
            // les noms des paramètres sont triés par ordre du plus long au plus court
            foreach (IParameter param in proc.Replacements)
            {
                if (this.varName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.varName);
                        return false;
                    }
                    this.varName = this.varName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.varName = node.InnerText;
            this.cachedComp = comp;
            this.enc = comp.EncodedFile;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            if (proc.CurrentScope.Exists(this.varName))
            {
                if (proc.CurrentScope.Exists(proc.DefaultWriter))
                {
                    string writer = proc.CurrentScope.GetVariable(proc.DefaultWriter).ValueString;
                    FinalFile.WriteToFile(ref writer, proc.CurrentScope.GetVariable(this.varName).ValueString, this.enc);
                    proc.CurrentScope.GetVariable(proc.DefaultWriter).Value = writer;
                    proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "print variable", this.varName));
                }
            }
            else if (proc.CurrentDictionnaire.Exists(this.varName))
            {
                if (proc.CurrentDictionnaire.IsString(this.varName))
                {
                    if (proc.CurrentScope.Exists(proc.DefaultWriter))
                    {
                        string writer = proc.CurrentScope.GetVariable(proc.DefaultWriter).ValueString;
                        FinalFile.WriteToFile(ref writer, proc.CurrentDictionnaire.GetString(this.varName), this.enc);
                        proc.CurrentScope.GetVariable(proc.DefaultWriter).Value = writer;
                        proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "print string", this.varName));
                    }
                }
            }
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            this.cachedProc = proc;
            if (proc.CurrentScope.Exists(this.varName))
            {
                this.isFromDict = false;
                this.value = proc.CurrentScope.GetVariable(this.varName);
                // update parameters for this variable to use
                this.cachedComp.UpdateParameters(converter, proc, this.varName, false);
            }
            else if (proc.CurrentDictionnaire.IsString(this.varName))
            {
                this.isFromDict = true;
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
            if (this.isFromDict)
            {
                if (!this.defaultWriter.IsGlobal)
                    converter.CurrentFunction.AddToSource("$[ifptr:" + MicrosoftCPPConverter.Escape(this.defaultWriter.PrefixedName) + "]WriteToFile(this->GetString(wstring(L\"" + MicrosoftCPPConverter.Escape(this.varName) + "\")));" + Environment.NewLine);
                else
                    converter.CurrentFunction.AddToSource(MicrosoftCPPConverter.Escape(this.defaultWriter.PrefixedName) + ".WriteToFile(this->GetString(wstring(L\"" + MicrosoftCPPConverter.Escape(this.varName) + "\")));" + Environment.NewLine);
            }
            else
            {
                if (!this.defaultWriter.IsGlobal)
                    converter.CurrentFunction.AddToSource("$[ifptr:" + MicrosoftCPPConverter.Escape(this.defaultWriter.PrefixedName) + "]WriteToFile($[byvalue:" + MicrosoftCPPConverter.Escape(this.value.PrefixedName) + "]);" + Environment.NewLine);
                else
                    converter.CurrentFunction.AddToSource(MicrosoftCPPConverter.Escape(this.defaultWriter.PrefixedName) + ".WriteToFile($[byvalue:" + MicrosoftCPPConverter.Escape(this.value.PrefixedName) + "]);" + Environment.NewLine);
            }
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            if (this.isFromDict)
            {
                converter.CurrentFunction.AddToSource("WriteToFile " + this.defaultWriter.Name + ", theDict.GetString(\"" + this.varName + "\")" + Environment.NewLine);
            }
            else
            {
                converter.CurrentFunction.AddToSource("WriteToFile " + this.defaultWriter.Name + ", " + this.varName + Environment.NewLine);
            }
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            if (this.isFromDict)
            {
                if (!this.defaultWriter.IsGlobal)
                    converter.CurrentFunction.AddToSource("WriteToFile S[byvalue:" + PowerShellConverter.Escape(this.defaultWriter.Name) + "] $theDict.GetString(\"" + PowerShellConverter.Escape(this.varName) + "\")" + Environment.NewLine);
                else
                    converter.CurrentFunction.AddToSource("WriteToFile $" + PowerShellConverter.Escape(this.defaultWriter.Name) + " $theDict.GetString(\"" + PowerShellConverter.Escape(this.varName) + "\")" + Environment.NewLine);
            }
            else
            {
                if (!this.defaultWriter.IsGlobal)
                    converter.CurrentFunction.AddToSource("WriteToFile S[byvalue:" + PowerShellConverter.Escape(this.defaultWriter.Name) + "] (S[byvalue:" + PowerShellConverter.Escape(this.varName) + "])" + Environment.NewLine);
                else
                    converter.CurrentFunction.AddToSource("WriteToFile $" + PowerShellConverter.Escape(this.defaultWriter.Name) + " (S[byvalue:" + PowerShellConverter.Escape(this.varName) + "])" + Environment.NewLine);
            }
        }

        #endregion
    }
}
