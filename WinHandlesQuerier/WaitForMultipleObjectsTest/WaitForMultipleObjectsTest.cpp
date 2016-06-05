// WaitForMultipleObjectsTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include<Windows.h>

int main()
{
	HANDLE hEvent = CreateEvent(nullptr, TRUE, FALSE, L"Alfred");
	HANDLE hMutex = CreateMutex(nullptr, FALSE, L"Bertha");
	HANDLE handles[] = { hEvent, hMutex };

	WaitForMultipleObjects(ARRAYSIZE(handles), handles, TRUE, 60000);

	CloseHandle(hEvent);

	return 0;
}

