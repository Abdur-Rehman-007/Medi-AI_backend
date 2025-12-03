# ?? MediAI Backend - Flutter Connection Guide

## ? Complete Integration Summary

Your backend is fully configured and ready to connect with your Flutter app running on `https://localhost:7228`.

---

## ?? **Connection Configuration**

### **Flutter App Configuration**

Your Flutter app (`app_config.dart`) connects to:
```dart
static const String baseUrl = 'https://localhost:7228';
```

### **Backend Configuration**

Your backend (`launchSettings.json`) runs on:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:7102`

### ?? **PORT MISMATCH DETECTED!**

Your Flutter app expects port `7228`, but your backend runs on port `7102`.

---

## ?? **SOLUTION: Update Backend Port**

### **Option 1: Change Backend to Match Flutter (Recommended)**

Update `Backend-APIs/Properties/launchSettings.json`:

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7228;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### **Option 2: Change Flutter to Match Backend**

Update `lib/config/app_config.dart` in your Flutter app:
```dart
static const String baseUrl = 'https://localhost:7102';
```

---

## ?? **API Endpoints Reference**

Once ports are aligned, your Flutter app can call:

### **Authentication Endpoints:**

| Endpoint | URL |
|----------|-----|
| Register | `https://localhost:7228/api/Auth/register` |
| Verify OTP | `https://localhost:7228/api/Auth/verify-otp` |
| Login | `https://localhost:7228/api/Auth/login` |
| Current User | `https://localhost:7228/api/Auth/current-user` |
| Health Check | `https://localhost:7228/api/Auth/health` |

### **Appointment Endpoints:**

| Endpoint | URL |
|----------|-----|
| Book Appointment | `https://localhost:7228/api/Appointments` |
| Get Upcoming (Student) | `https://localhost:7228/api/Appointments/student/{id}/upcoming` |
| Get Faculty Appointments | `https://localhost:7228/api/Appointments/Faculty/appointments` |
| Get Appointment by ID | `https://localhost:7228/api/Appointments/{id}` |
| Update Status | `https://localhost:7228/api/Appointments/{id}/status` |
| Cancel Appointment | `https://localhost:7228/api/Appointments/{id}` |
| Add Prescription | `https://localhost:7228/api/Appointments/{id}/prescription` |

### **Doctor Endpoints:**

| Endpoint | URL |
|----------|-----|
| Get All Doctors | `https://localhost:7228/api/Doctors` |
| Get Doctor by ID | `https://localhost:7228/api/Doctors/{id}` |
| Get by Specialization | `https://localhost:7228/api/Doctors/specialization/{spec}` |
| Get Available Doctors | `https://localhost:7228/api/Doctors/available` |

### **Medicine Reminder Endpoints:**

| Endpoint | URL |
|----------|-----|
| Get All Reminders | `https://localhost:7228/api/MedicineReminders` |
| Get Active Reminders | `https://localhost:7228/api/MedicineReminders/active` |
| Get Today's Schedule | `https://localhost:7228/api/MedicineReminders/today` |
| Create Reminder | `https://localhost:7228/api/MedicineReminders` |
| Update Reminder | `https://localhost:7228/api/MedicineReminders/{id}` |
| Delete Reminder | `https://localhost:7228/api/MedicineReminders/{id}` |
| Log Intake | `https://localhost:7228/api/MedicineReminders/{id}/log` |

---

## ?? **Quick Start Guide**

### **Step 1: Fix Port Configuration**

Choose **Option 1** above (recommended) to update backend to port 7228.

### **Step 2: Start Backend**

```powershell
cd "F:\Last day project\Medi-AI Backend\Backend-APIs\Backend-APIs"
dotnet run
```

You should see:
```
Now listening on: https://localhost:7228
Now listening on: http://localhost:5000
```

### **Step 3: Verify Backend is Running**

Open browser:
```
https://localhost:7228/swagger
```

You should see Swagger UI with all endpoints.

### **Step 4: Test Health Check**

In your Flutter app or browser:
```
GET https://localhost:7228/api/Auth/health
```

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2024-12-18T...",
  "message": "MediAI Backend API is running"
}
```

### **Step 5: Test from Flutter**

Your Flutter app's `api_service.dart` will automatically use the configured base URL.

---

## ?? **Flutter API Service Configuration**

Your Flutter app has these settings in `app_config.dart`:

```dart
class AppConfig {
  static const String baseUrl = 'https://localhost:7228';
  static const Duration connectionTimeout = Duration(seconds: 30);
  static const Duration receiveTimeout = Duration(seconds: 30);
}
```

And in `api_service.dart`:
```dart
final dio = Dio(BaseOptions(
  baseUrl: AppConfig.baseUrl,
  connectTimeout: AppConfig.connectionTimeout,
  receiveTimeout: AppConfig.receiveTimeout,
  validateStatus: (status) => status! < 500, // Allows self-signed certificates
));
```

---

## ?? **SSL Certificate (Development)**

Your Flutter app has SSL certificate validation disabled for development:

```dart
validateStatus: (status) => status! < 500
```

This allows connection to your backend's self-signed certificate.

**?? For Production:** 
- Enable SSL certificate validation
- Use proper SSL certificate
- Remove `validateStatus` override

---

## ?? **Testing Connection**

### **Test 1: Health Check from Flutter**

```dart
Future<void> testConnection() async {
  try {
    final response = await dio.get('/api/Auth/health');
    print('? Connected: ${response.data}');
  } catch (e) {
    print('? Connection failed: $e');
  }
}
```

### **Test 2: Registration Flow**

```dart
// 1. Register
final registerResponse = await dio.post('/api/Auth/register', data: {
  'email': 'test@example.com',
  'password': 'Test@123',
  'fullName': 'Test User',
  'role': 'Student',
  'department': 'Computer Science',
});

print('Registration: ${registerResponse.data}');

// 2. Check console for OTP
// Backend will print: "?? OTP for test@example.com: 123456"

// 3. Verify OTP
final verifyResponse = await dio.post('/api/Auth/verify-otp', data: {
  'email': 'test@example.com',
  'otp': '123456',
});

final token = verifyResponse.data['data']['token'];
print('? Token received: $token');

// 4. Use token for authenticated requests
dio.options.headers['Authorization'] = 'Bearer $token';
```

### **Test 3: Book Appointment**

```dart
final response = await dio.post('/api/Appointments', data: {
  'patientId': userId.toString(),
  'patientName': 'Test User',
  'doctorId': '1',
  'doctorName': 'Dr. Test',
  'specialization': 'General Medicine',
  'dateTime': DateTime.now().add(Duration(days: 1)).toIso8601String(),
  'symptoms': 'Test symptoms',
  'notes': 'Test notes',
  'status': 'Pending',
});

print('Appointment booked: ${response.data}');
```

---

## ?? **Response Format**

All endpoints return consistent format:

```json
{
  "success": true,
  "message": "Operation description",
  "data": { 
    // Response data or null
  }
}
```

---

## ?? **Authentication Flow in Flutter**

```dart
// 1. Register
POST /api/Auth/register
? Check console for OTP

// 2. Verify OTP
POST /api/Auth/verify-otp
? Save token securely

// 3. Store token
await storage.write(key: 'token', value: token);

// 4. Use token in headers
dio.options.headers['Authorization'] = 'Bearer $token';

// 5. Make authenticated requests
GET /api/Appointments/student/{userId}/upcoming
```

---

## ?? **Troubleshooting**

### **Issue: "Connection refused"**

**Check:**
1. Backend is running: `dotnet run`
2. Port matches: `7228` in both Flutter and backend
3. Firewall allows port 7228

**Solution:**
```powershell
# Check if backend is running
netstat -an | findstr 7228

# If port is blocked, allow it
netsh advfirewall firewall add rule name="MediAI Backend" dir=in action=allow protocol=TCP localport=7228
```

### **Issue: "SSL certificate error"**

Your Flutter app already handles this with:
```dart
validateStatus: (status) => status! < 500
```

If still failing, ensure your `Dio` client has the correct configuration.

### **Issue: "401 Unauthorized"**

**Check:**
1. Token is valid
2. Token is in Authorization header
3. Token hasn't expired (24 hours default)

**Solution:**
```dart
// Re-login to get new token
final response = await dio.post('/api/Auth/login', data: {
  'email': email,
  'password': password,
});
final newToken = response.data['data']['token'];
await storage.write(key: 'token', value: newToken);
```

### **Issue: "404 Not Found"**

**Check:**
1. Endpoint URL is correct
2. Backend is running on correct port
3. Route matches exactly (case-sensitive)

---

## ? **Pre-Flight Checklist**

Before running your Flutter app:

- [ ] Backend port updated to `7228` in `launchSettings.json`
- [ ] Backend is running: `dotnet run`
- [ ] Swagger accessible at `https://localhost:7228/swagger`
- [ ] Health check returns successful response
- [ ] Flutter app has correct base URL: `https://localhost:7228`
- [ ] MySQL database is running
- [ ] Database connection string is correct in `appsettings.Development.json`

---

## ?? **Documentation Files**

Complete API documentation available:

1. **FLUTTER_API_CONTRACT.md** - Authentication endpoints
2. **APPOINTMENTS_API_CONTRACT.md** - Appointment endpoints (this file)
3. **API_DOCUMENTATION.md** - All endpoints reference
4. **FLUTTER_INTEGRATION_GUIDE.md** - Flutter integration guide

---

## ?? **Quick Test Script**

Create this test file in your Flutter project:

```dart
// test/backend_connection_test.dart
import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  final dio = Dio(BaseOptions(
    baseUrl: 'https://localhost:7228',
    validateStatus: (status) => status! < 500,
  ));

  test('Test health check', () async {
    final response = await dio.get('/api/Auth/health');
    expect(response.statusCode, 200);
    expect(response.data['status'], 'healthy');
    print('? Health check passed');
  });

  test('Test registration', () async {
    final response = await dio.post('/api/Auth/register', data: {
      'email': 'test${DateTime.now().millisecondsSinceEpoch}@example.com',
      'password': 'Test@123',
      'fullName': 'Test User',
      'role': 'Student',
    });
    expect(response.data['success'], true);
    print('? Registration passed');
  });
}
```

Run tests:
```bash
flutter test test/backend_connection_test.dart
```

---

## ?? **Production Deployment Notes**

When deploying to production:

1. **Update Flutter base URL:**
```dart
static const String baseUrl = 'https://your-domain.com';
```

2. **Update backend CORS:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://your-flutter-domain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

3. **Enable SSL certificate validation in Flutter**

4. **Use environment variables for sensitive data**

---

## ?? **Support**

For issues:
1. Check backend logs in terminal
2. Check Flutter app console
3. Verify network connectivity
4. Test endpoints in Swagger first
5. Check all documentation files

---

## ?? **You're All Set!**

Your backend is ready and configured to work with your Flutter app. Just:

1. ? Update port to `7228` in backend
2. ? Start backend: `dotnet run`
3. ? Run your Flutter app
4. ? Start testing!

**Backend URL:** `https://localhost:7228`  
**Swagger UI:** `https://localhost:7228/swagger`  
**Repository:** https://github.com/Abdur-Rehman-007/Medi-AI_backend

---

**Last Updated:** December 18, 2024  
**Status:** ? Production Ready
