# ChatApp API

A real-time chat application backend built with ASP.NET Core 9.0, featuring JWT authentication, SignalR for real-time communication, and PostgreSQL database integration.

## 🚀 Features

- **Real-time Chat**: SignalR-powered instant messaging
- **JWT Authentication**: Secure user authentication and authorization
- **Message Reactions**: Emoji reactions on messages
- **User Management**: User registration, login, and online status
- **PostgreSQL Database**: Robust data persistence with Entity Framework Core
- **RESTful API**: Clean API endpoints for frontend integration
- **Swagger Documentation**: Interactive API documentation
- **CORS Support**: Cross-origin resource sharing configuration

## 🛠️ Tech Stack

- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM for database operations
- **PostgreSQL** - Primary database (Supabase)
- **SignalR** - Real-time communication
- **JWT Bearer** - Authentication tokens
- **Swagger/OpenAPI** - API documentation
- **BCrypt** - Password hashing

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/) or [Supabase](https://supabase.com/) account
- [Entity Framework Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

## 🔧 Installation

1. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd ChatApp.Api
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Install Entity Framework tools** (if not already installed)
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Set up environment variables**
   ```bash
   cp .env.example .env
   ```
   Edit `.env` with your actual configuration values.

5. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

6. **Start the application**
   ```bash
   dotnet run
   ```

## 🔐 Environment Configuration

Create a `.env` file in the root directory with the following variables:

```env
# Database Configuration
CONNECTION_STRING="Host=your-host.supabase.co;Database=postgres;Username=postgres;Password=your-password;Port=5432;SSL Mode=Require;Trust Server Certificate=true"

# JWT Configuration
JWT_KEY="your-super-secret-jwt-key-make-it-long-and-secure-at-least-32-characters"

# Application Configuration
ASPNETCORE_ENVIRONMENT="Development"
ASPNETCORE_URLS="http://localhost:5000"

# CORS Configuration
ALLOWED_ORIGINS="http://localhost:3000,https://yourdomain.com"
```

## 📚 API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login

### Messages
- `GET /api/messages` - Get all messages
- `POST /api/messages` - Send a message

### SignalR Hub
- `/chathub` - Real-time chat hub

## 🔌 SignalR Events

### Client to Server
- `SendMessage(content)` - Send a chat message
- `AddReaction(messageId, emoji)` - Add reaction to message
- `RemoveReaction(messageId, emoji)` - Remove reaction from message

### Server to Client
- `ReceiveMessage(message)` - New message received
- `UserConnected(username)` - User joined chat
- `UserDisconnected(username)` - User left chat
- `CurrentUsers(users[])` - List of connected users
- `ReactionAdded(data)` - Reaction added to message
- `ReactionRemoved(data)` - Reaction removed from message

## 🗄️ Database Schema

### Users Table
- `Id` (Primary Key)
- `Username` (Unique)
- `Email` (Unique)
- `PasswordHash`
- `CreatedAt`
- `IsOnline`

### Messages Table
- `Id` (Primary Key)
- `Content`
- `CreatedAt`
- `UserId` (Foreign Key)

### MessageReactions Table
- `Id` (Primary Key)
- `MessageId` (Foreign Key)
- `UserId` (Foreign Key)
- `Emoji`
- `CreatedAt`

## 🚀 Deployment

### Production Build
```bash
dotnet publish -c Release -o ./publish
```

### Docker Deployment
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ChatApp.Api.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatApp.Api.dll"]
```

## 🧪 Testing

### Run Tests
```bash
dotnet test
```

### API Testing
Visit `https://localhost:5001/swagger` for interactive API documentation.

## 📝 Development

### Database Migrations
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

### Code Structure
```
ChatApp.Api/
├── Controllers/     # API controllers
├── Data/           # Database context
├── DTOs/           # Data transfer objects
├── Hubs/           # SignalR hubs
├── Models/         # Entity models
├── Services/       # Business logic services
└── Program.cs      # Application entry point
```

## 🔒 Security Features

- **JWT Authentication** - Secure token-based authentication
- **Password Hashing** - BCrypt for secure password storage
- **CORS Configuration** - Controlled cross-origin access
- **Input Validation** - Data validation and sanitization
- **HTTPS Support** - Secure communication

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For support, email your-email@example.com or create an issue in the repository.

## 🔄 Version History

- **v1.0.0** - Initial release with basic chat functionality
- **v1.1.0** - Added message reactions
- **v1.2.0** - Enhanced authentication and user management
