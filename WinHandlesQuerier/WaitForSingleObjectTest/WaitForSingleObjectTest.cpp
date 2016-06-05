
#include "stdafx.h"
#include<Windows.h>
#include <iostream>
#include <VersionHelpers.h>
#include <windows.h>
#include <string>

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
	std::cout << "PID: " << GetCurrentProcessId() << std::endl;

	HANDLE hEvent = CreateEvent(nullptr, TRUE, FALSE, L"Alfred");

	std::cout << "WaitForSingleObject :: handle:" << hEvent << std::endl;

	WaitForSingleObject(hEvent, INFINITE);

	std::cout << "CloseHandle" << std::endl;

	CloseHandle(hEvent);

	return 0;
}



