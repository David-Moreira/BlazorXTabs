using System;

namespace BlazorXTabs
{
    [AttributeUsage(AttributeTargets.Class)]
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