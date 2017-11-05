dir /B /s "AssemblyInfo.cs" > list.txt
for /F "delims=;" %%i in (list.txt) do xcopy /Y ".\Versioning\Properties\AssemblyInfo.cs" "%%i"
del list.txt
