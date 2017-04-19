using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using System.Collections;

namespace Logic.Tutorial.Model
{
    public class TutorialProxy : SingletonMono<TutorialProxy>
    {
        public System.Action<int> onSkipCurrentChapterDelegate;

        private TutorialChapterData _currentTutorialChapterData;
        public TutorialChapterData CurrentTutorialChapterData
        {
            get
            {
                return _currentTutorialChapterData;
            }
        }

        private TutorialStepData _currentTutorialStepData;
        public TutorialStepData CurrentTutorialStepData
        {
            get
            {
                return _currentTutorialStepData;
            }
        }

        private int _cachedInitTutorialChapterID;
        public bool SkipSimpleIntroduction
        {
            get
            {
                if (!Tutorial.Controller.TutorialController.instance.IsTutorialOpen)
                    return true;
                return _cachedInitTutorialChapterID > 1;
            }
        }

        private bool _isCurrentTurorialChapterOpened = false;
        public bool IsCurrentTurorialChapterOpened
        {
            get
            {
                return _isCurrentTurorialChapterOpened;
            }
            set
            {
                _isCurrentTurorialChapterOpened = value;
            }
        }

        void Awake()
        {
            instance = this;
        }

        public void Init(int initTutorialChapterID)
        {
            _cachedInitTutorialChapterID = initTutorialChapterID;
            SetCurrentTutorialChapter(initTutorialChapterID);
        }

        public void SetCurrentTutorialChapter(int tutorialChapterID)
        {
            TutorialChapterData.TutorialChapterDataSortedDic.TryGetValue(tutorialChapterID, out _currentTutorialChapterData);
            if (_currentTutorialChapterData != null)
                _currentTutorialStepData = _currentTutorialChapterData.GetStepData(1);
            Debugger.Log("SetCurrentTutorialChapter=====>Chapter ID:" + tutorialChapterID);
        }

        public bool HasNextChapter()
        {
            return _currentTutorialChapterData.HasNextChapter();
        }

        public void MoveToNextChapter()
        {
            TutorialChapterData nextTutorialChapterData = _currentTutorialChapterData.GetNextTutorialChapterData();
            Logic.TalkingData.Controller.TalkingDataController.instance.TDGAMissionOnBegin(_currentTutorialChapterData.id.ToString(), Logic.TalkingData.Controller.TalkDataMissionType.Tutorial);
            Logic.TalkingData.Controller.TalkingDataController.instance.TDGAMissionOnCompleted(_currentTutorialChapterData.id.ToString(), Logic.TalkingData.Controller.TalkDataMissionType.Tutorial);
            if (nextTutorialChapterData != null)
            {
                _currentTutorialChapterData = nextTutorialChapterData;
                _currentTutorialStepData = _currentTutorialChapterData.GetStepData(1);
            }
            Debugger.Log("=====[Tutorial][MoveToNextChapter]::" + (nextTutorialChapterData.isBackup ? "[Backup:" : "[Main:") + nextTutorialChapterData.id.ToString() + "]");
        }

        public void MoveToChapter(TutorialChapterData tutorialChapterData)
        {
            IsCurrentTurorialChapterOpened = false;
            if (tutorialChapterData != null)
            {
                _currentTutorialChapterData = tutorialChapterData;
                _currentTutorialStepData = _currentTutorialChapterData.GetStepData(1);
				Debugger.Log("=====[Tutorial][MoveToChapter]::" + (tutorialChapterData.isBackup ? "[Backup:" : "[Main:") + tutorialChapterData.id.ToString() + "]");
            }
        }

        public bool HasNextStep()
        {
            TutorialStepData nextStepData = _currentTutorialChapterData.GetNextStepData(_currentTutorialStepData);
            return nextStepData != null;
        }

        public void MoveToNextStep(TutorialStepData stepData)
        {
            IsCurrentTurorialChapterOpened = false;
            if (stepData == null || stepData.id == _currentTutorialStepData.id)
            {
                _currentTutorialStepData = _currentTutorialChapterData.GetNextStepData(stepData);
                Debugger.Log("=====[Tutorial][MoveToNextStep]::" + (_currentTutorialChapterData.isBackup ? "[Backup:" : "[Main:") + _currentTutorialChapterData.id.ToString() + "]====================[Step:" + _currentTutorialStepData.id.ToString() + "]");
            }

            if (_currentTutorialStepData == null) //移动到下一章
            {
                MoveToNextChapter();
            }
        }

        public bool IsFirstStep()
        {
            return CurrentTutorialChapterData.IsFirstStep(CurrentTutorialStepData.id);
        }

        public bool IsCompleteStep()
        {
            return CurrentTutorialChapterData.IsComleteStep(CurrentTutorialStepData.id);
        }

        public bool IsAfterCompleteStep()
        {
            return CurrentTutorialChapterData.IsAfterCompleteStep(CurrentTutorialStepData.id);
        }

        public bool IsLastStep()
        {
            return CurrentTutorialChapterData.IsLastStep(CurrentTutorialStepData.id);
        }
    }
}