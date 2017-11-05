#include "stdafx.h"

void Parallel::LaunchThread() {

	this->thisFct(this->datas);
}

void Parallel::Start() {

	this->thread->Start();

}