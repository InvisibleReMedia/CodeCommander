using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Converters;

namespace o2Mate
{
    internal class EndJobs : ILanguageConverter
    {
        #region Private Fields
        private List<CallJob> jobs;
        #endregion

        #region Default Constructor
        public EndJobs()
        {
            this.jobs = new List<CallJob>();
        }
        #endregion

        #region Public Properties
        public List<CallJob> Jobs
        {
            get { return this.jobs; }
        }

        #endregion
        #region Public Methods
        public void WaitForAllJobs(ICodeConverter converter)
        {
            converter.Convert(this);
        }
        #endregion

        #region ILanguageConverter Members

        public void WriteInVBScript(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource("Get-RunspaceData -Wait" + Environment.NewLine + Environment.NewLine);
        }

        public void WriteInPerl(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInPython(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInMicrosoftCPP(ICodeConverter converter)
        {
            converter.CurrentFunction.AddToSource("Parallel().WaitToEnd();" + Environment.NewLine + Environment.NewLine);
        }

        public void WriteInUnixCPP(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInCSharp(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInJava(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        public void WriteInMacOSCPP(ICodeConverter converter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
