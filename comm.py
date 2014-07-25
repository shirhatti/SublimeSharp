import socket
import json
import types

class CompletionService():
    def __init__(self):
        self.cache = ''
        self.skt =socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    def __del__(self):
        try:
            self.skt.close()
        except:
            pass
        
    def start(self):
        self.skt.connect(('127.0.0.1',8888))
        self.skt.settimeout(2)

    def simple_complete(self, position, document_text):

        msg = {
            'HostId': 'testHost',
            'MessageType': 'SimpleFileCompletionMessage',
            'ContextId': 3,
            'Payload': {
                'DocumentText': document_text,
                'SuggestionPosition': position
            }
        }
        
        data = bytes(json.dumps(msg) + '\n','utf-8')

        self.skt.send(data)

        self.cache =''
        response=""
        completions = []
        while True:
            try:
                chunk = self.skt.recv(1024)
                if not chunk:
                    break
                chunk = chunk.decode('utf-8')
                delim_pos = chunk.find('\n')
                if delim_pos != -1:
                    self.cache += chunk[0:delim_pos]
                    response=self.cache[:]
                    self.cache = chunk[delim_pos:]
                    break
                else:
                    self.cache += chunk
            except Exception as e:
                break
        if len(response) == 0:
            return completions
        raw=json.loads(response)
        if not raw['Payload'] or raw['MessageType'] != 'CompletionSuggestionsMessage':
            return completions
        for entry in raw['Payload']['Suggestions']:
            print(entry)
            completions.append((entry['DisplayName'],entry['Snippet']))
        return completions

