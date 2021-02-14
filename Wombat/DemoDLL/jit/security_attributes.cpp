#include "security_attributes.hpp"

SecurityAttributes::SecurityAttributes()
{
	security_attributes.nLength = sizeof(SECURITY_ATTRIBUTES); 
	security_attributes.bInheritHandle = TRUE; 
	security_attributes.lpSecurityDescriptor = NULL; 
}

LPSECURITY_ATTRIBUTES SecurityAttributes::GetPointer()
{
	return &security_attributes;
}