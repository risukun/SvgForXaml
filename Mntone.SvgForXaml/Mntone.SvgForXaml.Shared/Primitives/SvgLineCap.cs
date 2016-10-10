using Mntone.SvgForXaml.Interfaces;
using System;

namespace Mntone.SvgForXaml.Primitives
{
    public class SvgLineCap : ICssValue
    {
        public SvgLineCap(string value)
        {
            if (string.Compare("round", value, StringComparison.OrdinalIgnoreCase) == 0)
            {
                LineCapType = SvgLineCapType.Round;
            }
            else if (string.Compare("square", value, StringComparison.OrdinalIgnoreCase) == 0)
            {
                LineCapType = SvgLineCapType.Square;
            }
            else
            {
                LineCapType = SvgLineCapType.Flat;
            }
        }

        public SvgLineCapType LineCapType { get; private set; }
    }
}
