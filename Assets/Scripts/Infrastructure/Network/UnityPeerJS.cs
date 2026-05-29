using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class UnityPeerJS
{
    public enum Channel
    {
        Reliable   = 0,  // events quan trọng: attack, die, spawn
        Unreliable = 1   // sync frame: position, rotation, state
    }

    private enum EventType
    {
        None             = 0,
        Initialized      = 1,
        Connected        = 2,
        Received         = 3,
        ConnClosed       = 4,
        PeerDisconnected = 5,
        PeerClosed       = 6,
        Error            = 7
    }

    public class Peer
    {
        private readonly Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();
        private readonly int _peerIndex;

        public Peer(string key, string id, string host, int port)
        {
            Init();
            _peerIndex = OpenPeer(key, id, host, port);
        }

        public event Action OnOpen;
        public event Action<IConnection> OnConnection;
        public event Action OnDisconnected;
        public event Action OnClose;
        public event Action<string> OnError;

        public void Pump()
        {
            EventType eventType;
            while ((eventType = (EventType)NextEventType(_peerIndex)) != EventType.None)
            {
                switch (eventType)
                {
                    case EventType.Initialized:
                    {
                        PopInitializedEvent(_peerIndex);
                        OnOpen?.Invoke();
                        break;
                    }
                    case EventType.Connected:
                    {
                        var size = PeekReceivedEventSize(_peerIndex);
                        var buffer = new byte[size];
                        var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        var connIndex = PopConnectedEvent(_peerIndex, pinnedBuffer.AddrOfPinnedObject(), size);
                        pinnedBuffer.Free();

                        var remoteId = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                        _connections[connIndex] = new Connection(this, connIndex, remoteId);

                        Debug.Log($"[PeerJS] Connected index={connIndex} remoteId={remoteId}");
                        OnConnection?.Invoke(_connections[connIndex]);
                        break;
                    }
                    case EventType.Received:
                    {
                        // Check which channel BEFORE popping
                        var channel = (Channel)PeekReceivedChannel(_peerIndex);

                        var size = PeekReceivedEventSize(_peerIndex);
                        var buffer = new byte[size];
                        var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        var connIndex = PopReceivedEvent(_peerIndex, pinnedBuffer.AddrOfPinnedObject(), size);
                        var str = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                        pinnedBuffer.Free();

                        if (_connections.TryGetValue(connIndex, out var conn))
                            conn.EmitOnData(str, channel);
                        break;
                    }
                    case EventType.ConnClosed:
                    {
                        var connIndex = PopConnClosedEvent(_peerIndex);
                        if (_connections.TryGetValue(connIndex, out var conn))
                            conn.EmitOnClose();
                        break;
                    }
                    case EventType.PeerDisconnected:
                    {
                        PopPeerDisconnectedEvent(_peerIndex);
                        OnDisconnected?.Invoke();
                        break;
                    }
                    case EventType.PeerClosed:
                    {
                        PopPeerClosedEvent(_peerIndex);
                        OnClose?.Invoke();
                        break;
                    }
                    case EventType.Error:
                    {
                        var buffer = new byte[256];
                        var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        PopErrorEvent(_peerIndex, pinnedBuffer.AddrOfPinnedObject(), buffer.Length);
                        pinnedBuffer.Free();
                        OnError?.Invoke(DecodeUtf8Z(buffer));
                        break;
                    }
                    default:
                        PopAnyEvent(_peerIndex);
                        break;
                }
            }
        }

        public void Connect(string remoteId)
        {
            UnityPeerJS.Connect(_peerIndex, remoteId);
        }

        public void Disconnect() => PeerDisconnect(_peerIndex);
        public void Destroy()    => PeerDestroy(_peerIndex);

        private string DecodeUtf8Z(byte[] buffer)
        {
            var str = Encoding.UTF8.GetString(buffer);
            var nullIdx = str.IndexOf('\0');
            return nullIdx >= 0 ? str.Substring(0, nullIdx) : str;
        }

        // ─────────────────────────────────────────────
        // IConnection
        // ─────────────────────────────────────────────

        public interface IConnection
        {
            Peer   Peer     { get; }
            string RemoteId { get; }

            /// <summary>Data from reliable channel (events)</summary>
            event Action<string> OnReliableData;

            /// <summary>Data from unreliable channel (frame sync)</summary>
            event Action<string> OnUnreliableData;

            event Action OnClose;

            /// <summary>Send important event — guaranteed delivery</summary>
            void Send(string str);

            /// <summary>Send frame state — fast, may drop, no retry</summary>
            void SendUnreliable(string str);

            void Close();
        }

        private class Connection : IConnection
        {
            private readonly int _connIndex;

            public Connection(Peer peer, int connIndex, string remoteId)
            {
                Peer      = peer;
                _connIndex = connIndex;
                RemoteId  = remoteId;
            }

            public event Action<string> OnReliableData;
            public event Action<string> OnUnreliableData;
            public event Action OnClose;

            public Peer   Peer     { get; set; }
            public string RemoteId { get; set; }

            public void Send(string str)
            {
                UnityPeerJS.Send(Peer._peerIndex, _connIndex, str, str.Length);
            }

            public void SendUnreliable(string str)
            {
                UnityPeerJS.SendUnreliable(Peer._peerIndex, _connIndex, str, str.Length);
            }

            public void Close()
            {
                ConnClose(Peer._peerIndex, _connIndex);
            }

            public void EmitOnData(string str, Channel channel)
            {
                if (channel == Channel.Reliable)
                    OnReliableData?.Invoke(str);
                else
                    OnUnreliableData?.Invoke(str);
            }

            public void EmitOnClose() => OnClose?.Invoke();
        }
    }

    // ─────────────────────────────────────────────
    // DllImport / Stubs
    // ─────────────────────────────────────────────

#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")] private static extern void Init();
    [DllImport("__Internal")] private static extern int  OpenPeer(string key, string id, string host, int port);
    [DllImport("__Internal")] private static extern void Connect(int peer, string remoteId);
    [DllImport("__Internal")] private static extern void Send(int peer, int conn, string ptr, int length);
    [DllImport("__Internal")] private static extern void SendUnreliable(int peer, int conn, string ptr, int length);
    [DllImport("__Internal")] private static extern void ConnClose(int peer, int conn);
    [DllImport("__Internal")] private static extern void PeerDisconnect(int peer);
    [DllImport("__Internal")] private static extern void PeerDestroy(int peer);
    [DllImport("__Internal")] private static extern int  NextEventType(int peer);
    [DllImport("__Internal")] private static extern int  PeekReceivedChannel(int peer);
    [DllImport("__Internal")] private static extern void PopAnyEvent(int peer);
    [DllImport("__Internal")] private static extern void PopInitializedEvent(int peer);
    [DllImport("__Internal")] private static extern int  PopConnectedEvent(int peer, IntPtr remoteIdPtr, int maxLen);
    [DllImport("__Internal")] private static extern int  PeekReceivedEventSize(int peer);
    [DllImport("__Internal")] private static extern int  PopReceivedEvent(int peer, IntPtr dataPtr, int maxLen);
    [DllImport("__Internal")] private static extern int  PopConnClosedEvent(int peer);
    [DllImport("__Internal")] private static extern void PopPeerDisconnectedEvent(int peer);
    [DllImport("__Internal")] private static extern void PopPeerClosedEvent(int peer);
    [DllImport("__Internal")] private static extern void PopErrorEvent(int peer, IntPtr errorPtr, int maxLen);

#else
    private static void Init()                                                    => throw new NotImplementedException();
    private static int  OpenPeer(string k, string i, string h, int p)            => throw new NotImplementedException();
    private static void Connect(int p, string id)                                 => throw new NotImplementedException();
    private static void Send(int p, int c, string d, int l)                      => throw new NotImplementedException();
    private static void SendUnreliable(int p, int c, string d, int l)            => throw new NotImplementedException();
    private static void ConnClose(int p, int c)                                   => throw new NotImplementedException();
    private static void PeerDisconnect(int p)                                     => throw new NotImplementedException();
    private static void PeerDestroy(int p)                                        => throw new NotImplementedException();
    private static int  NextEventType(int p)                                      => throw new NotImplementedException();
    private static int  PeekReceivedChannel(int p)                                => throw new NotImplementedException();
    private static void PopAnyEvent(int p)                                        => throw new NotImplementedException();
    private static void PopInitializedEvent(int p)                                => throw new NotImplementedException();
    private static int  PopConnectedEvent(int p, IntPtr ptr, int l)               => throw new NotImplementedException();
    private static int  PeekReceivedEventSize(int p)                              => throw new NotImplementedException();
    private static int  PopReceivedEvent(int p, IntPtr ptr, int l)                => throw new NotImplementedException();
    private static int  PopConnClosedEvent(int p)                                 => throw new NotImplementedException();
    private static void PopPeerDisconnectedEvent(int p)                           => throw new NotImplementedException();
    private static void PopPeerClosedEvent(int p)                                 => throw new NotImplementedException();
    private static void PopErrorEvent(int p, IntPtr ptr, int l)                   => throw new NotImplementedException();
#endif
}
