# MediAI Backend API

A comprehensive Healthcare Management System API built with ASP.NET Core 8.0. This backend provides robust features for managing healthcare operations including user authentication, doctor management, appointment scheduling, and medicine reminders.

## 📋 About The Project

MediAI Backend is a modern, RESTful API designed to power healthcare management applications. It provides a secure and scalable foundation for managing medical services, connecting patients with healthcare providers, and facilitating efficient healthcare delivery.

### Key Features

- **🔐 User Authentication & Authorization**
  - JWT-based authentication
  - Email verification with OTP
  - Password reset functionality
  - Role-based access control (Patient, Doctor, Admin)

- **👨‍⚕️ Doctor Management**
  - Doctor profiles with specializations
  - License and qualification verification
  - Doctor availability management
  - Doctor search and filtering
  - Rating and review system
  - Schedule management

- **📅 Appointment System**
  - Book appointments with doctors
  - Manage appointment schedules
  - View appointment history
  - Cancel and reschedule appointments
  - Conflict detection for time slots

- **💊 Medicine Reminder System**
  - Create and manage medicine reminders
  - Customizable dosage and frequency
  - Active reminder tracking
  - Reminder logs and history

- **👤 User Management**
  - User profile management
  - Profile photo upload
  - Emergency contact management
  - Medical history tracking

- **📧 Email Services**
  - OTP verification emails
  - Password reset emails
  - Appointment notifications

## 🛠️ Built With

- **Framework:** ASP.NET Core 8.0
- **Database:** MySQL with Entity Framework Core
- **Authentication:** JWT Bearer Tokens
- **Password Hashing:** BCrypt.Net
- **API Documentation:** Swagger/OpenAPI
- **ORM:** Entity Framework Core 9.0
- **MySQL Driver:** Pomelo.EntityFrameworkCore.MySql 9.0

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server 8.0+](https://dev.mysql.com/downloads/mysql/)
- A code editor (Visual Studio 2022, VS Code, or Rider)

## 🚀 Getting Started

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Abdur-Rehman-007/Medi-AI_backend.git
   cd Medi-AI_backend
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure the database**
   
   Update the connection string in `Backend-APIs/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "server=localhost;port=3306;database=mediaidb;user=root;password=YOUR_PASSWORD;"
   }
   ```

4. **Configure JWT settings**
   
   Update JWT settings in `Backend-APIs/appsettings.json`:
   ```json
   "Jwt": {
     "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!@#$%",
     "Issuer": "MediAI-Backend",
     "Audience": "MediAI-Users",
     "ExpiryInHours": 24
   }
   ```

5. **Configure Email settings**
   
   For email functionality, update in `Backend-APIs/appsettings.json`:
   ```json
   "EmailSettings": {
     "SmtpHost": "smtp.gmail.com",
     "SmtpPort": 587,
     "SenderEmail": "your-email@gmail.com",
     "SenderName": "MediAI Healthcare",
     "Username": "your-email@gmail.com",
     "Password": "your-app-password",
     "EnableSsl": true
   }
   ```

6. **Create the database**
   
   The database schema will be automatically created by Entity Framework. Ensure your MySQL server is running and the database exists:
   ```sql
   CREATE DATABASE mediaidb;
   ```

7. **Run the application**
   ```bash
   cd Backend-APIs
   dotnet run
   ```

   The API will be available at:
   - HTTPS: `https://localhost:7001`
   - HTTP: `http://localhost:5000`

## 📚 API Documentation

Once the application is running, you can access the Swagger UI documentation at:
```
https://localhost:7001/swagger
```

### Main API Endpoints

#### Authentication (`/api/Auth`)
- `POST /api/Auth/register` - Register a new user
- `POST /api/Auth/verify-otp` - Verify email with OTP
- `POST /api/Auth/login` - User login
- `POST /api/Auth/forgot-password` - Request password reset
- `POST /api/Auth/reset-password` - Reset password
- `POST /api/Auth/resend-otp` - Resend OTP
- `GET /api/Auth/current-user` - Get current user details
- `GET /api/Auth/health` - API health check

#### Doctors (`/api/Doctors`)
- `GET /api/Doctors` - Get all doctors
- `GET /api/Doctors/{id}` - Get doctor by ID
- `GET /api/Doctors/specializations` - Get all specializations
- `POST /api/Doctors/search` - Search doctors
- `GET /api/Doctors/{doctorId}/schedule` - Get doctor schedule
- `GET /api/Doctors/{doctorId}/available-slots` - Get available slots

#### Appointments (`/api/Appointments`)
- `POST /api/Appointments` - Book an appointment
- `GET /api/Appointments` - Get user appointments
- `GET /api/Appointments/{id}` - Get appointment details
- `PUT /api/Appointments/{id}/cancel` - Cancel appointment
- `PUT /api/Appointments/{id}/reschedule` - Reschedule appointment

#### Medicine Reminders (`/api/MedicineReminders`)
- `GET /api/MedicineReminders` - Get all reminders
- `GET /api/MedicineReminders/active` - Get active reminders
- `POST /api/MedicineReminders` - Create a reminder
- `PUT /api/MedicineReminders/{id}` - Update reminder
- `DELETE /api/MedicineReminders/{id}` - Delete reminder

#### Users (`/api/Users`)
- `GET /api/Users/profile` - Get user profile
- `PUT /api/Users/profile` - Update user profile
- `POST /api/Users/profile-photo` - Upload profile photo
- `POST /api/Users/change-password` - Change password

## 🗂️ Project Structure

```
Medi-AI_backend/
├── Backend-APIs/
│   ├── Controllers/         # API Controllers
│   │   ├── AuthController.cs
│   │   ├── DoctorsController.cs
│   │   ├── AppointmentsController.cs
│   │   ├── MedicineRemindersController.cs
│   │   └── UsersController.cs
│   ├── DTOs/               # Data Transfer Objects
│   ├── Models/             # Entity Models
│   │   ├── User.cs
│   │   ├── Doctor.cs
│   │   ├── Appointment.cs
│   │   ├── Medicinereminder.cs
│   │   └── MediaidbContext.cs
│   ├── Services/           # Business Logic Services
│   │   ├── AuthService.cs
│   │   ├── EmailService.cs
│   │   └── UserService.cs
│   ├── Program.cs          # Application entry point
│   ├── appsettings.json    # Configuration
│   └── Backend-APIs.csproj
├── DTOs/                   # Shared DTOs
├── Services/               # Shared Services
└── Backend-APIs.sln        # Solution file
```

## 🔒 Security Features

- JWT Bearer token authentication
- Password hashing using BCrypt
- Email verification via OTP
- Secure password reset mechanism
- Role-based authorization
- HTTPS enforcement
- CORS configuration

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Abdur Rehman**

## 🙏 Acknowledgments

- Built with ASP.NET Core
- Entity Framework Core for data access
- JWT for secure authentication
- Swagger for API documentation

---

For any questions or support, please open an issue on GitHub.
