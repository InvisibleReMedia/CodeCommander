using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace o2Mate
{
    /// <summary>
    /// Just a string
    /// </summary>
    [CoClass(typeof(IChaine))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IChaine
    {
        /// <summary>
        /// Gets or sets the string value
        /// </summary>
        string Value { get; set; }
    }

    /// <summary>
    /// Keeps a string value
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE51")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Chaine : IChaine
    {
        #region Private Fields
        private string value;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Chaine()
        {
            this.value = "";
        }
        #endregion

        #region IChaine Membres

        /// <summary>
        /// Gets or sets the string value
        /// </summary>
        public string Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        #endregion
    }
}
