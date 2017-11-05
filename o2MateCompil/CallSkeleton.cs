using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class CallSkeleton : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string name;
        private int indent;
        private ICompilateurInstance cachedComp;
        private IProcessInstance cachedProc;
        #endregion

        #region Public Properties
        public string Name
        {
            get
            {
                return this.name;
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

        #region Static Public Methods
        public static string SpecialChars(string input)
        {
            const string availables = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJLMNOPQRSTUVWXYZ0123456789";
            string output = String.Empty;
            foreach (char c in input)
            {
                if (availables.IndexOf(c) != -1)
                {
                    output += c;
                }
                else
                {
                    output += "_";
                }
            }
            return output;
        }
        #endregion

        #region Compiler Membres

        public string TypeName
        {
            get { return "CallSkeleton"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
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
                if (this.name.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.name);
                        return false;
                    }
                    this.name = this.name.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.name = node.InnerText;
            this.cachedComp = comp;
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayCallSkeleton", new object[] { this.name, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            list.Add(new DisplayElement(this.TypeName, "displayCallSkeleton", new object[] { this.name, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.name = this.GetElementByName(elem, "skeleton").InnerText;
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("callskeleton");
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteString(this.name);
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.name = node.InnerText;
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
        }

        public void Convert(Converters.ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            this.cachedProc = proc;
            if (converter.ImplementedFunctions.Exists(new Predicate<IFunction>(delegate(IFunction f) { return f.StrictName == CallSkeleton.SpecialChars(this.name); })))
            {
                converter.Convert(this);
            }
            else
            {
                throw new Exception("Pour convertir le programme, les processus doivent être déclarés auparavant; le processus '" + this.name + "' n'a pas été déclaré auparavant");
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
            IFunction f = converter.ImplementedFunctions.Find(new Predicate<IFunction>(delegate(IFunction func)
            {
                return func.Name == this.cachedProc.FunctionName;
            }));
            if (!converter.CallingFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return func.StrictName == CallSkeleton.SpecialChars(this.Name) && func.InstanceNumber == f.InstanceNumber; })))
            {
                converter.CallingFunctions.Add(f);
            }
            converter.CurrentFunction.AddToSource(Helper.MakeNewMethodForCPP(this.cachedProc, converter, f));
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            IFunction f = converter.ImplementedFunctions.Find(new Predicate<IFunction>(delegate(IFunction func)
            {
                return func.Name == this.cachedProc.FunctionName;
            }));
            if (!converter.CallingFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return func.StrictName == CallSkeleton.SpecialChars(this.Name) && func.InstanceNumber == f.InstanceNumber; })))
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
            IFunction f = converter.ImplementedFunctions.Find(new Predicate<IFunction>(delegate(IFunction func)
            {
                return func.Name == this.cachedProc.FunctionName;
            }));
            if (!converter.CallingFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return func.StrictName == CallSkeleton.SpecialChars(this.Name) && func.InstanceNumber == f.InstanceNumber; })))
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
