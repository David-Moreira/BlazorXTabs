using Microsoft.AspNetCore.Components;

namespace BlazorXTabs.Configuration
{
    public abstract class StepsContext
    {
        #region Public Constructors

        public StepsContext(EventCallback clicked, bool isDisabled)
        {
            Clicked = clicked;
            IsDisabled = isDisabled;
        }

        #endregion

        #region Public Properties

        public bool IsDisabled { get; set; }
        public EventCallback Clicked { get; set; }

        #endregion
    }

    public class PreviousStepsContext : StepsContext
    {
        #region Public Constructors

        public PreviousStepsContext(EventCallback clicked, bool isDisabled) : base(clicked, isDisabled)
        {
        }

        #endregion
    }

    public class NextStepsContext : StepsContext
    {
        #region Public Constructors

        public NextStepsContext(EventCallback clicked, bool isDisabled) : base(clicked, isDisabled)
        {
        }

        #endregion
    }
}