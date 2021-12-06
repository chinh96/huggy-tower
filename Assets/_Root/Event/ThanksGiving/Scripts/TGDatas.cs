using System;

public static class TGDatas
{
    public static string LuckySpinTimeStart
    {
        get => Data.GetString(TGConstants.LUCKY_SPIN_TIME_START, "");
        set => Data.SetString(TGConstants.LUCKY_SPIN_TIME_START, value);
    }

    public static int TotalTurkey
    {
        get => Data.GetInt(TGConstants.TOTAL_TURKEY, 0);
        set
        {
            Data.SetInt(TGConstants.TOTAL_TURKEY, value);
            EventController.TurkeyTotalChanged?.Invoke();
        }
    }

    public static int TotalTurkeyText
    {
        get => Data.GetInt(TGConstants.TOTAL_TURKEY_TEXT, 0);
        set
        {
            Data.SetInt(TGConstants.TOTAL_TURKEY_TEXT, value);
            EventController.TurkeyTotalTextChanged?.Invoke();
        }
    }

    public static TimeSpan TimeToTG => new DateTime(2022, 1, 15, 0, 0, 0) - DateTime.Now;
    public static bool IsInTG => DateTime.Now > new DateTime(2021, 11, 15, 0, 0, 0) && new DateTime(2022, 1, 15, 0, 0, 0) > DateTime.Now;
    public static bool IsAfter5Days => (DateTime.Now - new DateTime(2021, 12, 15, 0, 0, 0)).TotalDays > 5;
    public static bool IsAfterTG => DateTime.Now >= new DateTime(2022, 1, 2, 0, 0, 0);

    public static string[] ClaimedItems
    {
        get
        {
            var str = Data.GetString(TGConstants.TOTAL_ITEMS_CLAIMED, "");
            if (str != "")
            {
                var arr = JsonHelper.FromJson<string>(str);
                return arr;
            }

            return new string[] { };
        }
        set
        {
            var str = value;
            Data.SetString(TGConstants.TOTAL_ITEMS_CLAIMED, JsonHelper.ToJson(str));
        }
    }
}
