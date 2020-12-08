# BlazorXTabs
An extended tabs component library providing various tab features for Blazor!

![GitHub tag (latest SemVer)](https://img.shields.io/github/v/tag/David-Moreira/BlazorXTabs)
[![NuGet](https://img.shields.io/nuget/vpre/BlazorXTabs.svg)](https://www.nuget.org/profiles/DavidMoreira)
![Nuget](https://img.shields.io/nuget/dt/BlazorXTabs?flat)
[![MIT](https://img.shields.io/github/license/stsrki/Blazorise.svg)](LICENSE)



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

## Examples:
#### Using as wrapper to render pages as tabs:
      <XTabs RenderMode="BlazorXTabs.Configuration.RenderMode.Full" CloseTabs="true" NewTabSetActive="true">
          @Body
      </XTabs>
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
