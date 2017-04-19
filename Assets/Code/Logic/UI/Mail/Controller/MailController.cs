using UnityEngine;
using System.Collections;
using Logic.UI.Mail.Model;
using System.Collections.Generic;
using Logic.Protocol.Model;
using Logic.UI.RedPoint.Model;

namespace Logic.UI.Mail.Controller
{
	public class MailController : SingletonMono<MailController> 
	{
		
		void Awake()
		{
			instance = this;
		}
		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.MailResp).ToString(), LOBBY2CLIENT_Mail_Resp_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.MailAttachmentResp).ToString(), LOBBY2CLIENT_MailAttachmentResp_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.MailDelResp).ToString(), LOBBY2CLIENT_MailDelResp_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.MailSynResp).ToString(), LOBBY2CLIENT_MailSynResp_handler);

		}
		
		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.MailResp).ToString(), LOBBY2CLIENT_Mail_Resp_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.MailAttachmentResp).ToString(), LOBBY2CLIENT_MailAttachmentResp_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.MailDelResp).ToString(), LOBBY2CLIENT_MailDelResp_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.MailSynResp).ToString(), LOBBY2CLIENT_MailSynResp_handler);
			}
		}

		#region client to server
		public void CLIENT2LOBBY_Mail_REQ()
		{
			MailReq req = new MailReq();
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		public void  CLIENT2LOBBY_GetRewardOneMail_REQ(int id)
		{
			MailAttachmentReq req = new MailAttachmentReq();
			req.isGetAll = false;
			req.mailId = id;
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		public void CLIENT2LOBBY_GetRewardAllMail_REQ()
		{
			MailAttachmentReq req = new MailAttachmentReq();
			req.isGetAll = true;
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		public void CLIENT2LOBBY_DelateAllReadMail_REQ(bool isDelAll = true,int mailId = 0)
		{
			MailDelReq req = new MailDelReq();
			req.isDelAll = isDelAll;
			req.mailId = mailId;
			Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		public void CLIENT2LOBBY_MailReadReq_REQ(int id)
		{
			IntProto req = new IntProto();
			req.value = id;
			Protocol.ProtocolProxy.instance.SendProtocol((int)MSG.MailReadReq, req);
		}
		#endregion

		#region server
		//0x1102响应邮件(S->C)
		public bool LOBBY2CLIENT_Mail_Resp_handler(Observers.Interfaces.INotification note)
		{
			MailResp resp = note.Body as MailResp ;
			MailProxy.instance.AddAllMail(resp.mails);
			MailProxy.instance.RefreshAllByProtocol();
			RedPointProxy.instance.Refresh();
			return true;
		}
		
		//0x1104响应领取邮件附件(S->C)
		public bool LOBBY2CLIENT_MailAttachmentResp_handler(Observers.Interfaces.INotification note)
		{
			MailAttachmentResp resp = note.Body as MailAttachmentResp ;
			if(resp.isGetAll)
			{
				MailProxy.instance.UpdateGetAllMailRewardSuccessByProtocol();
				
			}else
			{
				MailProxy.instance.UpdateGetOneMailRewardSuccessByProtocol();
			}
			return true;
		}
		//0x1106响应删除邮件(S->C)
		public bool LOBBY2CLIENT_MailDelResp_handler(Observers.Interfaces.INotification note)
		{
			MailDelResp resp = note.Body as MailDelResp ;
			MailProxy.instance.RefreshAllByProtocol();
			return true;
		}
		//0x1108同步邮件(S->C)
		public bool LOBBY2CLIENT_MailSynResp_handler(Observers.Interfaces.INotification note)
		{
			MailSynResp resp = note.Body as MailSynResp ;
			MailProxy.instance.UpdateMail(resp.updateMails);
			MailProxy.instance.DeleteMail(resp.delMails);
			RedPointProxy.instance.Refresh();
			return true;
		}
		#endregion
	}

}
