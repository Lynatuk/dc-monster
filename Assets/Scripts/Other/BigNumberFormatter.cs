using System.Globalization;

public static class BigNumberFormatter
{
    private static readonly string[] postfixes = {
        "", "K", "M", "B", "T", "aa", "bb", "cc", "dd", "ee", "ff",
        "gg", "hh", "ii", "jj", "kk", "ll", "mm", "nn", "oo", "pp",
        "qq", "rr", "ss", "tt", "uu"
    };

    public static string Format(double value, int digits = 2)
    {
        if (value < 1000000)
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberGroupSeparator = " ";
            return value.ToString("N0", culture);
        }

        int i = 0;
        while (value >= 1000 && i < postfixes.Length - 1)
        {
            value /= 1000;
            i++;
        }

        string format = "N" + digits;
        return $"{value.ToString(format)}{postfixes[i]}";
    }
}
