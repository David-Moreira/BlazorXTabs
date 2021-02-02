using System;

namespace BlazorXTabs
{
    public class XTabPageAttribute : Attribute
    {
        #region Public Fields

        public readonly string Title;

        #endregion

        #region Public Constructors

        public XTabPageAttribute(string title)
            => Title = title;

        #endregion
    }
}