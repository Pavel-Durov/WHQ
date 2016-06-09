
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

CRITICAL_SECTION _critical_section = CRITICAL_SECTION();

int main()
{
	print_os_info();
	std::cout << "PID: " << GetCurrentProcessId() << std::endl;

	std::cout << "EnterCriticalSection" << std::endl;

	InitializeCriticalSection(&_critical_section);
	EnterCriticalSection(&_critical_section);

	std::cout << "Sleeping" << std::endl;

	Sleep(INFINITE);

	LeaveCriticalSection(&_critical_section);
	std::cout << "LeaveCriticalSection" << std::endl;


	return 0;
}
