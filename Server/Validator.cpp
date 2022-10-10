#include "Validator.h"
/*
The function cheks if the password is Vaild
INPUT : password - the password to check
OUTPUT : true if vaild and false if does'nt  
*/
bool Validator::isPasswordValid(string password)
{
	bool uppercase = false;
	bool lowercase = false;
	bool hasOneDigit = false;
	if (password.size() < 6)
	{
		return false;
	}
	for ( int i = 0; i < password.size() ; i++)
	{
		if (isalpha(password[i]))
		{
			if (isupper(password[i]))
			{
				uppercase = true;
			}
			if (islower(password[i]))
			{
				lowercase = true;
			}
		}
		if (isdigit(password[i]))
		{
			hasOneDigit = true;
		}

	}
	if (hasOneDigit && lowercase && uppercase && password.size() >= 4)
	{
		return true;
	}
	return false;
}
/*
The function cheks if the username is Vaild
INPUT : password - the username to check
OUTPUT : true if vaild and false if does'nt
*/
bool Validator::isUsernameValid(string username)
{
	if ( username.c_str() == "" || username.length() == 0)
	{
		return false;
	}
	if (username.size() < 6)
	{
		return false;
	}
	if (!isalpha(username[0]))
	{
		return false;
	}
	// there is space
	if (username.find_first_not_of(' ') == std::string::npos)
	{
		return false;
	}

	return true;
}
