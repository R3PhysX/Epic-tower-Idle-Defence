using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Get;

    private void Awake()
    {
        Get = this;
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        if (!Permission.HasUserAuthorizedPermission("android.permission.USE_EXACT_ALARM"))
        {
            Permission.RequestUserPermission("android.permission.USE_EXACT_ALARM");
        }
#endif


#if UNITY_IOS
        StartCoroutine(RequestAuthorization());
#endif

    }

    private void Start()
    {
#if UNITY_ANDROID
        AndroidNotificationChannel channel = new AndroidNotificationChannel()
        {
            Id = "IDT_Channel",
            Name = "IDT_Channel",
            Importance = Importance.High,
            Description = "IDT_Channel",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
#endif
    }

    public void ScheduleNotification_RewardComplete(int delayInSeconds)
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.USE_EXACT_ALARM"))
        {
            return;
        }

        AndroidNotification androidNotification = new AndroidNotification
        {
            Title = "Full! Ready to Collect",
            Text = "Tap to claim and fortify your Tower's defences!",
            FireTime = System.DateTime.Now.AddSeconds(delayInSeconds),
            SmallIcon = "icon_0",
            LargeIcon = "icon_1"
        };

        CancelNotificationReward();
        int id = AndroidNotificationCenter.SendNotification(androidNotification, "IDT_Channel");

        PlayerPrefs.SetInt("IdleTowerDefence_RewardNotification", id);

#endif


#if UNITY_IOS

        int hours, minutes, seconds; 
        
        hours = delayInSeconds / 3600;
        int remainingSeconds = delayInSeconds % 3600;
        minutes = remainingSeconds / 60;
        seconds = remainingSeconds % 60;

        if (hours < 0 && minutes < 0 && seconds < 0)
            return;

        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new System.TimeSpan(hours, minutes, seconds),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            // You can specify a custom identifier which can be used to manage the notification later.
            // If you don't provide one, a unique string will be generated automatically.
            Identifier = "IDT_Channel_1",
            Title = "Full! Ready to Collect",
            Body = "Tap to claim and fortify your Tower's defences!",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        CancelNotificationReward();
        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }

    public void ScheduleNotification_NotActive()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.USE_EXACT_ALARM"))
        {
            return;
        }


        AndroidNotification androidNotification = new AndroidNotification
        {
            Title = "Tower in Peril!",
            Text = "Attention, Commander!  your Tower is under threat!",
            FireTime = System.DateTime.Now.AddSeconds(57600),
            RepeatInterval = new System.TimeSpan(12, 0, 0),
            SmallIcon = "icon_0",
            LargeIcon = "icon_1"
        };

        CancelNotificationNoActivity();
        int id = AndroidNotificationCenter.SendNotification(androidNotification, "IDT_Channel");

        PlayerPrefs.SetInt("IdleTowerDefence_NoActivityNotification", id);
#endif



#if UNITY_IOS

        int hours, minutes, seconds;
        int schudleTime = 57600;
        hours = schudleTime / 3600;
        int remainingSeconds = schudleTime % 3600;
        minutes = remainingSeconds / 60;
        seconds = remainingSeconds % 60;

        if (hours < 0 && minutes < 0 && seconds < 0)
            return;

        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new System.TimeSpan(hours, minutes, seconds),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            // You can specify a custom identifier which can be used to manage the notification later.
            // If you don't provide one, a unique string will be generated automatically.
            Identifier = "IDT_Channel_2",
            Title = "Tower in Peril!",
            Body = "Attention, Commander!  your Tower is under threat!",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        CancelNotificationNoActivity();
        iOSNotificationCenter.ScheduleNotification(notification);
#endif

    }

    // Cancel a specific notification using its identifier
    private void CancelNotification(int notificationId)
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission("android.permission.USE_EXACT_ALARM"))
        {
            return;
        }

        AndroidNotificationCenter.CancelNotification(notificationId);
#endif
    }

    public void CancelNotificationReward()
    {
#if UNITY_ANDROID
        if (PlayerPrefs.HasKey("IdleTowerDefence_RewardNotification"))
            CancelNotification(PlayerPrefs.GetInt("IdleTowerDefence_RewardNotification"));
#endif

#if UNITY_IOS
        iOSNotificationCenter.RemoveScheduledNotification("IDT_Channel_1");
#endif
    }

    public void CancelNotificationNoActivity()
    {
#if UNITY_ANDROID
        if (PlayerPrefs.HasKey("IdleTowerDefence_NoActivityNotification"))
            CancelNotification(PlayerPrefs.GetInt("IdleTowerDefence_NoActivityNotification"));
#endif

#if UNITY_IOS
        iOSNotificationCenter.RemoveScheduledNotification("IDT_Channel_2");
#endif
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            ScheduleNotification_NotActive();
        }
    }


#if UNITY_IOS
    IEnumerator RequestAuthorization()
    {
        var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
        using (var req = new AuthorizationRequest(authorizationOption, true))
        {
            while (!req.IsFinished)
            {
                yield return null;
            };
        }
    }

    public bool getIOSNotificationPermission()
    {
        return iOSNotificationCenter.GetNotificationSettings().AuthorizationStatus == AuthorizationStatus.Authorized;
    }
#endif

}