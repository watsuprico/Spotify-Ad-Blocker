# Spotify Ad Blocker 

## Project currently deprecated.
Might update in the future, but unlikely.
Although, I know there **many** changes needed

###### Version 1.0
![image](https://cloud.githubusercontent.com/assets/10912095/15102620/8dc5c3e0-156f-11e6-9662-fd902dd60dce.png)


<hr/>
## What is Spotify Ad Blocker?
Spotify Ad Blocker allows you to run a proxy server that you port Spotify's traffic through. In doing this, you are able to block some or all sites that Spotify is trying to connect with. By default, the whitelist only allows Spotify's image sites, music streaming, and reconnecting to Spotify. The blacklist, (while not necessary when using whitelist), blocks known ad sites Spotify will try to connect to.
<hr/>
### How can I use Spotify Ad Blocker?
Setting up Spotify Ad Blocker is easy.
- Goto the [release page](https://github.com/watsuprico/Spotify-Ad-Blocker/releases/) and [download the latest release](https://github.com/watsuprico/Spotify-Ad-Blocker/releases/latest/). ![image](http://i.imgur.com/QZPSZsw.png)
- Startup Spotify Ad Blocker
  - Install the Titanium Root Certificate Authority (for the proxy)
![cert](https://cloud.githubusercontent.com/assets/10912095/15103907/d46d8dd2-157e-11e6-9557-b2bafae100b3.png)

  - Open Spotify and goto Preferences > Proxy settings (under advnace settings) and change the proxy settings to match the ones in Spotify Ad Blocker *(by default: Type: HTTP | Host: 127.0.0.1 | Port: 80)*.
![image](https://cloud.githubusercontent.com/assets/10912095/15103468/cbd34946-1579-11e6-959a-0e00f7bc1368.png)

 - Restart Spotify
 - And you're done!

**Please note Spotify Ad Blocker needs administrator privileges.**
<hr/>

## Spotify Ad Blocker Features

##### Fully customizable
![image](https://cloud.githubusercontent.com/assets/10912095/15102921/58a941ce-1573-11e6-983c-eb0ca007987d.png)

Spotify Ad Blocker allows you to change *almost* any setting you could think about.
<hr>


##### Monitor Spotify connections
![image](https://cloud.githubusercontent.com/assets/10912095/15102926/721f8244-1573-11e6-8d1b-ba628c39303b.png)

You can monitor connections made by Spotify by easily opening the console and viewing them.
<hr>

##### Material design
Spotify Ad Blocker was built utilizing a [material desgin look](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit) to be more user friendly.
<hr>

##### Out of the way
![sab](https://cloud.githubusercontent.com/assets/10912095/15102992/25408abc-1574-11e6-880e-216ee0056a37.png)

You can change the settings to allow starting up to the system tray to just run and forget about Spotify Ad Blocker.
<hr>

##### Not only for Spotify
You can use this to tunnel any program you want through this proxy to manage and monitor their connections. *(But if you are using this as a system-wide ad blocker, let me teach you about a little thing called a [host file](google.com/search?q=host+file). It'll work better as a system-wide ad blocker)*



<hr/>


## Common errors
#### Why are some ads playing?
This could be due to the configuration of Spotify Ad Blocker or Spotify has the ads cached
- Try enabling only white listing and disabling the custom block message.
- Try clearing the cache folder.
- Try redownloading the [blacklist](https://github.com/watsuprico/Spotify-Ad-Blocker/blob/master/include/blacklist) and [whitelist](https://github.com/watsuprico/Spotify-Ad-Blocker/blob/master/include/whitelist)

<hr/>
#### Spotify is not connecting
This is usually because Spotify Ad Blocker is not running *(and Spotify is set to connect to it)* or a misconfiguration in the proxy settings.
- Ensure that the proxy settings in Spotify match with those in Spotify Ad Blocker
- Make sure Spotify Ad Blocker and the proxy is running 
 -*duh*
If you are going to turn off the proxy or Spotify Ad Blocker, make sure you disable connecting to a proxy in Spotify.

<hr/>
#### I changed settings about the proxy, but they're not working
Try restarting the proxy, and if that doesn't work restart Spotify Ad Blocker.

<hr/>
#### What is "System.FormatException: An invaild IP address was specified"?
This is when the IP setting is misconfigured.
Change the proxy ip to 127.0.0.1



<hr/>



## Known sites

Ad sites:
```
audio-sp-ash.spotify.com
audio-fa.spotify.com
audio-sp.spotify.com
audio-ec.spotify.com
heads-fab.spotify.com
ads.yahoo.com
agkn.com
rlcdn.com
adnxs.com
doubleclick.net
fastclick.net
pubmatic.com
googlesyndication.com
googleadservices.com
googletagservices.com
cloudfront.net
ravenjs.com
gvt1.com
```

Good sites:
```
scdn.co
fbcdn.net
apresolve.spotify.com
```

<hr/>


## Editing the white list or the black list
The white/black list works by checking one line at a time and seeing if the connection Spotify is attempting contains that line.
On the white list, if the connection does not contain any of the lines, then it is blocked.
The black list, however, if the connection contains any of the lines, then it is blocked.

For example, if a line in the whitelist was `spotify.com`, any connection going to `spotify.com` would be allowed. *(Like `something.anotherthing.spotify.com/beepbeep/boopboop/thingy`)*.
However, if that line was in the blacklist file, it would block any connection going to `spotify.com`.

They can also work on a wildcard bases. So if the whitelist had a line that contained `.com`, all `.com` sites would be allowed.

<hr/>

## Building and compiling
- You'll need NuGet and VisualStudio
 - Open *`SpotifyAdBlocker.sln`*
 - Build *(`CTRL + Shift + B`)*
 - Copy allow files from **/SpotifyAdBlocker/include/** to **/SpotifyAdBlocker/bin/Debug/** *(where the .exe is located)*
![image](https://cloud.githubusercontent.com/assets/10912095/15103573/c428d8c2-157a-11e6-85b5-6f1f0adf8b3f.png)


That's it
