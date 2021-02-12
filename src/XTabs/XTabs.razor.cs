using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BlazorXTabs.Configuration;

using Microsoft.AspNetCore.Components;

namespace BlazorXTabs
{
    public partial class XTabs
    {
        #region Private Fields

        /// <summary>
        /// All the tabs contained in this XTabs instance.
        /// </summary>
        private IList<XTab> _tabContent = new List<XTab>();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// All the tabs contained in this XTabs instance.
        /// </summary>
        public IEnumerable<XTab> TabContent => _tabContent.AsEnumerable();

        [Inject]
        private NavigationManager _navigationManager { get; set; }

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
        /// Gets or sets the wrapping container css class
        /// </summary>
        [Parameter] public string CssClass { get; set; }

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
        [Parameter]
        public bool IsLoading { get; set; }

        /// <summary>
        /// Gets or sets the XTabs's drag feature.
        /// </summary>
        [Parameter]
        public bool IsDraggable { get; set; }

        /// <summary>
        /// Gets or sets if all tabs can be closed. 
        /// If this is false. One tab will always be open.
        /// </summary>
        [Parameter]
        public bool CloseAllTabs { get; set; }

        /// <summary>
        /// Gets or sets if a button to CloseAllTabs will be displayed
        /// </summary>
        [Parameter]
        public bool ShowCloseAllTabsButton { get; set; }

        /// <summary>
        /// Gets or sets the threshold to display the button to close all tabs.
        /// </summary>
        [Parameter]
        public int CloseAllTabsButtonThreshold { get; set; }

        /// <summary>
        /// Gets or sets if XTabs navigates to homepage if all tabs are closed.
        /// </summary>
        [Parameter]
        public bool NoTabsNavigatesToHomepage { get; set; }

        #endregion Public Properties

        #region Private Properties

        private XTab Active { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Call this method, if for some reason you need to notify XTabs that there have been changes.
        /// </summary>
        public Task NotifyStateHasChangedAsync() => InvokeAsync(() => StateHasChanged());

        /// <summary>
        /// Call this method, if for some reason you need to notify XTabs that there have been changes.
        /// </summary>
        public void NotifyStateHasChanged() => StateHasChanged();

        /// <summary>
        /// Sets tab to active.
        /// </summary>
        /// <param name="tab"></param>
        public async Task SetActiveAsync(XTab tab)
        {
            Active = tab;
            if (OnActiveTabChanged.HasDelegate)
                await OnActiveTabChanged.InvokeAsync(tab);
        }

        /// <summary>
        /// Checks if tab is active.
        /// </summary>
        /// <param name="tab"></param>
        /// <returns></returns>
        public bool IsActive(XTab tab) => tab == Active;

        /// <summary>
        /// Closes tab.
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
                await OnTabRemoved.InvokeAsync();

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
            foreach (var tab in TabContent)
                if (tab.Title.Equals(tabTitle))
                { 
                    await CloseTabAsync(tab);
                    break;
                }
        }

        /// <summary>
        /// Gets tab by title.
        /// </summary>
        /// <param name="tabName"></param>
        public XTab GetTabByTitle(string tabTitle)
        {
            foreach (var tab in TabContent)
                if (tab.Title.Equals(tabTitle))
                    return tab;

            return null;
        }

        #endregion Public Methods

        #region Internal Methods

        public async Task AddPageAsync(XTab tab)
        {
            ///TODO: Using Titles for now. Probably should use an ID.
            if (RenderMode == RenderMode.Full && _tabContent.FirstOrDefault(x => x.Title == tab.Title) is XTab existingTab)
                await SetActiveAsync(existingTab);
            else
            {
                _tabContent.Add(tab);
                if (_tabContent.Count == 1 || NewTabSetActive)
                    await SetActiveAsync(tab);
                if (OnTabAdded.HasDelegate)
                    await OnTabAdded.InvokeAsync(tab);
            }
            await NotifyStateHasChangedAsync();
        }

        #endregion Internal Methods

        #region Private Methods
        private bool CannotCloseLastTab()
            => !CloseAllTabs && _tabContent.Count == 1;

        private bool CloseAllTabsButton() 
            => this.ShowCloseAllTabsButton && ((_tabContent?.Count ?? 0) >= CloseAllTabsButtonThreshold) && !CannotCloseLastTab();

        private async Task CloseAllOpenTabsAsync()
        {
            for (var i = _tabContent.Count - 1; i >= 0; i--)
                await CloseTabAsync(_tabContent[i]);
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