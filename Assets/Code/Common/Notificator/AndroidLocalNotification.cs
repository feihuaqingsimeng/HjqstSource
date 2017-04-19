using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace Common.Notificator
{
    public class AndroidLocalNotification
    {
        /*
        private static AndroidJavaObject m_ANObj = null;

        //实例JO
        private static bool InitNotificator()
        {
            if (m_ANObj == null)
            {
                try
                {
                    m_ANObj = new AndroidJavaObject("com.dowan.hjqst.shunwang.AndroidNotificator");//自己的java类
                }
                catch
                {
                    return false;
                }
            }

            if (m_ANObj == null)
            {
                return false;
            }

            return true;
        }

        //推送
        public static void PushNotification(string pAppName, string pTitle, string pContent, int pDelaySecond, bool pIsDailyLoop)
        {
#if UNITY_ANDROID
            if (InitNotificator())
            {
                m_ANObj.CallStatic(
                    "ShowNotification",
                    pAppName,
                    pTitle,
                    pContent,
                    pDelaySecond,
                    pIsDailyLoop);
            }
#endif
        }

        public static void ClearNotifications()
        {
#if UNITY_ANDROID
            if (InitNotificator())
            {
                m_ANObj.CallStatic("ClearNotification");
            }
#endif
        }*/
        /// <summary>
        /// Inexact uses `set` method
        /// Exact uses `setExact` method
        /// ExactAndAllowWhileIdle uses `setAndAllowWhileIdle` method
        /// Documentation: https://developer.android.com/intl/ru/reference/android/app/AlarmManager.html
        /// </summary>
        public enum NotificationExecuteMode
        {
            Inexact = 0,
            Exact = 1,
            ExactAndAllowWhileIdle = 2
        }

#if UNITY_ANDROID && !UNITY_EDITOR
    private static string fullClassName = "net.agasper.unitynotification.UnityNotificationManager";
    private static string mainActivityClassName = "com.dowan.hjqst.shunwang.UnityPlayerNativeActivity";
#endif

        public static void SendNotification(int id, TimeSpan delay, string title, string message)
        {
            SendNotification(id, (int)delay.TotalSeconds, title, message, Color.white);
        }

        public static void SendNotification(int id, long delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetNotification", id, delay * 1000L, title, message, message, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "app_icon", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, (int)executeMode, mainActivityClassName);
        }
#endif
        }

        public static void SendRepeatingNotification(int id, long delay, long timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetRepeatingNotification", id, delay * 1000L, title, message, message, timeout * 1000, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "app_icon", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, mainActivityClassName);
        }
#endif
        }

        public static void CancelNotification(int id)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null) {
            pluginClass.CallStatic("CancelNotification", id);
        }
#endif
        }

//        public static void CancelAllNotifications()
//        {
//#if UNITY_ANDROID && !UNITY_EDITOR
//            AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
//            if (pluginClass != null)
//                pluginClass.CallStatic("CancelAll");
//#endif
//        }
    }
}