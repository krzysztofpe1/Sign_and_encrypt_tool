namespace SignAndEncyptTool.Utils;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? value)
    {
        if(value == null || value.Length == 0)
            return true;
        return false;
    }
}
