# InvisiLauncher

## a simple wrapper for powershell on Windows systems.

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

If for any reason text values cannot be expanded, launcher will use default ones specified above.
