using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;

namespace o2Mate
{
    /// <summary>
    /// Struct of primary key
    /// </summary>
    [ComVisible(true)]
    public struct primaryKey
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">field name</param>
        public primaryKey(string field)
        {
            this.field = field;
            this.valid = true;
        }
        /// <summary>
        /// Is a valid primary key
        /// </summary>
        public bool valid;
        /// <summary>
        /// Field name
        /// </summary>
        public string field;
    }

    /// <summary>
    /// Struct in connection (relational field)
    /// </summary>
    [ComVisible(true)]
    public struct inconnection
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="name">name of the array</param>
        /// <param name="field">field name in the array</param>
        public inconnection(string context, string name, string field)
        {
            this.context = context;
            this.name = name;
            this.field = field;
            this.valid = true;
        }
        /// <summary>
        /// Is a valid info
        /// </summary>
        public bool valid;
        /// <summary>
        /// Context
        /// </summary>
        public string context;
        /// <summary>
        /// Name of the array
        /// </summary>
        public string name;
        /// <summary>
        /// Field name
        /// </summary>
        public string field;
    }

    /// <summary>
    /// Struct depends on
    /// </summary>
    [ComVisible(true)]
    public struct depends
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="name">name of the string</param>
        /// <param name="value">the value</param>
        public depends(string context, string name, string value)
        {
            this.context = context;
            this.name = name;
            this.value = value;
            this.valid = true;
        }
        /// <summary>
        /// Is a valid info
        /// </summary>
        public bool valid;
        /// <summary>
        /// Context
        /// </summary>
        public string context;
        /// <summary>
        /// Name of the string
        /// </summary>
        public string name;
        /// <summary>
        /// Value
        /// </summary>
        public string value;
    }

    /// <summary>
    /// Summary element
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ILegende
    {
        /// <summary>
        /// Gets or sets the context
        /// it's a name which identifying a file source or a template
        /// </summary>
        string Context { get; set; }
        /// <summary>
        /// Gets or sets the name from the dictionary element
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets a description written for summary
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// Gets or sets a comment written for summary
        /// </summary>
        string Commentaire { get; set; }
        /// <summary>
        /// Gets or sets a specific type (enum, string, integer or a boolean)
        /// </summary>
        string Type { get; set; }
        /// <summary>
        /// Gets or sets an expression written for summary to work with summary
        /// </summary>
        string Expression { get; set; }
        /// <summary>
        /// Gets or sets if is free or not
        /// </summary>
        bool Free { get; set; }
        /// <summary>
        /// Gets or sets observe info
        /// </summary>
        string Observe { get; set; }
        /// <summary>
        /// Gets or sets relational info
        /// </summary>
        inconnection InConnection { get; set; }
        /// <summary>
        /// Gets or sets dependent reference
        /// </summary>
        depends DependsOn { get; set; }
        /// <summary>
        /// Gets or sets primary key
        /// </summary>
        primaryKey PrimaryKey { get; set; }

        /// <summary>
        /// Returns true if type is an array
        /// </summary>
        bool IsArray { get; }
        /// <summary>
        /// Returns true if type is a number
        /// </summary>
        bool IsNumber { get; }
        /// <summary>
        /// Returns true if type is a date
        /// </summary>
        bool IsDate { get; }
        /// <summary>
        /// Returns true if type is a string
        /// </summary>
        bool IsString { get; }
        /// <summary>
        /// Returns true if type is an enum set 
        /// </summary>
        bool IsEnumeration { get; }
        /// <summary>
        /// Returns true if type is a template
        /// </summary>
        bool IsTemplate { get; }
        /// <summary>
        /// Returns true if type is unknown
        /// </summary>
        bool IsUnknown { get; }

        /// <summary>
        /// Load data from xml node
        /// </summary>
        /// <param name="node">xml node object</param>
        void Load(XmlNode node);
        
        /// <summary>
        /// Save data into an xml writer
        /// </summary>
        /// <param name="writer">xml writer object</param>
        void Save(XmlWriter writer);

        /// <summary>
        /// Make a translation of the summary data in the current language
        /// </summary>
        /// <param name="value">value format to translate</param>
        /// <returns>the text translated</returns>
        string Translate(string value);
    }

    /// <summary>
    /// Summary element implementation
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE4E")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Legende : ILegende
    {
        #region Public Static Constants
        /// <summary>
        /// Array type
        /// </summary>
        public static readonly string TypeArray = "Array";
        /// <summary>
        /// Number type
        /// </summary>
        public static readonly string TypeNumber = "Number";
        /// <summary>
        /// Date type
        /// </summary>
        public static readonly string TypeDate = "Date";
        /// <summary>
        /// String type
        /// </summary>
        public static readonly string TypeString = "String";
        /// <summary>
        /// Enumeration type
        /// </summary>
        public static readonly string TypeEnumeration = "Enumeration";
        /// <summary>
        /// Unknown type
        /// </summary>
        public static readonly string TypeUnknown = "Unknown";
        /// <summary>
        /// Template type
        /// </summary>
        public static readonly string TypeTemplate = "Template";
        #endregion

        #region Private Fields
        private string context;
        private string name;
        private string description;
        private string commentaire;
        private string type;
        private string expression;
        private string observe;
        private inconnection relatedTo;
        private depends dependsOn;
        private primaryKey primaryKey;
        private bool free;
        #endregion

        #region Public Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Legende()
        {
            this.relatedTo = new inconnection();
            this.dependsOn = new depends();
            this.primaryKey = new primaryKey();
            this.free = true;
        }
        #endregion

        #region ILegende Membres

        /// <summary>
        /// Gets or sets the context
        /// it's a name which identifying a file source or a template
        /// </summary>
        public string Context
        {
            get
            {
                return this.context;
            }
            set
            {
                this.context = value;
            }
        }

        /// <summary>
        /// Gets or sets the name from the dictionary element
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets a comment written for summary
        /// </summary>
        public string Commentaire
        {
            get { return this.commentaire.Replace("\\", "\\\\").Replace(Environment.NewLine, "\\r\\n"); }
            set
            {
                o2Mate.LocaleGroup group = new LocaleGroup();
                string groupName, localeName;
                if (group.ExtractGroupAndName(value, out groupName, out localeName))
                {
                    if (!group.Exists(groupName))
                    {
                        group.Create(groupName);
                    }
                    o2Mate.ILocaleSet set = group.Get(groupName);
                    if (!set.ExistsOne(localeName, "fr-FR"))
                        set.Add(localeName, "fr-FR", "");
                    if (!set.ExistsOne(localeName, "en-US"))
                        set.Add(localeName, "en-US", "");
                }
                this.commentaire = value;
            }
        }

        /// <summary>
        /// Gets or sets a description written for summary
        /// </summary>
        public string Description
        {
            get
            {
                return this.description.Replace("\\", "\\\\").Replace(Environment.NewLine, "\\r\\n");
            }
            set
            {
                o2Mate.LocaleGroup group = new LocaleGroup();
                string groupName, localeName;
                if (group.ExtractGroupAndName(value, out groupName, out localeName))
                {
                    if (!group.Exists(groupName))
                    {
                        group.Create(groupName);
                    }
                    o2Mate.ILocaleSet set = group.Get(groupName);
                    if (!set.ExistsOne(localeName, "fr-FR"))
                        set.Add(localeName, "fr-FR", "");
                    if (!set.ExistsOne(localeName, "en-US"))
                        set.Add(localeName, "en-US", "");
                }
                this.description = value;
            }
        }

        /// <summary>
        /// Gets or sets a specific type (enum, string, integer, boolean, etc)
        /// </summary>
        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        /// <summary>
        /// Gets or sets an expression written for summary to work with summary
        /// </summary>
        public string Expression
        {
            get
            {
                return this.expression;
            }
            set
            {
                this.expression = value;
            }
        }

        /// <summary>
        /// Gets or sets if is free or not
        /// </summary>
        public bool Free
        {
            get
            {
                return this.free;
            }
            set
            {
                this.free = value;
            }
        }

        /// <summary>
        /// Gets or sets observe info
        /// </summary>
        public string Observe
        {
            get
            {
                return this.observe;
            }
            set
            {
                this.observe = value;
            }
        }

        /// <summary>
        /// Gets or sets relational info
        /// </summary>
        public inconnection InConnection
        {
            get
            {
                return this.relatedTo;
            }
            set
            {
                this.relatedTo = value;
            }
        }

        /// <summary>
        /// Gets or sets dependent reference
        /// </summary>
        public depends DependsOn
        {
            get
            {
                return this.dependsOn;
            }
            set
            {
                this.dependsOn = value;
            }
        }

        /// <summary>
        /// Gets or sets primary key
        /// </summary>
        public primaryKey PrimaryKey
        {
            get
            {
                return this.primaryKey;
            }
            set
            {
                this.primaryKey = value;
            }
        }

        /// <summary>
        /// Returns true if type is an array
        /// </summary>
        public bool IsArray
        {
            get
            {
                return String.Compare(this.type, Legende.TypeArray, true) == 0;
            }
        }

        /// <summary>
        /// Returns true if type is a number
        /// </summary>
        public bool IsNumber
        {
            get { return String.Compare(this.type, Legende.TypeNumber, true) == 0; }
        }

        /// <summary>
        /// Returns true if type is a date
        /// </summary>
        public bool IsDate
        {
            get { return String.Compare(this.type, Legende.TypeDate, true) == 0; }
        }

        /// <summary>
        /// Returns true if type is a string
        /// </summary>
        public bool IsString
        {
            get { return String.Compare(this.type, Legende.TypeString, true) == 0; }
        }

        /// <summary>
        /// Returns true if type is an enum set
        /// </summary>
        public bool IsEnumeration
        {
            get { return String.Compare(this.type, Legende.TypeEnumeration, true) == 0; }
        }

        /// <summary>
        /// Returns true if type is a template
        /// </summary>
        public bool IsTemplate
        {
            get { return String.Compare(this.type, Legende.TypeTemplate, true) == 0; }
        }

        /// <summary>
        /// Returns true if type is unknown
        /// </summary>
        public bool IsUnknown
        {
            get { return (String.Compare(this.type, Legende.TypeArray, true) != 0 &&
                          String.Compare(this.type, Legende.TypeNumber, true) != 0 &&
                          String.Compare(this.type, Legende.TypeString, true) != 0 &&
                          String.Compare(this.type, Legende.TypeDate, true) != 0 &&
                          String.Compare(this.type, Legende.TypeEnumeration, true) != 0 &&
                          String.Compare(this.type, Legende.TypeTemplate, true) != 0);
            }
        }

        /// <summary>
        /// Load data from xml node
        /// </summary>
        /// <param name="node">xml node object</param>
        public void Load(XmlNode node)
        {
            this.context = node.Attributes.GetNamedItem("context").Value;
            this.name = node.Attributes.GetNamedItem("name").Value;
            this.description = node.InnerText;
            this.type = node.Attributes.GetNamedItem("type").Value;
            this.expression = node.Attributes.GetNamedItem("expression").Value;
            this.commentaire = node.Attributes.GetNamedItem("commentaire").Value;
            if (node.Attributes.GetNamedItem("free") != null)
            {
                this.free = Boolean.Parse(node.Attributes.GetNamedItem("free").Value);
            }
            else
            {
                this.free = true;
            }
        }

        /// <summary>
        /// Save data into an xml writer
        /// </summary>
        /// <param name="writer">xml writer object</param>
        public void Save(XmlWriter writer)
        {
            writer.WriteStartElement("item");
            writer.WriteAttributeString("context", this.context);
            writer.WriteAttributeString("name", this.name);
            writer.WriteAttributeString("type", this.type);
            writer.WriteAttributeString("expression", this.expression);
            writer.WriteAttributeString("commentaire", this.commentaire);
            if (!this.free)
            {
                writer.WriteAttributeString("free", this.free.ToString());
            }
            writer.WriteString(this.description);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Update summary from html element
        /// </summary>
        /// <param name="elem">html element object</param>
        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            this.context = elem.GetAttribute("context");
            this.name = elem.GetAttribute("name");
            this.description = elem.GetAttribute("description");
            this.type = elem.GetAttribute("typeLegende");
            this.expression = elem.GetAttribute("expression");
            this.commentaire = elem.GetAttribute("commentaire");
            this.free = Boolean.Parse(elem.GetAttribute("free"));
            o2Mate.LocaleGroup group = new LocaleGroup();
            string groupName, localeName;
            if (group.ExtractGroupAndName(this.description, out groupName, out localeName))
            {
                if (!group.Exists(groupName))
                {
                    group.Create(groupName);
                }
                o2Mate.ILocaleSet set = group.Get(groupName);
                if (!set.ExistsOne(localeName, "fr-FR"))
                    set.Add(localeName, "fr-FR", "");
                if (!set.ExistsOne(localeName, "en-US"))
                    set.Add(localeName, "en-US", "");
            }
            if (group.ExtractGroupAndName(this.commentaire, out groupName, out localeName))
            {
                if (!group.Exists(groupName))
                {
                    group.Create(groupName);
                }
                o2Mate.ILocaleSet set = group.Get(groupName);
                if (!set.ExistsOne(localeName, "fr-FR"))
                    set.Add(localeName, "fr-FR", "");
                if (!set.ExistsOne(localeName, "en-US"))
                    set.Add(localeName, "en-US", "");
            }
        }

        /// <summary>
        /// Make a translation of the summary data in the current language
        /// </summary>
        /// <param name="value">value format to translate</param>
        /// <returns>the text translated</returns>
        public string Translate(string value)
        {
            o2Mate.LocaleGroup group = new LocaleGroup();
            string groupName, localeName;
            if (group.ExtractGroupAndName(value, out groupName, out localeName))
            {
                if (group.Exists(groupName))
                {
                    o2Mate.ILocaleSet set = group.Get(groupName);
                    if (set.ExistsOne(localeName, System.Threading.Thread.CurrentThread.CurrentUICulture.Name))
                    {
                        return set.Get(localeName, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
                    }
                    else
                    {
                        return value;
                    }
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return value;
            }
        }

        #endregion
    }
}
