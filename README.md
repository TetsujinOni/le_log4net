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

Before implementing this logger, you need to create an account on Logentries.
Once you have done this, you must download the getKey.py script on github which
is necessary for getting your user-key.  This user-key is essentially a password
to your account and is required for each of the steps listed below.

Once you have downloaded the script run it as follows `python getKey.rb --key`.
It will prompt you for your login credentials and then print out your user-key
which is needed in the steps below.

Log4net Setup
------------------

If you don't already have Log4net set up in your project, please follow these
steps:

Download Log4net from:

<http://archive.apache.org/dist/incubator/log4net/1.2.10/incubating-log4net-1.2.10.zip>

Retreive `log4net.dll` from 

    \incubating-log4net-1.2.10\log4net-1.2.10\bin\net\2.0\release\log4net.dll

and place it the bin folder of your project.

Then add a reference to this dll from within your project. This is done simply
by right-clicking References, Click Add Reference and locate the dll in your
project bin folder.

Logentries Log4net Plugin Setup
--------------------------------

The first file required is called LeAppender.cs and is available on github at

https://github.com/logentries/le_log4net/blob/master/LeAppender.cs

Add this file to your project as it is a plugin for Log4net to send logs to
Logentries.

LoggerConf
------------------

Create an xml file called loggerConf.xml in your project with the following

or you can find it at :  https://github.com/logentries/le_log4net/blob/master/loggerConf.xml
 
    <?xml version="1.0"?>
      <log4net>
        <appender name="LeAppender" type="log4net.Appender.LeAppender">
          <Key value="YOUR-USER-KEY-HERE" />
          <Location value="YOUR-LOCATION-HERE" />
          <layout type="log4net.Layout.PatternLayout">
            <param name="ConversionPattern" value="%d{ddd MMM dd HH:mm:ss zzz yyyy} %: %level%, %m" />
          </layout>
        </appender>
        <root>
          <level value="ALL" />
          <appender-ref ref="LeAppender" />
        </root>
      </log4net>

In this file you will enter your user-key as obtained above with the getKey script in the required
Key value.

You must also include in the required Location value the name of your host and logfile on Logentries

in the following format:        `localhost6/test.log`

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

