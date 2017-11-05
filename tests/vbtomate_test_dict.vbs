Dim obj
Set obj = CreateObject("o2Mate.Dictionnaire")
msgBox obj.IsString("test")
obj.AddString "test", "testValue"
msgBox obj.IsString("test")
msgBox obj.IsArray("test")
MsgBox obj.GetString("test")
Dim arr
Set arr = CreateObject("o2Mate.Array")
Dim f
Set f = CreateObject("o2Mate.Fields")
f.AddString "test2", "testValue2"
f.AddString "test3", "testValue3"
MsgBox f.Exists("test2")
MsgBox f.GetString("test2")
arr.add f
obj.AddArray "array1", arr
obj.Save "C:\Documents and Settings\Olivier\Mes documents\Visual Studio 2005\Projects\Projects\o2Mate\tests\vbtomate_dict.xml"

Dim obj2
Set obj2 = CreateObject("o2Mate.Dictionnaire")
obj2.Load "C:\Documents and Settings\Olivier\Mes documents\Visual Studio 2005\Projects\Projects\o2Mate\tests\vbtomate_dict.xml"

msgBox obj2.IsString("test")
obj2.AddString "test", "testValue4"
msgBox obj2.IsString("test")
msgBox obj2.IsArray("test")
MsgBox obj2.GetString("test")
Set arr = CreateObject("o2Mate.Array")
Set f = CreateObject("o2Mate.Fields")
f.AddString "test2", "testValue5"
f.AddString "test3", "testValue6"
MsgBox f.Exists("test2")
MsgBox f.GetString("test2")
arr.add f
'obj2.AddArray "array1", arr
obj2.AddArray "array2", arr
Dim arr2
Set arr2 = obj2.GetArray("array2")
arr2.add f
obj2.Save "C:\Documents and Settings\Olivier\Mes documents\Visual Studio 2005\Projects\Projects\o2Mate\tests\vbtomate_dict.xml"

MsgBox "fin"