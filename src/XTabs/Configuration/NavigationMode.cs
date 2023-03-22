namespace BlazorXTabs.Configuration
{
    /// <summary>
    /// Configures the navigation mode for XTabs.
    /// </summary>
    public enum NavigationMode
    {
        /// <summary>
        /// Clicking a tab will only set it as active.
        /// </summary>
        Standard,
        /// <summary>
        /// Clicking a tab will set it as active and trigger navigation.
        /// </summary>
        Navigable
    }
}