using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace o2Mate
{
    /// <summary>
    /// Interface declaration for a set of locale values
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [CoClass(typeof(ILocaleSet))]
    [ComVisible(true)]
    public interface ILocaleSet
    {
        /// <summary>
        /// Gets or sets the group name (the group name is a file name)
        /// </summary>
        string GroupName { get; set; }
        /// <summary>
        /// Says if a locale exists by name and language
        /// </summary>
        /// <param name="name">name to search</param>
        /// <param name="language">language switch</param>
        /// <returns>true if exists</returns>
        bool ExistsOne(string name, string language);
        /// <summary>
        /// Says if a locale exists by name (for any language)
        /// </summary>
        /// <param name="name">name to search</param>
        /// <returns>true if exists</returns>
        bool Exists(string name);
        /// <summary>
        /// Adds a newly locale with name, language and value
        /// </summary>
        /// <param name="name">locale key</param>
        /// <param name="language">language switch</param>
        /// <param name="value">value</param>
        void Add(string name, string language, string value);
        /// <summary>
        /// Modify an existing locale with name, language and value
        /// </summary>
        /// <param name="name">locale key</param>
        /// <param name="language">language switch</param>
        /// <param name="value">value</param>
        void Modify(string name, string language, string value);
        /// <summary>
        /// Supress an existing locale by name and language
        /// </summary>
        /// <param name="name">locale key</param>
        /// <param name="language">language switch</param>
        void DeleteOne(string name, string language);
        /// <summary>
        /// Deletes all existing locale by name (for any language)
        /// </summary>
        /// <param name="name">locale key</param>
        void Delete(string name);
        /// <summary>
        /// Rename an existing locale with name, language and value
        /// </summary>
        /// <param name="oldName">locale key to search</param>
        /// <param name="name">new locale key name</param>
        /// <param name="language">language switch</param>
        void RenameOne(string oldName, string name, string language);
        /// <summary>
        /// Rename all existing locale by name (for any language)
        /// </summary>
        /// <param name="oldName">locale key to search</param>
        /// <param name="name">new locale key name</param>
        void Rename(string oldName, string name);

        /// <summary>
        /// Gets an existing locale by name and language
        /// </summary>
        /// <param name="name">locale key</param>
        /// <param name="language">language switch</param>
        string Get(string name, string language);
        /// <summary>
        /// Returns a list of all names
        /// </summary>
        ITab Names { get; }
        /// <summary>
        /// Returns all locales values (all languages) by name
        /// </summary>
        /// <param name="name">locale key name to search</param>
        /// <returns>a list of all values</returns>
        ITab GetValues(string name);
    }

    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE4C")]
    [ClassInterface(ClassInterfaceType.None)]
    internal class LocaleSet : ILocaleSet
    {
        #region Private Fields
        private string dir;
        private string groupName;
        #endregion

        #region Default Constructor
        public LocaleSet(string from, string groupName)
        {
            this.dir = from;
            this.groupName = groupName;
        }
        #endregion

        #region ILocaleSet Membres

        public string GroupName
        {
            get
            {
                return this.groupName;
            }
            set
            {
                LocaleSystem.RenameGroup(this.dir, this.groupName, value);
                this.groupName = value;
            }
        }

        public bool ExistsOne(string name, string language)
        {
            return LocaleSystem.ExistLocale(this.dir, this.groupName, name, language);
        }

        public bool Exists(string name)
        {
            return LocaleSystem.ExistLocale(this.dir, this.groupName, name);
        }

        public void Add(string name, string language, string value)
        {
            LocaleSystem.InsertLocale(this.dir, this.groupName, name, language, value);
        }

        public void Modify(string name, string language, string value)
        {
            LocaleSystem.ModifyLocale(this.dir, this.groupName, name, language, value);
        }

        public void DeleteOne(string name, string language)
        {
            LocaleSystem.RemoveLocale(this.dir, this.groupName, name, language);
        }

        public void Delete(string name)
        {
            LocaleSystem.RemoveLocale(this.dir, this.groupName, name);
        }

        public void RenameOne(string oldName, string name, string language)
        {
            LocaleSystem.RenameLocale(this.dir, oldName, this.groupName, name, language);
        }

        public void Rename(string oldName, string name)
        {
            LocaleSystem.RenameLocale(this.dir, oldName, this.groupName, name);
        }

        public string Get(string name, string language)
        {
            LocaleElement elem = LocaleSystem.GetLocale(this.dir, this.groupName, name, language);
            if (elem != null)
            {
                return elem.Value;
            }
            else
            {
                throw new Exception("L'objet locale '" + name + "' pour le language '" + language + "' n'existe pas");
            }
        }

        public ITab Names
        {
            get
            {
                List<LocaleElement> list = LocaleSystem.GetNames(this.dir, this.groupName);
                return Tab.FromArray(list.ToArray());
            }
        }

        public ITab GetValues(string name)
        {
            List<LocaleElement> list = LocaleSystem.GetLocale(this.dir, this.groupName, name);
            return Tab.FromArray(list.ToArray());
        }

        #endregion
    }
}
