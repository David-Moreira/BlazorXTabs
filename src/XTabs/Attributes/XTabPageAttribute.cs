using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorXTabs
{
    public class XTabPageAttribute : Attribute
    {
        public readonly string Title;
        public XTabPageAttribute(string title) 
            => Title = title;
    }
}
