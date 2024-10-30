# OpenWrt Install

opkg update
opkg list | grep dotnet

scp -r /path/to/your/project root@<your_router_ip>:/root/TelegramBot

cd /root/TelegramBot
dotnet build

dotnet run
