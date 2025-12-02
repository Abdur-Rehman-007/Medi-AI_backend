# MediAI Backend API

A comprehensive healthcare management system backend built with .NET 8, providing REST APIs for patient management, doctor scheduling, appointments, and medicine reminders.

## ?? Features

- ? **User Authentication** - JWT-based secure authentication with OTP verification
- ? **Doctor Management** - Complete doctor profiles with specializations and availability
- ? **Appointment System** - Book, manage, and track appointments
- ? **Medicine Reminders** - Set and manage medicine intake reminders with logging
- ? **Email Notifications** - Beautiful HTML email templates (optional)
- ? **Role-Based Access** - Patient, Doctor, and Admin roles
- ? **Swagger Documentation** - Interactive API documentation

## ??? Tech Stack

- **.NET 8** - Web API Framework
- **MySQL** - Database (via Pomelo.EntityFrameworkCore.MySql)
- **Entity Framework Core 9.0** - ORM
- **JWT Bearer Authentication** - Secure token-based auth
- **BCrypt** - Password hashing
- **Swagger/OpenAPI** - API documentation
- **System.Net.Mail** - Email service (optional)

## ?? Prerequisites

- .NET 8 SDK
- MySQL Server 8.0+
- Visual Studio 2022 or VS Code
- Postman or similar API testing tool (optional)

## ?? Installation & Setup

### 1. Clone the Repository
```bash
git clone <your-repo-url>
cd Backend-APIs
```

### 2. Update Database Connection
Edit `appsettings.Development.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=mediaidb;user=root;password=YOUR_PASSWORD;"
}
```

### 3. Restore Packages
```bash
dotnet restore
```

### 4. Apply Database Migrations (if needed)
The models are already scaffolded from the existing database. If you need to create a new database:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:7102`
- Swagger UI: `https://localhost:7102/swagger`

## ?? API Documentation

### Quick Links
- **Full API Documentation**: See `API_DOCUMENTATION.md`
- **Flutter Integration Guide**: See `FLUTTER_INTEGRATION_GUIDE.md`
- **Swagger UI**: Available at `/swagger` when running

### Authentication Flow
1. **Register** ? `POST /api/Auth/register`
2. **Get OTP** from console (development) or email (production)
3. **Verify OTP** ? `POST /api/Auth/verify-otp`
4. **Login** ? `POST /api/Auth/login`
5. Use returned JWT token for authenticated requests

### Example Request
```bash
# Register
curl -X POST https://localhost:7102/api/Auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!",
    "fullName": "John Doe",
    "role": "patient"
  }'

# Login
curl -X POST https://localhost:7102/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!"
  }'

# Get Current User (with token)
curl -X GET https://localhost:7102/api/Auth/current-user \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## ?? Configuration

### JWT Settings (`appsettings.json`)
```json
"Jwt": {
  "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!@#$%",
  "Issuer": "MediAI-Backend",
  "Audience": "MediAI-Users",
  "ExpiryInHours": 24
}
```

?? **Important**: Change the JWT Key to a secure random string before deployment!

### Email Settings (Optional)
```json
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
```

For Gmail: Use App Password, not your regular password. See [Google App Passwords](https://support.google.com/accounts/answer/185833).

## ?? Frontend Integration

### Flutter
See `FLUTTER_INTEGRATION_GUIDE.md` for complete Flutter integration with:
- Complete setup instructions
- Code examples for all screens
- Connection testing guide
- Security best practices

### Base URLs
- **Android Emulator**: `http://10.0.2.2:5000`
- **iOS Simulator**: `http://localhost:5000`
- **Real Device**: `http://YOUR_COMPUTER_IP:5000`

## ?? Testing

### Using Swagger
1. Run the application
2. Navigate to `https://localhost:7102/swagger`
3. Test endpoints directly from browser

### Using Postman
Import the API collection (if provided) or manually create requests using the API documentation.

### Test Authentication Flow
1. Register user ? Get OTP from console
2. Verify OTP ? Get JWT token
3. Copy token ? Click "Authorize" in Swagger
4. Enter: `Bearer YOUR_TOKEN`
5. Test protected endpoints

## ?? Project Structure

```
Backend-APIs/
??? Controllers/
?   ??? AuthController.cs           # Authentication endpoints
?   ??? DoctorsController.cs        # Doctor management
?   ??? AppointmentsController.cs   # Appointment booking
?   ??? MedicineRemindersController.cs # Medicine reminders
??? Models/                          # EF Core entity models
??? DTOs/                            # Data transfer objects
??? Services/
?   ??? AuthService.cs              # Authentication logic
?   ??? EmailService.cs             # Email sending
?   ??? Interfaces/
??? Program.cs                       # Application startup
??? appsettings.json                 # Configuration
??? appsettings.Development.json     # Dev configuration
```

## ?? Security Features

- ? **Password Hashing** - BCrypt with salt
- ? **JWT Authentication** - Secure token-based auth
- ? **Role-Based Authorization** - Patient, Doctor, Admin roles
- ? **HTTPS** - SSL/TLS encryption
- ? **CORS** - Configured for mobile apps
- ? **Input Validation** - Model validation and sanitization
- ? **OTP Verification** - Email verification before account activation

## ?? Deployment

### Azure App Service
1. Create Azure App Service (Windows)
2. Configure MySQL connection string
3. Set JWT Key as environment variable
4. Deploy using Visual Studio or Azure CLI

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

### Environment Variables (Production)
```bash
ConnectionStrings__DefaultConnection="server=prod-server;database=mediaidb;user=produser;password=secure_password"
Jwt__Key="your-production-secret-key-minimum-32-characters"
EmailSettings__Username="production@email.com"
EmailSettings__Password="production-app-password"
```

## ?? Database Schema

The application uses an existing MySQL database with the following main tables:
- **users** - User accounts and profiles
- **doctors** - Doctor-specific information
- **appointments** - Appointment bookings
- **medicinereminders** - Medicine reminder schedules
- **emailverificationotps** - OTP verification codes
- **doctorreviews** - Doctor ratings and reviews
- And more...

See scaffolded models in `Models/` directory for complete schema.

## ?? Troubleshooting

### Port Already in Use
```bash
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

### Database Connection Failed
- Check MySQL is running: `mysql -u root -p`
- Verify connection string
- Check firewall rules

### CORS Errors
- CORS is configured to allow all origins in development
- For production, update `Program.cs` to specify allowed origins

### JWT Token Expired
- Tokens expire after 24 hours by default
- Re-login to get new token
- Implement refresh token mechanism for production

## ?? API Endpoints Summary

### Authentication
- `POST /api/Auth/register` - Register new user
- `POST /api/Auth/verify-otp` - Verify OTP
- `POST /api/Auth/login` - User login
- `GET /api/Auth/current-user` - Get authenticated user
- `GET /api/Auth/health` - Health check

### Doctors
- `GET /api/Doctors` - List all doctors
- `GET /api/Doctors/{id}` - Get doctor details
- `GET /api/Doctors/specialization/{spec}` - Search by specialization
- `GET /api/Doctors/available` - Get available doctors

### Appointments
- `GET /api/Appointments/my-appointments` - User's appointments
- `POST /api/Appointments` - Book appointment
- `PATCH /api/Appointments/{id}/status` - Update status
- `POST /api/Appointments/{id}/cancel` - Cancel appointment
- `GET /api/Appointments/upcoming` - Upcoming appointments

### Medicine Reminders
- `GET /api/MedicineReminders` - List reminders
- `GET /api/MedicineReminders/active` - Active reminders
- `GET /api/MedicineReminders/today` - Today's schedule
- `POST /api/MedicineReminders` - Create reminder
- `PUT /api/MedicineReminders/{id}` - Update reminder
- `DELETE /api/MedicineReminders/{id}` - Delete reminder
- `POST /api/MedicineReminders/{id}/log` - Log intake

## ?? Contributing

1. Fork the repository
2. Create feature branch: `git checkout -b feature/amazing-feature`
3. Commit changes: `git commit -m 'Add amazing feature'`
4. Push to branch: `git push origin feature/amazing-feature`
5. Open Pull Request

## ?? License

[Your License Here]

## ?? Authors

[Your Name/Team]

## ?? Support

For issues and questions:
- Open an issue on GitHub
- Email: [your-email]
- Documentation: See `API_DOCUMENTATION.md` and `FLUTTER_INTEGRATION_GUIDE.md`

## ? Acknowledgments

- Built with .NET 8
- Uses Pomelo MySQL provider
- JWT authentication implementation
- BCrypt password hashing

---

## ?? Next Steps

1. ? Backend is ready and running
2. ?? Configure email service (optional)
3. ?? Connect Flutter frontend
4. ?? Add more features (Reports, Health Tips, etc.)
5. ?? Deploy to production
6. ?? Set up CI/CD pipeline

**Happy Coding! ??**
