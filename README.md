# ?? MediAI Backend - Healthcare Management System

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=flat&logo=mysql&logoColor=white)](https://www.mysql.com/)
[![License](https://img.shields.io/badge/License-Educational-green.svg)](LICENSE)
[![Status](https://img.shields.io/badge/Status-Production%20Ready-success.svg)]()

A comprehensive healthcare management REST API designed for educational institutions, providing digital healthcare solutions for students, faculty, doctors, and administrators.

## ?? Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)

---

## ?? Overview

**MediAI Backend** is an enterprise-grade healthcare management system built with .NET 8 and MySQL, specifically designed for BUITEMS (Balochistan University of Information Technology, Engineering and Management Sciences). It provides a complete digital healthcare ecosystem with robust authentication, appointment scheduling, medicine reminders, and comprehensive medical record management.

### Key Highlights

- ? **27 Database Tables** - Comprehensive data model
- ? **40+ API Endpoints** - Full REST API coverage
- ? **JWT Authentication** - Secure token-based auth
- ? **OTP Email Verification** - Secure user registration
- ? **Role-Based Authorization** - Student, Faculty, Doctor, Admin
- ? **BCrypt Password Hashing** - Industry-standard security
- ? **Entity Framework Core** - Type-safe database access
- ? **Swagger Documentation** - Interactive API docs

---

## ?? Features

### ?? Authentication & Authorization
- User registration with email verification (OTP-based)
- JWT token-based authentication
- Password reset with email verification
- Role-based access control (4 roles)
- Secure password hashing with BCrypt

### ????? Doctor Management
- Doctor profile creation and management
- Specialization tracking
- Weekly schedule management
- Consultation fee management
- Average rating and reviews system
- Doctor availability status

### ?? Appointment System
- Book appointments with doctors
- View upcoming and past appointments
- Cancel appointments with reasons
- Appointment status tracking (Pending, Confirmed, InProgress, Completed, Cancelled, NoShow)
- Conflict detection for time slots
- Doctor-side appointment management

### ?? Medicine Reminders
- Create medicine reminders with custom schedules
- Frequency options (Once, Twice, Thrice, Four times, Custom)
- JSON-based time scheduling
- Intake logging system
- Active/inactive reminder toggle
- Reminder history tracking

### ?? Electronic Prescriptions
- Digital prescription management
- Multiple medicines per prescription
- Dosage and frequency tracking
- Treatment duration management
- Follow-up date scheduling

### ?? User Profile Management
- View and update user profiles
- Profile photo upload
- Change password functionality
- Emergency contact management
- Medical history tracking

### ?? Email Notifications
- OTP verification emails
- Welcome emails after registration
- Password reset emails
- Appointment confirmation emails
- Console mode for development

### ?? Additional Features
- Health tips and articles
- Doctor reviews and ratings
- System audit logs
- Notification system
- AI symptom checker integration
- Report generation system

---

## ??? Technology Stack

### Backend
- **Framework:** .NET 8.0
- **Language:** C# 12.0
- **Database:** MySQL 8.0
- **ORM:** Entity Framework Core 8.0
- **Authentication:** JWT Bearer Tokens
- **Password Security:** BCrypt.Net
- **Email:** System.Net.Mail (SMTP)

### Database
- **Provider:** Pomelo.EntityFrameworkCore.MySql
- **Connection Pooling:** Enabled
- **Charset:** UTF8MB4 Unicode
- **Tables:** 27
- **Views:** 3
- **Stored Procedures:** 2
- **Triggers:** 4

### Tools & Libraries
- **API Documentation:** Swashbuckle (Swagger)
- **Dependency Injection:** Built-in .NET DI Container
- **Logging:** Microsoft.Extensions.Logging
- **Configuration:** appsettings.json

---

## ?? Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL 8.0](https://dev.mysql.com/downloads/mysql/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [MySQL Workbench](https://www.mysql.com/products/workbench/) (optional)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Abdur-Rehman-007/Medi-AI_backend.git
   cd Backend-APIs
   ```

2. **Setup the database**
   ```sql
   -- Create database
   CREATE DATABASE mediaidb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   
   -- Run the schema script
   SOURCE path/to/mediaidb_schema.sql;
   ```

3. **Configure the application**
   
   Update `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "server=localhost;port=3306;database=mediaidb;user=root;password=YOUR_PASSWORD;"
     },
     "Jwt": {
       "Key": "YOUR-SECRET-KEY-MIN-32-CHARS",
       "Issuer": "MediAI-Backend",
       "Audience": "MediAI-Users",
       "ExpiryInHours": 24
     },
     "EmailSettings": {
       "SmtpHost": "smtp.gmail.com",
       "SmtpPort": 587,
       "SenderEmail": "your-email@gmail.com",
       "Username": "your-email@gmail.com",
       "Password": "your-gmail-app-password",
       "EnableSsl": true,
       "UseConsoleForDevelopment": true
     }
   }
   ```

4. **Restore dependencies**
   ```bash
   dotnet restore
   ```

5. **Run the application**
   ```bash
   dotnet run --project Backend-APIs
   ```

6. **Access Swagger UI**
   
   Navigate to: `https://localhost:7228/swagger`

---

## ?? API Documentation

### Base URL
```
Development: https://localhost:7228/api
Production: https://your-domain.com/api
```

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/Auth/register` | Register new user | No |
| POST | `/Auth/verify-otp` | Verify email with OTP | No |
| POST | `/Auth/login` | User login | No |
| POST | `/Auth/resend-otp` | Resend OTP | No |
| POST | `/Auth/forgot-password` | Request password reset | No |
| POST | `/Auth/reset-password` | Reset password | No |
| GET | `/Auth/current-user` | Get current user | Yes |
| GET | `/Auth/health` | API health check | No |

### Doctor Management

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/Doctors` | Get all doctors | No |
| GET | `/Doctors/{id}` | Get doctor by ID | No |
| GET | `/Doctors/{id}/schedule` | Get doctor schedule | No |
| GET | `/Doctors/{id}/available-slots` | Get available slots | No |
| POST | `/Doctors` | Create doctor profile | Yes (Admin) |
| PUT | `/Doctors/{id}` | Update doctor info | Yes |

### Appointments

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/Appointments` | Book appointment | Yes |
| GET | `/Appointments/student/{id}/upcoming` | Get upcoming appointments | Yes |
| GET | `/Appointments/student/{id}/history` | Get past appointments | Yes |
| GET | `/Appointments/Faculty/appointments` | Get doctor's appointments | Yes (Doctor) |
| PUT | `/Appointments/{id}/cancel` | Cancel appointment | Yes |
| POST | `/Appointments/{id}/prescription` | Add prescription | Yes (Doctor) |

### Medicine Reminders

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/MedicineReminders` | Get all reminders | Yes |
| GET | `/MedicineReminders/active` | Get active reminders | Yes |
| POST | `/MedicineReminders` | Create reminder | Yes |
| PUT | `/MedicineReminders/{id}` | Update reminder | Yes |
| PATCH | `/MedicineReminders/{id}/toggle` | Toggle active status | Yes |
| DELETE | `/MedicineReminders/{id}` | Delete reminder | Yes |
| POST | `/MedicineReminders/{id}/log` | Log intake | Yes |

### User Profile

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/Users/profile` | Get user profile | Yes |
| PUT | `/Users/profile` | Update profile | Yes |
| POST | `/Users/change-password` | Change password | Yes |
| POST | `/Users/upload-photo` | Upload profile photo | Yes |

### Sample Request/Response

**Register User**
```bash
POST /api/Auth/register
Content-Type: application/json

{
  "email": "student@buitms.edu.pk",
  "password": "SecurePass123",
  "fullName": "Ahmed Ali",
  "role": "Student",
  "department": "Computer Science",
  "registrationNumber": "59858",
  "phoneNumber": "03001234567",
  "dateOfBirth": "2002-05-15",
  "gender": "Male"
}
```

**Response**
```json
{
  "success": true,
  "message": "Registration successful! OTP sent to email",
  "data": {
    "otp": "485621"
  }
}
```

For complete API documentation, see [PROJECT_DOCUMENTATION.md](PROJECT_DOCUMENTATION.md)

---

## ??? Database Schema

### Core Tables (27 Total)

```
Users (Central Table)
??? Doctors (1:1)
?   ??? DoctorSchedule (1:*)
?   ??? DoctorLeaves (1:*)
?   ??? DoctorReviews (1:*)
??? Appointments (1:*)
?   ??? Prescriptions (1:*)
?       ??? PrescriptionMedicines (1:*)
??? MedicineReminders (1:*)
?   ??? MedicineReminderLogs (1:*)
??? EmailVerificationOTPs (1:*)
??? PasswordResetTokens (1:*)
??? Notifications (1:*)
??? MedicalHistory (1:*)
??? EmergencyContacts (1:*)
??? HealthTips (author: 1:*)
```

### Key Relationships

- **Users ? Doctors:** One-to-one (Doctor profile extends User)
- **Users ? Appointments:** One-to-many (as Patient)
- **Doctors ? Appointments:** One-to-many
- **Appointments ? Prescriptions:** One-to-many
- **Users ? MedicineReminders:** One-to-many
- **MedicineReminders ? Logs:** One-to-many

---

## ?? Project Structure

```
Backend-APIs/
??? Controllers/              # API Controllers
?   ??? AuthController.cs
?   ??? DoctorsController.cs
?   ??? AppointmentsController.cs
?   ??? MedicineRemindersController.cs
?   ??? UsersController.cs
??? Services/                 # Business Logic
?   ??? AuthService.cs
?   ??? UserService.cs
?   ??? EmailService.cs
??? Models/                   # EF Core Entities (27 tables)
?   ??? User.cs
?   ??? Doctor.cs
?   ??? Appointment.cs
?   ??? Medicinereminder.cs
?   ??? MediaidbContext.cs
??? DTOs/                     # Data Transfer Objects
?   ??? RegisterDto.cs
?   ??? LoginDto.cs
?   ??? ApiResponseDto.cs
?   ??? ... (20+ DTOs)
??? Program.cs               # Entry point
??? appsettings.json         # Configuration
??? Backend-APIs.csproj      # Project file
```

---

## ?? Configuration

### JWT Configuration
```json
{
  "Jwt": {
    "Key": "YOUR-SECRET-KEY-MINIMUM-32-CHARACTERS",
    "Issuer": "MediAI-Backend",
    "Audience": "MediAI-Users",
    "ExpiryInHours": 24
  }
}
```

### Email Configuration
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "MediAI Healthcare",
    "Username": "your-email@gmail.com",
    "Password": "your-gmail-app-password",
    "EnableSsl": true,
    "UseConsoleForDevelopment": true
  }
}
```

**Gmail App Password Setup:**
1. Enable 2-Factor Authentication on your Google account
2. Go to: https://myaccount.google.com/apppasswords
3. Generate app password for "Mail"
4. Use the 16-character password in configuration

### Database Connection
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=mediaidb;user=root;password=YOUR_PASSWORD;"
  }
}
```

---

## ?? Deployment

### Build for Production

```bash
# Restore packages
dotnet restore

# Build in Release mode
dotnet build --configuration Release

# Publish
dotnet publish --configuration Release --output ./publish
```

### Deploy to IIS (Windows)

1. Install .NET 8 Hosting Bundle
2. Copy `./publish` folder to IIS directory
3. Create new website in IIS Manager
4. Configure application pool (.NET Core)
5. Set up HTTPS binding with SSL certificate

### Deploy to Linux (Nginx)

1. Copy files to `/var/www/mediaibackend`
2. Create systemd service file
3. Configure Nginx reverse proxy
4. Install and configure SSL with Let's Encrypt
5. Start and enable service

```bash
# Create systemd service
sudo nano /etc/systemd/system/mediaibackend.service

# Enable and start
sudo systemctl enable mediaibackend
sudo systemctl start mediaibackend
```

---

## ?? Security Features

- ? **BCrypt Password Hashing** - Secure password storage
- ? **JWT Token Authentication** - Stateless authentication
- ? **Role-Based Authorization** - Fine-grained access control
- ? **OTP Email Verification** - Secure email verification
- ? **HTTPS Enforcement** - Encrypted communication
- ? **CORS Configuration** - Cross-origin security
- ? **SQL Injection Protection** - EF Core parameterization
- ? **Input Validation** - ModelState validation

---

## ?? Project Statistics

| Metric | Count |
|--------|-------|
| **Total Endpoints** | 40+ |
| **Controllers** | 5 |
| **Services** | 3 |
| **Database Tables** | 27 |
| **Entity Models** | 27 |
| **DTOs** | 20+ |
| **User Roles** | 4 |
| **Appointment Statuses** | 6 |
| **Lines of Code** | 5,000+ |

---

## ?? Testing

### Using Swagger UI
1. Start the application
2. Navigate to `https://localhost:7228/swagger`
3. Test endpoints interactively

### Using Postman
1. Import the provided Postman collection
2. Set environment variables
3. Run API tests

### Health Check
```bash
curl https://localhost:7228/api/Auth/health
```

**Expected Response:**
```json
{
  "status": "healthy",
  "timestamp": "2024-12-13T10:30:00Z",
  "message": "MediAI Backend API is running"
}
```

---

## ?? Documentation

- **[Complete API Documentation](PROJECT_DOCUMENTATION.md)** - Full project documentation
- **[Database Schema](mediaidb_schema.sql)** - Complete database script
- **[Swagger UI](https://localhost:7228/swagger)** - Interactive API explorer

---

## ?? Contributing

This is an educational project for BUITEMS. Contributions are welcome!

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -m 'Add YourFeature'`)
4. Push to the branch (`git push origin feature/YourFeature`)
5. Open a Pull Request

---

## ?? Development Status

### ? Completed Features

- [x] User Authentication & Authorization
- [x] Email Verification (OTP-based)
- [x] JWT Token Implementation
- [x] Doctor Management System
- [x] Appointment Booking & Management
- [x] Medicine Reminder System
- [x] Electronic Prescriptions
- [x] User Profile Management
- [x] Email Notifications
- [x] API Documentation (Swagger)
- [x] Database Schema (27 tables)
- [x] Role-Based Access Control
- [x] Password Reset Flow

### ?? Future Enhancements

- [ ] AI Symptom Checker Integration
- [ ] Push Notifications
- [ ] Real-time Chat with Doctors
- [ ] Video Consultation
- [ ] Payment Gateway Integration
- [ ] Mobile App Support (Flutter)
- [ ] Health Records Export (PDF)
- [ ] Analytics Dashboard

---

## ?? Team

**MediAI Development Team**  
Balochistan University of Information Technology, Engineering and Management Sciences (BUITEMS)

---

## ?? License

This project is developed for educational purposes at BUITEMS.  
See the [LICENSE](LICENSE) file for details.

---

## ?? Support

**Project:** MediAI Backend  
**Version:** 1.0.0  
**Institution:** BUITEMS  
**Repository:** [GitHub](https://github.com/Abdur-Rehman-007/Medi-AI_backend)

For issues and questions, please open an issue on GitHub.

---

## ?? Acknowledgments

- BUITEMS Faculty & Staff
- .NET Core Team
- MySQL Community
- Entity Framework Core Team
- Open Source Community

---

**Made with ?? at BUITEMS**

---

## ?? Quick Links

- [API Documentation](PROJECT_DOCUMENTATION.md)
- [Database Schema](mediaidb_schema.sql)
- [Swagger UI](https://localhost:7228/swagger) (when running)
- [GitHub Repository](https://github.com/Abdur-Rehman-007/Medi-AI_backend)

---

**Last Updated:** December 13, 2024  
**Document Version:** 1.0.0
