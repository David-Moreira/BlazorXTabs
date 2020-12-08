namespace BlazorXTabs.Configuration
{
    /// <summary>
    /// Partial: Will only render the active tab
    /// <para>Full: Will always fully render tabs, but hide them</para>
    /// Steps: Will render a Wizard like component
    /// </summary>
    public enum RenderMode
    {
        Partial,
        Full,
        Steps
    }
}