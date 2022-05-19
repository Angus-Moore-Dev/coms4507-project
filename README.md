# COMS4507-project
A project for COMS4507 @ UQ (Advanced Topics in Security).

## Daedalus/Blackwall C2 Server
Written in C# (WPF Framework), this is the command server. You just need Visual Studio to build it.

## Athena Nameserver
Written in Spring Boot (Java), this is the nameserver that provides a fixed static endpoint. 

## Icarus Bot
the actual bot that runs.  needs scapy, pywin32 and pyinstaller to work.

In order to compile it, run the following command:

pyinstaller -F --onefile --noconsole main.py
