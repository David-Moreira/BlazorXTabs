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

        #endregion

        #region Private Properties

        #endregion

        #region Public Methods

        #endregion

        #region Public Properties

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

        #endregion Public Properties

        #region Private Properties

        private XTab Active { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// If for some reason you need to notify XTabs that there have been changes.
        /// </summary>
        public Task NotifyStateHasChangedAsync() => InvokeAsync(() => StateHasChanged());

        /// <summary>
        /// If for some reason you need to notify XTabs that there have been changes.
        /// </summary>
        public void NotifyStateHasChanged() => StateHasChanged();

        /// <summary>
        /// Sets tab to active.
        /// </summary>
        /// <param name="tab"></param>
        public void SetActive(XTab tab)
        {
            Active = tab;
            if (OnActiveTabChanged.HasDelegate)
                OnActiveTabChanged.InvokeAsync(tab);
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
        public void CloseTab(XTab tab)
        {
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
                OnTabRemoved.InvokeAsync();

            SetActive(nextSelected);

            if (_tabContent.Count == 0)
                _navigationManager.NavigateTo("");

            StateHasChanged();
        }

        #endregion Public Methods

        #region Internal Methods

        public void AddPage(XTab tab)
        {
            ///TODO: Using Titles for now. Probably should use an ID.
            if (RenderMode == RenderMode.Full && _tabContent.FirstOrDefault(x => x.Title == tab.Title) is XTab existingTab)
                SetActive(existingTab);
            else
            {
                _tabContent.Add(tab);
                if (_tabContent.Count == 1 || NewTabSetActive)
                    SetActive(tab);
                if (OnTabAdded.HasDelegate)
                    OnTabAdded.InvokeAsync(tab);
            }
            StateHasChanged();
        }

        #endregion Internal Methods

        #region Public Methods



        #endregion Public Methods

        #region Steps Feature

        private bool IsTabHeaderDisabled => RenderMode == RenderMode.Steps;
        private bool IsPreviousDisabled => (_tabContent?.Count > 0 && _tabContent.IndexOf(Active) == 0);

        private bool IsNextDisabled => (_tabContent?.Count > 0 && _tabContent.IndexOf(Active) == _tabContent.IndexOf(_tabContent.Last()));

        private void NextTab()
        {
            var next = _tabContent.IndexOf(Active) + 1;
            SetActive(_tabContent[next]);
            if (OnNextSteps.HasDelegate)
                OnNextSteps.InvokeAsync();
        }

        private void PreviousTab()
        {
            var previous = _tabContent.IndexOf(Active) - 1;
            SetActive(_tabContent[previous]);
            if (OnPreviousSteps.HasDelegate)
                OnPreviousSteps.InvokeAsync();
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