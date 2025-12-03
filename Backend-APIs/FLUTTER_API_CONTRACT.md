# ?? Flutter-Backend API Contract

## ? **Backend Updated to Match Flutter Requirements**

All endpoints now return consistent response format and field names that match your Flutter app's expectations.

---

## ?? **Standard Response Format**

All API responses follow this structure:

```json
{
  "success": true/false,
  "message": "Description of what happened",
  "data": { ... } // or null
}
```

---

## ?? **1. REGISTRATION**

### **Endpoint:** `POST /api/Auth/register`

### **Request Body:**
```json
{
  "email": "ahmed@student.buitms.edu.pk",
  "password": "SecurePass123",
  "fullName": "Ahmed Ali Khan",
  "role": "Student",
  "department": "Computer Science",
  "registrationNumber": "59858",
  "phoneNumber": "03001234567",
  "dateOfBirth": "2002-05-15",
  "gender": "Male",
  "address": "House 123, Street 5, Quetta"
}
```

### **Required Fields:**
- ? `email` (string)
- ? `password` (string)
- ? `fullName` (string)
- ? `role` (string) - "Student", "Faculty", "Doctor", "Admin"

### **Optional Fields:**
- `department` (string)
- `registrationNumber` (string)
- `phoneNumber` (string)
- `dateOfBirth` (string) - Format: "yyyy-MM-dd"
- `gender` (string) - "Male" or "Female"
- `address` (string)

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Registration successful! OTP sent to ahmed@student.buitms.edu.pk. Please check your email (or console in development mode).",
  "data": null
}
```

### **Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Email already registered",
  "data": null
}
```

---

## ?? **2. OTP VERIFICATION**

### **Endpoint:** `POST /api/Auth/verify-otp`

### **Request Body:**
```json
{
  "email": "ahmed@student.buitms.edu.pk",
  "otp": "123456"
}
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Email verified successfully! Welcome to MediAI Healthcare.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "userId": 1,
      "email": "ahmed@student.buitms.edu.pk",
      "fullName": "Ahmed Ali Khan",
      "role": "Student",
      "department": "Computer Science",
      "registrationNumber": "59858",
      "phoneNumber": "03001234567",
      "dateOfBirth": "2002-05-15",
      "gender": "Male",
      "address": "House 123, Street 5, Quetta",
      "profileImageUrl": null,
      "isEmailVerified": true,
      "isActive": true
    }
  }
}
```

### **Error Responses:**

**Invalid OTP (400):**
```json
{
  "success": false,
  "message": "Invalid OTP",
  "data": null
}
```

**Expired OTP (400):**
```json
{
  "success": false,
  "message": "OTP expired",
  "data": null
}
```

**User Not Found (400):**
```json
{
  "success": false,
  "message": "User not found",
  "data": null
}
```

---

## ?? **3. LOGIN**

### **Endpoint:** `POST /api/Auth/login`

### **Request Body:**
```json
{
  "email": "ahmed@student.buitms.edu.pk",
  "password": "SecurePass123"
}
```

### **Required Fields:**
- ? `email` (string)
- ? `password` (string)

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "userId": 1,
      "email": "ahmed@student.buitms.edu.pk",
      "fullName": "Ahmed Ali Khan",
      "role": "Student",
      "department": "Computer Science",
      "registrationNumber": "59858",
      "phoneNumber": "03001234567",
      "dateOfBirth": "2002-05-15",
      "gender": "Male",
      "address": "House 123, Street 5, Quetta",
      "profileImageUrl": null,
      "isEmailVerified": true,
      "isActive": true
    }
  }
}
```

### **Error Responses:**

**Invalid Credentials (400):**
```json
{
  "success": false,
  "message": "Invalid email or password",
  "data": null
}
```

**Email Not Verified (400):**
```json
{
  "success": false,
  "message": "Please verify your email first",
  "data": null
}
```

**Account Deactivated (400):**
```json
{
  "success": false,
  "message": "Account is deactivated",
  "data": null
}
```

---

## ?? **4. GET CURRENT USER**

### **Endpoint:** `GET /api/Auth/current-user`
**Authentication Required:** ? Yes (Bearer Token)

### **Headers:**
```
Authorization: Bearer {your_jwt_token}
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "User retrieved successfully",
  "data": {
    "userId": 1,
    "email": "ahmed@student.buitms.edu.pk",
    "fullName": "Ahmed Ali Khan",
    "role": "Student",
    "department": "Computer Science",
    "registrationNumber": "59858",
    "phoneNumber": "03001234567",
    "dateOfBirth": "2002-05-15",
    "gender": "Male",
    "address": "House 123, Street 5, Quetta",
    "profileImageUrl": null,
    "isEmailVerified": true,
    "isActive": true
  }
}
```

### **Error Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Invalid token",
  "data": null
}
```

---

## ?? **5. HEALTH CHECK**

### **Endpoint:** `GET /api/Auth/health`
**Authentication Required:** ? No

### **Success Response (200 OK):**
```json
{
  "status": "healthy",
  "timestamp": "2024-12-18T10:30:00Z",
  "message": "MediAI Backend API is running"
}
```

---

## ?? **Field Mapping Reference**

### **User Object Fields:**

| Flutter Field Name | Type | Description | Optional |
|-------------------|------|-------------|----------|
| `userId` | int | User's unique ID | ? |
| `email` | string | User's email address | ? |
| `fullName` | string | User's full name | ? |
| `role` | string | User role (Student/Faculty/Doctor/Admin) | ? |
| `department` | string | User's department | ? |
| `registrationNumber` | string | CMS ID or registration number | ? |
| `phoneNumber` | string | Contact phone number | ? |
| `dateOfBirth` | string | Birth date (yyyy-MM-dd format) | ? |
| `gender` | string | Male/Female | ? |
| `address` | string | User's address | ? |
| `profileImageUrl` | string | Profile image URL | ? |
| `isEmailVerified` | bool | Email verification status | ? |
| `isActive` | bool | Account active status | ? |

---

## ?? **Authentication Flow**

```
1. User Registers
   ?
   POST /api/Auth/register
   ?
2. Backend sends OTP (check console in dev mode)
   ?
3. User enters OTP
   ?
   POST /api/Auth/verify-otp
   ?
4. Backend returns JWT token + user data
   ?
5. Save token in Flutter secure storage
   ?
6. Use token for authenticated requests
```

---

## ?? **Flutter Integration Example**

### **Register Request:**
```dart
final response = await http.post(
  Uri.parse('$baseUrl/api/Auth/register'),
  headers: {'Content-Type': 'application/json'},
  body: jsonEncode({
    'email': 'ahmed@student.buitms.edu.pk',
    'password': 'SecurePass123',
    'fullName': 'Ahmed Ali Khan',
    'role': 'Student',
    'department': 'Computer Science',
    'registrationNumber': '59858',
    'phoneNumber': '03001234567',
    'dateOfBirth': '2002-05-15',
    'gender': 'Male',
    'address': 'House 123, Street 5, Quetta'
  }),
);

final data = jsonDecode(response.body);
if (data['success']) {
  print(data['message']); // Show success message
}
```

### **Login Request:**
```dart
final response = await http.post(
  Uri.parse('$baseUrl/api/Auth/login'),
  headers: {'Content-Type': 'application/json'},
  body: jsonEncode({
    'email': 'ahmed@student.buitms.edu.pk',
    'password': 'SecurePass123'
  }),
);

final data = jsonDecode(response.body);
if (data['success']) {
  final token = data['data']['token'];
  final user = data['data']['user'];
  
  // Save token
  await storage.write(key: 'token', value: token);
  
  // Access user data
  print(user['userId']);
  print(user['fullName']);
  print(user['role']);
}
```

---

## ?? **Important Notes**

### **Date Format:**
- Flutter sends: `"yyyy-MM-dd"` (e.g., "2002-05-15")
- Backend expects: `DateOnly` type
- Backend returns: `"yyyy-MM-dd"` string format

### **Null Values:**
- Optional fields return `null` if not provided
- Check for null in Flutter: `user['department'] ?? 'Not specified'`

### **Boolean Values:**
- `isEmailVerified` defaults to `false` for new users
- `isActive` defaults to `true` for new users

### **Token Storage:**
- Store JWT token securely using `flutter_secure_storage`
- Token expires after 24 hours (configurable in backend)
- Include token in headers: `Authorization: Bearer {token}`

---

## ? **Testing Checklist**

- [ ] Registration with all fields
- [ ] Registration with only required fields
- [ ] OTP verification
- [ ] Login with correct credentials
- [ ] Login with wrong password
- [ ] Login before email verification
- [ ] Get current user with valid token
- [ ] Get current user with expired token

---

## ?? **Common Issues & Solutions**

### **Issue: "Invalid request data"**
**Solution:** Check field names match exactly (case-sensitive)

### **Issue: "Email already registered"**
**Solution:** Use a different email or login instead

### **Issue: "Invalid OTP"**
**Solution:** Check console for OTP in development mode

### **Issue: "OTP expired"**
**Solution:** Register again to get new OTP (valid for 10 minutes)

### **Issue: "Invalid token"**
**Solution:** Re-login to get new JWT token

---

## ?? **Response Status Codes**

| Code | Meaning | When |
|------|---------|------|
| 200 | OK | Success |
| 400 | Bad Request | Invalid data or business logic error |
| 401 | Unauthorized | Invalid or missing token |
| 404 | Not Found | Resource not found |
| 500 | Server Error | Backend error |

---

## ?? **Backend is Ready!**

All endpoints are tested and match your Flutter app's requirements exactly. Start testing with the Flutter app!

**Base URL:** `http://10.0.2.2:5000` (Android Emulator)

For any issues, check the response message in the `message` field for detailed error information.
