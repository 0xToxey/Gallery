#include "ShowPhoto.h"

BOOL WINAPI CloseAppHandler(DWORD fdwCtrlType);
PROCESS_INFORMATION processInfo;

ShowPhoto::ShowPhoto(std::string appPath, std::string photoPath)
{
	std::string command = appPath + " " + photoPath;
	const char* cmdArgs = command.c_str();

	// Process details.
	STARTUPINFO StartupInfo;
	ZeroMemory(&StartupInfo, sizeof(StartupInfo));
	StartupInfo.cb = sizeof StartupInfo;

	// Create the procces to open the photo.
	if (!(CreateProcess(NULL, (LPSTR)cmdArgs, NULL, NULL, FALSE, 0, NULL, NULL, &StartupInfo, &(processInfo))))
	{
		throw MyException("Error opening photo.");
	}

	std::cout << "Opening photo." << std::endl;
	std::cout << "	> Press Ctrl + C to close OR close the app." << std::endl;

	// Set the function to handle with Ctrl + C case.
	SetConsoleCtrlHandler(CloseAppHandler, TRUE);

	// Wait until the process has exit\time out.
	WaitForSingleObject(processInfo.hProcess, INFINITE);

	// Close the handler of process.
	CloseHandle(processInfo.hThread);
}

BOOL WINAPI CloseAppHandler(DWORD fdwCtrlType)
{
	switch (fdwCtrlType)
	{
	// Handle exit(ctrl+c). 
	case CTRL_C_EVENT:
		printf("Closing photo...\n");
		TerminateProcess(processInfo.hProcess, 9); // Kill the process.
		return TRUE;

	default:
		return FALSE;
	}
}