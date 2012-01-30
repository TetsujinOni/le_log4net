Logging To Logentries from AppHarbor using Log4net
========================================================

Simple Usage Example
----------------------


    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController).Name);
        
        public ActionResult Index()
        {
            log.Debug("Home page opened");
            
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            
            log.Warn("This is a warning message");
            
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
    
------------------------

To configure Log4Net, you will need to perform the following:

    * (1) Obtain your Logentries account key.
    * (2) Setup Log4Net (if you are not already using it).
    * (3) Configure the Logentries Log4Net plugin.

To obtain your Logentries account key you must download the getKey exe from github at:

    https://github.com/downloads/logentries/le_log4net/getkey.zip
    
This user-key is essentially a password to your account and is required for each of the steps listed below. To get the key unzip the file you download and run the following from the command line:

    getKey.exe --key

You will be required to provide your user name and password here. Save the key as you will need this later on. 

Log4net Setup
------------------

If you don't already have Log4net set up in your project, we suggest using Nuget.

Instructions to do so can be found at   http://nuget.org/List/Packages/log4net

Logentries Log4net Plugin Setup
--------------------------------

Now you need to get the Logentries Plugin Library, we suggest using Nuget for this as well.

The package is found at https://nuget.org/List/Packages/le_log4net

If you're not using Nuget, the library can be downloaded from:

https://github.com/downloads/logentries/le_log4net/le_log4net.dll

It will need to be referenced in your project.

LoggerConf
------------------

Create an xml file called loggerConf.xml in your project with the following

or you can find it at :  https://github.com/logentries/le_log4net/blob/master/loggerConf.xml
 
    <?xml version="1.0"?>
      <log4net>
        <appender name="LeAppender" type="log4net.Appender.LeAppender">
          <Key value="YOUR-USER-KEY-HERE" />
          <Location value="YOUR-LOG-DESTINATION-HERE" />
          <Debug value="true" />
          <layout type="log4net.Layout.PatternLayout">
            <param name="ConversionPattern" value="%d{ddd MMM dd HH:mm:ss zzz yyyy} %: %level%, %m" />
          </layout>
        </appender>
        <root>
          <level value="ALL" />
          <appender-ref ref="LeAppender" />
        </root>
      </log4net>

Using the current version of the appender on AppHarbor through their addon:

    <?xml version="1.0"?>
      <log4net>
        <appender name="LogEntries" type="log4net.Appender.LeAppender">
          <Key value="$AppSetting{LOGENTRIES_ACCOUNT_KEY}" />
          <Location value="$AppSetting{LOGENTRIES_LOCATION}" />
          <Debug value="true" />
          <layout type="log4net.Layout.PatternLayout">
            <param name="ConversionPattern" value="%date %: [%thread] %-5level %logger [%property{NDC}]- %message" />
          </layout>
        </appender>
        <root>
          <level value="DEBUG" />
          <appender-ref ref="LogEntries" />
        </root>
      </log4net>

You would then need to add AppSettings entries for the add-on configuration variables used in the configuration.

In this file you will enter your user-key as obtained above with the getKey script in the required
Key value.

You must also include in the required Location value the name of your host and logfile on Logentries

in the following format:        `hostname/logname.log`

Now place the following line in your `AssemblyInfo.cs` file:

    [assembly: log4net.Config.XmlConfigurator(ConfigFile="loggerConf.xml", Watch = true)]

In your `global.asax` file, enter the following using directives if you have not
done so already:

    using log4net;
    using log4net.Config;

Finally enter the following line in `Application_Start():`

    log4net.Config.BasicConfigurator.Configure();

Logging Messages
----------------

With that done, you are ready to send logs to Logentries.

In each class you wish to log from, enter the following using directives at the top if not already there:

    using log4net;
    using log4net.Config;

Then create this object at class-level:

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(your_class_name_here).Name);

Be sure to enter the name of current class in the indicated brackets above.

What this does is create a logger with the name of the current class for
clarity in the logs.

Now within your code in that class, you can log using log4net as normal and it
will log to Logentries.

Example:

    log.Debug("Debugging Message");
    log.Info("Informational message");
    log.Warn("Warning Message");

