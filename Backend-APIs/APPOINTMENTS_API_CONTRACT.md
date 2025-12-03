# ?? Appointments API Documentation

## ? Complete API Contract for Flutter Integration

All endpoints now match your Flutter app's requirements with consistent response formats.

---

## ?? Standard Response Format

```json
{
  "success": true/false,
  "message": "Description message",
  "data": { ... } // or null or []
}
```

---

## ?? Authentication

All endpoints except health check require JWT Bearer token:
```
Authorization: Bearer YOUR_JWT_TOKEN
```

---

## ?? **1. BOOK APPOINTMENT**

### **Endpoint:** `POST /api/Appointments`
**Authentication:** ? Required

### **Request Body:**
```json
{
  "patientId": "123",
  "patientName": "Ahmed Ali",
  "doctorId": "456",
  "doctorName": "Dr. Hassan Mahmood",
  "specialization": "General Medicine",
  "dateTime": "2025-12-10T10:30:00Z",
  "symptoms": "Headache and fever",
  "notes": "Additional notes here",
  "status": "Pending"
}
```

### **Field Details:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `patientId` | string | ? Yes | Patient user ID |
| `patientName` | string | No | Patient full name (optional) |
| `doctorId` | string | ? Yes | Doctor ID to book with |
| `doctorName` | string | No | Doctor name (optional) |
| `specialization` | string | No | Doctor specialization (optional) |
| `dateTime` | string | ? Yes | ISO 8601 format (e.g., "2025-12-10T10:30:00Z") |
| `symptoms` | string | No | Patient symptoms description |
| `notes` | string | No | Additional notes |
| `status` | string | No | Default: "Pending" |

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointment booked successfully",
  "data": {
    "id": "789",
    "patientId": "123",
    "patientName": "Ahmed Ali",
    "doctorId": "456",
    "doctorName": "Dr. Hassan Mahmood",
    "specialization": "General Medicine",
    "dateTime": "2025-12-10T10:30:00.0000000Z",
    "status": "Pending",
    "symptoms": "Headache and fever",
    "notes": "Additional notes here",
    "prescription": null,
    "createdAt": "2024-12-18T08:00:00.0000000Z"
  }
}
```

### **Error Responses:**

**Doctor Not Found (400):**
```json
{
  "success": false,
  "message": "Doctor not found",
  "data": null
}
```

**Doctor Unavailable (400):**
```json
{
  "success": false,
  "message": "Doctor is not available",
  "data": null
}
```

**Time Slot Conflict (400):**
```json
{
  "success": false,
  "message": "This time slot is already booked",
  "data": null
}
```

---

## ?? **2. GET STUDENT UPCOMING APPOINTMENTS**

### **Endpoint:** `GET /api/Appointments/student/{studentId}/upcoming`
**Authentication:** ? Required

### **Path Parameters:**
- `studentId` (string) - The student/patient user ID

### **Example Request:**
```
GET /api/Appointments/student/123/upcoming
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointments retrieved successfully",
  "data": [
    {
      "id": "789",
      "patientId": "123",
      "patientName": "Ahmed Ali",
      "doctorId": "456",
      "doctorName": "Dr. Hassan Mahmood",
      "specialization": "General Medicine",
      "dateTime": "2025-12-10T10:30:00.0000000Z",
      "status": "Pending",
      "symptoms": "Headache and fever",
      "notes": "Additional notes",
      "prescription": null,
      "createdAt": "2024-12-03T08:00:00.0000000Z"
    },
    {
      "id": "790",
      "patientId": "123",
      "patientName": "Ahmed Ali",
      "doctorId": "457",
      "doctorName": "Dr. Sara Ahmed",
      "specialization": "Cardiology",
      "dateTime": "2025-12-15T14:00:00.0000000Z",
      "status": "Confirmed",
      "symptoms": "Chest pain",
      "notes": "Follow-up appointment",
      "prescription": null,
      "createdAt": "2024-12-05T10:00:00.0000000Z"
    }
  ]
}
```

### **Empty List Response:**
```json
{
  "success": true,
  "message": "Appointments retrieved successfully",
  "data": []
}
```

---

## ?? **3. GET FACULTY APPOINTMENTS**

### **Endpoint:** `GET /api/Appointments/Faculty/appointments`
**Authentication:** ? Required (Doctor/Faculty/Admin role)

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointments retrieved successfully",
  "data": [
    {
      "id": "789",
      "patientId": "123",
      "patientName": "Ahmed Ali",
      "doctorId": "456",
      "doctorName": "Dr. Hassan Mahmood",
      "specialization": "General Medicine",
      "dateTime": "2025-12-10T10:30:00.0000000Z",
      "status": "Pending",
      "symptoms": "Headache and fever",
      "notes": "Additional notes",
      "prescription": null,
      "createdAt": "2024-12-03T08:00:00.0000000Z"
    }
  ]
}
```

### **Error Response (404):**
```json
{
  "success": false,
  "message": "Doctor profile not found",
  "data": null
}
```

---

## ?? **4. GET APPOINTMENT BY ID**

### **Endpoint:** `GET /api/Appointments/{appointmentId}`
**Authentication:** ? Required

### **Path Parameters:**
- `appointmentId` (string) - The appointment ID

### **Example Request:**
```
GET /api/Appointments/789
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointment retrieved successfully",
  "data": {
    "id": "789",
    "patientId": "123",
    "patientName": "Ahmed Ali",
    "doctorId": "456",
    "doctorName": "Dr. Hassan Mahmood",
    "specialization": "General Medicine",
    "dateTime": "2025-12-10T10:30:00.0000000Z",
    "status": "Pending",
    "symptoms": "Headache and fever",
    "notes": "Additional notes",
    "prescription": null,
    "createdAt": "2024-12-03T08:00:00.0000000Z"
  }
}
```

### **Error Response (404):**
```json
{
  "success": false,
  "message": "Appointment not found",
  "data": null
}
```

---

## ?? **5. UPDATE APPOINTMENT STATUS**

### **Endpoint:** `PUT /api/Appointments/{appointmentId}/status`
**Authentication:** ? Required (Doctor/Faculty/Admin role)

### **Request Body:**
```json
{
  "status": "Confirmed"
}
```

### **Valid Status Values:**
- `"Pending"` - Newly booked, awaiting confirmation
- `"Confirmed"` - Approved by doctor
- `"Completed"` - Visit completed, may have prescription
- `"Cancelled"` - Cancelled by patient or doctor

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointment status updated successfully",
  "data": {
    "status": "Confirmed"
  }
}
```

### **Error Responses:**

**Invalid Status (400):**
```json
{
  "success": false,
  "message": "Invalid status. Valid values: Pending, Confirmed, Completed, Cancelled",
  "data": null
}
```

**Not Found (404):**
```json
{
  "success": false,
  "message": "Appointment not found",
  "data": null
}
```

---

## ?? **6. CANCEL APPOINTMENT**

### **Endpoint:** `DELETE /api/Appointments/{appointmentId}`
**Authentication:** ? Required

### **Example Request:**
```
DELETE /api/Appointments/789
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Appointment cancelled successfully",
  "data": null
}
```

### **Error Responses:**

**Already Cancelled (400):**
```json
{
  "success": false,
  "message": "Appointment is already cancelled",
  "data": null
}
```

**Not Found (404):**
```json
{
  "success": false,
  "message": "Appointment not found",
  "data": null
}
```

---

## ?? **7. ADD PRESCRIPTION**

### **Endpoint:** `PUT /api/Appointments/{appointmentId}/prescription`
**Authentication:** ? Required (Doctor/Faculty/Admin role)

### **Request Body:**
```json
{
  "prescription": "Paracetamol 500mg, Take twice daily for 3 days. Rest and drink plenty of fluids."
}
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Prescription added successfully",
  "data": {
    "prescriptionId": 100
  }
}
```

**Note:** Adding a prescription automatically changes appointment status to "Completed"

### **Error Response (404):**
```json
{
  "success": false,
  "message": "Appointment not found",
  "data": null
}
```

---

## ?? **Appointment Object Structure**

### **Complete Appointment Object:**

```json
{
  "id": "string",                    // Appointment ID
  "patientId": "string",             // Patient user ID
  "patientName": "string",           // Patient full name
  "doctorId": "string",              // Doctor ID
  "doctorName": "string",            // Doctor full name
  "specialization": "string",        // Doctor's specialization
  "dateTime": "ISO 8601 string",     // Appointment date & time
  "status": "string",                // Pending|Confirmed|Completed|Cancelled
  "symptoms": "string|null",         // Patient symptoms (optional)
  "notes": "string|null",            // Additional notes (optional)
  "prescription": "string|null",     // Doctor's prescription (if added)
  "createdAt": "ISO 8601 string"     // When appointment was created
}
```

---

## ?? **Status Flow**

```
Pending ? Confirmed ? Completed
   ?
Cancelled
```

**Status Transitions:**
1. **Pending** - Patient books appointment
2. **Confirmed** - Doctor accepts appointment
3. **Completed** - Doctor adds prescription
4. **Cancelled** - Patient or doctor cancels

---

## ?? **Flutter Integration Examples**

### **Book Appointment:**
```dart
final response = await http.post(
  Uri.parse('$baseUrl/api/Appointments'),
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer $token',
  },
  body: jsonEncode({
    'patientId': userId.toString(),
    'patientName': userFullName,
    'doctorId': doctorId.toString(),
    'doctorName': doctorName,
    'specialization': specialization,
    'dateTime': selectedDateTime.toIso8601String(),
    'symptoms': symptomsController.text,
    'notes': notesController.text,
    'status': 'Pending',
  }),
);

final data = jsonDecode(response.body);
if (data['success']) {
  final appointment = data['data'];
  print('Booked: ${appointment['id']}');
}
```

### **Get Upcoming Appointments:**
```dart
final response = await http.get(
  Uri.parse('$baseUrl/api/Appointments/student/$userId/upcoming'),
  headers: {
    'Authorization': 'Bearer $token',
  },
);

final data = jsonDecode(response.body);
if (data['success']) {
  final appointments = data['data'] as List;
  // Display appointments list
}
```

### **Cancel Appointment:**
```dart
final response = await http.delete(
  Uri.parse('$baseUrl/api/Appointments/$appointmentId'),
  headers: {
    'Authorization': 'Bearer $token',
  },
);

final data = jsonDecode(response.body);
if (data['success']) {
  print(data['message']); // "Appointment cancelled successfully"
}
```

---

## ? **Quick Reference**

| Endpoint | Method | Auth | Purpose |
|----------|--------|------|---------|
| `/api/Appointments` | POST | ? | Book appointment |
| `/api/Appointments/student/{id}/upcoming` | GET | ? | Get patient's upcoming appointments |
| `/api/Appointments/Faculty/appointments` | GET | ? Doctor | Get doctor's appointments |
| `/api/Appointments/{id}` | GET | ? | Get single appointment |
| `/api/Appointments/{id}/status` | PUT | ? Doctor | Update status |
| `/api/Appointments/{id}` | DELETE | ? | Cancel appointment |
| `/api/Appointments/{id}/prescription` | PUT | ? Doctor | Add prescription |

---

## ?? **Common Issues & Solutions**

### **Issue: "This time slot is already booked"**
**Solution:** Choose a different date/time or check doctor's schedule

### **Issue: "Doctor is not available"**
**Solution:** Doctor has marked themselves unavailable, choose another doctor

### **Issue: "Invalid appointment ID"**
**Solution:** Ensure appointment ID is a valid integer string

### **Issue: "Doctor profile not found"**
**Solution:** User must have a doctor profile to access Faculty endpoints

---

## ? **Testing Checklist**

- [ ] Book appointment with all fields
- [ ] Book appointment with only required fields
- [ ] Get upcoming appointments for student
- [ ] Get all appointments for doctor/faculty
- [ ] Get single appointment by ID
- [ ] Update appointment status (Confirmed)
- [ ] Cancel appointment
- [ ] Add prescription to appointment
- [ ] Try booking conflicting time slot
- [ ] Try booking with unavailable doctor

---

## ?? **All Endpoints Ready!**

The Appointments API is fully implemented and tested. Start integrating with your Flutter app!

**Base URL:** `http://10.0.2.2:5000` (Android Emulator)

For issues, check the `message` field in response for detailed error information.
