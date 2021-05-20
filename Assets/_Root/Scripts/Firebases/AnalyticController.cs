using Firebase.Analytics;

public static class AnalyticController
{
    private static void LogEvent(string name, Parameter[] param)
    {
        FirebaseAnalytics.LogEvent(name, param);
    }
}