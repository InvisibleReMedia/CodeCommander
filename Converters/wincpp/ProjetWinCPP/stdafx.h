// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

// TODO: reference additional headers your program requires here
#include <sstream>
#include <iostream>
#include <fstream>
#include <valarray>
#include <regex>

using namespace std;

#include <vcclr.h>

#include "loader.h"
#include "compiled.h"
#include "writer.h"
#include "Parallel.h"

extern wchar_t x_SpaceChar;
extern wchar_t x_PlusChar;
extern wchar_t x_MinusChar;
extern wchar_t x_StarChar;
extern wchar_t x_SlashChar;
extern wchar_t x_ColonChar;
extern wchar_t x_CommaChar;
extern wchar_t x_DotChar;
extern wchar_t x_ExChar;
extern wchar_t x_IntChar;
extern wchar_t x_EmptyChar;
extern wchar_t x_InfChar;
extern wchar_t x_SupChar;
extern wchar_t x_OpenParChar;
extern wchar_t x_CloseParChar;
extern wchar_t x_OpenBraChar;
extern wchar_t x_CloseBraChar;
extern wchar_t x_OpenSquaBraChar;
extern wchar_t x_CloseSquaBraChar;
extern wstring x_CrLf;
extern bool x_trueValue;
extern bool x_falseValue;

int wstring2i(const wstring& w);
bool wstring2b(const wstring& w);
wchar_t wstring2ch(const wstring& w);
int wstring2i(const wchar_t* str);
bool wstring2b(const wchar_t* str);
wchar_t wstring2ch(const wchar_t* str);
wstring toString(int i);
wstring toString(bool b);

#include "SimpleType.h"


