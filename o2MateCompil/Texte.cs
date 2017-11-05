using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Texte : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string text;
        private int indent;
        private IData defaultWriter;
        private IData crlf;
        private ICompilateurInstance cachedComp;
        private IProcessInstance cachedProc;
        private List<o2Mate.ICompiler> parts;
        private Encoding enc;
        #endregion

        #region Constructors
        public Texte()
        {
        }

        public Texte(ICompilateurInstance comp, IProcessInstance proc)
        {
            this.cachedComp = comp;
            this.cachedProc = proc;
        }
        #endregion

        #region Public Properties
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
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
            get { return "Texte"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayText", new object[] { this.text, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayText", new object[] { this.text, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.text = this.GetElementByName(elem, "text").InnerText;
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("texte");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteString(this.text);
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            // remove whitespace
            string innerText = node.InnerText;
            this.text = innerText.Replace(" ", "").Replace("\t", "").Replace("\n\r", "");
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
            this.parts = new List<ICompiler>();
            Texte t = new Texte();
            t.Text = this.text;
            this.parts.Add(t);
            // les noms des paramètres sont triés par ordre du plus long au plus court
            foreach (IParameter param in proc.Replacements)
            {
                for(int index = 0; index < this.parts.Count; ++index)
                {
                    o2Mate.ICompiler part = this.parts[index];
                    if (part is o2Mate.Texte)
                    {
                        if (param.IsComputable)
                        {
                            (this.parts[index] as o2Mate.Texte).Text = (part as o2Mate.Texte).Text.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                        }
                        else
                        {
                            int pos = (part as o2Mate.Texte).Text.IndexOf(Compilateur.ParamIdentifier.ToString() + param.FormalParameter);
                            if (pos != -1)
                            {
                                Texte tOne = new Texte(this.cachedComp, this.cachedProc);
                                tOne.Text = (part as o2Mate.Texte).Text.Substring(0, pos);
                                this.parts.Insert(index, tOne);
                                ++index;
                                Variabble var = new Variabble(this.cachedComp, this.cachedProc);
                                var.VariableName = param.ReplacementParameter;
                                this.parts.Insert(index, var);
                                ++index;
                                Texte tThree = new Texte(this.cachedComp, this.cachedProc);
                                tThree.Text = (part as o2Mate.Texte).Text.Substring(pos + (Compilateur.ParamIdentifier.ToString() + param.FormalParameter).Length);
                                this.parts.Insert(index, tThree);
                                ++index;
                                this.parts.RemoveAt(index);
                                --index; // on traite la suite du texte
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            // remove whitespace
            string innerText = node.InnerText;
            this.text = innerText.Replace(" ", "").Replace("\t", "").Replace("\n\r", "");
            this.cachedComp = comp;
            this.enc = comp.EncodedFile;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            if (proc.CurrentScope.Exists(proc.DefaultWriter))
            {
                string writer = proc.CurrentScope.GetVariable(proc.DefaultWriter).ValueString;
                FinalFile.WriteToFile(ref writer, this.text.Replace(Environment.NewLine, "").Replace("\\","\\\\").Replace(" ", "").Replace("\t","").Replace('·', ' ').Replace('¬', '\t').Replace("¶", Environment.NewLine), this.enc);
                proc.CurrentScope.GetVariable(proc.DefaultWriter).Value = writer;
                proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "print"));
            }
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            this.cachedProc = proc;
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
                if (this.parts != null)
                {
                    foreach (ICompiler c in this.parts)
                    {
                        c.Convert(converter, proc, file);
                    }
                }
                else
                {
                    converter.Convert(this);
                }
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
            string printed = this.text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r\n", "").Replace("$[", "\\$[").Replace(" ", "").Replace("\t", "").Replace('·', ' ').Replace('¬', '\t').Replace("¶", "\") + " + this.crlf.PrefixedName + " + wstring(L\"");
            if (!this.defaultWriter.IsGlobal)
                converter.CurrentFunction.AddToSource("$[ifptr:" + MicrosoftCPPConverter.Escape(this.defaultWriter.PrefixedName) + "]WriteToFile(wstring(L\"" + printed + "\"));" + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource(MicrosoftCPPConverter.Escape(this.defaultWriter.PrefixedName) + ".WriteToFile(wstring(L\"" + printed + "\"));" + Environment.NewLine);
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            string printed = this.text.Replace("\"", "\"\"").Replace("\r\n", "").Replace(" ", "").Replace("\t", "").Replace('·', ' ').Replace('¬', '\t').Replace("¶", "\" & " + this.crlf.Name + " & \"");
            converter.CurrentFunction.AddToSource("WriteToFile " + this.defaultWriter + ", \"" + printed + "\"" + Environment.NewLine);
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            string printed = this.text.Replace("\\", "\\\\").Replace("\"", "\"\"").Replace("S[", "\\S[").Replace("\r\n", "").Replace(" ", "").Replace("\t", "").Replace('·', ' ').Replace('¬', '\t').Replace("¶", "\" + $" + this.crlf.Name + " + \"");
            if (!this.defaultWriter.IsGlobal)
                converter.CurrentFunction.AddToSource("WriteToFile S[byvalue:" + PowerShellConverter.Escape(this.defaultWriter.Name) + "] (\"" + printed + "\")" + Environment.NewLine);
            else
                converter.CurrentFunction.AddToSource("WriteToFile $" + PowerShellConverter.Escape(this.defaultWriter.Name) + " (\"" + printed + "\")" + Environment.NewLine);
        }

        #endregion
    }
}
