using Common.GameTime.Controller;
using System;
using UnityEngine;

namespace Logic.Protocol
{
    public class ProtocolItem
    {
        public int ProtocolId
        {
            get;
            set;
        }

        public ProtoBuf.IExtensible Protocol
        {
            set;
            get;
        }

        public byte[] ProtocolItemBytes
        {
            private set;
            get;
        }

        public byte[] ProtocolBytes
        {
            set;
            get;
        }

        /// <summary>
        /// 给服务器发送协议时
        /// </summary>
        /// <param name="protocol"></param>
        public ProtocolItem(ProtoBuf.IExtensible protocol)
        {
            if (protocol != null)
            {
                if (ProtocolProxy.instance.IsLoginServer)
                {
                    ProtocolId = (int)ProtocolConf.GetLoginServerIdByType(protocol.GetType());

                }
                else
                {
                    ProtocolId = (int)ProtocolConf.GetIdByType(protocol.GetType());
                }
                Protocol = protocol;
                ProtocolItemBytes = ParseItem();
                if (ProtocolConf.NeedShowMask(ProtocolId))
                    Logic.UI.Mask.Contorller.MaskController.instance.ShowMask();
            }
        }
        public ProtocolItem(int protocolId, ProtoBuf.IExtensible protocol)
        {
            if (protocol != null)
            {
                if (ProtocolProxy.instance.IsLoginServer)
                {
                    ProtocolId = (int)ProtocolConf.GetLoginServerIdByType(protocol.GetType());
                }
                else
                {
                    ProtocolId = protocolId;
                }
                Protocol = protocol;
                ProtocolItemBytes = ParseItem();
                if (ProtocolConf.NeedShowMask(ProtocolId))
                    Logic.UI.Mask.Contorller.MaskController.instance.ShowMask();
            }
        }
        public ProtocolItem(int protocolId, byte[] bytes)
        {
            ProtocolId = protocolId;
            byte[] enryptByteArray;
            MBinaryWriter mbw = new MBinaryWriter();

            if (!Game.GameConfig.instance.encrypt)
            {
                if (bytes == null)
                    mbw.Write(ProtocolConf.PROTOCOL_HEAD_LENGTH);
                else
                    mbw.Write(bytes.Length + ProtocolConf.PROTOCOL_HEAD_LENGTH);
            }
            mbw.Write(ProtocolProxy.instance.Version);
            mbw.Write(ProtocolProxy.instance.UserID0);
            mbw.Write(ProtocolProxy.instance.UserID1);
            mbw.Write(ProtocolProxy.instance.UserID2);
            mbw.Write(ProtocolProxy.instance.UserID3);
            mbw.Write(ProtocolProxy.instance.UserID4);
            mbw.Write(ProtocolProxy.instance.UserID5);
            mbw.Write(TimeController.instance.ServerTimeTicksMillisecondAfter9);
            mbw.Write(ProtocolProxy.instance.UserID7);
            mbw.Write(ProtocolId);
            if (bytes != null)
                mbw.Write(bytes);
            if (Game.GameConfig.instance.encrypt)
            {
                enryptByteArray = Common.Util.EncryptUtil.AESEncryptBytes(mbw.ToArray(), Game.GameConfig.instance.aesEncryptKey);
                mbw.Clear();
                mbw.Write(enryptByteArray.Length);
                mbw.Write(enryptByteArray);
            }
            ProtocolItemBytes = mbw.ToArray();
            mbw.Close();
            mbw = null;
            if (ProtocolConf.NeedShowMask(ProtocolId))
                Logic.UI.Mask.Contorller.MaskController.instance.ShowMask();
        }

        /// <summary>
        /// 收到服务器协议时
        /// </summary>
        public ProtocolItem()
            : this(null)
        {
        }
        /// <summary>
        /// 向服务器发送协议时封装协议
        /// </summary>
        /// <returns></returns>
        private byte[] ParseItem()
        {
            if (Protocol == null)
            {
                return null;
            }

            byte[] tByteArray, enryptByteArray;
            MBinaryWriter mbw = new MBinaryWriter();
            if (ProtocolProxy.instance.IsLoginServer)
            {
                tByteArray = (Protocol as IProtocol).ToBytes();
                mbw.Write((ushort)(tByteArray.Length + ProtocolConf.PROTOCOL_HEAD_LENGTH));
                mbw.Write((ushort)ProtocolId);
            }
            else
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    ProtoBuf.Serializer.NonGeneric.Serialize(ms, Protocol);
                    tByteArray = ms.ToArray();
                }
                if (!Game.GameConfig.instance.encrypt)
                    mbw.Write(tByteArray.Length + ProtocolConf.PROTOCOL_HEAD_LENGTH);
                mbw.Write(ProtocolProxy.instance.Version);
                mbw.Write(ProtocolProxy.instance.UserID0);
                mbw.Write(ProtocolProxy.instance.UserID1);
                mbw.Write(ProtocolProxy.instance.UserID2);
                mbw.Write(ProtocolProxy.instance.UserID3);
                mbw.Write(ProtocolProxy.instance.UserID4);
                mbw.Write(ProtocolProxy.instance.UserID5);
                mbw.Write(TimeController.instance.ServerTimeTicksMillisecondAfter9);
                mbw.Write(ProtocolProxy.instance.UserID7);
                mbw.Write(ProtocolId);
            }
            mbw.Write(tByteArray);
            if (Game.GameConfig.instance.encrypt)
            {
                enryptByteArray = Common.Util.EncryptUtil.AESEncryptBytes(mbw.ToArray(), Game.GameConfig.instance.aesEncryptKey);
                mbw.Clear();
                mbw.Write(enryptByteArray.Length);
                mbw.Write(enryptByteArray);
            }
            tByteArray = mbw.ToArray();
            mbw.Close();
            mbw = null;
            return tByteArray;
        }

        public override string ToString()
        {
            return string.Format("ProtocolId:{0}, _protocol:[{1}]", ProtocolId, Protocol != null ? Protocol.ToString() : "nullProtocol");
        }
    }
}
