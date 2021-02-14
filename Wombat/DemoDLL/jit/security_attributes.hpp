#ifndef __SECURITY_ATTRIBUTES__
#define __SECURITY_ATTRIBUTES__

#include <windows.h>

class SecurityAttributes
{
private:
	SECURITY_ATTRIBUTES security_attributes; 

public:
	SecurityAttributes();
	LPSECURITY_ATTRIBUTES GetPointer();
};

#endif