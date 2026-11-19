# UrlAdaptor Razor Pages Dialog Editing

This sample demonstrates a Syncfusion EJ2 Grid in an ASP.NET Core Razor Pages application. The grid uses the `UrlAdaptor` to send paging, sorting, filtering, and CRUD requests to Razor Page handlers. Its sample order data is held in memory.

## Requirements

- .NET 9 SDK
- A Syncfusion license appropriate for your use of Syncfusion EJ2 for ASP.NET Core

## Run the application

From the project directory, restore dependencies and start the app:

```powershell
dotnet restore
dotnet run
```

The configured launch profile uses `https://localhost:7298` and `http://localhost:5047`. Open the URL printed by `dotnet run` if the selected profile assigns different ports.

## Using the sample

The home page displays an editable grid of 1,200 generated order records. Use the toolbar to add, edit, or delete rows; the grid also supports paging, sorting, and Excel-style filtering. The initial record count is displayed below the grid.

The grid's UrlAdaptor sends POST requests to these Razor Page handlers:

| Handler | Purpose |
| --- | --- |
| `/Index?handler=DataSource` | Returns data and applies requested search, sort, filter, grouping, and paging operations. |
| `/Index?handler=Insert` | Adds a row to the in-memory collection. |
| `/Index?handler=Update` | Updates a row in the in-memory collection. |
| `/Index?handler=Delete` | Removes a row from the in-memory collection. |

Anti-forgery validation is enabled for requests from the page; the grid sends the page's anti-forgery token in the `XSRF-TOKEN` header.

## Data and configuration

- Sample data is generated in `Pages/Index.cshtml.cs` and stored in a static in-memory list. Changes are not persisted to a database and reset when the application restarts.
- The project targets `net9.0`. It references Syncfusion EJ2 ASP.NET Core and licensing packages; it also references Entity Framework Core SQL Server, but this sample's order operations do not use a database.
- The request culture provider sets the culture to `fr-CH`.
- Configure Syncfusion licensing in `Startup.cs` with a license valid for your deployment. Do not commit a private license key to source control.
