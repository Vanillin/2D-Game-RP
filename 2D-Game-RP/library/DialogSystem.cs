﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoD_Game_RP
{
    public class Phrase
    {
        public string Index;
        public string TaskToStart;
        public string Dialog;
        public List<string> NextIndexes;
        public List<string> NewTasks;
        public List<string> EndingTasks;
    }
}
