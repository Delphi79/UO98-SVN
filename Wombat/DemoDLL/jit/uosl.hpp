#ifndef __UOSL__
#define __UOSL__

class InterfaceToUOSL
{
private:
	char *PathToUOSL;
	
public:
	InterfaceToUOSL();
	
	const char *GetPath();
	bool IsActive();
	
	char *TranslateScript(const char *Input);
};

#endif