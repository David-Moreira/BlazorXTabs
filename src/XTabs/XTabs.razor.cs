using System.Collections.Generic;
using System.Linq;

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
        private List<XTab> TabContent = new List<XTab>();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Sets the XTabs RenderMode.
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

        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Sets the template that handles the previous click handler.
        /// </summary>
        [Parameter] public RenderFragment<PreviousStepsContext> PreviousStepsContent { get; set; }

        /// <summary>
        /// Sets the template that handles the next click handler.
        /// </summary>
        [Parameter] public RenderFragment<NextStepsContext> NextStepsContent { get; set; }

        /// <summary>
        /// Sets the wrapping container css class
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

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        #endregion Public Properties

        #region Private Properties

        private XTab Active { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Notifies XTabs that there have been changes.
        /// <para>If there are children that depend on each other's state, you should notify this parent component that the state has changed.</para>
        /// </summary>
        public void NotifyStateHasChanged()
        {
            StateHasChanged();
        }

        #endregion Public Methods

        #region Internal Methods

        internal void AddPage(XTab tab)
        {
            ///TODO: Using Titles for now. Probably should use an ID.
            if (RenderMode == RenderMode.Full && TabContent.FirstOrDefault(x => x.Title == tab.Title) is XTab existingTab)
                SetActive(existingTab);
            else
            {
                TabContent.Add(tab);
                if (TabContent.Count == 1 || NewTabSetActive)
                    SetActive(tab);
                if (OnTabAdded.HasDelegate)
                    OnTabAdded.InvokeAsync(tab);
            }
            StateHasChanged();
        }

        #endregion Internal Methods

        #region Private Methods

        private void SetActive(XTab tab)
        {
            Active = tab;
            if (OnActiveTabChanged.HasDelegate)
                OnActiveTabChanged.InvokeAsync(tab);
        }

        private bool IsActive(XTab tab)
        {
            return tab == Active;
        }

        private void CloseTab(XTab tab)
        {
            var nextSelected = Active;
            if (Active == tab && TabContent.Count > 1)
                for (int i = 0; i < TabContent.Count; i++)
                {
                    if (i > 0 && TabContent[i] == Active)
                        nextSelected = TabContent[i - 1];
                    if (i > 0 && TabContent[i - 1] == Active)
                        nextSelected = TabContent[i];
                }

            TabContent.Remove(tab);
            if (OnTabRemoved.HasDelegate)
                OnTabRemoved.InvokeAsync();

            SetActive(nextSelected);

            if (TabContent.Count == 0)
                NavigationManager.NavigateTo("");

            StateHasChanged();
        }

        #endregion Private Methods

        #region Steps

        private bool IsTabHeaderDisabled => RenderMode == RenderMode.Steps;
        private bool IsPreviousDisabled => (TabContent?.Count > 0 && TabContent.IndexOf(Active) == 0);

        private bool IsNextDisabled => (TabContent?.Count > 0 && TabContent.IndexOf(Active) == TabContent.IndexOf(TabContent.Last()));

        private void NextTab()
        {
            var next = TabContent.IndexOf(Active) + 1;
            SetActive(TabContent[next]);
        }

        private void PreviousTab()
        {
            var previous = TabContent.IndexOf(Active) - 1;
            SetActive(TabContent[previous]);
        }

        #endregion Steps
    }
}