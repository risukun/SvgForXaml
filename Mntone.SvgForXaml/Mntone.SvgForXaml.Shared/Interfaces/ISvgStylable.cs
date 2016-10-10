namespace Mntone.SvgForXaml.Interfaces
{
    public enum StyleInheritanceBehavior
    {
        All,
        ExceptFillAndStroke
    }

	public interface ISvgStylable
	{
		string ClassName { get; }
		CssStyleDeclaration Style { get; }

		ICssValue GetPresentationAttribute(string name);

        StyleInheritanceBehavior StyleInheritanceBehavior { get; }
	}
}