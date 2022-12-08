# Based on https://stackoverflow.com/questions/48542644/python-and-windows-named-pipes
# Tianjiao Yang, 08.12.2022, Stuttgart

import time
import sys
import win32pipe, win32file, pywintypes
import numpy as np

# exchange_data = np.array([[11, 12, 5, 2], [15, 6, 10, 0], [10, 8, 12, 5]]);
exchange_data = np.random.rand(10,10)
r, c = exchange_data.shape
nr_data = exchange_data.size


def pipe_server():
    print("pipe server")
    pipe = win32pipe.CreateNamedPipe(
        r'\\.\pipe\icdca',
        win32pipe.PIPE_ACCESS_DUPLEX,
        win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE | win32pipe.PIPE_WAIT,
        1, 65536, 65536,
        0,
        None)
    try:
        print("waiting for client")
        win32pipe.ConnectNamedPipe(pipe, None)
        print("got client")
        print("communicating array shape")
        win32file.WriteFile(pipe, str.encode(str(r)))
        win32file.WriteFile(pipe, str.encode("\n"))
        time.sleep(1)
        win32file.WriteFile(pipe, str.encode(str(c)))
        win32file.WriteFile(pipe, str.encode("\n"))
        time.sleep(1)

        print("communicating array")
        i = 0
        while i < r:
            print(f"writing row {i}")
            # convert to bytes
            alsstr = ','.join(str(item) for item in exchange_data[i])
            win32file.WriteFile(pipe, str.encode(alsstr))
            win32file.WriteFile(pipe, str.encode("\n"))
            time.sleep(1)
            i += 1
        print("finished now")
    finally:
        win32file.CloseHandle(pipe)


def pipe_client():
    print("pipe client")
    quit = False
    arrrec = []

    while not quit:
        try:
            handle = win32file.CreateFile(
                r'\\.\pipe\acdci',
                win32file.GENERIC_READ | win32file.GENERIC_WRITE,
                0,
                None,
                win32file.OPEN_EXISTING,
                0,
                None
            )
            # Send instruction data to c# server
            win32file.WriteFile(handle, str.encode('transmitting data ... \n'))
            res = win32pipe.SetNamedPipeHandleState(handle, win32pipe.PIPE_READMODE_MESSAGE, None, None)
            if res == 0:
                print(f"SetNamedPipeHandleState return code: {res}")
            while True:
                # Receive data from Python client
                resp = win32file.ReadFile(handle, 64*1024)
                print("array data received: \n")
                print(resp[1].decode())
                temp_row = resp[1].decode().rstrip().split(",")
                arrrec.append([float(x) for x in temp_row])
        except pywintypes.error as e:
            if e.args[0] == 2:
                print("no pipe, trying again in a sec")
                time.sleep(2)
            elif e.args[0] == 109:
                print("broken pipe, now exit\n\n")
                quit = True
    print("The received data, repeat:\n")
    print(np.array(arrrec))


if __name__ == '__main__':
    if len(sys.argv) < 2:
        print("need s or c as argument")
    elif sys.argv[1] == "s":
        pipe_server()
    elif sys.argv[1] == "c":
        pipe_client()
    else:
        print(f"no can do: {sys.argv[1]}")