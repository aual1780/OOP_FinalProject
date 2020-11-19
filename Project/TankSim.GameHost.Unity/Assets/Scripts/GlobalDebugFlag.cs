using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIPC.Core.Channels;

public static class GlobalDebugFlag
{
    public static bool IsDebug { get; set; } = false;

    public static MessageCategoryTypes LogTypes = MessageCategoryTypes.All;
}
