import sublime
import sublime_plugin
import urllib.request
import threading
import json
import os
import sys
import subprocess
import locale

AC_OPTS = sublime.INHIBIT_WORD_COMPLETIONS | sublime.INHIBIT_EXPLICIT_COMPLETIONS


class KKultureCompletion( sublime_plugin.EventListener):
    def on_query_completions(self, view, prefix, locations):
        pos = locations[0]
        scopes = view.scope_name(pos).split()
        if "source.cs" not in scopes:
            return []
        documenttext = view.substr(sublime.Region(0,view.size()))
        params = dict()
        params["position"] = pos
        params["document"] = documenttext
        data=json.dumps(params).encode('utf-8')
        try:
            request = urllib.request.Request("http://localhost:5000/complete", data=data, headers={"User-Agent": "Sublime","Content-type":"application/json"}, method= "POST")
            http_file = urllib.request.urlopen(request, timeout=5)
            resp = http_file.read().decode('utf-8')
            temp = """{"val":"""+resp+"""}"""
            temp = json.loads(temp)['val']
            completions = []
            for i in temp:
                t = (i[0],i[1])
                completions.append(t)
            print(completions)
            return completions
        except Exception as e:
            print(e)
            return []
