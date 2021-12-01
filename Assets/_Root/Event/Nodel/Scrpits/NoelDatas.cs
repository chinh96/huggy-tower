using System;

public static class NoelDatas
{
    public static string LuckySpinTimeStart
    {
        get => Data.GetString(TGConstants.LUCKY_SPIN_TIME_START, "");
        set => Data.SetString(TGConstants.LUCKY_SPIN_TIME_START, value);
    }

    public static int TotalSock
    {
        get => Data.GetInt(TGConstants.TOTAL_TURKEY, 0);
        set
        {
            Data.SetInt(TGConstants.TOTAL_TURKEY, value);
            EventController.TurkeyTotalChanged?.Invoke();
        }
    }

    public static int TotalSockText
    {
        get => Data.GetInt(TGConstants.TOTAL_TURKEY_TEXT, 0);
        set
        {
            Data.SetInt(TGConstants.TOTAL_TURKEY_TEXT, value);
            EventController.TurkeyTotalTextChanged?.Invoke();
        }
    }

    public static TimeSpan TimeToNoel => new DateTime(DateTime.Now.Year, 12, 2, 0, 0, 0) - DateTime.Now;
    public static bool IsInNoel => DateTime.Now > new DateTime(DateTime.Now.Year, 11, 15, 0, 0, 0) && new DateTime(DateTime.Now.Year, 12, 2, 0, 0, 0) > DateTime.Now;
    public static bool IsAfter5Days => (DateTime.Now - new DateTime(DateTime.Now.Year, 11, 15, 0, 0, 0)).TotalDays > 5;
    public static bool IsAfterNoel => DateTime.Now >= new DateTime(DateTime.Now.Year, 12, 2, 0, 0, 0);
}
