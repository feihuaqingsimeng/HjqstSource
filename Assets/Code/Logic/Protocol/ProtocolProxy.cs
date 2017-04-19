
//#define blowfish
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Logic.Protocol
{
    public sealed class ProtocolProxy : SingletonMono<ProtocolProxy>
    {
        private bool _isLoginServer = false;

        public bool IsLoginServer
        {
            get { return _isLoginServer; }
            set
            {
                _isLoginServer = value;
                if (_isLoginServer)
                {

                }
                else
                {

                }
            }
        }

        private string _host;
        private int _port;

#if UNITY_EDITOR
        public string EditorHost
        {
            get { return _host; }
        }
        public int EditorPort
        {
            get { return _port; }
        }
#endif
        private Socket _socket;
        private MemoryStream _recvStream;

        private Thread _sendThread;
        private Thread _recvThread;
        private Thread _connThread;

        private Queue<ProtocolItem> _sendQueue;
        private Queue<ProtocolItem> _reConnSendQueue;
        private Queue<ProtocolItem> _recvQueue;

        private int _reConnCount = 0;
        private long _recvStreamPos = 0;
        private bool _gameServerErr = false;
        private bool _reconnect = false;
        #region 消息头信息
        public int Version { get; set; }
        public int UserID0 { get; set; }//客户端角色ID
        public int UserID1 { get; set; }//游戏通讯服务器ID
        public int UserID2 { get; set; }//服务器端使用序列号
        public int UserID3 { get; set; }//场景ID
        public int UserID4 { get; set; }//是否群发
        public int UserID5 { get; set; }//ip
        public int UserID6 { get; set; }
        public int UserID7 { get; set; }
        public int MsgType { get; set; }//协议号

        //public int pingTime { get; set; }//ping time

        private int _userID2;//服务器端使用序列号
        #endregion

#if blowfish
        private BlowFishCS.BlowFish _encryptBF;
        private BlowFishCS.BlowFish _decryptBF;
#endif
        public delegate void ConnectedHandler(bool connected);
        public ConnectedHandler connectedHandler;

        void Awake()
        {
            _sendQueue = new Queue<ProtocolItem>();
            _reConnSendQueue = new Queue<ProtocolItem>();
            instance = this;
        }

        void OnDestroy()
        {
            DisConnect(true);
            instance = null;
        }

        private IEnumerator CheckConnStatus()
        {
            WaitForSeconds wfs = new WaitForSeconds(2f);
            while (true)
            {
                yield return wfs;
                if (_gameServerErr)
                {
                    DisConnect(true);
                    Debugger.LogError("服务器错误，断开连接，重新登陆");
                    if (connectedHandler != null)
                        connectedHandler(false);
                    yield break;
                }

                if (!Connected)
                {
                    if (_reConnCount == 0)
                        _reconnectable = true;
                    else if (_reConnCount > 3)
                    {
                        DisConnect(true);
                        if (connectedHandler != null)
                            connectedHandler(false);
                        connectedHandler = null;
                        Debugger.Log("连接服务器超时");
                        yield break;
                    }

                    Reconnect();
                }
            }
        }


        private ProtocolItem _protocolItem2;
        private float _lastProtocolTime = 0;
        private const float BEAT_INTERVAL = 120f;
        private const int MAX_SEND_COUNT = 3;
        //private int _beatFailCount = 0;
        private const int BEAT_MAX_COUNT = 2;
        private bool _reconnectable = false;
        //private long _lastHeartbeatTime;
        void Update()
        {
            if (_recvQueue != null && _recvQueue.Count > 0)
            {
                lock (_recvQueue)
                {
                    int sendCount = 0;
                    while (_recvQueue != null && _recvQueue.Count > 0)
                    {
                        if (sendCount <= MAX_SEND_COUNT)
                        {
                            _protocolItem2 = _recvQueue.Dequeue();
                            if (ProtocolConf.instance.ExistInLuaProcotols(_protocolItem2.ProtocolId))
                            {
                                LuaInterface.ToLuaPb.SetFromSocket(_protocolItem2.ProtocolId, _protocolItem2.ProtocolBytes);
                            }
                            else
                            {
                                Debugger.Log(_protocolItem2.ProtocolId.ToString() + "收到服务器的协议：" + _protocolItem2.ToString());
                                if (_protocolItem2.ProtocolId == (int)Logic.Protocol.Model.MSG.ReconnectResp)
                                {
                                    SendCacheMessage();//发送协议缓存
                                }
                                else if (_protocolItem2.ProtocolId == (int)Logic.Protocol.Model.MSG.ClientActiveReq)
                                {
                                    Common.GameTime.Controller.TimeController.instance.ServerTimeTicksMillisecondAfter9 = ProtocolProxy.instance.UserID6;
                                }
                                else
                                {
                                    if (_protocolItem2.ProtocolId == (int)Logic.Protocol.Model.MSG.LoginResp)
                                    {
                                        UserID2 = _userID2;//登陆成功时保存
                                    }
                                    //else if (_protocolItem2.ProtocolId == (int)Logic.Protocol.Model.MSG.FightCmdSynResp)
                                    //{
                                    //    Logic.Protocol.Model.FightCmdSynResp resp = _protocolItem2.Protocol as Logic.Protocol.Model.FightCmdSynResp;
                                    //    Debugger.Log("update time:{0},release Time:{1}", Common.Util.TimeUtil.GetTimeStamp(), resp.releaseTime);
                                    //}
                                    Observers.Facade.Instance.SendNotification(_protocolItem2.ProtocolId.ToString(), _protocolItem2.Protocol);

                                }
                            }
                            if (_recvQueue.Count == 0)
                                Logic.UI.Mask.Contorller.MaskController.instance.HideMask();
                            //_beatFailCount = 0;
                            _lastProtocolTime = Time.realtimeSinceStartup;
                            sendCount++;
                        }
                        else
                            break;
                    }
                }
            }
            if (Time.realtimeSinceStartup - _lastProtocolTime > BEAT_INTERVAL)
            {
                //if (_beatFailCount >= BEAT_MAX_COUNT)
                //{
                //    Debugger.Log("断线重连{0}次超时", BEAT_MAX_COUNT);
                //    if (connectedHandler != null)
                //        connectedHandler(false);
                //    return;
                //}
                //if (_beatFailCount > 0)
                //{
                //    Debugger.Log("断线重连第{0}次", _beatFailCount);
                //    _lastProtocolTime = Time.realtimeSinceStartup + BEAT_INTERVAL / 2;
                //}
                //else
                _lastProtocolTime = Time.realtimeSinceStartup;
                SendProtocol(new Logic.Protocol.Model.ClientActiveReq());
                Debugger.Log("beat connect !");
                //_lastHeartbeatTime = Common.Util.TimeUtil.GetTimeStamp();
                //_beatFailCount++;
            }
        }

        private IEnumerator CheckConnectedHandler()
        {
            float startTime = Time.realtimeSinceStartup;
            while (true)
            {
                yield return null;
                if (Connected)
                {
                    //_beatFailCount = 0;
                    _lastProtocolTime = Time.realtimeSinceStartup;
                    _reConnCount = 0;
                    _gameServerErr = false;
                    StopCoroutine("CheckConnStatus");
                    StartCoroutine("CheckConnStatus");
                    if (_reconnect)
                        ReConnLoginSucc();
                    if (connectedHandler != null)
                        connectedHandler(true);
                    yield break;
                }
                else
                {
                    float time = Time.realtimeSinceStartup - startTime;
                    if (time > 10)
                    {
                        if (!_reconnect)
                        {
                            DisConnect(true);
                            if (connectedHandler != null)
                                connectedHandler(false);
                            connectedHandler = null;
                        }
                        else if (/*_sendQueue.Count > 0 ||*/ _reConnSendQueue.Count > 0)
                        {
                            _reconnectable = true;
                        }
                        yield break;
                    }
                }
            }
        }

        public bool Connected
        {
            get { return _socket != null ? _socket.Connected : false; }
        }
        /// <summary>
        /// 发送协议到协议队列
        /// </summary>
        /// <param name="iProtocol"></param>
        public void SendProtocol(ProtoBuf.IExtensible iProtocol)
        {
            ProtocolItem tProtocolItem = new ProtocolItem(iProtocol);

            lock (_sendQueue)
            {
                _sendQueue.Enqueue(tProtocolItem);
            }
            _waitSend.Set();//启动被ManualResetEvent阻塞的线程
        }

        public void SendProtocol(int protocolId, ProtoBuf.IExtensible iProtocol)
        {
            ProtocolItem tProtocolItem = new ProtocolItem(protocolId, iProtocol);

            lock (_sendQueue)
            {
                _sendQueue.Enqueue(tProtocolItem);
            }
            _waitSend.Set();//启动被ManualResetEvent阻塞的线程
        }

        public void SendProtocolByte(int protocolId, byte[] bytes)
        {
            ProtocolItem tProtocolItem = new ProtocolItem(protocolId, bytes);

            lock (_sendQueue)
            {
                _sendQueue.Enqueue(tProtocolItem);
            }
            _waitSend.Set();//启动被ManualResetEvent阻塞的线程
        }

        public void DisConnect(bool dispose)
        {
            StopCoroutine("CheckConnStatus");
            StopCoroutine("CheckConnectedHandler");
            if (_sendThread != null)
            {
                _sendThread.Abort();
                _sendThread = null;
            }

            if (_recvThread != null)
            {
                _recvThread.Abort();
                _recvThread = null;
            }

            if (_connThread != null)
            {
                _connThread.Abort();
                _connThread = null;
            }

            if (_recvQueue != null)
            {
                lock (_recvQueue)
                {
                    _recvQueue.Clear();
                }
                _recvQueue = null;
            }

            if (dispose)
            {
                if (_reConnSendQueue != null)
                    _reConnSendQueue.Clear();
                if (_sendQueue != null)
                {
                    lock (_sendQueue)
                    {
                        _sendQueue.Clear();
                    }
                }

            }
            _recvBuff = null;

            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }

        public void ReConnLoginSucc()
        {
            _reconnect = false;
            SendProtocol(new Logic.Protocol.Model.ReconnectReq());
            Logic.UI.Mask.Contorller.MaskController.instance.HideMask();
        }

        public void SendCacheMessage()
        {
            lock (_sendQueue)
            {
                while (_reConnSendQueue.Count > 0)
                    _sendQueue.Enqueue(_reConnSendQueue.Dequeue());
            }
            _waitSend.Set();
        }

        private void Reconnect()
        {
            if (!_reconnectable) return;
            _reconnect = true;
            _reconnectable = false;
            lock (_sendQueue)
            {
                while (_sendQueue.Count > 0)
                {
                    ProtocolItem protocolItem = _sendQueue.Dequeue();
                    if (protocolItem.GetType() != typeof(Logic.Protocol.Model.ClientActiveReq))
                        _reConnSendQueue.Enqueue(protocolItem);
                }
            }
            if (_reConnSendQueue.Count > 0)//有协议需要发送才计算次数
            {
                _reConnCount++;
                Debugger.Log(string.Format("第{0}次尝试重新连接服务器", _reConnCount));
            }
            Connect(_host, _port);
            //Logic.UI.Mask.Contorller.MaskController.instance.ShowMask();
        }

        /// <summary>
        /// 链接服务器
        /// </summary>
        /// <param name="host">服务器IP</param>
        /// <param name="port">服务器端口</param>
        public void Connect(string host, int port)
        {
            Debugger.Log("conn" + host + ":" + port);
#if blowfish
            _encryptBF = new BlowFishCS.BlowFish(Encoding.ASCII.GetBytes("#$youwillbedead#$"));
            _decryptBF = new BlowFishCS.BlowFish(Encoding.ASCII.GetBytes("#$youwillbefucked#$"));
#endif
            if (_recvQueue == null)
                _recvQueue = new Queue<ProtocolItem>();//发送队列不能在这里初始化
            else
                _recvQueue.Clear();

            if (_host != host)
            {
                _host = host;
            }

            if (_port != port)
            {
                _port = port;
            }

            try
            {
                _connThread = new Thread(new ThreadStart(ConnThread));
                StopCoroutine("CheckConnectedHandler");
                StartCoroutine("CheckConnectedHandler");
                _connThread.Start();
            }
            catch (Exception e)
            {
                Debugger.LogError("socket exception:" + e);
            }
        }

        private System.Threading.ManualResetEvent _waitSend = new ManualResetEvent(true);

        private void ConnThread()
        {
            IPAddress ip = null;
            IPAddress[] ipAry = Dns.GetHostAddresses(_host);
            foreach (IPAddress ipAddress in ipAry)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = ipAddress;
                    break;
                }
            }
            IPEndPoint ipEndPoint = new IPEndPoint(ip, _port);
#if UNITY_EDITOR

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Blocking = true;
            _socket.Connect(ipEndPoint);
#endif

#if UNITY_ANDROID
            //IPEndPoint ipEndPoint = new IPEndPoint(ip, _port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Blocking = true;
            _socket.Connect(ipEndPoint);
#endif
#if UNITY_IOS
            AddressFamily addressFamily = AddressFamily.InterNetwork;
            string newIPAddress = string.Empty;
            IPv6SupportMidleware.getIPType(ip.ToString(), _port.ToString(), out newIPAddress, out addressFamily);

            _socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Blocking = true;
            _socket.Connect(newIPAddress, _port);
#endif
            _recvStream = new MemoryStream();
            _recvThread = new Thread(new ThreadStart(RecvThread));
            _sendThread = new Thread(new ThreadStart(SendThread));
            _recvThread.Start();
            _sendThread.Start();
            _connThread = null;
        }

        private void RecvThread()
        {
            try
            {
                while (true)
                {
                    if (!Connected)
                    {
                        _recvThread = null;
                        break;
                    }
                    ParseProtocol();
                }
            }
            catch (Exception e)
            {
                Debugger.LogError("socket exception" + e);
            }
        }


        private ProtocolItem _sendProtocolItem;
#if blowfish
        private MBinaryWriter _mbw = new MBinaryWriter();
#endif
        private byte[] _bytes;
        private void SendThread()
        {
            try
            {
                while (true)
                {
                    if (!Connected)
                    {
                        _sendThread = null;
                        break;
                    }
                    _waitSend.WaitOne();//阻塞当前线程
                    _waitSend.Reset();//线程只处理一次，立即阻塞

                    lock (_sendQueue)
                    {
                        while (_sendQueue.Count > 0)
                        {
                            _sendProtocolItem = _sendQueue.Peek();
                            _bytes = _sendProtocolItem.ProtocolItemBytes;
#if blowfish
                            _mbw.Clear();
                            _bytes = _encryptBF.Encrypt_ECB(_bytes);
                            _mbw.Write(_bytes.Length + ProtocolConf.PACKGE_LEN);
                            _mbw.Write(_bytes);
                            _bytes = _mbw.ToArray();
#endif
                            int result = 0;
                            int byteSize = _bytes.Length;
                            while (result != byteSize)
                            {
                                result += _socket.Send(_bytes, result, byteSize - result, SocketFlags.None);
                            }
                            _sendQueue.Dequeue();

                            Debugger.Log("给服务器发送数据:" + result + ",    协议体:" + _sendProtocolItem.ToString());
                        }
                    }
                }
            }
            catch (SocketException se)
            {
                Debugger.LogError("MSocketException" + se);
            }
            catch (Exception e)
            {
                Debugger.LogError("MSocketException" + e);
            }
        }

        private int _publicInt;
        private MBinaryReader _mbr;
        private ProtocolItem _protocolItem = new ProtocolItem();
        private byte[] _recvBuff = new byte[65535];
        private void ParseProtocol()
        {
            if (!Connected) return;
            _publicInt = _socket.Receive(_recvBuff, SocketFlags.None);
            if (_publicInt <= 0)
            {
                _gameServerErr = true;
                return;
            }
            _recvStream.Position = _recvStream.Length;
            _recvStream.Write(_recvBuff, 0, _publicInt);
            _recvStream.Position = _recvStreamPos;
            byte[] bytes = new byte[ProtocolConf.PACKGE_LEN];
            while (_recvStream.Length - _recvStream.Position >= ProtocolConf.PACKGE_LEN)
            {
                //long t1 = Common.Util.TimeUtil.GetTimeStamp();
                _recvStreamPos = _recvStream.Position;
                _recvStream.Read(bytes, 0, ProtocolConf.PACKGE_LEN);
                _mbr = new MBinaryReader(bytes);

                _publicInt = _mbr.ReadInt32();
                if (_recvStream.Length - _recvStream.Position >= _publicInt)
                {
                    _recvStreamPos = 0;
                    bytes = new byte[_publicInt];
                    _recvStream.Read(bytes, 0, _publicInt);
                    if (Game.GameConfig.instance.encrypt)
                    {
                        byte[] decryptBytes = Common.Util.EncryptUtil.AESDecryptBytes(bytes, Game.GameConfig.instance.aesEncryptKey);
                        _publicInt = decryptBytes.Length;
                        _mbr = new MBinaryReader(decryptBytes);
                    }
                    else
                        _mbr = new MBinaryReader(bytes);
                    ProtocolProxy.instance.Version = _mbr.ReadInt32();
                    ProtocolProxy.instance.UserID0 = _mbr.ReadInt32();
                    ProtocolProxy.instance.UserID1 = _mbr.ReadInt32();
                    _userID2 = _mbr.ReadInt32();
                    ProtocolProxy.instance.UserID3 = _mbr.ReadInt32();
                    ProtocolProxy.instance.UserID4 = _mbr.ReadInt32();
                    ProtocolProxy.instance.UserID5 = _mbr.ReadInt32();
                    ProtocolProxy.instance.UserID6 = _mbr.ReadInt32();
                    ProtocolProxy.instance.UserID7 = _mbr.ReadInt32();
                    int protocolId = _mbr.ReadInt32();
                    _publicInt -= ProtocolConf.PROTOCOL_HEAD_LENGTH;
                    byte[] bytes2 = new byte[_publicInt];
                    _mbr.Read(bytes2, 0, _publicInt);
                    if (ProtocolConf.instance.ExistInLuaProcotols(protocolId))
                    {
                        if (_recvQueue != null)
                        {
                            lock (_recvQueue)
                            {
                                _recvQueue.Enqueue(new ProtocolItem() { ProtocolId = protocolId, ProtocolBytes = bytes2 });
                            }
                        }
                    }
                    else
                    {
                        _protocolItem = new ProtocolItem();
                        _protocolItem.ProtocolId = protocolId;
                        using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes2))
                        {
                            System.Type t = ProtocolProxy.instance.IsLoginServer ? ProtocolConf.GetLoginServerTypeByID(_protocolItem.ProtocolId) : ProtocolConf.GetTypeByID(_protocolItem.ProtocolId);
                            if (t == null)
                            {
                                Debugger.LogError("未配置ID为" + _protocolItem.ProtocolId + "的协议 , 不含包头长：" + bytes.Length + "字节");
                            }
                            else
                            {
                                if (ProtocolProxy.instance.IsLoginServer)
                                {
                                    _protocolItem.Protocol = Activator.CreateInstance(t) as IProtocol;
                                    (_protocolItem.Protocol as IProtocol).FromBytes(new MBinaryReader(ms));
                                }
                                else
                                {
                                    if (t != typeof(EmptyMessage))
                                        _protocolItem.Protocol = ProtoBuf.Serializer.NonGeneric.Deserialize(t, ms) as ProtoBuf.IExtensible;
                                }
                            }
                        }
                        if (_recvQueue != null)
                        {
                            lock (_recvQueue)
                            {
                                _recvQueue.Enqueue(_protocolItem);
                            }
                        }
                    }
                }
                else
                {
                    _recvStream.Position = _recvStreamPos;
                    break;
                }
                _mbr.Close();
                _mbr = null;
            }
            if (_recvStream.Position == _recvStream.Length)
            {
                _recvStream.SetLength(0);
            }
        }
    }
}
