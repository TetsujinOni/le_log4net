/*
   Logentries NLog Logging agent
   Copyright 2010,2011 Logentries, Jlizard
   Mark Lacomber <marklacomber@gmail.com>
                                            */


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Security;
using System.Net.Sockets;
using System.IO;

using log4net.Layout;
using log4net.Core;
using log4net.Util;
using log4net.Appender;

namespace log4net.Appender
{
    public class LeAppender : AppenderSkeleton
    {
        #region Private Instance Fields
        private SslStream sslSock = null;
        private TcpClient leSocket = null;
        private System.Text.ASCIIEncoding encoding;
        private String m_Key;  
        private String m_Location; 
        #endregion

        #region Public Instance Properties

        public string Key
        {
            get { return m_Key; }
            set { m_Key = value; }
        }

        public string Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }
        #endregion

        #region Constructor

        public LeAppender()
        {
        }

        #endregion

        private void createSocket(String key, String location)
        {
            this.encoding = new System.Text.ASCIIEncoding();
            this.leSocket = new TcpClient("api.logentries.com", 443);
            this.sslSock = new SslStream(this.leSocket.GetStream());

            this.sslSock.AuthenticateAsClient("logentries.com");

            String output = "PUT /" + key + "/hosts/" + location + "/?realtime=1 HTTP/1.1\r\n";
            this.sslSock.Write(this.encoding.GetBytes(output), 0, output.Length);
            output = "Host: api.logentries.com\r\n";
            this.sslSock.Write(this.encoding.GetBytes(output), 0, output.Length);
            output = "Accept-Encoding: identity\r\n";
            this.sslSock.Write(this.encoding.GetBytes(output), 0, output.Length);
            output = "Transfer_Encoding: chunked\r\n\r\n";
            this.sslSock.Write(this.encoding.GetBytes(output), 0, output.Length);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (this.sslSock == null)
            {
                try
                {
                    this.createSocket(this.Key, this.Location);
                }
                catch (SocketException e)
                {
                    Console.Error.WriteLine("Error connecting to Logentries");
                    Console.Error.WriteLine(e.ToString());
                }
                catch (IOException e)
                {
                    Console.Error.WriteLine("Error connecting to Logentries");
                    Console.Error.WriteLine(e.ToString());
                }
            }

            String final = RenderLoggingEvent(loggingEvent) + "\r\n";

            try
            {
                this.sslSock.Write(this.encoding.GetBytes(final), 0, final.Length);
                Console.Error.WriteLine("Sent");
            }
            catch (SocketException e1)
            {
                Console.Error.WriteLine("Error sending log to logentries");
                Console.Error.WriteLine(e1.ToString());
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("Error sending log to Logentries");
                Console.Error.WriteLine(e.ToString());
            }
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            if (this.sslSock == null)
            {
                try
                {
                    this.createSocket(this.Key, this.Location);
                }
                catch (SocketException e)
                {
                    Console.Error.WriteLine("Error connecting to Logentries");
                    Console.Error.WriteLine(e.ToString());
                }
            }
            foreach (LoggingEvent logEvent in loggingEvents)
            {
                this.Append(logEvent);
            }
        }

        protected override bool RequiresLayout
        {
            get { return true; }
        }
    }
}