


class writer {
public:
	writer(wstring& f) : fileName(f), numIndent(0), line(true) {};

	void Indent() { ++this->numIndent; }
	void Unindent() { --this->numIndent; }

	inline wstring Spaces() {
		return wstring(L' ', writer::indentation * this->numIndent);
	}

	void WriteToFile(System::String^ text) {

		if (!System::String::IsNullOrEmpty(text)) {
			System::IO::FileInfo fi(gcnew System::String(fileName.c_str()));
			System::IO::FileStream^ fs = fi.Open(System::IO::FileMode::Append, System::IO::FileAccess::Write, System::IO::FileShare::Write);
			System::IO::StreamWriter^ sw = gcnew System::IO::StreamWriter(fs, System::Text::Encoding::GetEncoding("windows-1252"));
			sw->Write(this->WriteIndent(text));
			sw->Close();
			fs->Close();
			delete sw;
			delete fs;
		}

	}

	void WriteToFile(wstring& text) {

		if (text.length() > 0)
		{
			System::IO::FileInfo fi(gcnew System::String(fileName.c_str()));
			System::IO::FileStream^ fs = fi.Open(System::IO::FileMode::Append, System::IO::FileAccess::Write, System::IO::FileShare::Write);
			System::IO::StreamWriter^ sw = gcnew System::IO::StreamWriter(fs, System::Text::Encoding::GetEncoding("windows-1252"));
			sw->Write(this->WriteIndent(text));
			sw->Close();
			fs->Close();
			delete sw;
			delete fs;
		}
	}

	void WriteToFile(int value) {

		System::IO::FileInfo fi(gcnew System::String(fileName.c_str()));
		System::IO::FileStream^ fs = fi.Open(System::IO::FileMode::Append, System::IO::FileAccess::Write, System::IO::FileShare::Write);
		System::IO::StreamWriter^ sw = gcnew System::IO::StreamWriter(fs, System::Text::Encoding::GetEncoding("windows-1252"));
		sw->Write(this->WriteIndent(value.ToString()));
		sw->Close();
		fs->Close();
		delete sw;
		delete fs;
	}

	void WriteToFile(bool value) {

		System::IO::FileInfo fi(gcnew System::String(fileName.c_str()));
		System::IO::FileStream^ fs = fi.Open(System::IO::FileMode::Append, System::IO::FileAccess::Write, System::IO::FileShare::Write);
		System::IO::StreamWriter^ sw = gcnew System::IO::StreamWriter(fs, System::Text::Encoding::GetEncoding("windows-1252"));
		wstring s(value ? L"1" : L"0");
		sw->Write(this->WriteIndent(s));
		sw->Close();
		fs->Close();
		delete sw;
		delete fs;
	}

	void Start() {

		System::IO::FileInfo fi(gcnew System::String(fileName.c_str()));
		if (fi.Exists) {

			fi.Delete();

		}
	}

private:
	wstring fileName;
	int numIndent;
	bool line;
	static const int indentation = 4;

	System::String^ WriteIndent(wstring& text) {

		return this->WriteIndent(gcnew System::String(text.c_str()));

	}

	System::String^ WriteIndent(System::String^ text) {
		System::String^ output = gcnew System::String(L"");
		System::Text::RegularExpressions::Regex r(gcnew System::String(L"(((.*\\n\\r)+).*$|.*$)"), System::Text::RegularExpressions::RegexOptions::Multiline);
		for each(System::Text::RegularExpressions::Match^ m in r.Matches(text)) {
			if (this->line) {
				if (!System::String::IsNullOrEmpty(m->Value)) {
					output += gcnew System::String(this->Spaces().c_str());

					if (m->Value->IndexOf(L"\n\r") == -1) {
						output += m->Value;
						this->line = false;
					} else {
						output += m->Value->Replace("\n\r", System::Environment::NewLine);
					}
				}
			} else {
				if (!System::String::IsNullOrEmpty(m->Value)) {

					if (m->Value->IndexOf(L"\n\r") == -1) {
						output += m->Value;
					} else {
						output += m->Value->Replace("\n\r", System::Environment::NewLine);
						this->line = true;
					}
				}
			}
		}
		return output;
	}

};