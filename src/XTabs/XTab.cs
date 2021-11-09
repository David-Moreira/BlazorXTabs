using System;
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

        public XTab(XTabs parent, string title, RenderFragment renderFragment, string cssClass, bool inactiveRender)
        {
            _parent = parent;
            Title = title;
            ChildContent = renderFragment;
            CssClass = cssClass;
            InactiveRender = inactiveRender;
        }

        #endregion

        #region Private Properties

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
        /// Sets the tab's title.
        /// </summary>
        [Parameter]
        public string Title { get; set; }

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

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (_parent == null)
                throw new ArgumentNullException(nameof(_parent), "XTabs must exist!");

            await _parent.AddPageAsync(this);
        }

        #endregion
    }
}