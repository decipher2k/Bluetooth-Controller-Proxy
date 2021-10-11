# Bluetooth Controller Proxy
Allows you to use your Bluetooth gamepad on your PC via your mobile phone and WiFi or USB tethering. (so you don't need a Bluetooth dongle)<br>
<br>
Connection handling is still a mess. Just like the sourcecode (don't look at it! you have been warned!) <br>
You will have to keep the following order to get the client connected:<br>
(this will be changed in the future)<br>
<br>
-Stop both the client (Android) and the server (Windows) if they are running.<br>
-Start the server at the Windows PC<br>
-Start the Android client<br>
<br>
(Basically, the Android client should be started after the server)<br>
This procedure has to be repeated after each IP change, and when one of the programs gets closed.<br>

FIFA 22 can be played ok via wifi 5ghz, but for games without input prediction, you should use USB tethering.<br>
Whe using USB tethering, you should disable mobile data on your mobile phone, or it could drain your mobile data contingent!<br>
<br>
When installing the android apk, a warning about the author of the app being unknow will be shown. That's because it uses a self-signed certificate.<br>
If you get asked to upload the app for scanning, please do so! It helps me getting it trusted by Google.<br>


<br>
Technical stuff:<br>
There is no serverside prediction<br>
The connection is not secured (an attacker who has infiltrated your network could fake gamepad input at your pc.)<br>
The axis are being transmitted using UDP (package loss won't matter, but speed does)<br>
The buttons are being transmitted using TCP (they are mandatory, and a loss of button press would be fatal)
