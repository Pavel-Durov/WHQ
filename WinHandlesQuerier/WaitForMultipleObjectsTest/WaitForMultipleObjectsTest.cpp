
#include "stdafx.h"
#include<Windows.h>
#include <iostream>

int main()
{
	HANDLE hEvent = CreateEvent(nullptr, TRUE, FALSE, L"Alfred");
	HANDLE hMutex = CreateMutex(nullptr, FALSE, L"Bertha");
	HANDLE handles[] = { hEvent, hMutex };

	std::cout << "WaitForMultipleObjects" << std::endl;

	WaitForMultipleObjects(ARRAYSIZE(handles), handles, TRUE, 60000);

	std::cout << "CloseHandle" << std::endl;

	CloseHandle(hEvent);

	return 0;
}

