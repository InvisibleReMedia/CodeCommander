Dim obj, elem
Set obj = CreateObject("o2Mate.Scope")
MsgBox "ajout:"
obj.Add "test", "test"
For Each elem in obj
	MsgBox elem.Name & "=" & elem.ValueString
Next
MsgBox "remove:"
obj.Remove "test"
MsgBox "ajout"
obj.Add "test2", "test2"
MsgBox "GetVariable:" & obj.GetVariable("test2").ValueString
For Each elem in obj
	MsgBox elem.Name & "=" & elem.ValueString
Next
MsgBox "push:"
obj.Push
MsgBox "ajout:"
obj.Add "test2", "test3"
MsgBox "GetVariable:" & obj.GetVariable("test2").ValueString
For Each elem in obj
	MsgBox elem.Name & "=" & elem.ValueString
Next
MsgBox "pop:"
obj.Pop
MsgBox "GetVariable:" & obj.GetVariable("test2").ValueString
For Each elem in obj
	MsgBox elem.Name & "=" & elem.ValueString
Next
MsgBox "remove:"
obj.Remove "test2"
MsgBox "fin:"
