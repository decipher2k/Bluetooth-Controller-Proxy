# Bluetooth Controller Proxy
Allows you to use your Bluetooth gamepad on your PC via your mobile phone and WiFi or USB tethering. (so you don't need a Bluetooth dongle)<br>
Nefarius.ViGEm is used for XBox 360 controller emulation.<br>
<br>
The sourcecode is still a messs. (don't look at it! you have been warned!) <br>
<br>
Installation:<br>
1.) Install the android app<br>
https://techcult.com/install-apk-using-adb-commands/<br>
2.) Unzip the Windows server at your gaming PC<br>
3.) Install the controller driver which can be found in the file "Bluetoot Controller Proxy.zip".<br>
<br>
Usage:<br>
1.) Run the android app<br>
2.) Run the Windows server<br>
3.) Map the buttons by clicking on them in the Windows server, followed by pressing the according button on the controller<br>
4.) (optional) Connect your mobile using USB tethering and disable mobile data<br>
https://www.geeksforgeeks.org/what-is-usb-tethering-and-how-to-enable-it/<br>
https://www.businessinsider.com/how-to-turn-off-cellular-data-on-android<br>
5.) Enter your computer's IP at the android app and press "set".<br>
https://www.businessinsider.com/how-to-find-ip-address-on-windows<br>
6.) Play :D<br>
<br>
<br>
Update:
There is now an auto reconnect. No need for proper connection handling anymore.

FIFA 22 can be played ok via wifi 5ghz, but for games without input prediction, you should use USB tethering.<br>
Whe using USB tethering, you should disable mobile data on your mobile phone, or it could drain your mobile data contingent!<br>
<br>
When installing the android apk, a warning about the author of the app being unknow will be shown. That's because it uses a self-signed certificate.<br>
If you get asked to upload the app for scanning, please do so! It helps me geting it trusted by Google.<br>


<br>
Technical stuff:<br>
There is no serverside prediction<br>
The connection is not secured (an attacker who has infiltrated your network could fake gamepad input at your pc.)<br>
The axis are being transmitted using UDP (package loss won't matter, but speed does)<br>
The buttons are being transmitted using TCP (they are mandatory, and a loss of button press would be fatal)
