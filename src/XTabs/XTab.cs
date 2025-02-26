﻿using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace BlazorXTabs
{
    public class XTab : ComponentBase
    {
        #region Public Constructors

        public XTab()
        {

        }

        #endregion

        #region Public Methods

        public static XTab Create(XTabs parent, string title, RenderFragment renderFragment, string cssClass, bool inactiveRender, string routeUrl)
        {
            var xTab = new XTab()
            {
                PageTab = true,
                _parent = parent,
                Id = title,
                Title = title,
                ChildContent = renderFragment,
                CssClass = cssClass,
                InactiveRender = inactiveRender,
                RouteUrl = routeUrl
            };
            xTab.LoadTitle();
            return xTab;
        }

        #endregion
        #region Private Properties


        internal bool PageTab;
        internal string RouteUrl;

        [CascadingParameter]
        private XTabs _parent { get; set; }

        #endregion

        #region Public Properties

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Sets the tab's loading content.
        /// </summary>
        [Parameter]
        public RenderFragment LoadingContent { get; set; }

        /// <summary>
        /// Sets the tab's Id.
        /// If this has not been set. It'll take in the Title's value.
        /// </summary>
        /// <remarks>
        /// Id should always be unique.
        /// </remarks>
        [Parameter]
        public string Id { get; set; }

        /// <summary>
        /// Sets the tab's title.
        /// </summary>
        [Parameter]
        public string Title { get; set; }

        /// <summary>
        /// Event: Tracks the tab's title changed event.
        /// </summary>
        [Parameter]
        public EventCallback<string> TitleChanged { get; set; }

        /// <summary>
        /// Sets the tab's css class.
        /// </summary>
        [Parameter]
        public string CssClass { get; set; }

        /// <summary>
        /// Even if inactive, renders in the page hidden.
        /// </summary>
        [Parameter]
        public bool InactiveRender { get; set; }

        #endregion

        #region Protected Methods

        public async Task SetTitle(string title)
        {
            Title = title;
            await TitleChanged.InvokeAsync(title);
            await _parent.NotifyStateHasChangedAsync();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (string.IsNullOrEmpty(Id))
                Id = Title;
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (_parent == null)
                throw new ArgumentNullException(nameof(_parent), "XTabs must exist!");

            await LoadTitle();

            await _parent.AddPageAsync(this);
        }

        internal Task LoadTitle()
        {
            if (_parent.TitleFunc is not null)
                return SetTitle(_parent.TitleFunc(this));
            return Task.CompletedTask;
        }

        #endregion
    }
}