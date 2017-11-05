// Parallel.h
//
// This file was written for implements somethings
// with CodeCommander
//
// codecommander.codeplex.com
//


ref class Parallel {
public:
	delegate void worker(void *ptr);
	Parallel() { }
	Parallel(void *ptr, worker^ w) : datas(ptr), thisFct(w) {
		this->thread = gcnew System::Threading::Thread(gcnew System::Threading::ThreadStart(this, &Parallel::LaunchThread));
	}

	void Init(int count);
	void Start();
	void WaitToEnd();

private:
	static System::Threading::CountdownEvent ^evt;
	worker^ thisFct;
	void *datas;

	System::Threading::Thread^ thread;
	void LaunchThread();

};