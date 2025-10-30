import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation as animation
import scipy.linalg as linalg
import re
import math
import socket
import time
import keyboard

sServer = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
host = 'xxx.xxx.xxx.xxx'
port = 8009
sServer.bind((host, port))
sServer.listen(1)
c, addr = sServer.accept()
print('Connected', addr)

while True:
    data = c.recv(1024).decode("utf-8")
    print(data)
    

# sClient.close()
sServer.close()

