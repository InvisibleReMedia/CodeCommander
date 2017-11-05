# Script pour remplacer les espaces
# dans les fichiers XML

function ReplaceWhitespace([string] $fileName) {

	# sauvegarde
	Copy-Item -Path $fileName -Destination ($fileName + ".bak")

	$xmlObj = New-Object XML
	$xmlObj.Load($fileName)
	$res = $xmlObj | Select-Xml -XPath "//texte"
	$res | ForEach-Object { $_.Node.innerText = $_.Node.innerText.Replace("\", "\\").Replace(" ", "·").Replace("\t", "").Replace("\r\n", "¶") }
	$xmlObj.Save($fileName)

	$xmlObj = New-Object XML
	$xmlObj.PreserveWhitespace = $false
	$xmlObj.Load($fileName)
	$xmlObj.PreserveWhitespace = $false
	$xmlObj.Save($fileName)
}

function Get-ScriptDirectory() {
	$invocation = (Get-Variable MyInvocation -Scope 1).Value
	Split-Path $invocation.MyCommand.Path
}
# Sets the current path
$currentPath = Join-Path (Get-ScriptDirectory) "."

$args | ForEach-Object {
	Get-Item -Path (Join-Path $_ "\*.xml") | ForEach-Object { ReplaceWhitespace($_) }
}

