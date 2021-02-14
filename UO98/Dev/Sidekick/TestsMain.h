#pragma once

#include "Classes.h"

extern LocationObject location;
extern LocationObject locationPlus1;
extern LocationObject locationPlus8;

void InitTestLocations(void);

void InitializeTests();

void OnTestResult(bool passed, char* message, ...);

void Tests_Lists_Execute(void);
void Tests_World_Execute(void);
void Tests_ObjectVars_Execute(void);
void Tests_ObjectScripts_Execute(void);
void Tests_Classes_Execute(void);

void DoTests();

