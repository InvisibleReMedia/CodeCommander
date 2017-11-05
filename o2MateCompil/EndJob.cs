using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Converters;

namespace o2Mate
{
    internal class EndJob : ICompiler
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
            get { return "EndJob"; }
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
            this.cachedComp.PopProcess();
            converter.SetCurrentFunction(converter.PopFunction());
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
