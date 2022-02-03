using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Extension
{
    public static class TimeFormatterExtension
    {
        public static string ConvertToTime(this int time)
        {
            var timeStringBuilder = new StringBuilder();
            if (time >= 10)
                timeStringBuilder.Append(time).Append(":00");
            else
                timeStringBuilder.Append("0").Append(time).Append(":00");
            return timeStringBuilder.ToString();
        }
    }
}
