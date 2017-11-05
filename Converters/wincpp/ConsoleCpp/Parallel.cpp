// Parallel.cpp
//
// This file was written for implements somethings
// with CodeCommander
//
// codecommander.codeplex.com
//


#include "stdafx.h"

void Parallel::LaunchThread() {

	this->thisFct(this->datas);
	this->evt->Signal();
}

void Parallel::Start() {

	this->thread->Start();

}

void Parallel::Init(int count) {
	this->evt = gcnew System::Threading::CountdownEvent(count);
}

void Parallel::WaitToEnd() {

	this->evt->Wait();
	this->evt->Reset();

}
