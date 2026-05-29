var LibraryUnityPeerJS = {
    $UnityPeerJS: {
        peers: [],
    },

    // Init không cần load script nữa vì PeerJS đã được load trong index.html
    Init: function () {
        if (typeof Peer === 'undefined') {
            console.error('[PeerJS] Peer is not defined! Make sure PeerJS is loaded in index.html');
        } else {
            console.log('[PeerJS] Peer is ready');
        }
    },

    OpenPeer: function (key, id, host, port) {
        var keystr = UTF8ToString(key);
        var idstr = UTF8ToString(id);
        var hoststr = UTF8ToString(host);

        var options = { debug: 0 };

        if (hoststr && hoststr !== '0.peerjs.com') {
            options.host = hoststr;
            options.port = port;
            options.path = '/';
        }

        if (keystr && keystr.length > 0 && keystr !== 'peerjs') {
            options.key = keystr;
        }

        var peer = {
            peer: idstr && idstr.length > 0 ? new Peer(idstr, options) : new Peer(options),
            initialized: false,
            connPairs: [],
            events: [],
        };

        var peerInstance = UnityPeerJS.peers.push(peer) - 1;

        peer.registerPair = function(reliableConn, unreliableConn, remoteId) {
            var pair = {
                reliable: reliableConn,
                unreliable: unreliableConn,
                remoteId: remoteId,
                reliableOpen: false,
                unreliableOpen: false,
            };

            var connIndex = peer.connPairs.push(pair) - 1;

            var tryFireConnected = function() {
                if (pair.reliableOpen && pair.unreliableOpen) {
                    peer.events.push({ ev: 2, conn: connIndex, id: remoteId });

                    reliableConn.on('data', function(data) {
                        var dataStr = typeof data === 'string' ? data : JSON.stringify(data);
                        peer.events.push({ ev: 3, conn: connIndex, data: dataStr, channel: 0 });
                    });

                    unreliableConn.on('data', function(data) {
                        var dataStr = typeof data === 'string' ? data : JSON.stringify(data);
                        peer.events.push({ ev: 3, conn: connIndex, data: dataStr, channel: 1 });
                    });
                }
            };

            reliableConn.on('open', function() {
                pair.reliableOpen = true;
                tryFireConnected();
            });

            unreliableConn.on('open', function() {
                pair.unreliableOpen = true;
                tryFireConnected();
            });

            reliableConn.on('close', function() {
                peer.events.push({ ev: 4, conn: connIndex });
            });

            reliableConn.on('error', function(err) {
                peer.events.push({ ev: 7, err: err.type || err.message || 'conn-error' });
            });
        };

        peer.pendingIncoming = {};

        peer.peer.on('connection', function(conn) {
            var label = conn.label || '';
            var remoteId = conn.peer;

            var isReliable = label === 'reliable';
            var isUnreliable = label === 'unreliable';

            if (!isReliable && !isUnreliable) {
                console.warn('[PeerJS] Unknown channel label:', label);
                return;
            }

            if (!peer.pendingIncoming[remoteId])
                peer.pendingIncoming[remoteId] = {};

            if (isReliable)
                peer.pendingIncoming[remoteId].reliable = conn;
            else
                peer.pendingIncoming[remoteId].unreliable = conn;

            var pending = peer.pendingIncoming[remoteId];
            if (pending.reliable && pending.unreliable) {
                delete peer.pendingIncoming[remoteId];
                peer.registerPair(pending.reliable, pending.unreliable, remoteId);
            }
        });

        peer.popEvent = function (eventType) {
            if (peer.events.length == 0) return null;
            if (eventType != 0 && peer.events[0].ev != eventType) return null;
            return peer.events.shift();
        };

        peer.peer.on('open',        function()    { peer.initialized = true; peer.events.push({ ev: 1 }); });
        peer.peer.on('disconnected',function()    { peer.events.push({ ev: 5 }); });
        peer.peer.on('close',       function()    { peer.events.push({ ev: 6 }); });
        peer.peer.on('error',       function(err) { peer.events.push({ ev: 7, err: err.type || err.message }); });

        return peerInstance;
    },

    Connect: function (peerInstance, id) {
        var idstr = UTF8ToString(id);
        var peer = UnityPeerJS.peers[peerInstance];

        var reliableConn = peer.peer.connect(idstr, { label: 'reliable',   reliable: true,  serialization: 'json' });
        var unreliableConn = peer.peer.connect(idstr, { label: 'unreliable', reliable: false, serialization: 'json' });

        peer.registerPair(reliableConn, unreliableConn, idstr);
    },

    Send: function (peerInstance, connInstance, data, length) {
        var peer = UnityPeerJS.peers[peerInstance];
        var pair = peer.connPairs[connInstance];
        var datastr = UTF8ToString(data);
        if (pair && pair.reliable && pair.reliableOpen)
            pair.reliable.send(datastr);
    },

    SendUnreliable: function (peerInstance, connInstance, data, length) {
        var peer = UnityPeerJS.peers[peerInstance];
        var pair = peer.connPairs[connInstance];
        var datastr = UTF8ToString(data);
        if (pair && pair.unreliable && pair.unreliableOpen)
            pair.unreliable.send(datastr);
    },

    ConnClose: function (peerInstance, connInstance) {
        var peer = UnityPeerJS.peers[peerInstance];
        var pair = peer.connPairs[connInstance];
        if (pair) {
            if (pair.reliable)   pair.reliable.close();
            if (pair.unreliable) pair.unreliable.close();
        }
    },

    PeerDisconnect: function (peerInstance) {
        UnityPeerJS.peers[peerInstance].peer.disconnect();
    },

    PeerDestroy: function (peerInstance) {
        UnityPeerJS.peers[peerInstance].peer.destroy();
    },

    NextEventType: function (peerInstance) {
        var peer = UnityPeerJS.peers[peerInstance];
        if (peer.events.length == 0) return 0;
        return peer.events[0].ev;
    },

    PeekReceivedChannel: function (peerInstance) {
        var peer = UnityPeerJS.peers[peerInstance];
        if (peer.events.length == 0) return -1;
        var ev = peer.events[0];
        if (ev.ev !== 3) return -1;
        return ev.channel || 0;
    },

    PopAnyEvent: function (peerInstance) {
        UnityPeerJS.peers[peerInstance].popEvent(0);
    },

    PopInitializedEvent: function (peerInstance) {
        UnityPeerJS.peers[peerInstance].popEvent(1);
    },

    PopConnectedEvent: function (peerInstance, remoteIdPtr, remoteIdMaxLength) {
        var peer = UnityPeerJS.peers[peerInstance];
        var ev = peer.popEvent(2);
        stringToUTF8(ev.id, remoteIdPtr, remoteIdMaxLength);
        return ev.conn;
    },

    PeekReceivedEventSize: function (peerInstance) {
        var peer = UnityPeerJS.peers[peerInstance];
        if (peer.events.length == 0) return 0;
        var ev = peer.events[0];
        if (ev.ev == 3) return lengthBytesUTF8(ev.data) + 1;
        if (ev.ev == 2) return lengthBytesUTF8(ev.id)  + 1;
        return 0;
    },

    PopReceivedEvent: function (peerInstance, dataPtr, dataMaxLength) {
        var peer = UnityPeerJS.peers[peerInstance];
        var ev = peer.popEvent(3);
        if (ArrayBuffer.isView(ev.data)) return ev.conn;
        stringToUTF8(ev.data, dataPtr, dataMaxLength);
        return ev.conn;
    },

    PopConnClosedEvent: function (peerInstance) {
        return UnityPeerJS.peers[peerInstance].popEvent(4).conn;
    },

    PopPeerDisconnectedEvent: function (peerInstance) {
        UnityPeerJS.peers[peerInstance].popEvent(5);
    },

    PopPeerClosedEvent: function (peerInstance) {
        UnityPeerJS.peers[peerInstance].popEvent(6);
    },

    PopErrorEvent: function (peerInstance, errorPtr, errorMaxLength) {
        var peer = UnityPeerJS.peers[peerInstance];
        var ev = peer.popEvent(7);
        stringToUTF8((ev.err || 'unknown').slice(0, errorMaxLength - 1), errorPtr, errorMaxLength);
    },
};

autoAddDeps(LibraryUnityPeerJS, '$UnityPeerJS');
mergeInto(LibraryManager.library, LibraryUnityPeerJS);
