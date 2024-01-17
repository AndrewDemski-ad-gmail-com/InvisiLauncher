# InvisiLauncher

| Builds | 
| ----- | 
| [![Builds](https://github.com/AndrewDemski-ad-gmail-com/InvisiLauncher/actions/workflows/build-dotnet-desktop.yml/badge.svg)](https://github.com/AndrewDemski-ad-gmail-com/InvisiLauncher/actions/workflows/build-dotnet-desktop.yml)|





## a simple wrapper for powershell on Windows systems.
Main process is launched without window by setting the ProcessStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
I am not adding the infamous -WIndowstyle Hidden switch because PS console may still blink.
Luancher does not create a window and its subprocess will not generate window unless PS1 script brings own window up.

## How it works
1. InvisiLauncher.exe looks for configuration file in own folder (named InvisiLauncher.config)
Configuration file is a simple XML file with two nodes:
- filename
it expands ```%Windir%\System32\WINDOWSPOWERSHELL\v1.0\powershell.exe``` into ```C:\WINDOWS\System32\WINDOWSPOWERSHELL\v1.0\powershell.exe```\
- arguments
it expands from formatted template ```-ExecutionPolicy Bypass -sta -noprofile -File {0}``` 

Together those two will define what we will launch when process is executed.

> **we expect path to PS1 file on 1st position, at least in default configuration in this project**.

> **path to PS1 file can be relative to location of launcher/executable file**.

if we want to launch a powershell script with no additional parameters, then invoke launcher with path to the script.\
F.ex. ```launcher.exe C:\path\to\scriptfile.ps1```\
That will translate into ```powershell.exe -ExecutionPolicy Bypass -sta -noprofile -File C:\path\to\scriptfile.ps1```

If we want to launch powershell script with additional parameters, then
- add them in commandline\
F.ex. ```launcher.exe C:\path\to\scriptfile.ps1 -some other -required arguments```\
it will translate into ```powershell.exe -ExecutionPolicy Bypass -sta -noprofile -File C:\path\to\scriptfile.ps1 -some other -required arguments```

- or define them in xml configuration file where those will be expanded using string format.
by changing ```-ExecutionPolicy Bypass -sta -noprofile -File {0}``` to sth like ```-ExecutionPolicy Bypass -sta -noprofile -File {0} -additional parameters``` 

If for any reason text values cannot be expanded, launcher will use default ones specified above and continue with passed script/script+parameters.
If everything goes well, launcher will return exit code of powershell.exe it just launched.

> I think about adding 2nd command line parameter set. 
It should be UNC/URI parameter leading to config file.
That way it would be possible to manage the action without changing configuration file placed in installdir, I expect it to be ready-only.

## error codes
all problems are reported into Debug Stream. I may add file logger if project gets traction

| errorcode | description |
|-----|-----|
| -1 | configuration file is missing! We need it |
| -2 | problem when navigating XML file (configuration), check Debug stream |
| -3 | somethng went wrong during initialization of our custom process (the hidden one, it could be parent of anything), check Debug stream |
