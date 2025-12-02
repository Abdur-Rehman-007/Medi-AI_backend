# ?? GitHub Setup Guide

## Step-by-Step Instructions to Push Your Code to GitHub

### ? Prerequisites
- Git installed on your system
- GitHub account created
- Repository created at: https://github.com/Abdur-Rehman-007/Medi-AI_backend

---

## ?? Quick Setup (Run These Commands)

### 1. Open Terminal in Project Root
```bash
cd "F:\Last day project\Medi-AI Backend\Backend-APIs"
```

### 2. Initialize Git Repository (if not done)
```bash
git init
```

### 3. Add All Files
```bash
git add .
```

### 4. Create Initial Commit
```bash
git commit -m "Initial commit: Complete MediAI Backend with Auth, Doctors, Appointments, and Medicine Reminders"
```

### 5. Add Remote Repository
```bash
git remote add origin https://github.com/Abdur-Rehman-007/Medi-AI_backend.git
```

### 6. Rename Branch to Main
```bash
git branch -M main
```

### 7. Push to GitHub
```bash
git push -u origin main
```

---

## ?? If Authentication is Required

### Option 1: Use Personal Access Token (Recommended)

1. **Generate Token on GitHub:**
   - Go to: https://github.com/settings/tokens
   - Click "Generate new token (classic)"
   - Select scopes: `repo` (full control)
   - Copy the token (save it securely!)

2. **Use Token When Pushing:**
   ```bash
   git push -u origin main
   # Username: Abdur-Rehman-007
   # Password: <paste your token here>
   ```

### Option 2: Use GitHub CLI

```bash
# Install GitHub CLI
# Windows: winget install --id GitHub.cli

# Authenticate
gh auth login

# Push
git push -u origin main
```

### Option 3: Use SSH (Advanced)

1. **Generate SSH Key:**
   ```bash
   ssh-keygen -t ed25519 -C "your-email@example.com"
   ```

2. **Add SSH Key to GitHub:**
   - Copy public key: `cat ~/.ssh/id_ed25519.pub`
   - Add at: https://github.com/settings/keys

3. **Change Remote to SSH:**
   ```bash
   git remote set-url origin git@github.com:Abdur-Rehman-007/Medi-AI_backend.git
   git push -u origin main
   ```

---

## ? Verify Upload

After pushing, verify on GitHub:
1. Go to: https://github.com/Abdur-Rehman-007/Medi-AI_backend
2. Check that all files are visible
3. Verify README.md is displayed on the homepage

---

## ?? What Gets Pushed

? **Included:**
- Source code (`.cs` files)
- Controllers, Models, DTOs, Services
- Configuration files (`appsettings.json`)
- Documentation (`.md` files)
- Project file (`.csproj`)
- `.gitignore` file

? **Excluded (by .gitignore):**
- `bin/` and `obj/` folders
- `.vs/` folder
- NuGet packages
- User-specific files
- Build artifacts
- Sensitive data in `appsettings.*.json` (except template)

---

## ?? Future Updates

### Update Code
```bash
# Make changes to your code

# Stage changes
git add .

# Commit
git commit -m "Add new feature or fix bug"

# Push
git push origin main
```

### Pull Latest Changes
```bash
git pull origin main
```

---

## ?? Customize README

Before pushing, you may want to customize:

1. **Repository Description** (on GitHub)
   - Go to repository settings
   - Add description: "Healthcare management system backend with .NET 8 and MySQL"
   - Add topics: `dotnet`, `csharp`, `mysql`, `healthcare`, `rest-api`, `jwt`, `swagger`

2. **Add License**
   - GitHub ? Add file ? Create new file
   - Name: `LICENSE`
   - Choose: MIT License

3. **Add Screenshots** (optional)
   - Create `screenshots/` folder
   - Add Swagger UI screenshots
   - Reference in README.md

---

## ?? Common Issues

### Issue: "fatal: remote origin already exists"
**Solution:**
```bash
git remote remove origin
git remote add origin https://github.com/Abdur-Rehman-007/Medi-AI_backend.git
```

### Issue: "! [rejected] main -> main (non-fast-forward)"
**Solution:**
```bash
# If you're sure you want to overwrite remote
git push -f origin main

# Or merge remote changes first
git pull origin main --allow-unrelated-histories
git push origin main
```

### Issue: "Permission denied (publickey)"
**Solution:**
Use Personal Access Token or set up SSH keys (see above)

### Issue: Large files warning
**Solution:**
Check `.gitignore` is working:
```bash
git status
# Should not show bin/, obj/, or .vs/ folders
```

---

## ?? Repository Statistics

After setup, you can add badges to README.md:

```markdown
![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)
```

---

## ?? Success Checklist

- [ ] Git initialized
- [ ] All files added and committed
- [ ] Remote repository added
- [ ] Code pushed to main branch
- [ ] README.md visible on GitHub
- [ ] Repository description added
- [ ] Topics/tags added
- [ ] License file added (optional)

---

## ?? Need Help?

- GitHub Docs: https://docs.github.com/
- Git Documentation: https://git-scm.com/doc
- GitHub Support: https://support.github.com/

---

**You're all set! Your MediAI Backend is now on GitHub! ??**
