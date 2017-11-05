using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class Call : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string processName;
        private int indent;
        private ICompilateurInstance cachedComp;
        private IProcessInstance cachedProc;
        #endregion

        #region Constructors
        public Call()
        {
            this.processName = "";
        }

        public Call(ICompilateurInstance comp, IProcessInstance proc)
        {
            this.cachedComp = comp;
            this.cachedProc = proc;
        }
        #endregion

        #region Public Properties
        public string ProcessName
        {
            get
            {
                return this.processName;
            }
            set
            {
                this.processName = value;
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
            get { return "Call"; }
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
            list.Add(new DisplayElement(this.TypeName, "displayCall", new object[] { this.processName, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayCall", new object[] { this.processName, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.processName = this.GetElementByName(elem, "process").InnerText;
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("call");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteString(this.processName);
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.processName = node.InnerText;
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
                if (this.processName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.processName);
                        return false;
                    }
                    this.processName = this.processName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.processName = node.InnerText;
            this.cachedComp = comp;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            this.cachedProc = proc;
            if (converter.ImplementedFunctions.Exists(new Predicate<IFunction>(delegate(IFunction f) { return f.StrictName == this.processName; })))
            {
                converter.Convert(this);
            }
            else
            {
                throw new Exception("Pour convertir le programme, les processus doivent être implémentés avant leur exécution; le processus '" + this.processName + "' n'a pas été déclaré.");
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
            IFunction f = converter.ImplementedFunctions.FindLast(new Predicate<IFunction>(delegate(IFunction func)
            {
                return func.Name == this.cachedProc.FunctionName;
            }));
            if (!converter.CallingFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return func.StrictName == this.processName && func.InstanceNumber == f.InstanceNumber; })))
            {
                converter.CallingFunctions.Add(f);
            }
            converter.CurrentFunction.AddToSource(Helper.MakeNewMethodForCPP(this.cachedProc, converter, f));
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            IFunction f = converter.ImplementedFunctions.FindLast(new Predicate<IFunction>(delegate(IFunction func)
            {
                return func.Name == this.cachedProc.FunctionName;
            }));
            if (!converter.CallingFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return func.StrictName == this.processName && func.InstanceNumber == f.InstanceNumber; })))
            {
                converter.CallingFunctions.Add(f);
            }
            converter.CurrentFunction.AddToSource((f.IsMacro ? "macro_" : f.IsJob ? "job_" : "func_") + f.Name + " ");
            bool first = true;
            List<string> names = new List<string>();
            foreach (IParameter p in f.Parameters)
            {
                if (!names.Contains(p.VariableName))
                {
                    if (!first) { converter.CurrentFunction.AddToSource(", "); } else { first = false; }
                    converter.CurrentFunction.AddToSource(p.VariableName);
                    names.Add(p.VariableName);
                }
            }
            converter.CurrentFunction.AddToSource(Environment.NewLine);
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            IFunction f = converter.ImplementedFunctions.FindLast(new Predicate<IFunction>(delegate(IFunction func)
            {
                return func.Name == this.cachedProc.FunctionName;
            }));
            if (!converter.CallingFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return func.StrictName == this.processName && func.InstanceNumber == f.InstanceNumber; })))
            {
                converter.CallingFunctions.Add(f);
            }
            converter.CurrentFunction.AddToSource(PowerShellConverter.Escape((f.IsMacro ? "macro_" : f.IsJob ? "job_" : "func_") + f.Name + " "));
            bool first = true;
            // ne pas ajouter des paramètres typés, utiliser des VariableName distinct
            List<string> distincts = new List<string>();
            foreach (IParameter p in f.Parameters)
            {
                if (!distincts.Contains(p.VariableName))
                {
                    if (!first) { converter.CurrentFunction.AddToSource(" "); } else { first = false; }
                    converter.CurrentFunction.AddToSource("-" + p.VariableName);
                    if (p.IsMutableParameter)
                        converter.CurrentFunction.AddToSource(" S[byref:" + p.VariableName + "]");
                    else
                        converter.CurrentFunction.AddToSource(" S[byvalue:" + p.VariableName + "]");
                    distincts.Add(p.VariableName);
                }
            }
            converter.CurrentFunction.AddToSource(Environment.NewLine);
        }

        #endregion
    }
}
