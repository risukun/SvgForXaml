using System;
using System.Collections.Generic;

namespace Mntone.SvgForXaml.Interfaces
{
	public interface INode
	{
		SvgDocument OwnerDocument { get; }
		INode ParentNode { get; }
		IReadOnlyCollection<SvgElement> ChildNodes { get; }
		SvgElement FirstChild { get; }
		SvgElement LastChild { get; }
        StyleInheritanceBehavior StyleInheritanceBehavior { get; }
        IList<T> FindDescendants<T>() where T : SvgElement;

		INode CloneNode(bool deep = false);
	}
}