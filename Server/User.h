#pragma once

#include <WinSock2.h>
#include <Windows.h>
#include "Helper.h"
#include <queue>
#include <fstream>
#include <thread>
#include <mutex>
#include <map>
#include <vector>

using namespace std;

class User
{
private:
	string _username;
	SOCKET _sock;
public:
	User(string username, SOCKET sock);
	~User();
	void send(string message);
	//getters
	string getUsername();
	SOCKET getSocket();
	
};

