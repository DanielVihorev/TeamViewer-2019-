#pragma once

#include <iostream>
#include <WinSock2.h>
#include "User.h"
#include <vector>
#include <string>
#include <winsock.h>
#include <WinSock2.h>

using std::string;

class RecievedMessage
{
private:
	SOCKET _sock;
	User * _user;
	int _messageCode;
	vector<string> _values;
public:
	RecievedMessage(SOCKET socket, int messageCode);
	RecievedMessage(SOCKET socket, int messageCode,vector<string> values);
	SOCKET getSock();
	User* getUser();
	void setUser(User* newUser);
	int getMessageCode();
	vector<string>& getValues();
	void closeSocket();
};

