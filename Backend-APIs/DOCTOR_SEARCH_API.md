# ?? Doctor Search & Availability API

## ? Phase 3 Complete - Doctor Search & Scheduling Endpoints

Four essential doctor discovery and availability endpoints have been implemented.

---

## ?? **Standard Response Format**

All endpoints return:
```json
{
  "success": true/false,
  "message": "Description message",
  "data": { ... } // or [] or null
}
```

---

## ?? **1. SEARCH DOCTORS**

### **Endpoint:** `GET /api/Doctors/search`
**Authentication:** ? Not Required

Advanced search for doctors by name, specialization, and availability.

### **Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `query` | string | No | Search by doctor name or specialization |
| `specialization` | string | No | Filter by specific specialization |
| `availableOnly` | boolean | No | Show only available doctors (default: false) |

### **Example Requests:**

**Search by name:**
```
GET /api/Doctors/search?query=hassan
```

**Search by specialization:**
```
GET /api/Doctors/search?specialization=cardiology
```

**Get available doctors only:**
```
GET /api/Doctors/search?availableOnly=true
```

**Combined search:**
```
GET /api/Doctors/search?query=hassan&specialization=general&availableOnly=true
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Found 3 doctor(s)",
  "data": [
    {
      "doctorId": 5,
      "doctorName": "Dr. Hassan Mahmood",
      "specialization": "General Medicine",
      "qualifications": "MBBS, MD",
      "experienceYears": 10,
      "consultationFee": 1500.00,
      "isAvailable": true,
      "averageRating": 4.5,
      "totalReviews": 45,
      "profileImageUrl": "/uploads/doctors/5_profile.jpg",
      "bio": "Experienced general physician with 10 years of practice"
    },
    {
      "doctorId": 7,
      "doctorName": "Dr. Sara Ahmed",
      "specialization": "Cardiology",
      "qualifications": "MBBS, FCPS",
      "experienceYears": 8,
      "consultationFee": 2000.00,
      "isAvailable": true,
      "averageRating": 4.8,
      "totalReviews": 32,
      "profileImageUrl": "/uploads/doctors/7_profile.jpg",
      "bio": "Specialist in cardiovascular diseases"
    }
  ]
}
```

### **Empty Results:**
```json
{
  "success": true,
  "message": "Found 0 doctor(s)",
  "data": []
}
```

---

## ?? **2. GET SPECIALIZATIONS**

### **Endpoint:** `GET /api/Doctors/specializations`
**Authentication:** ? Not Required

Get list of all available medical specializations with doctor count.

### **Request:**
```
GET /api/Doctors/specializations
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Specializations retrieved successfully",
  "data": [
    {
      "specialization": "Cardiology",
      "doctorCount": 5
    },
    {
      "specialization": "Dermatology",
      "doctorCount": 3
    },
    {
      "specialization": "General Medicine",
      "doctorCount": 8
    },
    {
      "specialization": "Neurology",
      "doctorCount": 2
    },
    {
      "specialization": "Orthopedics",
      "doctorCount": 4
    },
    {
      "specialization": "Pediatrics",
      "doctorCount": 6
    }
  ]
}
```

**Use Case:** Perfect for populating dropdown/filter lists in your Flutter app!

---

## ?? **3. GET AVAILABLE TIME SLOTS**

### **Endpoint:** `GET /api/Doctors/{id}/available-slots`
**Authentication:** ? Not Required

Get available appointment time slots for a specific doctor on a specific date.

### **Path Parameters:**
- `id` (integer) - Doctor ID

### **Query Parameters:**
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `date` | string | ? Yes | Date in yyyy-MM-dd format |

### **Example Request:**
```
GET /api/Doctors/5/available-slots?date=2025-12-10
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Available slots retrieved successfully",
  "data": {
    "date": "2025-12-10",
    "doctorId": 5,
    "doctorName": "Dr. Hassan Mahmood",
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
      },
      {
        "time": "10:00",
        "duration": 30,
        "available": true
      },
      {
        "time": "10:30",
        "duration": 30,
        "available": true
      },
      {
        "time": "11:00",
        "duration": 30,
        "available": false
      },
      {
        "time": "11:30",
        "duration": 30,
        "available": true
      }
    ]
  }
}
```

### **Doctor Not Available on Date:**
```json
{
  "success": true,
  "message": "Doctor is not available on this day",
  "data": {
    "date": "2025-12-10",
    "doctorId": 5,
    "doctorName": "Dr. Hassan Mahmood",
    "slots": []
  }
}
```

### **Error Responses:**

**Invalid Date Format (400):**
```json
{
  "success": false,
  "message": "Invalid date format. Use yyyy-MM-dd",
  "data": null
}
```

**Past Date (400):**
```json
{
  "success": false,
  "message": "Cannot get slots for past dates",
  "data": null
}
```

**Doctor Not Found (404):**
```json
{
  "success": false,
  "message": "Doctor not found",
  "data": null
}
```

---

## ?? **4. GET DOCTOR SCHEDULE**

### **Endpoint:** `GET /api/Doctors/{id}/schedule`
**Authentication:** ? Not Required

Get doctor's weekly schedule showing available days and times.

### **Path Parameters:**
- `id` (integer) - Doctor ID

### **Example Request:**
```
GET /api/Doctors/5/schedule
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Schedule retrieved successfully",
  "data": [
    {
      "scheduleId": 1,
      "dayOfWeek": "Monday",
      "startTime": "09:00",
      "endTime": "17:00",
      "isAvailable": true
    },
    {
      "scheduleId": 2,
      "dayOfWeek": "Tuesday",
      "startTime": "09:00",
      "endTime": "17:00",
      "isAvailable": true
    },
    {
      "scheduleId": 3,
      "dayOfWeek": "Wednesday",
      "startTime": "09:00",
      "endTime": "13:00",
      "isAvailable": true
    },
    {
      "scheduleId": 5,
      "dayOfWeek": "Friday",
      "startTime": "14:00",
      "endTime": "18:00",
      "isAvailable": true
    }
  ]
}
```

**Note:** Days are sorted Monday to Sunday. Missing days mean doctor is not available.

---

## ?? **Flutter Integration Examples**

### **Example 1: Search Doctors**

```dart
import 'package:dio/dio.dart';

class DoctorSearchService {
  final Dio dio;

  DoctorSearchService(this.dio);

  Future<List<Doctor>> searchDoctors({
    String? query,
    String? specialization,
    bool availableOnly = false,
  }) async {
    try {
      final queryParams = <String, dynamic>{};
      if (query != null && query.isNotEmpty) {
        queryParams['query'] = query;
      }
      if (specialization != null && specialization.isNotEmpty) {
        queryParams['specialization'] = specialization;
      }
      if (availableOnly) {
        queryParams['availableOnly'] = true;
      }

      final response = await dio.get(
        '/api/Doctors/search',
        queryParameters: queryParams,
      );

      if (response.data['success']) {
        final List<dynamic> data = response.data['data'];
        return data.map((json) => Doctor.fromJson(json)).toList();
      }
      
      return [];
    } catch (e) {
      print('Search error: $e');
      return [];
    }
  }
}

// Usage
final doctors = await searchService.searchDoctors(
  query: 'hassan',
  availableOnly: true,
);
```

### **Example 2: Get Specializations**

```dart
Future<List<Specialization>> getSpecializations() async {
  try {
    final response = await dio.get('/api/Doctors/specializations');

    if (response.data['success']) {
      final List<dynamic> data = response.data['data'];
      return data.map((json) => Specialization.fromJson(json)).toList();
    }
    
    return [];
  } catch (e) {
    print('Error getting specializations: $e');
    return [];
  }
}

// Usage in dropdown
DropdownButton<String>(
  items: specializations.map((spec) {
    return DropdownMenuItem(
      value: spec.specialization,
      child: Text('${spec.specialization} (${spec.doctorCount})'),
    );
  }).toList(),
  onChanged: (value) {
    setState(() => selectedSpecialization = value);
  },
);
```

### **Example 3: Get Available Slots**

```dart
Future<AvailableSlots?> getAvailableSlots(int doctorId, DateTime date) async {
  try {
    final dateStr = DateFormat('yyyy-MM-dd').format(date);
    
    final response = await dio.get(
      '/api/Doctors/$doctorId/available-slots',
      queryParameters: {'date': dateStr},
    );

    if (response.data['success']) {
      return AvailableSlots.fromJson(response.data['data']);
    }
    
    return null;
  } catch (e) {
    print('Error getting slots: $e');
    return null;
  }
}

// Display slots in UI
ListView.builder(
  itemCount: slots.slots.length,
  itemBuilder: (context, index) {
    final slot = slots.slots[index];
    return ListTile(
      title: Text(slot.time),
      trailing: slot.available
          ? ElevatedButton(
              onPressed: () => bookAppointment(slot.time),
              child: Text('Book'),
            )
          : Chip(
              label: Text('Booked'),
              backgroundColor: Colors.grey,
            ),
      enabled: slot.available,
    );
  },
);
```

### **Example 4: Get Doctor Schedule**

```dart
Future<List<DoctorSchedule>> getDoctorSchedule(int doctorId) async {
  try {
    final response = await dio.get('/api/Doctors/$doctorId/schedule');

    if (response.data['success']) {
      final List<dynamic> data = response.data['data'];
      return data.map((json) => DoctorSchedule.fromJson(json)).toList();
    }
    
    return [];
  } catch (e) {
    print('Error getting schedule: $e');
    return [];
  }
}

// Display weekly schedule
Column(
  children: schedules.map((schedule) {
    return Card(
      child: ListTile(
        leading: Icon(Icons.calendar_today),
        title: Text(schedule.dayOfWeek),
        subtitle: Text('${schedule.startTime} - ${schedule.endTime}'),
        trailing: schedule.isAvailable
            ? Icon(Icons.check_circle, color: Colors.green)
            : Icon(Icons.cancel, color: Colors.red),
      ),
    );
  }).toList(),
);
```

---

## ?? **Complete Flutter Doctor Selection Screen**

```dart
class DoctorSelectionScreen extends StatefulWidget {
  @override
  _DoctorSelectionScreenState createState() => _DoctorSelectionScreenState();
}

class _DoctorSelectionScreenState extends State<DoctorSelectionScreen> {
  List<Doctor> _doctors = [];
  List<Specialization> _specializations = [];
  String? _selectedSpecialization;
  String _searchQuery = '';
  bool _availableOnly = false;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _loadSpecializations();
    _searchDoctors();
  }

  Future<void> _loadSpecializations() async {
    final specializations = await getSpecializations();
    setState(() => _specializations = specializations);
  }

  Future<void> _searchDoctors() async {
    setState(() => _isLoading = true);
    
    final doctors = await searchDoctors(
      query: _searchQuery.isEmpty ? null : _searchQuery,
      specialization: _selectedSpecialization,
      availableOnly: _availableOnly,
    );
    
    setState(() {
      _doctors = doctors;
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Find a Doctor')),
      body: Column(
        children: [
          // Search Bar
          Padding(
            padding: EdgeInsets.all(16),
            child: TextField(
              decoration: InputDecoration(
                hintText: 'Search doctors...',
                prefixIcon: Icon(Icons.search),
                border: OutlineInputBorder(),
              ),
              onChanged: (value) {
                setState(() => _searchQuery = value);
                _searchDoctors();
              },
            ),
          ),
          
          // Filters
          Padding(
            padding: EdgeInsets.symmetric(horizontal: 16),
            child: Row(
              children: [
                Expanded(
                  child: DropdownButton<String>(
                    value: _selectedSpecialization,
                    hint: Text('All Specializations'),
                    isExpanded: true,
                    items: _specializations.map((spec) {
                      return DropdownMenuItem(
                        value: spec.specialization,
                        child: Text('${spec.specialization} (${spec.doctorCount})'),
                      );
                    }).toList(),
                    onChanged: (value) {
                      setState(() => _selectedSpecialization = value);
                      _searchDoctors();
                    },
                  ),
                ),
                SizedBox(width: 16),
                FilterChip(
                  label: Text('Available Only'),
                  selected: _availableOnly,
                  onSelected: (selected) {
                    setState(() => _availableOnly = selected);
                    _searchDoctors();
                  },
                ),
              ],
            ),
          ),
          
          // Results
          Expanded(
            child: _isLoading
                ? Center(child: CircularProgressIndicator())
                : _doctors.isEmpty
                    ? Center(child: Text('No doctors found'))
                    : ListView.builder(
                        itemCount: _doctors.length,
                        itemBuilder: (context, index) {
                          final doctor = _doctors[index];
                          return DoctorCard(
                            doctor: doctor,
                            onTap: () {
                              Navigator.push(
                                context,
                                MaterialPageRoute(
                                  builder: (context) => DoctorDetailScreen(
                                    doctorId: doctor.doctorId,
                                  ),
                                ),
                              );
                            },
                          );
                        },
                      ),
          ),
        ],
      ),
    );
  }
}

// Doctor Card Widget
class DoctorCard extends StatelessWidget {
  final Doctor doctor;
  final VoidCallback onTap;

  const DoctorCard({required this.doctor, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundImage: doctor.profileImageUrl != null
              ? NetworkImage('$baseUrl${doctor.profileImageUrl}')
              : null,
          child: doctor.profileImageUrl == null
              ? Icon(Icons.person)
              : null,
        ),
        title: Text(doctor.doctorName),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(doctor.specialization),
            if (doctor.experienceYears != null)
              Text('${doctor.experienceYears} years experience'),
            Row(
              children: [
                Icon(Icons.star, size: 16, color: Colors.amber),
                Text(' ${doctor.averageRating?.toStringAsFixed(1) ?? 'N/A'}'),
                Text(' (${doctor.totalReviews ?? 0} reviews)'),
              ],
            ),
          ],
        ),
        trailing: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              'Rs ${doctor.consultationFee?.toStringAsFixed(0) ?? 'N/A'}',
              style: TextStyle(fontWeight: FontWeight.bold),
            ),
            Chip(
              label: Text(
                doctor.isAvailable ? 'Available' : 'Busy',
                style: TextStyle(fontSize: 10),
              ),
              backgroundColor: doctor.isAvailable
                  ? Colors.green[100]
                  : Colors.red[100],
            ),
          ],
        ),
        onTap: onTap,
      ),
    );
  }
}
```

---

## ? **Quick Reference**

| Endpoint | Method | Auth | Purpose |
|----------|--------|------|---------|
| `/api/Doctors/search` | GET | ? | Search doctors |
| `/api/Doctors/specializations` | GET | ? | List specializations |
| `/api/Doctors/{id}/available-slots` | GET | ? | Get available time slots |
| `/api/Doctors/{id}/schedule` | GET | ? | Get weekly schedule |

---

## ?? **Search Features**

### **What You Can Search By:**
- ? Doctor name (partial match)
- ? Specialization (partial match)
- ? Availability status
- ? Combined filters

### **Search Results Include:**
- ? Doctor basic info
- ? Specialization & qualifications
- ? Experience years
- ? Consultation fee
- ? Availability status
- ? Rating & reviews
- ? Profile photo
- ? Bio/description

---

## ?? **Slot Calculation Logic**

### **How Slots Are Generated:**
1. Get doctor's schedule for requested day
2. Generate 30-minute slots between start and end time
3. Check existing appointments for that date
4. Mark slots as booked or available
5. Return complete list with availability status

### **Example:**
```
Schedule: Monday 09:00 - 17:00
Slot Duration: 30 minutes
Existing Appointments: 09:30, 14:00

Generated Slots:
09:00 ? Available
09:30 ? Booked
10:00 ? Available
10:30 ? Available
...
14:00 ? Booked
14:30 ? Available
...
```

---

## ? **Testing Checklist**

- [ ] Search doctors by name
- [ ] Search doctors by specialization
- [ ] Search with availableOnly=true filter
- [ ] Get all specializations list
- [ ] Get available slots for valid date
- [ ] Get available slots for past date (should fail)
- [ ] Get available slots for doctor with no schedule
- [ ] Get available slots for fully booked day
- [ ] Get doctor's weekly schedule
- [ ] Get schedule for non-existent doctor (should fail)

---

## ?? **Common Issues & Solutions**

### **Issue: "No slots available"**
**Solution:** Check if doctor has schedule set for that day of week

### **Issue: "Invalid date format"**
**Solution:** Use yyyy-MM-dd format (e.g., "2025-12-10")

### **Issue: "Cannot get slots for past dates"**
**Solution:** Only request slots for today or future dates

### **Issue: "Empty search results"**
**Solution:** Check spelling, try partial matches, remove filters

---

## ?? **Phase 3 Complete!**

All doctor search and availability endpoints are implemented and ready for Flutter integration!

**Base URL:** `https://localhost:7228`  
**Swagger UI:** `https://localhost:7228/swagger`

---

**Last Updated:** December 18, 2024  
**Status:** ? Phase 3 Complete - Doctor Search & Availability APIs Ready
