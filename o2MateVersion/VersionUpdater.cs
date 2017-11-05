using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace o2Mate
{
    /// <summary>
    /// Interface to jump all versions for each projects
    /// </summary>
    [CoClass(typeof(IVersionUpdater))]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IVersionUpdater
    {
        /// <summary>
        /// Initialize parameters
        /// </summary>
        /// <param name="dir">path where the solution projects starts</param>
        /// <param name="programName">program name set</param>
        /// <param name="language">culture language</param>
        void SetParameters(string dir, string programName, string language);
        /// <summary>
        /// Commit : confirms all changes
        /// </summary>
        void Commit();
        /// <summary>
        /// Update a file and change all found keys with their values
        /// </summary>
        /// <param name="fileName">fileName to read</param>
        /// <param name="extension">extension to add to the file name</param>
        /// <param name="progName">program name set</param>
        /// <param name="language">culture language</param>
        void UpdateFile(string fileName, string extension, string progName, string language);
        /// <summary>
        /// The solution's principal project is going to be built
        /// </summary>
        void UpdateAssemblyVersion();
        /// <summary>
        /// Gets the short version of this software
        /// </summary>
        /// <returns>a version object</returns>
        IVersion GetSoftwareVersion();
    }


    /// <summary>
    /// Class implementation to update files and computes version number
    /// </summary>
    [ComVisible(true)]
    [Guid("CA901DF6-4ACC-48b6-8B1A-54406322EE55")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Serializable]
    public class FetchedVersion : IVersionUpdater
    {
        #region Private Constants
        private static readonly string[] assemblyNames = new string[] { "CodeCommander", "Converters", "Documents",
                                                          "NotifyProgress", "o2MateCompil", "o2MateDict",
                                                          "o2MateExpression", "o2MateLegende", "o2MateLocale",
                                                          "o2MateScope", "o2MateVersion", "UniqueNames"};
        private static readonly string[] registerForCom = new string[] { "o2MateVersion" };
        #endregion

        #region Private fields
        [NonSerialized]
        private o2Mate.ILocaleGroup versions;
        [NonSerialized]
        private System.Collections.Specialized.NameValueCollection nv;
        [NonSerialized]
        private DirectoryInfo sources;
        [NonSerialized]
        private string programName;
        [NonSerialized]
        private string language;
        #endregion

        #region Public Properties

        /// <summary>
        /// Dll or Exe Name
        /// </summary>
        public string ProgramName
        {
            get
            {
                if (this.programName != null)
                    return this.programName;
                else
                    throw new ArgumentException("Utilisation des fonctionnalités hors du champ d'action");
            }
        }

        /// <summary>
        /// CultureInfo language (two letters format)
        /// </summary>
        public string Language
        {
            get
            {
                if (this.language != null)
                    return this.language;
                else
                    throw new ArgumentException("Utilisation des fonctionnalités hors du champ d'action");
            }
        }

        #endregion

        #region Public Constructors
        /// <summary>
        /// Constructor with explicit parameters (only used for update versions)
        /// </summary>
        /// <param name="src">directory path of the solution</param>
        /// <param name="programName">Dll or Exe name</param>
        /// <param name="language">CultureInfo name</param>
        public FetchedVersion(string src, string programName, string language)
        {
            this.SetParameters(src, programName, language);
            this.versions = new LocaleGroup(CodeCommander.Documents.HostVersions);
            this.nv = new System.Collections.Specialized.NameValueCollection();
        }

        /// <summary>
        /// Constructor for update sources files
        /// <param name="src">source path</param>
        /// </summary>
        public FetchedVersion(string src)
        {
            this.sources = new DirectoryInfo(src);
            this.versions = new LocaleGroup(CodeCommander.Documents.HostVersions);
            this.nv = new System.Collections.Specialized.NameValueCollection();
        }

        /// <summary>
        /// Constructor with no prameters
        /// </summary>
        public FetchedVersion()
        {
            this.versions = new LocaleGroup(CodeCommander.Documents.HostVersions);
            this.nv = new System.Collections.Specialized.NameValueCollection();
        }
        #endregion

        #region Private Static Methods
        private static void RecursiveDirectoryCreation(string dir)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            if (!di.Exists)
            {
                FetchedVersion.RecursiveDirectoryCreation(Path.GetDirectoryName(di.FullName));
                di.Create();
            }
        }

        #endregion

        #region Private Methods

        private IVersion TestModifiedFiles(IVersion currentVersion)
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(this.sources.FullName, this.ProgramName));
            ILocaleGroup group = new LocaleGroup(CodeCommander.Documents.HostVersions);
            ILocaleSet set;
            if (!group.Exists(this.ProgramName + "_files"))
            {
                group.Create(this.ProgramName + "_files");
            }
            set = group.Get(this.ProgramName + "_files");
            // compte le nombre de fichiers modifiés
            uint countModified = 0;
            // chercher les dernières dates de modification des fichiers
            foreach (FileInfo fi in this.GetFiles(dir))
            {
                if (set.ExistsOne(fi.Name, this.Language))
                {
                    string value = set.Get(fi.Name.Replace(" ", "_"), this.Language);
                    if (value != fi.LastWriteTimeUtc.ToString())
                    {
                        set.Modify(fi.Name.Replace(" ", "_"), this.Language, fi.LastWriteTimeUtc.ToString());
                        ++countModified;
                    }
                }
                else
                {
                    set.Add(fi.Name.Replace(" ", "_"), this.Language, fi.LastWriteTimeUtc.ToString());
                    ++countModified;
                }
            }

            if (countModified > 0)
                return new Version(countModified, currentVersion.Minor + 1, currentVersion.Build, currentVersion.Revision);
            else
                return new Version(currentVersion.Major, currentVersion.Minor + 1, currentVersion.Build, currentVersion.Revision);
        }

        private IEnumerable<FileInfo> GetFiles(DirectoryInfo di)
        {
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension != ".snk" && fi.Extension != ".pfx" && fi.Extension != ".png" && fi.Extension != ".jpg" && fi.Extension != ".bmp" && fi.Extension != ".ico")
                    yield return fi;
            }
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                if (subDi.Name != "bin" && subDi.Name != "obj" && subDi.Name != "Docs")
                {
                    foreach (FileInfo fi in this.GetFiles(subDi))
                    {
                        yield return fi;
                    }
                }
            }
        }

        private IVersion CountSource()
        {
            DirectoryInfo dir = new DirectoryInfo(Path.Combine(this.sources.FullName, this.ProgramName));
            // compter le nombre de fichiers .cs
            int counterFiles = 0;
            // compte le nombre de lignes (non vides)
            int counterLines = 0;
            FileInfo fiSources = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "all_sources.txt");
            if (fiSources.Exists)
            {
                fiSources.Delete();
                using (StreamWriter sw = fiSources.CreateText())
                {
                    sw.Close();
                }
            }
            foreach (FileInfo f in this.GetFiles(dir))
            {
                ++counterFiles;
                using (StreamWriter sw = fiSources.AppendText()) {
                    using(StreamReader sr = f.OpenText()) {
                        sw.Write(sr.ReadToEnd());
                        sr.Close();
                    }
                    sw.Close();
                }
            }
            using (StreamReader sr = fiSources.OpenText()) {
                while (!sr.EndOfStream)
                {
                    if (!String.IsNullOrWhiteSpace(sr.ReadLine())) ++counterLines;
                }
                sr.Close();
            }
            long value = counterLines * (counterFiles + 1) + counterFiles;
            string binRep = Convert.ToString(value, 2);
            string left, right;
            if (binRep.Length % 2 == 0)
            {
                left = binRep.Substring(0, binRep.Length >> 1);
                right = binRep.Substring(binRep.Length >> 1, binRep.Length >> 1);
            }
            else
            {
                left = binRep.Substring(0, binRep.Length >> 1);
                right = binRep.Substring(binRep.Length >> 1, (binRep.Length >> 1) + 1);
            }

            return new Version(Convert.ToUInt32(left, 2), Convert.ToUInt32(right, 2), 0, 0);
        }

        /// <summary>
        /// Calcule la version du produit
        /// (voir le fichier numérotation.txt pour les détails)
        /// 
        /// </summary>
        /// <returns>le numéro de version</returns>
        private IVersion GetProductVersion()
        {
            // la version de la production du
            // logiciel est stockée dans
            // le répertoire où je stocke les
            // versions actuelles des fichiers .DLL et .EXE
            ILocaleSet set = this.versions.Get("Versioning");
            if (set.ExistsOne("ProductVersion", "")) {
                uint majorVersion = 0, minorVersion = 0;
                Regex r = new Regex(@"^([0-9]+)\.([0-9]+)");
                Match m = r.Match(set.Get("ProductVersion", ""));
                if (m.Success) {
                    majorVersion = Convert.ToUInt32(m.Groups[1].Value);
                    minorVersion = Convert.ToUInt32(m.Groups[2].Value);
                }
                return new Version(majorVersion, minorVersion, 0, 0);
            }
            else
            {
                // version par défaut
                return new Version(global::o2Mate.Properties.Settings.Default.DefaultMajorVersion, global::o2Mate.Properties.Settings.Default.DefaultMinorVersion, 0, 0);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize parameters (used for COM object initializing)
        /// </summary>
        /// <param name="dir">directory path of the solution</param>
        /// <param name="progName">Dll or Exe name</param>
        /// <param name="language">CultureInfo name</param>
        public void SetParameters(string dir, string progName, string language)
        {
            this.sources = new DirectoryInfo(dir);
            this.programName = progName;
            this.language = language;
        }

        /// <summary>
        /// Computes the product version
        /// The product version is the user comprehensive version
        /// and next numbers are the number of files and the number of lines in all files
        /// </summary>
        public void ComputeProductVersion()
        {
            IVersion product = this.GetProductVersion();
            IVersion files = this.CountSource();
            IVersion result = new Version(product.Major, product.Minor, files.Major, files.Minor);
            this.nv.Add("ProductVersion", result.ToString());
        }

        /// <summary>
        /// Computes the file version
        /// The file version are the number of files and the number of lines in all files
        /// and the next numbers are the number of modified files and a number of compilation execution
        /// </summary>
        public void ComputeFileVersion()
        {
            IVersion files = this.CountSource();
            if (!this.versions.Exists(this.ProgramName))
            {
                this.versions.Create(this.ProgramName);
            }
            ILocaleSet set = this.versions.Get(this.ProgramName);
            IVersion current = null;
            if (set.ExistsOne("FileVersion", this.Language))
            {
                current = new Version(set.Get("FileVersion", this.Language));
            }
            else
            {
                current = new Version(1, 0, 0, 0);
            }
            IVersion product = this.TestModifiedFiles(current);
            IVersion result = new Version(files.Major, files.Minor, product.Major, product.Minor);
            this.nv.Add("FileVersion", result.ToString());
        }

        /// <summary>
        /// Computes the assembly version
        /// The file version are the number of modified files and a number of compilation execution
        /// and the next numbers are the number of files and the number of lines in all files
        /// </summary>
        public void ComputeAssemblyVersion()
        {
            IVersion files = this.CountSource();
            if (!this.versions.Exists(this.ProgramName))
            {
                this.versions.Create(this.ProgramName);
            }
            ILocaleSet set = this.versions.Get(this.ProgramName);
            IVersion current = null;
            if (set.ExistsOne("FileVersion", this.Language))
            {
                current = new Version(set.Get("FileVersion", this.Language));
            }
            else
            {
                current = new Version(1, 0, 0, 0);
            }
            IVersion product = this.TestModifiedFiles(current);
            IVersion result = new Version(product.Major, product.Minor, files.Major, files.Minor);
            this.nv.Add("AssemblyVersion", result.ToString());
        }

        /// <summary>
        /// This function updates version values associated with the program name
        /// </summary>
        public void Commit()
        {
            if (!this.versions.Exists(this.ProgramName))
            {
                this.versions.Create(this.ProgramName);
            }
            ILocaleSet set = this.versions.Get(this.ProgramName);

            this.ComputeProductVersion();
            this.ComputeFileVersion();
            this.ComputeAssemblyVersion();

            foreach (string key in this.nv.Keys)
            {
                if (set.Exists(key))
                {
                    Console.WriteLine("update " + this.programName + " :" + key + "," + this.nv[key]);
                    set.Modify(key, this.Language, this.nv[key]);
                }
                else
                {
                    Console.WriteLine("add " + this.programName + " :" + key + "," + this.nv[key]);
                    set.Add(key, this.Language, this.nv[key]);
                }
            }
        }

        /// <summary>
        /// This function reads a file and rewrites it with reported values
        /// </summary>
        /// <param name="fileName">file to rewrite</param>
        /// <param name="extension">new file extension (ex: .cs)</param>
        /// <param name="progName">Dll or Exe name</param>
        /// <param name="language">CultureInfo name</param>
        public void UpdateFile(string fileName, string extension, string progName, string language)
        {
            if (!this.versions.Exists(progName))
            {
                this.versions.Create(progName);
            }
            ILocaleSet set = this.versions.Get(progName);

            using(FileStream fsR = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fsR))
                {
                    using (FileStream fsW = new FileStream(fileName + extension, FileMode.Create, FileAccess.Write, FileShare.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fsW))
                        {
                            Regex r = new Regex(@"(\$<([^>]+)>)", RegexOptions.Multiline);
                            string dataToWrite = r.Replace(sr.ReadToEnd(), new MatchEvaluator(a => {
                                string name = a.Groups[2].Value;
                                if (set.ExistsOne(name, language))
                                {
                                    return set.Get(name, language).Replace(Environment.NewLine, "\\r\\n");
                                }
                                else
                                {
                                    return String.Empty;
                                }
                            }));
                            sw.Write(dataToWrite);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Update assemblies and recompile
        /// </summary>
        public void UpdateAssemblyVersion()
        {
            foreach (string name in FetchedVersion.assemblyNames)
            {
                string assemblyInfoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AssemblyInfo.txt");
                this.UpdateFile(assemblyInfoPath, ".cs", name, "");
                FileInfo fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AssemblyInfo.txt.cs"));
                fi.CopyTo(Path.Combine(this.sources.FullName, name, "Properties", "AssemblyInfo.cs"), true);
            }

            ProcessStartInfo procInfo = new ProcessStartInfo(@"C:\Program Files (x86)\MSBuild\12.0\Bin\amd64\msbuild.exe",
                                    " /t:Rebuild /p:Configuration=Debug;Platform=x64 \"" + Path.Combine(this.sources.FullName, CodeCommander.Documents.ProgramName, CodeCommander.Documents.ProgramName + ".csproj") + "\"");
            procInfo.UseShellExecute = false;
            procInfo.RedirectStandardError = true;
            Process.Start(procInfo).WaitForExit();
        }

        /// <summary>
        /// Gets the software version
        /// </summary>
        /// <returns>an IVersion interface</returns>
        public IVersion GetSoftwareVersion()
        {
            if (!this.versions.Exists(CodeCommander.Documents.ProgramName))
            {
                this.versions.Create(CodeCommander.Documents.ProgramName);
            }
            ILocaleSet set = this.versions.Get(CodeCommander.Documents.ProgramName);

            return new Version(set.Get("ProductVersion", System.Globalization.CultureInfo.InvariantCulture.Name));
        }
        #endregion
    }
}

