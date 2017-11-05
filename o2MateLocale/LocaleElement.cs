using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;

namespace o2Mate
{
    /// <summary>
    /// Interface declaration of a locale
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface ILocale
    {
        /// <summary>
        /// Key name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Language
        /// </summary>
        string Language { get; }
        /// <summary>
        /// Content
        /// </summary>
        string Value { get; }
    }

    [Guid("CA901DF6-4ACC-48B6-8B1A-54406322EE4A")]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    internal class LocaleElement : ILocale
    {
        #region Private Fields
        private string name;
        private string language;
        private string value;
        #endregion

        #region Private Static Methods
        private static string IdentifyWord(StreamReader sr, ref int position, bool isLast)
        {
            bool innerQuote = false;
            string output = "";
            for (int index = position; !sr.EndOfStream; ++index)
            {
                char c = Convert.ToChar(sr.Read());
                if (c == '\"')
                {
                    if (!innerQuote) { innerQuote = true; }
                    else { innerQuote = false; }
                }
                else if (!innerQuote) {

                    if (!isLast)
                    {
                        if (c == (char)9)
                        {
                            position = index;
                            break;
                        }
                        else
                        {
                            output += c;
                        }
                    }
                    else
                    {
                        if (c == (char)10)
                        {
                            position = index;
                            break;
                        }
                        else if (c == (char)13) { }
                        else
                        {
                            output += c;
                        }
                    }
                }
                else
                {
                    output += c;
                }
            }
            return output;
        }

        #endregion

        #region Public Static Methods
        public static bool ValidateName(string name)
        {
            if (name.IndexOf('\"') != -1)
            {
                throw new ArgumentException("Le nom d'un objet Locale ne peut pas avoir de caractère \"");
            }
            else if (name.IndexOf(' ') != -1 || name.IndexOf('\t') != -1 || name.IndexOf(Environment.NewLine) != -1)
            {
                throw new ArgumentException("Le nom d'un objet Locale ne peut avoir d'espaces, de tabulation ou de retour chariot");
            }
            return true;
        }

        public static bool ValidateLanguage(string name)
        {
            if (name.IndexOf('\"') != -1)
            {
                throw new ArgumentException("Le langage d'un objet Locale ne peut avoir de caractère \"");
            }
            return true;
        }

        public static string FormatValue(string value)
        {
            string output = "";
            if (!String.IsNullOrEmpty(value))
            {
                string ret = "";
                ret += ((char)13);
                ret += ((char)10);
                output = value.Replace(ret, "\\r\\n");
                output = output.Replace("\"", "");
            }
            return output;
        }

        public static LocaleElement Parse(StreamReader sr, ref int next)
        {
            string name = LocaleElement.IdentifyWord(sr, ref next, false);
            string language = LocaleElement.IdentifyWord(sr, ref next, false);
            string value = LocaleElement.IdentifyWord(sr, ref next, true);
            return new LocaleElement(name, language, value);
        }
        #endregion

        #region Public Constructor
        public LocaleElement(string name, string language, string value)
        {
            if (LocaleElement.ValidateName(name) && LocaleElement.ValidateLanguage(language))
            {
                this.Name = name;
                this.Language = language;
                this.Value = value;
            }
        }
        #endregion

        #region Public Properties
        public string Name
        {
            get { return this.name; }
            set { if (LocaleElement.ValidateName(value)) { this.name = value; } }
        }

        public string Language
        {
            get { return this.language; }
            set { if (LocaleElement.ValidateLanguage(value)) { this.language = value; } }
        }

        public string Value
        {
            get
            {
                string ret = "";
                ret += ((char)13);
                ret += ((char)10);
                return this.value.Replace("\\r\\n", ret);
            }
            set { this.value = LocaleElement.FormatValue(value); }
        }
        #endregion

        #region Public Methods
        public void WriteToFile(StreamWriter sw)
        {
            sw.WriteLine("\"" + this.Name + "\"" + "\t\"" + this.Language + "\"\t" + this.value);
        }
        #endregion
    }
}
