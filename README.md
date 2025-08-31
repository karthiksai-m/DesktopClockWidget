#### Desktop Clock Widget



##### Description



* A minimal desktop clock widget for Windows, built in C# WPF (.NET 9.0).
* Displays large time and small date
* Click-through / transparent window.
* 12-hour or 24-hour format supported.
* Optional Run-at-Startup toggle via F9



##### Features



* Dynamic positioning: Default top-center, configurable
* Self-contained MSI installer: Includes EXE, DLLs, and settings



##### Installation



* Run the DesktopClockInstaller.msi
* Follow the installer wizard: choose install folder, create shortcuts
* Launch via Desktop or Start Menu shortcut



##### Usage



* F8: Toggle click-through mode
* F9: Toggle auto-start at Windows startup



##### Development Setup



* Open the solution in Visual Studio 2022/2023
* Build the clock project (Release)
* Add / verify Primary Output in the Setup Project
* Rebuild Setup Project to generate MSI
