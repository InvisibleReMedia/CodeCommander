using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;

namespace o2Mate
{
    /// <summary>
    /// An array object
    /// </summary>
    [CoClass(typeof(IArray))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IArray
    {
        /// <summary>
        /// Gets the nth-item
        /// </summary>
        /// <param name="index">number (starts with 1)</param>
        /// <returns>an array</returns>
        Object Item(int index);
        /// <summary>
        /// Gives the count of elements in the array
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Add a new element
        /// </summary>
        /// <param name="fields">fields list</param>
        void Add(Object fields);
        /// <summary>
        /// Load from an xml node
        /// </summary>
        /// <param name="node">xml node</param>
        void Load(XmlNode node);
        /// <summary>
        /// Saves the array into an xml document
        /// </summary>
        /// <param name="doc">xml document</param>
        /// <returns>array's items at xml format</returns>
        XmlNodeList Save(XmlDocument doc);
    }

    /// <summary>
    /// Array class
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE52")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Array : IArray
    {
        #region Private Fields
        List<Fields> list;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Array()
        {
            this.list = new List<Fields>();
        }
        #endregion

        #region IArray Membres

        /// <summary>
        /// Get the item object
        /// </summary>
        /// <param name="index">position (from 1 to N)</param>
        /// <returns>fields object</returns>
        public object Item(int index)
        {
            return this.list[index - 1];
        }

        /// <summary>
        /// Gets the count list
        /// </summary>
        public int Count
        {
            get { return this.list.Count; }
        }

        /// <summary>
        /// Add a new item into the array
        /// </summary>
        /// <param name="fields">fields object</param>
        public void Add(object fields)
        {
            this.list.Add(fields as Fields);
        }

        /// <summary>
        /// Load array from an XML node
        /// </summary>
        /// <param name="node">XML node</param>
        public void Load(XmlNode node)
        {
            XmlNodeList nodes = node.SelectNodes("item");
            if (nodes != null)
            {
                foreach (XmlNode item in nodes)
                {
                    Fields f = new Fields();
                    f.Load(item);
                    this.list.Add(f);
                }
            }
        }

        /// <summary>
        /// Save array data in a XML node
        /// </summary>
        /// <param name="doc">XML document</param>
        /// <returns>xml node list</returns>
        public XmlNodeList Save(XmlDocument doc)
        {
            XmlNode root = doc.CreateElement("root");
            foreach (Fields item in this.list)
            {
                XmlNode node = item.Save(doc);
                root.AppendChild(node);
            }
            return root.ChildNodes;
        }

        #endregion
    }
}
