using System;
using System.Collections.Generic;
using System.Text;

namespace o2Mate
{
    /// <summary>
    /// Skeleton class
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class Skeleton
    {
        #region Private Fields
        private Node<ICompiler> node;
        private IProcess process;
        private ILegendeDict legendes;
        private string path;
        private string name;
        #endregion

        #region Default Constructor
        internal Skeleton(string path, string name)
        {
            this.process = null;
            this.legendes = null;
            this.node = null;
            this.path = path;
            this.name = name;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the skeleton path
        /// </summary>
        public string Path
        {
            get
            {
                return this.path;
            }
        }

        /// <summary>
        /// Gets the skeleton name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }
        #endregion

        #region Internal Properties
        internal IProcess Process
        {
            get { return this.process; }
            set { this.process = value; }
        }

        internal Node<ICompiler> Objects
        {
            get { return this.node; }
            set { this.node = value; }
        }

        internal ILegendeDict Legendes
        {
            get { return this.legendes; }
            set { this.legendes = value; }
        }
        #endregion
    }
}
