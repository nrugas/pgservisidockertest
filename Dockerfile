# defaultni dockerfile za wcf
FROM microsoft/wcf:4.7.1-windowsservercore-ltsc2016
WORKDIR /inetpub/wwwroot
COPY ${source:-obj/Docker/publish} .

