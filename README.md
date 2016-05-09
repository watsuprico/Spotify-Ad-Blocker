# Spotify Ad Blocker 
###### Version 1.0
![image](https://cloud.githubusercontent.com/assets/10912095/15102620/8dc5c3e0-156f-11e6-9662-fd902dd60dce.png)
> An overview of Spotify Ad Blocker

<hr/>
## What is Spotify Ad Blocker?
Spotify Ad Blocker allows you to run a proxy server that you port Spotify's traffic threw. In doing this, you are able to block some are all sites that Spotify is trying to connect with. By default the whitelist only allows Spotify's image sites, music streaming, and reconnecting to Spotify. The blacklist, (while not necessary when using whitelist), blocks known ad sites Spotify will try to connect to.
<hr/>
### How can I use Spotify Ad Blocker?
Setting up Spotify Ad Blocker is easy.
- Goto the release page, (https://github.com/watsuprico/Spotify-Ad-Blocker/releases/), and download the latest release. ![image](http://i.imgur.com/QZPSZsw.png)
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

#### Full customization
![image](https://cloud.githubusercontent.com/assets/10912095/15102921/58a941ce-1573-11e6-983c-eb0ca007987d.png)
> Edit any setting you would like.

- Change startup settings, proxy settings, and more.
 - *You can also change the theme settings inside the config file*

<hr>
#### Monitor Spotify connections
![image](https://cloud.githubusercontent.com/assets/10912095/15102926/721f8244-1573-11e6-8d1b-ba628c39303b.png)
> Monitor the connections via the console.

All connections to and from Spotify are logged inside the console.

<hr>
#### Material design
Uses https://github.com/ButchersBoy/MaterialDesignInXamlToolkit to gain a material design look.

<hr>
#### Out of the way
![sab](https://cloud.githubusercontent.com/assets/10912095/15102992/25408abc-1574-11e6-880e-216ee0056a37.png)
> System tray icon controls

Start Spotify Ad Blocker and just minimize it to the system tray and forget about it.

<hr>
#### ~~System wide Ad blocking~~
~~Spotify Ad Blocker can be applied system wide to block any site in the blacklist or not in the whitelist.
*Please note this is experimental, and is no way what Spotify Ad Block is intended for.*~~





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
Site | Spotify's use for it | Needed? | Necessary? 
------------- | ------------- | ------------- | -------------
scdn.co | Images and profile info | It makes it look nice - Yes | Unless you like photos, no.
fbcdn.net | Images and profile info | Read above ^ - Yes | ^
apresolve.spotify.com | Used to reconnect to Spotify - Yes | Not really.
-- Known for ads -- | ------------- | No | No.
heads-fab.spotify.com | Audible ads | No
audio-**xx**.spotify.com | Audible ads | No.
cloudfront.net | Non-skipable audiovisual popup ads | No.
pubmatic.com | The ads on the bottom of Spotify | No.
googlesyndication.com | General ads | No.
googleadservices.com | General ads | No.
googletagservices.com | General ads | No.
ravenjs.com | General ads | No.
gvt1.com | General ads | No.
-- Unknown, but not essential -- | ------------- | No.
audio-ex.spotify.com | Unknown, but may be audible ads | No effect without it.

Other blocked ad sites (it'd be smart to block them):
```
ads.yahoo.com
agkn.com
rlcdn.com
adnxs.com
doubleclick.net
fastclick.net
googlesyndication.com
googleadservices.com
googletagservices.com
```

<hr/>


## Editing the white list or the black list
All list check to see if that line x is in the current connection and base their actions off that 
##### Whitelist
If the current connection does **not** contain any of the addresses in the whitelist file, it will block it.
For example, if I try to connect to google.com, but that is not in a line inside the whitelist file, it will block that connection.
However, if ".com" is inside the white list, any site with ".com" is not blocked.
If any connection includes x line in it, the whitelist will not block it.
<hr/>
##### Blacklist
If the current connection **does** contain any of the addresses in the blacklist file, it will block it. *(The opposite of the whitelist)*
*(Blacklisted sites are disconnected before checking if they are whitelisted. ((If a blacklisted site is on the whitelist, it'll still get blocked)).)* 
For example, if I try to connect to dropbox.com, but that is a line inside the whitelist file, it will block that connection.
However, for say, if dropbox.com was not in the blacklist file, then it will not be blocked.
If any connection includes x line in it, the blacklist will block it.

<hr/>

## Building and compiling
- You'll need NuGet and VisualStudio
 - Open *SpotifyAdBlocker.sln*
 - Build *(CTRL + Shift + B)*
 - Copy allow files from **/SpotifyAdBlocker/include/** to **/SpotifyAdBlocker/bin/Debug/** *(where the .exe is located)*
![image](https://cloud.githubusercontent.com/assets/10912095/15103573/c428d8c2-157a-11e6-85b5-6f1f0adf8b3f.png)


That's it
