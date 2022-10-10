#include "User.h"

/*
The function is a constractor to User
INPUT : NULL
OUTPUT: NULL
*/

User::User(string username, SOCKET sock)
{
	this->_username = username;
	this->_sock = sock;
}

/*
The function is a destractor to User
INPUT : NULL
OUTPUT: NULL
*/
User::~User()
{
}

/*
The function send the massage to the user
INPUT : message - the massage to send
OUTPUT: NULL
*/
void User::send(string message)
{
	Helper::sendData(this->_sock, message);
}
/*
The function is a getter to the username of the User
INPUT: NULL
OUTPUT: _username - the username of the user
*/
string User::getUsername()
{
	return this->_username;
}
/*
The function is a getter to the socket of the User
INPUT: NULL
OUTPUT: _sock - the socket of the user
*/
SOCKET User::getSocket()
{
	return _sock;
}



