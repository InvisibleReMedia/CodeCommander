Dim argPath
Dim argFile
Dim fso
Dim loadStream
Dim textStream
argPath = WScript.Arguments.Named("Path")
argFile = WScript.Arguments.Named("File")
Set fso = CreateObject("Scripting.FileSystemObject")
Set textStream = fso.CreateTextFile(argPath & "\" & argFile & ".xml", True, True)
Set loadStream = fso.OpenTextFile(argPath & "\" & argFile)

Dim src
src = loadStream.ReadAll

startPos = InStr(src, "C:\Documents and Settings\Olivier\Mes documents\Visual Studio 2005\Projects\Projects\codecommander\CodeCommander\bin\Debug\Editeur\") - 1
endPos = startPos + Len("C:\Documents and Settings\Olivier\Mes documents\Visual Studio 2005\Projects\Projects\codecommander\CodeCommander\bin\Debug\Editeur\") + 1
src = Mid(src, 1, startPos) & Mid(src, endPos, len(src) - endPos + 1)
src = Replace(src, " ", Chr(183))
src = Replace(src, "&", "&amp;")
src = Replace(src, "<", "&lt;")
src = Replace(src, ">", "&gt;")
src = Replace(src, vbCrLf, "¶")
' deux caractères au lieu d'un seul modifie le startPos
startPos = InStr(src, "directory=""") + len("directory=""") - 1
loadStream.Close

Dim dest
dest = "<?xml version=""1.0"" standalone=""yes""?>"
dest = dest & "<root><infos/><code>"
dest = dest & "<texte>" & Mid(src, 1, startPos) & "</texte>"
dest = dest & "<variable>dir</variable>"
dest = dest & "<texte>" & Mid(src, startPos + 1, len(src) - startPos + 1)
dest = dest & "</texte></code></root>"

textStream.Write dest

textStream.Close

