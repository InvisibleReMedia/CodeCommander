Option Explicit
Const ForReading = 1
Dim countTools
Dim directory
countTools = 0
directory="C:\Documents and Settings\Olivier\Mes documents\Visual Studio 2005\Projects\Projects\codecommander\CodeCommander\bin\Debug\Editeur\"

Function CreateLocaleTab(dict)
    Dim fso
    Set fso = CreateObject("Scripting.FileSystemObject")
    Dim ws
    Set ws = CreateObject("WScript.Shell")
    ' creer un fichier temporaire
  	Dim objFolder, sFolder
	Set objFolder = ws.SpecialFolders
	sFolder = objFolder("mydocuments")
	Set objFolder = Nothing
	Set ws = Nothing
	Dim fileNameDict
	fileNameDict = sFolder & "\CodeCommander\temp\dict.xml"
    dict.Save fileNameDict
    Dim fileNameFinal
    fileNameFinal = sFolder & "\CodeCommander\temp\locale.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
	comp.Compilation directory & "outils\locale.xml", fileNameDict, fileNameFinal, Nothing
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateLocaleTab = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set fso = Nothing
End Function

