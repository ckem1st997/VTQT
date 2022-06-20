using System;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace VTQT.Web.Framework.UI
{

    public abstract class ComponentRenderer<TComponent> : IHtmlContent where TComponent : Component
    {

        protected ComponentRenderer()
        {
        }

        protected ComponentRenderer(TComponent component)
        {
            this.Component = component;
        }

        protected internal TComponent Component
        {
            get;
            set;
        }

        protected internal IHtmlHelper HtmlHelper
        {
            get;
            internal set;
        }

        protected internal ViewContext ViewContext
        {
            get;
            internal set;
        }

        protected internal ViewDataDictionary ViewData
        {
            get;
            internal set;
        }

        public virtual void VerifyState()
        {
            Guard.NotNull(() => this.Component);
            if (this.Component.NameIsRequired && !this.Component.Id.HasValue())
            {
                throw Error.InvalidOperation("A component must have a unique 'Name'. Please provide a name.");
            }
        }

        protected void WriteHtml(TextWriter writer)
        {
            this.VerifyState();
            this.Component.Id = SanitizeId(this.Component.Id);

            this.WriteHtmlCore(writer);
        }

        protected virtual void WriteHtmlCore(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            this.WriteHtml(this.ViewContext.Writer);
        }

        public string ToHtmlString()
        {
            string str;
            using (var stringWriter = new StringWriter())
            {
                this.WriteHtml(stringWriter);
                str = stringWriter.ToString();
            }
            return str;
        }


        protected string SanitizeId(string id)
        {
            return id.SanitizeHtmlId();
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            string str;
            using (var stringWriter = new StringWriter())
            {
                this.WriteHtml(stringWriter);
                str = stringWriter.ToString();

                writer.Write(str);
            }
        }
    }

}
