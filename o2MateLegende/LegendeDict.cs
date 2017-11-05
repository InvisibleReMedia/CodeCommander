using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;

namespace o2Mate
{
    /// <summary>
    /// Summary dictionary
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILegendeDict
    {
        /// <summary>
        /// Load data from an xml node
        /// </summary>
        /// <param name="dict"></param>
        void Load(XmlNode dict);
        /// <summary>
        /// Save data to an xml writer
        /// </summary>
        /// <param name="writer">xml writer object</param>
        void Save(XmlWriter writer);
        /// <summary>
        /// Update from the web browser
        /// </summary>
        /// <param name="elem">html element</param>
        void Update(System.Windows.Forms.HtmlElement elem);
        /// <summary>
        /// Clear data
        /// </summary>
        void Clear();
        /// <summary>
        /// Add a new element
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="name">name to create</param>
        /// <param name="desc">description</param>
        /// <param name="type">type of data</param>
        /// <param name="expr">expression</param>
        void Add(string context, string name, string desc, string type, string expr);
        /// <summary>
        /// Copy from an another summary
        /// </summary>
        /// <param name="dict">the summary to copy</param>
        void CopyFrom(ILegendeDict dict);
        /// <summary>
        /// Gets the number of element
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Gets an element by index
        /// </summary>
        /// <param name="index">position number</param>
        /// <returns>a summary element</returns>
        ILegende GetLegendeByIndex(int index);
        /// <summary>
        /// Gets an element by name
        /// </summary>
        /// <param name="name">name of the element to search</param>
        /// <returns>a summary element</returns>
        ILegende GetLegendeByName(string name);
        /// <summary>
        /// Gets an element by name in a particular context
        /// </summary>
        /// <param name="name">name of the element to search</param>
        /// <param name="context">a particular context</param>
        /// <returns>a summary element</returns>
        ILegende GetLegendeByName(string name, string context);
    }

    /// <summary>
    /// Summary implementation
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE4D")]
    [ClassInterface(ClassInterfaceType.None)]
    public class LegendeDict : ILegendeDict, IEnumerable<ILegende>
    {
        #region Private Fields
        private List<ILegende> legendes;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public LegendeDict()
        {
            this.legendes = new List<ILegende>();
        }
        #endregion

        #region ILegendeDict Membres

        /// <summary>
        /// Clear data
        /// </summary>
        public void Clear()
        {
            this.legendes.Clear();
        }

        /// <summary>
        /// Load data from an xml node
        /// </summary>
        /// <param name="dict"></param>
        public void Load(XmlNode dict)
        {
            foreach (XmlNode child in dict)
            {
                Legende leg = new Legende();
                leg.Load(child);
                this.legendes.Add(leg);
            }
        }

        /// <summary>
        /// Save data to an xml writer
        /// </summary>
        /// <param name="writer">xml writer object</param>
        public void Save(XmlWriter writer)
        {
            foreach (ILegende leg in this.legendes)
            {
                leg.Save(writer);
            }
        }

        /// <summary>
        /// Update from the web browser
        /// </summary>
        /// <param name="elem">html element</param>
        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            // un update des legendes => on efface les anciennes legendes
            this.Clear();
            foreach (System.Windows.Forms.HtmlElement child in elem.Children)
            {
                Legende leg = new Legende();
                leg.Update(child);
                this.legendes.Add(leg);
            }
        }

        /// <summary>
        /// Gets the number of element
        /// </summary>
        public int Count
        {
            get { return this.legendes.Count; }
        }

        /// <summary>
        /// Gets an element by index
        /// </summary>
        /// <param name="index">position number</param>
        /// <returns>a summary element</returns>
        public ILegende GetLegendeByIndex(int index)
        {
            return this.legendes[index];
        }

        /// <summary>
        /// Gets an element by name
        /// </summary>
        /// <param name="name">name of the element to search</param>
        /// <returns>a summary element</returns>
        public ILegende GetLegendeByName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                ILegende foundLegende = this.legendes.Find(new Predicate<ILegende>(delegate(ILegende a) { return a.Name == name; }));
                if (foundLegende != null)
                {
                    return foundLegende;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets an element by name in a particular context
        /// </summary>
        /// <param name="name">name of the element to search</param>
        /// <param name="context">a particular context</param>
        /// <returns>a summary element</returns>
        public ILegende GetLegendeByName(string name, string context)
        {
            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(context))
            {
                ILegende foundLegende = this.legendes.Find(new Predicate<ILegende>(delegate(ILegende a) { return a.Context == context && a.Name == name; }));
                if (foundLegende != null)
                {
                    return foundLegende;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Add a new element
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="name">name to create</param>
        /// <param name="desc">description</param>
        /// <param name="type">type of data</param>
        /// <param name="expr">expression</param>
        public void Add(string context, string name, string desc, string type, string expr)
        {
            if (!String.IsNullOrEmpty(name) && context != null)
            {
                ILegende foundLegende = this.legendes.Find(new Predicate<ILegende>(delegate(ILegende a) { return a.Context == context && a.Name == name; }));
                if (foundLegende != null)
                {
                    foundLegende.Description = desc;
                    foundLegende.Type = type;
                    foundLegende.Expression = expr;
                }
                else
                {
                    ILegende legende = new Legende();
                    legende.Context = context;
                    legende.Name = name;
                    legende.Description = desc;
                    legende.Type = type;
                    legende.Expression = expr;
                    this.legendes.Add(legende);
                }
            }
        }

        /// <summary>
        /// Copy from an another summary
        /// </summary>
        /// <param name="dict">the summary to copy</param>
        public void CopyFrom(ILegendeDict dict)
        {
            this.legendes.AddRange(dict as LegendeDict);
        }

        #endregion

        #region IEnumerable<ILegende> Membres

        /// <summary>
        /// Get enumerator to iterate through summary elements
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ILegende> GetEnumerator()
        {
            return this.legendes.GetEnumerator();
        }

        #endregion

        #region IEnumerable Membres

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)this.legendes.GetEnumerator();
        }

        #endregion
    }
}
