public static class LuckySpinDatas
{
    public static string LuckySpinTimeStart
    {
        get => Data.GetString(LuckySpinConstants.LUCKY_SPIN_TIME_START, "");

        set => Data.SetString(LuckySpinConstants.LUCKY_SPIN_TIME_START, value);
    }
}
