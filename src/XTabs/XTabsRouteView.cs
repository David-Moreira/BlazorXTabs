using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorXTabs
{

    public class XTabsRouteView : IComponent
    {
        private readonly RenderFragment _renderDelegate;
        private readonly RenderFragment _renderPageWithParametersDelegate;
        private RenderHandle _renderHandle;
        private XTabs _xTabs;
        private RenderFragment _xTabsRenderFragment;

        /// <summary>
        /// Gets or sets the route data. This determines the page that will be
        /// displayed and the parameter values that will be supplied to the page.
        /// </summary>
        [Parameter]
        public RouteData RouteData { get; set; }

        /// <summary>
        /// Gets or sets the type of a layout to be used if the page does not
        /// declare any layout. If specified, the type must implement <see cref="IComponent"/>
        /// and accept a parameter named <see cref="LayoutComponentBase.Body"/>.
        /// </summary>
        [Parameter]
        public Type DefaultLayout { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="RouteView"/>.
        /// </summary>
        public XTabsRouteView()
        {
            // Cache the delegate instances
            _renderDelegate = Render;
            _renderPageWithParametersDelegate = RenderPageWithParameters;
        }

        /// <inheritdoc />
        public void Attach(RenderHandle renderHandle)
            => _renderHandle = renderHandle;

        /// <inheritdoc />
        public Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);

            if (RouteData == null)
            {
                throw new InvalidOperationException($"The {nameof(RouteView)} component requires a non-null value for the parameter {nameof(RouteData)}.");
            }

            _renderHandle.Render(_renderDelegate);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Renders the component.
        /// </summary>
        /// <param name="builder">The <see cref="RenderTreeBuilder"/>.</param>
        protected virtual void Render(RenderTreeBuilder builder)
        {
            var pageLayoutType = RouteData.PageType.GetCustomAttribute<LayoutAttribute>()?.LayoutType
                ?? DefaultLayout;

            builder.OpenComponent<LayoutView>(0);
            builder.AddAttribute(1, nameof(LayoutView.Layout), pageLayoutType);
            builder.AddAttribute(2, nameof(LayoutView.ChildContent), _renderPageWithParametersDelegate);
            builder.CloseComponent();
        }

        private void RenderPageWithParameters(RenderTreeBuilder builder)
        {
            var pageFragment = new RenderFragment(rBuilder =>
            {
                rBuilder.OpenComponent(0, RouteData.PageType);
                foreach (var kvp in RouteData.RouteValues)
                {
                    rBuilder.AddAttribute(1, kvp.Key, kvp.Value);
                }
                rBuilder.CloseComponent();
            });

            if (_xTabs is null)
            {
                var xTabFragment = new RenderFragment(rBuilder =>
                {
                    rBuilder.OpenComponent(0, typeof(XTab));
                    rBuilder.AddAttribute(1, nameof(XTab.ChildContent), pageFragment);
                    rBuilder.AddAttribute(1, nameof(XTab.Title), Guid.NewGuid().ToString());
                    rBuilder.CloseComponent();
                });

                _xTabsRenderFragment = new RenderFragment(rBuilder =>
                {
                    rBuilder.OpenComponent<XTabs>(0);
                    rBuilder.AddAttribute(1, "RenderMode", BlazorXTabs.Configuration.RenderMode.Full);
                    rBuilder.AddAttribute(1, "CloseTabs", true);
                    rBuilder.AddAttribute(1, "NewTabSetActive", true);
                    rBuilder.AddAttribute(1, "IsLoading", false);
                    rBuilder.AddAttribute(1, "ChildContent", xTabFragment);
                    rBuilder.AddComponentReferenceCapture(5, compRef => _xTabs = (XTabs)compRef);
                    rBuilder.CloseComponent();

                });
            }
            else
            {
                var xtab = new XTab(_xTabs, pageFragment)
                {
                    Title = Guid.NewGuid().ToString()
                };
                _xTabs.AddPage(xtab);

                //var xTabFragment = new RenderFragment(rBuilder =>
                //{
                //    rBuilder.OpenComponent(0, typeof(XTab));
                //    rBuilder.AddAttribute(1, nameof(XTab.ChildContent), pageFragment);
                //    rBuilder.AddAttribute(1, nameof(XTab.Title), Guid.NewGuid().ToString());
                //    rBuilder.CloseComponent();

                //});
                //builder.OpenComponent<XTabs>(0);
                //builder.AddAttribute(1, "RenderMode", BlazorXTabs.Configuration.RenderMode.Full);
                //builder.AddAttribute(1, "CloseTabs", true);
                //builder.AddAttribute(1, "NewTabSetActive", true);
                //builder.AddAttribute(1, "IsLoading", false);
                //builder.AddAttribute(1, "ChildContent", _xTabs.ChildContent);
                //builder.AddAttribute(1, "_tabContent", _xTabs?._tabContent);
                //builder.AddComponentReferenceCapture(5, compRef => _xTabs = (XTabs)compRef);
                //builder.CloseComponent();
            }
            builder.OpenElement(0, "Xtabs");
            builder.AddContent(1, _xTabsRenderFragment);
            builder.CloseComponent();


        }
    }
}
