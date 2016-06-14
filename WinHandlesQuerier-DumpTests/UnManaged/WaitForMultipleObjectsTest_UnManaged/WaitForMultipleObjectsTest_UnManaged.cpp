
#include "stdafx.h"
#include<Windows.h>
#include <iostream>

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

int main()
{
	print_os_info();
	HANDLE hEvent = CreateEvent(nullptr, TRUE, FALSE, L"Alfred");
	HANDLE hMutex = CreateMutex(nullptr, FALSE, L"Bertha");
	HANDLE handles[] = { hEvent, hMutex };

	std::cout << "PID: " << GetCurrentProcessId() << std::endl;

	auto size = ARRAYSIZE(handles);

	std::cout << "WaitForMultipleObjects" <<"size : "
		<< size <<",handles : " << handles <<", WaitTime (INFINITE): "<< INFINITE << std::endl;

	WaitForMultipleObjects(size, handles, TRUE, INFINITE);

	std::cout << "CloseHandle" << std::endl;

	CloseHandle(hEvent);

	return 0;
}