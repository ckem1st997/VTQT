using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using VTQT.Core;
using VTQT.Core.Infrastructure;

namespace VTQT.Web.Framework.UI
{

    public abstract class ComponentBuilder<TComponent, TBuilder> : IHtmlContent, IHideObjectMembers
        where TComponent : Component
        where TBuilder : ComponentBuilder<TComponent, TBuilder>
    {
        private ComponentRenderer<TComponent> _renderer;

        protected ComponentBuilder(TComponent component, IHtmlHelper htmlHelper)
        {
            Guard.ArgumentNotNull(() => component);
            Guard.ArgumentNotNull(() => htmlHelper);

            this.Component = component;
            this.HtmlHelper = htmlHelper;
        }

        protected internal IHtmlHelper HtmlHelper
        {
            get;
            private set;
        }

        protected internal TComponent Component
        {
            get;
            private set;
        }

        public TComponent ToComponent()
        {
            return this.Component;
        }

        protected ComponentRenderer<TComponent> Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = EngineContext.Current.Resolve<ComponentRenderer<TComponent>>();
                    EnrichRenderer(_renderer);
                }
                return _renderer;
            }
            private set
            {
                _renderer = value;
                if (_renderer != null)
                {
                    EnrichRenderer(_renderer);
                }
            }
        }

        private void EnrichRenderer(ComponentRenderer<TComponent> renderer)
        {
            renderer.Component = this.Component;
            renderer.HtmlHelper = this.HtmlHelper;
            renderer.ViewContext = this.HtmlHelper.ViewContext;
            renderer.ViewData = this.HtmlHelper.ViewData;
        }

        public TBuilder WithRenderer<T>()
            where T : ComponentRenderer<TComponent>
        {
            return this.WithRenderer(typeof(T));
        }

        public TBuilder WithRenderer<T>(ComponentRenderer<TComponent> instance)
            where T : ComponentRenderer<TComponent>
        {
            Guard.ArgumentNotNull(() => instance);
            return this.WithRenderer(typeof(T));
        }

        public TBuilder WithRenderer(Type rendererType)
        {
            Guard.ArgumentNotNull(() => rendererType);
            Guard.Implements<ComponentRenderer<TComponent>>(rendererType);
            var renderer = Activator.CreateInstance(rendererType) as ComponentRenderer<TComponent>;
            if (renderer != null)
            {
                this.Renderer = renderer;
            }
            return this as TBuilder;
        }

        public virtual TBuilder Name(string name)
        {
            this.Component.Name = name;
            return this as TBuilder;
        }

        public virtual TBuilder HtmlAttributes(object attributes)
        {
            return this.HtmlAttributes(CommonHelper.ObjectToDictionary(attributes));
        }

        public virtual TBuilder HtmlAttributes(IDictionary<string, object> attributes)
        {
            this.Component.HtmlAttributes.Merge(attributes);
            return this as TBuilder;
        }

        public string ToHtmlString()
        {
            return this.Renderer.ToHtmlString();
        }

        public override string ToString()
        {
            return this.ToHtmlString();
        }

        public virtual void Render()
        {
            this.Renderer.Render();
        }

        public static implicit operator TComponent(ComponentBuilder<TComponent, TBuilder> builder)
        {
            return builder.ToComponent();
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            this.Renderer.WriteTo(writer, encoder);
        }

    }

}
