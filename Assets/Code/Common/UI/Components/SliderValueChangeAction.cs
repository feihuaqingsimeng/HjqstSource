using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Common.UI.Components
{
	public class SliderValueChangeAction : MonoBehaviour 
	{
		
		public static SliderValueChangeAction Get(GameObject go)
		{
			if(go == null)
				return null;
			SliderValueChangeAction sliderAction = go.GetComponent<SliderValueChangeAction>();
			if(sliderAction == null)
				sliderAction = go.AddComponent<SliderValueChangeAction>();
			return sliderAction;
		}

		private Slider _slider;
		private System.Action<float> onUpdateValueDelegate;
		private System.Action onUpdateReach100PercentDelegate;
		private Queue<float> _rateQueue = new Queue<float>();
		private Queue<float> _duringTimeQueue = new Queue<float>();
		private Queue<bool> _isPerLineQueue = new Queue<bool>();
		private bool _isComplete = true;
		private int _valueInt = 0;
		/// <summary>
		/// Starts the value change action.
		/// </summary>
		/// <param name="toRate">结束时的value值，可循环：如1.1表示value会先涨到1再从0涨到0.1</param>
		/// <param name="duringTime">During time.</param>
		public SliderValueChangeAction StartValueChangeActionTo(float toRate,float duringTime)
		{
            //int times = (int)toRate;
            //float end = toRate-times;
			if(_slider == null)
				_slider = GetComponent<Slider>();
			_valueInt = 0;
			LeanTween.cancel(gameObject);
			LeanTween.value(gameObject,_slider.value,toRate,duringTime).setOnUpdate(UpdateValue);
			return this;
		}
		//isPerLineTime ：true duringTime 是一行时间
		public SliderValueChangeAction StartValueChangeActionList(float byRate,float duringTime,bool isPerLineTime = true)
		{
			if(_slider == null)
				_slider = GetComponent<Slider>();
			_rateQueue.Enqueue(byRate);
			_duringTimeQueue.Enqueue(duringTime);
			_isPerLineQueue.Enqueue(isPerLineTime);
			int count = _rateQueue.Count;
			if(count == 1 && _isComplete)
			{
				_isComplete = false;
				OnComplete();
			}

			return this;
		}
		public void CancelAction()
		{
			LeanTween.cancel(gameObject);
			_rateQueue.Clear();
			_duringTimeQueue.Clear();
			_isPerLineQueue.Clear();
			_isComplete = true;
		}
		public SliderValueChangeAction SetValueChangeUpdate(System.Action<float> action)
		{
			onUpdateValueDelegate = action;
			return this;
		}
		public SliderValueChangeAction SetReach100PercentUpdate(System.Action action)
		{
			onUpdateReach100PercentDelegate = action;
			return this;
		}
		private void UpdateValue(float t)
		{
			int v = (int)t;
			if(t>1 && t != v)
				_slider.value = t-v;
			else
				_slider.value = t;
			if(onUpdateValueDelegate!= null)
				onUpdateValueDelegate(t);
			if (v > _valueInt )
			{
				//Debugger.LogError("v:{0},valueInt:{0}",v,_valueInt);
				_valueInt = v;

				if(onUpdateReach100PercentDelegate != null)
					onUpdateReach100PercentDelegate();
			}
		}
		private void OnComplete()
		{
			if(_rateQueue.Count == 0){
				_isComplete = true;
				return;
			}
			_valueInt = 0;
			float byRate = _rateQueue.Dequeue();
			float duringTime = _duringTimeQueue.Dequeue();
			bool isPerLine = _isPerLineQueue.Dequeue();
            //int times = (int)byRate;
            //float end = byRate-times;
			_valueInt = 0;
			float value = _slider.value;
			if (value >= 1)
				value = 0;
			if(isPerLine)
			{
				duringTime =  (byRate-value)*duringTime;
			}
			//Debugger.LogError("[OnComplete],value:{0},byRate:{1},duringTime:{2}",value,byRate,duringTime);
			LeanTween.value(gameObject,value,byRate,duringTime).setOnUpdate(UpdateValue).onComplete = OnComplete;
		}
	}
}

