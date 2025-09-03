# ASP.NET-Core-MVC-MSSQL

## Summary
HowToDo Website is a content-sharing platform built with ASP.NET Core MVC and Microsoft SQL Server.
Users can log in, create, edit, and delete “How-To-Do” posts. Administrators have extended privileges to manage content and users. The application supports both dark and light themes via two different layouts.

## Features
User Management
- Username/password authentication
- Role-based access (User and Admin)
- Admins can oversee and manage all content
- Admins can manage user roles and suspend accounts

Content Management (CRUD)
- Create, update, and delete posts
- Create, update, and delete comments on posts
- Search and filter posts by keywords
- View all posts created by a specific user

Layouts
- Light mode layout
- Dark mode layout

Data Handling
- Code-First approach with Entity Framework Core
- ViewModels for structured data transfer
- Validation rules for form inputs
- Data passing with ViewBag, ViewData, TempData, and ViewModels

Tech Stack
- ASP.NET Core MVC
- .NET 8
- Microsoft SQL Server
- Entity Framework Core (Code-First)
- Razor View Engine
