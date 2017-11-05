Option Explicit
Const ForReading = 1
Dim countTools
Dim countGap
Dim depth
Dim directory
Dim legendesID
Dim stackLegendes()
Dim countStackLegendes
countStackLegendes = 0
countTools = 0
countGap = 0
depth = 0
directory="C:\Documents and Settings\Olivier\Mes documents\Visual Studio 2005\Projects\Projects\codecommander\CodeCommander\bin\Debug\Editeur\"
Dim currentCopied
ReDim stackLegendes(-1)
Dim countTabNames
Dim toolNames()

Sub addTool(name)
	ReDim preserve toolNames(countTabNames)
	toolNames(countTabNames) = name
	countTabNames = countTabNames + 1
End Sub

Sub InitNames()
	ReDim toolNames(-1)
	countTabNames = 0
End Sub

Function GetNames()
	Dim first
	Dim index
	Dim output
	first = true
	output = ""
	For index = LBound(toolNames) To UBound(toolNames)
		If Not first Then
			output = output & ","
		End If
		first = false
		output = output & toolNames(index)
	Next
	GetNames = output
End Function

Function CreatePaste(indent)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "divpaste" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\paste.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    comp.Compilation directory & "outils\outil_paste.xml", fileNameDict, fileNameFinal, Nothing
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreatePaste = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateGap()
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countGap = countGap + 1
    dict.AddString "Number", countGap
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
    fileNameFinal = sFolder & "\CodeCommander\temp\gap.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    comp.Compilation directory & "outils\outil_gap.xml", fileNameDict, fileNameFinal, Nothing
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateGap = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateIndent(indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\indent.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_indent.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_indent.xml", fileNameDict, fileNameFinal, Nothing
	End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateIndent = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateUnindent(indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\unindent.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_uindent.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_unindent.xml", fileNameDict, fileNameFinal, Nothing
	End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateUnindent = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateText(text, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Indent", indent
    Dim arr
    Set arr = CreateObject("o2Mate.Array")
    dict.AddArray "textes", arr
    Dim stream
    stream = split(text, Chr(13) & Chr(10))
    Dim first
    first = True
    Dim field
    Dim index
    For index = LBound(stream) To UBound(stream)
        If Not first Then
            Set field = CreateObject("o2Mate.Fields")
            field.AddString "isCrLf", "1"
            arr.Add field
            Set field = Nothing
        Else
            first = False
        End If
        Dim line
        line = stream(index)
        If line <> "" Then
            line = Replace(line, Chr(32), Chr(183)) 
            line = Replace(line, "&", "&amp;")
            line = Replace(line, "<", "&lt;")
            line = Replace(line, ">", "&gt;")
            Set field = CreateObject("o2Mate.Fields")
            field.AddString "value", line
            arr.Add field
            Set field = Nothing
        End If
    Next
    Set arr = Nothing
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
    fileNameFinal = sFolder & "\CodeCommander\temp\texte.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_texte.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_texte.xml", fileNameDict, fileNameFinal, Nothing
	End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateText = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateVariable(variableName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Indent", indent
    dict.AddString "VariableName", variableName
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
    fileNameFinal = sFolder & "\CodeCommander\temp\variable.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_variable.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_variable.xml", fileNameDict, fileNameFinal, Nothing
	End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateVariable = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateAffectation(varName, expression, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "VariableName", varName
    dict.AddString "ExpressionData", expression
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\affectation.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_affectation.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_affectation.xml", fileNameDict, fileNameFinal, Nothing
	End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateAffectation = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateAffectationChaine(varName, stringName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "VariableName", varName
    dict.AddString "StringName", stringName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\affectationChaine.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_affectationChaine.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_affectationChaine.xml", fileNameDict, fileNameFinal, Nothing
	End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateAffectationChaine = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateAffectationChamp(varName, tabName, expression, field, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "VariableName", varName
    dict.AddString "TableauData", tabName
    dict.AddString "ExpressionData", expression
    dict.AddString "ChampData", field
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\affectationChamp.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_affectationChamp.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_affectationChamp.xml", fileNameDict, fileNameFinal, Nothing
	End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateAffectationChamp = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateBeginProcess(processName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "ProcessName", processName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\beginProcess.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_beginProcess.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_beginProcess.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateBeginProcess = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateBeginSkeleton(skeletonName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "SkeletonName", skeletonName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\beginSkeleton.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_beginSkeleton.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_beginSkeleton.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateBeginSkeleton = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateCall(processName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "ProcessName", processName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\call.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_call.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_call.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateCall = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateInjector(injectorName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "InjectorName", injectorName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\injector.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_injector.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_injector.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateInjector = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateCallSkeleton(skeletonName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "SkeletonName", skeletonName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\callSkeleton.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_callSkeleton.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_callSkeleton.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateCallSkeleton = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateChamp(tabName, expression, fieldName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "TableauData", tabName
    dict.AddString "ExpressionData", expression
    dict.AddString "ChampData", fieldName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\champ.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_champ.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_champ.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateChamp = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateCoding(codingName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Indent", indent
    depth = depth + 1
    dict.AddString "Depth", depth
    dict.AddString "CodingName", codingName
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
    fileNameFinal = sFolder & "\CodeCommander\temp\coding.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_coding.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_coding.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateCoding = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateCondition(expression, labelTrue, labelFalse, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "ExpressionData", expression
    dict.AddString "LabelTrue", labelTrue
    dict.AddString "LabelFalse", labelFalse
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\condition.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_condition.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_condition.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateCondition = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateEndProcess(processName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "ProcessName", processName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\endProcess.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_endProcess.xml", fileNameDict, fileNameFinal, Nothing
	Else
	    comp.Compilation directory & "outils\outil_endProcess.xml", fileNameDict, fileNameFinal, Nothing
	End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateEndProcess = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateEndSkeleton(skeletonName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "SkeletonName", skeletonName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\endSkeleton.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_endSkeleton.xml", fileNameDict, fileNameFinal, Nothing
	Else
	    comp.Compilation directory & "outils\outil_endSkeleton.xml", fileNameDict, fileNameFinal, Nothing
	End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateEndSkeleton = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateHandler(handlerName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "HandlerName", handlerName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\handler.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_handler.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_handler.xml", fileNameDict, fileNameFinal, Nothing
	End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateHandler = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateLabel(labelName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "LabelName", labelName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\label.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_label.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_label.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateLabel = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateSize(varName, tabName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "VariableName", varName
    dict.AddString "TableauName", tabName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\size.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_size.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_size.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateSize = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateTemplate(path, params, xmlCode, myLegendes, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Path", path
    dict.AddString "Params", params
    dict.AddString "xml", xmlCode
    dict.AddString "Indent", indent
    Dim leg
    Dim arr
    Dim fields
    Set arr = CreateObject("o2Mate.Array")
    Dim index
    For index = 0 To (myLegendes.Count - 1)
		Set fields = CreateObject("o2Mate.Fields")
		Set leg = myLegendes.GetLegendeByIndex(index)
		fields.AddString "context", leg.Context
		fields.AddString "name", leg.Name
		fields.AddString "description", Replace(leg.Description, "'", "&#39;")
		fields.AddString "commentaire", Replace(leg.Commentaire, "'", "&#39;")
		fields.AddString "type", leg.Type
		fields.AddString "expression", leg.Expression
		If leg.Free Then
			fields.AddString "free", "true"
		Else
			fields.AddString "free", "false"
		End If
		arr.Add fields
		Set fields = Nothing
    Next
    dict.AddArray "legendes", arr
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
    fileNameFinal = sFolder & "\CodeCommander\temp\template.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_template.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_template.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateTemplate = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
    Set arr = Nothing
End Function

Function CreateMOP(language, name, params, xmlCode, myLegendes, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Language", language
    dict.AddString "Name", name
    dict.AddString "Params", params
    dict.AddString "xml", xmlCode
    dict.AddString "Indent", indent
    Dim leg
    Dim arr
    Dim fields
    Set arr = CreateObject("o2Mate.Array")
    Dim index
    For index = 0 To (myLegendes.Count - 1)
		Set fields = CreateObject("o2Mate.Fields")
		Set leg = myLegendes.GetLegendeByIndex(index)
		fields.AddString "context", leg.Context
		fields.AddString "name", leg.Name
		fields.AddString "description", Replace(leg.Description, "'", "&#39;")
		fields.AddString "commentaire", Replace(leg.Commentaire, "'", "&#39;")
		fields.AddString "type", leg.Type
		fields.AddString "expression", leg.Expression
		If leg.Free Then
			fields.AddString "free", "true"
		Else
			fields.AddString "free", "false"
		End If
		arr.Add fields
		Set fields = Nothing
    Next
    dict.AddArray "legendes", arr
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
    fileNameFinal = sFolder & "\CodeCommander\temp\createmop.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_createmop.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_createmop.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateMOP = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
    Set arr = Nothing
End Function

Function CreateUseTemplate(path, params, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Path", path
    dict.AddString "Params", params
    dict.AddString "Indent", indent
    depth = depth + 1
    dict.AddString "Depth", depth
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
    fileNameFinal = sFolder & "\CodeCommander\temp\useTemplate.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_useTemplate.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_useTemplate.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateUseTemplate = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateUseMOP(language, command, expression, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Language", language
    dict.AddString "Command", command
    dict.AddString "Expression", expression
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\useMop.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_usemop.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_usemop.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateUseMOP = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateWriter(name, file, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "WriterName", name
    dict.AddString "FileName", file
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\createWriter.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_createWriter.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_createWriter.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateWriter = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateUseWriter(varName, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "WriterName", varName
    dict.AddString "Indent", indent
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
    fileNameFinal = sFolder & "\CodeCommander\temp\defaultWriter.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_defaultWriter.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_defaultWriter.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateUseWriter = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateZoneSelection(toolName)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    dict.AddString "toolName", toolName
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
    fileNameFinal = sFolder & "\CodeCommander\temp\selection.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    comp.Compilation directory & "outils\outil_selection.xml", fileNameDict, fileNameFinal, Nothing
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateZoneSelection = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreatePopup(name)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    dict.AddString "Depth", getCountPopup()
    dict.AddString "TemplateName", name
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
    fileNameFinal = sFolder & "\CodeCommander\temp\popup.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    comp.Compilation directory & "outils\outil_popup.xml", fileNameDict, fileNameFinal, Nothing
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreatePopup = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateLegende(myLegendes)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    dict.AddString "Number", countTools
    Dim leg
    Dim arr
    Dim fields
    Set arr = CreateObject("o2Mate.Array")
    Dim index
    For index = 0 To (myLegendes.Count - 1)
		Set fields = CreateObject("o2Mate.Fields")
		Set leg = myLegendes.GetLegendeByIndex(index)
		fields.AddString "context", leg.Context
		fields.AddString "name", leg.Name
		fields.AddString "description", Replace(leg.Description, "'", "&#39;")
		fields.AddString "commentaire", Replace(leg.Commentaire, "'", "&#39;")
		fields.AddString "type", leg.Type
		fields.AddString "expression", leg.Expression
		If leg.Free Then
			fields.AddString "free", "true"
		Else
			fields.AddString "free", "false"
		End If
		arr.Add fields
		Set fields = Nothing
    Next
    dict.AddArray "legendes", arr
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
    fileNameFinal = sFolder & "\CodeCommander\temp\legende.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    comp.Compilation directory & "outils\outil_legende.xml", fileNameDict, fileNameFinal, Nothing
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    ' le fichier peut être vide !
    If Not txtStream.AtEndOfStream Then
	    CreateLegende = txtStream.ReadAll
	End If
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
    Set arr = Nothing
End Function

Function CreateParallel(indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Indent", indent
    depth = depth + 1
    dict.AddString "Depth", depth
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
    fileNameFinal = sFolder & "\CodeCommander\temp\parallel.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_parallel.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_parallel.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateParallel = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
End Function

Function CreateSyntax(name, xmlCode, myLegendes, indent, readonly)
    Dim dict
    Set dict = CreateObject("o2Mate.Dictionnaire")
    countTools = countTools + 1
    addTool "table" & countTools
    dict.AddString "Number", countTools
    dict.AddString "Name", name
    dict.AddString "xml", xmlCode
    dict.AddString "Indent", indent
    Dim leg
    Dim arr
    Dim fields
    Set arr = CreateObject("o2Mate.Array")
    Dim index
    For index = 0 To (myLegendes.Count - 1)
		Set fields = CreateObject("o2Mate.Fields")
		Set leg = myLegendes.GetLegendeByIndex(index)
		fields.AddString "context", leg.Context
		fields.AddString "name", leg.Name
		fields.AddString "description", Replace(leg.Description, "'", "&#39;")
		fields.AddString "commentaire", Replace(leg.Commentaire, "'", "&#39;")
		fields.AddString "type", leg.Type
		fields.AddString "expression", leg.Expression
		If leg.Free Then
			fields.AddString "free", "true"
		Else
			fields.AddString "free", "false"
		End If
		arr.Add fields
		Set fields = Nothing
    Next
    dict.AddArray "legendes", arr
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
    fileNameFinal = sFolder & "\CodeCommander\temp\syntax.htm"
    Dim comp
    Set comp = CreateObject("o2Mate.Compilateur")
    If readonly Then
	    comp.Compilation directory & "outils\outil_readonly_syntax.xml", fileNameDict, fileNameFinal, Nothing
    Else
	    comp.Compilation directory & "outils\outil_syntax.xml", fileNameDict, fileNameFinal, Nothing
    End If
    Set comp = Nothing
    ' écriture du fichier final dans le source
    Dim txtStream
    Set txtStream = fso.OpenTextFile(fileNameFinal, ForReading, False, -2)
    CreateSyntax = txtStream.ReadAll
    txtStream.Close
    Set txtStream = Nothing
    Set dict = Nothing
    Set fso = Nothing
    Set arr = Nothing
End Function

Sub pushLegendes(newLegendesID)
	countStackLegendes = countStackLegendes + 1
	ReDim Preserve stackLegendes(countStackLegendes)
	stackLegendes(countStackLegendes - 1) = legendesID
	legendesID = newLegendesID
End Sub

Sub popLegendes()
	countStackLegendes = countStackLegendes - 1
	legendesID = stackLegendes(countStackLegendes)
	If countStackLegendes = 0 Then
		ReDim stackLegendes(-1)
	Else
		ReDim Preserve stackLegendes(countStackLegendes)
	End If
End Sub

Sub LoadMasterLegendes(obj)
	Dim ulLegendes
	Set ulLegendes = document.getElementById("legendes0")
	ulLegendes.innerHTML = CreateLegende(obj)
	legendesID = "legendes0"
	countStackLegendes = 0
	ReDim stackLegendes(-1)
End Sub

Function getLegende(context, name)
	Dim ulLegendes
	Dim liLegende
	Set ulLegendes = document.getElementById(legendesID)
	For Each liLegende in ulLegendes.childNodes
		If liLegende.name = name And liLegende.context = context Then
			Set getLegende = liLegende
			Exit Function
		End If
	Next
	Set getLegende = Nothing
End Function

Sub addLegende(context, name, desc, com, myType, expr, free)
	Dim searchLegende
	Set searchLegende = getLegende(context, name)
	If Not searchLegende Is Nothing Then
		searchLegende.description = desc
		searchLegende.commentaire = com
		searchLegende.typeLegende = myType
		searchLegende.expression = expr
		If free Then
			searchLegende.free = "true"
		Else
			searchLegende.free = "false"
		End If
	Else
		Dim ulLegendes
		Set ulLegendes = document.getElementById(legendesID)
		Dim liLegende
		Set liLegende = document.createElement("li")
		liLegende.setAttribute "context", context
		liLegende.setAttribute "name", name
		liLegende.setAttribute "description", desc
		liLegende.setAttribute "commentaire", com
		liLegende.setAttribute "typeLegende", myType
		liLegende.setAttribute "expression", expr
		If free Then
			liLegende.setAttribute "free", "true"
		Else
			liLegende.setAttribute "free", "false"
		End If
		ulLegendes.AppendChild liLegende
	End If
End Sub

Function getOption(obj, value)
	Dim opt
	For Each opt in obj.options
		If opt.value = value Then
			Set getOption = opt
		End If
	Next
End Function

Function SetLegendeIHM(context, name, forcedType)
	Dim legende
	Dim elem
	Set elem = document.getElementById("legendeContext")
	elem.innerText = context
	Set elem = document.getElementById("legendeName")
	elem.innerText = name
	Set legende = getLegende(context, name)
	If Not legende Is Nothing Then
		Set elem = document.getElementById("legendeDescription")
		elem.value = legende.description
		Set elem = document.getElementById("legendeCommentaire")
		elem.value = legende.commentaire
		Set elem = document.getElementById("legendeType")
		If forcedType = "Tableau" Then
			getOption(elem, "Array").disabled = False
			getOption(elem, "Array").selected = True
			elem.disabled = True
			Set elem = document.getElementById("legendeTrFree")
			elem.style.display = "block"
		Else
			elem.disabled = False
			getOption(elem, "Array").disabled = True
			Select Case legende.typeLegende
				Case "Number":
					getOption(elem, "Number").selected = True
				Case "String":
					getOption(elem, "String").selected = True
				Case "Date":
					getOption(elem, "Date").selected = True
				Case "Enumeration":
					getOption(elem, "Enumeration").selected = True
			End Select
			Set elem = document.getElementById("legendeTrFree")
			elem.style.display = "none"
		End If
		Set elem = document.getElementById("legendeExpression")
		elem.value = legende.expression
		Set elem = document.getElementById("legendeFree")
		elem.checked = legende.free
	Else
		Set elem = document.getElementById("legendeDescription")
		elem.value = ""
		Set elem = document.getElementById("legendeCommentaire")
		elem.value = ""
		Set elem = document.getElementById("legendeType")
		If forcedType = "Tableau" Then
			getOption(elem, "Array").disabled = False
			getOption(elem, "Array").selected = True
			elem.disabled = True
			Set elem = document.getElementById("legendeTrFree")
			elem.style.display = "block"
		Else
			getOption(elem, "Array").disabled = True
			elem.options(0).selected = True
			elem.disabled = False
			Set elem = document.getElementById("legendeTrFree")
			elem.style.display = "none"
		End If
		Set elem = document.getElementById("legendeExpression")
		elem.value = ""
		Set elem = document.getElementById("legendeFree")
		elem.checked = "true"
	End If
End Function

Function GetLegendeIHM()
	Dim context
	Dim name
	Dim desc
	Dim com
	Dim elem
	Dim myType
	Dim expr
	Dim free
	context = document.getElementById("legendeContext").innerText
	name = document.getElementById("legendeName").innerText
	desc = document.getElementById("legendeDescription").value
	com = document.getElementById("legendeCommentaire").value
	Set elem = document.getElementById("legendeType")
	myType = elem.options(elem.selectedIndex).value
	expr = document.getElementById("legendeExpression").value
	free = document.getElementById("legendeFree").checked
	addLegende context, name, desc, com, myType, expr, free
	setDirty
End Function

Sub RenameLegende(oldContext, oldName, newContext, newName)
	Dim legende
	Set legende = getLegende(oldContext, oldName)
	If Not legende Is Nothing Then
		legende.context = newContext
		legende.name = newName
	End If
End Sub