﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MoreShipUpgrades.Misc.UI
{
    internal class TextElement : ITextElement
    {
        internal string Text { get; set; }
        public string GetText(int availableLength)
        {
            return Text; // TODO: wrap with availableLength
        }
    }
}
