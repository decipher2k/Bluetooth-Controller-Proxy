# Bluetooth Controller Proxy
Allows you to use your Bluetooth gamepad on your PC via your mobile phone and WiFi or USB tethering. (so you don't need a Bluetooth dongle)<br>
<br>
Connection handling is still a mess. Just like the sourcecode (don't look at it! you have been warned!) <br>
You will have to keep the following order to get the client conntected:<br>
<br>
-Stop both the client (Android) and the server (Windows) if they are running.<br>
-Start the server at the Windows PC<br>
-Start the Android client<br>
<br>
(Basically, the Android client should be started after the server)<br>
This procedure has to be repeated after each IP change.<br>
FIFA 2022 can be played ok via wifi 5ghz, but for games without input prediction, you should use USB tethering.<br>
This procedure has to be repeated after each IP change.<br>

<br>
Technical stuff:<br>
There is no serverside prediction<br>
The axis are being transmitted using UDP (package loss won't matter, but speed does)<br>
The buttons are being transmitted using TCP (they are mandatory, and a loss of button press would be fatal)
