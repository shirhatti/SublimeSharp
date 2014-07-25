import sublime
import sublime_plugin
import urllib.request
import threading
import json
import os
import sys
import subprocess
import locale
import traceback
from Test.comm import CompletionService

AC_OPTS = sublime.INHIBIT_WORD_COMPLETIONS | sublime.INHIBIT_EXPLICIT_COMPLETIONS

completion_server = CompletionService()
completion_server.start()

class KKultureCompletion( sublime_plugin.EventListener ):
    def on_query_completions(self, view, prefix, locations):
        global completion_server
        pos = locations[0]
        scopes = view.scope_name(pos).split()
        if "source.cs" not in scopes:
            return []
        documenttext = view.substr(sublime.Region(0,view.size()))
        try:
            return completion_server.simple_complete(pos,documenttext)
        except Exception as e:
            print(e)
            return []
