#include "stdafx.h"
#include<Windows.h>
#include <string>
#include <iostream>
#include <thread>

#pragma warning(disable : 4996)

void print_os_info()
{
	//http://stackoverflow.com/questions/1963992/check-windows-version
	OSVERSIONINFOW info;
	ZeroMemory(&info, sizeof(OSVERSIONINFOW));
	info.dwOSVersionInfoSize = sizeof(OSVERSIONINFOW);

	LPOSVERSIONINFOW lp_info = &info;
	GetVersionEx(lp_info);

	printf("Windows version: %u.%u\n", info.dwMajorVersion, info.dwMinorVersion);
}

CRITICAL_SECTION _critical_section = CRITICAL_SECTION();
using namespace std;

void task();
void innerTask();

int main()
{
	print_os_info();
	std::cout << "PID: " << GetCurrentProcessId() << std::endl;

	thread t1(task);
	t1.join();

	return 0;
}

void innerTask()
{
	cout << "--DEAD LOCK--" << endl;

	EnterCriticalSection(&_critical_section);

	cout << "Leaving critical section" << endl;
}

void task()
{

	std::cout << "InitializeCriticalSection" << std::endl;

	InitializeCriticalSection(&_critical_section);

	std::cout << "EnterCriticalSection" << std::endl;

	EnterCriticalSection(&_critical_section);

	thread t1(innerTask);
	t1.join();
}
