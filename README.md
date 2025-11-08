## ⚠️If you publish an executor with SapiV or modify it, don’t forget to give me credits! I really appreciate it.⚠️

# SapiV
**(Saturn Api Velocity)**

SapiV is an Api based on Velocity, open source and easy to use, allowing anyone to quickly create their exploits quickly, securely, in a safe way.
It may have bugs or issues. If you'd like to help fix them, open a ticket on our Discord server: https://discord.gg/PHWymfWpr4

- UNC: 99%
- Level: 8
- Only works on roblox web version

## SapiV doesn't work on WinForm .Net framework.
Use WinForms .Net or WPF

# Functions

SapiV.Api.Inject();

SapiV.Api.Execute();

SapiV.Api.IsRobloxOpen();

SapiV.Api.IsInjected();

SapiV.Api.KillRoblox();

SapiV.Api.SetCustomInjectionNotification(string title, string text, string idIcon, string duration)

SapiV.Api.SetCustomUserAgent(string Name);

SapiV.Api.SetCustomNameExecutor(string Name, string Version);

SapiV.Api.EnableConsole = bool;

# Examples

SapiV.Api.SetCustomInjectionNotification("Test executor", "Injected successfully!", "93547137238535", "3");

SapiV.Api.SetCustomNameExecutor("Test executor", "v1.0.0");

SapiV.Api.SetCustomUserAgent("Test executor");

SapiV.Api.EnableConsole = false;

If you don't use these customization functions, or if you want to leave a parameter empty, default values will be applied. To leave a parameter empty for example, the icon, use an empty string like this: "".
These functions go in the constructor.

## For the custom icon you must use an image, not a decal!!

# Credits

Velocity

https://discord.gg/mEgfFaTy6A

https://getvelocity.lol/
