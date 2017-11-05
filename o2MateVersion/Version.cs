using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Security.Policy;
using System.Security;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace o2Mate
{
    /// <summary>
    /// Interface to store a number version
    /// </summary>
    [CoClass(typeof(IVersion))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IVersion
    {
        /// <summary>
        /// Major number
        /// </summary>
        uint Major { get; }
        /// <summary>
        /// Minor number
        /// </summary>
        uint Minor { get; }
        /// <summary>
        /// Build number
        /// </summary>
        uint Build { get; }
        /// <summary>
        /// Revision number
        /// </summary>
        uint Revision { get; }
    }

    /// <summary>
    /// Implementation for storing version number
    /// </summary>
    [ComVisible(true)]
    [Guid("CA901DF6-4ACC-48b6-8B1A-54406322EE54")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class Version : IVersion
    {
        #region Private Fields
        private uint major, minor, build, revision;
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Parses a string and returs a Version class
        /// </summary>
        /// <param name="s">string to parse</param>
        /// <returns>the version class if succeeded (else throw an exception)</returns>
        public static Version Parse(string s)
        {
            Regex r = new Regex(@"([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)");
            Match m = r.Match(s);
            if (m.Success)
            {
                uint major = Convert.ToUInt32(m.Groups[1].Value);
                uint minor = Convert.ToUInt32(m.Groups[2].Value);
                uint build = Convert.ToUInt32(m.Groups[3].Value);
                uint revision = Convert.ToUInt32(m.Groups[4].Value);

                return new Version(major, minor, build, revision);
            }
            else
                throw new FormatException("La version '" + s + "' n'est pas bien formée");
        }
        #endregion

        #region Public Constructors
        /// <summary>
        /// Constructor with each parts of a version
        /// </summary>
        /// <param name="major">major version number</param>
        /// <param name="minor">minor version number</param>
        /// <param name="build">build version number</param>
        /// <param name="revision">revision version number</param>
        public Version(uint major, uint minor, uint build, uint revision)
        {
            this.major = major;
            this.minor = minor;
            this.build = build;
            this.revision = revision;
        }

        /// <summary>
        /// Constructs a new version number from a string
        /// </summary>
        /// <param name="str">version string format</param>
        public Version(string str)
        {
            Version v = Version.Parse(str);
            this.major = v.major;
            this.minor = v.minor;
            this.build = v.build;
            this.revision = v.revision;
        }

        
        /// <summary>
        /// il faut un constructeur sans paramètre
        /// pour satisfaire la création d'un un objet COM
        /// </summary>
        public Version()
        {
            Version v = Version.Parse("1.0.0.0");
            this.major = v.major;
            this.minor = v.minor;
            this.build = v.build;
            this.revision = v.revision;
        }
        #endregion

        #region Public Static Properties
        /// <summary>
        /// Returns the current software version
        /// </summary>
        /// <returns></returns>
        public static IVersion GetSoftwareVersion()
        {
            return new FetchedVersion().GetSoftwareVersion();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Major number
        /// </summary>
        public uint Major
        {
            get { return this.major; }
        }

        /// <summary>
        /// Minor number
        /// </summary>
        public uint Minor
        {
            get { return this.minor; }
        }

        /// <summary>
        /// Build number
        /// </summary>
        public uint Build
        {
            get { return this.build; }
        }

        /// <summary>
        /// Revision number
        /// </summary>
        public uint Revision
        {
            get { return this.revision; }
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Transform the object's data to a string format
        /// </summary>
        /// <returns>the version string format</returns>
        public override string ToString()
        {
            return this.major.ToString() + "." + this.minor.ToString() + "." + this.build.ToString() + "." + this.revision.ToString();
        }
        #endregion
    }
}