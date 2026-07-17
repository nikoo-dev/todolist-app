# To-do List Application

In this project, you must design and develop a web application according to the requirements that are specified in the task description.
  * The application you have to develop must contain two web components - a web and a web API applications.
  * The web application is a [ASP.NET Core MVC application](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview).
  * The web API application is a [controller-based ASP.NET Core Web API application](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/apis).
  * The data store must be a relational database management system such as SQL Server Express.
  * To access application data the application must use [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/).
  * The application must use the [ASP.NET Core Identity API](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity) to manage users, passwords and profile data.


## Getting Started

The solution has three tiers: `TodoListApp.WebApi` (the Web API), `TodoListApp.WebApp` (the MVC web app, which calls the API), and SQLite databases created automatically on first run. There is also a `TodoListApp.WebApi.Tests` project with unit tests for the API's database services.

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Running the application

Both `TodoListApp.WebApi` and `TodoListApp.WebApp` must be running at the same time — the web app has no data access of its own and calls the API for everything.

1. Restore and build the whole solution:
   ```
   dotnet build TodoListApp.sln
   ```
2. Start the Web API (in its own terminal):
   ```
   dotnet run --project TodoListApp.WebApi
   ```
   It listens on `http://localhost:5143` and exposes a Swagger UI at `/swagger`. On first run it creates and migrates `todolist.db` (SQLite) in the project folder.
3. Start the Web App (in a second terminal):
   ```
   dotnet run --project TodoListApp.WebApp
   ```
   It listens on `http://localhost:5217` and creates/migrates `users.db` (SQLite, ASP.NET Core Identity) on first run.
4. Open `http://localhost:5217` in a browser, sign up for an account, and sign in.

### Authentication between the two tiers

The Web App authenticates its own users with ASP.NET Core Identity (cookie auth), then calls the Web API using short-lived JWT bearer tokens it mints per request. Both projects' `appsettings.json` have a `Jwt` section (`SigningKey`, `Issuer`, `Audience`) that **must match exactly** between the two projects — if you change one, change the other the same way, or API calls from the web app will fail with 401 Unauthorized.

The `TodoListWebApi:BaseUrl` setting in `TodoListApp.WebApp/appsettings.json` must point at wherever the Web API is actually listening (defaults to `http://localhost:5143/`, matching the default `dotnet run` port above).

### Running the tests

```
dotnet test TodoListApp.WebApi.Tests
```

## Backlog

The application functional requirements are described in the [Functional Requirements](functional-requirements.md) document.

The [backlog with the user stories](https://en.wikipedia.org/wiki/Product_backlog) you need to implement is given in the table below. The full list of user stories with descriptions is in the [User Stories](user-stories.md) document.

Here are some hints for you:
* Follow the priority order. Start working on user stories in the order listed in the "Priority" column.
* Mark stories as completed in the [README.md](README.md) file. Once you've finished developing a user story, mark it as completed in the "Is completed?" column (use the "+" or any other text). This mark will help the mentor who will review your project understand which functionality is implemented and which is not.
* Focus on quality. Implement as many user stories as possible, but if you see that you do not have enough time to complete lower-priority user stories, polish the user stories you have already completed.

| Epic | User Story | Description                                                                     | Priority | Is completed? |
|------|------------|---------------------------------------------------------------------------------|----------|---------------|
| EP01 | US01       | View the list of my to-do lists.                                                | 1        | +             |
| EP01 | US02       | Add a new to-do list.                                                           | 1        | +             |
| EP01 | US03       | Delete a to-do list.                                                            | 1        | +             |
| EP01 | US04       | Edit a to-do list.                                                              | 1        | +             |
| EP02 | US05       | View the list of tasks in a to-do list.                                         | 1        | +             |
| EP02 | US06       | View the task details page.                                                     | 1        | +             |
| EP02 | US07       | Add a new to-do task.                                                           | 1        | +             |
| EP02 | US08       | Delete a to-do task.                                                            | 1        | +             |
| EP02 | US09       | Edit a to-do task.                                                              | 1        | +             |
| EP02 | US10       | Highlight tasks that are overdue.                                               | 1        | +             |
| EP03 | US11       | View a list of tasks assigned to me.                                            | 2        | +             |
| EP03 | US12       | Filter tasks in my assigned task list.                                          | 2        | +             |
| EP03 | US13       | Sort tasks in my assigned task list.                                            | 2        | +             |
| EP03 | US14       | Change the status of a task from the list of assigned tasks.                    | 2        | +             |
| EP04 | US15       | Search for tasks with specified text in the task title.                         | 3        | +             |
| EP04 | US16       | Highlight tasks that are overdue on the search result page.                     | 3        | +             |
| EP05 | US17       | View a list of tags on the task details page.                                   | 5        | +             |
| EP05 | US18       | View a list of all tags.                                                        | 5        | +             |
| EP05 | US19       | View a list of tasks tagged by a specific tag.                                  | 5        | +             |
| EP05 | US20       | Add a tag to a task.                                                            | 5        | +             |
| EP05 | US21       | Remove a tag that is added to a task.                                           | 5        | +             |
| EP06 | US22       | View the comments on the task details page.                                     | 6        | +             |
| EP06 | US23       | Add a new comment to the task.                                                  | 6        | +             |
| EP06 | US24       | Delete a comment that is added to a task.                                       | 6        | +             |
| EP06 | US25       | Edit a new comment                                                              | 6        | +             |
| EP07 | US26       | Sign up                                                                         | 7        | +             |
| EP07 | US27       | Sign in                                                                         | 7        | +             |
| EP07 | US28       | Sign out                                                                        | 7        | +             |
| EP07 | US29       | Restore password                                                                | 8        | +             |
| EP08 | US30       | Application menu                                                                | 4        | +             |


## Software Architecture

The architecture of the application is described in the [Software Architecture](software-architecture.md) document.


## Solution Requirements

The requirements for the application are described in the [Solution Requirements](solution-requirements.md) document.