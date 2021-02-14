UOSL Language Package for Visual Studio 2010 v2.0
=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

Features:
	Active UOSL source parsing displays syntax and grammar errors as you work.
	Supports multiple language extensions (UOSL Extended, Enhanced and Native)
	Intellisense QuickInfo, Statement Completion and Parameter info:
		Keywords, local and inherited variables and functions
		Full library of core functions
		Trigger definitions
	Automatic line indenting.
	Navigate to inherited files with a triple click on the inherits line.
	Visual brace matching
	Visual highlighting of selected identifiers within scope.
	Text colorization for keywords and constants.

Requirements:
	Visual Studio 2010.
		Express editions cannot be supported currently.
	.Net 3.5
	A decent workstation:
		This extension does real-time parsing of UOSL source files. 
			If you see a lot of hourglasses, and VS stops responding frequently, please send me your hardware specifications.

Install Instructions:
	Uninstall First:
		If you installed an earlier release of the UOSL Extensions using a Setup.exe program, uninstall this software via Add/Remove programs.
		If you installed an earlier version using a vsix package, uninstall the existing version From the Tools\Extension Manager menu in VS2010
	Double click UOSLpkg.vsix to install this extension to Visual Studio.
	Restart Visual Studio

Version 2.0.0.0: Release Date 5/23/2011
  Known issues:
	On an reported error parsing an inherits file, each reporting file may need to be closed and reopened, or edited to clear the error condition after the error in the offending file is corrected.
	The code context menu items "Go To Reference", "Go To Declaration" and "Go To Definition" do not do anything.
	Friendly names for core function parameters are not yet available
	In order for parameter help to pop up in an existing function invocation after typing (, put a space after the ( before you type it. This is not an issue for commas.
	Quick Info does not handle the scope of the objects under the cursor, for example when hovering over "callback" you will see the definition or both the core function and the trigger declaration

Version 1.0.0.0: Release date 4/10/2011
  Known issues:
	Do not use.

More information:
	http://uodemo.joinuo.com/index.php?title=UOSL_Language_Package

Project Credits:
	Batlin: batlin@joinuo.com
		Project Founder; UOSL Language specification; Divination; Demo SDK; and many other even more important things too numerous and mysterious to list.
	Derrick: derrick@joinuo.com
		UOSL Parser and VS2010 language extension.