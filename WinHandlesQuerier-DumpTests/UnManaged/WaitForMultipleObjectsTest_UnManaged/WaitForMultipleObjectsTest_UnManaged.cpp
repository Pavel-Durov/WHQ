
#include "stdafx.h"
#include<Windows.h>
#include <iostream>

#include <tchar.h>
#pragma warning(disable : 4996)
typedef BOOL(WINAPI *LPFN_ISWOW64PROCESS) (HANDLE, PBOOL);

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

LPFN_ISWOW64PROCESS fnIsWow64Process;
void print_process_info() {
	BOOL bIsWow64 = FALSE;

	LPFN_ISWOW64PROCESS fnIsWow64Process;
	fnIsWow64Process = (LPFN_ISWOW64PROCESS)GetProcAddress(
		GetModuleHandle(TEXT("kernel32")), "IsWow64Process");

	if (NULL != fnIsWow64Process)
	{
		if (!fnIsWow64Process(GetCurrentProcess(), &bIsWow64))
		{
			//handle error
		}
	}

	if (bIsWow64)
		_tprintf(TEXT("32 Bit Process\n"));
	else
		_tprintf(TEXT("64 Bit Process\n"));
}

int main()
{
	print_os_info();
	print_process_info();
	

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