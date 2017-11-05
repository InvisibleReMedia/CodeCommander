// SimpleType.h
//
// This file was written to implement somethings
// with CodeCommander
//
// codecommander.codeplex.com
//

//
// advertising for unions
// http://www.informit.com/guides/content.aspx?g=cplusplus&seqNum=408
//

enum EnumSimpleType {
        E_WCHAR,
        E_STRING,
        E_NUMBER,
        E_BOOL,
        E_STRING_OBJECT,
        E_CONST_STRING_OBJECT
};

extern "C" {
	union UnionSimpleType {
		wchar_t m_ch;
		int m_i;
		bool m_b;
		wstring* m_ws;

	public:
		explicit UnionSimpleType(EnumSimpleType type) {
			// init
			m_ws = NULL;
			if (type == E_BOOL) { m_b = 0; }
			else if (type == E_NUMBER) { m_i = 0; }
			else if (type == E_WCHAR) { m_ch = '\0'; }
			else if (type == E_STRING || type == E_STRING_OBJECT || type == E_CONST_STRING_OBJECT) { m_ws = new wstring(); }
		};
	};
};

class SimpleType {

public:
	SimpleType() : e(E_NUMBER), u(E_NUMBER) { u.m_i = 0; }
	SimpleType(wchar_t c) : e(E_WCHAR), u(E_WCHAR) { u.m_ch = c; }
	SimpleType(bool b) : e(E_BOOL), u(E_BOOL) { u.m_b = b; }
	SimpleType(int i) : e(E_NUMBER), u(E_NUMBER) { u.m_i = i; }
	SimpleType(wchar_t *ptr) : e(E_STRING), u(E_STRING) { u.m_ws->assign(ptr); }
	SimpleType(const wstring& ws) : e(E_CONST_STRING_OBJECT), u(E_CONST_STRING_OBJECT) { u.m_ws->assign(ws); }
	SimpleType(wstring& ws) : e(E_STRING_OBJECT), u(E_STRING_OBJECT) { u.m_ws->assign(ws); }
	SimpleType(const SimpleType& st) : e(st.e), u(st.e) { 
		this->copy(st);
	}
	~SimpleType() {
		this->reset();
	}

	SimpleType& operator=(SimpleType& st) {
		if (&st != this)
			this->copy(st);
		return *this;
	}

	SimpleType operator=(int& i) {
		this->set(i);
		return *this;
	}

	SimpleType operator=(bool& b) {
		this->set(b);
		return *this;
	}

	SimpleType operator=(wchar_t& c) {
		this->set(c);
		return *this;
	}

	SimpleType operator=(wchar_t* str) {
		this->set(str);
		return *this;
	}

	SimpleType operator=(const wstring& cws) {
		this->set(cws);
		return *this;
	}

	SimpleType operator=(wstring& ws) {
		this->set(ws);
		return *this;
	}

	// static accessors
	wchar_t getWCHAR() const { SimpleType st(*this); st.convert(st.e, E_WCHAR); return st.u.m_ch; }
	bool getBOOL() const { SimpleType st(*this); st.convert(st.e, E_BOOL); return st.u.m_b; }
	int getNUMBER() const { SimpleType st(*this); st.convert(st.e, E_NUMBER); return st.u.m_i; }
	const wchar_t* getSTRING() { SimpleType st(*this); st.convert(st.e, E_STRING); return st.u.m_ws->c_str(); }
	wstring getWSTRING() const { SimpleType st(*this); st.convert(st.e, E_STRING_OBJECT); return *st.u.m_ws; }
	const wstring getCWSTRING() const { SimpleType st(*this); st.convert(st.e, E_CONST_STRING_OBJECT); return *st.u.m_ws; }

	wchar_t inferWCHAR() { this->convert(this->e, E_WCHAR); return this->getWCHAR(); }
	bool inferBOOL() { this->convert(this->e, E_BOOL); return this->getBOOL(); }
	int inferNUMBER() { this->convert(this->e, E_NUMBER); return this->getNUMBER(); }
	const wchar_t* inferSTRING() { this->convert(this->e, E_STRING); return this->getSTRING(); }
	wstring inferWSTRING() { this->convert(this->e, E_STRING_OBJECT); return this->getWSTRING(); }
	const wstring inferCWSTRING() { this->convert(this->e, E_CONST_STRING_OBJECT); return this->getCWSTRING(); }

	// assignments
	void set(wchar_t c) { this->reset(); this->e = E_WCHAR; this->u.m_ch = c; }
	void set(bool b) { this->reset(); this->e = E_BOOL; this->u.m_b = b; }
	void set(int i) { this->reset(); this->e = E_NUMBER; this->u.m_i = i; }
	void set(const wchar_t* ptr) { this->reset(); this->e = E_STRING; this->u.m_ws = new wstring(ptr); }
	void set(wchar_t* ptr) { this->reset(); this->e = E_STRING; this->u.m_ws = new wstring(ptr); }
	void set(const wstring& ws) { this->reset(); this->e = E_CONST_STRING_OBJECT; this->u.m_ws = new wstring(ws); }
	void set(wstring& ws) { this->reset(); this->e = E_STRING_OBJECT; this->u.m_ws = new wstring(ws); }


	// operators
	SimpleType& operator+(int i) { return this->add(*this, i); }
	SimpleType& operator+(wchar_t c) { return this->concat(*this, c); }
	SimpleType& operator+(const wchar_t* str) { return this->concat(*this, str); }
	SimpleType& operator+(wchar_t* str) { return this->concat(*this, str); }
	SimpleType& operator+(wstring& ws) { return this->concat(*this, ws); }
	SimpleType& operator+(const wstring& ws) { return this->concat(*this, ws); }
	SimpleType& operator-(int i) { return this->substract(*this, i); }
	SimpleType& operator*(int i) { return this->mult(*this, i); }
	SimpleType& operator/(int i) { return this->div(*this, i); }
	SimpleType operator<(int i) { return this->inf(*this, i); }
	SimpleType operator>(int i) { return this->sup(*this, i); }
	SimpleType operator==(int i) {
		return this->equals(*this, i);
	}
	SimpleType operator==(wchar_t c) {
		return this->equals(*this, c);
	}

	SimpleType operator==(const wchar_t* str) {
		return this->equals(*this, str);
	}

	SimpleType operator==(wchar_t* str) {
		return this->equals(*this, str);
	}

	SimpleType operator==(wstring& ws) {
		return this->equals(*this, ws);
	}

	SimpleType operator==(const wstring& ws) {
		return this->equals(*this, ws);
	}

	wostream& print(wostream& o) {
		
		if (this->e == E_BOOL) { o << this->u.m_b; }
		else if (this->e == E_NUMBER) { o << this->u.m_i; }
		else if (this->e == E_WCHAR) { o << this->u.m_ch; }
		else if (this->e == E_STRING || this->e == E_STRING_OBJECT || this->e == E_CONST_STRING_OBJECT) { o << this->u.m_ws->c_str(); }
		return o;
	}

	operator bool() {
		return this->getBOOL();
	}

private:
	EnumSimpleType e;
	UnionSimpleType u;

	void reset() {

		// remove old
		if (this->e == E_STRING || this->e == E_STRING_OBJECT || this->e == E_CONST_STRING_OBJECT) {
			delete this->u.m_ws;
		}

		// change type
		this->e = E_NUMBER;

		// set resetted
		this->u.m_i = 0;
	}

	void copy(const SimpleType& st) {

		// remove old
		this->reset();

		// change type
		this->e = st.e;

		// copy new
		if (st.e == E_BOOL) { this->u.m_b = st.u.m_b; }
		else if (st.e == E_NUMBER) { this->u.m_i = st.u.m_i; }
		else if (st.e == E_WCHAR) { this->u.m_ch = st.u.m_ch; }
		else if (st.e == E_STRING || st.e == E_STRING_OBJECT || st.e == E_CONST_STRING_OBJECT) {
			this->u.m_ws = new wstring();
			this->u.m_ws->assign(st.u.m_ws->c_str());
		}
	}

	SimpleType& add(const SimpleType& st, int i) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_NUMBER)
			loc.convert(loc.e, E_NUMBER);
		// compute
		this->set(loc.u.m_i + i);
		return *this;
	}

	SimpleType& substract(const SimpleType& st, int i) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_NUMBER)
			loc.convert(loc.e, E_NUMBER);
		// compute
		this->set(loc.u.m_i - i);
		return *this;
	}

	SimpleType& mult(const SimpleType& st, int i) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_NUMBER)
			loc.convert(loc.e, E_NUMBER);
		// compute
		this->set(loc.u.m_i * i);
		return *this;
	}

	SimpleType& div(const SimpleType& st, int i) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_NUMBER)
			loc.convert(loc.e, E_NUMBER);
		// compute
		this->set(loc.u.m_i / i);
		return *this;
	}

	SimpleType& concat(const SimpleType& st, wchar_t c) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_STRING)
			loc.convert(loc.e, E_STRING);
		// compute
		loc.u.m_ws->push_back(c);
		this->set(loc.u.m_ws->c_str());
		return *this;
	}

	SimpleType& concat(const SimpleType& st, wchar_t* str) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_STRING)
			loc.convert(loc.e, E_STRING);
		// compute
		this->set(loc.u.m_ws->append(str));
		return *this;
	}

	SimpleType& concat(const SimpleType& st, const wchar_t* str) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_STRING)
			loc.convert(loc.e, E_STRING);
		// compute
		this->set(loc.u.m_ws->append(str));
		return *this;
	}

	SimpleType& concat(const SimpleType& st, wstring& ws) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_STRING_OBJECT)
			loc.convert(loc.e, E_STRING_OBJECT);
		// compute
		this->set(loc.u.m_ws->append(ws));
		return *this;
	}

	SimpleType& concat(const SimpleType& st, const wstring& ws) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_CONST_STRING_OBJECT)
			loc.convert(loc.e, E_CONST_STRING_OBJECT);
		// compute
		this->set(loc.u.m_ws->append(ws));
		return *this;
	}

	SimpleType inf(SimpleType& st, int i) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_NUMBER)
			loc.convert(loc.e, E_NUMBER);
		// compute
		return SimpleType(loc.u.m_i < i);
	}

	SimpleType sup(SimpleType& st, int i) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_NUMBER)
			loc.convert(loc.e, E_NUMBER);
		// compute
		return SimpleType(loc.u.m_i > i);
	}

	SimpleType equals(SimpleType& st, int i) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_NUMBER)
			loc.convert(loc.e, E_NUMBER);
		// compute
		return SimpleType(loc.u.m_i == i);
	}

	SimpleType equals(SimpleType& st, wchar_t c) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_WCHAR)
			loc.convert(loc.e, E_WCHAR);
		// compute
		return SimpleType(loc.u.m_ch == c);
	}

	SimpleType equals(SimpleType& st, bool b) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_BOOL)
			loc.convert(loc.e, E_BOOL);
		// compute
		return SimpleType(loc.u.m_b == b);
	}

	SimpleType equals(SimpleType& st, const wchar_t* str) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_STRING)
			loc.convert(loc.e, E_STRING);
		// compute
		return SimpleType(loc.u.m_ws->compare(str) == 0);
	}

	SimpleType equals(SimpleType& st, wchar_t* str) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_STRING)
			loc.convert(loc.e, E_STRING);
		// compute
		return SimpleType(loc.u.m_ws->compare(str) == 0);
	}

	SimpleType equals(SimpleType& st, const wstring& ws) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_STRING_OBJECT)
			loc.convert(loc.e, E_STRING_OBJECT);
		// compute
		return SimpleType(loc.u.m_ws->compare(ws) == 0);
	}

	SimpleType equals(SimpleType& st, wstring& ws) {

		// current copy
		SimpleType loc(st);
		// adapt type
		if (loc.e != E_CONST_STRING_OBJECT)
			loc.convert(loc.e, E_CONST_STRING_OBJECT);
		// compute
		return SimpleType(loc.u.m_ws->compare(ws) == 0);
	}

	void convert(EnumSimpleType from, EnumSimpleType to)
	{
		SimpleType::convert(*this, from, to);
	}

	static void convert(SimpleType& st, EnumSimpleType from, EnumSimpleType to) {

		st.e = to;

		if (from == to) { }
		else if (from == E_WCHAR) {
			if (to == E_BOOL) {
				st.u.m_b = (st.u.m_ch != L'\0') ? true : false;
			} else if (to == E_NUMBER) {
				st.u.m_i = (st.u.m_ch >= '1' && st.u.m_ch <= '9') ? (st.u.m_ch - '0') : 0;
			} else if (to == E_STRING || to == E_STRING_OBJECT || to == E_CONST_STRING_OBJECT) {
				wstring* ptr = new wstring();
				if (st.u.m_i > 0)
					ptr->push_back(st.u.m_ch);
				st.u.m_ws = ptr;
			}
		} else if (from == E_BOOL) {
			if (to == E_WCHAR) {
				st.u.m_ch = (st.u.m_b) ? '1' : '0';
			} else if (to == E_NUMBER) {
				st.u.m_i = (st.u.m_b) ? 1 : 0;
			} else if (to == E_STRING || to == E_STRING_OBJECT || to == E_CONST_STRING_OBJECT) {
				wstring* ptr = new wstring();
				ptr->push_back(st.u.m_b ? '1' : '0');
				st.u.m_ws = ptr;
			}
		} else if (from == E_NUMBER) {
			if (to == E_WCHAR) {
				int i = st.u.m_i;
				if (i < 0) i = 0;
				if (i > 9) i = 9;
				st.u.m_ch = i + '0';
			} else if (to == E_BOOL) {
				st.u.m_b = (st.u.m_i != 0);
			} else if (to == E_STRING || to == E_STRING_OBJECT || to == E_CONST_STRING_OBJECT) {
				wstring* ptr = new wstring();
				ptr->assign(toString(st.u.m_i));
				st.u.m_ws = ptr;
			}
		} else if (from == E_STRING || from == E_STRING_OBJECT || from == E_CONST_STRING_OBJECT) {
			if (to == E_WCHAR) {
				SimpleType loc('\0');
				loc.u.m_ch = st.u.m_ws->c_str()[0];
				st.reset();
				st = loc.u.m_ch;
			} else if (to == E_BOOL) {
				SimpleType loc(false);
				loc.u.m_b = (st.u.m_ws->c_str()[0] != L'\0') ? true : false;
				st.reset();
				st = loc.u.m_b;
			} else if (to == E_NUMBER) {
				SimpleType loc(false);
				loc.u.m_i = wstring2i(st.u.m_ws->c_str());
				st.reset();
				st = loc.u.m_i;
			}
		}
	}

};

wostream& operator<<(wostream& o, SimpleType& st);
