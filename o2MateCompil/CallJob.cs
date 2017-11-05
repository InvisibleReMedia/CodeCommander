using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class CallJob : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private string processName;
        private IStructure structRef;
        private ICompilateurInstance cachedComp;
        private IFunction[] existingFunctions;
        #endregion

        #region Constructors
        public CallJob()
        {
            this.processName = "";
        }

        public CallJob(ICompilateurInstance comp)
        {
            this.cachedComp = comp;
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

        public IStructure StructureReference
        {
            get { return this.structRef; }
            set { this.structRef = value; }
        }

        public IFunction[] ExistingFunctions
        {
            get { return this.existingFunctions; }
            set { this.existingFunctions = value; }
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
            get { return "CallJob"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            throw new NotImplementedException();
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            throw new NotImplementedException();
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            throw new NotImplementedException();
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            throw new NotImplementedException();
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            throw new NotImplementedException();
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
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
        }

        public void Convert(ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
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
            // aller chercher les variables exterieures au processus appellé
            IFunction f = converter.ImplementedFunctions.FindLast(new Predicate<IFunction>(delegate(IFunction func) { return func.IsJob && func.StrictName == this.processName; }));
            if (!converter.CallingFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return func.IsJob && func.StrictName == this.processName && func.InstanceNumber == f.InstanceNumber; })))
            {
                converter.CallingFunctions.Add(f);
            }

            ++converter.CurrentFunction.PrivateVariableCounter;
            // obligé de créer une nouvelle fonction à implementer
            IFunction jobFunc = new Function();
            jobFunc.StrictName = f.StrictName;
            jobFunc.InstanceNumber = f.InstanceNumber;
            // indiquer le paramètre de la structure comme étant void *
            IParameter param = f.Parameters[0].Clone() as IParameter;
            param.DataType = EnumDataType.E_VOID;
            param.IsMutableParameter = true;
            jobFunc.Parameters.Add(param);
            jobFunc.AddToSource("job_" + f.Name + "((" + this.StructureReference.StructureType + "*)" + param.EffectiveParameter + ");" + Environment.NewLine);
            if (!converter.ImplementedFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return !func.IsJob && func.StrictName == jobFunc.StrictName && func.InstanceNumber == jobFunc.InstanceNumber; })))
            {
                converter.ImplementedFunctions.Add(jobFunc);
            }
            if (!converter.CallingFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return !func.IsJob && func.StrictName == jobFunc.StrictName && func.InstanceNumber == jobFunc.InstanceNumber; })))
            {
                converter.CallingFunctions.Add(jobFunc);
            }

            string localName = "p" + converter.CurrentFunction.PrivateVariableCounter.ToString();
            converter.CurrentFunction.AddToSource("Parallel " + localName +
                "((void*)&" + this.StructureReference.PrefixedFieldName + ", gcnew Parallel::worker(this, &Compiled::" +
                "func_" + jobFunc.Name + "));" + Environment.NewLine);
            converter.CurrentFunction.AddToSource(localName + ".Start();" + Environment.NewLine);
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            // aller chercher les variables exterieures au processus appellé
            IFunction f = converter.ImplementedFunctions.FindLast(new Predicate<IFunction>(delegate(IFunction func) { return func.IsJob && func.StrictName == this.processName; }));
            if (!converter.CallingFunctions.Exists(new Predicate<IFunction>(delegate(IFunction func) { return func.IsJob && func.StrictName == this.processName && func.InstanceNumber == f.InstanceNumber; })))
            {
                converter.CallingFunctions.Add(f);
            }
            converter.CurrentFunction.AddToSource("New-SharedMemoryThread -ScriptBlock {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource("Param ($hash)" + Environment.NewLine + Environment.NewLine);
            converter.CurrentFunction.AddToSource("try {" + Environment.NewLine);
            converter.CurrentFunction.IndentSize += 1;
            converter.CurrentFunction.AddToSource("job_" + f.Name + " -" + this.StructureReference.FieldName + " ([ref] $hash)" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("} catch { Write-Host $_.Exception.Message }" + Environment.NewLine);
            converter.CurrentFunction.IndentSize -= 1;
            converter.CurrentFunction.AddToSource("} -InitializationScript $init -hash $" + this.StructureReference.FieldName + Environment.NewLine + Environment.NewLine);
        }

        #endregion
    }
}
