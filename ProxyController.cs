using System;
using System.IO;
using System.Linq;
using System.Net;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace SpotifyAdBlocker
{
    public class ProxyController
    {
        public IPAddress connectorIP = IPAddress.Parse(Properties.Settings.Default["proxyIP"].ToString());
        public int connectorPort = int.Parse(Properties.Settings.Default["proxyPort"].ToString());
        public bool serverRunning = false;

        private string blockMessage = "<h1>This site has been blocked by Spotify Ad Blocker.</h1>";
        private bool whiteList = true;
        private bool blackList = true;

        private MainWindow main = new MainWindow();

        private bool discard = false;

        public ExplicitProxyEndPoint explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Parse(Properties.Settings.Default["proxyIP"].ToString()), int.Parse(Properties.Settings.Default["proxyPort"].ToString()), true) { };

        public void StartProxy()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                ProxyServer.BeforeRequest += OnRequest;
                ProxyServer.BeforeResponse += OnResponse;

                connectorIP = IPAddress.Parse(Properties.Settings.Default["proxyIP"].ToString());
                connectorPort = int.Parse(Properties.Settings.Default["proxyPort"].ToString());

                explicitEndPoint = new ExplicitProxyEndPoint(connectorIP, connectorPort, true)
                {
                    // ExcludedHttpsHostNameRegex = new List<string>() { "spotify.com" }
                };

                ProxyServer.AddEndPoint(explicitEndPoint);
                ProxyServer.Start();

                foreach (var endPoint in ProxyServer.ProxyEndPoints)
                    Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ", endPoint.GetType().Name, endPoint.IpAddress, endPoint.Port);

                loadSettings(true);
                serverRunning = true;
            }
            catch (Exception a)
            {
                if (!Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"])) System.Windows.MessageBox.Show(a.ToString(), "Alert!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
            }
        }

        public void loadSettings(bool output)
        {
            if (output) Console.WriteLine("Loading settings...");
            if (Convert.ToBoolean(Properties.Settings.Default["EnableBlackList"].ToString())) { if (output) Console.WriteLine("Black listing enabled."); blackList = true; }
            else { if (output) { Console.ForegroundColor = ConsoleColor.DarkRed; Console.WriteLine("Black listing disabled!"); Console.ForegroundColor = ConsoleColor.Gray; } blackList = false; }
            if (Convert.ToBoolean(Properties.Settings.Default["EnableWhiteList"].ToString())) { if (output) Console.WriteLine("White listing enabled."); whiteList = true; }
            else { if (output) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("White listing disabled!"); Console.ForegroundColor = ConsoleColor.Gray; } whiteList = false; }
            if (Convert.ToBoolean(Properties.Settings.Default["CustomBlock"])) { if (output) Console.WriteLine("Custom block message enabled."); if (File.Exists(Environment.CurrentDirectory + "/block.html")) blockMessage = File.ReadAllText(Environment.CurrentDirectory + "/block.html"); else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("Can't enable custom block message! File not found."); Console.ForegroundColor = ConsoleColor.Gray; blockMessage = "<h1>This site has been blocked by Spotify AD Blocker</h1><br/><a href='http://watsuprico.com/'>Watsuprico.com</a>"; } }
            else { if (output) Console.WriteLine("Custom block message disabled."); blockMessage = "<h1>This site has been blocked by Spotify AD Blocker</h1>"; };
        }

        public void Stop()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Shutting down...");
            Console.ForegroundColor = ConsoleColor.Gray;
            ProxyServer.BeforeRequest -= OnRequest;
            ProxyServer.BeforeResponse -= OnResponse;
            if (serverRunning) { ProxyServer.RemoveEndPoint(explicitEndPoint); ProxyServer.Stop(); }
            Console.WriteLine("Stopped");
            serverRunning = false;
        }

        public void Restart()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Restarting...");
            Console.ForegroundColor = ConsoleColor.Gray;
            ProxyServer.BeforeRequest -= OnRequest;
            ProxyServer.BeforeResponse -= OnResponse;
            if (serverRunning) { ProxyServer.RemoveEndPoint(explicitEndPoint); ProxyServer.Stop(); }
            serverRunning = false;
            StartProxy();
            Console.WriteLine("Restarted.");
        }

        public void OnRequest(object sender, SessionEventArgs e)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Request to: {0}", e.ProxySession.Request.RequestUri.Host);

                //read request headers
                var requestHeaders = e.ProxySession.Request.RequestHeaders;
                if ((e.RequestMethod.ToUpper() == "POST" || e.RequestMethod.ToUpper() == "PUT"))
                {
                    //Get/Set request body bytes
                    byte[] bodyBytes = e.GetRequestBody();
                    e.SetRequestBody(bodyBytes);

                    //Get/Set request body as string
                    string bodyString = e.GetRequestBodyAsString();
                    e.SetRequestBodyString(bodyString);
                }
                discard = false;

                //Blacklist
                if (blackList)
                {
                    int counter = 0;
                    string line;
                    System.IO.StreamReader blacklist = new System.IO.StreamReader(Environment.CurrentDirectory + "/blacklist");
                    while ((line = blacklist.ReadLine()) != null)
                    {
                        if (e.ProxySession.Request.RequestUri.AbsoluteUri.Contains(line))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("[Blacklist] Blocked site (request): {0}", e.ProxySession.Request.RequestUri.Host);
                            e.Ok(blockMessage);
                            discard = true;
                            //return;
                        }
                        counter++;
                    }
                    blacklist.Close();
                }


                //Whitelist
                if (whiteList && !discard)
                {
                    string line2;
                    int counter2 = 0;
                    int count = 0;
                    System.IO.StreamReader whitelist = new System.IO.StreamReader(Environment.CurrentDirectory + "/whitelist");
                    while ((line2 = whitelist.ReadLine()) != null)
                    {
                        if (!e.ProxySession.Request.RequestUri.AbsoluteUri.Contains(line2)) // Site is not allowed
                            count++;
                        counter2++;
                    }
                    whitelist.Close();
                    if (count == System.IO.File.ReadLines(Environment.CurrentDirectory + "/whitelist").Count())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[Whitelist] Blocked site (response): {0}", e.ProxySession.Request.RequestUri.Host);
                        e.Ok(blockMessage);
                        //return;
                    }
                }
            }
            catch (Exception b)
            {
                if (!Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"])) System.Windows.MessageBox.Show(b.ToString(), "Alert!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Exclamation);
            }
        }

        public void OnResponse(object sender, SessionEventArgs e)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Reponse from: {0}", e.ProxySession.Request.RequestUri.Host);

                var responseHeaders = e.ProxySession.Response.ResponseHeaders;
                if (e.RequestMethod == "GET" || e.RequestMethod == "POST")
                {
                    if (e.ProxySession.Response.ResponseStatusCode == "200")
                    {
                        if (e.ProxySession.Response.ContentType.Trim().ToLower().Contains("text/html"))
                        {
                            byte[] bodyBytes = e.GetResponseBody();
                            e.SetResponseBody(bodyBytes);

                            string body = e.GetResponseBodyAsString();
                            e.SetResponseBodyString(body);
                        }
                    }
                }
                discard = false;

                // Blacklist
                if (blackList)
                {
                    int counter = 0;
                    string line;
                    System.IO.StreamReader blacklist = new System.IO.StreamReader(Environment.CurrentDirectory + "/blacklist");
                    while ((line = blacklist.ReadLine()) != null)
                    {
                        if (e.ProxySession.Request.RequestUri.AbsoluteUri.Contains(line))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("[Blacklist] Blocked site (response): {0}", e.ProxySession.Request.RequestUri.Host);
                            e.Ok(blockMessage);
                            discard = true;
                            //return;
                        }
                        counter++;
                    }
                    blacklist.Close();
                }

                //Whitelist
                if (whiteList && !discard)
                {
                    string line2;
                    int counter2 = 0;
                    int count = 0;
                    System.IO.StreamReader whitelist = new System.IO.StreamReader(Environment.CurrentDirectory + "/whitelist");
                    while ((line2 = whitelist.ReadLine()) != null)
                    {
                        if (!e.ProxySession.Request.RequestUri.AbsoluteUri.Contains(line2)) // Site is not allowed
                            count++;
                        counter2++;
                    }
                    whitelist.Close();
                    if (count == System.IO.File.ReadLines(Environment.CurrentDirectory + "/whitelist").Count())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[Whitelist] Blocked site (response): {0}", e.ProxySession.Request.RequestUri.Host);
                        e.Ok(blockMessage);
                        //return;
                    }
                }
            }
            catch (Exception c)
            {
                if (!Convert.ToBoolean(Properties.Settings.Default["DismissWarnings"])) System.Windows.MessageBox.Show(c.ToString(),"Alert!",System.Windows.MessageBoxButton.OK,System.Windows.MessageBoxImage.Exclamation);
            }
        }
    }
}