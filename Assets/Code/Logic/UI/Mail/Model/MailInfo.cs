using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.Protocol.Model;

namespace Logic.UI.Mail.Model
{
	public class MailInfo 
	{
		public int id;
		public MailData mailData;
		public long createTime;
		public List<GameResData> rewardList = new List<GameResData>();
		public bool isRead;
		public bool isGetReward;
		public string picParam;
		public string[] titleParam;
		public string[] contentParam;
		public MailInfo(int id,int mailDataId,string[] titleParam,string[] contentParam, bool isGetReward = false ,List<GameResData> rewardList = null)
		{
			this.id = id;
			mailData = MailData.GetMailDataByID(mailDataId);
			this.titleParam = titleParam;
			this.contentParam = contentParam;
			this.isGetReward = isGetReward;
			if(rewardList != null)
				this.rewardList = rewardList;
		}
		public MailInfo(MailProtoData data)
		{
			Update(data);
		}
		public void Update(MailProtoData data)
		{
			this.id = data.id;
			if(data.state!= 0)//(0未领取,1已领取,2查看过)
			{
				this.isGetReward = data.state == 1 ? true : false;
				isRead = data.state > 0 ? true : false;
			}
				
			if(data.mailNo!= 0)
				mailData = MailData.GetMailDataByID(data.mailNo);
			if(!string.IsNullOrEmpty(data.picPath))
				picParam = data.picPath;
			int count = data.headParams.Count;
			if(count != 0 )
			{
				titleParam = data.headParams.ToArray();
			}
				
			if(data.contentParams.Count != 0)
				contentParam = data.contentParams.ToArray();
			if(data.createTime != 0)
				createTime = data.createTime;
			if(!string.IsNullOrEmpty(data.attachment))
			{
				rewardList.Clear();
				string[] attachment = data.attachment.Split(';');
				count = attachment.Length;
				for(int i = 0;i<count;i++)
				{
					rewardList.Add(new GameResData(attachment[i]));
				}
			}

		}
	}

}
