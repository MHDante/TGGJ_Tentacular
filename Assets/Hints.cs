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
        new string[]{"This is a level 0 hint","Zeroooooooooo" },
        new string[]{"This is a level 1 hint","Juanito" },
        new string[]{"This is a level 2 hint","Jacob", "Harvey Dent" },

    };


}
