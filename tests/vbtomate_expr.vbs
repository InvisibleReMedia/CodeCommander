dim e
dim scope
dim res
Set e = CreateObject("o2Mate.Expression")
Set scope = CreateObject("o2Mate.Scope")
scope.Add "i", "1"
MsgBox "continue"
Set res = e.Evaluate("i+1", scope)
if Not res Is Nothing Then
	MsgBox res.ValueString
Else
	MsgBox "nothing"
End If