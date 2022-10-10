#pragma once
//added includes
#include <WinSock2.h>
#include <Windows.h>
#include "Helper.h"
#include "sqlite3.h"
#include "User.h"
#include "DataBase.h"
#include "RecievedMessage.h"
#include "Validator.h"
#include "Server.h"
//visual studio includes
#include <queue>
#include <fstream>
#include <thread>
#include <mutex>
#include <map>
#include <vector>
#include <exception>
#include <iostream>
#include <string>
//defines
#define FILE_CONTENT_SIZE 5
#define USERNAME_SIZE 2

#define LOG_IN 200
#define SIGN_OUT 201
#define SIGN_UP 203
#define SUCCESS 206
//#define AVAILABLE_ROOM_REQUEST 205//Not used
//#define ROOM_USERS_REQUEST 207//Not used
//#define ROOM_JOIN 209
//#define ROOM_LEAVE 211//Not used
//#define ROOM_CREATE 213
//#define ROOM_CLOSE 215//Not used
//#define LEAVE_ROOM 222//Not used 
#define SIZE 1000 
#define TXT_FILE "text"
#define DEFAULT_BUFLEN 1024
#define FAIL "200"
#define LEN_CHECK 2//Not needed?



using std::cout;

class Server
{
public:
	Server();
	~Server();
	void serve(int port);
	//user connection
	bool handleLogIn(RecievedMessage * msg);//200
	void handleSignOut(RecievedMessage * msg);//201
	bool handleSignUp(RecievedMessage * msg);//203 
	//gets the user 
	User * getUserByName(string name);
	User* getUserBySocket(SOCKET socket);
	//massage
	RecievedMessage * buildRecievedMessage(SOCKET socket, int messageCode);
	void handleRecievedMessages();
	void addRecievedMessage(RecievedMessage * rcvMessage);
	//room
	//bool handleCreateRoom(RecievedMessage* msg);//213
	//bool handleCloseRoom(RecievedMessage* msg);//215
	//bool handleJoinRoom(RecievedMessage* msg);//209
	//bool handleLeaveRoom(RecievedMessage * msg);//211 - check what to do 

	//make a check if the room exists
	//controlling
	//void handleControl(RecievedMessage * msg);//220

private:

	void accept();
	void clientHandler(SOCKET clientSocket);

	map<SOCKET, User*> _connectedUsers;
	vector<SOCKET> _users;
	mutex _mtxRecievedMessages;
	queue<RecievedMessage*> _queRcvMessages;
	DataBase _db;
	condition_variable _cond;

	SOCKET _serverSocket;

};

