dir /B /s /A:D bin obj > list.txt
for /F "delims=;" %%i in (list.txt) do rmdir /S /Q "%%i"
del list.txt

dir /B /s *.csproj.user > list.txt
for /F "delims=;" %%i in (list.txt) do del "%%i"
del list.txt

dir /B /s *.bak > list.txt
for /F "delims=;" %%i in (list.txt) do del "%%i"
del list.txt

dir /B /s Thumbs.db > list.txt
for /F "delims=;" %%i in (list.txt) do del "%%i"
del list.txt
