using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Converters;

namespace o2Mate
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class CreateWriter : ICompiler, ILanguageConverter
    {
        #region Private Fields
        private int indent;
        private string writerName;
        private IData newWriter;
        private List<string> filePath;
        private ICompilateurInstance cachedComp;
        private List<string> directories, fileName;
        #endregion

        #region Public Properties
        public string WriterName
        {
            get { return this.writerName; }
        }

        public List<string> FilePath
        {
            get { return this.filePath; }
        }
        #endregion

        #region Private Methods
        private System.Windows.Forms.HtmlElement GetElementByName(System.Windows.Forms.HtmlElement from, string name)
        {
            if (from.Name == name)
            {
                return from;
            }
            else
            {
                foreach (System.Windows.Forms.HtmlElement child in from.Children)
                {
                    try
                    {
                        return GetElementByName(child, name);
                    }
                    catch { }
                }
                throw new Exception("this sub-element does not contain that name element");
            }
        }
        #endregion

        #region Compiler Membres

        public string TypeName
        {
            get { return "CreateWriter"; }
        }

        public void ExtractDictionary(IProcessInstance proc)
        {
        }

        public void Inject(Injector injector, ICompilateurInstance comp, System.Xml.XmlNode node, string modifier)
        {
            this.Load(comp, node);
            if (comp.UnderConversion)
            {
                string s = String.Empty;
                if (!this.IsComputable(injector.InjectedProcess, out s))
                {
                    throw new Exception(s);
                }
            }
        }

        public bool IsComputable(IProcess proc, out string reason)
        {
            reason = null;
            // les noms des paramètres sont triés par ordre du plus long au plus court
            foreach (IParameter param in proc.Replacements)
            {
                if (this.writerName.Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                {
                    if (!param.IsComputable)
                    {
                        reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.writerName);
                        return false;
                    }
                    this.writerName = this.writerName.Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                }
                for (int index = 0; index < this.filePath.Count; ++index)
                {
                    if (this.filePath[index].Contains(Compilateur.ParamIdentifier.ToString() + param.FormalParameter))
                    {
                        if (!param.IsComputable)
                        {
                            reason = String.Format("Le paramètre '{0:G}' n'est pas calculable et ne peut pas être remplacé dans '{1:G}'", param.FormalParameter, this.filePath[index]);
                            return false;
                        }
                        this.filePath[index] = this.filePath[index].Replace(Compilateur.ParamIdentifier.ToString() + param.FormalParameter, param.EffectiveParameter);
                    }
                }
            }
            return true;
        }

        public void Load(ICompilateurInstance comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.writerName = node.Attributes.GetNamedItem("name").Value;
            this.filePath = new List<string>();
            if (node.SelectNodes("file")[0].FirstChild != null)
            {
                foreach (XmlNode paramNode in node.SelectNodes("file/expression"))
                {
                    string paramValue = paramNode.InnerText;
                    this.filePath.Add(paramValue);
                }
            }
            // on en a besoin pour la conversion
            this.cachedComp = comp;
        }

        public void Display(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            string strFile = String.Empty;
            bool first = true;
            foreach (string expression in this.filePath)
            {
                if (!first) { strFile += "/"; } else { first = false; }
                strFile += expression;
            }
            list.Add(new DisplayElement(this.TypeName, "displayWriter", new object[] { this.writerName, strFile, this.indent.ToString(), false }));
        }

        public void DisplayReadOnly(BindingList<DisplayElement> list, Node<ICompiler> node, bool forcedIndent, ref int indent)
        {
            if (forcedIndent)
            {
                this.indent = indent;
            }
            string strFile = String.Empty;
            bool first = true;
            foreach (string expression in this.filePath)
            {
                if (!first) { strFile += "/"; } else { first = false; }
                strFile += expression;
            }
            list.Add(new DisplayElement(this.TypeName, "displayWriter", new object[] { this.writerName, strFile, this.indent.ToString(), true }));
        }

        public void Update(System.Windows.Forms.HtmlElement elem)
        {
            Int32.TryParse(elem.GetAttribute("indent"), out this.indent);
            this.writerName = this.GetElementByName(elem, "writer").InnerText;
            if (String.IsNullOrEmpty(this.writerName)) this.writerName = "";
            string strFile = this.GetElementByName(elem, "file").InnerText;
            this.filePath = new List<string>();
            string[] splitPath = strFile.Split('/');
            foreach (string directory in splitPath)
            {
                if (!String.IsNullOrEmpty(directory))
                {
                    string value = directory;
                    value = value.Replace("&", "&amp;");
                    value = value.Replace("<", "&lt;");
                    value = value.Replace(">", "&gt;");
                    this.filePath.Add(value);
                }
            }
        }

        public void Save(ICompilateur comp, System.Xml.XmlWriter writer, ref System.Windows.Forms.HtmlElement child)
        {
            writer.WriteStartElement("createwriter");
            writer.WriteAttributeString("name", this.writerName);
            if (this.indent > 0)
                writer.WriteAttributeString("indent", this.indent.ToString());
            writer.WriteStartElement("file");
            foreach (string expression in this.filePath)
            {
                writer.WriteStartElement("expression");
                writer.WriteString(expression);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            child = child.NextSibling;
        }

        public void Parse(ICompilateur comp, System.Xml.XmlNode node)
        {
            if (node.Attributes.GetNamedItem("indent") != null)
                Int32.TryParse(node.Attributes.GetNamedItem("indent").Value, out this.indent);
            this.writerName = node.Attributes.GetNamedItem("name").Value;
            this.filePath = new List<string>();
            if (node.SelectNodes("file")[0].FirstChild != null)
            {
                foreach (XmlNode paramNode in node.SelectNodes("file/expression"))
                {
                    string paramValue = paramNode.InnerText;
                    this.filePath.Add(paramValue);
                }
            }
        }

        public void WriteToFile(IProcessInstance proc, FinalFile file)
        {
            string fileName = FinalFile.BuildDirectory;
            for(int index = 0; index < this.filePath.Count - 1; ++index)
            {
                string expression = this.filePath[index];
                o2Mate.Expression expr = new o2Mate.Expression();
                o2Mate.IData result = expr.Evaluate(expression, proc.CurrentScope);
                string value = result.ValueString;
                foreach (char c in Path.GetInvalidPathChars())
                {
                    int position = -1;
                    if ((position = value.IndexOf(c)) != -1) value = value.Substring(0, position) + value.Substring(position + 1);
                }
                fileName += value + Path.DirectorySeparatorChar;
                FinalFile.EnsureDirectoryCreated(fileName);
            }
            {
                string expression = this.filePath[this.filePath.Count - 1];
                o2Mate.Expression expr = new o2Mate.Expression();
                // le séparateur . permet d'utiliser des expressions
                string[] split = expression.Split('.');
                bool first = true;
                foreach (string s in split)
                {
                    o2Mate.IData result = expr.Evaluate(s, proc.CurrentScope);
                    string value = result.ValueString;
                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        int position = -1;
                        if ((position = value.IndexOf(c)) != -1)
                            value = value.Substring(0, position) + value.Substring(position + 1);
                    }
                    if (!first) fileName += "."; else first = false;
                    fileName += value;
                }
            }
            FinalFile.EraseFile(fileName);
            if (proc.CurrentScope.Exists(this.writerName))
            {
                IData d = proc.CurrentScope.GetVariable(this.writerName);
                d.Value = "{\"" + fileName + "\",0,true}";
                d.IsComputable = true;
                proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "create writer", this.writerName, fileName));
            }
            else
            {
                proc.CurrentScope.Add(this.writerName, "{\"" + fileName + "\",0,true}", proc.Name, true);
                proc.CurrentProject.Add(new ProjectItem(proc.Name, proc.CurrentPositionExecution, "create writer", this.writerName, fileName));
            }
        }

        public void Convert(Converters.ICodeConverter converter, IProcessInstance proc, FinalFile file)
        {
            o2Mate.Expression expr = new o2Mate.Expression();
            this.directories = new List<string>();
            for (int index = 0; index < this.filePath.Count - 1; ++index)
            {
                string expression = this.filePath[index];
                string path = Helper.ConvertNewExpression(this.cachedComp, proc, converter, expression, EnumDataType.E_STRING_OBJECT);
                this.directories.Add(path);
            }
            this.fileName = new List<string>();
            {
                string expression = this.filePath[this.filePath.Count - 1];
                string[] split = expression.Split('.');
                foreach (string s in split)
                {
                    string path = Helper.ConvertNewExpression(this.cachedComp, proc, converter, s, EnumDataType.E_STRING_OBJECT);
                    this.fileName.Add(path);
                }
            }

            if (proc.CurrentScope.Exists(this.writerName))
            {
                this.newWriter = proc.CurrentScope.GetVariable(this.writerName);
                if (this.newWriter.TypeExists(EnumDataType.E_WRITER))
                {
                    proc.CurrentScope.Update(this.writerName, "", this.newWriter.PrefixInfo(EnumDataType.E_WRITER).BelongsTo, false, EnumDataType.E_WRITER);
                    this.cachedComp.UpdateParameters(converter, proc, this.writerName, true);
                }
                else
                {
                    this.newWriter = proc.CurrentScope.Update(this.writerName, "", proc.Name, false, EnumDataType.E_WRITER);
                    IStructure st = converter.CreateNewField(converter.RootStructureInstance, this.newWriter, false);
                    converter.CurrentFunction.LocalVariables.Add(st);
                }
            }
            else
            {
                this.newWriter = proc.CurrentScope.Add(this.writerName, "", proc.Name, false, EnumDataType.E_WRITER);
                IStructure st = converter.CreateNewField(converter.RootStructureInstance, this.newWriter, false);
                converter.CurrentFunction.LocalVariables.Add(st);
            }
            converter.Convert(this);
        }

        #endregion

        #region ILanguageConverter Membres

        public void WriteInPerl(ICodeConverter converter)
        {
        }

        public void WriteInPython(ICodeConverter converter)
        {
        }

        public void WriteInUnixCPP(ICodeConverter converter)
        {
        }

        public void WriteInCSharp(ICodeConverter converter)
        {
        }

        public void WriteInJava(ICodeConverter converter)
        {
        }

        public void WriteInMacOSCPP(ICodeConverter converter)
        {
        }

        public void WriteInMicrosoftCPP(ICodeConverter converter)
        {
            string currentDir = "wstring(L\"" + FinalFile.BuildDirectory.Replace("\\", "\\\\\\\\") + "\")";
            this.directories.ForEach(new Action<string>(delegate(string dir)
            {
                converter.CurrentFunction.AddToSource("this->CreateDirectoryIfNotExists(" + currentDir + " + " + dir.Replace("\\", "\\\\\\\\") + ");" + Environment.NewLine);
                currentDir += " + " + dir.Replace("\\", "\\\\\\\\") + " + wstring(L\"" + MicrosoftCPPConverter.Escape(Path.DirectorySeparatorChar.ToString()) + "\")";
            }));
            string completeFileName = "";
            bool first = true;
            this.fileName.ForEach(new Action<string>(delegate(string s)
            {
                if (!first) completeFileName += " + wstring(L\".\") + "; else first = false;
                completeFileName += s.Replace("\\", "\\\\\\\\");
            }));
            converter.CurrentFunction.AddToSource("this->EraseFile(" + currentDir + " + " + completeFileName + ");" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("$[left:" + MicrosoftCPPConverter.Escape(this.newWriter.PrefixedName) + "] = writer(" + currentDir + " + " + completeFileName + ");" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("$[ifptr:" + MicrosoftCPPConverter.Escape(this.newWriter.PrefixedName) + "]Start();" + Environment.NewLine);
        }

        public void WriteInVBScript(ICodeConverter converter)
        {
            string currentDir = "\"" + FinalFile.BuildDirectory + "\"";
            this.directories.ForEach(new Action<string>(delegate(string dir)
            {
                converter.CurrentFunction.AddToSource("CreateDirectoryIfNotExists " + currentDir + " & " + dir + Environment.NewLine);
                currentDir += " & " + dir + " & \"" + Path.DirectorySeparatorChar + "\"";
            }));
            string completeFileName = "";
            bool first = true;
            this.fileName.ForEach(new Action<string>(delegate(string s)
            {
                if (!first) completeFileName += " & \".\" & "; else first = false;
                completeFileName += s;
            }));
            converter.CurrentFunction.AddToSource("EraseFile " + currentDir + " & " + completeFileName + Environment.NewLine);
            converter.CurrentFunction.AddToSource(this.newWriter.Name + " = Array(" + currentDir + " & " + completeFileName + ", 0, true)" + Environment.NewLine);
        }

        public void WriteInPowerShell(ICodeConverter converter)
        {
            string currentDir = "\"" + FinalFile.BuildDirectory + "\"";
            this.directories.ForEach(new Action<string>(delegate(string dir)
            {
                converter.CurrentFunction.AddToSource("CreateDirectoryIfNotExists (" + currentDir + " + " + dir + ")" + Environment.NewLine);
                currentDir += " + " + dir + " + \"" + PowerShellConverter.Escape(Path.DirectorySeparatorChar.ToString()) + "\"";
            }));
            string completeFileName = "";
            bool first = true;
            this.fileName.ForEach(new Action<string>(delegate(string s)
            {
                if (!first) completeFileName += " + \".\" + "; else first = false;
                completeFileName += s;
            }));
            converter.CurrentFunction.AddToSource("EraseFile (" + currentDir + " + " + completeFileName + ")" + Environment.NewLine);
            converter.CurrentFunction.AddToSource("S[left:" + PowerShellConverter.Escape(this.newWriter.Name) + "] = @((" + currentDir + " + " + completeFileName + ")" + ", 0, \"\", $true)" + Environment.NewLine);
        }

        #endregion
    }
}
