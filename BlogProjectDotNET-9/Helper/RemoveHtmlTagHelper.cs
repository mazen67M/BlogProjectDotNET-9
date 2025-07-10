using System;
using System.Text.RegularExpressions;

namespace BlogProjectDotNET_9.Helper;

public static class RemoveHtmlTagHelper
{
    public static string RemoveHtmlTags(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
}
