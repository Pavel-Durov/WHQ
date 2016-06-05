
#include "stdafx.h"
#include<Windows.h>
#include <iostream>


int main()
{
	std::cout << "PID: " << GetCurrentProcessId() << std::endl;

	HANDLE hEvent = CreateEvent(nullptr, TRUE, FALSE, L"Alfred");
	
	std::cout << "WaitForSingleObject :: handle:"<< hEvent << std::endl;

	WaitForSingleObject(hEvent, INFINITE);

	std::cout << "CloseHandle" << std::endl;

	CloseHandle(hEvent);

	return 0;
}

