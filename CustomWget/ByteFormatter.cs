namespace CustomWget;

internal static class ByteFormatter
{
    internal static string Format(double bytes)
    {
        string[] suffix = { "B", "KB", "MB", "GB", "TB" };
        var dblSByte = bytes;
        int i;
        for (i = 0; i < suffix.Length && bytes >= 1024; i++, bytes /= 1024)
        {
            dblSByte = bytes / 1024.0;
        }
        return $"{dblSByte:0.##} {suffix[i]}";
    }
}