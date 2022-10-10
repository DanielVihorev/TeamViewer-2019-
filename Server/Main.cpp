#pragma comment (lib, "ws2_32.lib")

#include "Server.h"
#include <iostream>
#include <exception>
#include <WinSock2.h>
#include "WSAInitializer.h"

#define PORT 8820

using namespace std;

int main()
{
	try
	{
		WSAInitializer wsaInit;
		Server myServer;
		myServer.serve(PORT);
	}
	catch (exception& e)
	{
		cout << "Error occured: " << e.what() << endl;
	}
	system("PAUSE");
	return 0;
}