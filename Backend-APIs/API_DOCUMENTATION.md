# MediAI Backend API Documentation

## Base URL
- **Development**: `http://localhost:5000` or `https://localhost:7102`
- **Production**: `https://your-domain.com`

## Authentication
All protected endpoints require JWT Bearer token in the Authorization header:
```
Authorization: Bearer YOUR_JWT_TOKEN
```

---

## 📋 Table of Contents
1. [Authentication API](#authentication-api)
2. [Doctors API](#doctors-api)
3. [Appointments API](#appointments-api)
4. [Medicine Reminders API](#medicine-reminders-api)
5. [Health Check API](#health-check-api)

---

## 🔐 Authentication API

### 1. Register User
**Endpoint:** `POST /api/Auth/register`  
**Authentication:** Not required

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "fullName": "John Doe",
  "role": "patient",
  "phoneNumber": "1234567890",
  "dateOfBirth": "1990-01-15",
  "gender": "male",
  "address": "123 Main St, City"
}
```

**Response (200 OK):**
```json
{
  "message": "Registration successful! OTP sent to user@example.com. Please check your email (or console in development mode)."
}
```

**Response (400 Bad Request):**
```json
{
  "message": "Email already registered"
}
```

---

### 2. Verify OTP
**Endpoint:** `POST /api/Auth/verify-otp`  
**Authentication:** Not required

**Request Body:**
```json
{
  "email": "user@example.com",
  "otp": "123456"
}
```

**Response (200 OK):**
```json
{
  "message": "Email verified successfully! Welcome to MediAI Healthcare.",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "email": "user@example.com",
    "fullName": "John Doe",
    "role": "patient",
    "phoneNumber": "1234567890",
    "isEmailVerified": true,
    "isActive": true,
    "createdAt": "2024-01-15T10:00:00Z"
  }
}
```

**Response (400 Bad Request):**
```json
{
  "message": "Invalid OTP"
}
```

---

### 3. Login
**Endpoint:** `POST /api/Auth/login`  
**Authentication:** Not required

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response (200 OK):**
```json
{
  "message": "Login successful",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "email": "user@example.com",
    "fullName": "John Doe",
    "role": "patient",
    "phoneNumber": "1234567890",
    "isEmailVerified": true,
    "isActive": true,
    "lastLoginAt": "2024-01-15T15:30:00Z"
  }
}
```

**Response (400 Bad Request):**
```json
{
  "message": "Invalid email or password"
}
```

---

### 4. Get Current User
**Endpoint:** `GET /api/Auth/current-user`  
**Authentication:** Required (Bearer Token)

**Response (200 OK):**
```json
{
  "id": 1,
  "email": "user@example.com",
  "fullName": "John Doe",
  "role": "patient",
  "phoneNumber": "1234567890",
  "dateOfBirth": "1990-01-15",
  "gender": "male",
  "address": "123 Main St, City",
  "isEmailVerified": true,
  "isActive": true,
  "createdAt": "2024-01-15T10:00:00Z",
  "lastLoginAt": "2024-01-15T15:30:00Z"
}
```

---

### 5. Health Check
**Endpoint:** `GET /api/Auth/health`  
**Authentication:** Not required

**Response (200 OK):**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T15:30:00Z",
  "message": "MediAI Backend API is running"
}
```

---

## 👨‍⚕️ Doctors API

### 1. Get All Doctors
**Endpoint:** `GET /api/Doctors`  
**Authentication:** Not required

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "userId": 5,
    "specialization": "Cardiology",
    "licenseNumber": "MD12345",
    "qualification": "MBBS, MD",
    "experience": 10,
    "consultationFee": 500.00,
    "roomNumber": "Room 101",
    "bio": "Experienced cardiologist...",
    "averageRating": 4.5,
    "totalRatings": 120,
    "isAvailable": true,
    "createdAt": "2024-01-01T00:00:00Z",
    "user": {
      "id": 5,
      "fullName": "Dr. Sarah Smith",
      "email": "sarah@hospital.com",
      "phoneNumber": "9876543210",
      "profileImageUrl": null
    }
  }
]
```

---

### 2. Get Doctor by ID
**Endpoint:** `GET /api/Doctors/{id}`  
**Authentication:** Not required

**Response (200 OK):**
```json
{
  "id": 1,
  "userId": 5,
  "specialization": "Cardiology",
  "licenseNumber": "MD12345",
  "qualification": "MBBS, MD",
  "experience": 10,
  "consultationFee": 500.00,
  "roomNumber": "Room 101",
  "bio": "Experienced cardiologist...",
  "averageRating": 4.5,
  "totalRatings": 120,
  "isAvailable": true,
  "user": {
    "id": 5,
    "fullName": "Dr. Sarah Smith",
    "email": "sarah@hospital.com",
    "phoneNumber": "9876543210",
    "gender": "female",
    "department": "Cardiology"
  },
  "schedules": [
    {
      "id": 1,
      "dayOfWeek": "Monday",
      "startTime": "09:00:00",
      "endTime": "17:00:00",
      "isActive": true
    }
  ],
  "reviews": [
    {
      "id": 1,
      "rating": 5,
      "review": "Excellent doctor!",
      "createdAt": "2024-01-10T10:00:00Z",
      "patientName": "John Doe"
    }
  ]
}
```

---

### 3. Get Doctors by Specialization
**Endpoint:** `GET /api/Doctors/specialization/{specialization}`  
**Authentication:** Not required

**Example:** `GET /api/Doctors/specialization/cardiology`

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "specialization": "Cardiology",
    "qualification": "MBBS, MD",
    "experience": 10,
    "consultationFee": 500.00,
    "averageRating": 4.5,
    "totalRatings": 120,
    "isAvailable": true,
    "user": {
      "fullName": "Dr. Sarah Smith",
      "profileImageUrl": null
    }
  }
]
```

---

### 4. Get Available Doctors
**Endpoint:** `GET /api/Doctors/available`  
**Authentication:** Not required

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "specialization": "Cardiology",
    "consultationFee": 500.00,
    "averageRating": 4.5,
    "experience": 10,
    "user": {
      "fullName": "Dr. Sarah Smith",
      "profileImageUrl": null
    }
  }
]
```

---

### 5. Update Doctor Availability
**Endpoint:** `PATCH /api/Doctors/{id}/availability`  
**Authentication:** Required (Doctor or Admin role)

**Request Body:**
```json
true
```

**Response (200 OK):**
```json
{
  "message": "Availability updated successfully",
  "isAvailable": true
}
```

---

## 📅 Appointments API

### 1. Get My Appointments
**Endpoint:** `GET /api/Appointments/my-appointments`  
**Authentication:** Required

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "appointmentDate": "2024-01-20",
    "appointmentTime": "10:00:00",
    "duration": 30,
    "status": "scheduled",
    "symptoms": "Chest pain",
    "notes": "Follow-up required",
    "createdAt": "2024-01-15T10:00:00Z",
    "patient": {
      "id": 1,
      "fullName": "John Doe",
      "phoneNumber": "1234567890"
    },
    "doctor": {
      "id": 1,
      "specialization": "Cardiology",
      "consultationFee": 500.00,
      "roomNumber": "Room 101",
      "doctorName": "Dr. Sarah Smith"
    }
  }
]
```

---

### 2. Get Appointment by ID
**Endpoint:** `GET /api/Appointments/{id}`  
**Authentication:** Required

**Response (200 OK):**
```json
{
  "id": 1,
  "appointmentDate": "2024-01-20",
  "appointmentTime": "10:00:00",
  "duration": 30,
  "status": "scheduled",
  "symptoms": "Chest pain",
  "notes": "Follow-up required",
  "createdAt": "2024-01-15T10:00:00Z",
  "updatedAt": "2024-01-15T10:00:00Z",
  "patient": {
    "id": 1,
    "fullName": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "1234567890",
    "dateOfBirth": "1990-01-15",
    "gender": "male"
  },
  "doctor": {
    "id": 1,
    "specialization": "Cardiology",
    "consultationFee": 500.00,
    "roomNumber": "Room 101",
    "doctorName": "Dr. Sarah Smith",
    "doctorPhone": "9876543210"
  },
  "prescriptions": []
}
```

---

### 3. Book Appointment
**Endpoint:** `POST /api/Appointments`  
**Authentication:** Required (Patient role)

**Request Body:**
```json
{
  "doctorId": 1,
  "appointmentDate": "2024-01-20",
  "appointmentTime": "10:00:00",
  "duration": 30,
  "symptoms": "Chest pain, shortness of breath"
}
```

**Response (201 Created):**
```json
{
  "message": "Appointment booked successfully",
  "appointmentId": 1
}
```

**Response (400 Bad Request):**
```json
{
  "message": "This time slot is already booked"
}
```

---

### 4. Update Appointment Status
**Endpoint:** `PATCH /api/Appointments/{id}/status`  
**Authentication:** Required

**Request Body:**
```json
{
  "status": "confirmed"
}
```

**Valid statuses:** `scheduled`, `confirmed`, `completed`, `cancelled`, `no-show`

**Response (200 OK):**
```json
{
  "message": "Appointment status updated successfully",
  "status": "confirmed"
}
```

---

### 5. Cancel Appointment
**Endpoint:** `POST /api/Appointments/{id}/cancel`  
**Authentication:** Required

**Request Body:**
```json
{
  "reason": "Unable to attend due to emergency"
}
```

**Response (200 OK):**
```json
{
  "message": "Appointment cancelled successfully"
}
```

---

### 6. Get Upcoming Appointments
**Endpoint:** `GET /api/Appointments/upcoming`  
**Authentication:** Required

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "appointmentDate": "2024-01-20",
    "appointmentTime": "10:00:00",
    "status": "scheduled",
    "symptoms": "Chest pain",
    "patient": {
      "fullName": "John Doe",
      "phoneNumber": "1234567890"
    },
    "doctor": {
      "specialization": "Cardiology",
      "doctorName": "Dr. Sarah Smith",
      "roomNumber": "Room 101"
    }
  }
]
```

---

## 💊 Medicine Reminders API

### 1. Get My Reminders
**Endpoint:** `GET /api/MedicineReminders`  
**Authentication:** Required

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "medicineName": "Aspirin",
    "dosage": "100mg",
    "frequency": "daily",
    "customFrequency": null,
    "times": "09:00,21:00",
    "startDate": "2024-01-15",
    "endDate": "2024-02-15",
    "notes": "Take with food",
    "isActive": true,
    "createdAt": "2024-01-15T10:00:00Z"
  }
]
```

---

### 2. Get Active Reminders
**Endpoint:** `GET /api/MedicineReminders/active`  
**Authentication:** Required

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "medicineName": "Aspirin",
    "dosage": "100mg",
    "frequency": "daily",
    "times": "09:00,21:00",
    "startDate": "2024-01-15",
    "endDate": "2024-02-15",
    "notes": "Take with food"
  }
]
```

---

### 3. Get Today's Medicine Schedule
**Endpoint:** `GET /api/MedicineReminders/today`  
**Authentication:** Required

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "medicineName": "Aspirin",
    "dosage": "100mg",
    "times": "09:00,21:00",
    "notes": "Take with food"
  }
]
```

---

### 4. Create Medicine Reminder
**Endpoint:** `POST /api/MedicineReminders`  
**Authentication:** Required

**Request Body:**
```json
{
  "medicineName": "Aspirin",
  "dosage": "100mg",
  "frequency": "daily",
  "customFrequency": null,
  "times": "09:00,21:00",
  "startDate": "2024-01-15",
  "endDate": "2024-02-15",
  "notes": "Take with food"
}
```

**Response (201 Created):**
```json
{
  "message": "Medicine reminder created successfully",
  "reminderId": 1
}
```

---

### 5. Update Medicine Reminder
**Endpoint:** `PUT /api/MedicineReminders/{id}`  
**Authentication:** Required

**Request Body:**
```json
{
  "medicineName": "Aspirin",
  "dosage": "150mg",
  "frequency": "daily",
  "customFrequency": null,
  "times": "08:00,20:00",
  "startDate": "2024-01-15",
  "endDate": "2024-02-15",
  "notes": "Take with food and water"
}
```

**Response (200 OK):**
```json
{
  "message": "Reminder updated successfully"
}
```

---

### 6. Toggle Reminder Status
**Endpoint:** `PATCH /api/MedicineReminders/{id}/toggle`  
**Authentication:** Required

**Response (200 OK):**
```json
{
  "message": "Reminder activated",
  "isActive": true
}
```

---

### 7. Delete Reminder
**Endpoint:** `DELETE /api/MedicineReminders/{id}`  
**Authentication:** Required

**Response (200 OK):**
```json
{
  "message": "Reminder deleted successfully"
}
```

---

### 8. Log Medicine Intake
**Endpoint:** `POST /api/MedicineReminders/{id}/log`  
**Authentication:** Required

**Request Body:**
```json
{
  "scheduledTime": "2024-01-15T09:00:00Z",
  "status": "taken",
  "notes": "Taken on time"
}
```

**Response (200 OK):**
```json
{
  "message": "Intake logged successfully"
}
```

---

## 🏥 Health Check API

### Check API Status
**Endpoint:** `GET /api/Auth/health`  
**Authentication:** Not required

**Response (200 OK):**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-15T15:30:00Z",
  "message": "MediAI Backend API is running"
}
```

---

## 📝 Common Response Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request |
| 401 | Unauthorized (Token missing or invalid) |
| 403 | Forbidden (Insufficient permissions) |
| 404 | Not Found |
| 500 | Internal Server Error |

---

## 🔒 User Roles

| Role | Description |
|------|-------------|
| `patient` | Regular user who can book appointments and manage reminders |
| `doctor` | Healthcare provider who can manage their availability and appointments |
| `admin` | System administrator with full access |

---

## ⚠️ Important Notes

1. **Date Formats:**
   - Date: `YYYY-MM-DD` (e.g., `2024-01-15`)
   - Time: `HH:mm:ss` (e.g., `09:00:00`)
   - DateTime: ISO 8601 format (e.g., `2024-01-15T10:00:00Z`)

2. **Token Expiry:**
   - JWT tokens expire after 24 hours
   - Store tokens securely in Flutter using `flutter_secure_storage`

3. **CORS:**
   - API allows all origins in development
   - Configure specific origins for production

4. **Rate Limiting:**
   - Consider implementing rate limiting for production

---

## 📱 Ready for Flutter Integration!

All endpoints are tested and ready to be consumed by your Flutter frontend. See `FLUTTER_INTEGRATION_GUIDE.md` for implementation examples.
