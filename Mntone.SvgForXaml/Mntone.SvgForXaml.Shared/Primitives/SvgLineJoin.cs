using Mntone.SvgForXaml.Interfaces;
using System;

namespace Mntone.SvgForXaml.Primitives
{
    public class SvgLineJoin : ICssValue
    {
        public SvgLineJoin(string value)
        {
            if (string.Compare("round", value, StringComparison.OrdinalIgnoreCase) == 0)
            {
                LineJoinType = SvgLineJoinType.Round;
            }
            else if (string.Compare("bevel", value, StringComparison.OrdinalIgnoreCase) == 0)
            {
                LineJoinType = SvgLineJoinType.Bevel;
            }
            else // "miter"
            {
                LineJoinType = SvgLineJoinType.Miter;
            }
        }

        public SvgLineJoinType LineJoinType { get; private set; }
    }
}
