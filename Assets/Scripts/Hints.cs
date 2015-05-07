using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
static class Hints
{
    private static int level;
    private static int hintNum;
    private static int hintmax;
    public static int Level
    {
        get { return level; }
        set
        {
            level = value;
            hintNum = 0;
            hintmax = (level >= hints.Length)?0 : hints[level].Length;
        }
    }

    public static string GetHint()
    {
        if (hintmax == 0) return "";
        var ret = hints[level][hintNum];
        hintNum = (hintNum + 1)%hintmax;
        return ret;
    }

    private static String[][] hints =
    {
        new string[]{"Use the Number Keys!","Stay inside him when he's happy" },
        new string[]{"You have to keep them in their color.","Press 4 to paint black.","Without the Loops you'll fail." },
        new string[]{"The head makes them bounce","They won't last in a short loop.", "Press 4 to paint black" },
        new string[]{"The head makes them bounce","They won't last in a short loop.", "Press 4 to paint black" },
        new string[]{"The head makes them bounce","They won't last in a short loop.", "Press 4 to paint black" },

    };


}
