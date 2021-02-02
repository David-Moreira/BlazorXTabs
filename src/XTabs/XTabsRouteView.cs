using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BlazorXTabs.Configuration;

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

        private Dictionary<string, RenderFragment> _renderDelegates;

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
        /// Gets or sets the XTabs RenderMode.
        /// <code>Defaults to: <see cref="RenderMode.Partial" /></code>
        /// </summary>
        [Parameter] public RenderMode RenderMode { get; set; } = RenderMode.Partial;

        /// <summary>
        /// Can close tabs.
        /// </summary>
        [Parameter] public bool CloseTabs { get; set; }

        /// <summary>
        /// When a new tab is added, sets it to active.
        /// </summary>
        [Parameter] public bool NewTabSetActive { get; set; }


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

        private RenderFragment RenderNewPage(out string xTabTitle)
        {
            //Let's make sure not to capture the RouteData reference in the delegate.
            var pageType = RouteData.PageType;
            var values = RouteData.RouteValues;
            xTabTitle = pageType.Name;
            var pageAttr = pageType.GetCustomAttribute<XTabPageAttribute>();
            if (pageAttr is not null)
                xTabTitle = pageAttr.Title;

            return new RenderFragment(rBuilder =>
            {
                rBuilder.OpenComponent(0, pageType);
                foreach (var kvp in values)
                    rBuilder.AddAttribute(1, kvp.Key, kvp.Value);
                rBuilder.CloseComponent();
            });
        }
        private void RenderPageWithParameters(RenderTreeBuilder builder)
        {
            var pageFragment = RenderNewPage(out var xTabTitle);

            if (_xTabs is null)
            {
                var xTabFragment = new RenderFragment(rBuilder =>
                {
                    rBuilder.OpenComponent(0, typeof(XTab));
                    rBuilder.AddAttribute(1, nameof(XTab.ChildContent), pageFragment);
                    rBuilder.AddAttribute(2, nameof(XTab.Title), xTabTitle);
                    rBuilder.CloseComponent();
                });

                _xTabsRenderFragment = new RenderFragment(rBuilder =>
                {
                    rBuilder.OpenComponent<XTabs>(0);

                    rBuilder.AddAttribute(1, nameof(XTabs.RenderMode), RenderMode);
                    rBuilder.AddAttribute(2, nameof(XTabs.CloseTabs), CloseTabs);
                    rBuilder.AddAttribute(3, nameof(XTabs.NewTabSetActive), NewTabSetActive);
                    rBuilder.AddAttribute(4, nameof(XTabs.ChildContent), xTabFragment);

                    rBuilder.AddComponentReferenceCapture(5, compRef => _xTabs = (XTabs)compRef);
                    rBuilder.CloseComponent();

                });
            }
            else
            {
                var xtab = new XTab(_xTabs, xTabTitle, pageFragment);
                _xTabs.AddPage(xtab);
            }
            //Need to figure out how to Cascade XTabs using the RenderTreeBuilder API
            builder.OpenElement(0, "XTabs");
            builder.AddContent(1, _xTabsRenderFragment);
            builder.CloseElement();
        }
    }
}
