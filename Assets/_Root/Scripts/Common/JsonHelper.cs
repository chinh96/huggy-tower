public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        var wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.items;
    }

    public static string ToJson<T>(T[] array)
    {
        var wrapper = new Wrapper<T> {items = array};
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    public static string FixJson(string value)
    {
        return "{\"items\":" + value + "}";
    }

    public static string RemoveItems(string value)
    {
        value = value.Replace("{\"items\":", "");
        value = value.Remove(value.Length - 1, 1);
        return value;
    }

    public static string RemoveArray(string value)
    {
        value = value.Remove(0, 1);
        value = value.Remove(value.Length - 1, 1);
        return value;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}