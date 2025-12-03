# ?? User Profile Management API

## ? Phase 2 Complete - User Profile Endpoints

Four essential user profile management endpoints have been implemented.

---

## ?? **Standard Response Format**

All endpoints return:
```json
{
  "success": true/false,
  "message": "Description message",
  "data": { ... } // or null
}
```

---

## ?? **Authentication Required**

All endpoints require JWT Bearer token:
```
Authorization: Bearer YOUR_JWT_TOKEN
```

---

## ?? **1. GET USER PROFILE**

### **Endpoint:** `GET /api/Users/profile`
**Authentication:** ? Required

Get the current authenticated user's profile information.

### **Request:**
```
GET https://localhost:7228/api/Users/profile
Headers:
  Authorization: Bearer {token}
```

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Profile retrieved successfully",
  "data": {
    "userId": 123,
    "email": "ahmed@student.buitms.edu.pk",
    "fullName": "Ahmed Ali Khan",
    "role": "Student",
    "department": "Computer Science",
    "registrationNumber": "59858",
    "phoneNumber": "03001234567",
    "dateOfBirth": "2002-05-15",
    "gender": "Male",
    "address": "House 123, Street 5, Quetta",
    "profileImageUrl": "/uploads/profiles/123_abc123.jpg",
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

### **Error Response (404 Not Found):**
```json
{
  "success": false,
  "message": "User not found",
  "data": null
}
```

---

## ?? **2. UPDATE USER PROFILE**

### **Endpoint:** `PUT /api/Users/profile`
**Authentication:** ? Required

Update the current user's profile information.

### **Request Body:**
```json
{
  "fullName": "Ahmed Ali Khan Updated",
  "phoneNumber": "03009876543",
  "dateOfBirth": "2002-05-15",
  "gender": "Male",
  "address": "New Address, Quetta",
  "department": "Software Engineering",
  "registrationNumber": "59858"
}
```

### **Field Details:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `fullName` | string | No | User's full name |
| `phoneNumber` | string | No | Contact phone number |
| `dateOfBirth` | string | No | Birth date (yyyy-MM-dd format) |
| `gender` | string | No | Male/Female |
| `address` | string | No | Full address |
| `department` | string | No | Department/Faculty |
| `registrationNumber` | string | No | CMS ID or registration number |

**Note:** All fields are optional. Only provide fields you want to update.

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Profile updated successfully",
  "data": {
    "userId": 123,
    "email": "ahmed@student.buitms.edu.pk",
    "fullName": "Ahmed Ali Khan Updated",
    "role": "Student",
    "department": "Software Engineering",
    "registrationNumber": "59858",
    "phoneNumber": "03009876543",
    "dateOfBirth": "2002-05-15",
    "gender": "Male",
    "address": "New Address, Quetta",
    "profileImageUrl": "/uploads/profiles/123_abc123.jpg",
    "isEmailVerified": true,
    "isActive": true
  }
}
```

### **Error Responses:**

**Invalid Request (400):**
```json
{
  "success": false,
  "message": "Invalid request data",
  "data": null
}
```

**Unauthorized (401):**
```json
{
  "success": false,
  "message": "Invalid token",
  "data": null
}
```

---

## ?? **3. CHANGE PASSWORD**

### **Endpoint:** `POST /api/Users/change-password`
**Authentication:** ? Required

Change the current user's password.

### **Request Body:**
```json
{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword456!"
}
```

### **Field Details:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `currentPassword` | string | ? Yes | User's current password |
| `newPassword` | string | ? Yes | New password (min 6 characters recommended) |

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Password changed successfully",
  "data": null
}
```

### **Error Responses:**

**Incorrect Current Password (400):**
```json
{
  "success": false,
  "message": "Current password is incorrect",
  "data": null
}
```

**Invalid Request (400):**
```json
{
  "success": false,
  "message": "Invalid request data",
  "data": null
}
```

**Unauthorized (401):**
```json
{
  "success": false,
  "message": "Invalid token",
  "data": null
}
```

---

## ?? **4. UPLOAD PROFILE PHOTO**

### **Endpoint:** `POST /api/Users/upload-photo`
**Authentication:** ? Required  
**Content-Type:** `multipart/form-data`

Upload or update the user's profile photo.

### **Request:**
```
POST https://localhost:7228/api/Users/upload-photo
Headers:
  Authorization: Bearer {token}
  Content-Type: multipart/form-data
Body:
  photo: [Image File]
```

### **File Requirements:**
- **Allowed formats:** JPG, JPEG, PNG, GIF
- **Maximum size:** 5 MB
- **Field name:** `photo`

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Profile photo uploaded successfully",
  "data": {
    "imageUrl": "/uploads/profiles/123_abc123-def456.jpg"
  }
}
```

### **Error Responses:**

**No File Uploaded (400):**
```json
{
  "success": false,
  "message": "No file uploaded",
  "data": null
}
```

**Invalid File Type (400):**
```json
{
  "success": false,
  "message": "Invalid file type. Only JPG, PNG, and GIF are allowed",
  "data": null
}
```

**File Too Large (400):**
```json
{
  "success": false,
  "message": "File size must be less than 5MB",
  "data": null
}
```

**Unauthorized (401):**
```json
{
  "success": false,
  "message": "Invalid token",
  "data": null
}
```

---

## ?? **Flutter Integration Examples**

### **Example 1: Get Profile**

```dart
Future<UserProfile?> getProfile() async {
  try {
    final token = await storage.read(key: 'token');
    
    final response = await dio.get(
      '/api/Users/profile',
      options: Options(
        headers: {'Authorization': 'Bearer $token'},
      ),
    );

    if (response.data['success']) {
      return UserProfile.fromJson(response.data['data']);
    }
  } catch (e) {
    print('Error getting profile: $e');
  }
  return null;
}
```

### **Example 2: Update Profile**

```dart
Future<bool> updateProfile({
  String? fullName,
  String? phoneNumber,
  String? address,
}) async {
  try {
    final token = await storage.read(key: 'token');
    
    final response = await dio.put(
      '/api/Users/profile',
      options: Options(
        headers: {'Authorization': 'Bearer $token'},
      ),
      data: {
        if (fullName != null) 'fullName': fullName,
        if (phoneNumber != null) 'phoneNumber': phoneNumber,
        if (address != null) 'address': address,
      },
    );

    if (response.data['success']) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(response.data['message']),
          backgroundColor: Colors.green,
        ),
      );
      return true;
    }
  } catch (e) {
    print('Error updating profile: $e');
  }
  return false;
}
```

### **Example 3: Change Password**

```dart
Future<bool> changePassword({
  required String currentPassword,
  required String newPassword,
}) async {
  try {
    final token = await storage.read(key: 'token');
    
    final response = await dio.post(
      '/api/Users/change-password',
      options: Options(
        headers: {'Authorization': 'Bearer $token'},
      ),
      data: {
        'currentPassword': currentPassword,
        'newPassword': newPassword,
      },
    );

    if (response.data['success']) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Password changed successfully'),
          backgroundColor: Colors.green,
        ),
      );
      return true;
    }
  } catch (e) {
    final errorMessage = e is DioError && e.response != null
        ? e.response!.data['message']
        : 'Failed to change password';
    
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(errorMessage),
        backgroundColor: Colors.red,
      ),
    );
  }
  return false;
}
```

### **Example 4: Upload Profile Photo**

```dart
import 'package:image_picker/image_picker.dart';
import 'package:dio/dio.dart' as dio_pkg;

Future<String?> uploadProfilePhoto() async {
  try {
    // Pick image
    final picker = ImagePicker();
    final pickedFile = await picker.pickImage(
      source: ImageSource.gallery,
      maxWidth: 1024,
      maxHeight: 1024,
      imageQuality: 85,
    );

    if (pickedFile == null) return null;

    final token = await storage.read(key: 'token');
    
    // Create multipart request
    final formData = dio_pkg.FormData.fromMap({
      'photo': await dio_pkg.MultipartFile.fromFile(
        pickedFile.path,
        filename: 'profile.jpg',
      ),
    });

    final response = await dio.post(
      '/api/Users/upload-photo',
      options: Options(
        headers: {'Authorization': 'Bearer $token'},
      ),
      data: formData,
    );

    if (response.data['success']) {
      final imageUrl = response.data['data']['imageUrl'];
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Photo uploaded successfully'),
          backgroundColor: Colors.green,
        ),
      );
      return imageUrl;
    }
  } catch (e) {
    final errorMessage = e is DioError && e.response != null
        ? e.response!.data['message']
        : 'Failed to upload photo';
    
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(errorMessage),
        backgroundColor: Colors.red,
      ),
    );
  }
  return null;
}
```

---

## ?? **Complete Flutter Profile Screen Example**

```dart
class ProfileScreen extends StatefulWidget {
  @override
  _ProfileScreenState createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  UserProfile? _profile;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadProfile();
  }

  Future<void> _loadProfile() async {
    setState(() => _isLoading = true);
    
    final profile = await getProfile();
    
    setState(() {
      _profile = profile;
      _isLoading = false;
    });
  }

  Future<void> _pickAndUploadPhoto() async {
    final imageUrl = await uploadProfilePhoto();
    if (imageUrl != null) {
      await _loadProfile(); // Reload profile
    }
  }

  Future<void> _showEditDialog() async {
    final nameController = TextEditingController(text: _profile?.fullName);
    final phoneController = TextEditingController(text: _profile?.phoneNumber);
    final addressController = TextEditingController(text: _profile?.address);

    await showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text('Edit Profile'),
        content: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              TextField(
                controller: nameController,
                decoration: InputDecoration(labelText: 'Full Name'),
              ),
              TextField(
                controller: phoneController,
                decoration: InputDecoration(labelText: 'Phone Number'),
              ),
              TextField(
                controller: addressController,
                decoration: InputDecoration(labelText: 'Address'),
                maxLines: 2,
              ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: Text('Cancel'),
          ),
          ElevatedButton(
            onPressed: () async {
              final success = await updateProfile(
                fullName: nameController.text,
                phoneNumber: phoneController.text,
                address: addressController.text,
              );
              
              if (success) {
                Navigator.pop(context);
                await _loadProfile();
              }
            },
            child: Text('Save'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return Scaffold(
        appBar: AppBar(title: Text('Profile')),
        body: Center(child: CircularProgressIndicator()),
      );
    }

    return Scaffold(
      appBar: AppBar(
        title: Text('Profile'),
        actions: [
          IconButton(
            icon: Icon(Icons.edit),
            onPressed: _showEditDialog,
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: EdgeInsets.all(16),
        child: Column(
          children: [
            // Profile Photo
            Stack(
              children: [
                CircleAvatar(
                  radius: 60,
                  backgroundImage: _profile?.profileImageUrl != null
                      ? NetworkImage('$baseUrl${_profile!.profileImageUrl}')
                      : null,
                  child: _profile?.profileImageUrl == null
                      ? Icon(Icons.person, size: 60)
                      : null,
                ),
                Positioned(
                  bottom: 0,
                  right: 0,
                  child: IconButton(
                    icon: Icon(Icons.camera_alt, color: Colors.white),
                    onPressed: _pickAndUploadPhoto,
                    style: IconButton.styleFrom(
                      backgroundColor: Theme.of(context).primaryColor,
                    ),
                  ),
                ),
              ],
            ),
            SizedBox(height: 24),
            
            // Profile Information
            _buildInfoTile('Name', _profile?.fullName ?? 'N/A'),
            _buildInfoTile('Email', _profile?.email ?? 'N/A'),
            _buildInfoTile('Phone', _profile?.phoneNumber ?? 'N/A'),
            _buildInfoTile('Department', _profile?.department ?? 'N/A'),
            _buildInfoTile('Registration #', _profile?.registrationNumber ?? 'N/A'),
            _buildInfoTile('Gender', _profile?.gender ?? 'N/A'),
            _buildInfoTile('Address', _profile?.address ?? 'N/A'),
            
            SizedBox(height: 24),
            
            // Change Password Button
            ElevatedButton.icon(
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => ChangePasswordScreen(),
                  ),
                );
              },
              icon: Icon(Icons.lock),
              label: Text('Change Password'),
              style: ElevatedButton.styleFrom(
                minimumSize: Size(double.infinity, 50),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildInfoTile(String label, String value) {
    return ListTile(
      title: Text(label, style: TextStyle(fontSize: 12, color: Colors.grey)),
      subtitle: Text(value, style: TextStyle(fontSize: 16)),
    );
  }
}
```

---

## ? **Quick Reference**

| Endpoint | Method | Auth | Purpose |
|----------|--------|------|---------|
| `/api/Users/profile` | GET | ? | Get user profile |
| `/api/Users/profile` | PUT | ? | Update profile |
| `/api/Users/change-password` | POST | ? | Change password |
| `/api/Users/upload-photo` | POST | ? | Upload profile photo |

---

## ?? **Security Features**

### **Profile Access:**
- ? JWT authentication required
- ? Users can only access their own profile
- ? Token validation on every request

### **Password Change:**
- ? Requires current password verification
- ? BCrypt password hashing
- ? Secure password storage

### **Photo Upload:**
- ? File type validation (JPG, PNG, GIF only)
- ? File size limit (5MB max)
- ? Unique filename generation
- ? Old photo deletion on update
- ? Secure file storage

---

## ?? **File Storage**

### **Upload Directory:**
```
Backend-APIs/
??? uploads/
    ??? profiles/
        ??? 123_abc123-def456.jpg
        ??? 124_xyz789-ghi012.png
        ??? ...
```

### **Image URL Format:**
```
/uploads/profiles/{userId}_{guid}.{extension}
```

### **Accessing Images:**
```
Full URL: https://localhost:7228/uploads/profiles/123_abc123.jpg
In Flutter: $baseUrl/uploads/profiles/123_abc123.jpg
```

---

## ? **Testing Checklist**

- [ ] Test get profile with valid token
- [ ] Test get profile with invalid token
- [ ] Test update profile with all fields
- [ ] Test update profile with partial fields
- [ ] Test change password with correct current password
- [ ] Test change password with incorrect current password
- [ ] Test upload photo with valid image (JPG)
- [ ] Test upload photo with valid image (PNG)
- [ ] Test upload photo with invalid file type
- [ ] Test upload photo with file too large
- [ ] Test uploading second photo (should replace first)
- [ ] Verify old photo is deleted when new one uploaded

---

## ?? **Common Issues & Solutions**

### **Issue: "Invalid token"**
**Solution:** Re-login to get fresh JWT token

### **Issue: "Current password is incorrect"**
**Solution:** Verify user is entering correct current password

### **Issue: "File size must be less than 5MB"**
**Solution:** Compress image before upload in Flutter app

### **Issue: "Profile photo not displayed"**
**Solution:** Ensure static files middleware is enabled and uploads directory exists

### **Issue: "401 Unauthorized"**
**Solution:** Check Authorization header format: "Bearer {token}"

---

## ?? **Phase 2 Complete!**

All user profile management endpoints are implemented and ready for Flutter integration!

**Base URL:** `https://localhost:7228`  
**Swagger UI:** `https://localhost:7228/swagger`

---

**Last Updated:** December 18, 2024  
**Status:** ? Phase 2 Complete - User Profile APIs Ready
