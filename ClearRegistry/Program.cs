using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.MSUtil;
using System.Text.RegularExpressions;
using System.IO;

namespace ClearRegistry
{
    class Program
    {
        static readonly string fileName = AppDomain.CurrentDomain.BaseDirectory + "output.txt";

        static RegistryKey GetRegistryKey(string name) {
            switch(name) {
                case "HKEY_LOCAL_MACHINE":
                    return Registry.LocalMachine;
                case "HKEY_CLASSES_ROOT":
                    return Registry.ClassesRoot;
                case "HKEY_CURRENT_CONFIG":
                    return Registry.CurrentConfig;
                case "HKCR":
                    return Registry.ClassesRoot;
                case "HKCU":
                    return Registry.CurrentUser;
                case "HKLM":
                    return Registry.LocalMachine;
                case "HKEY_USERS":
                    return Registry.Users;
            }
            return null;
        }

        static void IterateList(List<string> keys, bool suppress, out bool haveToDo, out bool keyNotFound)
        {
            using (FileStream fs = new FileStream(Program.fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    haveToDo = false;
                    keyNotFound = false;
                    for (int index = 0; index < keys.Count; ++index)
                    {
                        try
                        {
                            bool deleted = false;
                            bool notFound = false;
                            string keyStr = keys[index];
                            sw.WriteLine(keyStr);
                            Regex r = new Regex(@"^([A-Z_]+)\\(.*CLSID\\\{[0-9A-Fa-f\-]+\})$");
                            Match m = r.Match(keyStr);
                            if (m.Success)
                            {
                                RegistryKey hKey = Program.GetRegistryKey(m.Groups[1].Value);
                                RegistryKey subKey = hKey.OpenSubKey(m.Groups[2].Value);
                                if (subKey != null)
                                {
                                    deleted = true;
                                    sw.WriteLine("delete " + subKey.Name);
                                    subKey.Close();
                                    if (suppress)
                                        hKey.DeleteSubKeyTree(m.Groups[2].Value, false);
                                    else haveToDo = true;
                                }
                                else
                                {
                                    // clé non trouvée ?? => Regarder dans Wow6432Node
                                    subKey = hKey.OpenSubKey("Wow6432Node\\" + m.Groups[2].Value);
                                    if (subKey != null)
                                    {
                                        deleted = true;
                                        sw.WriteLine("delete " + subKey.Name);
                                        subKey.Close();
                                        // suppress other key from the list
                                        keys.Remove(m.Groups[1].Value + "\\Wow6432Node\\" + m.Groups[2].Value);
                                        if (suppress)
                                            hKey.DeleteSubKeyTree("Wow6432Node\\" + m.Groups[2].Value, false);
                                        else haveToDo = true;
                                    }
                                    else notFound = true;
                                }
                                hKey.Close();
                            }

                            if (!deleted)
                            {
                                r = new Regex(@"^([A-Z_]+)\\(.*Classes\\)(.*CLSID\\\{[0-9A-Fa-f\-]+\})$");
                                m = r.Match(keyStr);
                                if (m.Success)
                                {
                                    RegistryKey hKey = Program.GetRegistryKey(m.Groups[1].Value);
                                    RegistryKey subKey = hKey.OpenSubKey(m.Groups[2].Value + m.Groups[3].Value);
                                    if (subKey != null)
                                    {
                                        deleted = true;
                                        sw.WriteLine("delete " + subKey.Name);
                                        subKey.Close();
                                        if (suppress)
                                            hKey.DeleteSubKeyTree(m.Groups[2].Value + m.Groups[3].Value, false);
                                        else haveToDo = true;
                                    }
                                    else
                                    {
                                        // clé non trouvée ?? => Regarder dans Wow6432Node
                                        subKey = hKey.OpenSubKey(m.Groups[2].Value + "Wow6432Node\\" + m.Groups[3].Value);
                                        if (subKey != null)
                                        {
                                            deleted = true;
                                            sw.WriteLine("delete " + subKey.Name);
                                            subKey.Close();
                                            // suppress other key from the list
                                            keys.Remove(m.Groups[1].Value + "\\" + m.Groups[2].Value + "Wow6432Node\\" + m.Groups[3].Value);
                                            if (suppress)
                                                hKey.DeleteSubKeyTree(m.Groups[2].Value + "Wow6432Node\\" + m.Groups[3].Value, false);
                                            else haveToDo = true;
                                        }
                                        else notFound = true;
                                    }
                                    hKey.Close();
                                }
                            }

                            if (!deleted)
                            {
                                r = new Regex(@"^([A-Z_]+)\\(.*o2Mate\.[A-Za-z]+)$");
                                m = r.Match(keyStr);
                                if (m.Success)
                                {
                                    RegistryKey hKey = Program.GetRegistryKey(m.Groups[1].Value);
                                    RegistryKey subKey = hKey.OpenSubKey(m.Groups[2].Value);
                                    if (subKey != null)
                                    {
                                        deleted = true;
                                        sw.WriteLine("delete " + subKey.Name);
                                        subKey.Close();
                                        if (suppress)
                                            hKey.DeleteSubKeyTree(m.Groups[2].Value, false);
                                        else haveToDo = true;
                                    }
                                    else notFound = true;
                                    hKey.Close();
                                }
                            }

                            if (!deleted)
                            {
                                r = new Regex(@"^([A-Z_]+)\\(.*Record\\\{[0-9A-Fa-f\-]+\})$");
                                m = r.Match(keyStr);
                                if (m.Success)
                                {
                                    RegistryKey hKey = Program.GetRegistryKey(m.Groups[1].Value);
                                    RegistryKey subKey = hKey.OpenSubKey(m.Groups[2].Value);
                                    if (subKey != null)
                                    {
                                        deleted = true;
                                        sw.WriteLine("delete " + subKey.Name);
                                        subKey.Close();
                                        if (suppress)
                                            hKey.DeleteSubKeyTree(m.Groups[2].Value, false);
                                        else haveToDo = true;
                                    }
                                    else notFound = true;
                                    hKey.Close();
                                }
                            }

                            if (!deleted)
                            {
                                r = new Regex(@"^([A-Z_]+)\\(.*TypeLib\\\{[0-9A-Fa-f\-]+\})$");
                                m = r.Match(keyStr);
                                if (m.Success)
                                {
                                    RegistryKey hKey = Program.GetRegistryKey(m.Groups[1].Value);
                                    RegistryKey subKey = hKey.OpenSubKey(m.Groups[2].Value);
                                    if (subKey != null)
                                    {
                                        deleted = true;
                                        sw.WriteLine("delete " + subKey.Name);
                                        subKey.Close();
                                        if (suppress)
                                            hKey.DeleteSubKeyTree(m.Groups[2].Value, false);
                                        else haveToDo = true;
                                    }
                                    else notFound = true;
                                    hKey.Close();
                                }
                            }

                            if (notFound && !deleted)
                            {
                                sw.WriteLine("Clé '" + keyStr + "' non trouvée (et non supprimée)");
                                keyNotFound = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            sw.WriteLine("Exception occurred : " + ex.Message);
                        }
                    }
                }
            }
        }

        static void Print(string str)
        {
            using (FileStream fs = new FileStream(Program.fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Console.WriteLine(str);
                    sw.WriteLine(str);
                }
            }
        }

        static List<string> ScanRegistry()
        {
            List<string> keys = new List<string>();
            ILogRecordset rs = null;
            try
            {
                LogQueryClass logQ = new LogQueryClass();
                COMRegistryInputContextClass registryFormat = new COMRegistryInputContextClass();
                string query = @"SELECT Path FROM \ where Value LIKE 'o2Mate%'";
                rs = logQ.Execute(query, registryFormat);
                for (; !rs.atEnd(); rs.moveNext())
                {
                    keys.Add(rs.getRecord().toNativeString(","));
                }
            }
            finally
            {
                rs.close();
            }
            return keys;
        }

        static void Main(string[] args)
        {
            bool haveToDo = false;
            bool keyNotFound = false;
            bool suppress = false;
            if (args.Length >= 1)
                suppress = Convert.ToBoolean(args[0]);
            FileStream fs = new FileStream(Program.fileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            fs.Close();
            if (suppress)
                Program.Print("Suppress all obsolete registration of COM o2Mate keys");
            else
                Program.Print("Scanning registry to detect obsolete registration of COM o2Mate keys");

            List<string> keys = Program.ScanRegistry();

            do
            {
                if (!suppress)
                {
                    Program.IterateList(keys, false, out haveToDo, out keyNotFound);

                    if (keyNotFound)
                        Program.Print("At least one key had not found");


                    if (haveToDo)
                    {
                        Program.Print("Some keys have to be deleted. Do you want to delete these keys ? (say Yes or No)");
                        Program.Print("(You can open the file '" + AppDomain.CurrentDomain.BaseDirectory + "output.txt' to verify what keys will be deleted)");
                        string asked = Console.ReadLine();
                        if (asked.ToUpper().StartsWith("Y"))
                        {
                            Program.Print("Yes, so, doing suppress is working...waiting for the end...");

                            Program.IterateList(keys, true, out haveToDo, out keyNotFound);

                            // loop redo
                            haveToDo = true;
                            Program.Print("Rescan keys...");

                            keys = Program.ScanRegistry();
                        }
                        else
                        {
                            Program.Print("Operation canceled. Ending the program.");
                            break;
                        }
                    }
                    else
                    {
                        Program.Print("No keys have to be deleted. Everything is ok. Ending the program.");
                    }
                }
                else
                {
                    Program.IterateList(keys, true, out haveToDo, out keyNotFound);

                    Program.Print("Rescan keys...");

                    keys = Program.ScanRegistry();

                    // redo the same work if necessary
                    suppress = false;

                }
            } while (haveToDo);

            Console.WriteLine("Press any key to close the window...");
            Console.ReadKey(false);
        }
    }
}
