using System;

public class BigNumberStringify
{
    public static String decorate(long number)
    {
        if (number < 1_000)
            return number.ToString();
        else if (number < 1_000_000)
            return (number / 1_000.0).ToString("0.0") + "K";
        else if (number < 1_000_000_000)
            return (number / 1_000_000.0).ToString("0.0") + "M";
        else if (number < 1_000_000_000_000)
            return (number / 1_000_000_000.0).ToString("0.0") + "B";
        else
            return (number / 1_000_000_000_000.0).ToString("0.0") + "T";
    }
}