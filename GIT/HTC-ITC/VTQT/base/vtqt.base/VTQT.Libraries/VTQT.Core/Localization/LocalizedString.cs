using System;
using Microsoft.AspNetCore.Html;

namespace VTQT.Core.Localization
{
    /// <summary>
    /// Localized string
    /// </summary>
    [Serializable]
    public class LocalizedString : HtmlString
    {
        private readonly string _localized;
        private readonly string _textHint;
        private readonly object[] _args;

        public LocalizedString() : base(string.Empty)
        {
        }

        public LocalizedString(string localized) : base(localized)
        {
            _localized = localized;
        }

        public LocalizedString(string localized, string textHint, object[] args) : base(localized)
        {
            _localized = localized;
            _textHint = textHint;
            _args = args;
        }

        public static LocalizedString TextOrDefault(string text, LocalizedString defaultValue)
        {
            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return new LocalizedString(text);
        }

        public string TextHint => _textHint;

        public object[] Args => _args;

        public string Text => _localized;

        /// <summary>
        /// Returns a js encoded string which already contains delimiters.
        /// </summary>
        public HtmlString JsText => new HtmlString(_localized.EncodeJsString());

        public static implicit operator string(LocalizedString obj)
        {
            return obj.Text;
        }

        public static implicit operator LocalizedString(string obj)
        {
            return new LocalizedString(obj);
        }

        public override string ToString()
        {
            return _localized;
        }

        public string ToHtmlString()
        {
            return _localized;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            if (_localized != null)
                hashCode ^= _localized.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var that = (LocalizedString)obj;
            return string.Equals(_localized, that._localized);
        }
    }
}
