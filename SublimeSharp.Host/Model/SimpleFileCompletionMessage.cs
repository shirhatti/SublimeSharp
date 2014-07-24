using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SublimeSharp.Host.Model
{
    public class SimpleFileCompletionMessage
    {
        public int SuggestionPosition {get; set;}
        public string DocumentText {get; set;}
    }
}