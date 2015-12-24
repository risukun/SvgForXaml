﻿using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml
{
	public sealed class SvgSymbolElement : SvgElement, ISvgStylable
	{
		internal SvgSymbolElement(INode parent, XmlElement element)
			: base(parent, element)
		{
			this._stylableHelper = new SvgStylableHelper(this, element);
		}

		protected override void DeepCopy(SvgElement element)
		{
			var casted = (SvgSymbolElement)element;
			casted._stylableHelper = this._stylableHelper.DeepCopy(casted);
		}

		public override string TagName => "symbol";

		#region ISvgStylable
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private SvgStylableHelper _stylableHelper;
		public string ClassName => this._stylableHelper.ClassName;
		public CssStyleDeclaration Style => this._stylableHelper.Style;
		public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
		#endregion
	}
}