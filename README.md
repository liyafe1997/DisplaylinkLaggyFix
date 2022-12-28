# Background
DisplayLink Windows driver uses a User-Mode process to render graphics to the device. As default, the process CPU priority is NORMAL. So, when the CPU load is high, the DisplayLink graphics will become laggy.

In Windows 10 and above, the DisplayLink driver use [WUDFHost.exe] to load its driver. The dll be loaded is [dlidusb3.dll] or [dlidusb2.dll] (From the name, seems it will load different dll when the DisplayLink device is connected to different USB Bus, USB2.0/USB3.0).

In Windows 7/8/8.1, the process is [DisplayLinkManager.exe], no [dlidusbx.dll].

This program will automatically find the DisplayLink process and set the priority to high. It uses WMI to listening a specific process starts (In Windows 10/11, it is WUDFHost.exe), then will check the dll with specific name prefix is loaded or not. Finally it will set the process priority to HIGH.

# Usage
Be careful, please use the 64-bit (x64) version in 64-bit Windows. (32-bit version does not work in 64-bit OS because 32-bit process can not access 64-bit process in 64-bit Windows)

The first argument is the process name to be looked for (without .exe). The second argument is the dll name prefix to be looked for.

In Windows 10+, you can just run it without any arguments. it will set [WUDFHost] as the process name and [dlidusb] as the dll name prefix.

In Windows 7/8/8.1, you can run:
```
DisplaylinkLaggyFix_x64.exe DisplayLinkManager DisplayLinkManager
```

It is suggested to run this program by Windows's Task Scheduler, set the trigger is when computer is started. And set the runner user is set to [SYSTEM].
