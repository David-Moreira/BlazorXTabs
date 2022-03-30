# BlazorXTabs
An extended tabs component library providing various tab features for Blazor!

![GitHub tag (latest SemVer)](https://img.shields.io/github/v/tag/David-Moreira/BlazorXTabs)
[![NuGet](https://img.shields.io/nuget/vpre/BlazorXTabs.svg)](https://www.nuget.org/profiles/DavidMoreira)
![Nuget](https://img.shields.io/nuget/dt/BlazorXTabs?flat)
[![MIT](https://img.shields.io/github/license/David-Moreira/BlazorXTabs.svg)](LICENSE)



![Screenshot of sample](sample.png)
Checkout all the examples at: [BlazorXTabs Demo](https://david-moreira.github.io/BlazorXTabs/)

## Main Features:
- Close tabs
- Navigate to page and render it as a tab
- Tabs won't duplicate when navigating to the same page
- Use XTabs classes to further customize the look
- Keep tabs state when switching between tabs
- Bind to events: Selected / Changed / Removed
- **v1.2.0**
  - Setup wizard like tabs with events: Previous / Next and customizable previous/next buttons
  - Possibility to set the active tab to Loading
- **v1.3.0**
  - Able to drag & drop tabs
- **v1.4.0**
  - Able to replace the standard RouteView component with a XTabsRouteView component that automatically renders the pages as tabs
- **v1.5.0**
  - When CloseTabs is enabled. Able to close all tabs and configure a threshold to display the close all tabs button.
  - When CloseTabs is enabled. Able to use mouse middle button click to close the tab.
  - When CloseTabs is enabled. Able to limit closeable tabs to 1.
- **v1.6.0**
  - Fixed RenderMode.Partial duplicating tabs
  - Introduced XTabsAuthorizeRouteView to allow tabs integration with Authentication
- **v1.7.0**
  - NET6 Support

## Examples:

#### Standard usage:
    <XTabs>
        <XTab Title="Tab1">
            Content 1
        </XTab>
        <XTab Title="Tab2">
            Content 2
        </XTab>
    </XTabs>

#### Using the XTabsRouteView or XTabsAuthorizeRouteView to render pages as tabs:
    <Router AppAssembly="@typeof(Program).Assembly">
        <Found Context="routeData">
            <XTabsRouteView CloseTabs="true" NewTabSetActive="true" RenderMode="BlazorXTabs.Configuration.RenderMode.Full" RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        </Found>
        <NotFound>
            <LayoutView Layout="@typeof(MainLayout)">
                <p>Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
##### Use the XTabPageAttribute to set the page's tab title & other relevant parameters
     @attribute [XTabPageAttribute("Home")]

#### Using as a wizard:
        <XTab Title="Steps example">
            <XTabs RenderMode="BlazorXTabs.Configuration.RenderMode.Steps">
                <XTab Title="Step1">
                       This is step 1!
                </XTab>
                <XTab Title="Step2">
                    This is step 2!
                </XTab>
                <XTab Title="Step3">
                    This is the last step. Step 3!
                </XTab>
            </XTabs>
        </XTab>
#### Drag&Drop:
        <XTab Title="Drag Example">
            <XTabs IsDraggable="true" RenderMode="BlazorXTabs.Configuration.RenderMode.Partial" >
                    <XTab Title="1. I can be dragged!!">
                        <p>I can be dragged!!</p>
                    </XTab>
                    <XTab Title="2. Drag me!">
                        <p>Drag me!</p>
                    </XTab>
                    <XTab Title="3. Please drag me!! I hate being last place!">
                        <p>Please drag me!! I hate being last place!</p>
                    </XTab>
            </XTabs>
        </XTab>
## Setup: 
- You can install the package via the nuget package manager just search for *BlazorXTabs*. You can also install via powershell using the following command.

```powershell
Install-Package BlazorXTabs
```

- Or via the dotnet CLI.

```bash
dotnet add package BlazorXTabs
```

- If you'd like to bring in XTabs default styling, don't forget to add:
```    
<link href="{YOUR-PROJECT-NAME}.styles.css" rel="stylesheet" />
```
T
