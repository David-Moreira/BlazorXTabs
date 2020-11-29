using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;

namespace BlazorXTabs.Configuration
{
    public abstract class StepsContext
    {
        public StepsContext(EventCallback clicked, bool isDisabled)
        {
            Clicked = clicked;
            IsDisabled = isDisabled;
        }

        public bool IsDisabled { get; set; }
        public EventCallback Clicked { get; set; }
    }

    public class PreviousStepsContext : StepsContext
    {
        public PreviousStepsContext(EventCallback clicked, bool isDisabled) : base(clicked, isDisabled)
        {
        }
    }

    public class NextStepsContext : StepsContext
    {
        public NextStepsContext(EventCallback clicked, bool isDisabled) : base(clicked, isDisabled)
        {
        }
    }
}