# AppBlueprint.UiKit

## Installation

1. Ensure the web project that the nuget package is being added to is of the same .net version or above the one used in
   Appblueprint.UiKit library as specified in the .csproj file

       <TargetFramework>net9.0</TargetFramework>

2. Remove Components/Mainlayout.css such that it does interefere with the Ui Kit styling

3. Remove wwwroot/bootstrap folder

4. Remove wwwroot/App.css such that it does interefere with the Ui Kit styling

5. Update the Mudblazor nuget package in the web project to the same as the one in the UI kit - currently

       <PackageReference Include="mudblazor" Version="7.15.0" />

6. Add the UI Kit nuget package

       <PackageReference Include="SaaS-Factory.AppBlueprint.UiKit" Version="1.0.1"/>

7. Run dotnet restore in the web project

       dotnet restore

8. Add the using statement to include the UiKit inside Mainlayout.razor

       @using AppBlueprint.UiKit.Components

9. Ensure Mainlayout.razor looks similar to this

       @inherits LayoutComponentBase
       <MudThemeProvider />
       <MudPopoverProvider />
       <MudDialogProvider />
       <MudSnackbarProvider />
       
       @using AppBlueprint.UiKit.Components
       
       <div class="page">
           <div class="sidebar">
               <NavigationMenu></NavigationMenu>
           </div>
       
           <main>
               <article class="content px-4">
                   @Body
               </article>
           </main>
       </div>
       
       <div id="blazor-error-ui">
           An unhandled error has occurred.
           <a href="" class="reload">Reload</a>
           <a class="dismiss">🗙</a>
       </div>

## UI pages and components

The pages and components will serve as blueprints to implement most SaaS apps with a dashboard, customers, and products
and custom functionality using standalone components. The components that are showcased in the pages can be used in
other pages or components. The blueprint template inspiration is taken from Cruip.com Mosaic VueJs + Tailwind CSS
template and implemented using Blazor Server with MudBlazor customizable UI components that use Material Design
principles and Tailwind CSS. The template will be uploaded to Mudblazor/Templates as a reusable template for other
developers to use freely as a contribution to the Mudblazor community.

### Pages ##

#### - Dashboard.razor #

- Charts
- Cards

#### - Customers.razor #

- Datagrid (interactive table with search, sort, pagination)

#### - Invoices.razor

- Datagrid (interactive table with search, sort, pagination)
- Mudchip (selection tags)

### - Appbar ##

### - Navigation Menu - Navmenu.razor

- Sidebar menu
- Logo
- Group navigation links
- Standalone navigation links

### Standalone components 
