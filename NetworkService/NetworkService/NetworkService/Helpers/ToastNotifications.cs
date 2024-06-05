using Notification.Wpf;

namespace NetworkService.Helpers
{
    public static class ToastNotifications
    {
        static readonly NotificationManager NotificationManager;
        static ToastNotifications()
        {
            NotificationManager = new NotificationManager();
        }

        public static void RaiseToast(string mainText, string description, NotificationType type)
        {
            NotificationManager.Show(mainText, description, type, "MainNotificationArea");
        }
    }
}
