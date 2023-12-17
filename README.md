# InvisiLauncher

## a simple wrapper for powershell on Windows systems.
Main process is launched without window by setting the ProcessStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
I am not adding the infamous -WIndowstyle Hidden switch because PS console may still blink.
Luancher does not create a window and its subprocess will not generate window unless PS1 script brings own window up.

## How it works
1. InvisiLauncher.exe looks for configuration file in own folder (named InvisiLauncher.config)
Configuration file is a simple XML file with two nodes:
- filename
- arguments

filename expands ```%Windir%\System32\WINDOWSPOWERSHELL\v1.0\powershell.exe``` into ```C:\WINDOWS\System32\WINDOWSPOWERSHELL\v1.0\powershell.exe```\
\
arguments expands from formatted template ```-ExecutionPolicy Bypass -sta -noprofile -File {0}``` into full command line.
> **we expect PS1 file on 1st position**.

if we want to launch a powershell script with no additional parameters, then invoke launcher with path to the script.\
F.ex. ```launcher.exe C:\path\to\scriptfile.ps1```\
That will translate into ```powershell.exe -ExecutionPolicy Bypass -sta -noprofile -File C:\path\to\scriptfile.ps1```

If we want to launch powershell script with additional parameters, then add them in commandline\
F.ex. ```launcher.exe C:\path\to\scriptfile.ps1 -some other -required arguments```\
it will translate into ```powershell.exe -ExecutionPolicy Bypass -sta -noprofile -File C:\path\to\scriptfile.ps1 -some other -required arguments```

If for any reason text values cannot be expanded, launcher will use default ones specified above and continue with passed script/script+parameters.
If everything goes well, launcher will exit with exit code of powershell.exe it launched.

> I think about adding 2nd command line parameter set. 
It should be UNC/URI parameter lading to config file.
That way it would be possible to manage the action without changing configuration file placed in installdir, I expect it to be ready-only.

## error codes
all problems are reported into Debug Stream. I may add file logger if project gets traction

| errorcode | description |
|-----|-----|
| -2 | args[] is null or empty! We need arguments in commandline, I cannot launch raw invisible PS console |
| -3 | number of arguments was acceptable (>0), most likely indicates a problem with XML configuration file |
