# BlazorXTabs
An extended tabs component library providing various features for Blazor!

![Screenshot of sample](sample.png)

## Main Features:
- Close tabs
- Navigate to page and render it as a tab
- Tabs won't duplicate when navigating to the same page
- Use XTabs classes to further customize the look
- Keep tabs state when switching between tabs
- Bind to events: Selected / Changed / Removed
- Setup wizard like tabs

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
        
## Setup: 
You can install the package via the nuget package manager just search for *Blazored.Modal*. You can also install via powershell using the following command.

```powershell
Install-Package BlazorXTabs
```

Or via the dotnet CLI.

```bash
dotnet add package BlazorXTabs
```
