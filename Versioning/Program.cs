using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versioning
{
    class Program
    {
        private static void RecursiveCopy(DirectoryInfo src, DirectoryInfo dest)
        {
            foreach (FileInfo fi in src.GetFiles())
            {
                if (fi.Extension != ".dll" && fi.Extension != ".exe" && fi.Extension != ".pdb")
                {
                    File.Copy(fi.FullName, Path.Combine(dest.FullName, fi.Name), true);
                }
            }
            foreach (DirectoryInfo di in src.GetDirectories())
            {
                if (di.Name != "bin" && di.Name != "obj" && di.Name != "binaries")
                {
                    DirectoryInfo newDi = new DirectoryInfo(Path.Combine(dest.FullName, di.Name));
                    if (!newDi.Exists)
                    {
                        newDi.Create();
                    }
                    Program.RecursiveCopy(di, newDi);
                }
            }
        }

        static void Main(string[] args)
        {

            try {

                DirectoryInfo diSrc = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "versions\\");
                DirectoryInfo diDest = new DirectoryInfo(CodeCommander.Documents.HostVersions);
                Program.RecursiveCopy(diSrc, diDest);

                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "ClearRegistry", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "CodeCommander", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "Converters", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "Documents", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "NotifyProgress", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "o2MateCompil", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "o2MateDict", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "o2MateExpression", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "o2MateLegende", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "o2MateLocale", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "o2MateScope", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "o2MateVersion", "")).Commit();
                (new o2Mate.FetchedVersion(Environment.CurrentDirectory, "UniqueNames", "")).Commit();

                o2Mate.IVersionUpdater f = new o2Mate.FetchedVersion(Environment.CurrentDirectory);

                // met à jour les fichiers avec le numéro de version en cours
                f.UpdateAssemblyVersion();

                Console.WriteLine("Launching software " + CodeCommander.Documents.ProgramName + "...");
                string path = Path.Combine(Environment.CurrentDirectory + "\\", CodeCommander.Documents.ProgramName + "\\", "bin\\x64\\Debug\\", CodeCommander.Documents.ProgramName + ".exe");
                Console.WriteLine(path);
                Process.Start(path);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press any key to close the window...");
            Console.ReadKey();
        }
    }
}
