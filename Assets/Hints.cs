using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
static class Hints
{
    private static int level;
    public static int hintNum;
    private static int hintmax;
    public static int Level
    {
        get { return level; }
        set
        {
            level = value;
            hintNum = 0;
            if (level >= hints.Length)
            {
                hintNum = hintmax = 0;
                return;
            }
            
        }
    }

    public static String[][] hints;


}
