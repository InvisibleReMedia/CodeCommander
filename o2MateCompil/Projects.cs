using System;
using System.Collections.Generic;
using System.Text;

namespace o2Mate
{
    /// <summary>
    /// Project class
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class Projects
    {
        #region Private Fields
        private o2Mate.Dictionnaire dict;
        private bool record;
        #endregion

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Projects()
        {
            this.dict = new o2Mate.Dictionnaire();
            this.record = false;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the dictionary values
        /// </summary>
        public o2Mate.Dictionnaire Dictionary
        {
            get { return this.dict; }
        }

        /// <summary>
        /// Gets or sets if statements has to be recorded
        /// </summary>
        public bool Record
        {
            get { return this.record; }
            set { this.record = value; }
        }
        #endregion

        #region Internal Static Methods
        internal static bool AddIntoArray(o2Mate.Dictionnaire dict, string arrayName, Dictionary<string, string> values)
        {
            bool add = true;
            o2Mate.Array arr = null;
            if (dict.IsArray(arrayName))
            {
                arr = dict.GetArray(arrayName) as o2Mate.Array;
                for (int index = 1; index <= arr.Count; ++index)
                {
                    o2Mate.Fields f = arr.Item(index) as o2Mate.Fields;
                    string pKey = f.GetString("primaryKey");
                    if (pKey == values["primaryKey"])
                    {
                        add = false;
                    }
                }
            }
            else
            {
                arr = new o2Mate.Array();
                dict.AddArray(arrayName, arr);
            }
            if (add)
            {
                o2Mate.Fields f = new o2Mate.Fields();
                foreach (KeyValuePair<string, string> keyVal in values)
                {
                    f.AddString(keyVal.Key, keyVal.Value);
                }
                arr.Add(f);
            }
            // retourne vrai si l'objet a été ajouté
            return add;
        }

        internal static void AddIntoProcess(o2Mate.Dictionnaire dict, string processName, int position, Dictionary<string, string> values)
        {
            int firstPos = 0;
            int currentPos = 1;
            o2Mate.Array statements = null;
            o2Mate.Fields process = null;
            if (dict.IsArray("statements"))
            {
                statements = dict.GetArray("statements") as o2Mate.Array;
                currentPos = statements.Count + 1;
            }
            else
            {
                statements = new o2Mate.Array();
                dict.AddArray("statements", statements);
            }
            o2Mate.Array processes = null;
            bool found = false;
            if (dict.IsArray("processes"))
            {
                // je recherche le processus qui a le même nom
                processes = dict.GetArray("processes") as o2Mate.Array;
                for (int index = 1; index <= processes.Count; ++index)
                {
                    process = processes.Item(index) as o2Mate.Fields;
                    if (process.GetString("processName") == processName)
                    {
                        Int32.TryParse(process.GetString("current"), out currentPos);
                        Int32.TryParse(process.GetString("first"), out firstPos);
                        found = true;
                        break;
                    }
                }
            }
            else
            {
                processes = new o2Mate.Array();
                dict.AddArray("processes", processes);
            }
            if (!found)
            {
                process = new o2Mate.Fields();
                process.AddString("processName", processName);
                process.AddString("first", currentPos.ToString());
                process.AddString("current", currentPos.ToString());
                processes.Add(process);
            }
            else
            {
                found = false;
                for (int index = firstPos; index <= currentPos; ++index)
                {
                    o2Mate.Fields f = statements.Item(index) as o2Mate.Fields;
                    // si c'est le même process et que c'est la même position dans le process
                    if (f.GetString("processName") == processName && f.GetString("position") == position.ToString())
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    o2Mate.Fields previous = statements.Item(currentPos) as o2Mate.Fields;
                    previous.AddString("next", (statements.Count + 1).ToString());
                }
            }
            if (!found)
            {
                o2Mate.Fields statmt = new o2Mate.Fields();
                statmt.AddString("processName", processName);
                statmt.AddString("position", position.ToString());
                statmt.AddString("next", "0");
                foreach (KeyValuePair<string, string> keyVal in values)
                {
                    statmt.AddString(keyVal.Key, keyVal.Value);
                }
                statements.Add(statmt);
                process.AddString("current", statements.Count.ToString());
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds an new project item
        /// </summary>
        /// <param name="elem">newly project item</param>
        internal void Add(ProjectItem elem)
        {
            if (this.record)
                elem.Print(this.dict);
        }
        #endregion
    }
}
