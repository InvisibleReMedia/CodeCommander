
ref class Loader {
public:
	Loader(wstring& d) : dictFileName(gcnew System::String(d.c_str())) {
		this->dict = gcnew o2Mate::Dictionnaire();
		this->dict->Load(dictFileName);
	};

	void CreateDirectoryIfNotExists(wstring& dir) {

		System::IO::DirectoryInfo di(gcnew System::String(dir.c_str()));
		if (!di.Exists) {
			di.Create();
		}
	}

	void EraseFile(wstring& file) {
		System::IO::FileInfo fi(gcnew System::String(file.c_str()));
		if (fi.Exists) {
			fi.Delete();
		}
	}

	wstring ReplaceString(wstring subject, const wstring& search,
                          const wstring& replace) {
		size_t pos = 0;
		while ((pos = subject.find(search, pos)) != std::string::npos) {
			 subject.replace(pos, search.length(), replace);
			 pos += replace.length();
		}
		return subject;
	}

	wstring GetString(wstring s) {
		System::String^ output = this->InternalGetString(s);
		if (System::String::IsNullOrEmpty(output)) {
			return wstring();
		} else {
			pin_ptr<const wchar_t> str = ::PtrToStringChars(output);
			return wstring(str);
		}
	}

	wstring GetField(wstring name, int index, wstring fieldName) {
		System::String^ output = this->InternalGetField(name, index, fieldName);
		if (System::String::IsNullOrEmpty(output)) {
			return wstring();
		} else {
			pin_ptr<const wchar_t> str = ::PtrToStringChars(output);
			return wstring(str);
		}
	}

	int GetArrayCount(wstring& name) {
		if (this->dict->IsArray(gcnew System::String(name.c_str()))) {
			o2Mate::Array^ arr = dynamic_cast<o2Mate::Array^>(this->dict->GetArray(gcnew System::String(name.c_str())));
			return arr->Count;
		} else {
			return 0;
		}
	}

private:
	o2Mate::Dictionnaire^ dict;
	System::String^ dictFileName;


	System::String^ InternalGetString(wstring& s) {
		return this->dict->GetString(gcnew System::String(s.c_str()));
	}

	System::String^ InternalGetField(wstring& name, int index, wstring& fieldName) {
		if (this->dict->IsArray(gcnew System::String(name.c_str()))) {
			o2Mate::Array^ arr = dynamic_cast<o2Mate::Array^>(this->dict->GetArray(gcnew System::String(name.c_str())));
			if (index <= arr->Count) {
				o2Mate::Fields^ fields = dynamic_cast<o2Mate::Fields^>(arr->Item(index));
				return fields->GetString(gcnew System::String(fieldName.c_str()));
			} else {
				return gcnew System::String(L"");
			}
		} else {
			return gcnew System::String(L"");
		}
	}

};