using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace o2Mate
{
    /// <summary>
    /// Interface declaration of a locale group
    /// </summary>
    [CoClass(typeof(ILocaleGroup))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface ILocaleGroup
    {
        /// <summary>
        /// Checks and returns parts
        /// </summary>
        /// <param name="text">text to parse</param>
        /// <param name="groupName">returned group name</param>
        /// <param name="name">returned locale name</param>
        /// <returns>true if well-formed</returns>
        bool ExtractGroupAndName(string text, out string groupName, out string name);
        /// <summary>
        /// Says if a locale group by name exists
        /// </summary>
        /// <param name="name">the group name to search</param>
        /// <returns>true if exists</returns>
        bool Exists(string name);
        /// <summary>
        /// Creates a new locale group
        /// </summary>
        /// <param name="name">the group name to create</param>
        void Create(string name);
        /// <summary>
        /// Suppress an existing locale group
        /// </summary>
        /// <param name="name">the group name to suppress</param>
        void Remove(string name);
        /// <summary>
        /// Reads the group name and returns a reference to it
        /// </summary>
        /// <param name="name">group name</param>
        /// <returns>a locale set</returns>
        ILocaleSet Get(string name);
        /// <summary>
        /// Search for all group names
        /// </summary>
        string Groups { get; }
    }

    /// <summary>
    /// Implementation of a locale group
    /// </summary>
    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE4B")]
    [ClassInterface(ClassInterfaceType.None)]
    public class LocaleGroup : ILocaleGroup
    {
        #region Private Fields
        private string directory;
        #endregion

        #region Public Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public LocaleGroup() { this.directory = CodeCommander.Documents.LocalesDirectory; }

        /// <summary>
        /// Creates a locale group (a defined directory where to search)
        /// </summary>
        /// <param name="dir">directory string</param>
        public LocaleGroup(string dir)
        {
            this.directory = dir;
        }
        #endregion

        #region ILocaleGroup Membres

        /// <summary>
        /// Checks and returns parts
        /// </summary>
        /// <param name="text">text to parse</param>
        /// <param name="groupName">returned group name</param>
        /// <param name="name">returned locale name</param>
        /// <returns>true if well-formed</returns>
        public bool ExtractGroupAndName(string text, out string groupName, out string name)
        {
            groupName = "";
            name = "";
            bool result = false;
            try
            {
                string[] split = text.Split('.');
                if (split.Length > 1)
                {
                    LocaleSystem.ValidateGroupName(split[0]);
                    LocaleElement.ValidateName(String.Join(".", split, 1, split.Length - 1));
                    groupName = split[0];
                    name = String.Join(".", split, 1, split.Length - 1);
                    result = true;
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// Says if a locale group by name exists
        /// </summary>
        /// <param name="name">the group name to search</param>
        /// <returns>true if exists</returns>
        public bool Exists(string name)
        {
            return LocaleSystem.ExistsGroup(this.directory, name);
        }

        /// <summary>
        /// Creates a new locale group
        /// </summary>
        /// <param name="name">the group name to create</param>
        public void Create(string name)
        {
            LocaleSystem.CreateGroup(this.directory, name);
        }

        /// <summary>
        /// Suppress an existing locale group
        /// </summary>
        /// <param name="name">the group name to suppress</param>
        public void Remove(string name)
        {
            LocaleSystem.DeleteGroup(this.directory, name);
        }

        /// <summary>
        /// Reads the locale group and returns a reference to it
        /// </summary>
        /// <param name="name">group name</param>
        /// <returns>a locale set</returns>
        public ILocaleSet Get(string name)
        {
            return LocaleSystem.GetSet(this.directory, name);
        }

        /// <summary>
        /// Search for all group names
        /// </summary>
        public string Groups
        {
            get
            {
                List<string> groups = LocaleSystem.GetGroups(this.directory);
                string output = String.Empty;
                foreach (string s in groups)
                {
                    output += " " + s;
                }
                return output.Substring(1);
            }
        }

        #endregion
    }
}
