using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using BlazorXTabs.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorXTabs
{
    /// <summary>
    /// Combines the behaviors of <see cref="AuthorizeView"/> and <see cref="XTabsRouteView"/>,
    /// so that it displays the page matching the specified route but only if the user
    /// is authorized to see it.
    ///
    /// Additionally, this component supplies a cascading parameter of type <see cref="Task{AuthenticationState}"/>,
    /// which makes the user's current authentication state available to descendants.
    /// </summary>

    public sealed class XTabsAuthorizeRouteView : XTabsRouteView
    {
        // We expect applications to supply their own authorizing/not-authorized content, but
        // it's better to have defaults than to make the parameters mandatory because in some
        // cases they will never be used (e.g., "authorizing" in out-of-box server-side Blazor)
        private static readonly RenderFragment<AuthenticationState> _defaultNotAuthorizedContent
            = state => builder => builder.AddContent(0, "Not authorized");
        private static readonly RenderFragment _defaultAuthorizingContent
            = builder => builder.AddContent(0, "Authorizing...");

        private readonly RenderFragment _renderAuthorizeRouteViewCoreDelegate;
        private readonly RenderFragment<AuthenticationState> _renderAuthorizedDelegate;
        private readonly RenderFragment<AuthenticationState> _renderNotAuthorizedDelegate;
        private readonly RenderFragment _renderAuthorizingDelegate;

        /// <summary>
        /// Tracks whether the component has been authorized by tracking the calls to the Authorized Delegate.
        /// This serves as an instruction to XTabs to know when to reset.
        /// </summary>
        private bool _isAuthorized;

        /// <summary>
        /// Initialize a new instance of a <see cref="AuthorizeRouteView"/>.
        /// </summary>
        public XTabsAuthorizeRouteView()
        {
            // Cache the rendering delegates so that we only construct new closure instances
            // when they are actually used (e.g., we never prepare a RenderFragment bound to
            // the NotAuthorized content except when you are displaying that particular state)
            _renderAuthorizedDelegate = authenticationState => RenderAuthorizedInDefaultLayout;
            _renderNotAuthorizedDelegate = authenticationState => builder => RenderNotAuthorizedInDefaultLayout(builder, authenticationState);
            _renderAuthorizingDelegate = RenderAuthorizingInDefaultLayout;
            _renderAuthorizeRouteViewCoreDelegate = RenderAuthorizeRouteViewCore;
        }

        /// <summary>
        /// The content that will be displayed if the user is not authorized.
        /// </summary>
        [Parameter]
        public RenderFragment<AuthenticationState>? NotAuthorized { get; set; }

        /// <summary>
        /// The content that will be displayed while asynchronous authorization is in progress.
        /// </summary>
        [Parameter]
        public RenderFragment? Authorizing { get; set; }

        /// <summary>
        /// The resource to which access is being controlled.
        /// </summary>
        [Parameter]
        public object? Resource { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? ExistingCascadedAuthenticationState { get; set; }

        /// <inheritdoc />
        protected override void Render(RenderTreeBuilder builder)
        {
            if (ExistingCascadedAuthenticationState != null)
            {
                // If this component is already wrapped in a <CascadingAuthenticationState> (or another
                // compatible provider), then don't interfere with the cascaded authentication state.
                _renderAuthorizeRouteViewCoreDelegate(builder);
            }
            else
            {
                // Otherwise, implicitly wrap the output in a <CascadingAuthenticationState>
                builder.OpenComponent<CascadingAuthenticationState>(0);
                builder.AddAttribute(1, nameof(CascadingAuthenticationState.ChildContent), _renderAuthorizeRouteViewCoreDelegate);
                builder.CloseComponent();
            }
        }

        private void RenderAuthorizeRouteViewCore(RenderTreeBuilder builder)
        {
            builder.OpenComponent<AuthorizeRouteViewCore>(0);
            builder.AddAttribute(1, nameof(AuthorizeRouteViewCore.RouteData), RouteData);
            builder.AddAttribute(2, nameof(AuthorizeRouteViewCore.Authorized), _renderAuthorizedDelegate);
            builder.AddAttribute(3, nameof(AuthorizeRouteViewCore.Authorizing), _renderAuthorizingDelegate);
            builder.AddAttribute(4, nameof(AuthorizeRouteViewCore.NotAuthorized), _renderNotAuthorizedDelegate);
            builder.AddAttribute(5, nameof(AuthorizeRouteViewCore.Resource), Resource);
            builder.CloseComponent();
        }

        private void RenderContentInDefaultLayout(RenderTreeBuilder builder, RenderFragment content)
        {
            builder.OpenComponent<LayoutView>(0);
            builder.AddAttribute(1, nameof(LayoutView.Layout), DefaultLayout);
            builder.AddAttribute(2, nameof(LayoutView.ChildContent), new RenderFragment(builder => RenderPageWithCustomContent(builder, content, true)));
            builder.CloseComponent();
        }

        private void RenderAuthorizedInDefaultLayout(RenderTreeBuilder builder)
        {
            var toReset = !_isAuthorized;
            builder.OpenComponent<LayoutView>(0);
            builder.AddAttribute(1, nameof(LayoutView.Layout), DefaultLayout);
            builder.AddAttribute(2, nameof(LayoutView.ChildContent), new RenderFragment(builder => RenderPageWithCustomContent(builder, null, toReset)));
            builder.CloseComponent();
            _isAuthorized = true;
        }

        private void RenderNotAuthorizedInDefaultLayout(RenderTreeBuilder builder, AuthenticationState authenticationState)
        {
            _isAuthorized = false;
            var content = NotAuthorized ?? _defaultNotAuthorizedContent;
            RenderContentInDefaultLayout(builder, content(authenticationState));
        }

        private void RenderAuthorizingInDefaultLayout(RenderTreeBuilder builder)
        {
            _isAuthorized = false;
            var content = Authorizing ?? _defaultAuthorizingContent;
            RenderContentInDefaultLayout(builder, content);
        }

        private sealed class AuthorizeRouteViewCore : AuthorizeViewCore
        {
            [Parameter]
            public RouteData RouteData { get; set; } = default!;

            protected override IAuthorizeData[]? GetAuthorizeData()
                => AttributeAuthorizeDataCache.GetAuthorizeDataForType(RouteData.PageType);
        }
    }

    internal static class AttributeAuthorizeDataCache
    {
        private static readonly ConcurrentDictionary<Type, IAuthorizeData[]?> _cache = new();

        public static IAuthorizeData[]? GetAuthorizeDataForType(Type type)
        {
            if (!_cache.TryGetValue(type, out var result))
            {
                result = ComputeAuthorizeDataForType(type);
                _cache[type] = result; // Safe race - doesn't matter if it overwrites
            }

            return result;
        }

        private static IAuthorizeData[]? ComputeAuthorizeDataForType(Type type)
        {
            // Allow Anonymous skips all authorization
            var allAttributes = type.GetCustomAttributes(inherit: true);
            List<IAuthorizeData>? authorizeDatas = null;
            for (var i = 0; i < allAttributes.Length; i++)
            {
                if (allAttributes[i] is IAllowAnonymous)
                {
                    return null;
                }

                if (allAttributes[i] is IAuthorizeData authorizeData)
                {
                    authorizeDatas ??= new();
                    authorizeDatas.Add(authorizeData);
                }
            }

            return authorizeDatas?.ToArray();
        }
    }
}