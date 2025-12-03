# ?? MediAI Backend - Missing Endpoints Implementation Plan

## ?? **Current Status**

| Category | Implemented | Total Required | Missing | Progress |
|----------|-------------|----------------|---------|----------|
| **Authentication** | 5 | 8 | 3 | 62% |
| **Doctors** | 6 | 19 | 13 | 31% |
| **Appointments** | 7 | 11 | 4 | 63% |
| **Medicine Reminders** | 9 | 12 | 3 | 75% |
| **Users** | 0 | 7 | 7 | 0% |
| **Health Tips** | 0 | 6 | 6 | 0% |
| **Notifications** | 0 | 6 | 6 | 0% |
| **Symptom Checker** | 0 | 2 | 2 | 0% |
| **Reports** | 0 | 3 | 3 | 0% |
| **TOTAL** | **27** | **74** | **47** | **36%** |

---

## ?? **Priority Levels**

### **Priority 1 - CRITICAL (Flutter App Needs These Now)**
Required for basic app functionality
- [ ] Forgot Password Flow
- [ ] Resend OTP
- [ ] User Profile Management
- [ ] Doctor Search
- [ ] Doctor Available Slots

### **Priority 2 - HIGH (Essential Features)**
Important for complete user experience
- [ ] Reset Password
- [ ] Change Password
- [ ] Upload Profile Photo
- [ ] Doctor Schedules Management
- [ ] Appointment Rescheduling
- [ ] Medicine Reminder Logs
- [ ] Notifications System

### **Priority 3 - MEDIUM (Enhanced Features)**
Nice to have for better UX
- [ ] Doctor Specializations List
- [ ] Doctor Statistics
- [ ] Medicine Compliance Stats
- [ ] User Medical History
- [ ] User Prescriptions List

### **Priority 4 - LOW (Admin/Future Features)**
Can be implemented later
- [ ] Health Tips CRUD
- [ ] Symptom Checker
- [ ] Admin Reports
- [ ] Doctor CRUD (Admin)

---

## ?? **Implementation Roadmap**

### **Phase 1: Authentication Completion** (1-2 hours)
Essential auth features needed now

- [ ] POST `/api/Auth/forgot-password` - Request password reset
- [ ] POST `/api/Auth/reset-password` - Reset password with token
- [ ] POST `/api/Auth/resend-otp` - Resend OTP code

**Models Needed:**
- `ForgotPasswordDto`
- `ResetPasswordDto`
- `ResendOtpDto`

**Database Tables:**
- `passwordresettokens` (already exists)

---

### **Phase 2: User Profile Management** (2-3 hours)
User management features

- [ ] GET `/api/Users/profile` - Get user profile
- [ ] PUT `/api/Users/profile` - Update profile
- [ ] POST `/api/Users/upload-photo` - Upload profile photo
- [ ] POST `/api/Users/change-password` - Change password

**Models Needed:**
- `UpdateProfileDto`
- `ChangePasswordDto`

**Storage:**
- File storage for profile photos (Azure Blob/Local)

---

### **Phase 3: Doctor Search & Availability** (3-4 hours)
Critical for appointment booking

- [ ] GET `/api/Doctors/search` - Advanced search
- [ ] GET `/api/Doctors/specializations` - List specializations
- [ ] GET `/api/Doctors/{id}/available-slots` - Get available time slots
- [ ] GET `/api/Doctors/{id}/schedule` - Get doctor schedule

**Features:**
- Search by name, specialization, availability
- Calculate available slots based on schedule
- Exclude booked appointments

---

### **Phase 4: Appointment Enhancements** (2-3 hours)
Better appointment management

- [ ] PUT `/api/Appointments/{id}/reschedule` - Reschedule appointment
- [ ] GET `/api/Appointments/history` - Past appointments

**Features:**
- Conflict detection for rescheduling
- Appointment history with filters

---

### **Phase 5: Doctor Schedule Management** (3-4 hours)
Doctors manage their availability

- [ ] POST `/api/Doctors/{id}/schedule` - Create schedule
- [ ] PUT `/api/Doctors/{id}/schedule/{scheduleId}` - Update schedule
- [ ] DELETE `/api/Doctors/{id}/schedule/{scheduleId}` - Delete schedule
- [ ] PUT `/api/Doctors/{id}/status` - Update status

**Features:**
- Weekly schedule management
- Recurring availability
- Holiday/leave management

---

### **Phase 6: Medicine Reminder Enhancements** (2 hours)
Better medication tracking

- [ ] GET `/api/MedicineReminders/{id}/logs` - Get intake logs
- [ ] GET `/api/MedicineReminders/statistics` - Compliance stats

**Features:**
- Detailed intake history
- Compliance percentage
- Missed doses tracking

---

### **Phase 7: Notifications System** (4-5 hours)
Real-time notifications

- [ ] GET `/api/Notifications` - Get user notifications
- [ ] GET `/api/Notifications/unread-count` - Unread count
- [ ] PUT `/api/Notifications/{id}/read` - Mark as read
- [ ] PUT `/api/Notifications/mark-all-read` - Mark all as read
- [ ] DELETE `/api/Notifications/{id}` - Delete notification
- [ ] POST `/api/Notifications/register-device` - FCM token

**Features:**
- Push notification support (Firebase)
- In-app notifications
- Notification preferences

---

### **Phase 8: Health Tips** (2-3 hours)
Health education content

- [ ] GET `/api/HealthTips` - List health tips
- [ ] GET `/api/HealthTips/{id}` - Get tip details
- [ ] GET `/api/HealthTips/categories` - List categories

**Admin Features (Later):**
- [ ] POST `/api/HealthTips` - Create tip
- [ ] PUT `/api/HealthTips/{id}` - Update tip
- [ ] DELETE `/api/HealthTips/{id}` - Delete tip

---

### **Phase 9: Admin Features** (4-5 hours)
Admin panel functionality

**Doctor Management:**
- [ ] POST `/api/Doctors` - Create doctor
- [ ] PUT `/api/Doctors/{id}` - Update doctor
- [ ] DELETE `/api/Doctors/{id}` - Delete doctor
- [ ] GET `/api/Doctors/{id}/statistics` - Performance stats

**Reports:**
- [ ] GET `/api/Reports/dashboard` - Dashboard overview
- [ ] GET `/api/Reports/appointments` - Appointments analytics
- [ ] GET `/api/Reports/doctors` - Doctor performance

---

### **Phase 10: Advanced Features** (Future)
AI and advanced analytics

**Symptom Checker:**
- [ ] POST `/api/SymptomChecker/check` - AI symptom analysis
- [ ] GET `/api/SymptomChecker/history` - Check history

**Medical History:**
- [ ] GET `/api/Users/medical-history` - Complete history
- [ ] POST `/api/Users/medical-history` - Add record
- [ ] GET `/api/Users/prescriptions` - All prescriptions

---

## ?? **Detailed Endpoint Specifications**

### **1. Forgot Password**

```csharp
POST /api/Auth/forgot-password
Request: { "email": "user@example.com" }
Response: {
  "success": true,
  "message": "Password reset code sent to email",
  "data": null
}
```

### **2. Reset Password**

```csharp
POST /api/Auth/reset-password
Request: {
  "email": "user@example.com",
  "token": "123456",
  "newPassword": "NewPass@123"
}
Response: {
  "success": true,
  "message": "Password reset successful",
  "data": null
}
```

### **3. Resend OTP**

```csharp
POST /api/Auth/resend-otp
Request: { "email": "user@example.com" }
Response: {
  "success": true,
  "message": "OTP resent successfully",
  "data": null
}
```

### **4. User Profile**

```csharp
GET /api/Users/profile
Response: {
  "success": true,
  "message": "Profile retrieved",
  "data": {
    "userId": "123",
    "email": "user@example.com",
    "fullName": "Ahmed Ali",
    "phoneNumber": "03001234567",
    "dateOfBirth": "2000-01-15",
    "gender": "Male",
    "address": "House 123",
    "department": "CS",
    "profileImageUrl": "https://..."
  }
}
```

### **5. Update Profile**

```csharp
PUT /api/Users/profile
Request: {
  "fullName": "Ahmed Ali Khan",
  "phoneNumber": "03009876543",
  "address": "New Address"
}
Response: {
  "success": true,
  "message": "Profile updated successfully",
  "data": { /* updated profile */ }
}
```

### **6. Change Password**

```csharp
POST /api/Users/change-password
Request: {
  "currentPassword": "OldPass@123",
  "newPassword": "NewPass@123"
}
Response: {
  "success": true,
  "message": "Password changed successfully",
  "data": null
}
```

### **7. Upload Photo**

```csharp
POST /api/Users/upload-photo
Content-Type: multipart/form-data
Request: FormFile (image)
Response: {
  "success": true,
  "message": "Photo uploaded successfully",
  "data": {
    "imageUrl": "https://storage.../profile.jpg"
  }
}
```

### **8. Doctor Search**

```csharp
GET /api/Doctors/search?query=Hassan&specialization=Cardiology&available=true
Response: {
  "success": true,
  "message": "Search results",
  "data": [
    {
      "id": "456",
      "doctorName": "Dr. Hassan",
      "specialization": "Cardiology",
      "isAvailable": true,
      "rating": 4.5
    }
  ]
}
```

### **9. Available Slots**

```csharp
GET /api/Doctors/{id}/available-slots?date=2024-12-20
Response: {
  "success": true,
  "message": "Available slots retrieved",
  "data": {
    "date": "2024-12-20",
    "slots": [
      {
        "time": "09:00",
        "duration": 30,
        "available": true
      },
      {
        "time": "09:30",
        "duration": 30,
        "available": false
      }
    ]
  }
}
```

### **10. Notifications**

```csharp
GET /api/Notifications
Response: {
  "success": true,
  "message": "Notifications retrieved",
  "data": [
    {
      "id": "1",
      "title": "Appointment Reminder",
      "message": "Your appointment is tomorrow at 10 AM",
      "type": "appointment",
      "isRead": false,
      "createdAt": "2024-12-18T08:00:00Z"
    }
  ]
}
```

---

## ??? **Implementation Strategy**

### **Recommended Approach:**

1. **Start with Phase 1** (Authentication) - 1-2 hours
   - Implement forgot/reset password
   - Add resend OTP
   - Test complete auth flow

2. **Move to Phase 2** (User Profile) - 2-3 hours
   - User profile CRUD
   - Password change
   - Photo upload

3. **Continue with Phase 3** (Doctor Search) - 3-4 hours
   - Search functionality
   - Available slots calculation
   - Schedule viewing

4. **Then Phase 4** (Appointment Enhancements) - 2-3 hours
   - Reschedule feature
   - History view

5. **Implement other phases based on priority**

---

## ?? **What to Implement First?**

For your Flutter app to work completely, implement in this order:

### **Week 1: Core Features**
1. ? Phase 1: Forgot/Reset Password (DONE)
2. ? Phase 2: User Profile Management (DONE)
3. ? Phase 3: Doctor Search & Slots (DONE)

### **Week 2: Enhanced Features**
4. Phase 4: Appointment Enhancements
5. Phase 5: Doctor Schedule Management
6. Phase 6: Medicine Reminder Logs

### **Week 3: Advanced Features**
7. Phase 7: Notifications System
8. Phase 8: Health Tips
9. Testing and bug fixes

### **Week 4: Admin & Polish**
10. Phase 9: Admin Features
11. Phase 10: Advanced features (optional)
12. Production deployment

---

## ? **Success Criteria**

Each phase is complete when:
- [ ] All endpoints implemented
- [ ] DTOs created and tested
- [ ] Build successful
- [ ] Swagger documentation updated
- [ ] Tested in Swagger UI
- [ ] Flutter integration tested
- [ ] Documentation updated
- [ ] Pushed to GitHub

---

## ?? **Resources Needed**

### **For Photo Upload:**
- Azure Blob Storage or local file storage
- Image processing library (optional)

### **For Notifications:**
- Firebase Cloud Messaging (FCM) setup
- Firebase Admin SDK

### **For Symptom Checker:**
- AI/ML model or external API
- OpenAI API (optional)

### **For Reports:**
- Chart data aggregation
- Date range filtering
- Export functionality (PDF/Excel)

---

## ?? **Immediate Next Steps**

**What should we implement right now?**

I recommend starting with **Phase 1 (Authentication)** as it's:
- ? Critical for Flutter app
- ? Quick to implement (1-2 hours)
- ? Uses existing infrastructure
- ? No external dependencies

**Shall I start implementing Phase 1 endpoints now?**

1. Forgot Password
2. Reset Password  
3. Resend OTP

This will give your Flutter app complete authentication functionality!

---

**Ready to start? Let me know which phase to implement first!** ??
