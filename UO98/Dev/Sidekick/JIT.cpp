#pragma unmanaged

#include "stdafx.h"

#include <ctype.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "patcher.h"

#pragma warn -8002 // Restarting compile using assembly

namespace NativeMethods
{

    bool PrintConstantConversions = false;

    bool DebugOutput = false;

    /*********************************************/
    /*                                           */
    /* FUNCTIONS RELATED WITH THE UODEMO SCRIPTS */
    /*                                           */
    /*********************************************/

    int ConvertEventNameToIndex(const char *Name)
    {
      for(int i = 0; i < 71; i ++)
      {
        register char *s = (char *)*((int *) 0x5EE458 + 2 * i + 1);
        if(!strcmp(s, Name))
          return i;
      }
      return -1;
    }

    const unsigned int GetLastValidScriptByte()
    {
      return * (unsigned int * const) 0x611984;
    }

    unsigned short *GetObfuscatedScriptByteTable()
    {
      return (unsigned short *) 0x610B40;
    }

    bool IsObfuscatedScriptByte(unsigned short ObfuscatedWord)
    {
      const unsigned int LastValidScriptByte = GetLastValidScriptByte();
      unsigned short *ObfuscatedScriptByteTable = GetObfuscatedScriptByteTable();
      for(unsigned int i = 0; i < LastValidScriptByte; i ++)
        for(unsigned int j = 0; j < 5; j ++)
          if(ObfuscatedWord == *(ObfuscatedScriptByteTable + i * 5 + j))
            return true;
      return false;
    }

    unsigned short ObfuscateScriptByte(unsigned char ScriptByte)
    {
      if(ScriptByte > (unsigned char) GetLastValidScriptByte())
        return 0xFFFF;
   
      return *(GetObfuscatedScriptByteTable() + ScriptByte * 5);
    }

    /* FUNCTIONS THAT EXTEND THE SCRIPT OBJECT */

    unsigned char GetCurrentScriptType()
    {
      unsigned char _AL;
      __asm
      {
        mov edx, 0x63E128    // Points to the current script being compiled (Being-CompiledObject class)
        mov ecx, [edx]
        mov al, [ecx + 0x54] // See MyCompiledObjectConstructor...
        mov _AL, al
      }
      return _AL; // 0 = Unknown, 1 = M, 2 = UOC
    }

    void SetCurrentScriptType(unsigned char sType)
    {
      __asm
      {
        mov al, sType
        mov edx, 0x63E128    // Points to the current script being compiled (Being-CompiledObject class)
        mov ecx, [edx]
        mov [ecx + 0x54], al // See MyCompiledObjectConstructor...
      }
    }

    unsigned char GetCurrentScriptExtraByte()
    {
      unsigned char _AL;
 
      __asm
      {
        mov edx, 0x63E128    // Points to the current script being compiled (Being-CompiledObject class)
        mov ecx, [edx]
        mov al, [ecx + 0x55] // See MyCompiledObjectConstructor...
        mov _AL, al
      }
      return _AL;
    }

    void SetCurrentScriptExtraByte(unsigned char ExtraByte)
    {
      __asm
      {
        mov al, ExtraByte
        mov edx, 0x63E128    // Points to the current script being compiled (Being-CompiledObject class)
        mov ecx, [edx]
        mov [ecx + 0x55], al // See MyCompiledObjectConstructor...
      }
    }

    /*********************************************/
    /*                                           */
    /* FUNCTIONS USED BY THE TOKEN CONVERTOR */
    /*                                           */
    /*********************************************/

    void FATAL(const char *ErrorMessage)
    {
      puts(ErrorMessage);
      MessageBox(HWND_DESKTOP, ErrorMessage, "FATAL SCRIPT ERROR", MB_OK);
    }

    bool isid(char c)
    {
      return c == '_' || isalnum(c);
    }

    bool isidstart(char c)
    {
      return c == '_' || isalpha(c);
    }

    bool iswhitespace(char c)
    {
      return c && (c <= ' '); // chars between 0x01 and 0x20 are considered whitespace
    }

    bool isspecial(char c)
    {
      switch(c)
      {
        case '(':
        case '<':
        case '{':
        case '[':
        case ')':
        case '>':
        case '}':
        case ']':
        case '=':
        case ':':
        case ';':
        case '!':
        case ',':
        case '*':
        case '/':
        case '%':
        case '+':
        case '-':
        case '^':
        case '&':
        case '|':
                  return true;
      }
      return false;
    }

    char *GetRawScriptToken(char *Walker, char *Target, bool ObfuscateTarget)
    {
      // left trim
      while(iswhitespace(*Walker))
        Walker ++;

      // end reached?
      if(*Walker == '\0')
      {
        memset(Target, 0, 8);
        return Walker;
      }
      const char cWalker = *Walker;
      const char *Next   = Walker + 1;
      const char cNext   = *Next;

      // const?
      if(isdigit(cWalker))
      {
        int size = 0;
        if(cWalker != '0')
          size = 10;
        else if(isspecial(cNext) || iswhitespace(cNext))
          size = 10;
        else
        {
          switch(cNext)
          {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9': size = 10;
                      break;
            case 'd': size = 10;
                      Walker += 2;
                      break;
            case 'b': size = 2;
                      Walker += 2;
                      break;
            case 'o': size = 8;
                      Walker += 2;
                      break;
            case 'x': size = 16;
                      Walker += 2;
                      break;
          }
          if(size == 0)
            FATAL("INVALID CONST");      
        }
        char *TempTarget = Target;
        while(*Walker && !iswhitespace(*Walker) && !isspecial(*Walker))
        {
          char Modifier = 0;
          int isvalid = 0;
          switch(size)
          {
            case 2:  isvalid = (*Walker >= '0' && *Walker <= '1');
                     break;
            case 8:  isvalid = (*Walker >= '0' && *Walker <= '7');
                     break;
            case 10: isvalid = (*Walker >= '0' && *Walker <= '9');
                     break;
            case 16: isvalid = (*Walker >= '0' && *Walker <= '9');
                     if(!isvalid && (*Walker >= 'A' && *Walker <= 'F'))
                     {
                       isvalid = 1;
                       Modifier = 'A' - '0' - 10;
                     }
                     if(!isvalid && (*Walker >= 'a' && *Walker <= 'f'))
                     {
                       isvalid = 1;
                       Modifier = 'a' - '0' - 10;
                     }
                     break;
          }
          if(!isvalid)
            FATAL("INVALID CHARACTER IN CONST");
          *TempTarget ++ = (*Walker ++) - Modifier;
        }
        *TempTarget = '\0';
        register const unsigned int copied = TempTarget - Target;
        TempTarget = Target;
        unsigned int constant = 0;
        if(size == 10)
        {
          constant = atoi(TempTarget);
        }
        else
        {
          int shifter = size == 2 ? 1 : size == 8 ? 3 : 4;
          while(*TempTarget != '\0')
          {
            constant = (constant << shifter) | (*TempTarget - '0');
            TempTarget ++;
          }
        }

        if(PrintConstantConversions)
          printf("Create Constant: %d,%d,%s-->%u\n", size, copied, Target, constant);

        if(ObfuscateTarget)
        {
          *(unsigned short *)(Target + 0) = 0x188F;
          *(unsigned int   *)(Target + 2) = constant;
        }
        else
          if(DebugOutput) sprintf(Target, "%u", constant);
        return Walker;
      }
      else if(cWalker == '"')
      {
        // Copy until "
        do
        {
          if(*Walker < ' ')
            FATAL("Invalid character in string constant");
          *Target ++ = *Walker ++;
        }
        while(*Walker != '\0' && *Walker != '"');
        {
          if(*Walker == '\0')
            FATAL("EOL in string constant");
          Walker ++;
        }
        *Target ++ = '"';
        *Target    = '\0';
        return Walker;
      }
      else if(cWalker == '/' && cNext == '/')
      {
        char *ClearFrom = Walker;

        // Scan until EOL
        Walker += 2;
        while(*Walker != '\0' && *Walker != '\n' && *Walker != '\r')
          Walker ++;

        // Replace the comment by whitespace
        // NOTE: this is needed to easily implement the BackwardsScanForSM_LPAREN code
        while(ClearFrom < Walker)
          *ClearFrom ++ = ' ';

        // We must return a valid token
        return GetRawScriptToken(Walker, Target, ObfuscateTarget);
      }
      else if(cWalker == '/' && cNext == '*')
      {
        char *ClearFrom = Walker;

        // Scan until */
        Walker += 2;
        while(*Walker != '\0' && *(Walker + 0) != '*' && *(Walker + 1) != '/')
          Walker ++;
        if(*Walker)
          Walker += 2;

        // Replace the comment by whitespace
        // NOTE: this is needed to easily implement the BackwardsScanForSM_LPAREN code
        while(ClearFrom < Walker)
          *ClearFrom ++ = ' ';

        // We must return a valid token
        return GetRawScriptToken(Walker, Target, ObfuscateTarget);
      }
      else if(isidstart(cWalker))
      {
        // Copy as long as we have a valid character
        char *TempTarget = Target;
        while(*Walker != '\0' && isid(*Walker))
          *TempTarget ++ = *Walker ++;
        *TempTarget = '\0';

    #pragma warn -8008  // Condition is always false
        // Some id's are keywords, detect those
        char PossibleByte = -1;
        if(0);
        else if(!strcmp(Target, "int") || !strcmp(Target, "integer"))
          PossibleByte = 0x1D;
        else if(!strcmp(Target, "string") || !strcmp(Target, "str"))
          PossibleByte = 0x1E;
        else if(!strcmp(Target, "ustring") || !strcmp(Target, "ust"))
          PossibleByte = 0x1F;
        else if(!strcmp(Target, "loc") || !strcmp(Target, "location"))
          PossibleByte = 0x20;
        else if(!strcmp(Target, "obj") || !strcmp(Target, "object"))
          PossibleByte = 0x21;
        else if(!strcmp(Target, "list"))
          PossibleByte = 0x22;
        else if(!strcmp(Target, "void"))
          PossibleByte = 0x23;
        else if(!strcmp(Target, "if"))
          PossibleByte = 0x25;
        else if(!strcmp(Target, "else"))
          PossibleByte = 0x26;
        else if(!strcmp(Target, "while"))
          PossibleByte = 0x28;
        else if(!strcmp(Target, "for"))
          PossibleByte = 0x2A;
        else if(!strcmp(Target, "continue"))
          PossibleByte = 0x2C;
        else if(!strcmp(Target, "break"))
          PossibleByte = 0x2D;
        else if(!strcmp(Target, "switch"))
          PossibleByte = 0x2F;
        else if(!strcmp(Target, "case"))
          PossibleByte = 0x31;
        else if(!strcmp(Target, "default"))
          PossibleByte = 0x32;
        else if(!strcmp(Target, "return"))
          PossibleByte = 0x33;
        else if(!strcmp(Target, "scriptvar") || !strcmp(Target, "shared") || !strcmp(Target, "member"))
          PossibleByte = 0x36;
        else if(!strcmp(Target, "function") || !strcmp(Target, "implement"))
          PossibleByte = 0x34;
        else if(!strcmp(Target, "on") || !strcmp(Target, "trigger"))
          PossibleByte = 0x35;
        else if(!strcmp(Target, "include") || !strcmp(Target, "inherits"))
          PossibleByte = 0x37;
        else if(!strcmp(Target, "prototype") || !strcmp(Target, "extern") || !strcmp(Target, "forward"))
          PossibleByte = 0x38;
        if(PossibleByte != -1)
        {
          if(PossibleByte == 0x35)
          {
            // NOTE: the following code will not work if you put a { in a comment before the actual {
            bool  isPostName = false;
            bool  ClearMode  = false;
            char *Current    = Walker;
            char *End        = Walker;
            while(*End && *End != '{')
            {
              if(iswhitespace(*End))
                *End = ' ';
              End ++;
            }
            if(DebugOutput) printf("TRIGGER BEFORE: $35%.*s\n", End - Walker, Walker);
            while(Current < End)
            {
              if(ClearMode)
              {
                if(*Current == ')')
                  ClearMode = false;
                *Current = ' ';
              }
              else
              {
                if(*Current == '(')
                  if(isPostName)
                  {
                    ClearMode  = true;
                    *Current = ' ';
                  }
                  else
                    *Current = ' ';
                if(*Current == '<')
                  if(isPostName)
                    *Current = '(';
                  else
                    *Current = ' ';
                if(*Current == '>')
                  if(isPostName)
                    *Current = ')';
                  else
                    *Current = ' ';
                if(!isPostName)
                {
                  char CurrentAsToken[384];
                  GetRawScriptToken(Current, CurrentAsToken, false);
                  if(ConvertEventNameToIndex(CurrentAsToken) >= 0)
                    isPostName = true;
                }
              }
              Current ++;
            }
            if(DebugOutput) printf("TRIGGER AFTER:  $35%.*s\n", End - Walker, Walker);
          }

          char NextToken[384];
          if(PossibleByte >= 0x1D && PossibleByte <= 23) // id's are types if followed by a name
            GetRawScriptToken(Walker, NextToken, false);
          else if(PossibleByte == 0x25 || PossibleByte == 0x28 || PossibleByte == 0x2F)
          {
            GetRawScriptToken(Walker, NextToken, false);
            if(!strcmp(NextToken, "("))
              NextToken[0] = '_';
            else
              NextToken[0] = '0'; // if/for/while/switch are names if not followed by (
          }
          else if(PossibleByte == 0x31 || PossibleByte == 0x32)
          {        
            if(PossibleByte == 0x31)
              GetRawScriptToken(GetRawScriptToken(Walker, NextToken, false), NextToken, false);
            else
              GetRawScriptToken(Walker, NextToken, false);
            if(!strcmp(NextToken, ":"))
              NextToken[0] = '_';
            else
              NextToken[0] = '0'; // case/default are names if not followed by :
          }
          else
            NextToken[0] = '_'; // 

          if(isidstart(*NextToken))
          {
            // Convert the token to an obfuscated script byte
            if(ObfuscateTarget)
              *(unsigned short *)Target = ObfuscateScriptByte(PossibleByte);
          }
          else
          {
            printf("Usage of an keyword as a variable: %s\n", Target);
          }
        }

        return Walker;
      }
    #pragma warn +8008  // Condition is always false

    #pragma warn -8008  // Condition is always false
      // Convert the token to an obfuscated scriptbyte
      Target[0] = cWalker;
      Target[1] = '\0';
      char Byte = -1;
      if(0);
      else if(strncmp(Walker, "++", 2) == 0)
      {
        strcat(Target, "+");
        Walker ++;
        Byte = 0x1B;
      }
      else if(strncmp(Walker, "--", 2) == 0)
      {
        strcat(Target, "-");
        Walker ++;
        Byte = 0x1C;
      }
      else if(strncmp(Walker, "&&", 2) == 0)
      {
        strcat(Target, "&");
        Walker ++;
        Byte = 0x18;
      }
      else if(strncmp(Walker, "||", 2) == 0)
      {
        strcat(Target, "|");
        Walker ++;
        Byte = 0x19;
      }
      else if(strncmp(Walker, "!=", 2) == 0)
      {
        strcat(Target, "=");
        Walker ++;
        Byte = 0x12;
      }
      else if(strncmp(Walker, "==", 2) == 0)
      {
        strcat(Target, "=");
        Walker ++;
        Byte = 0x11;
      }
      else if(strncmp(Walker, "<=", 2) == 0)
      {
        strcat(Target, "=");
        Walker ++;
        Byte = 0x15;
      }
      else if(strncmp(Walker, ">=", 2) == 0)
      {
        strcpy(Target, ">");
        Walker ++;
        Byte = 0x16;
      }
      else switch(*Walker)
      {
        case ':': if(!ObfuscateTarget)
                    return Walker + 1;
                  return GetRawScriptToken(Walker + 1, Target, true);
        case '(': Byte = 0x02; break;
        case ')': Byte = 0x03; break;
        case '{': Byte = 0x07; break;
        case '}': Byte = 0x08; break;
        case '[': Byte = 0x09; break;
        case ']': Byte = 0x0A; break;
        case ',': Byte = 0x04; break;
        case ';': Byte = 0x06; break;
        case '!': Byte = 0x0B; break;
        case '+': Byte = 0x0C; break;
        case '-': Byte = 0x0D; break;
        case '*': Byte = 0x0E; break;
        case '/': Byte = 0x0F; break;
        case '%': Byte = 0x10; break;
        case '^': Byte = 0x1A; break;
        case '<': Byte = 0x13; break;
        case '>': Byte = 0x14; break;
        case '=': Byte = 0x17; break;
      }
    #pragma warn +8008  // Condition is always false

      if(Byte == -1)
        FATAL("Invalid token!");

      // Convert the token to an obfuscated script byte
      if(ObfuscateTarget)
        *(unsigned short *)Target = ObfuscateScriptByte(Byte);

      return ++ Walker;
    }

    int BackwardsScanForSM_LPAREN(char *Walker)
    {
      if(*Walker == '(')
        return 1;
      if(*Walker != '"')
        return 0;
      while(*--Walker != '"');
      return 0;
    }

    /**********************************************************************/
    /*                                                                    */
    /* THE FOLLOWING MyXXX FUNCTIONS INTERCEPT DEMO CORE SCRIPT FUNCTIONS */
    /*                                                                    */
    /*                                                                    */
    /**********************************************************************/

    typedef unsigned int (* __cdecl __LoadFileByCode)(unsigned int DirectoryCode, char *ScriptNameWithM, const char *OpenMode);
    __LoadFileByCode LoadFileByCode = (__LoadFileByCode) 0x480877;

    // This function is and must be declared naked!!!
    // Do not modify this function unless you know enough assembler
    // And also unless you know deep down in your heart what "naked" functions are
    int __declspec(naked) MyCompiledObjectConstructor()
    {
      __asm
      {
         // WARNING! This also relies on the PI_Compiler_CompiledObjectSize patch!!!
         //          Failure to apply that patch will cause unusual behavior
         mov byte ptr [ecx + 0x54], 0 // Is UOC (2) or M (1) or Unknown (0) file ?
         mov byte ptr [ecx + 0x55], 0 // A free to use extra byte, currently unused
         mov eax, 0x426E20
         jmp eax
      }
    }

    char * __stdcall MyGetRawScriptToken(char *Walker, char *Target)
    {
      if(!*Walker)
        return NULL;
      if(GetCurrentScriptType() == 0)
        SetCurrentScriptType(IsObfuscatedScriptByte(* (unsigned short *) Walker) ? 1 : 2);
      if(GetCurrentScriptType() == 1)
        return NULL;
      return GetRawScriptToken(Walker, Target, true);
    }

    unsigned int __cdecl MyLoadScript(const unsigned int DirectoryCode, char *ScriptNameWithM, const char * const OpenMode)
    {
      unsigned int RetCode;

      // NOTE: do not modify the DirectoryCode and/or OpenMode
      RetCode = LoadFileByCode(DirectoryCode, ScriptNameWithM, OpenMode);
      if(RetCode)
      {
        if(DebugOutput) printf("%s -> M-file\n", ScriptNameWithM);
        return RetCode;
      }

      char NewFileName[MAX_PATH];
      strcpy(NewFileName, "..\\scripts.uosl\\");
      strcat(NewFileName, ScriptNameWithM);
      strcpy(NewFileName + strlen(NewFileName) - 1, "uosl");

      RetCode = LoadFileByCode(DirectoryCode, NewFileName, OpenMode);
      if(RetCode)
      {
        if(DebugOutput) printf("%s -> UOC-file\n", NewFileName);
        return RetCode;
      }
  
      //
      if(DebugOutput) printf("%s > Failure!\n", NewFileName);
      return NULL; 
    }

    int __cdecl MyBackwardsScanForSM_LPAREN(char *Walker, int)
    {
      if((GetCurrentScriptType() == 0) || (GetCurrentScriptType() == 1))
      {
        int _EAX;
        // Use the demo's code to handle the M files
        __asm
        {
          push 2
          push Walker
          mov  eax, 0x42B1B0
          call eax
          add  esp, 8
          mov _EAX, eax
        }
        return _EAX;
      }

      return BackwardsScanForSM_LPAREN(Walker);
    }

    PATCHINFO PI_Compiler_CompiledObjectSize =
    {
      0x426EF4,
     2, {0x6A, 0x54},
     2, {0x6A, 0x56},
    };

    PATCHINFO PI_Compiler_CompiledObjectConstructor =
    {
      0x426F1A,
     5, {0xE8, 0x01, 0xFF, 0xFF, 0xFF},
     5, {0xE8, 0x00, 0x00, 0x00, 0x00},
    };

    PATCHINFO PI_Intercept_LoadScript =
    {
      0x426217,
     5, {0xE8, 0x5B, 0xA6, 0x05, 0x00},
     5, {0XE8, 0x00, 0x00, 0x00, 0x00},
    };

    PATCHINFO PI_Intercept_GetRawScriptToken =
    {
      0x428407,
     19, {0x66, 0xB9, 0x00, 0x00, 0x66, 0x39, 0x08, 0x75, 0x05, 0xE8, 0xEB, 0xF5, 0x0B, 0x00, 0xE8, 0xC6, 0x87, 0x19, 0x00},
     19, {0xFF, 0x75, 0x0C, 0xFF, 0x75, 0x08, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x09, 0xC0, 0x0f, 0x85, 0xBF, 0x01, 0x00, 0x00},
    };

    PATCHINFO PI_Intercept_HandleBackwardsScanForSM_LPAREN =
    {
      0x42A2F9,
     11, {0x6A, 0x02, 0x8B, 0x55, 0x0C, 0x52, 0xE8, 0xAC, 0x0E, 0x00, 0x00},
     11, {0x6A, 0x02, 0x8B, 0x55, 0x0C, 0x52, 0xE8, 0x00, 0x00, 0x00, 0x00},
    };


    void Initialize_jit()
    {
        // Prepare the patches
        SetRel32_AtPatch(&PI_Compiler_CompiledObjectConstructor, MyCompiledObjectConstructor);
        SetRel32_AtPatch(&PI_Intercept_LoadScript, MyLoadScript);
        SetRel32_AtRelPatch(&PI_Intercept_GetRawScriptToken, 6, MyGetRawScriptToken);
        SetRel32_AtRelPatch(&PI_Intercept_HandleBackwardsScanForSM_LPAREN, 6, MyBackwardsScanForSM_LPAREN);

        // Apply the patches
        Patch(&PI_Compiler_CompiledObjectSize);
        Patch(&PI_Compiler_CompiledObjectConstructor);
        Patch(&PI_Intercept_LoadScript);
        Patch(&PI_Intercept_GetRawScriptToken);
        Patch(&PI_Intercept_HandleBackwardsScanForSM_LPAREN);
    }
}
