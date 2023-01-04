#define WIN32_LEAN_AND_MEAN

#include <Windows.h>
#include <filesystem>

bool LaunchGame(
    PROCESS_INFORMATION* ProcInfo,
    const char* cExePath,
    const char* cTeardownPath
) {
    STARTUPINFOA StartupInfo;
    ZeroMemory(&StartupInfo, sizeof(StartupInfo));
    SetEnvironmentVariableA("SteamAppId", "1167630"); // Set SteamAppId var to initialize SteamAPI
    return CreateProcessA(const_cast<LPSTR>(cExePath), nullptr, nullptr, nullptr, true, CREATE_DEFAULT_ERROR_MODE | CREATE_SUSPENDED, nullptr, cTeardownPath, &StartupInfo, ProcInfo);
}

extern "C" __declspec(dllexport) bool LaunchAndInjectAndWaitForGameToClose(const char* cTeardownPath) {
    char cDLLPath[MAX_PATH];
    char cExePath[MAX_PATH];
    PROCESS_INFORMATION ProcInfo;
    STARTUPINFOA StartupInfo;
    sprintf_s(cDLLPath, "%s\\%s", cTeardownPath, "TDMP.dll");
    sprintf_s(cExePath, "%s\\%s", cTeardownPath, "teardown.exe");
    ZeroMemory(&ProcInfo, sizeof(ProcInfo));
    ZeroMemory(&StartupInfo, sizeof(StartupInfo));

    const char* cDLLPath2 = cDLLPath;

    if (!std::filesystem::exists(cDLLPath)) {
        return false;
    }

    if (!std::filesystem::exists(cExePath)) {
        return false;
    }

    FILE* TeardownExe;
    fopen_s(&TeardownExe, cExePath, "rb");
    if (!TeardownExe) {
        return false;
    }

    fseek(TeardownExe, 0, SEEK_END);
    long lFileSize = ftell(TeardownExe);
    rewind(TeardownExe);

    void* pExeBuffer = malloc(lFileSize);
    if (!pExeBuffer) {
        return false;
    }

    fread(pExeBuffer, lFileSize, 1, TeardownExe);
    fclose(TeardownExe);

    if (!LaunchGame(&ProcInfo, cExePath, cTeardownPath)) {
        return false;
    }

    if (!ProcInfo.hProcess) {
        return false;
    }

    const size_t dwDLLPath2Length = strlen(cDLLPath2);

    // Allocate memory for the DLL
    const LPVOID pRemoteDLL = VirtualAllocEx(ProcInfo.hProcess, nullptr, dwDLLPath2Length + 1, MEM_COMMIT, PAGE_READWRITE);
    if (!pRemoteDLL) {
        return false;
    }

    // Write the DLL to the process
    if (!WriteProcessMemory(ProcInfo.hProcess, pRemoteDLL, cDLLPath2, dwDLLPath2Length + 1, nullptr)) {
        return false;
    }

    // Get the address of LoadLibraryA
    const auto pLoadLibraryA = reinterpret_cast<LPVOID>(GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryA"));
    if (!pLoadLibraryA) {
        return false;
    }

    CreateRemoteThread(ProcInfo.hProcess, nullptr, 0, reinterpret_cast<LPTHREAD_START_ROUTINE>(pLoadLibraryA), pRemoteDLL, 0, nullptr);

    if (!ProcInfo.hThread) {
        CloseHandle(ProcInfo.hProcess);
        return false;
    }

    // Resume the process
    ResumeThread(ProcInfo.hThread);

    Sleep(2000);

    WaitForSingleObject(ProcInfo.hProcess, INFINITE);
    CloseHandle(ProcInfo.hProcess);
    CloseHandle(ProcInfo.hThread);

    return true;
}

BOOL APIENTRY DllMain(
    HMODULE hModule,
    DWORD ul_reason_for_call,
    LPVOID lpReserved
) {
    switch (ul_reason_for_call) {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}