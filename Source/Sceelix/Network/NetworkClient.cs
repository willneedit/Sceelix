﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Sceelix.Network
{
    public delegate void TcpClientEventHandler();

    public delegate void TcpClientErrorHandler(Exception ex);

    public delegate void TcpClientRawMessageEventHandler(string message);

    public delegate void TcpClientTokenMessageEventHandler(JToken token);

    public delegate void TcpClientParsedMessageEventHandler(string message, object data);

    public class NetworkClient
    {
        private readonly Queue<Action> _actions = new Queue<Action>();
        private readonly TcpClient _client;

        private readonly NetworkStream _clientConnection;
        private readonly StreamReader _clientStreamReader;
        private readonly StreamWriter _clientStreamWriter;

        private readonly Timer _pingTimer;
        private readonly Thread _processMessageThread;
        private int _pingPeriod;



        public NetworkClient(string hostname, int port, int pingPeriod = 3000)
        {
            _client = new TcpClient(hostname, port);

            _pingPeriod = pingPeriod;

            _clientConnection = _client.GetStream();
            _clientStreamReader = new StreamReader(_clientConnection);
            _clientStreamWriter = new StreamWriter(_clientConnection);

            _processMessageThread = new Thread(ProcessMessages) {Name = "ProcessMessages", IsBackground = true};
            _processMessageThread.Start();

            _pingTimer = new Timer(state => OnPing(), null, 0, pingPeriod);
        }



        /// <summary>
        /// Indicates if the client is connected to the server.
        /// </summary>
        public bool Connected => _client != null && _client.Connected;



        /// <summary>
        /// Time interval between server pings. 
        /// Required to understand if the connection is active. 
        /// Default is 3000ms.
        /// </summary>
        public int PingPeriod
        {
            get { return _pingPeriod; }
            set
            {
                if (value != _pingPeriod)
                {
                    _pingPeriod = value;

                    _pingTimer.Change(0, _pingPeriod);
                }
            }
        }



        public event Action ClientDisconnected;



        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Disconnect()
        {
            _clientStreamWriter.Close();
            _clientStreamReader.Close();
            _clientConnection.Close();
            _client.Close();
            _processMessageThread.Abort();
            _pingTimer.Dispose();

            if (ClientDisconnected != null)
                ClientDisconnected();
        }



        //delegates to inform of various connection events
        public event Action<Exception> ErrorOccurred;



        private void OnPing()
        {
            //TcpClient / NetworkStream does not get notified when the connection is closed.
            //IOControlCode.KeepAliveValues does not work in Unity
            //The only option available is to catch exceptions when writing to the stream.
            //so we send this message periodically (which should just be ignored on the other side) to figure 
            //out if the connection is active
            if (Connected)
                SendRawMessage(string.Empty);
        }



        public event Action<string, object> ParsedMessageReceived;



        private void ProcessMessages()
        {
            try
            {
                while (true)
                {
                    string rawMessageString = _clientStreamReader.ReadLine();

                    //discard empty messages or pings here
                    if (!string.IsNullOrEmpty(rawMessageString))
                        try
                        {
                            if (RawMessageReceived != null)
                                RunOnMainThread(() => RawMessageReceived(rawMessageString));

                            if (TokenMessageReceived != null)
                            {
                                JToken jToken = JToken.Parse(rawMessageString);
                                RunOnMainThread(() => TokenMessageReceived(jToken));
                            }

                            if (ParsedMessageReceived != null)
                            {
                                NetworkMessage message = NetworkMessage.Deserialize(rawMessageString);
                                RunOnMainThread(() => ParsedMessageReceived(message.Subject, message.Data));
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ErrorOccurred != null)
                                ErrorOccurred.Invoke(ex);
                        }
                }
            }
            //when the connection is closed, this exception will be thrown, ending the threaded function
            catch (IOException)
            {
                if (ClientDisconnected != null)
                    ClientDisconnected();
            }
        }



        public event Action<string> RawMessageReceived;



        private void RunOnMainThread(Action action)
        {
            lock (_actions)
            {
                _actions.Enqueue(action);
            }
        }



        /// <summary>
        /// Sends a message to the server in the form of a subject-data .
        /// Uses Json.NET to serialize the data. 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="data"></param>
        public void SendMessage(string subject, object data)
        {
            if (_client.Connected)
            {
                //string message = JsonConvert.SerializeObject(new TcpMessage(subject, data), Formatting.None);
                string message = new NetworkMessage(subject, data).Serialize();

                _clientStreamWriter.WriteLine(message);
                _clientStreamWriter.Flush();
            }
        }



        /// <summary>
        /// Sends a string message to the server without any kind of construction.
        /// </summary>
        /// <param name="message"></param>
        public void SendRawMessage(string message)
        {
            try
            {
                if (_client.Connected)
                {
                    _clientStreamWriter.WriteLine(message);
                    _clientStreamWriter.Flush();
                }
            }
            catch (SocketException)
            {
                Disconnect();
            }
        }



        /// <summary>
        /// Executes the events in the main thread.
        /// </summary>
        public void Synchronize()
        {
            lock (_actions)
            {
                while (_actions.Count > 0) _actions.Dequeue().Invoke();
            }
        }



        public event Action<JToken> TokenMessageReceived;
    }
}