using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BlazorXTabs.Configuration;

using Microsoft.AspNetCore.Components;

namespace BlazorXTabs
{
    public partial class XTabs : ComponentBase
    {
        #region Private Fields

        /// <summary>
        /// All the tabs contained in this XTabs instance.
        /// </summary>
        private IList<XTab> _tabContent = new List<XTab>();

        /// <summary>
        /// The page tabs that has been removed. 
        /// This is to make sure to track it and that it does not get re-added by the re-rendering.
        /// </summary>
        internal XTab _tabPageRemoved;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// All the tabs contained in this XTabs instance.
        /// </summary>
        public IEnumerable<XTab> TabContent => _tabContent.AsEnumerable();

        [Inject] private NavigationManager _navigationManager { get; set; }

        /// <summary>
        /// Gets or sets the XTabs NavigationMode. Please note that this is only applicable to the <see cref="XTabsRouteView"/>.
        /// <code>Defaults to: <see cref="NavigationMode.Standard" /></code>
        /// </summary>
        internal NavigationMode NavigationMode { get; set; } = NavigationMode.Standard;

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
        /// Gets or sets the XTabs ChildContent.
        /// XTab should be inserted here.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the template that handles the previous click handler.
        /// </summary>
        [Parameter] public RenderFragment<PreviousStepsContext> PreviousStepsContent { get; set; }

        /// <summary>
        /// Gets or sets the template that handles the next click handler.
        /// </summary>
        [Parameter] public RenderFragment<NextStepsContext> NextStepsContent { get; set; }

        /// <summary>
        /// Gets or sets the wrapping container css class.
        /// </summary>
        [Parameter] public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the wrapping container style.
        /// </summary>
        [Parameter] public string Style { get; set; }

        /// <summary>
        /// Event: When a tab is added to XTabs.
        /// </summary>
        [Parameter] public EventCallback<XTab> OnTabAdded { get; set; }

        /// <summary>
        /// Event: When the active tab is changed on XTabs.
        /// </summary>
        [Parameter] public EventCallback<XTab> OnActiveTabChanged { get; set; }

        /// <summary>
        /// Event: When a tab is removed from XTabs.
        /// </summary>
        [Parameter] public EventCallback<XTab> OnTabRemoved { get; set; }

        /// <summary>
        /// Event: When on XTabs Steps Mode, triggers on previous click.
        /// </summary>
        [Parameter] public EventCallback OnPreviousSteps { get; set; }

        /// <summary>
        /// Event: When on XTabs Steps Mode, triggers on next click.
        /// </summary>
        [Parameter] public EventCallback OnNextSteps { get; set; }

        /// <summary>
        /// Sets the active tab's loading state.
        /// </summary>
        [Parameter] public bool IsLoading { get; set; }

        /// <summary>
        /// Gets or sets the XTabs's drag feature.
        /// </summary>
        [Parameter] public bool IsDraggable { get; set; }

        /// <summary>
        /// Gets or sets if all tabs can be closed. 
        /// If this is false, one tab will always be open.
        /// </summary>
        [Parameter] public bool CloseAllTabs { get; set; }

        /// <summary>
        /// Gets or sets if a button to CloseAllTabs will be displayed.
        /// </summary>
        [Parameter] public bool ShowCloseAllTabsButton { get; set; }

        /// <summary>
        /// Gets or sets the threshold to display the button to close all tabs.
        /// </summary>
        [Parameter] public int CloseAllTabsButtonThreshold { get; set; }

        /// <summary>
        /// Gets or sets if XTabs navigates to homepage if all tabs are closed.
        /// </summary>
        [Parameter] public bool NoTabsNavigatesToHomepage { get; set; }

        /// <summary>
        /// Gets or Sets whether the tabs header is justified taking up the whole available header space.
        /// </summary>
        [Parameter] public bool JustifiedHeader { get; set; }

        /// <summary>
        /// Func : Provides a way to evaluate an XTab and provide a title.
        /// Specially usefull to evaluate tabs added through the route view and translate the titles.
        /// <para>This is only evaluated when the component is adding the tab for the first time.</para>
        /// </summary>
        [Parameter] public Func<XTab, string> TitleFunc { get; set; }

        #endregion Public Properties

        #region Private Properties

        private XTab Active { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Call this method, if for some reason you need to notify XTabs that there have been changes.
        /// </summary>
        public Task NotifyStateHasChangedAsync() => InvokeAsync(StateHasChanged);

        /// <summary>
        /// Call this method, if for some reason you need to notify XTabs that there have been changes.
        /// </summary>
        public void NotifyStateHasChanged() => StateHasChanged();

        /// <summary>
        /// Sets tab to active.
        /// This will trigger navigation if appropriate. <see cref="XTabsRouteView.NavigationMode"/>
        /// </summary>
        /// <param name="tab"></param>
        public async Task SetActiveAsync(XTab tab)
        {
            Active = tab;
            if (OnActiveTabChanged.HasDelegate)
                await OnActiveTabChanged.InvokeAsync(tab);

            if (NavigationMode == NavigationMode.Navigable && !string.IsNullOrWhiteSpace(tab.RouteUrl))
            {
                if (tab.RouteUrl.StartsWith('/'))
                    _navigationManager.NavigateTo(tab.RouteUrl[1..]);
                else
                    _navigationManager.NavigateTo(tab.RouteUrl);
            }

            await NotifyStateHasChangedAsync();
        }

        /// <summary>
        /// Sets tab to active.
        /// Internal use... RouteView
        /// </summary>
        /// <param name="tab"></param>
        internal void SetActive(XTab tab)
        {
            Active = tab;
            //TODO: This is not working quite right once listened to on RouteView.
            //if (OnActiveTabChanged.HasDelegate)
            //    await OnActiveTabChanged.InvokeAsync(tab);
        }

        /// <summary>
        /// Checks if tab is active.
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public bool IsActive(XTab tab) => tab == Active;

        /// <summary>
        /// Closes tab while still complying to the existing configuration => CloseAllTabs.
        /// </summary>
        /// <param name="tab"></param>
        public async Task CloseTabAsync(XTab tab)
        {
            if (CannotCloseLastTab())
                return;

            var nextSelected = Active;
            if (Active == tab && _tabContent.Count > 1)
                for (int i = 0; i < _tabContent.Count; i++)
                {
                    if (i > 0 && _tabContent[i] == Active)
                        nextSelected = _tabContent[i - 1];
                    if (i > 0 && _tabContent[i - 1] == Active)
                        nextSelected = _tabContent[i];
                }

            _tabContent.Remove(tab);
            if (OnTabRemoved.HasDelegate)
            {
                if (tab.PageTab)
                    _tabPageRemoved = tab;
                await OnTabRemoved.InvokeAsync(tab);
            }

            await SetActiveAsync(nextSelected);

            if (_tabContent.Count == 0 && NoTabsNavigatesToHomepage)
                _navigationManager.NavigateTo("");

            await NotifyStateHasChangedAsync();
        }

        /// <summary>
        /// Closes tab by title.
        /// </summary>
        /// <param name="tabName"></param>
        public async Task CloseTabByTitleAsync(string tabTitle)
        {
            var tab = GetTabByTitle(tabTitle);
            if (tab is not null)
                await CloseTabAsync(tab);
        }

        /// <summary>
        /// Closes tab by id.
        /// </summary>
        /// <param name="tabName"></param>
        public async Task CloseTabByIdAsync(string id)
        {
            var tab = GetTabById(id);
            if (tab is not null)
                await CloseTabAsync(tab);
        }

        /// <summary>
        /// Gets tab by title.
        /// </summary>
        /// <param name="tabName"></param>
        public XTab GetTabByTitle(string tabTitle)
        {
            foreach (var tab in TabContent)
                if (tab.Title?.Equals(tabTitle) ?? false)
                    return tab;

            return null;
        }

        /// <summary>
        /// Gets tab by id.
        /// </summary>
        /// <param name="id"></param>
        public XTab GetTabById(string id)
        {
            foreach (var tab in TabContent)
                if (tab.Id?.Equals(id) ?? false)
                    return tab;

            return null;
        }

        /// <summary>
        /// Adds a new tab to the XTabs content
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public async Task AddPageAsync(XTab tab)
        {
            if (GetTabById(tab.Id) is XTab existingTab)
                await SetActiveAsync(existingTab);
            else
                await AddTabAsync(tab);

            await NotifyStateHasChangedAsync();
        }

        /// <summary>
        /// Adds a new tab to the XTabs content.
        /// For internal usage on XTabsRouteView
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        internal void AddPage(XTab tab)
        {
            if (GetTabById(tab.Id) is XTab existingTab)
                SetActive(existingTab);
            else
                InvokeAsync(() => AddTabAsync(tab));

            NotifyStateHasChanged();
        }



        /// <summary>
        /// Adds or replaces a tab to the XTabs content
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public async Task AddOrReplacePageAsync(XTab tab)
        {
            if (GetTabById(tab.Id) is XTab existingTab)
            {
                var idx = _tabContent.IndexOf(existingTab);
                _tabContent.Remove(existingTab);
                _tabContent.Insert(idx, tab);
                await SetActiveAsync(tab);
            }
            else
            {
                await AddTabAsync(tab);
            }

            await NotifyStateHasChangedAsync();
        }

        /// <summary>
        /// Adds or replaces a tab to the XTabs content
        /// For internal usage on XTabsRouteView
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        internal void AddOrReplacePage(XTab tab)
        {
            if (GetTabById(tab.Id) is XTab existingTab)
            {
                var idx = _tabContent.IndexOf(existingTab);
                _tabContent.Remove(existingTab);
                _tabContent.Insert(idx, tab);
                SetActive(tab);
            }
            else
            {
                AddTabAsync(tab).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            NotifyStateHasChanged();
        }

        /// <summary>
        /// Closes all open tabs while still complying to the existing configuration => CloseAllTabs.
        /// </summary>
        /// <returns></returns>
        public async Task CloseAllOpenTabsAsync()
        {
            for (var i = _tabContent.Count - 1; i >= 0; i--)
                await CloseTabAsync(_tabContent[i]);
        }

        #endregion Public Methods

        #region Private Methods

        private async Task AddTabAsync(XTab tab)
        {
            _tabContent.Add(tab);
            if (_tabContent.Count == 1 || NewTabSetActive)
                await SetActiveAsync(tab);
            if (OnTabAdded.HasDelegate)
                await OnTabAdded.InvokeAsync(tab);
        }

        private bool CannotCloseLastTab()
            => !CloseAllTabs && _tabContent.Count == 1;

        private bool CloseAllTabsButton()
            => this.ShowCloseAllTabsButton && CanCloseOpenTabs();

        private bool CanCloseOpenTabs() => ((_tabContent?.Count ?? 0) >= CloseAllTabsButtonThreshold) && !CannotCloseLastTab();

        private string BuildHeaderClasses()
        {
            StringBuilder sb = new StringBuilder();
            if (JustifiedHeader)
                sb.Append(" nav-justified");
            if (_isDragging)
                sb.Append(" drag");
            return sb.ToString();
        }

        #endregion Private Methods

        #region Steps Feature

        private bool IsTabHeaderDisabled => RenderMode == RenderMode.Steps;
        private bool IsPreviousDisabled => (_tabContent?.Count > 0 && _tabContent.IndexOf(Active) == 0);
        private bool IsNextDisabled => (_tabContent?.Count > 0 && _tabContent.IndexOf(Active) == _tabContent.IndexOf(_tabContent.Last()));

        private async Task NextTabAsync()
        {
            var next = _tabContent.IndexOf(Active) + 1;
            await SetActiveAsync(_tabContent[next]);
            if (OnNextSteps.HasDelegate)
                await OnNextSteps.InvokeAsync();
        }

        private async Task PreviousTabAsync()
        {
            var previous = _tabContent.IndexOf(Active) - 1;
            await SetActiveAsync(_tabContent[previous]);
            if (OnPreviousSteps.HasDelegate)
                await OnPreviousSteps.InvokeAsync();
        }

        #endregion Steps Feature

        #region Drag Feature

        private bool _isDragging;
        private XTab _isDraggedTab;

        private void DragStart(XTab tab)
        {
            _isDragging = true;
            _isDraggedTab = tab;
            return;
        }

        private void DragDrop(XTab tab)
        {
            var indexToReplace = _tabContent.IndexOf(_isDraggedTab);
            _tabContent[_tabContent.IndexOf(tab)] = _isDraggedTab;
            _tabContent[indexToReplace] = tab;
            return;
        }

#pragma warning disable IDE0060 // Remove unused parameter

        private void DragEnd(XTab tab)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            _isDragging = false;
            _isDraggedTab = null;
            return;
        }

        #endregion Drag Feature
    }
}