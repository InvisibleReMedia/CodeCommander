using System;
using System.Collections.Generic;
using System.Text;

namespace o2Mate
{
    /// <summary>
    /// a project item class
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class ProjectItem
    {
        #region Private Fields
        private string processName;
        private int position;
        private string type;
        private string[] values;
        #endregion

        #region Public Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="processName">the process name where the statement is executed</param>
        /// <param name="position">the position pointer of statement</param>
        /// <param name="type">type of statement</param>
        /// <param name="values">dictionary values</param>
        public ProjectItem(string processName, int position, string type, params string[] values)
        {
            this.processName = processName;
            this.position = position;
            this.type = type;
            this.values = values;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the type of statement
        /// </summary>
        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// Gets or sets the process name
        /// </summary>
        public string ProcessName
        {
            get { return this.processName; }
            set { this.processName = value; }
        }

        /// <summary>
        /// Gets or sets the position pointer of statement
        /// </summary>
        public int Position
        {
            get { return this.position; }
            set { this.position = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Print all statements into the current project
        /// </summary>
        /// <param name="dict">dictionary values</param>
        public void Print(o2Mate.Dictionnaire dict)
        {
            Dictionary<string, string> tabVal = new Dictionary<string, string>();
            tabVal.Add("processName", this.processName);
            tabVal.Add("position", this.position.ToString());
            tabVal.Add("type", this.type);
            switch (this.type)
            {
                case "thread":
                    break;
                case "affectation":
                    tabVal.Add("primaryKey", this.values[0]);
                    tabVal.Add("varName", this.values[0]);
                    Projects.AddIntoArray(dict, "affectation", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "dictionary string":
                    tabVal.Add("primaryKey", this.values[0] + ":" + this.values[1]);
                    tabVal.Add("varName", this.values[0]);
                    tabVal.Add("stringName", this.values[1]);
                    Projects.AddIntoArray(dict, "dictionary string", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "dictionary field":
                    tabVal.Add("primaryKey", this.values[0] + ":" + this.values[1] + "." + this.values[2]);
                    tabVal.Add("varName", this.values[0]);
                    tabVal.Add("tabName", this.values[1]);
                    tabVal.Add("fieldName", this.values[2]);
                    Projects.AddIntoArray(dict, "dictionary field", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "begin process":
                    tabVal.Add("beginProcessName", this.values[0]);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "begin skeleton":
                    tabVal.Add("beginSkeletonName", this.values[0]);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "call process":
                    tabVal.Add("primaryKey", this.values[0]);
                    tabVal.Add("callProcessName", this.values[0]);
                    Projects.AddIntoArray(dict, "call process", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "call skeleton":
                    tabVal.Add("primaryKey", this.values[0]);
                    tabVal.Add("callSkeletonName", this.values[0]);
                    Projects.AddIntoArray(dict, "call skeleton", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "print field":
                    tabVal.Add("primaryKey", this.values[0] + "." + this.values[1]);
                    tabVal.Add("tabName", this.values[0]);
                    tabVal.Add("fieldName", this.values[1]);
                    Projects.AddIntoArray(dict, "print field", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "goto":
                    tabVal.Add("primaryKey", this.values[0]);
                    tabVal.Add("label", this.values[0]);
                    Projects.AddIntoArray(dict, "goto", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "create mop":
                    tabVal.Add("primaryKey", this.values[1] + "(" + this.values[0] + ")");
                    tabVal.Add("languageName", this.values[0]);
                    tabVal.Add("createMopName", this.values[1]);
                    Projects.AddIntoArray(dict, "create mop", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "create writer":
                    tabVal.Add("primaryKey", this.values[1]);
                    tabVal.Add("writerName", this.values[0]);
                    tabVal.Add("fileName", this.values[1]);
                    Projects.AddIntoArray(dict, "create writer", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "default writer":
                    tabVal.Add("varName", this.values[0]);
                    Projects.AddIntoArray(dict, "default writer", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "end process":
                    tabVal.Add("endProcessName", this.values[0]);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "end skeleton":
                    tabVal.Add("endSkeletonPath", this.values[0]);
                    tabVal.Add("endSkeletonName", this.values[1]);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "injector":
                    tabVal.Add("primaryKey", this.values[0]);
                    tabVal.Add("injectorName", this.values[0]);
                    Projects.AddIntoArray(dict, "injector", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "label":
                    tabVal.Add("primaryKey", this.processName + "." + this.values[0]);
                    tabVal.Add("labelName", this.values[0]);
                    Projects.AddIntoArray(dict, "label", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "size":
                    tabVal.Add("primaryKey", this.values[0] + ":" + this.values[1]);
                    tabVal.Add("varName", this.values[0]);
                    tabVal.Add("tabName", this.values[1]);
                    Projects.AddIntoArray(dict, "size", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "declare template":
                    tabVal.Add("primaryKey", this.values[0] + "/" + this.values[1]);
                    tabVal.Add("templatePath", this.values[0]);
                    tabVal.Add("templateName", this.values[1]);
                    tabVal.Add("parameters", this.values[2]);
                    Projects.AddIntoArray(dict, "declare template", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "print":
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "use mop":
                    tabVal.Add("primaryKey", this.values[1] + "(" + this.values[0] + ")");
                    tabVal.Add("languageName", this.values[0]);
                    tabVal.Add("command", this.values[1]);
                    Projects.AddIntoArray(dict, "use mop", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "use template":
                    tabVal.Add("primaryKey", this.values[0]);
                    tabVal.Add("templateName", this.values[0]);
                    Projects.AddIntoArray(dict, "use template", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "print variable":
                    tabVal.Add("primaryKey", this.values[0]);
                    tabVal.Add("varName", this.values[0]);
                    Projects.AddIntoArray(dict, "print variable", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "print string":
                    tabVal.Add("primaryKey", this.values[0]);
                    tabVal.Add("stringName", this.values[0]);
                    Projects.AddIntoArray(dict, "print string", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
                case "project":
                    tabVal.Add("primaryKey", this.values[0]);
                    tabVal.Add("name", this.values[0]);
                    Projects.AddIntoArray(dict, "project", tabVal);
                    Projects.AddIntoProcess(dict, this.processName, this.position, tabVal);
                    break;
            }
        }
        #endregion
    }
}
