# ?? Authentication API - Password Reset & OTP Management

## ? New Endpoints Added - Phase 1 Complete

Three critical authentication endpoints have been implemented to complete the authentication flow.

---

## ?? **Standard Response Format**

All endpoints return:
```json
{
  "success": true/false,
  "message": "Description message",
  "data": null
}
```

---

## ?? **1. FORGOT PASSWORD**

### **Endpoint:** `POST /api/Auth/forgot-password`
**Authentication:** ? Not Required

Request a password reset code to be sent via email.

### **Request Body:**
```json
{
  "email": "user@example.com"
}
```

### **Field Details:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `email` | string | ? Yes | User's registered email address |

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Password reset code sent to your email",
  "data": null
}
```

**Note:** For security reasons, the same response is returned even if the email doesn't exist in the system.

### **Backend Behavior:**
1. Checks if user exists with provided email
2. Generates 6-digit reset code
3. Stores code in `passwordresettokens` table
4. Code expires in 10 minutes
5. Sends code via email
6. Invalidates any previous unused reset codes

### **Error Response (400 Bad Request):**
```json
{
  "success": false,
  "message": "Failed to process request: [error details]",
  "data": null
}
```

---

## ?? **2. RESET PASSWORD**

### **Endpoint:** `POST /api/Auth/reset-password`
**Authentication:** ? Not Required

Reset password using the code received via email.

### **Request Body:**
```json
{
  "email": "user@example.com",
  "token": "123456",
  "newPassword": "NewSecurePass@123"
}
```

### **Field Details:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `email` | string | ? Yes | User's registered email |
| `token` | string | ? Yes | 6-digit code from email |
| `newPassword` | string | ? Yes | New password (min 6 chars, recommended strong) |

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Password reset successful. You can now login with your new password",
  "data": null
}
```

### **Backend Behavior:**
1. Validates email and token combination
2. Checks if token has expired (10 minutes)
3. Hashes new password with BCrypt
4. Updates user's password in database
5. Marks reset token as used
6. Updates user's `UpdatedAt` timestamp

### **Error Responses:**

**Invalid Token (400):**
```json
{
  "success": false,
  "message": "Invalid or expired reset token",
  "data": null
}
```

**Expired Token (400):**
```json
{
  "success": false,
  "message": "Reset token has expired",
  "data": null
}
```

**Invalid Email/Token Combination (400):**
```json
{
  "success": false,
  "message": "Invalid email or token",
  "data": null
}
```

---

## ?? **3. RESEND OTP**

### **Endpoint:** `POST /api/Auth/resend-otp`
**Authentication:** ? Not Required

Resend email verification OTP to user who hasn't verified their email yet.

### **Request Body:**
```json
{
  "email": "user@example.com"
}
```

### **Field Details:**
| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `email` | string | ? Yes | User's registered email address |

### **Success Response (200 OK):**
```json
{
  "success": true,
  "message": "OTP has been resent to your email",
  "data": null
}
```

### **Backend Behavior:**
1. Checks if user exists
2. Verifies email is not already verified
3. Marks all previous unused OTPs as used
4. Generates new 6-digit OTP
5. OTP expires in 10 minutes
6. Sends new OTP via email

### **Error Responses:**

**User Not Found (400):**
```json
{
  "success": false,
  "message": "User not found",
  "data": null
}
```

**Already Verified (400):**
```json
{
  "success": false,
  "message": "Email is already verified",
  "data": null
}
```

---

## ?? **Complete Authentication Flows**

### **Flow 1: New User Registration**
```
1. POST /api/Auth/register
   ?
2. Check email for OTP (or console in dev mode)
   ?
3. POST /api/Auth/verify-otp
   ?
4. Receive JWT token
   ?
5. Use token for authenticated requests
```

### **Flow 2: Forgot Password**
```
1. POST /api/Auth/forgot-password
   ?
2. Check email for reset code
   ?
3. POST /api/Auth/reset-password (with code)
   ?
4. POST /api/Auth/login (with new password)
   ?
5. Receive JWT token
```

### **Flow 3: Resend OTP (Registration)**
```
1. POST /api/Auth/register (initial registration)
   ?
2. Didn't receive OTP
   ?
3. POST /api/Auth/resend-otp
   ?
4. Check email for new OTP
   ?
5. POST /api/Auth/verify-otp
   ?
6. Receive JWT token
```

---

## ?? **Flutter Integration Examples**

### **Example 1: Forgot Password**

```dart
Future<void> forgotPassword(String email) async {
  try {
    final response = await dio.post('/api/Auth/forgot-password', data: {
      'email': email,
    });

    if (response.data['success']) {
      // Show success message
      showSnackbar(response.data['message']);
      
      // Navigate to reset password screen
      Navigator.push(
        context,
        MaterialPageRoute(
          builder: (context) => ResetPasswordScreen(email: email),
        ),
      );
    }
  } catch (e) {
    showErrorDialog('Failed to send reset code: $e');
  }
}
```

### **Example 2: Reset Password**

```dart
Future<void> resetPassword({
  required String email,
  required String token,
  required String newPassword,
}) async {
  try {
    final response = await dio.post('/api/Auth/reset-password', data: {
      'email': email,
      'token': token,
      'newPassword': newPassword,
    });

    if (response.data['success']) {
      // Show success message
      showSnackbar(response.data['message']);
      
      // Navigate to login screen
      Navigator.pushAndRemoveUntil(
        context,
        MaterialPageRoute(builder: (context) => LoginScreen()),
        (route) => false,
      );
    }
  } catch (e) {
    showErrorDialog('Failed to reset password: $e');
  }
}
```

### **Example 3: Resend OTP**

```dart
Future<void> resendOtp(String email) async {
  try {
    final response = await dio.post('/api/Auth/resend-otp', data: {
      'email': email,
    });

    if (response.data['success']) {
      // Show success message
      showSnackbar(response.data['message']);
      
      // Reset timer for OTP expiry
      startOtpExpiryTimer();
    }
  } catch (e) {
    final errorMessage = e is DioError && e.response != null
        ? e.response!.data['message']
        : 'Failed to resend OTP';
    showErrorDialog(errorMessage);
  }
}
```

---

## ?? **Complete Flutter Screens**

### **Forgot Password Screen**

```dart
class ForgotPasswordScreen extends StatefulWidget {
  @override
  _ForgotPasswordScreenState createState() => _ForgotPasswordScreenState();
}

class _ForgotPasswordScreenState extends State<ForgotPasswordScreen> {
  final _emailController = TextEditingController();
  bool _isLoading = false;

  Future<void> _sendResetCode() async {
    if (_emailController.text.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Please enter your email')),
      );
      return;
    }

    setState(() => _isLoading = true);

    try {
      final response = await dio.post('/api/Auth/forgot-password', data: {
        'email': _emailController.text.trim(),
      });

      if (response.data['success']) {
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => ResetPasswordScreen(
              email: _emailController.text.trim(),
            ),
          ),
        );
      }
    } catch (e) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Failed to send reset code'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Forgot Password')),
      body: Padding(
        padding: EdgeInsets.all(16.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              'Enter your email to receive a password reset code',
              textAlign: TextAlign.center,
              style: TextStyle(fontSize: 16),
            ),
            SizedBox(height: 24),
            TextField(
              controller: _emailController,
              decoration: InputDecoration(
                labelText: 'Email',
                prefixIcon: Icon(Icons.email),
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.emailAddress,
            ),
            SizedBox(height: 24),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: _isLoading ? null : _sendResetCode,
                style: ElevatedButton.styleFrom(
                  padding: EdgeInsets.symmetric(vertical: 16),
                ),
                child: _isLoading
                    ? CircularProgressIndicator(color: Colors.white)
                    : Text('Send Reset Code', style: TextStyle(fontSize: 18)),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
```

### **Reset Password Screen**

```dart
class ResetPasswordScreen extends StatefulWidget {
  final String email;

  ResetPasswordScreen({required this.email});

  @override
  _ResetPasswordScreenState createState() => _ResetPasswordScreenState();
}

class _ResetPasswordScreenState extends State<ResetPasswordScreen> {
  final _tokenController = TextEditingController();
  final _passwordController = TextEditingController();
  bool _isLoading = false;

  Future<void> _resetPassword() async {
    if (_tokenController.text.isEmpty || _passwordController.text.isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Please fill all fields')),
      );
      return;
    }

    setState(() => _isLoading = true);

    try {
      final response = await dio.post('/api/Auth/reset-password', data: {
        'email': widget.email,
        'token': _tokenController.text,
        'newPassword': _passwordController.text,
      });

      if (response.data['success']) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(response.data['message']),
            backgroundColor: Colors.green,
          ),
        );
        
        // Navigate to login
        Navigator.pushAndRemoveUntil(
          context,
          MaterialPageRoute(builder: (context) => LoginScreen()),
          (route) => false,
        );
      }
    } catch (e) {
      final message = e is DioError && e.response != null
          ? e.response!.data['message']
          : 'Failed to reset password';
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(message), backgroundColor: Colors.red),
      );
    } finally {
      setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Reset Password')),
      body: Padding(
        padding: EdgeInsets.all(16.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              'Enter the code sent to ${widget.email}',
              textAlign: TextAlign.center,
            ),
            SizedBox(height: 24),
            TextField(
              controller: _tokenController,
              decoration: InputDecoration(
                labelText: 'Reset Code',
                prefixIcon: Icon(Icons.lock_outline),
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.number,
              maxLength: 6,
            ),
            SizedBox(height: 16),
            TextField(
              controller: _passwordController,
              decoration: InputDecoration(
                labelText: 'New Password',
                prefixIcon: Icon(Icons.lock),
                border: OutlineInputBorder(),
              ),
              obscureText: true,
            ),
            SizedBox(height: 24),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: _isLoading ? null : _resetPassword,
                style: ElevatedButton.styleFrom(
                  padding: EdgeInsets.symmetric(vertical: 16),
                ),
                child: _isLoading
                    ? CircularProgressIndicator(color: Colors.white)
                    : Text('Reset Password', style: TextStyle(fontSize: 18)),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
```

---

## ? **Quick Reference**

| Endpoint | Method | Auth | Purpose |
|----------|--------|------|---------|
| `/api/Auth/forgot-password` | POST | ? | Request password reset code |
| `/api/Auth/reset-password` | POST | ? | Reset password with code |
| `/api/Auth/resend-otp` | POST | ? | Resend email verification OTP |

---

## ?? **Security Features**

### **Password Reset:**
- ? 6-digit random code
- ? 10-minute expiry
- ? Single use (marked as used after reset)
- ? Previous unused tokens invalidated
- ? Password hashed with BCrypt
- ? Generic response for non-existent emails (prevents email enumeration)

### **OTP Resend:**
- ? Only for unverified emails
- ? Previous OTPs invalidated
- ? New OTP with 10-minute expiry
- ? Rate limiting recommended (future enhancement)

---

## ?? **Database Tables Used**

### **passwordresettokens:**
```sql
- Id (int)
- UserId (int) - Foreign key to users
- Token (string) - 6-digit reset code
- ExpiresAt (datetime) - Expiry timestamp
- IsUsed (bool) - Single-use flag
- CreatedAt (datetime)
```

### **emailverificationotps:**
```sql
- Id (int)
- UserId (int) - Foreign key to users
- Otp (string) - 6-digit code
- ExpiresAt (datetime) - Expiry timestamp
- IsUsed (bool) - Single-use flag
- CreatedAt (datetime)
```

---

## ? **Testing Checklist**

- [ ] Test forgot password with valid email
- [ ] Test forgot password with invalid email
- [ ] Test reset password with valid code
- [ ] Test reset password with expired code
- [ ] Test reset password with used code
- [ ] Test reset password with invalid code
- [ ] Test resend OTP for unverified user
- [ ] Test resend OTP for verified user
- [ ] Test resend OTP for non-existent user
- [ ] Verify email delivery in development mode (console)
- [ ] Test complete forgot password flow
- [ ] Test complete registration with OTP resend

---

## ?? **Common Issues & Solutions**

### **Issue: "Reset token has expired"**
**Solution:** Request a new reset code (codes expire in 10 minutes)

### **Issue: "Email is already verified"**
**Solution:** User trying to resend OTP but email is already verified - direct them to login

### **Issue: "Invalid or expired reset token"**
**Solution:** User entered wrong code or code already used - request new code

### **Issue: "OTP not received"**
**Solution:** Check email spam folder, or use resend OTP feature

---

## ?? **Phase 1 Complete!**

All critical authentication endpoints are now implemented and ready for Flutter integration!

**Base URL:** `https://localhost:7228`  
**Swagger UI:** `https://localhost:7228/swagger`

---

**Last Updated:** December 18, 2024  
**Status:** ? Phase 1 Complete - Authentication APIs Ready
