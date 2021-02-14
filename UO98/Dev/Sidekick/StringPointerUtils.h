
using namespace System;
using namespace System::Runtime::InteropServices;

public ref class StringPointerUtils abstract sealed
{
public:
    static String^ GetAsciiString(unsigned char* pChars)
    {
        return GetAsciiString(pChars,255);
    }

    static String^ GetAsciiString(unsigned char* pChars, int max)
    {
        int length;
        for (length = 0; *(pChars + length) != 0 && length < max; length++) ;
        if (length == 0) return String::Empty;

        String^ myManagedString = Marshal::PtrToStringAnsi((IntPtr)(unsigned char*)pChars, length);
        return System::String::Intern(myManagedString);
    }
};
