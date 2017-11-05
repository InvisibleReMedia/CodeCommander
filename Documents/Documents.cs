using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCommander
{
    /// <summary>
    /// All documents files and directory references
    /// for the project
    /// </summary>
    public static class Documents
    {
        /// <summary>
        /// Program Name
        /// </summary>
        public static string ProgramName
        {
            get { return "CodeCommander"; }
        }

        /// <summary>
        /// Author
        /// </summary>
        public static string Author
        {
            get { return "Skercrow"; }
        }

        /// <summary>
        /// Directory versions
        /// </summary>
        public static string HostVersions
        {
            get { return Documents.CodeCommanderDirectory + "versions\\"; }
        }

        /// <summary>
        /// Directory which contains tools to displaying html object
        /// </summary>
        public static string OutilsDirectory
        {
            get { return Documents.EditeurDirectory + "outils\\"; }
        }

        /// <summary>
        /// Tool to search a template
        /// </summary>
        public static string OutilSearchTemplate
        {
            get { return Documents.OutilsDirectory + "outil_search_template.xml"; }
        }

        /// <summary>
        /// Tool to search a syntax
        /// </summary>
        public static string OutilSearchSyntax
        {
            get { return Documents.OutilsDirectory + "outil_search_syntax.xml"; }
        }

        /// <summary>
        /// Tool to cut and paste data
        /// </summary>
        public static string OutilCutter
        {
            get { return Documents.OutilsDirectory + "outil_pastestore.xml"; }
        }

        /// <summary>
        /// File Name which store results
        /// </summary>
        public static string SearchResult
        {
            get { return Documents.TempDirectory + "searchResult.txt"; }
        }

        /// <summary>
        /// Source VBScript file (requires to set the application directory)
        /// </summary>
        public static string OutilsVBSXML
        {
            get { return Documents.EditeurDirectory + "outils.vbs.xml"; }
        }

        /// <summary>
        /// Source Html file (requires to set the application directory)
        /// </summary>
        public static string EditorPageXML
        {
            get { return Documents.EditeurDirectory + "editeur.xml"; }
        }

        /// <summary>
        /// VBScript file (created on the fly)
        /// </summary>
        public static string OutilsVBS
        {
            get { return Documents.EditeurDirectory + "outils.vbs"; }
        }

        /// <summary>
        /// Source VBScript file (requires to set the application directory)
        /// </summary>
        public static string LocalesVBSXML
        {
            get { return Documents.EditeurDirectory + "locales.vbs.xml"; }
        }

        /// <summary>
        /// VBScript file (created on the fly)
        /// </summary>
        public static string LocalesVBS
        {
            get { return Documents.EditeurDirectory + "locales.vbs"; }
        }

        /// <summary>
        /// File containing Templates for displaying the dictionary related pages
        /// </summary>
        public static string DictionaryTemplate
        {
            get { return Documents.TempDirectory + "dictTemplates.xml"; }
        }

        /// <summary>
        /// Dictionary for the syntax tool
        /// </summary>
        public static string DictionarySyntax
        {
            get { return Documents.TempDirectory + "dictSyntax.xml"; }
        }

        /// <summary>
        /// Html page to display locale words list
        /// </summary>
        public static string LocalizationPage
        {
            get { return Documents.EditeurDirectory + "locales.htm"; }
        }

        /// <summary>
        /// Html page for the principal window
        /// </summary>
        public static string EditorPage
        {
            get { return Documents.EditeurDirectory + "editeur.htm"; }
        }

        /// <summary>
        /// File needed for displaying the objects of the current project
        /// </summary>
        public static string ProjectSourceCode
        {
            get { return Documents.EditeurDirectory + "project.xml"; }
        }

        /// <summary>
        /// Html page to display the objects of the current project
        /// </summary>
        public static string ProjectPage
        {
            get { return Documents.TempDirectory + "project.htm";  }
        }

        /// <summary>
        /// Html page to display read only documents
        /// </summary>
        public static string ReadOnlyViewPage
        {
            get { return Documents.EditeurDirectory + "viewTemplate.htm"; }
        }

        /// <summary>
        /// First Html page for the dictionary creation
        /// </summary>
        public static string BienvenuePage
        {
            get { return Documents.GeneratedDictionariesDirectory + "bienvenue.xml"; }
        }

        /// <summary>
        /// Last Html page for the dictionary creation (you just choice the execution)
        /// </summary>
        public static string ExecutePage
        {
            get { return Documents.GeneratedDictionariesDirectory + "execute.xml"; }
        }

        /// <summary>
        /// File for displaying free fields (dictionary)
        /// </summary>
        public static string SaisieLibrePage
        {
            get { return Documents.GeneratedDictionariesDirectory + "saisieLibre.xml"; }
        }

        /// <summary>
        /// File for displaying array (dictionary)
        /// </summary>
        public static string SaisieTableauPage
        {
            get { return Documents.GeneratedDictionariesDirectory + "saisieTableau.xml"; }
        }

        /// <summary>
        /// File for displaying array's fields (dictionary)
        /// </summary>
        public static string SaisieChampsPage
        {
            get { return Documents.GeneratedDictionariesDirectory + "saisieChamps.xml"; }
        }

        /// <summary>
        /// Html page to display Help (videos)
        /// </summary>
        public static string VideoPage
        {
            get { return Documents.EditeurDirectory + "videoHotspot.htm"; }
        }

        /// <summary>
        /// Image
        /// </summary>
        public static string ImageTabUp
        {
            get { return "\\tabUp.png"; }
        }

        /// <summary>
        /// Image
        /// </summary>
        public static string ImageTabDown
        {
            get { return "\\tabDown.png"; } 
        }

        /// <summary>
        /// Image
        /// </summary>
        public static string ImageEdit
        {
            get { return "\\lsc_edit.png"; }
        }

        /// <summary>
        /// Image
        /// </summary>
        public static string ImageAdd
        {
            get { return "\\Button-Add-Icon.png"; } 
        }

        /// <summary>
        /// Directory containing dictionary pages data
        /// </summary>
        public static string GeneratedDictionariesDirectory
        {
            get { return Documents.EditeurDirectory + "dicts\\"; }
        }

        /// <summary>
        /// Directory containing templates for dictionary pages data
        /// </summary>
        public static string GeneratedDictionariesTemplatesDirectory
        {
            get { return Documents.GeneratedDictionariesDirectory + "templates\\"; }
        }

        /// <summary>
        /// Directory which resides the editor page and so on
        /// </summary>
        public static string EditeurDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "Editeur\\"; }
        }

        /// <summary>
        /// Directory containing some files for specific language conversion
        /// </summary>
        public static string LanguageConvertersDirectory(string language)
        {
            return AppDomain.CurrentDomain.BaseDirectory + language + "\\";
        }

        /// <summary>
        /// Directory containing your templates
        /// </summary>
        public static string TemplatesDirectory
        {
            get { return Documents.CodeCommanderDirectory + "templates\\"; }
        }

        /// <summary>
        /// Directory containing standard templates
        /// </summary>
        public static string SrcTemplatesDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "templates\\"; }
        }

        /// <summary>
        /// Directory of your files
        /// </summary>
        public static string CodeCommanderDirectory
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CodeCommander\\"; }
        }

        /// <summary>
        /// Directory containing your dictionaries
        /// </summary>
        public static string DictionariesDirectory
        {
            get { return Documents.CodeCommanderDirectory + "dicts\\"; }
        }

        /// <summary>
        /// Directory containing standard examples
        /// </summary>
        public static string ExamplesDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "sources\\"; }
        }

        /// <summary>
        /// Directory containing your sources
        /// </summary>
        public static string SourcesDirectory
        {
            get { return Documents.CodeCommanderDirectory + "sources\\"; }
        }

        /// <summary>
        /// Directory containing your skeletons
        /// </summary>
        public static string SkeletonsDirectory
        {
            get { return Documents.CodeCommanderDirectory + "skeletons\\"; }
        }

        /// <summary>
        /// Directory containing standard skeletons
        /// </summary>
        public static string SrcSkeletonsDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "skeletons\\"; }
        }

        /// <summary>
        /// Directory containing your syntax
        /// </summary>
        public static string SyntaxDirectory
        {
            get { return Documents.CodeCommanderDirectory + "syntax\\"; }
        }

        /// <summary>
        /// Directory containing standard syntax
        /// </summary>
        public static string SrcSyntaxDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "syntax\\"; }
        }

        /// <summary>
        /// Directory containing your build
        /// </summary>
        public static string BuildDirectory
        {
            get { return Documents.CodeCommanderDirectory + "files\\"; }
        }

        /// <summary>
        /// Directory containing temporary files
        /// It's cleaned before quit
        /// </summary>
        public static string TempDirectory
        {
            get { return Documents.CodeCommanderDirectory + "temp\\"; }
        }

        /// <summary>
        /// Directory containing your localization files
        /// </summary>
        public static string LocalesDirectory
        {
            get { return Documents.CodeCommanderDirectory + "locales\\"; }
        }

        /// <summary>
        /// Directory containing standard localization files
        /// </summary>
        public static string SrcLocalesDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "locales\\"; }
        }

        /// <summary>
        /// File name of a temporary dictionary
        /// </summary>
        public static string TempDictFile
        {
            get { return Documents.TempDirectory + "dict.xml"; }
        }

        /// <summary>
        /// File name where redirect something
        /// </summary>
        public static string UnusedFile
        {
            get { return Documents.TempDirectory + "result.tmp";  }
        }
    }
}
