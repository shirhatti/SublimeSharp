import socket
import json
import types

msg = {
    'HostId': 'testHost',
    'MessageType': 'SimpleFileCompletionMessage',
    'ContextId': 3,
    'Payload': {
        'DocumentText': 'using System.',
        'SuggestionPosition': 13
    }
}
data = bytes(json.dumps(msg) + '\n','utf-8')

skt =socket.socket(socket.AF_INET, socket.SOCK_STREAM)
skt.connect(('127.0.0.1',8888))
skt.send(data)
data = skt.recv(1024)
skt.close()
print('Received', repr(data))

