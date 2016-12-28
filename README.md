# Slam
Slam : A new approach to (dependency) injection!!!

Intro and Part 1 of Slam - https://www.youtube.com/watch?v=VjNTbzEPE88
Part 2 of Slam - https://www.youtube.com/watch?v=9n6936cBCWk


I got tired of the amount of uncessary code that modern day injection models require, and non-essential interfaces that make it difficult for the developer to navigate and debug code.  

This effort is largely thanks to Jerry Wang http://www.codeproject.com/Articles/463508/NET-CLR-Injection-Modify-IL-Code-during-Run-time who created the most difficult part.

Slam is similar to DI or Mock frameworks except that the code is simply replaced at run time. And it allows for replacement and execution of sealed and private methods otherwise not accessible.

# Requirements
Before you get started, this is the currently required environment:
-Visual Studio 2013 (you might try 2015, but it might not work)
-MVC Classes (these don't always get installed; re-run setup by using Uninstall a Program feature in Windows and select Change.  Check the MVC Classes box and install
-Visual Studio Unit Tests for C# (probaby installed but obtainable through Visual Studio by select Tools / Exentions and Updates
-Visual Studio Exentsibility - (not a hard requirement, but you won't get Visaualizations)

# Getting Started
It is going to take a while for me to write up the full functionality, so here is the easiest way to get started.  Buid the entire solution.  Then select Test / Windows / Test Explorer.  Examine each of the Unit Tests their.  Place a break point at the start of each unit test and debug one at a time.  To understand just the basics, step OVER the injector, e.g. Injector.SlamClass(typeof(MySourceClass), typeof(MyMockClass)).  It will detract from the initial core learning.  Do examine each of the classes (such as MySourceClass) to understand better what is going on.

Lastly, there is one necessary change to make.  The RunSealed tests currently requires a hard path to the unittest dll.  Make this modification to reflectc your local build environment.  @"C:\Users\Patrick\Documents\GitHub\Slam\bin\Slam.UnitTests.Classes.dll"

Please leave all comments in issues on this site.  Enjoy!

-Patrick Dengler


