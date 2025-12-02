# ?? MediAI Backend API

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-Production%20Ready-success?style=for-the-badge)

A comprehensive healthcare management system backend built with .NET 8, providing REST APIs for patient management, doctor scheduling, appointments, and medicine reminders.

## ?? Features

- ? **User Authentication** - JWT-based secure authentication with OTP verification
- ? **Doctor Management** - Complete doctor profiles with specializations and availability
- ? **Appointment System** - Book, manage, and track appointments with conflict detection
- ? **Medicine Reminders** - Set and manage medicine intake reminders with logging
- ? **Email Notifications** - Beautiful HTML email templates for OTP and welcome messages
- ? **Role-Based Access** - Patient, Doctor, and Admin roles with proper authorization
- ? **Swagger Documentation** - Interactive API documentation and testing

## ?? API Statistics

| Metric | Count |
|--------|-------|
| **Total Controllers** | 4 |
| **Total Endpoints** | 28 |
| **Public Endpoints** | 8 |
| **Protected Endpoints** | 20 |

## ??? Tech Stack

- **.NET 8** - Modern web API framework
- **MySQL 8.0+** - Relational database
- **Entity Framework Core 9.0** - ORM for database operations
- **Pomelo.EntityFrameworkCore.MySql** - MySQL provider for EF Core
- **JWT Bearer Authentication** - Secure token-based authentication
- **BCrypt.Net** - Password hashing
- **Swagger/OpenAPI** - Interactive API documentation
- **System.Net.Mail** - Email service integration

## ?? Prerequisites

Before running this project, ensure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MySQL Server 8.0+](https://dev.mysql.com/downloads/mysql/)
- Visual Studio 2022, VS Code, or JetBrains Rider
- Git

## ?? Installation

### 1. Clone the Repository

```bash
git clone https://github.com/Abdur-Rehman-007/Medi-AI_backend.git
cd Medi-AI_backend/Backend-APIs
```

### 2. Configure Database Connection

Update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=mediaidb;user=root;password=YOUR_PASSWORD;"
  }
}
```

### 3. Restore NuGet Packages

```bash
dotnet restore
```

### 4. Run Database Migrations (if needed)

The models are already scaffolded from the existing database. If starting fresh:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The API will be available at:
- **HTTP:** `http://localhost:5000`
- **HTTPS:** `https://localhost:7102`
- **Swagger UI:** `https://localhost:7102/swagger`

## ?? API Documentation

### Quick Links

- **[Complete API Documentation](Backend-APIs/API_DOCUMENTATION.md)** - All 28 endpoints with examples
- **[Flutter Integration Guide](Backend-APIs/FLUTTER_INTEGRATION_GUIDE.md)** - Step-by-step frontend integration
- **[Implementation Status](Backend-APIs/IMPLEMENTATION_STATUS.md)** - Detailed feature status

### Main Endpoints

#### ?? Authentication (`/api/Auth`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/register` | Register new user with OTP |
| POST | `/verify-otp` | Verify OTP and activate account |
| POST | `/login` | User login with JWT token |
| GET | `/current-user` | Get authenticated user details |
| GET | `/health` | API health check |

#### ????? Doctors (`/api/Doctors`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | List all doctors |
| GET | `/{id}` | Get doctor details with reviews |
| GET | `/specialization/{spec}` | Filter by specialization |
| GET | `/available` | Get available doctors only |
| PATCH | `/{id}/availability` | Update availability |
| PUT | `/{id}` | Update doctor profile |

#### ?? Appointments (`/api/Appointments`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/my-appointments` | Get user's appointments |
| GET | `/{id}` | Get appointment details |
| GET | `/upcoming` | Get upcoming appointments |
| POST | `/` | Book new appointment |
| PATCH | `/{id}/status` | Update appointment status |
| POST | `/{id}/cancel` | Cancel appointment |

#### ?? Medicine Reminders (`/api/MedicineReminders`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | List all reminders |
| GET | `/active` | Get active reminders |
| GET | `/today` | Today's medicine schedule |
| POST | `/` | Create new reminder |
| PUT | `/{id}` | Update reminder |
| PATCH | `/{id}/toggle` | Toggle active/inactive |
| DELETE | `/{id}` | Delete reminder |
| POST | `/{id}/log` | Log medicine intake |

## ?? Testing

### Using Swagger UI

1. Start the application: `dotnet run`
2. Navigate to: `https://localhost:7102/swagger`
3. Test endpoints directly from the browser

### Authentication Flow

```bash
# 1. Register
POST /api/Auth/register
{
  "email": "user@example.com",
  "password": "Password123!",
  "fullName": "John Doe",
  "role": "patient"
}

# 2. Check console for OTP (development mode)
# OTP: 123456

# 3. Verify OTP
POST /api/Auth/verify-otp
{
  "email": "user@example.com",
  "otp": "123456"
}

# 4. Copy the JWT token from response

# 5. In Swagger, click "Authorize" button
# Enter: Bearer YOUR_JWT_TOKEN

# 6. Test protected endpoints
GET /api/Auth/current-user
```

## ?? Security Features

- ? **Password Hashing** - BCrypt with automatic salt generation
- ? **JWT Authentication** - Secure token-based authentication
- ? **Role-Based Authorization** - Fine-grained access control
- ? **HTTPS/TLS** - Encrypted communication
- ? **CORS Policy** - Configured for mobile and web apps
- ? **Input Validation** - Request model validation
- ? **OTP Verification** - Email verification before activation

## ?? Frontend Integration

### Flutter Integration

Complete Flutter integration guide with code examples is available in [`FLUTTER_INTEGRATION_GUIDE.md`](Backend-APIs/FLUTTER_INTEGRATION_GUIDE.md).

**Quick Start:**

```dart
// Configure base URL
static const String baseUrl = 'http://10.0.2.2:5000'; // Android Emulator

// Test connection
final response = await http.get(
  Uri.parse('$baseUrl/api/Auth/health'),
);
```

### Base URLs for Testing

- **Android Emulator:** `http://10.0.2.2:5000`
- **iOS Simulator:** `http://localhost:5000`
- **Real Device:** `http://YOUR_COMPUTER_IP:5000`

## ?? Project Structure

```
Backend-APIs/
??? Controllers/
?   ??? AuthController.cs              # Authentication endpoints
?   ??? DoctorsController.cs           # Doctor management
?   ??? AppointmentsController.cs      # Appointment booking
?   ??? MedicineRemindersController.cs # Medicine reminders
??? Models/                            # EF Core entity models
?   ??? User.cs
?   ??? Doctor.cs
?   ??? Appointment.cs
?   ??? Medicinereminder.cs
??? DTOs/                              # Data transfer objects
?   ??? RegisterDto.cs
?   ??? LoginDto.cs
?   ??? UserDto.cs
?   ??? AuthResponseDto.cs
??? Services/
?   ??? AuthService.cs                 # Authentication logic
?   ??? EmailService.cs                # Email sending
?   ??? Interfaces/
??? Program.cs                         # Application startup
??? appsettings.json                   # Configuration
??? appsettings.Development.json       # Dev configuration
```

## ?? Configuration

### JWT Settings

Update `appsettings.json` for production:

```json
{
  "Jwt": {
    "Key": "YOUR_SECURE_KEY_MINIMUM_32_CHARACTERS_LONG",
    "Issuer": "MediAI-Backend",
    "Audience": "MediAI-Users",
    "ExpiryInHours": 24
  }
}
```

### Email Settings (Optional)

Configure SMTP for email functionality:

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderName": "MediAI Healthcare",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "EnableSsl": true,
    "UseConsoleForDevelopment": true
  }
}
```

For Gmail, use [App Passwords](https://support.google.com/accounts/answer/185833).

## ?? Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Backend-APIs.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend-APIs.dll"]
```

### Azure App Service

1. Create Azure App Service (Windows, .NET 8)
2. Configure connection string in Application Settings
3. Set JWT key as environment variable
4. Deploy using Visual Studio or Azure CLI

### Environment Variables

```bash
ConnectionStrings__DefaultConnection="server=prod-db;database=mediaidb;user=prod;password=secure"
Jwt__Key="your-production-secret-key"
EmailSettings__Username="production@email.com"
EmailSettings__Password="production-password"
```

## ?? Troubleshooting

### Port Already in Use

```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5000
kill -9 <PID>
```

### Database Connection Failed

- Verify MySQL is running: `mysql -u root -p`
- Check connection string in `appsettings.Development.json`
- Ensure MySQL allows remote connections

### CORS Errors

CORS is configured to allow all origins in development. For production, update `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://your-frontend-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

## ?? Database Schema

The application uses the following main tables:

- **users** - User accounts and profiles
- **doctors** - Doctor-specific information
- **appointments** - Appointment bookings
- **medicinereminders** - Medicine reminder schedules
- **emailverificationotps** - OTP verification codes
- **doctorreviews** - Doctor ratings and reviews
- **doctorschedules** - Doctor availability schedules

See scaffolded models in `Models/` directory for complete schema.

## ?? Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Author

**Abdur Rehman**
- GitHub: [@Abdur-Rehman-007](https://github.com/Abdur-Rehman-007)

## ?? Acknowledgments

- Built with [.NET 8](https://dotnet.microsoft.com/)
- Database provider: [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
- Authentication: [JWT Bearer](https://jwt.io/)
- Password hashing: [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)

## ?? Support

For issues, questions, or feature requests:
- Open an [Issue](https://github.com/Abdur-Rehman-007/Medi-AI_backend/issues)
- Check [Documentation](Backend-APIs/API_DOCUMENTATION.md)

## ?? Roadmap

- [x] User authentication with JWT
- [x] Doctor management system
- [x] Appointment booking
- [x] Medicine reminders
- [x] Email notifications
- [ ] Push notifications
- [ ] Real-time chat
- [ ] Video consultation
- [ ] Payment integration
- [ ] Analytics dashboard

---

**Made with ?? for better healthcare management**
