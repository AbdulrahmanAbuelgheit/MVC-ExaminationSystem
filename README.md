# 🎓 Comprehensive Education Management System

A full-featured examination platform built with ASP.NET Core 9.0 that streamlines academic operations from student enrollment to exam results, featuring a sophisticated multi-level management system.

## ✨ Features

### 🔐 Advanced Role-Based System
- **Students**: Exam taking, course enrollment, grade tracking
- **Instructors**: Course management, exam creation, question authoring
- **Track Supervisors**: Instructor management within tracks
- **Branch Managers**: Track oversight, student assignment, resource allocation
- **System Admins**: Complete system administration

### 📚 Core Functionality
- **Course Management**: Complete CRUD operations with instructor assignment
- **Exam System**: Automated exam generation, real-time taking, and scoring
- **Track Supervision**: Instructors can be promoted to supervise specific tracks
- **Branch Management**: Instructors can manage entire branches with multiple tracks
- **Student Assignment**: Dynamic track and course enrollment
- **Real-time Features**: Live exam countdowns and status updates
- **Comprehensive Reports**: Academic performance analytics

### 🎨 User Experience
- Role-specific dashboards with hierarchical permissions
- Modern card-based layouts with hover effects
- Responsive design optimized for all devices
- Color-coded grade systems (A+ to F)
- Professional educational theme with smooth animations

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 9.0 (MVC)
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: JWT Authentication & Authorization
- **Object Mapping**: AutoMapper
- **Frontend**: Bootstrap 5 with custom CSS
- **Password Hashing**: BCrypt.Net
- **Reporting**: Microsoft Reporting Services
- **Validation**: jQuery Validation

## 🔑 Key Components

### Authentication & Authorization
- JWT-based authentication with cookie storage
- Role-based access control
- Secure password hashing with BCrypt

### Database Architecture
- Entity Framework Core with Code-First approach
- Lazy loading for optimized performance
- Repository pattern with Unit of Work

### Exam Management
- Automated exam generation from question pools
- Real-time exam taking with countdown timers
- Automatic scoring and grade calculation

### Hierarchical Management
- Branch → Track → Course structure
- Multi-role instructors (Teacher, Supervisor, Manager)
- Dynamic permission system


## 🚀 Getting Started

### 1. Clone the Repository
git clone https://github.com/AbdulrahmanAbuelgheit/MVC-ExaminationSystem.git cd MVC-ExaminationSystem

### 2. Database Setup
Update the connection string in appsettings.json
Run Entity Framework migrations

### 3. Run the Application




## 🎯 Usage Examples

### Creating an Exam
1. Login as an Instructor
2. Navigate to "Generate Exam"
3. Select course and configure parameters
4. System automatically selects questions from pool
5. Set exam schedule and duration

### Taking an Exam
1. Login as a Student
2. View available exams on dashboard
3. Click "Take Exam Now" when available
4. Complete within time limit
5. View results immediately after submission

### Managing Students (Branch Manager)
1. Access branch management dashboard
2. Assign students to tracks
3. Monitor performance across courses
4. Generate comprehensive reports
