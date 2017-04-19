using UnityEngine;
using System.Collections;
using Logic.Protocol.Model;
using Logic.UI.Task.Model;

namespace Logic.UI.Task.Controller
{
	public class TaskController : SingletonMono<TaskController> 
	{
		
		void Awake()
		{
			instance = this;
		}
		void Start ()
		{
			Observers.Facade.Instance.RegisterObserver(((int)MSG.GetTasksResp).ToString(),LOBBY2CLIENT_GET_TASKS_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.TaskUpdateResp).ToString(),LOBBY2CLIENT_TASK_UPDATE_RESP_handler);
			Observers.Facade.Instance.RegisterObserver(((int)MSG.GetTaskRewardResp).ToString(),LOBBY2CLIENT_GET_TASK_REWARD_RESP_handler);


		}
		
		void OnDestroy ()
		{
			if (Observers.Facade.Instance != null)
			{
				Observers.Facade.Instance.RemoveObserver(((int)MSG.GetTasksResp).ToString(),LOBBY2CLIENT_GET_TASKS_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.TaskUpdateResp).ToString(),LOBBY2CLIENT_TASK_UPDATE_RESP_handler);
				Observers.Facade.Instance.RemoveObserver(((int)MSG.GetTaskRewardResp).ToString(),LOBBY2CLIENT_GET_TASK_REWARD_RESP_handler);

			}
		}
		#region client to server
		/// <summary>
		///请求任务信息
		/// </summary>
		public void CLIENT2LOBBY_GET_TASK_REQ()
		{
			GetTasksReq req = new GetTasksReq();
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
		}
		/// <summary>
		///请求领取任务奖励
		/// </summary>
		public void CLIENT2LOBBY_GET_TASK_REWARD_REQ(int id)
        {
			GetTaskRewardReq req = new GetTaskRewardReq();
			req.id = id;
			Logic.Protocol.ProtocolProxy.instance.SendProtocol(req);
        }

        #endregion
        
        #region sever
		//响应任务信息
		private bool LOBBY2CLIENT_GET_TASKS_RESP_handler(Observers.Interfaces.INotification note)
		{
			GetTasksResp resp= note.Body as GetTasksResp ;
			TaskProxy.instance.UpdateAllTask(resp.tasks);
			TaskProxy.instance.UpdateUIRefreshByProtocol();
			Logic.UI.RedPoint.Model.RedPointProxy.instance.Refresh();
            return true;
        }
		//任务更新
		private bool LOBBY2CLIENT_TASK_UPDATE_RESP_handler(Observers.Interfaces.INotification note)
		{
			TaskUpdateResp resp = note.Body as TaskUpdateResp ;
			int count = resp.addTasks.Count;
			for(int i = 0;i<count;i++)
			{
				TaskProxy.instance.AddTask(resp.addTasks[i]);
			}
			count = resp.delTasks.Count;
			for(int i = 0;i<count;i++)
			{
				TaskProxy.instance.DeleteTask(resp.delTasks[i]);
				
			}
			count = resp.updateTasks.Count;
            for(int i = 0;i<count;i++)
            {
                TaskProxy.instance.UpdateTask(resp.updateTasks[i]);


				
            }
			TaskProxy.instance.UpdateDelayUIRefreshByProtocol();
			Logic.UI.RedPoint.Model.RedPointProxy.instance.RefreshDelay();
			return true;
        }
		//响应领取任务奖励
		private bool LOBBY2CLIENT_GET_TASK_REWARD_RESP_handler(Observers.Interfaces.INotification note)
		{
			GetTaskRewardResp resp = note.Body as GetTaskRewardResp ;
			TaskProxy.instance.UpdateGetDailyTaskRewardByProtocol();
			//Logic.UI.RedPoint.Model.RedPointProxy.instance.Refresh();
            return true;
        }

        #endregion
	}
}

