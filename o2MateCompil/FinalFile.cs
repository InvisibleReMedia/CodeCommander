using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace o2Mate
{
    /// <summary>
    /// Implementation class to write the final output in a file
    /// during the execution of the CodeCommander source code
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class FinalFile
    {
        #region Private Constants
        private const string IndentString = "    ";
        #endregion

        #region Private Fields
        private string fileName;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        public string FileName
        {
            get { return this.fileName; }
            set { this.fileName = value; }
        }
        #endregion

        #region Public Static Properties
        /// <summary>
        /// the CodeCommander directory
        /// </summary>
        public static string CodeCommanderDirectory
        {
            get { return CodeCommander.Documents.CodeCommanderDirectory; }
        }

        /// <summary>
        /// The build directory
        /// </summary>
        public static string BuildDirectory
        {
            get { return CodeCommander.Documents.BuildDirectory; }
        }
        #endregion

        #region Private Methods
        private static bool ExtractInfos(string infos, out string fileName, out int indent, out bool startLine)
        {
            string data = infos.Substring(1, infos.Length - 2);
            string[] split = data.Split(',');
            fileName = split[0].Substring(1, split[0].Length - 2);
            indent = Int32.Parse(split[1]);
            startLine = Boolean.Parse(split[2]);
            return true;
        }

        private static string Indent(int indent)
        {
            string output = "";
            for(int index = 0; index < indent; ++index)
            {
                output += FinalFile.IndentString;
            }
            return output;
        }

        private static string IndentText(string text, int indent, ref bool startLine)
        {
            string output = "";
            Regex reg = new Regex("((.*" + Environment.NewLine + ")|.*$)");
            MatchCollection matches = reg.Matches(text);
            foreach (Match m in matches)
            {
                if (!String.IsNullOrEmpty(m.Groups[1].Value))
                {
                    if (!startLine)
                    {
                        if (m.Groups[1].Value.EndsWith(Environment.NewLine))
                        {
                            startLine = true;
                        }
                        output += m.Groups[1].Value;
                    }
                    else
                    {
                        if (!m.Groups[1].Value.EndsWith(Environment.NewLine))
                        {
                            startLine = false;
                        }
                        output += FinalFile.Indent(indent) + m.Groups[1].Value;
                    }
                }
            }
            return output;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The file begins to write
        /// </summary>
        /// <param name="fileName">file name where to write</param>
        public void StartWithFile(string fileName)
        {
            this.FileName = fileName;
            this.Start();
        }

        /// <summary>
        /// The known file begins to write
        /// </summary>
        public void Start()
        {
            FileStream fs = new FileStream(this.FileName, FileMode.Create);
            fs.Close();
        }

        /// <summary>
        /// This method allows to write a text in the file
        /// The file is opened and then immediately closed
        /// This method is synchronized in order to work for multi-threading
        /// </summary>
        /// <param name="text">the text to write</param>
        /// <param name="enc">encoding format</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteToFile(string text, Encoding enc)
        {
            FileStream fs = new FileStream(this.FileName, FileMode.Append, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(fs, enc);
            sw.Write(text);
            sw.Dispose();
            fs.Dispose();
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// This function ensures that the directory where to write exists
        /// </summary>
        /// <param name="fileName">full path of the file</param>
        public static void EnsureDirectoryCreated(string fileName)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(fileName);
                di.Create();
            }
            catch { }
        }

        /// <summary>
        /// This method kills any existing file
        /// </summary>
        /// <param name="fileName">file name</param>
        public static void EraseFile(string fileName)
        {
            try
            {
                FileInfo fi = new FileInfo(fileName);
                fi.Create().Close();
            }
            catch { }
        }

        /// <summary>
        /// This method offers to augment indentation
        /// It keeps the number of indentation in a formatted string
        /// </summary>
        /// <param name="destination">formatted string data</param>
        public static void Indent(ref string destination)
        {
            try
            {
                string fileName;
                int indent;
                bool startLine;
                if (FinalFile.ExtractInfos(destination, out fileName, out indent, out startLine))
                {
                    destination = "{\"" + fileName + "\"," + (indent + 1).ToString() + "," + startLine.ToString() + "}";
                }
            }
            catch { }
        }

        /// <summary>
        /// This method offers to decrease indentation
        /// It keeps the number of indentation in a formatted string
        /// </summary>
        /// <param name="destination">formatted string data</param>
        public static void Unindent(ref string destination)
        {
            try
            {
                string fileName;
                int indent;
                bool startLine;
                if (FinalFile.ExtractInfos(destination, out fileName, out indent, out startLine))
                {
                    if (indent > 0)
                        destination = "{\"" + fileName + "\"," + (indent - 1).ToString() + "," + startLine.ToString() + "}";
                }
            }
            catch { }
        }

        /// <summary>
        /// This method works with a formatted string data
        /// The first data is the full path of the file where to write
        /// The second data is the number of indentation
        /// The thirth data says true when starting a new line
        /// </summary>
        /// <param name="destination">formatted data information</param>
        /// <param name="text">text to write</param>
        /// <param name="enc">encoding object</param>
        public static void WriteToFile(ref string destination, string text, Encoding enc)
        {
            try
            {
                string fileName;
                int indent;
                bool startLine;
                if (FinalFile.ExtractInfos(destination, out fileName, out indent, out startLine))
                {
                    FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Write);
                    StreamWriter sw = new StreamWriter(fs, enc);
                    sw.Write(FinalFile.IndentText(text, indent, ref startLine));
                    sw.Dispose();
                    fs.Dispose();
                    destination = "{\"" + fileName + "\"," + indent.ToString() + "," + startLine.ToString() + "}";
                }
            }
            catch { }
        }
        #endregion
    }
}
