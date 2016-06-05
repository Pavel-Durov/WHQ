
#include "stdafx.h"
#include<Windows.h>
#include <iostream>


CRITICAL_SECTION _critical_section = CRITICAL_SECTION();

int main()
{
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

