# 🏙️ CivicResolve: Municipal Smart Grievance & Complaint Redressal Portal

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0_MVC-purple.svg)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/Entity_Framework_Core-8.0-blue.svg)](https://docs.microsoft.com/ef/core/)
[![MySQL](https://img.shields.io/badge/Database-MySQL_8.0-orange.svg)](https://www.mysql.com/)
[![Bootstrap](https://img.shields.io/badge/UI-Bootstrap_5.3-teal.svg)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/Academic-Mini_Project-success.svg)](#)

> **Web Technologies with .NET — Mini Project & Technical Poster Presentation**  
> **Total Marks:** 24  
> **Student Name:** Dakhara Parth Akshaybhai  
> **Enrollment No:** 202406101410001  
> **Branch:** B.Tech in Computer Science (CS)  

---

## 📌 Project Overview
**CivicResolve** is a full-stack municipal grievance redressal web application built using **ASP.NET Core 8 MVC**, **Entity Framework Core**, and **MySQL**. It bridges citizens and civic departments (Sanitation, Roads, Water, Electricity, Public Health) by providing:
- Instant online grievance lodging with photo attachments.
- Automated unique tracking tokens (e.g., `CR-2026-XXXXX`).
- Enforced Service Level Agreements (SLAs) with automated overdue escalations.
- Interactive 4-stage visual progress stepper for public tracking.
- Officer command console with multi-parameter filtering, status audit trails, and CSV reports.
- Post-resolution citizen feedback and 5-star satisfaction ratings.

---

## 🎯 Syllabus & Evaluation Mapping (24 Marks)

| Criterion | Implementation in CivicResolve |
| :--- | :--- |
| **MVC Architecture** | Decoupled 3-tier structure (`Models`, `ViewModels`, `Controllers`, `Views`). |
| **Database Connectivity** | Entity Framework Core 8 using `Pomelo.EntityFrameworkCore.MySql` on port 3306. |
| **CRUD Operations** | Full Create, Read, Update, Delete for Complaints, Departments, Categories, and Audit logs. |
| **Form Handling** | Razor Tag Helpers (`asp-for`, `asp-validation-for`, `asp-items`), AJAX cascading dropdowns, and `IFormFile` photo upload. |
| **Model Validation** | Server-side `ModelState.IsValid` + client-side jQuery unobtrusive validation (`[Required]`, `[EmailAddress]`, regex). |
| **State Management** | `HttpContext.Session` (citizen tracking history & officer role state) + `TempData` (flash notification toasts). |
| **Responsive UI** | Mobile-friendly Bootstrap 5.3 + custom civic styling and visual progress stepper. |
| **Technical Poster** | Standalone A2 size soft-copy poster (`A2_Poster_CivicResolve.html`) covering all 8 required sections. |

---

## 🛠️ Technology Stack
- **Framework:** ASP.NET Core 8.0 (MVC)
- **Programming Language:** C# 12
- **ORM:** Entity Framework Core 8.0
- **Database:** MySQL 8.0 / MariaDB (with smart offline fallback)
- **Front-End:** Razor Views, HTML5, CSS3, Bootstrap 5.3, Bootstrap Icons
- **Scripting:** jQuery 3.6, jQuery Validation Unobtrusive

---

## 🚀 Quick Setup & Execution

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Optional) MySQL Server / XAMPP / MariaDB

### Steps to Run:
1. **Clone the repository:**
   ```bash
   git clone https://github.com/YOUR_USERNAME/CivicResolve.git
   cd CivicResolve
   ```

2. **(Optional) Setup MySQL Database:**
   - Open phpMyAdmin or MySQL Workbench and run:
     [`civicresolve_mysql_schema_and_seed.sql`](./civicresolve_mysql_schema_and_seed.sql)
   - Connection string is pre-configured in `appsettings.json`:
     ```json
     "Server=localhost;Port=3306;Database=civicresolve_db;User=root;Password=;"
     ```

3. **Run the Project:**
   ```bash
   dotnet run
   ```
   Open your browser at: **`http://localhost:5241`** or **`http://localhost:5000`**

> **Note:** The application automatically seeds default departments, categories, and test grievances on initial launch!

---

## 🖼️ A2 Technical Poster (Soft Copy)
The technical poster is ready to view and export in:
- File: [`A2_Poster_CivicResolve.html`](./A2_Poster_CivicResolve.html)
- Open it in any browser and press **`Ctrl + P`** -> **Save as PDF** (Paper Size: A2) for printing or soft-copy submission.

---

## 👨‍💻 Author
**Dakhara Parth Akshaybhai**  
- Enrollment No: `202406101410001`  
- Program: B.Tech Computer Science (CS)  
