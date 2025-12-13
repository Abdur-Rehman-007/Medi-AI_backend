# ?? MediAI Backend - Complete Project Documentation

## ?? Table of Contents

1. [Project Overview](#project-overview)
2. [System Architecture](#system-architecture)
3. [Technology Stack](#technology-stack)
4. [Database Schema](#database-schema)
5. [Authentication Flow](#authentication-flow)
6. [API Endpoints Documentation](#api-endpoints-documentation)
7. [Data Models](#data-models)
8. [Business Logic Flow](#business-logic-flow)
9. [Security Implementation](#security-implementation)
10. [Deployment Guide](#deployment-guide)

---

## ?? Project Overview

**MediAI Backend** is a comprehensive healthcare management system designed specifically for educational institutions like BUITEMS. It provides a complete digital healthcare solution for students, faculty, doctors, and administrators.

### Key Features

- ? User Registration & Email Verification (OTP-based)
- ? JWT-based Authentication & Authorization
- ? Doctor Profile Management
- ? Appointment Booking & Management
- ? Medicine Reminder System
- ? Electronic Prescriptions
- ? Health Tips & Articles
- ? Doctor Reviews & Ratings
- ? Emergency Contact Management
- ? Medical History Tracking
- ? Email Notifications

### Project Metadata

| Property | Value |
|----------|-------|
| **Framework** | .NET 8 |
| **Language** | C# 12.0 |
| **Database** | MySQL 8.0 |
| **ORM** | Entity Framework Core |
| **Authentication** | JWT Bearer Token |
| **Email Service** | SMTP (Gmail) |
| **API Style** | RESTful |
| **Architecture** | Layered (MVC + Service Layer) |

---

## ??? System Architecture

### High-Level Architecture Diagram

```
???????????????????????????????????????????????????????????????
?                      CLIENT LAYER                            ?
?  (Flutter Mobile App / Web Frontend / Postman / Swagger)    ?
???????????????????????????????????????????????????????????????
                     ? HTTPS/REST API
                     ?
???????????????????????????????????????????????????????????????
?                   API LAYER (Controllers)                    ?
?  ????????????????????????????????????????????????????????? ?
?  ?AuthController?DoctorsCtrl   ?Appointments  ?Users     ? ?
?  ?              ?              ?Controller    ?Controller? ?
?  ????????????????????????????????????????????????????????? ?
???????????????????????????????????????????????????????????????
                     ? Dependency Injection
                     ?
???????????????????????????????????????????????????????????????
?               BUSINESS LOGIC LAYER (Services)                ?
?  ????????????????????????????????????????????????????????? ?
?  ?AuthService   ?UserService   ?EmailService  ?...       ? ?
?  ?(Business     ?(Profile Mgmt)?(SMTP)        ?          ? ?
?  ? Logic)       ?              ?              ?          ? ?
?  ????????????????????????????????????????????????????????? ?
???????????????????????????????????????????????????????????????
                     ? Entity Framework Core
                     ?
???????????????????????????????????????????????????????????????
?              DATA ACCESS LAYER (DbContext)                   ?
?  ?????????????????????????????????????????????????????????? ?
?  ?          MediaidbContext (EF Core DbContext)           ? ?
?  ?  - User Entities                                       ? ?
?  ?  - Doctor Entities                                     ? ?
?  ?  - Appointment Entities                                ? ?
?  ?  - Medicine Reminder Entities                          ? ?
?  ?????????????????????????????????????????????????????????? ?
???????????????????????????????????????????????????????????????
                     ? MySQL Connector
                     ?
???????????????????????????????????????????????????????????????
?                    DATABASE LAYER                            ?
?              MySQL 8.0 (mediaidb database)                   ?
?  ?????????????????????????????????????????????????????????? ?
?  ?  27 Tables + Views + Stored Procedures + Triggers      ? ?
?  ?????????????????????????????????????????????????????????? ?
???????????????????????????????????????????????????????????????
```

### Project Structure

```
Backend-APIs/
??? Controllers/              # API Endpoints (REST Controllers)
?   ??? AuthController.cs    # Authentication endpoints
?   ??? UsersController.cs   # User profile management
?   ??? DoctorsController.cs # Doctor management
?   ??? AppointmentsController.cs # Appointment booking
?   ??? MedicineRemindersController.cs # Medicine reminders
??? Services/                 # Business Logic Layer
?   ??? IAuthService.cs      # Auth service interface
?   ??? AuthService.cs       # Auth implementation
?   ??? IEmailService.cs     # Email service interface
?   ??? EmailService.cs      # Email implementation
?   ??? IUserService.cs      # User service interface
?   ??? UserService.cs       # User implementation
??? Models/                   # Database Entities (EF Core)
?   ??? MediaidbContext.cs   # DbContext configuration
?   ??? User.cs              # User entity
?   ??? Doctor.cs            # Doctor entity
?   ??? Appointment.cs       # Appointment entity
?   ??? Medicinereminder.cs  # Medicine reminder entity
?   ??? ... (27 total entities)
??? DTOs/                     # Data Transfer Objects
?   ??? RegisterDto.cs       # Registration request
?   ??? LoginDto.cs          # Login request
?   ??? ApiResponseDto.cs    # Standardized response
?   ??? ... (20+ DTOs)
??? Program.cs               # Application entry point
??? appsettings.json         # Configuration (Production)
??? appsettings.Development.json # Configuration (Development)
```

---

## ??? Technology Stack

### Backend Technologies

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| **Framework** | .NET | 8.0 | Web API framework |
| **Language** | C# | 12.0 | Programming language |
| **Database** | MySQL | 8.0 | Relational database |
| **ORM** | Entity Framework Core | 8.0 | Database access |
| **MySQL Provider** | Pomelo.EntityFrameworkCore.MySql | 8.0 | MySQL EF provider |
| **Authentication** | JWT Bearer | - | Token-based auth |
| **Password Hashing** | BCrypt.Net | - | Secure password storage |
| **Email** | System.Net.Mail | - | Email notifications |
| **API Documentation** | Swashbuckle (Swagger) | - | API docs |
| **DI Container** | Built-in | - | Dependency injection |

### Development Tools

- **IDE:** Visual Studio 2022
- **Version Control:** Git & GitHub
- **API Testing:** Postman, Swagger UI
- **Database Client:** MySQL Workbench
- **Package Manager:** NuGet

---

## ??? Database Schema

### Entity Relationship Diagram (ERD)

```
????????????????????
?      Users       ?
?  (Central Table) ?
????????????????????
? PK: Id           ?
?    Email         ?
?    PasswordHash  ?
?    FullName      ?
?    Role (ENUM)   ?
?    Department    ?
?    PhoneNumber   ?
?    IsEmailVer... ?
????????????????????
         ? 1
         ?
         ??????????????? 1:1
         ?             ?
         ?   ????????????????????
         ?   ?     Doctors      ?
         ?   ????????????????????
         ?   ? PK: Id           ?
         ?   ? FK: UserId       ?
         ?   ?    Specialization?
         ?   ?    LicenseNumber ?
         ?   ?    ConsultFee    ?
         ?   ?    AverageRating ?
         ?   ????????????????????
         ?            ? 1
         ?            ?
         ?            ? *
         ?   ????????????????????
         ?   ?  DoctorSchedule  ?
         ?   ????????????????????
         ?   ? PK: Id           ?
         ?   ? FK: DoctorId     ?
         ?   ?    DayOfWeek     ?
         ?   ?    StartTime     ?
         ?   ?    EndTime       ?
         ?   ????????????????????
         ?
         ??????????????? 1:*
         ?             ?
         ?   ????????????????????
         ?   ?  Appointments    ?
         ?   ????????????????????
         ?   ? PK: Id           ?
         ?   ? FK: PatientId    ?????
         ?   ? FK: DoctorId     ?   ?
         ?   ?    Date          ?   ?
         ?   ?    Time          ?   ?
         ?   ?    Status        ?   ?
         ?   ?    Symptoms      ?   ?
         ?   ????????????????????   ?
         ?            ? 1            ?
         ?            ? *            ?
         ?   ????????????????????   ?
         ?   ?  Prescriptions   ?   ?
         ?   ????????????????????   ?
         ?   ? PK: Id           ?   ?
         ?   ? FK: AppointmentId?   ?
         ?   ?    Diagnosis     ?   ?
         ?   ????????????????????   ?
         ?            ? 1            ?
         ?            ? *            ?
         ?   ????????????????????   ?
         ?   ?PrescriptionMeds  ?   ?
         ?   ????????????????????   ?
         ?   ? PK: Id           ?   ?
         ?   ? FK: PrescriptionId   ?
         ?   ?    MedicineName  ?   ?
         ?   ?    Dosage        ?   ?
         ?   ????????????????????   ?
         ?                           ?
         ??????????????? 1:*         ?
         ?             ?             ?
         ?   ????????????????????   ?
         ?   ?MedicineReminders ?   ?
         ?   ????????????????????   ?
         ?   ? PK: Id           ?   ?
         ?   ? FK: StudentId    ?????
         ?   ?    MedicineName  ?
         ?   ?    Dosage        ?
         ?   ?    Frequency     ?
         ?   ?    Times (JSON)  ?
         ?   ????????????????????
         ?            ? 1
         ?            ? *
         ?   ????????????????????
         ?   ?  ReminderLogs    ?
         ?   ????????????????????
         ?   ? PK: Id           ?
         ?   ? FK: ReminderId   ?
         ?   ?    ScheduledTime ?
         ?   ?    TakenTime     ?
         ?   ?    Status        ?
         ?   ????????????????????
         ?
         ??????????????? 1:*
                       ?
             ????????????????????
             ?EmailVerificationOTPs?
             ????????????????????
             ? PK: Id           ?
             ? FK: UserId       ?
             ?    OTP           ?
             ?    ExpiresAt     ?
             ?    IsUsed        ?
             ????????????????????
```

### Database Tables (27 Total)

| # | Table Name | Purpose | Key Relationships |
|---|-----------|---------|-------------------|
| 1 | **Users** | Central user table for all roles | Parent to most tables |
| 2 | **Doctors** | Doctor-specific profiles | 1:1 with Users |
| 3 | **DoctorSchedule** | Weekly availability | * to Doctors |
| 4 | **DoctorLeaves** | Leave/unavailability dates | * to Doctors |
| 5 | **Appointments** | Appointment bookings | * to Users, * to Doctors |
| 6 | **Prescriptions** | Medical prescriptions | * to Appointments |
| 7 | **PrescriptionMedicines** | Medicines in prescription | * to Prescriptions |
| 8 | **MedicineReminders** | Medicine reminder schedules | * to Users |
| 9 | **MedicineReminderLogs** | Intake tracking | * to MedicineReminders |
| 10 | **EmailVerificationOTPs** | Email verification codes | * to Users |
| 11 | **PasswordResetTokens** | Password reset tokens | * to Users |
| 12 | **DoctorReviews** | Patient reviews for doctors | * to Doctors, * to Users |
| 13 | **HealthTips** | Health tips & articles | * to Users (author) |
| 14 | **HealthTipInteractions** | User likes/bookmarks | * to HealthTips, * to Users |
| 15 | **Notifications** | User notifications | * to Users |
| 16 | **MedicalHistory** | Patient medical records | * to Users |
| 17 | **EmergencyContacts** | Emergency contact info | * to Users |
| 18 | **SymptomChecks** | AI symptom checker logs | * to Users |
| 19 | **AuditLogs** | System audit trail | * to Users |
| 20 | **Reports** | System reports metadata | * to Users |
| 21 | **SystemSettings** | System configuration | Key-value pairs |
| 22-27 | **Views** | Database views for reporting | Read-only |

---

## ?? Authentication Flow

### Complete Registration & Login Flow Diagram

```
???????????????????????????????????????????????????????????????????????
?                     USER REGISTRATION FLOW                           ?
???????????????????????????????????????????????????????????????????????

1. User Submits Registration Form
   ?
   ?
2. POST /api/Auth/register
   ?? Email: "student@buitms.edu.pk"
   ?? Password: "SecurePass123"
   ?? FullName: "Ahmed Ali"
   ?? Role: "Student"
   ?? Department: "Computer Science"
   ?? RegistrationNumber: "59858"
   ?? PhoneNumber: "03001234567"
   ?? DateOfBirth: "2002-05-15"
   ?? Gender: "Male"
   ?
   ?
3. AuthController.Register()
   ?
   ?
4. Validate ModelState
   ?? Check required fields
   ?? Validate email format
   ?? Validate data types
   ?
   ?
5. AuthService.RegisterAsync()
   ?
   ?
6. Check if email already exists
   ?
   ?? Yes ??????? Return "Email already registered"
   ?
   ?? No
      ?
      ?
7. Hash password with BCrypt
   ?  (Plain: "SecurePass123" ? Hash: "$2a$11$...")
   ?
   ?
8. Create User entity
   ?? Email: "student@buitms.edu.pk"
   ?? PasswordHash: "$2a$11$..."
   ?? FullName: "Ahmed Ali"
   ?? Role: "Student"
   ?? IsEmailVerified: false
   ?? IsActive: true
   ?
   ?
9. INSERT INTO users
   ?  (SQL transaction started)
   ?
   ?
10. Generate 6-digit OTP
    ?  (Random: 485621)
    ?
    ?
11. Create OTP record
    ?? UserId: 1
    ?? OTP: "485621"
    ?? ExpiresAt: NOW() + 10 minutes
    ?? IsUsed: false
    ?
    ?
12. INSERT INTO emailverificationotps
    ?  (SQL transaction committed)
    ?
    ?
13. Send OTP via Email
    ?? To: student@buitms.edu.pk
    ?? Subject: "Email Verification - MediAI"
    ?? Body: HTML template with OTP
    ?
    ?
14. Return Success Response
    {
      "success": true,
      "message": "Registration successful! OTP sent",
      "data": { "otp": "485621" }  ? Dev mode only
    }

???????????????????????????????????????????????????????????????????????
?                     EMAIL VERIFICATION FLOW                          ?
???????????????????????????????????????????????????????????????????????

1. User Enters OTP (485621)
   ?
   ?
2. POST /api/Auth/verify-otp
   ?? Email: "student@buitms.edu.pk"
   ?? OTP: "485621"
   ?
   ?
3. AuthController.VerifyOtp()
   ?
   ?
4. AuthService.VerifyOtpAsync()
   ?
   ?
5. Find User by Email
   ?
   ?? Not Found ??? Return "User not found"
   ?
   ?? Found
      ?
      ?
6. Find OTP Record
   ?  WHERE UserId = 1 AND OTP = "485621" AND IsUsed = false
   ?
   ?? Not Found ??? Return "Invalid OTP"
   ?
   ?? Found
      ?
      ?
7. Check OTP Expiration
   ?
   ?? Expired ??????? Return "OTP expired"
   ?
   ?? Valid
      ?
      ?
8. Update Database
   ?? UPDATE emailverificationotps SET IsUsed = true
   ?? UPDATE users SET IsEmailVerified = true
   ?
   ?
9. Send Welcome Email
   ?
   ?
10. Generate JWT Token
    ?? Claims:
    ?  - NameIdentifier: "1"
    ?  - Email: "student@buitms.edu.pk"
    ?  - Name: "Ahmed Ali"
    ?  - Role: "Student"
    ?? Algorithm: HMAC SHA256
    ?? Expiry: 24 hours
    ?? Token: "eyJhbGciOiJIUzI1NiIs..."
    ?
    ?
11. Return Success Response
    {
      "success": true,
      "message": "Email verified successfully!",
      "data": {
        "token": "eyJhbGciOiJIUzI1NiIs...",
        "user": {
          "userId": 1,
          "email": "student@buitms.edu.pk",
          "fullName": "Ahmed Ali",
          "role": "Student",
          "isEmailVerified": true
        }
      }
    }

???????????????????????????????????????????????????????????????????????
?                          LOGIN FLOW                                  ?
???????????????????????????????????????????????????????????????????????

1. User Submits Login Form
   ?
   ?
2. POST /api/Auth/login
   ?? Email: "student@buitms.edu.pk"
   ?? Password: "SecurePass123"
   ?
   ?
3. AuthController.Login()
   ?
   ?
4. AuthService.LoginAsync()
   ?
   ?
5. Find User by Email
   ?
   ?? Not Found ??? Return "Invalid email or password"
   ?
   ?? Found
      ?
      ?
6. Verify Password
   ?  BCrypt.Verify("SecurePass123", user.PasswordHash)
   ?
   ?? Invalid ?????? Return "Invalid email or password"
   ?
   ?? Valid
      ?
      ?
7. Check Email Verified
   ?
   ?? Not Verified ? Return "Please verify your email first"
   ?
   ?? Verified
      ?
      ?
8. Check Account Active
   ?
   ?? Inactive ????? Return "Account is deactivated"
   ?
   ?? Active
      ?
      ?
9. Update Last Login Timestamp
   ?  UPDATE users SET LastLoginAt = NOW()
   ?
   ?
10. Generate JWT Token
    ?  (Same as verification flow)
    ?
    ?
11. Return Success Response
    {
      "success": true,
      "message": "Login successful",
      "data": {
        "token": "eyJhbGciOiJIUzI1NiIs...",
        "user": { ... }
      }
    }
```

### JWT Token Structure

```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "nameid": "1",
    "email": "student@buitms.edu.pk",
    "unique_name": "Ahmed Ali",
    "role": "Student",
    "nbf": 1702468800,
    "exp": 1702555200,
    "iat": 1702468800,
    "iss": "MediAI-Backend",
    "aud": "MediAI-Users"
  },
  "signature": "HMACSHA256(...)"
}
```

---

## ?? API Endpoints Documentation

### Base URL
```
Development: https://localhost:7228/api
Production: https://your-domain.com/api
```

### Authentication Endpoints

#### 1. User Registration

**Endpoint:** `POST /Auth/register`

**Request Headers:**
```
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "student@buitms.edu.pk",
  "password": "SecurePass123",
  "fullName": "Ahmed Ali",
  "role": "Student",
  "department": "Computer Science",
  "registrationNumber": "59858",
  "phoneNumber": "03001234567",
  "dateOfBirth": "2002-05-15",
  "gender": "Male",
  "address": "Quetta, Pakistan"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Registration successful! OTP sent to email",
  "data": {
    "otp": "485621"  // Development mode only
  }
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Email already registered",
  "data": null
}
```

---

#### 2. Verify OTP

**Endpoint:** `POST /Auth/verify-otp`

**Request Body:**
```json
{
  "email": "student@buitms.edu.pk",
  "otp": "485621"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Email verified successfully! Welcome to MediAI Healthcare.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "userId": 1,
      "email": "student@buitms.edu.pk",
      "fullName": "Ahmed Ali",
      "role": "Student",
      "department": "Computer Science",
      "registrationNumber": "59858",
      "phoneNumber": "03001234567",
      "dateOfBirth": "2002-05-15",
      "gender": "Male",
      "isEmailVerified": true,
      "isActive": true
    }
  }
}
```

---

#### 3. User Login

**Endpoint:** `POST /Auth/login`

**Request Body:**
```json
{
  "email": "student@buitms.edu.pk",
  "password": "SecurePass123"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": { /* Same as verify-otp response */ }
  }
}
```

---

#### 4. Resend OTP

**Endpoint:** `POST /Auth/resend-otp`

**Request Body:**
```json
{
  "email": "student@buitms.edu.pk"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "OTP has been resent to your email",
  "data": {
    "otp": "123456"  // Development mode only
  }
}
```

---

#### 5. Send OTP (Alias)

**Endpoint:** `POST /Auth/send-otp`

Same as `resend-otp` - provided for Flutter compatibility.

---

#### 6. Forgot Password

**Endpoint:** `POST /Auth/forgot-password`

**Request Body:**
```json
{
  "email": "student@buitms.edu.pk"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Password reset code sent to your email",
  "data": null
}
```

---

#### 7. Reset Password

**Endpoint:** `POST /Auth/reset-password`

**Request Body:**
```json
{
  "email": "student@buitms.edu.pk",
  "token": "485621",
  "newPassword": "NewSecurePass456"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Password reset successful. You can now login with your new password",
  "data": null
}
```

---

#### 8. Get Current User

**Endpoint:** `GET /Auth/current-user`

**Request Headers:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "User retrieved successfully",
  "data": {
    "userId": 1,
    "email": "student@buitms.edu.pk",
    "fullName": "Ahmed Ali",
    "role": "Student",
    /* ... */
  }
}
```

---

#### 9. Health Check

**Endpoint:** `GET /Auth/health`

**Success Response (200 OK):**
```json
{
  "status": "healthy",
  "timestamp": "2024-12-13T10:30:00Z",
  "message": "MediAI Backend API is running"
}
```

---

### Doctor Management Endpoints

#### 10. Get All Doctors

**Endpoint:** `GET /Doctors`

**Authentication:** Not required

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "userId": 3,
    "specialization": "General Physician",
    "licenseNumber": "PMC-12345",
    "qualification": "MBBS, FCPS",
    "experience": 8,
    "consultationFee": 500.00,
    "roomNumber": "Room 201",
    "bio": "Experienced general physician...",
    "averageRating": 4.75,
    "totalRatings": 24,
    "isAvailable": true,
    "user": {
      "id": 3,
      "fullName": "Dr. Muhammad Hassan",
      "email": "doctor@buitms.edu.pk",
      "phoneNumber": "+92-300-3456789",
      "profileImageUrl": null
    }
  }
]
```

---

#### 11. Get Doctor by ID

**Endpoint:** `GET /Doctors/{id}`

**Success Response (200 OK):**
```json
{
  "id": 1,
  "userId": 3,
  "specialization": "General Physician",
  /* ... doctor details ... */,
  "schedules": [
    {
      "id": 1,
      "dayOfWeek": "Monday",
      "startTime": "09:00:00",
      "endTime": "17:00:00",
      "isActive": true
    },
    {
      "id": 2,
      "dayOfWeek": "Tuesday",
      "startTime": "09:00:00",
      "endTime": "17:00:00",
      "isActive": true
    }
  ],
  "reviews": [
    {
      "id": 1,
      "patientName": "Ahmed Ali",
      "rating": 5,
      "review": "Excellent doctor!",
      "createdAt": "2024-12-10T14:30:00Z"
    }
  ]
}
```

---

### Appointment Management Endpoints

#### 12. Book Appointment

**Endpoint:** `POST /Appointments`

**Authentication:** Required (Bearer Token)

**Request Headers:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

**Request Body:**
```json
{
  "doctorId": "1",
  "dateTime": "2024-12-20T14:30:00",
  "symptoms": "Fever, headache, and cough for 3 days",
  "notes": "Need urgent consultation"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointment booked successfully",
  "data": {
    "id": "15",
    "patientId": "1",
    "patientName": "Ahmed Ali",
    "doctorId": "1",
    "doctorName": "Dr. Muhammad Hassan",
    "specialization": "General Physician",
    "dateTime": "2024-12-20T14:30:00",
    "status": "Pending",
    "symptoms": "Fever, headache, and cough for 3 days",
    "notes": "Need urgent consultation",
    "prescription": null,
    "createdAt": "2024-12-13T10:45:00Z"
  }
}
```

**Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "This time slot is already booked",
  "data": null
}
```

---

#### 13. Get Student Upcoming Appointments

**Endpoint:** `GET /Appointments/student/{studentId}/upcoming`

**Authentication:** Required

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointments retrieved successfully",
  "data": [
    {
      "id": "15",
      "patientId": "1",
      "patientName": "Ahmed Ali",
      "doctorId": "1",
      "doctorName": "Dr. Muhammad Hassan",
      "specialization": "General Physician",
      "dateTime": "2024-12-20T14:30:00",
      "status": "Pending",
      "symptoms": "Fever, headache, and cough for 3 days",
      "notes": "Need urgent consultation",
      "prescription": null,
      "createdAt": "2024-12-13T10:45:00Z"
    }
  ]
}
```

---

#### 14. Get Doctor Appointments

**Endpoint:** `GET /Appointments/Faculty/appointments`

**Authentication:** Required (Doctor, Faculty, or Admin role)

**Authorization:** `[Authorize(Roles = "Doctor,Faculty,Admin")]`

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointments retrieved successfully",
  "data": [
    {
      "id": "15",
      "patientId": "1",
      "patientName": "Ahmed Ali",
      "doctorId": "1",
      "doctorName": "Dr. Muhammad Hassan",
      "specialization": "General Physician",
      "dateTime": "2024-12-20T14:30:00",
      "status": "Confirmed",
      "symptoms": "Fever, headache, and cough for 3 days",
      "notes": null,
      "prescription": null,
      "createdAt": "2024-12-13T10:45:00Z"
    }
  ]
}
```

---

#### 15. Cancel Appointment

**Endpoint:** `PUT /Appointments/{id}/cancel`

**Authentication:** Required

**Request Body:**
```json
{
  "cancellationReason": "Unable to attend due to emergency"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointment cancelled successfully",
  "data": null
}
```

---

#### 16. Add Prescription

**Endpoint:** `POST /Appointments/{id}/prescription`

**Authentication:** Required (Doctor role)

**Request Body:**
```json
{
  "diagnosis": "Viral Fever with Upper Respiratory Tract Infection",
  "notes": "Rest for 3 days. Drink plenty of fluids.",
  "medicines": [
    {
      "medicineName": "Paracetamol",
      "dosage": "500mg",
      "frequency": "Three times daily",
      "duration": "5 days",
      "instructions": "Take after meals"
    },
    {
      "medicineName": "Amoxicillin",
      "dosage": "250mg",
      "frequency": "Twice daily",
      "duration": "7 days",
      "instructions": "Complete the course"
    }
  ]
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Prescription added successfully",
  "data": {
    "prescriptionId": 5,
    "appointmentId": 15
  }
}
```

---

### Medicine Reminder Endpoints

#### 17. Get All Reminders

**Endpoint:** `GET /MedicineReminders`

**Authentication:** Required

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "medicineName": "Paracetamol",
    "dosage": "500mg",
    "frequency": "Twice",
    "customFrequency": null,
    "times": ["09:00", "21:00"],
    "startDate": "2024-12-13",
    "endDate": "2024-12-18",
    "notes": "Take after meals",
    "isActive": true,
    "createdAt": "2024-12-13T08:00:00Z"
  }
]
```

---

#### 18. Get Active Reminders

**Endpoint:** `GET /MedicineReminders/active`

**Authentication:** Required

**Success Response (200 OK):**
```json
[
  {
    "id": 1,
    "medicineName": "Paracetamol",
    "dosage": "500mg",
    "frequency": "Twice",
    "times": ["09:00", "21:00"],
    "startDate": "2024-12-13",
    "endDate": "2024-12-18",
    "notes": "Take after meals"
  }
]
```

---

#### 19. Create Reminder

**Endpoint:** `POST /MedicineReminders`

**Authentication:** Required

**Request Body:**
```json
{
  "medicineName": "Paracetamol",
  "dosage": "500mg",
  "frequency": "Twice",
  "customFrequency": null,
  "times": ["09:00", "21:00"],
  "startDate": "2024-12-13",
  "endDate": "2024-12-18",
  "notes": "Take after meals"
}
```

**Success Response (201 Created):**
```json
{
  "message": "Medicine reminder created successfully",
  "reminderId": 1
}
```

---

#### 20. Update Reminder

**Endpoint:** `PUT /MedicineReminders/{id}`

**Authentication:** Required

**Request Body:**
```json
{
  "medicineName": "Paracetamol",
  "dosage": "500mg",
  "frequency": "Thrice",
  "customFrequency": null,
  "times": ["08:00", "14:00", "20:00"],
  "startDate": "2024-12-13",
  "endDate": "2024-12-18",
  "notes": "Take after meals"
}
```

**Success Response (200 OK):**
```json
{
  "message": "Reminder updated successfully"
}
```

---

#### 21. Toggle Reminder

**Endpoint:** `PATCH /MedicineReminders/{id}/toggle`

**Authentication:** Required

**Success Response (200 OK):**
```json
{
  "message": "Reminder activated",
  "isActive": true
}
```

---

#### 22. Delete Reminder

**Endpoint:** `DELETE /MedicineReminders/{id}`

**Authentication:** Required

**Success Response (200 OK):**
```json
{
  "message": "Reminder deleted successfully"
}
```

---

#### 23. Log Medicine Intake

**Endpoint:** `POST /MedicineReminders/{id}/log`

**Authentication:** Required

**Request Body:**
```json
{
  "status": "Taken",
  "notes": "Took medicine at 9:15 AM"
}
```

**Success Response (200 OK):**
```json
{
  "message": "Intake logged successfully"
}
```

---

### User Profile Endpoints

#### 24. Get User Profile

**Endpoint:** `GET /Users/profile`

**Authentication:** Required

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Profile retrieved successfully",
  "data": {
    "userId": 1,
    "email": "student@buitms.edu.pk",
    "fullName": "Ahmed Ali",
    "role": "Student",
    "department": "Computer Science",
    "registrationNumber": "59858",
    "phoneNumber": "03001234567",
    "dateOfBirth": "2002-05-15",
    "gender": "Male",
    "address": "Quetta, Pakistan",
    "profileImageUrl": null,
    "isEmailVerified": true,
    "isActive": true
  }
}
```

---

#### 25. Update Profile

**Endpoint:** `PUT /Users/profile`

**Authentication:** Required

**Request Body:**
```json
{
  "fullName": "Ahmed Ali Khan",
  "phoneNumber": "03001234567",
  "dateOfBirth": "2002-05-15",
  "gender": "Male",
  "address": "Quetta, Balochistan, Pakistan",
  "department": "Computer Science"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Profile updated successfully",
  "data": {
    /* Updated user profile */
  }
}
```

---

#### 26. Change Password

**Endpoint:** `POST /Users/change-password`

**Authentication:** Required

**Request Body:**
```json
{
  "currentPassword": "SecurePass123",
  "newPassword": "NewSecurePass456",
  "confirmPassword": "NewSecurePass456"
}
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Password changed successfully",
  "data": null
}
```

---

#### 27. Upload Profile Photo

**Endpoint:** `POST /Users/upload-photo`

**Authentication:** Required

**Request Headers:**
```
Content-Type: multipart/form-data
```

**Request Body (Form Data):**
```
photo: [File]
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Photo uploaded successfully",
  "data": {
    "profileImageUrl": "/uploads/profiles/user-1-20241213.jpg"
  }
}
```

---

## ?? Data Models

### User Roles

```csharp
public enum UserRole
{
    Student,   // Can book appointments, set reminders
    Faculty,   // Same as Student + teaching features
    Doctor,    // Can manage appointments, prescriptions
    Admin      // Full system access
}
```

### Appointment Statuses

```csharp
public enum AppointmentStatus
{
    Pending,      // Just booked
    Confirmed,    // Doctor confirmed
    InProgress,   // Consultation ongoing
    Completed,    // Successfully completed
    Cancelled,    // Cancelled by patient/doctor
    NoShow        // Patient didn't show up
}
```

### Medicine Reminder Frequency

```csharp
public enum ReminderFrequency
{
    Once,        // Once daily
    Twice,       // Twice daily
    Thrice,      // Three times daily
    FourTimes,   // Four times daily
    Custom       // Custom schedule (use Times JSON)
}
```

---

## ?? Security Implementation

### Password Security

- **Algorithm:** BCrypt with salt rounds
- **Hashing:** One-way encryption
- **Storage:** Only hashed passwords stored
- **Verification:** BCrypt.Verify() for authentication

```csharp
// Hashing
var passwordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

// Verification
bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);
```

### JWT Token Security

- **Algorithm:** HMAC SHA256
- **Secret Key:** Minimum 32 characters
- **Claims:** User ID, Email, Name, Role
- **Expiry:** 24 hours (configurable)
- **Validation:**
  - Issuer validation
  - Audience validation
  - Lifetime validation
  - Signature validation

### API Security

- **CORS:** Configured for allowed origins
- **HTTPS:** Enforced in production
- **Authorization:** Role-based access control
- **Input Validation:** ModelState validation
- **SQL Injection:** Protected by EF Core parameterization

---

## ?? Deployment Guide

### Prerequisites

1. **.NET 8 Runtime** installed
2. **MySQL 8.0** server running
3. **Web server** (IIS, Nginx, or Kestrel)
4. **SSL Certificate** for HTTPS

### Step 1: Prepare Database

```sql
-- Create database
CREATE DATABASE mediaidb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Run schema script
SOURCE mediaidb_schema.sql;

-- Verify tables
USE mediaidb;
SHOW TABLES;
```

### Step 2: Configure Application

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=your-server;port=3306;database=mediaidb;user=your-user;password=your-password;"
  },
  "Jwt": {
    "Key": "YOUR-PRODUCTION-SECRET-KEY-MIN-32-CHARS",
    "Issuer": "MediAI-Backend",
    "Audience": "MediAI-Users",
    "ExpiryInHours": 24
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "MediAI Healthcare",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "EnableSsl": true,
    "UseConsoleForDevelopment": false
  }
}
```

### Step 3: Build Application

```bash
# Restore packages
dotnet restore

# Build in Release mode
dotnet build --configuration Release

# Publish
dotnet publish --configuration Release --output ./publish
```

### Step 4: Deploy to Server

**Option A: IIS (Windows)**
1. Copy `./publish` folder to IIS directory
2. Create new website in IIS Manager
3. Set application pool to ".NET Core"
4. Configure bindings (HTTPS)

**Option B: Linux (Nginx + Kestrel)**
1. Copy files to `/var/www/mediaibackend`
2. Create systemd service
3. Configure Nginx reverse proxy
4. Enable and start service

### Step 5: SSL Configuration

```bash
# Using Let's Encrypt (certbot)
certbot --nginx -d your-domain.com
```

### Step 6: Test Deployment

```bash
# Health check
curl https://your-domain.com/api/Auth/health

# Expected response
{
  "status": "healthy",
  "timestamp": "2024-12-13T10:30:00Z",
  "message": "MediAI Backend API is running"
}
```

---

## ?? Support & Contact

**Project:** MediAI Backend  
**Version:** 1.0.0  
**Institution:** BUITEMS (Balochistan University of Information Technology, Engineering and Management Sciences)  
**Repository:** [GitHub Link]

---

## ?? License

This project is developed for educational purposes at BUITEMS.

---

**Document Version:** 1.0  
**Last Updated:** December 13, 2024  
**Generated By:** MediAI Development Team
