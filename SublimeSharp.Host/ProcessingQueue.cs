using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using SublimeSharp.Host.Model;

namespace SublimeSharp.Host
{
    public class ProcessingQueue
    {
        private readonly List<Message> _queue = new List<Message>();
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        public ProcessingQueue(Stream stream)
        {
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream);
            _writer.AutoFlush = true;
        }

        public event Action<Message> OnReceive;

        public void Start()
        {
            Trace.TraceInformation("[ProcessingQueue]: Start()");
            new Thread(ReceiveMessages).Start();
        }

        public void Post(Message message)
        {
            lock (_writer)
            {
                Trace.TraceInformation("[ProcessingQueue]: Post({0})", message);
                var msg = JsonConvert.SerializeObject(message);
                msg += "\n";
                _writer.Write(msg);
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    var input = _reader.ReadLine();
                    if (input == null)
                    {
                        continue;
                    }
                    var message = JsonConvert.DeserializeObject<Message>(input);
                    Console.WriteLine(message);
                    Trace.TraceInformation("[ProcessingQueue]: OnReceive({0})", message);
                    if (message != null)
                    {
                        OnReceive(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}