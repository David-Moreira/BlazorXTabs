using System;

namespace BlazorXTabs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class XTabPageAttribute : Attribute
    {
        #region Public Fields

        /// <summary>
        /// Sets the tab's title.
        /// </summary>
        public readonly string Title;

        /// <summary>
        /// Sets the tab's css class.
        /// </summary>
        public readonly string CssClass;

        /// <summary>
        /// Even if inactive, renders in the page hidden.
        /// </summary>
        public readonly bool InactiveRender;

        #endregion

        #region Public Constructors

        public XTabPageAttribute(string title, string cssClass = "", bool inactiveRender = false)
        {
            Title = title;
            CssClass = cssClass;
            InactiveRender = inactiveRender;
        }

        #endregion
    }
}