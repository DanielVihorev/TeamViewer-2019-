#pragma once

#include <iostream>
#include <string>
#include <ctype.h>

#define EMPTY ""

using std::string;

class Validator
{
public:
	static bool isPasswordValid(string password);
	static bool isUsernameValid(string username);

};

