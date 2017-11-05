using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace o2Mate
{
    static class LocaleSystem
    {
        #region Public Static Constants
        public static readonly string LocalesDirectory = CodeCommander.Documents.LocalesDirectory;
        #endregion

        #region Private Static Methods
        private static DirectoryInfo GetDirectory(string dir)
        {
            return new DirectoryInfo(LocaleSystem.GetDirectoryString(dir));
        }

        private static string GetDirectoryString(string dir)
        {
            if (!String.IsNullOrEmpty(dir))
                return dir;
            else
                return LocaleSystem.LocalesDirectory;
        }
        #endregion

        #region Public Static Methods
        public static bool ValidateGroupName(string name)
        {
            char[] chars = Path.GetInvalidFileNameChars();
            foreach (char c in chars)
            {
                if (name.IndexOf(c) != -1)
                {
                    throw new ArgumentException("Le nom du groupe contient des caractères invalides");
                }
            }
            if (name.IndexOf(' ') != -1)
            {
                throw new ArgumentException("Le nom du groupe ne doit pas contenir d'espaces");
            }
            return true;
        }

        public static void CreateGroup(string from, string name)
        {
            if (LocaleSystem.ValidateGroupName(name))
            {
                DirectoryInfo dir = LocaleSystem.GetDirectory(from);
                if (!dir.Exists)
                {
                    dir.Create();
                }
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + name + ".tsv");
                if (!fi.Exists)
                {
                    fi.Create().Close();
                }
            }
        }

        public static void RenameGroup(string from, string name, string newName)
        {
            if (LocaleSystem.ValidateGroupName(name) && LocaleSystem.ValidateGroupName(newName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + name + ".tsv");
                if (fi.Exists)
                {
                    fi.MoveTo(LocaleSystem.GetDirectoryString(from) + newName + ".tsv");
                }
            }
        }

        public static void DeleteGroup(string from, string name)
        {
            if (LocaleSystem.ValidateGroupName(name))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + name + ".tsv");
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }
        }

        public static bool ExistsGroup(string from, string name)
        {
            if (LocaleSystem.ValidateGroupName(name))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + name + ".tsv");
                return fi.Exists;
            }
            return false;
        }

        public static ILocaleSet GetSet(string from, string name)
        {
            if (!ExistsGroup(from, name))
            {
                CreateGroup(from, name);
            }
            return new LocaleSet(from, name);
        }

        public static List<string> GetGroups(string from)
        {
            List<string> list = new List<string>();
            DirectoryInfo dir = LocaleSystem.GetDirectory(from);
            if (dir.Exists)
            {
                foreach (FileInfo fi in dir.GetFiles("*.tsv"))
                {
                    list.Add(Path.GetFileNameWithoutExtension(fi.Name));
                }
            }
            return list;
        }

        public static void InsertLocale(string from, string groupName, string name, string language, string value)
        {
            if (!LocaleSystem.ExistsGroup(from, groupName))
            {
                LocaleSystem.CreateGroup(from, groupName);
            }
            LocaleElement newLe = new LocaleElement(name, language, value);
            bool done = false;
            FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
            List<LocaleElement> list = LocaleSystem.GetAll(from, groupName);
            using (StreamWriter sw = new StreamWriter(fi.Open(FileMode.Truncate, FileAccess.Write, FileShare.Write)))
            {
                foreach (LocaleElement le in list)
                {
                    if (le.Name == name && le.Language == language)
                    {
                        done = true;
                    }
                    le.WriteToFile(sw);
                }
                if (!done)
                {
                    newLe.WriteToFile(sw);
                }
            }
        }

        public static void ModifyLocale(string from, string groupName, string name, string language, string value)
        {
            if (!LocaleSystem.ExistsGroup(from, groupName))
            {
                LocaleSystem.CreateGroup(from, groupName);
            }
            LocaleElement newLe = new LocaleElement(name, language, value);
            FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
            List<LocaleElement> list = LocaleSystem.GetAll(from, groupName);
            using (StreamWriter sw = new StreamWriter(fi.Open(FileMode.Truncate, FileAccess.Write, FileShare.Write)))
            {
                foreach (LocaleElement le in list)
                {
                    if (le.Name == name && le.Language == language)
                    {
                        le.Value = value;
                    }
                    le.WriteToFile(sw);
                }
            }
        }

        public static void RenameLocale(string from, string oldName, string groupName, string name, string language)
        {
            if (LocaleSystem.ExistsGroup(from, groupName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
                List<LocaleElement> list = LocaleSystem.GetAll(from, groupName);
                using (StreamWriter sw = new StreamWriter(fi.Open(FileMode.Truncate, FileAccess.Write, FileShare.Write)))
                {
                    foreach (LocaleElement le in list)
                    {
                        if (le.Name == oldName && le.Language == language)
                        {
                            le.Name = name;
                        }
                        le.WriteToFile(sw);
                    }
                }
            }
        }

        public static void RenameLocale(string from, string oldName, string groupName, string name)
        {
            if (LocaleSystem.ExistsGroup(from, groupName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
                List<LocaleElement> list = LocaleSystem.GetAll(from, groupName);
                using (StreamWriter sw = new StreamWriter(fi.Open(FileMode.Truncate, FileAccess.Write, FileShare.Write)))
                {
                    foreach (LocaleElement le in list)
                    {
                        if (le.Name == oldName)
                        {
                            le.Name = name;
                        }
                        le.WriteToFile(sw);
                    }
                }
            }
        }

        public static void RemoveLocale(string from, string groupName, string name, string language)
        {
            if (LocaleSystem.ExistsGroup(from, groupName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
                List<LocaleElement> list = LocaleSystem.GetAll(from, groupName);
                using (StreamWriter sw = new StreamWriter(fi.Open(FileMode.Truncate, FileAccess.Write, FileShare.Write)))
                {
                    foreach (LocaleElement le in list)
                    {
                        if (!(le.Name == name && le.Language == language))
                        {
                            le.WriteToFile(sw);
                        }
                    }
                }
            }
        }

        public static void RemoveLocale(string from, string groupName, string name)
        {
            if (LocaleSystem.ExistsGroup(from, groupName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
                List<LocaleElement> list = LocaleSystem.GetAll(from, groupName);
                using (StreamWriter sw = new StreamWriter(fi.Open(FileMode.Truncate, FileAccess.Write, FileShare.Write)))
                {
                    foreach (LocaleElement le in list)
                    {
                        if (!(le.Name == name))
                        {
                            le.WriteToFile(sw);
                        }
                    }
                }
            }
        }

        public static bool ExistLocale(string from, string groupName, string name, string language)
        {
            if (LocaleSystem.ExistsGroup(from, groupName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
                using (StreamReader sr = new StreamReader(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    int next = 0;
                    while (!sr.EndOfStream)
                    {
                        LocaleElement le = LocaleElement.Parse(sr, ref next);
                        if (le.Name == name && le.Language == language)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool ExistLocale(string from, string groupName, string name)
        {
            if (LocaleSystem.ExistsGroup(from, groupName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
                using (StreamReader sr = new StreamReader(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    int next = 0;
                    while (!sr.EndOfStream)
                    {
                        LocaleElement le = LocaleElement.Parse(sr, ref next);
                        if (le.Name == name)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static LocaleElement GetLocale(string from, string groupName, string name, string language)
        {
            if (LocaleSystem.ExistsGroup(from, groupName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
                using (StreamReader sr = new StreamReader(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    int next = 0;
                    while (!sr.EndOfStream)
                    {
                        LocaleElement le = LocaleElement.Parse(sr, ref next);
                        if (le.Name == name && le.Language == language)
                        {
                            return le;
                        }
                    }
                }
            }
            return null;
        }

        public static List<LocaleElement> GetLocale(string from, string groupName, string name)
        {
            List<LocaleElement> list = new List<LocaleElement>();
            if (LocaleSystem.ExistsGroup(from, groupName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
                using (StreamReader sr = new StreamReader(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    int next = 0;
                    while (!sr.EndOfStream)
                    {
                        LocaleElement le = LocaleElement.Parse(sr, ref next);
                        if (le.Name == name)
                        {
                            list.Add(le);
                        }
                    }
                }
            }
            return list;
        }

        public static List<LocaleElement> GetAll(string from, string groupName)
        {
            List<LocaleElement> list = new List<LocaleElement>();
            if (LocaleSystem.ExistsGroup(from, groupName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
                using (StreamReader sr = new StreamReader(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    int next = 0;
                    while (!sr.EndOfStream)
                    {
                        LocaleElement le = LocaleElement.Parse(sr, ref next);
                        list.Add(le);
                    }
                }
            }
            return list;
        }

        public static List<LocaleElement> GetNames(string from, string groupName)
        {
            List<LocaleElement> list = new List<LocaleElement>();
            if (LocaleSystem.ExistsGroup(from, groupName))
            {
                FileInfo fi = new FileInfo(LocaleSystem.GetDirectoryString(from) + groupName + ".tsv");
                using (StreamReader sr = new StreamReader(fi.Open(FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    int next = 0;
                    while (!sr.EndOfStream)
                    {
                        LocaleElement le = LocaleElement.Parse(sr, ref next);
                        if (!list.Exists(new Predicate<LocaleElement>(delegate(LocaleElement loc) { return loc.Name == le.Name; })))
                        {
                            list.Add(le);
                        }
                    }
                }
            }
            return list;
        }
        #endregion
    }
}
