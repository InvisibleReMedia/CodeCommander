using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;

namespace o2Mate
{
    /// <summary>
    /// Dictionary interface
    /// </summary>
    [CoClass(typeof(IDictionnaire))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IDictionnaire
    {
        /// <summary>
        /// Says if the dictionary is empty
        /// </summary>
        /// <returns>true if empty, else false</returns>
        bool IsEmpty();
        /// <summary>
        /// Says if that name exists and is a string
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>true or false</returns>
        bool IsString(string name);
        /// <summary>
        /// Says if that name exists and is an array
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>true or false</returns>
        bool IsArray(string name);
        /// <summary>
        /// Says just if that name exists
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>true or false</returns>
        bool Exists(string name);
        /// <summary>
        /// Load an XML string
        /// </summary>
        /// <param name="xml">XML string</param>
        void LoadXml(string xml);
        /// <summary>
        /// Load an XML file
        /// </summary>
        /// <param name="fileName">file name</param>
        void Load(string fileName);
        /// <summary>
        /// Save data to an XML file
        /// </summary>
        /// <param name="fileName">file name</param>
        void Save(string fileName);
        /// <summary>
        /// Add a new string
        /// replace if string already exists
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        void AddString(string name, string value);
        /// <summary>
        /// Add a new array
        /// replace if array already exists
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        void AddArray(string name, Object value);
        /// <summary>
        /// Gets the list of string keys
        /// </summary>
        IEnumerable StringKeys { get;}
        /// <summary>
        /// Gets the list of array keys
        /// </summary>
        IEnumerable ArrayKeys { get; }
        /// <summary>
        /// Gets a string from a name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>the string or null if not found</returns>
        string GetString(string name);
        /// <summary>
        /// Sets the value of an existing string
        /// Do nothing if not exists or is not a string
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        void SetString(string name, string value);
        /// <summary>
        /// Gets an array
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>the array or null if not found</returns>
        Object GetArray(string name);
        /// <summary>
        /// Summary
        /// </summary>
        ILegendeDict Legendes { get; }
        /// <summary>
        /// Parse expression to insert into a summary
        /// </summary>
        /// <param name="legende">summary object</param>
        void ParseExpression(ILegende legende);
        /// <summary>
        /// Test depends on
        /// </summary>
        /// <param name="legende">summary object</param>
        /// <returns>true or false</returns>
        bool TestDependsOn(ILegende legende);
        /// <summary>
        /// Gets a relation with an another name
        /// </summary>
        /// <param name="legende">summary object</param>
        /// <param name="keyTab">tabular name</param>
        /// <param name="index">position in the tabular</param>
        /// <param name="arr">array name</param>
        /// <param name="field">field name</param>
        /// <returns>true if found</returns>
        bool GetInConnection(ILegende legende, string keyTab, int index, out string arr, out string field);
    }


    /// <summary>
    /// Dictionary implementation
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE50")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Dictionnaire : IDictionnaire
    {
        #region Private Fields
        private List<string> names;
        private Dictionary<string, Object> table;
        private ILegendeDict legendes;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Dictionnaire()
        {
            this.names = null;
            this.table = new Dictionary<string, Object>();
            this.legendes = new LegendeDict();
        }
        #endregion

        #region Private Methods
        private bool ExtractName(string name, out int index, out string context, out string extracted)
        {
            // on stocke dans une liste tous les noms avec les champs
            if (this.names == null)
            {
                this.names = new List<string>();
                foreach (string key in this.StringKeys)
                {
                    // c'est une chaîne: pour la reconnaitre, on met un ":" devant
                    this.names.Add(":" + key);
                }
                foreach (string key in this.ArrayKeys)
                {
                    // c'est un tableau : pour le reconnaitre, on met en ";" devant
                    this.names.Add(";" + key);
                    Array arr = this.GetArray(key) as Array;
                    if (arr.Count > 0)
                    {
                        Fields fields = arr.Item(1) as Fields;
                        foreach (string field in fields.Keys)
                        {
                            if (!this.names.Exists(new Predicate<string>(delegate (string s) { return s == key + "." + field; })))
                                // c'est un champ, on met "-" devant pour le reconnaitre
                                this.names.Add("-" + key + "." + field);
                        }
                    }
                }
            }
            int maxlength = 0;
            bool found = false;
            index = 0;
            context = "";
            extracted = "";
            foreach (string key in this.names)
            {
                if (name.StartsWith(key.Substring(1)) && key.Length > maxlength)
                {
                    found = true;
                    maxlength = key.Length;
                    extracted = key;
                    index = key.Length - 1;
                }
            }
            if (found)
            {
                if (extracted.StartsWith(":"))
                {
                    // c'est une chaîne
                    context = "";
                    extracted = extracted.Substring(1);
                }
                else if (extracted.StartsWith("-"))
                {
                    // c'est un champ
                    int point = extracted.IndexOf(".");
                    context = extracted.Substring(1, point - 1);
                    extracted = extracted.Substring(point + 1);
                }
                else if (extracted.StartsWith(";"))
                {
                    // c'est un tableau
                    context = extracted.Substring(1);
                    extracted = "";
                }
            }
            return found;
        }
        #endregion

        #region IDictionnaire Membres

        /// <summary>
        /// Summary
        /// </summary>
        public ILegendeDict Legendes
        {
            get { return this.legendes; }
        }

        /// <summary>
        /// Says if the dictionary is empty
        /// </summary>
        /// <returns>true if empty, else false</returns>
        public bool IsEmpty()
        {
            return (this.table.Count == 0);
        }

        /// <summary>
        /// Says if that name exists and is a string
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>true or false</returns>
        public bool IsString(string name)
        {
            if (this.Exists(name))
            {
                Object elem = this.table[name];
                if (elem is o2Mate.Chaine)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Says if that name exists and is an array
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>true or false</returns>
        public bool IsArray(string name)
        {
            if (this.Exists(name))
            {
                Object elem = this.table[name];
                if (elem is o2Mate.Array)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Says just if that name exists
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>true or false</returns>
        public bool Exists(string name)
        {
            return this.table.ContainsKey(name);
        }

        /// <summary>
        /// Load an XML string
        /// </summary>
        /// <param name="xml">XML string</param>
        public void LoadXml(string xml)
        {
            this.table.Clear();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml(xml);
            XmlNodeList nodes = doc.SelectNodes("/Dictionnaire/value");
            foreach (XmlNode node in nodes)
            {
                string name;
                string typeName;
                name = node.Attributes.GetNamedItem("name").Value;
                typeName = node.Attributes.GetNamedItem("type").Value;
                if (String.Compare(typeName, "string", true) == 0)
                {
                    Chaine c = new Chaine();
                    c.Value = node.Value;
                    this.table.Add(name, c);
                }
                else if (String.Compare(typeName, "array") == 0)
                {
                    Array a = new Array();
                    a.Load(node);
                    this.table.Add(name, a);
                }

            }
        }

        /// <summary>
        /// Load an XML file
        /// </summary>
        /// <param name="fileName">file name</param>
        public void Load(string fileName)
        {
            this.table.Clear();
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load(fileName);
            XmlNodeList nodes = doc.SelectNodes("/Dictionnaire/value");
            foreach (XmlNode node in nodes)
            {
                string name;
                string typeName;
                name = node.Attributes.GetNamedItem("name").Value;
                typeName = node.Attributes.GetNamedItem("type").Value;
                if (String.Compare(typeName, "string", true) == 0)
                {
                    Chaine c = new Chaine();
                    c.Value = node.InnerText;
                    this.table.Add(name, c);
                }
                else if (String.Compare(typeName, "array") == 0)
                {
                    Array a = new Array();
                    a.Load(node);
                    this.table.Add(name, a);
                }

            }
        }

        /// <summary>
        /// Save data to an XML file
        /// </summary>
        /// <param name="fileName">file name</param>
        public void Save(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            XmlNode node = doc.CreateElement("Dictionnaire");
            XmlAttribute att = doc.CreateAttribute("version");
            att.Value = o2Mate.Version.GetSoftwareVersion().ToString();
            node.Attributes.Append(att);
            foreach (KeyValuePair<string, Object> elem in this.table)
            {
                XmlNode elemNode = doc.CreateElement("value");
                XmlAttribute elemName, elemType;
                if (elem.Value is o2Mate.Chaine)
                {
                    o2Mate.Chaine c = elem.Value as o2Mate.Chaine;
                    elemName = doc.CreateAttribute("name");
                    elemName.Value = elem.Key;
                    elemNode.Attributes.Append(elemName);
                    elemType = doc.CreateAttribute("type");
                    elemType.Value = "string";
                    elemNode.Attributes.Append(elemType);
                    elemNode.InnerText = c.Value;
                }
                else if (elem.Value is o2Mate.Array)
                {
                    o2Mate.Array a = elem.Value as o2Mate.Array;
                    elemName = doc.CreateAttribute("name");
                    elemName.Value = elem.Key;
                    elemNode.Attributes.Append(elemName);
                    elemType = doc.CreateAttribute("type");
                    elemType.Value = "array";
                    elemNode.Attributes.Append(elemType);
                    XmlNodeList items = a.Save(doc);
                    // chaque noeud est supprimé auotmatiquement de la liste
                    while(items.Count > 0)
                    {
                        elemNode.AppendChild(items[0]);
                    }
                }
                node.AppendChild(elemNode);
            }
            doc.AppendChild(node);
            doc.PreserveWhitespace = true;
            doc.Save(fileName);
        }

        /// <summary>
        /// Add a new string
        /// replace if string already exists
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        public void AddString(string name, string value)
        {
            Chaine c = new Chaine();
            c.Value = value;
            if (this.Exists(name))
            {
                this.table[name] = c;
            }
            else
            {
                this.names = null;
                this.table.Add(name, c);
            }
        }

        /// <summary>
        /// Add a new array
        /// replace if array already exists
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        public void AddArray(string name, object value)
        {
            if (this.Exists(name))
            {
                this.table[name] = value;
            }
            else
            {
                this.names = null;
                this.table.Add(name, value);
            }
        }

        /// <summary>
        /// Gets the list of string keys
        /// </summary>
        public IEnumerable StringKeys
        {
            get
            {
                List<string> list = new List<string>();
                foreach (string key in this.table.Keys)
                {
                    if (this.table[key] is o2Mate.Chaine)
                    {
                        list.Add(key);
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Gets the list of array keys
        /// </summary>
        public IEnumerable ArrayKeys
        {
            get
            {
                List<string> list = new List<string>();
                foreach (string key in this.table.Keys)
                {
                    if (this.table[key] is o2Mate.Array)
                    {
                        list.Add(key);
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Gets a string from a name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>the string or null if not found</returns>
        public string GetString(string name)
        {
            if (this.IsString(name))
            {
                Chaine c = this.table[name] as Chaine;
                return c.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the value of an existing string
        /// Do nothing if not exists or is not a string
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        public void SetString(string name, string value)
        {
            if (this.IsString(name))
            {
                Chaine c = this.table[name] as Chaine;
                c.Value = value;
            }
        }

        /// <summary>
        /// Gets an array
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>the array or null if not found</returns>
        public object GetArray(string name)
        {
            if (this.IsArray(name))
            {
                return this.table[name];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Parse expression to insert into a summary
        /// </summary>
        /// <param name="legende">summary object</param>
        public void ParseExpression(ILegende legende)
        {
            if (legende != null)
            {
                legende.DependsOn = new depends();
                legende.InConnection = new inconnection();
                legende.PrimaryKey = new primaryKey();
                legende.Observe = "";
                Regex reg = new Regex(@"^observe \(((([^,]+),?)+)\)$|^primary key is (.*)$|^depends on (.*)$|^in connection with (.*)$");
                Match match = reg.Match(legende.Expression);
                if (match.Success)
                {
                    if (match.Groups[1].Success)
                    {
                        if (!legende.IsArray)
                        {
                            // groupe observe
                            legende.Observe = match.Groups[1].Value;
                        }
                    }
                    else if (match.Groups[4].Success)
                    {
                        // groupe primary key
                        if (legende.IsArray)
                        {
                            legende.PrimaryKey = new primaryKey(match.Groups[4].Value);
                        }
                    }
                    else if (match.Groups[5].Success)
                    {
                        // groupe depends on
                        if (legende.IsArray)
                        {
                            string sub = match.Groups[5].Value;
                            int next;
                            string context;
                            string depends;
                            if (this.ExtractName(sub, out next, out context, out depends))
                            {
                                Regex nextReg = new Regex(@" to (.*)$");
                                Match nextMatch = nextReg.Match(sub.Substring(next));
                                if (nextMatch.Success)
                                {
                                    legende.DependsOn = new depends(context, depends, nextMatch.Groups[1].Value);
                                }
                            }
                        }
                    }
                    else if (match.Groups[6].Success)
                    {
                        if (!legende.IsArray)
                        {
                            // groupe related to
                            int next;
                            string context;
                            string related;
                            string sub = match.Groups[6].Value;
                            if (this.ExtractName(sub, out next, out context, out related))
                            {
                                Regex nextReg = new Regex(@" to (.*)$");
                                Match nextMatch = nextReg.Match(sub.Substring(next));
                                if (nextMatch.Success)
                                {
                                    legende.Type = Legende.TypeNumber;
                                    legende.InConnection = new inconnection(context, related, nextMatch.Groups[1].Value);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Test depends on
        /// </summary>
        /// <param name="legende">summary object</param>
        /// <returns>true or false</returns>
        public bool TestDependsOn(ILegende legende)
        {
            if (legende != null)
            {
                if (legende.DependsOn.valid)
                {
                    // cette legende représente un tableau qui dépend d'un champ ou d'une chaîne
                    if (String.IsNullOrEmpty(legende.DependsOn.context))
                    {
                        // c'est une chaine, vérifier qu'elle existe et qu'elle a la bonne valeur
                        if (this.IsString(legende.DependsOn.name) && legende.DependsOn.value == this.GetString(legende.DependsOn.name))
                        {
                            // c'est une chaîne et elle a la valeur pour ce tableau
                            return true;
                        }
                    }
                    else
                    {
                        // il y a un context
                        if (this.IsArray(legende.DependsOn.context))
                        {
                            Array arr = this.GetArray(legende.DependsOn.context) as Array;
                            Fields fields = arr.Item(1) as Fields;
                            if (fields.Exists(legende.DependsOn.name) && fields.GetString(legende.DependsOn.name) == legende.DependsOn.value)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a relation with an another name
        /// </summary>
        /// <param name="legende">summary object</param>
        /// <param name="keyTab">tabular name</param>
        /// <param name="index">position in the tabular</param>
        /// <param name="arr">array name</param>
        /// <param name="field">field name</param>
        /// <returns>true if found</returns>
        public bool GetInConnection(ILegende legende, string keyTab, int index, out string arr, out string field)
        {
            if (legende != null)
            {
                if (legende.InConnection.valid)
                {
                    if (String.IsNullOrEmpty(legende.InConnection.context))
                    {
                        // cette legende indique que cette chaine pointe sur un tableau
                        string value = this.GetString(legende.InConnection.name);
                        if (!String.IsNullOrEmpty(value))
                        {
                            if (this.IsArray(value))
                            {
                                arr = value;
                                field = legende.InConnection.field;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(legende.InConnection.name))
                        {
                            // cette legende indique un tableau
                            if (this.IsArray(legende.InConnection.context))
                            {
                                arr = legende.InConnection.context;
                                field = legende.InConnection.field;
                                return true;
                            }
                        }
                        else
                        {
                            // cette legende indique un champ du tableau pointant sur un tableau
                            if (legende.InConnection.context == keyTab)
                            {
                                Array relatedArray = this.GetArray(keyTab) as Array;
                                if (index > 0)
                                {
                                    Fields relatedFields = relatedArray.Item(index) as Fields;
                                    if (relatedFields.Exists(legende.InConnection.name))
                                    {
                                        string value = relatedFields.GetString(legende.InConnection.name);
                                        if (!String.IsNullOrEmpty(value))
                                        {
                                            if (this.IsArray(value))
                                            {
                                                arr = value;
                                                field = legende.InConnection.field;
                                                return true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }
            }
            arr = null;
            field = null;
            return false;
        }
        #endregion
    }
}
