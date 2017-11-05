// MicrosoftCPP.cpp : main project file.

#include "stdafx.h"

using namespace System;
using namespace o2Mate;

int main(array<System::String ^> ^args)
{
	Dictionnaire^ dict = gcnew Dictionnaire();
	dict->Load(L"C:\\Documents and Settings\\Olivier\\Mes documents\\CodeCommander\\temp\\dict.xml");
	Console::WriteLine(dict->GetString(L"Number"));

	bool b = 1 && 1;
	Console::WriteLine(b);
	Console::WriteLine((int)b);

	int a = 0;

	std::stringstream s;

	s << 100 << ' ' << 200 << ' ';
	s << (int)b << ' ';
	s << (a==0 ? "false" : "true");


	std::cout << s.str() << std::endl;

	

	std::wstringstream s2;
	s2.str(std::wstring(L"true"));

	bool res;
	s2 >> res;

	std::cout << res << std::endl;

    return 0;
}
