using UnityEngine;

namespace Common.Animators
{
	public class AnimationEventLuaForwarder : MonoBehaviour
	{
		public const string name = "AnimationEventLuaForwarder";

		public void OnAnimationEventTriggered (string eventName)
		{
			Debugger.Log(string.Format("{0}::{1}", "OnAnimationEventTriggered", eventName));
			Observers.Facade.Instance.SendNotification("AnimationEventLuaForwarder::OnAnimationEventTriggered", eventName);
		}
	}
}
