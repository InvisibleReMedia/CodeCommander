

ref class Parallel {
public:
	delegate void worker(valarray<wstring*>& data);
	Parallel(valarray<wstring*>& arr, worker^ w) : datas(arr), thisFct(w) {
		this->thread = gcnew System::Threading::Thread(gcnew System::Threading::ThreadStart(this, &Parallel::LaunchThread));
	}

	void Start();

private:
	worker^ thisFct;
	valarray<wstring*>& datas;

	System::Threading::Thread^ thread;
	void LaunchThread();

};