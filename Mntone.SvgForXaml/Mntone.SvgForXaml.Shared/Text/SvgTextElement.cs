using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Internal;
using Mntone.SvgForXaml.Primitives;
using Windows.Data.Xml.Dom;

namespace Mntone.SvgForXaml.Text
{
    public sealed class SvgTextElement : SvgElement, ISvgStylable, ISvgTransformable        
    {
        internal SvgTextElement(INode parent, XmlElement element)
            : base(parent, element)
        {
            this._stylableHelper = new SvgStylableHelper(this, element);
            this._transformableHelper = new SvgTransformableHelper(element);

            //NOTE: greatly simplified flattened text model
            this.TextContent = element.InnerText; 

            this.X = element.ParseCoordinate("x", 0.0F);
            this.Y = element.ParseCoordinate("y", 0.0F);
        }

        protected override void DeepCopy(SvgElement element)
        {
            var casted = (SvgTextElement)element;
            casted.TextContent = this.TextContent;
            casted._stylableHelper = this._stylableHelper.DeepCopy(casted);
            casted._transformableHelper = this._transformableHelper.DeepCopy();
        }

        public override string TagName => "text";

        public SvgLength X { get; }
        public SvgLength Y { get; }

        public string TextContent { get; internal set; }

        #region ISvgStylable
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private SvgStylableHelper _stylableHelper;
        public string ClassName => this._stylableHelper.ClassName;
        public CssStyleDeclaration Style => this._stylableHelper.Style;
        public ICssValue GetPresentationAttribute(string name) => this._stylableHelper.GetPresentationAttribute(name);
        #endregion

        #region ISvgTransformable
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private SvgTransformableHelper _transformableHelper;
        public SvgTransformCollection Transform => this._transformableHelper.Transform;
        #endregion
    }
}
