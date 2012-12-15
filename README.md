QLog - quick logging for Windows Azure .NET applications
========================================================
QLog is very lightweight and simple to use logging framework designed to work with Windows Azure cloud applications. It comes with QLogBrowser that allows you to view and filter all logs. QLog allows not only to log events on popular areas (levels) like TRACE, DEBUG, INFO or ERROR, but also to provide your own custom areas. Follow the documentation in Docs folder to learn more about this project. The best way to start using it is to get it via NuGet package manager.

Sample usage
------------

Once you set up logger, you can use following syntax to start logging inside your code:

```csharp
QLog.Logger.LogDebug("Sample debug message");
QLog.Logger.Log<QSampleArea>("Sample area message");
```

All logs are being stored on Azure Table storage so you will need to set up Azure storage account or use emulator.