using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    internal class BeginJob : ICompiler
    {
        #region Private Fields
        private string jobName;
        private ICompilateur cachedComp;
        #endregion

        #region Public Properties
        public string JobName
        {
            get
            {
                return this.jobName;
            }
            set
            {
                this.jobName = value;
            }
        }
        #endregion

        #region Compiler Membres

        public string TypeName
        {
            get { return "BeginJob"; }
        }

        public void ExtractDictionary(IProcess proc)
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
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
        }

        public void Inject(ICompiler obj, ICompilateur comp, System.Xml.XmlNode node, string modifier)
        {
        }

        public bool IsComputable(ICodeConverter converter, out string reason)
        {
            reason = String.Empty;
            return false;
        }

        public void Load(ICompilateur comp, System.Xml.XmlNode node)
        {
        }

        public void WriteToFile(IProcess proc, FinalFile file)
        {
        }

        public void Convert(ICodeConverter converter, IProcess proc, FinalFile file)
        {
            Process jobProc = new Process();
            Function f = new Function();
            f.IsJob = true;
            f.Name = this.jobName;
            // la première fonction commence à xxx_1
            f.InstanceNumber = converter.ImplementedFunctions.FindAll(new Predicate<IFunction>(delegate(IFunction func)
            {
                return func.IsJob && func.StrictName == this.jobName;
            })).Count + 1;
            jobProc.Name = ":" + f.Name;
            jobProc.FunctionName = f.Name;
            this.cachedComp.PushProcess(jobProc);
            converter.ImplementedFunctions.Add(f);
            converter.PushFunction(converter.CurrentFunction);
            converter.SetCurrentFunction(f);
        }

        #endregion

        #region Public Methods
        public void SetCompile(ICompilateur comp)
        {
            this.cachedComp = comp;
        }
        #endregion
    }
}
