using Mntone.SvgForXaml.Interfaces;
using Mntone.SvgForXaml.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mntone.SvgForXaml
{
	public sealed class CssStyleDeclaration
	{
		private static readonly string[] NON_INHERIT_PROPERTIES = { "fill", "stroke" };

		private readonly ISvgStylable _parent;
		private readonly List<string> _items;
		private readonly Dictionary<string, Tuple<string, ICssValue>> _cache;

		private CssStyleDeclaration(ISvgStylable parent, List<string> items, Dictionary<string, Tuple<string, ICssValue>> cache)
		{
			this._parent = parent;
			this._items = items;
			this._cache = cache;
		}

		internal CssStyleDeclaration(ISvgStylable parent, string css)
		{
			this._parent = parent;
			this._items = new List<string>();
			this._cache = new Dictionary<string, Tuple<string, ICssValue>>();
			this.ParseText(css);
		}

		internal CssStyleDeclaration DeepCopy(ISvgStylable parent)
		{
			var item = new List<string>(this._items);
			var cache = new Dictionary<string, Tuple<string, ICssValue>>();
			foreach (var c in this._cache)
			{
				ICssValue value;
				var target = c.Value.Item2;
				if (target.GetType() == typeof(SvgPaint) || target.GetType() == typeof(SvgColor))
				{
					value = ((SvgColor)c.Value.Item2).Clone();
				}
				else if (target.GetType() == typeof(SvgNumber) || target.GetType() == typeof(SvgLength))
				{
					value = c.Value.Item2;
				}
				else if (target.GetType() == typeof(SvgIri))
				{
					value = new SvgIri(((SvgIri)c.Value.Item2).Uri);
				}
				else
				{
					throw new InvalidOperationException();
				}
				cache.Add(c.Key, Tuple.Create(c.Value.Item1, value));
			}
			return new CssStyleDeclaration(parent, item, cache);
		}

		public string GetPropertyValue(string propertyName) => this.GetPropertyValuePrivate(propertyName)?.Item1;
		public ICssValue GetPropertyCssValue(string propertyName) => this.GetPropertyValuePrivate(propertyName)?.Item2;

		public void SetProperty(string propertyName, string value, string priority, bool presentation)
		{
			this.ParseValue(propertyName, value, priority, presentation);
		}
		public void SetProperty(string propertyName, string value)
		{
			SetProperty(propertyName, value, string.Empty, false);
		}

		public void SetProperty(string propertyName, ICssValue value, string priority, bool presentation)
		{
			SetCacheValue(propertyName, value.ToString(), value, priority, presentation, true);
		}

        public void SetProperty(string propertyName, ICssValue value)
        {
            SetProperty(propertyName, value, string.Empty, false);
        }

		private Tuple<string, ICssValue> GetPropertyValuePrivate(string propertyName)
		{
			if (!this._cache.ContainsKey(propertyName))
			{
				if (this._parent.StyleInheritanceBehavior == StyleInheritanceBehavior.All ||
                    !NON_INHERIT_PROPERTIES.Any(p => p == propertyName))
				{
					var target = ((INode)this._parent)?.ParentNode as ISvgStylable;
					if (target != null)
					{
						return target.Style.GetPropertyValuePrivate(propertyName);
					}
				}
				return null;
			}
			return this._cache[propertyName];
		}

		public string this[ulong index] => this._items[(int)index];

		public SvgPaint Fill
		{
            get { return this.GetPropertyCssValue("fill") as SvgPaint; }
            set { SetProperty("fill", value); }
		}
		public SvgNumber? FillOpacity
		{
            get { return this.GetPropertyCssValue("fill-opacity") as SvgNumber?; }
            set { SetProperty("fill-opacity", value); }
		}
		public SvgFillRule? FillRule
		{
            get { return this.GetPropertyCssValue("fill-rule") as SvgFillRule?; }
            set { SetProperty("fill-rule", value); }
		}
		public SvgPaint Stroke
		{
            get { return this.GetPropertyCssValue("stroke") as SvgPaint; }
            set { SetProperty("stroke", value); }
		}
        public SvgLength? StrokeWidth
        {
            get { return this.GetPropertyCssValue("stroke-width") as SvgLength?; }
            set { SetProperty("stroke-width", (ICssValue)value); }
        }
        public SvgNumber? StrokeOpacity
        {
            get { return this.GetPropertyCssValue("stroke-opacity") as SvgNumber?; }
            set { SetProperty("stroke-opacity", value); }
        }
        public SvgColor StopColor
        {
            get { return this.GetPropertyCssValue("stop-color") as SvgColor; }
            set { SetProperty("stop-color", value); }
        }
        public SvgNumber? StopOpacity
        {
            get { return this.GetPropertyCssValue("stop-opacity") as SvgNumber?; }
            set { SetProperty("stop-opacity", value); }
        }
        public SvgIri ClipPath
        {
            get { return this.GetPropertyCssValue("clip-path") as SvgIri; }
            set { SetProperty("clip-path", value); }
        }
        public SvgNumber? Opacity
        {
            get { return this.GetPropertyCssValue("opacity") as SvgNumber?; }
            set { SetProperty("opacity", value); }
        }
        public SvgLineCap LineCap
        {
            get { return this.GetPropertyCssValue("stroke-linecap") as SvgLineCap; }
            set { SetProperty("stroke-linecap", value); }
        }
        public SvgLineJoin LineJoin
        {
            get { return this.GetPropertyCssValue("stroke-linejoin") as SvgLineJoin; }
            set { SetProperty("stroke-linejoin", value); }
        }
        public SvgNumber? FontSize
        {
            get { return this.GetPropertyCssValue("font-size") as SvgNumber?; }
            set { SetProperty("font-size", value); }
        }

		private void ParseText(string css)
		{
			if (string.IsNullOrWhiteSpace(css)) return;

			var props = css.Split(new[] { ';' }).Where(p => !string.IsNullOrEmpty(p)).Select(prop =>
			{
				var kv = prop.Split(new[] { ':' }).ToArray();
				if (kv.Length != 2) throw new Exception();
				return new KeyValuePair<string, string>(kv[0].Trim(), kv[1].Trim());
			});
			foreach (var prop in props)
			{
				var result = this.ParseValue(prop.Key, prop.Value, string.Empty, false);
				this._items.Add(result.Item1);
			}
		}

		private Tuple<string, ICssValue> ParseValue(string name, string value, string priority, bool presentation)
		{
			if (!presentation) name = name.ToLower();

			ICssValue parsedValue = null;
			switch (name)
			{
				case "fill":
				case "stroke":
					parsedValue = new SvgPaint(value);
					break;

				case "stroke-width":
					parsedValue = SvgLength.Parse(value, presentation);
					break;

				case "stop-color":
					parsedValue = new SvgColor(value);
					break;

				case "fill-opacity":
				case "stroke-opacity":
				case "stop-opacity":
				case "opacity":
					parsedValue = SvgNumber.Parse(value, 0.0F, 1.0F);
					break;

				case "clip-path":
					parsedValue = new SvgIri(value);
					break;

				case "fill-rule":
				case "clip-rule":
					parsedValue = new SvgFillRule(presentation ? value : value.ToLower());
					break;

                case "stroke-linecap":
                    parsedValue = new SvgLineCap(presentation ? value : value.ToLower());
                    break;
                case "stroke-linejoin":
                    parsedValue = new SvgLineJoin(presentation ? value : value.ToLower());
                    break;

                case "font-size":
                    parsedValue = SvgNumber.Parse(value);
                    break;
			}


            return SetCacheValue(name, value, parsedValue, priority, presentation, false);
		}

        private Tuple<string, ICssValue> SetCacheValue(string name, string value, ICssValue parsedValue, string priority, bool presentation, bool replaceValue)
        {
            var important = priority == "important" || replaceValue;
            if (!presentation) name = name.ToLower();

            if (!this._cache.ContainsKey(name))
            {
                var result = Tuple.Create(value, parsedValue);
                this._cache.Add(name, result);
                return result;
            }
            else if (important)
            {
                var result = Tuple.Create(value, parsedValue);
                this._cache[name] = result;
                return result;
            }

			return null;
		}
	}
}