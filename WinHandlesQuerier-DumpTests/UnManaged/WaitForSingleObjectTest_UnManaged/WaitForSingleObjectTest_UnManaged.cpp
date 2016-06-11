#include "stdafx.h"
#include<Windows.h>
#include <iostream>
#include <VersionHelpers.h>
#include <windows.h>
#include <string>
#include <stdlib.h>

#pragma warning(disable : 4996)

void print_os_info()
{
	OSVERSIONINFOW info;
	ZeroMemory(&info, sizeof(OSVERSIONINFOW));
	info.dwOSVersionInfoSize = sizeof(OSVERSIONINFOW);

	LPOSVERSIONINFOW lp_info = &info;
	GetVersionEx(lp_info);

	//std::cout<<(system("systeminfo")) << std::endl;

	printf("Windows version: %u.%u\n", info.dwMajorVersion, info.dwMinorVersion);
}

int main()
{

#ifdef DEBUG
	std::cout << "DEBUG MODE" << std::endl;
#else
	std::cout << "RELEASE MODE" << std::endl;
#endif


	

	print_os_info();
	std::cout << "PID: " << GetCurrentProcessId() << std::endl;

	HANDLE hEvent = CreateEvent(nullptr, TRUE, FALSE, L"Alfred");

	std::cout << "WaitForSingleObject :: handle:" << hEvent << std::endl;

	WaitForSingleObject(hEvent, INFINITE);

	std::cout << "CloseHandle" << std::endl;

	CloseHandle(hEvent);

	return 0;
}
