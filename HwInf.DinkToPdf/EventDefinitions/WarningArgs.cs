﻿using DinkToPdf.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinkToPdf.EventDefinitions
{
    public class WarningArgs:EventArgs
    {
        public IDocument Document { get; set; }

        public string Message { get; set; }
    }
}
