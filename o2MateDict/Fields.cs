using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;
using System.Collections;

namespace o2Mate
{
    /// <summary>
    /// Fields interface (into each item array)
    /// </summary>
    [CoClass(typeof(IFields))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IFields
    {
        /// <summary>
        /// Says if field name exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true or false</returns>
        bool Exists(string name);
        /// <summary>
        /// Load from an xml node
        /// </summary>
        /// <param name="node"></param>
        void Load(XmlNode node);
        /// <summary>
        /// Save to an xml document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>xml node</returns>
        XmlNode Save(XmlDocument doc);
        /// <summary>
        /// Add a field name/value
        /// replace if already exists
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        void AddString(string name, string value);
        /// <summary>
        /// Get a field value
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>field value or empty string</returns>
        string GetString(string name);
        /// <summary>
        /// Gets the list of keys
        /// </summary>
        IEnumerable Keys { get; }
    }

    /// <summary>
    /// Fields class (into each item array)
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE47")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Fields : IFields
    {
        #region Private Fields
        private Dictionary<string, string> table;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Fields()
        {
            this.table = new Dictionary<string, string>();
        }
        #endregion

        #region IFields Membres

        /// <summary>
        /// Says if field name exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true or false</returns>
        public bool Exists(string name)
        {
            return this.table.ContainsKey(name);
        }

        /// <summary>
        /// Load from an xml node
        /// </summary>
        /// <param name="node"></param>
        public void Load(XmlNode node)
        {
            this.table.Clear();
            XmlNodeList nodes = node.SelectNodes("field");
            if (nodes != null)
            {
                foreach (XmlNode field in nodes)
                {
                    string name = field.Attributes.GetNamedItem("name").Value;
                    string value = field.InnerText;
                    this.table.Add(name, value);
                }
            }
        }

        /// <summary>
        /// Save to an xml document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>xml node</returns>
        public XmlNode Save(XmlDocument doc)
        {
            XmlNode root = doc.CreateElement("item");
            foreach (string key in this.table.Keys)
            {
                string value = this.table[key];
                XmlNode field = doc.CreateElement("field");
                XmlAttribute fieldName = doc.CreateAttribute("name");
                fieldName.Value = key;
                field.Attributes.SetNamedItem(fieldName);
                field.InnerText = value;
                root.AppendChild(field);
            }
            return root;
        }

        /// <summary>
        /// Add a field name/value
        /// replace if already exists
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        public void AddString(string name, string value)
        {
            if (this.Exists(name))
            {
                this.table[name] = value;
            }
            else
            {
                this.table.Add(name, value);
            }
        }

        /// <summary>
        /// Get a field value
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>field value or empty string</returns>
        public string GetString(string name)
        {
            if (this.Exists(name))
            {
                return this.table[name];
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the list of keys
        /// </summary>
        public IEnumerable Keys
        {
            get
            {
                string[] arr = new string[this.table.Count];
                this.table.Keys.CopyTo(arr, 0);
                return arr;
            }
        }

        #endregion
    }
}
