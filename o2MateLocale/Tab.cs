using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace o2Mate
{
    /// <summary>
    /// List of locale object
    /// </summary>
    [Guid("1D40B19F-6F16-4240-8221-3ADC28B7B82D")]
    [CoClass(typeof(ITab))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface ITab
    {
        /// <summary>
        /// Gets the count locale
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets a particular locale
        /// </summary>
        /// <param name="index">index number (zero-based index)</param>
        /// <returns>a locale instance</returns>
        ILocale Item(int index);
    }

    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    internal class Tab : ITab
    {
        #region Private Fields
        private List<ILocale> list;
        #endregion

        #region Public Constructor
        public Tab()
        {
            this.list = new List<ILocale>();
        }

        public Tab(Array arr)
        {
            this.list = new List<ILocale>();
            foreach (ILocale loc in arr)
            {
                this.list.Add(loc);
            }
        }
        #endregion

        #region Public Static Methods
        public static ITab FromArray(Array arr)
        {
            return new Tab(arr);
        }
        #endregion

        #region ITab Membres

        public int Count
        {
            get { return this.list.Count; }
        }

        public ILocale Item(int index)
        {
            return this.list[index];
        }

        #endregion
    }
}
