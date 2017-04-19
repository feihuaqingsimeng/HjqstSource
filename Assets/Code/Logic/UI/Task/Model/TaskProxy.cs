using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Task.Model;
using Logic.Enums;
using Logic.Protocol.Model;
using Logic.UI.Task.View;

namespace Logic.UI.Task.Model
{
    public class TaskProxy : SingletonMono<TaskProxy>
    {
        void Awake()
        {
            instance = this;
            Observers.Facade.Instance.RegisterObserver(Logic.UI.Main.View.MainView.PREFAB_PATH, Observer_MainView_handler);
        }
        void OnDestroy()
        {
            if (Observers.Facade.Instance != null)
            {
                Observers.Facade.Instance.RemoveObserver(Logic.UI.Main.View.MainView.PREFAB_PATH, Observer_MainView_handler);
            }
        }
        public System.Action onUpdateUIRereshByProtocolDelegate;
        public System.Action onUpdateGetTaskRewardByProtocolDelegate;

        public System.Action onUpdateNewTaskCompleteTipDelegate;

        public System.Action<int> onTaskCompleteDelegate;

        public Dictionary<int, TaskInfo> taskDictionary = new Dictionary<int, TaskInfo>();
        //public Dictionary<int,TaskInfo> dailyTaskDictionary = new Dictionary<int, TaskInfo>();
        //public Dictionary<int,TaskInfo> achievementTaskDictionary = new Dictionary<int, TaskInfo>();

        private List<TaskInfo> _taskCompleteTipList = new List<TaskInfo>();

        public int currentTaskBigType;
        public TaskInfo completeTaskInfo;
        public int completeTaskInfoIndexInCurrentTaskList;
        public List<TaskInfo> currentTaskInfoList;
        public List<TaskInfo> GetTaskListByType()
        {

            List<TaskInfo> taskList = new List<TaskInfo>();
            List<TaskInfo> taskTempList = new List<TaskInfo>();

            taskList = taskDictionary.GetValues();
            TaskInfo info;
            for (int i = 0, count = taskList.Count; i < count; i++)
            {
                info = taskList[i];
                if (info.taskData.task == currentTaskBigType)
                {
                    taskTempList.Add(info);
                }
            }
            taskTempList.Sort(CompareTaskInfo);
            currentTaskInfoList = taskTempList;
            return taskTempList;
        }

        private static int CompareTaskInfo(TaskInfo a, TaskInfo b)
        {


            int aWeight = a.isFinished && !a.isGetReward ? 5 : 0;
            int bWeight = b.isFinished && !b.isGetReward ? 10 : 0;

            if (aWeight - bWeight != 0)
            {
                return bWeight - aWeight;
            }

            int aRewardWeight = a.isGetReward ? 10 : 0;
            int bRewardWeight = b.isGetReward ? 5 : 0;
            if (aRewardWeight - bRewardWeight != 0)
            {
                return aRewardWeight - bRewardWeight;
            }
            return a.id - b.id;

        }

        public bool IsTaskComplete(int id)
        {
            TaskInfo taskInfo = null;
            taskDictionary.TryGetValue(id, out taskInfo);
            if (taskInfo != null && taskInfo.isFinished)
                return true;
            return false;
        }

        public void UpdateAllTask(List<TaskProtoData> dataList)
        {
            taskDictionary.Clear();
            for (int i = 0, count = dataList.Count; i < count; i++)
            {
                AddTask(dataList[i]);
            }
        }
        public void AddTask(TaskProtoData data)
        {
            if (!taskDictionary.ContainsKey(data.id))
            {
                TaskInfo info = new TaskInfo(data);
                if (info.taskData == null)
                {
                    Debugger.LogError("[error]task data can not find id:" + data.id + ",please fix it!!!!!!!!!!!!!!!!!!!");
                }
                else
                {
                    Logic.TalkingData.Controller.TalkingDataController.instance.TDGAMissionOnBegin(info.id.ToString(), Logic.TalkingData.Controller.TalkDataMissionType.Task);
                    taskDictionary.Add(data.id, info);
                }
                //Debugger.Log("add :"+info.ToString());
            }
        }
        public void DeleteTask(int id)
        {
            if (taskDictionary.ContainsKey(id))
            {

                taskDictionary.Remove(id);
                //Debugger.Log("delete :"+id);
            }
        }
        public void UpdateTask(TaskProtoData data)
        {
            if (taskDictionary.ContainsKey(data.id))
            {
                TaskInfo info = taskDictionary[data.id];
                info.UpdateTaskInfo(data);
                //Debugger.Log("update :"+info.ToString());

                UpdateTaskCompleteTip(info);

                if (onTaskCompleteDelegate != null && info.isFinished)
                    onTaskCompleteDelegate(info.id);

            }
        }
        public int GetIndexInCurrentTaskList(int id)
        {
            for (int i = 0, count = currentTaskInfoList.Count; i < count; i++)
            {
                if (id == currentTaskInfoList[i].id)
                {
                    return i;
                }
            }
            return -1;
        }
        public int GetAllCompleteTaskCount()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Task))
                return 0;
            int num = 0;
            //num += GetCompleteDailyTaskCount();
            num += GetCompleteAchievementTaskCount();
            num += GetCompleteMainTaskCount();
            return num;
        }
        public int GetCompleteMainTaskCount()
        {
            int num = 0;
            List<TaskInfo> dailyList = taskDictionary.GetValues();
            int count = dailyList.Count;
            TaskInfo info;
            for (int i = 0; i < count; i++)
            {
                info = dailyList[i];
                if (info.taskData.task == (int)TaskBigType.Task_main && info.isFinished && !info.isGetReward)
                    num++;
            }
            return num;
        }

        public int GetCompleteDailyTaskCount()
        {
            if (!FunctionOpen.Model.FunctionOpenProxy.instance.IsFunctionOpen(FunctionOpenType.MainView_Task))
                return 0;
            int num = 0;
            List<TaskInfo> dailyList = taskDictionary.GetValues();
            int count = dailyList.Count;
            TaskInfo info;
            for (int i = 0; i < count; i++)
            {
                info = dailyList[i];
                if (info.taskData.task == (int)TaskBigType.Task_daliy && info.isFinished && !info.isGetReward)
                    num++;
            }
            return num;
        }

        public int GetCompleteAchievementTaskCount()
        {
            int num = 0;
            List<TaskInfo> achievementList = taskDictionary.GetValues();
            int count = achievementList.Count;
            TaskInfo info;
            for (int i = 0; i < count; i++)
            {
                info = achievementList[i];
                if (info.taskData.task == (int)TaskBigType.Task_achievement && info.isFinished && !info.isGetReward)
                    num++;
            }
            return num;
        }
        private void UpdateTaskCompleteTip(TaskInfo info)
        {
            if (info.isFinished && !info.isGetReward)
            {
                _taskCompleteTipList.Add(info);
                if (onUpdateNewTaskCompleteTipDelegate != null)
                {
                    onUpdateNewTaskCompleteTipDelegate();
                }
                Logic.TalkingData.Controller.TalkingDataController.instance.TDGAMissionOnCompleted(info.id.ToString(), Logic.TalkingData.Controller.TalkDataMissionType.Task);
                if (isMainViewOpen)
                {
                    ShowTaskCompleteTip();
                }
            }
        }
        private void ShowTaskCompleteTip()
        {
            if (_isShowTaskTip)
                return;
            StartCoroutine("ShowTaskCompleteTipCoroutine");
        }
        private void StopTaskCompleteTip()
        {
            StopCoroutine("ShowTaskCompleteTipCoroutine");
            _isShowTaskTip = false;
            _taskCompleteTipList.Clear();
            TaskCompleteTipView.Close();
        }
        private bool _isShowTaskTip;
        private IEnumerator ShowTaskCompleteTipCoroutine()
        {
            _isShowTaskTip = true;
            yield return null;
            while (_taskCompleteTipList.Count > 0)
            {
                Logic.UI.Task.View.TaskCompleteTipView.Open(_taskCompleteTipList.First());
                _taskCompleteTipList.RemoveAt(0);
                if (_taskCompleteTipList.Count == 0)
                    break;
                yield return new WaitForSeconds(2.7f);
            }
            _isShowTaskTip = false;
        }
        #region refrsh by server
        public void UpdateUIRefreshByProtocol()
        {
            if (onUpdateUIRereshByProtocolDelegate != null)
            {
                onUpdateUIRereshByProtocolDelegate();
                //Debugger.Log("[taskProxy]UpdateUIRefresh......");
            }
        }
        //延时刷新(服务器发的相同操蛋消息TaskUpdateResp太多了，延时下刷新）
        public void UpdateDelayUIRefreshByProtocol()
        {
            CancelInvoke("UpdateUIRefreshByProtocol");
            Invoke("UpdateUIRefreshByProtocol", 0.2f);
        }


        public void UpdateGetDailyTaskRewardByProtocol()
        {
            if (onUpdateGetTaskRewardByProtocolDelegate != null)
            {
                onUpdateGetTaskRewardByProtocolDelegate();
            }
        }
        #endregion
        private bool isMainViewOpen = false;
        private bool Observer_MainView_handler(Observers.Interfaces.INotification note)
        {
            if ("open".Equals(note.Type))
            {
                ShowTaskCompleteTip();
                isMainViewOpen = true;
            }
            else if ("close".Equals(note.Type))
            {
                isMainViewOpen = false;
                StopTaskCompleteTip();
            }
            return true;
        }
    }
}

