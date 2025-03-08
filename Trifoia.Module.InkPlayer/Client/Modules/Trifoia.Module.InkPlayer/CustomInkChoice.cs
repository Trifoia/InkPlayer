﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Trifoia.Module.InkPlayer
{
    [DebuggerDisplay("\"{Text}\" Tags:[{(Tags?.Count==1?Tags[0]:Tags?.Count)}] Index:{Index}")]
    public class CustomInkChoice
    {
        public string Text { get; set; }
        public List<string> Tags { get; set; } = new();
        public int Index { get; set; }
        public string PathStringOnChoice { get; set; } 
    }
}
