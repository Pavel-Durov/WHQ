// NativeWinDeclarationsValidator.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <windows.h>

using namespace std;

void PrintCriticalSectionStruct();
void PrintContextStruct();

int main()
{

	//PrintContextStruct();
	PrintCriticalSectionStruct();
	return 0;
}

void PrintCriticalSectionStruct()
{
	#define PRINTMBR(m) cout << #m": " << offsetof(CRITICAL_SECTION, m) << endl;
	cout << "Struct CRITICAL_SECTION Total Size: " << sizeof(CRITICAL_SECTION) << endl << endl;

	cout << endl;
	cout << "Member Offsets:" << endl;
	cout << "---------------" << endl;

	PRINTMBR(DebugInfo);
	PRINTMBR(LockCount);
	PRINTMBR(RecursionCount);
	PRINTMBR(OwningThread);
	PRINTMBR(LockSemaphore);
	PRINTMBR(SpinCount);

}


void PrintContextStruct() 
{
	#define PRINTMBR(m) cout << #m": " << offsetof(CONTEXT, m) << endl;
	cout << "Struct CONTEXT Total Size: " << sizeof(CONTEXT) << endl << endl;

	cout << "Data Types:" << endl;
	cout << "-----------" << endl;
	cout << "DWORD64: " << sizeof(DWORD64) << " bytes" << endl;
	cout << "DWORD: " << sizeof(DWORD) << " bytes" << endl;
	cout << "WORD: " << sizeof(WORD) << " bytes" << endl;
	cout << "ULONGLONG: " << sizeof(ULONGLONG) << " bytes" << endl;
	cout << "LONGLONG: " << sizeof(LONGLONG) << " bytes" << endl;
	cout << "M128A: " << sizeof(M128A) << " bytes" << endl;
	cout << "XMM_SAVE_AREA32: " << sizeof(XMM_SAVE_AREA32) << " bytes" << endl;

	cout << endl;
	cout << "Member Offsets:" << endl;
	cout << "---------------" << endl;

	PRINTMBR(P1Home);
	PRINTMBR(P2Home);
	PRINTMBR(P3Home);
	PRINTMBR(P4Home);
	PRINTMBR(P5Home);
	PRINTMBR(P6Home);
	PRINTMBR(ContextFlags);

}

