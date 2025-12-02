# Flutter Integration Guide - MediAI Backend

## ?? Complete Setup Guide for Flutter Frontend

This guide will help you integrate the MediAI Backend API with your Flutter application.

---

## ?? Required Flutter Packages

Add these packages to your `pubspec.yaml`:

```yaml
dependencies:
  flutter:
    sdk: flutter
  
  # HTTP Client
  http: ^1.1.0
  
  # Secure Storage for Token
  flutter_secure_storage: ^9.0.0
  
  # State Management (Choose one)
  provider: ^6.1.1
  # OR
  riverpod: ^2.4.9
  # OR
  bloc: ^8.1.3
  
  # JSON Serialization
  json_annotation: ^4.8.1
  
dev_dependencies:
  build_runner: ^2.4.7
  json_serializable: ^6.7.1
```

---

## ?? Step 1: Configure Base URL

Create `lib/config/api_config.dart`:

```dart
class ApiConfig {
  // For Android Emulator
  static const String baseUrl = 'http://10.0.2.2:5000';
  
  // For iOS Simulator
  // static const String baseUrl = 'http://localhost:5000';
  
  // For Real Device (use your computer's IP)
  // static const String baseUrl = 'http://192.168.1.100:5000';
  
  // For Production
  // static const String baseUrl = 'https://your-domain.com';
  
  // API Endpoints
  static const String authBase = '$baseUrl/api/Auth';
  static const String doctorsBase = '$baseUrl/api/Doctors';
  static const String appointmentsBase = '$baseUrl/api/Appointments';
  static const String remindersBase = '$baseUrl/api/MedicineReminders';
}
```

### ?? Finding Your Computer's IP Address:

**Windows:**
```powershell
ipconfig
```
Look for "IPv4 Address" under your active network adapter.

**Mac/Linux:**
```bash
ifconfig
```
Look for "inet" under your active network interface.

---

## ?? Step 2: Create Token Storage Service

Create `lib/services/storage_service.dart`:

```dart
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class StorageService {
  static const _storage = FlutterSecureStorage();
  static const _tokenKey = 'auth_token';
  static const _userKey = 'user_data';

  // Save token
  static Future<void> saveToken(String token) async {
    await _storage.write(key: _tokenKey, value: token);
  }

  // Get token
  static Future<String?> getToken() async {
    return await _storage.read(key: _tokenKey);
  }

  // Delete token
  static Future<void> deleteToken() async {
    await _storage.delete(key: _tokenKey);
  }

  // Save user data
  static Future<void> saveUser(String userData) async {
    await _storage.write(key: _userKey, value: userData);
  }

  // Get user data
  static Future<String?> getUser() async {
    return await _storage.read(key: _userKey);
  }

  // Clear all data
  static Future<void> clearAll() async {
    await _storage.deleteAll();
  }

  // Check if user is logged in
  static Future<bool> isLoggedIn() async {
    final token = await getToken();
    return token != null && token.isNotEmpty;
  }
}
```

---

## ?? Step 3: Create Data Models

### User Model (`lib/models/user_model.dart`):

```dart
import 'package:json_annotation/json_annotation.dart';

part 'user_model.g.dart';

@JsonSerializable()
class User {
  final int id;
  final String email;
  final String fullName;
  final String role;
  final String? phoneNumber;
  final String? dateOfBirth;
  final String? gender;
  final String? address;
  final bool? isEmailVerified;
  final bool? isActive;
  final String? createdAt;
  final String? lastLoginAt;

  User({
    required this.id,
    required this.email,
    required this.fullName,
    required this.role,
    this.phoneNumber,
    this.dateOfBirth,
    this.gender,
    this.address,
    this.isEmailVerified,
    this.isActive,
    this.createdAt,
    this.lastLoginAt,
  });

  factory User.fromJson(Map<String, dynamic> json) => _$UserFromJson(json);
  Map<String, dynamic> toJson() => _$UserToJson(this);
}
```

### Auth Response Model (`lib/models/auth_response_model.dart`):

```dart
import 'package:json_annotation/json_annotation.dart';
import 'user_model.dart';

part 'auth_response_model.g.dart';

@JsonSerializable()
class AuthResponse {
  final String message;
  final String? token;
  final User? user;

  AuthResponse({
    required this.message,
    this.token,
    this.user,
  });

  factory AuthResponse.fromJson(Map<String, dynamic> json) => 
      _$AuthResponseFromJson(json);
  Map<String, dynamic> toJson() => _$AuthResponseToJson(this);
}
```

**Run code generation:**
```bash
flutter pub run build_runner build
```

---

## ?? Step 4: Create API Service

### Auth Service (`lib/services/auth_service.dart`):

```dart
import 'dart:convert';
import 'package:http/http.dart' as http;
import '../config/api_config.dart';
import '../models/auth_response_model.dart';
import '../models/user_model.dart';
import 'storage_service.dart';

class AuthService {
  // Register
  static Future<Map<String, dynamic>> register({
    required String email,
    required String password,
    required String fullName,
    String? phoneNumber,
    String? dateOfBirth,
    String? gender,
    String? address,
  }) async {
    try {
      final response = await http.post(
        Uri.parse('${ApiConfig.authBase}/register'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'email': email,
          'password': password,
          'fullName': fullName,
          'role': 'patient',
          'phoneNumber': phoneNumber,
          'dateOfBirth': dateOfBirth,
          'gender': gender,
          'address': address,
        }),
      );

      final data = jsonDecode(response.body);

      if (response.statusCode == 200) {
        return {'success': true, 'message': data['message']};
      } else {
        return {'success': false, 'message': data['message'] ?? 'Registration failed'};
      }
    } catch (e) {
      return {'success': false, 'message': 'Network error: $e'};
    }
  }

  // Verify OTP
  static Future<Map<String, dynamic>> verifyOtp({
    required String email,
    required String otp,
  }) async {
    try {
      final response = await http.post(
        Uri.parse('${ApiConfig.authBase}/verify-otp'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'email': email,
          'otp': otp,
        }),
      );

      final data = jsonDecode(response.body);

      if (response.statusCode == 200) {
        // Save token and user data
        await StorageService.saveToken(data['token']);
        await StorageService.saveUser(jsonEncode(data['user']));

        return {
          'success': true,
          'message': data['message'],
          'user': User.fromJson(data['user']),
        };
      } else {
        return {'success': false, 'message': data['message'] ?? 'Verification failed'};
      }
    } catch (e) {
      return {'success': false, 'message': 'Network error: $e'};
    }
  }

  // Login
  static Future<Map<String, dynamic>> login({
    required String email,
    required String password,
  }) async {
    try {
      final response = await http.post(
        Uri.parse('${ApiConfig.authBase}/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'email': email,
          'password': password,
        }),
      );

      final data = jsonDecode(response.body);

      if (response.statusCode == 200) {
        // Save token and user data
        await StorageService.saveToken(data['token']);
        await StorageService.saveUser(jsonEncode(data['user']));

        return {
          'success': true,
          'message': data['message'],
          'user': User.fromJson(data['user']),
        };
      } else {
        return {'success': false, 'message': data['message'] ?? 'Login failed'};
      }
    } catch (e) {
      return {'success': false, 'message': 'Network error: $e'};
    }
  }

  // Get Current User
  static Future<User?> getCurrentUser() async {
    try {
      final token = await StorageService.getToken();
      if (token == null) return null;

      final response = await http.get(
        Uri.parse('${ApiConfig.authBase}/current-user'),
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        return User.fromJson(data);
      }
      return null;
    } catch (e) {
      print('Error getting current user: $e');
      return null;
    }
  }

  // Logout
  static Future<void> logout() async {
    await StorageService.clearAll();
  }
}
```

---

## ?? Step 5: Example UI Screens

### Register Screen (`lib/screens/register_screen.dart`):

```dart
import 'package:flutter/material.dart';
import '../services/auth_service.dart';

class RegisterScreen extends StatefulWidget {
  @override
  _RegisterScreenState createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _fullNameController = TextEditingController();
  final _phoneController = TextEditingController();
  
  bool _isLoading = false;

  Future<void> _register() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    final result = await AuthService.register(
      email: _emailController.text.trim(),
      password: _passwordController.text,
      fullName: _fullNameController.text.trim(),
      phoneNumber: _phoneController.text.trim(),
    );

    setState(() => _isLoading = false);

    if (result['success']) {
      // Navigate to OTP verification screen
      Navigator.pushNamed(
        context,
        '/verify-otp',
        arguments: _emailController.text.trim(),
      );
      
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result['message']), backgroundColor: Colors.green),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result['message']), backgroundColor: Colors.red),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Register')),
      body: Padding(
        padding: EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: ListView(
            children: [
              // Full Name
              TextFormField(
                controller: _fullNameController,
                decoration: InputDecoration(
                  labelText: 'Full Name',
                  prefixIcon: Icon(Icons.person),
                ),
                validator: (value) =>
                    value?.isEmpty ?? true ? 'Please enter your name' : null,
              ),
              SizedBox(height: 16),
              
              // Email
              TextFormField(
                controller: _emailController,
                decoration: InputDecoration(
                  labelText: 'Email',
                  prefixIcon: Icon(Icons.email),
                ),
                keyboardType: TextInputType.emailAddress,
                validator: (value) {
                  if (value?.isEmpty ?? true) return 'Please enter email';
                  if (!value!.contains('@')) return 'Invalid email';
                  return null;
                },
              ),
              SizedBox(height: 16),
              
              // Phone
              TextFormField(
                controller: _phoneController,
                decoration: InputDecoration(
                  labelText: 'Phone Number',
                  prefixIcon: Icon(Icons.phone),
                ),
                keyboardType: TextInputType.phone,
              ),
              SizedBox(height: 16),
              
              // Password
              TextFormField(
                controller: _passwordController,
                decoration: InputDecoration(
                  labelText: 'Password',
                  prefixIcon: Icon(Icons.lock),
                ),
                obscureText: true,
                validator: (value) {
                  if (value?.isEmpty ?? true) return 'Please enter password';
                  if (value!.length < 6) return 'Password too short';
                  return null;
                },
              ),
              SizedBox(height: 24),
              
              // Register Button
              ElevatedButton(
                onPressed: _isLoading ? null : _register,
                style: ElevatedButton.styleFrom(
                  padding: EdgeInsets.symmetric(vertical: 16),
                ),
                child: _isLoading
                    ? CircularProgressIndicator(color: Colors.white)
                    : Text('Register', style: TextStyle(fontSize: 18)),
              ),
              
              // Login Link
              TextButton(
                onPressed: () => Navigator.pop(context),
                child: Text('Already have an account? Login'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
```

### OTP Verification Screen (`lib/screens/verify_otp_screen.dart`):

```dart
import 'package:flutter/material.dart';
import '../services/auth_service.dart';

class VerifyOtpScreen extends StatefulWidget {
  final String email;

  VerifyOtpScreen({required this.email});

  @override
  _VerifyOtpScreenState createState() => _VerifyOtpScreenState();
}

class _VerifyOtpScreenState extends State<VerifyOtpScreen> {
  final _otpController = TextEditingController();
  bool _isLoading = false;

  Future<void> _verifyOtp() async {
    if (_otpController.text.length != 6) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Please enter 6-digit OTP')),
      );
      return;
    }

    setState(() => _isLoading = true);

    final result = await AuthService.verifyOtp(
      email: widget.email,
      otp: _otpController.text,
    );

    setState(() => _isLoading = false);

    if (result['success']) {
      // Navigate to home screen
      Navigator.pushNamedAndRemoveUntil(context, '/home', (route) => false);
      
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result['message']), backgroundColor: Colors.green),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result['message']), backgroundColor: Colors.red),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Verify OTP')),
      body: Padding(
        padding: EdgeInsets.all(16.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.email, size: 80, color: Colors.blue),
            SizedBox(height: 24),
            Text(
              'Enter OTP sent to',
              style: TextStyle(fontSize: 18),
            ),
            Text(
              widget.email,
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 32),
            
            TextField(
              controller: _otpController,
              decoration: InputDecoration(
                labelText: 'OTP',
                hintText: '123456',
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.number,
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 24, letterSpacing: 8),
              maxLength: 6,
            ),
            SizedBox(height: 24),
            
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: _isLoading ? null : _verifyOtp,
                style: ElevatedButton.styleFrom(
                  padding: EdgeInsets.symmetric(vertical: 16),
                ),
                child: _isLoading
                    ? CircularProgressIndicator(color: Colors.white)
                    : Text('Verify', style: TextStyle(fontSize: 18)),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
```

### Login Screen (`lib/screens/login_screen.dart`):

```dart
import 'package:flutter/material.dart';
import '../services/auth_service.dart';

class LoginScreen extends StatefulWidget {
  @override
  _LoginScreenState createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  bool _isLoading = false;

  Future<void> _login() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    final result = await AuthService.login(
      email: _emailController.text.trim(),
      password: _passwordController.text,
    );

    setState(() => _isLoading = false);

    if (result['success']) {
      Navigator.pushNamedAndRemoveUntil(context, '/home', (route) => false);
      
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result['message']), backgroundColor: Colors.green),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result['message']), backgroundColor: Colors.red),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Login')),
      body: Padding(
        padding: EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              TextFormField(
                controller: _emailController,
                decoration: InputDecoration(
                  labelText: 'Email',
                  prefixIcon: Icon(Icons.email),
                ),
                keyboardType: TextInputType.emailAddress,
                validator: (value) =>
                    value?.isEmpty ?? true ? 'Please enter email' : null,
              ),
              SizedBox(height: 16),
              
              TextFormField(
                controller: _passwordController,
                decoration: InputDecoration(
                  labelText: 'Password',
                  prefixIcon: Icon(Icons.lock),
                ),
                obscureText: true,
                validator: (value) =>
                    value?.isEmpty ?? true ? 'Please enter password' : null,
              ),
              SizedBox(height: 24),
              
              SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  onPressed: _isLoading ? null : _login,
                  style: ElevatedButton.styleFrom(
                    padding: EdgeInsets.symmetric(vertical: 16),
                  ),
                  child: _isLoading
                      ? CircularProgressIndicator(color: Colors.white)
                      : Text('Login', style: TextStyle(fontSize: 18)),
                ),
              ),
              
              TextButton(
                onPressed: () => Navigator.pushNamed(context, '/register'),
                child: Text('Don\'t have an account? Register'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
```

---

## ?? Step 6: Testing Connection

### Health Check Widget:

```dart
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import '../config/api_config.dart';

class ConnectionTest extends StatefulWidget {
  @override
  _ConnectionTestState createState() => _ConnectionTestState();
}

class _ConnectionTestState extends State<ConnectionTest> {
  String _status = 'Not tested';
  bool _isLoading = false;

  Future<void> _testConnection() async {
    setState(() {
      _isLoading = true;
      _status = 'Testing...';
    });

    try {
      final response = await http.get(
        Uri.parse('${ApiConfig.authBase}/health'),
      ).timeout(Duration(seconds: 5));

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        setState(() {
          _status = '? Connected!\n${data['message']}';
          _isLoading = false;
        });
      } else {
        setState(() {
          _status = '? Failed: ${response.statusCode}';
          _isLoading = false;
        });
      }
    } catch (e) {
      setState(() {
        _status = '? Error: $e';
        _isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        ElevatedButton(
          onPressed: _isLoading ? null : _testConnection,
          child: Text('Test API Connection'),
        ),
        SizedBox(height: 16),
        Text(_status, textAlign: TextAlign.center),
      ],
    );
  }
}
```

---

## ?? Quick Start Checklist

- [ ] Add required packages to `pubspec.yaml`
- [ ] Run `flutter pub get`
- [ ] Create `api_config.dart` with correct base URL
- [ ] Create `storage_service.dart` for token management
- [ ] Create data models and run `build_runner`
- [ ] Create `auth_service.dart`
- [ ] Create UI screens (register, OTP, login)
- [ ] Test connection with backend
- [ ] Start backend: `dotnet run --project Backend-APIs`
- [ ] Run Flutter app: `flutter run`

---

## ?? Testing on Different Devices

### Android Emulator
```dart
static const String baseUrl = 'http://10.0.2.2:5000';
```

### iOS Simulator
```dart
static const String baseUrl = 'http://localhost:5000';
```

### Real Device (Same WiFi)
1. Get your computer's IP address
2. Update base URL:
```dart
static const String baseUrl = 'http://YOUR_IP:5000';
```
3. Ensure Windows Firewall allows port 5000

---

## ?? Security Best Practices

1. **Never** commit API keys or tokens to version control
2. Use `flutter_secure_storage` for sensitive data
3. Implement token refresh mechanism
4. Handle token expiration gracefully
5. Use HTTPS in production
6. Implement certificate pinning for production

---

## ?? Common Issues & Solutions

### Issue: "Connection refused"
**Solution:** Check base URL, ensure backend is running, verify IP address

### Issue: "OTP not received"
**Solution:** Check backend console output (development mode)

### Issue: "401 Unauthorized"
**Solution:** Token expired or invalid, re-login required

### Issue: "CORS error"
**Solution:** CORS is already configured in backend, check backend logs

---

## ?? Next Steps

1. ? Test authentication flow
2. Create doctor listing screen
3. Create appointment booking screen
4. Create medicine reminder screen
5. Add push notifications
6. Implement offline support
7. Add image upload functionality

---

## ?? Pro Tips

- Use `dio` package instead of `http` for better features (interceptors, timeout, etc.)
- Implement a centralized API client class
- Use `freezed` package for immutable models
- Implement proper error handling with custom exceptions
- Add loading states and error states to UI
- Use state management (Provider, Riverpod, or Bloc)
- Implement pull-to-refresh for data lists
- Add pagination for large data sets

---

## ?? You're Ready!

Your backend is fully configured and ready to be consumed by Flutter. Start with the authentication flow, then gradually add more features!

For complete API reference, see `API_DOCUMENTATION.md`
