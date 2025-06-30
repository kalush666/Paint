# 🎨 Paint Project – Collaborative Drawing Application

A comprehensive fullstack collaborative drawing application built with WPF, C#, and MongoDB. This project demonstrates modern software architecture patterns including Command Pattern, Factory Pattern, MVVM, and client-server communication using TCP sockets. Users can draw shapes collaboratively, save/load drawings from the database, and manage sketches through an intuitive interface.

## ✨ Key Features

- 🎨 **Collaborative Drawing**: Multiple clients can draw simultaneously
- 🔒 **Concurrency Control**: Lock management prevents conflicts during collaborative editing
- 💾 **Persistent Storage**: All drawings saved to MongoDB with full CRUD operations
- 🏗️ **Modern Architecture**: Clean separation using Command, Factory, and MVVM patterns
- 🌐 **Real-time Communication**: TCP-based client-server communication
- 🎯 **Shape Management**: Draw, select, highlight, and manipulate shapes (Line, Rectangle, Circle)
- 📦 **Import/Export**: Import sketches from database and export current drawings
- ⚙️ **Configuration**: Customizable options through dedicated settings window

---

## 📁 Project Structure

```
PaintProject/
├── PaintProject.sln              # Solution file
├── run.bat                       # One-click launcher script
├── docker-compose.yml           # MongoDB containerization
├── AppErrors.cs                 # Global error definitions
│
├── Client/                      # WPF Drawing Client
│   ├── Client.csproj            # Project configuration
│   ├── App.xaml                 # Application entry point
│   ├── App.xaml.cs
│   ├── AssemblyInfo.cs
│   │
│   ├── Commands/                # Command Pattern Implementation
│   │   ├── IDrawingCommand.cs   # Command interface
│   │   ├── ClearCommand.cs      # Clear canvas command
│   │   ├── ImportCommand.cs     # Import from database
│   │   ├── OptionsCommand.cs    # Open settings
│   │   ├── ShapeSelectionCommand.cs # Shape selection/highlighting
│   │   └── UploadCommand.cs     # Save to database
│   │
│   ├── Enums/
│   │   └── CommandTypes.cs      # Available command types
│   │
│   ├── Factories/               # Factory Pattern Implementation
│   │   ├── DrawingCommandFactory.cs    # Creates drawing commands
│   │   ├── IDrawingCommandFactory.cs   # Factory interface
│   │   ├── ShapeFactory.cs      # Creates shape models
│   │   └── UIShapeFactory.cs    # Creates UI elements
│   │
│   ├── Handlers/
│   │   └── DrawingHandler.cs    # Orchestrates drawing operations
│   │
│   ├── Helpers/
│   │   ├── BrushMappingHelper.cs        # Color/brush utilities
│   │   ├── GeometryHelper.cs    # Shape geometry calculations
│   │   └── ShapeSelectionHighlighter.cs # Visual selection feedback
│   │
│   ├── Mappers/
│   │   └── SketchMapper.cs      # Maps between DTOs and models
│   │
│   ├── Models/                  # Core drawing models
│   │   ├── ShapeBase.cs         # Base shape class
│   │   ├── Line.cs              # Line shape implementation
│   │   ├── Rectangle.cs         # Rectangle shape implementation
│   │   ├── Circle.cs            # Circle shape implementation
│   │   └── Sketch.cs            # Collection of shapes
│   │
│   ├── Services/                # Client services
│   │   ├── ClientCommunicationService.cs # TCP communication
│   │   ├── CommandServiceProvider.cs     # Command resolution
│   │   └── ICommandServiceProvider.cs    # Service interface
│   │
│   ├── UIModels/                # UI-specific models
│   │   ├── UIBaseShape.cs       # Base UI shape
│   │   ├── UILine.cs            # UI line representation
│   │   ├── UIRectangle.cs       # UI rectangle representation
│   │   └── UICircle.cs          # UI circle representation
│   │
│   └── Views/                   # WPF Views
│       ├── MainClientWindow/
│       │   ├── ClientWindow.xaml        # Main drawing interface
│       │   └── ClientWindow.xaml.cs
│       └── Service Windows/
│           ├── Import Selection Window/
│           │   ├── ImportSelectionWindow.xaml   # Sketch selection UI
│           │   └── ImportSelectionWindow.xaml.cs
│           └── Options Window/
│               ├── OptionsWindow.xaml   # Settings UI
│               └── OptionsWindow.xaml.cs
│
├── Common/                      # Shared Library
│   ├── Common.csproj
│   │
│   ├── Constants/
│   │   └── Ports.cs            # Network port definitions
│   │
│   ├── Convertors/
│   │   └── ObjectIdToJsonConvertor.cs  # MongoDB ObjectId serialization
│   │
│   ├── DTO/                    # Data Transfer Objects
│   │   ├── ShapeDto.cs         # Shape data transfer
│   │   └── SketchDto.cs        # Sketch data transfer
│   │
│   ├── Enums/
│   │   └── BasicShapeType.cs   # Shape type enumeration
│   │
│   ├── Errors/
│   │   └── AppErrors.cs        # Application error definitions
│   │
│   ├── Events/
│   │   └── LockHub.cs          # Concurrency control events
│   │
│   ├── Helpers/
│   │   ├── RelayCommand.cs     # MVVM command implementation
│   │   ├── ResponseHelper.cs   # HTTP-like response wrapper
│   │   └── Result.cs           # Result pattern implementation
│   │
│   └── Models/
│       └── Position.cs         # 2D coordinate representation
│
└── Server/                     # TCP Server & Management UI
    ├── Server.csproj
    ├── App.xaml                # Server WPF app entry
    ├── App.xaml.cs
    │
    ├── Config/
    │   └── MongoConfig.cs      # MongoDB connection configuration
    │
    ├── Events/                 # Event-driven architecture
    │   ├── SketchEvent.cs      # Sketch modification events
    │   └── SketchEventBus.cs   # Event broadcasting
    │
    ├── Factories/
    │   ├── IHandlerFactory.cs  # Handler creation interface
    │   └── SketchRequestFactory.cs  # Request handler factory
    │
    ├── Handlers/               # Request processing
    │   ├── IRequestHandler.cs          # Handler interface
    │   ├── IRequestProcessor.cs        # Processor interface
    │   ├── GetAllNamesHandler.cs       # Fetch sketch names
    │   ├── GetAllSketchesHandler.cs    # Fetch all sketches
    │   ├── GetSpecificSketchHandler.cs # Fetch specific sketch
    │   └── UploadSketchHandler.cs      # Save sketch to DB
    │
    ├── Mappers/
    │   └── ServerSketchMappers.cs      # Server-side data mapping
    │
    ├── Models/                 # Server-specific models
    │   ├── ServerBaseShape.cs  # Server shape representation
    │   ├── ServerSketch.cs     # Server sketch model
    │   └── SketchEntry.cs      # Database entity model
    │
    ├── Repositories/
    │   └── MongoSketchStore.cs # MongoDB data access layer
    │
    ├── Services/
    │   ├── LockManager.cs      # Concurrency control service
    │   └── TcpSketchServer.cs  # TCP communication server
    │
    ├── ViewModel/
    │   └── SketchListViewModel.cs      # MVVM view model
    │
    └── Views/
        ├── ServerWindow.xaml   # Server management interface
        └── ServerWindow.xaml.cs
```

---

## 🏗️ Architecture Overview

The application follows a three-tier architecture:

1. **Client Layer (WPF)**: Interactive drawing interface with command-based operations
2. **Server Layer (TCP Server)**: Handles business logic, concurrency, and database operations
3. **Data Layer (MongoDB)**: Persistent storage for sketches and shapes

### Design Patterns Implemented

- **Command Pattern**: All drawing operations (Clear, Upload, Import, Options, Shape Selection)
- **Factory Pattern**: Shape creation and UI element generation
- **MVVM Pattern**: Clean separation of UI logic and business logic
- **Repository Pattern**: Data access abstraction for MongoDB operations
- **Event-Driven Architecture**: Real-time updates and lock management

---

## 🛠️ Technology Stack

| Layer                    | Technology              | Purpose                                           |
| ------------------------ | ----------------------- | ------------------------------------------------- |
| **Frontend**             | WPF (.NET 5.0)          | Rich desktop UI with XAML-based interfaces        |
| **Client Framework**     | CommunityToolkit.MVVM   | Modern MVVM implementation with source generators |
| **Backend**              | C# .NET 5.0             | Server logic and business rules                   |
| **Database**             | MongoDB 6.0             | Document-based storage for sketches and shapes    |
| **Communication**        | TCP Sockets             | Low-latency client-server communication           |
| **Serialization**        | Newtonsoft.Json         | JSON data exchange format                         |
| **Containerization**     | Docker & Docker Compose | MongoDB deployment and environment isolation      |
| **Reactive Programming** | System.Reactive         | Event-driven programming for real-time updates    |
| **Database Driver**      | MongoDB.Driver 3.4.0    | Official .NET MongoDB integration                 |

### Key NuGet Packages

- **CommunityToolkit.Mvvm 8.4.0**: Modern MVVM framework
- **MongoDB.Driver 3.4.0**: MongoDB connectivity and operations
- **Newtonsoft.Json 13.0.3**: JSON serialization/deserialization
- **System.Reactive 6.0.1**: Reactive extensions for .NET

---

## 🚀 Quick Start

### Prerequisites

- **.NET 5.0 SDK** or later
- **Docker Desktop** (for MongoDB)
- **Git** (for cloning)

### One-Click Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/kalush666/PaintProject.git
   cd PaintProject
   ```

2. **Launch everything** (Windows)
   ```cmd
   run.bat
   ```
   This will:
   - 🐳 Start MongoDB container with authentication
   - 🖥️ Launch the server with management UI
   - 🖼️ Open two client instances for testing collaboration

### Manual Setup

If you prefer manual control:

1. **Start MongoDB**

   ```cmd
   docker-compose up -d
   ```

2. **Start the Server**

   ```cmd
   dotnet run --project Server
   ```

3. **Start Client(s)**
   ```cmd
   dotnet run --project Client
   ```

## ⚙️ Configuration

### Network Ports

- **MongoDB**: `27017`
- **TCP Server**: Configured in `Common/Constants/Ports.cs`

---

## 📦 Core Features

### Drawing Capabilities

- **Shape Tools**: Line, Rectangle, Circle with real-time preview
- **Interactive Drawing**: Mouse-based shape creation with visual feedback
- **Shape Selection**: Click to select and highlight shapes
- **Canvas Operations**: Clear entire canvas with single command

### Collaboration Features

- **Multi-Client Support**: Multiple users can draw simultaneously
- **Real-time Synchronization**: Changes appear instantly across all clients
- **Concurrency Control**: Lock management prevents editing conflicts
- **Session Management**: Automatic client connection/disconnection handling

### Data Management

- **Persistent Storage**: All sketches automatically saved to MongoDB
- **Sketch Library**: Browse and load previously saved drawings
- **Import/Export**: Load sketches from database into current session
- **Backup & Recovery**: Robust data persistence with MongoDB

### User Interface

- **Modern WPF Design**: Clean, intuitive drawing interface
- **Command-Based Operations**: Toolbar with dedicated action buttons
- **Settings Panel**: Configurable options and preferences
- **Server Management**: Administrative UI for monitoring connections

### Technical Features

- **Command Pattern**: Undo-friendly operation structure
- **Factory Patterns**: Extensible shape and UI creation system
- **MVVM Architecture**: Clean separation of concerns
- **TCP Communication**: Fast, reliable client-server messaging
- **Event-Driven Updates**: Reactive programming for real-time features

---

## 🔧 Docker Configuration

The application uses MongoDB running in a Docker container with authentication enabled.

### `docker-compose.yml`

```yaml
version: "3.8"

services:
  mongo:
    image: mongo:6
    restart: always
    ports:
      - "27017:27017"
    environment:
      MONGO_INITDB_ROOT_USERNAME: USERNAME
      MONGO_INITDB_ROOT_PASSWORD: PASSWORD
      MONGO_INITDB_DATABASE: DBName
    volumes:
      - mongo_data:/data/db

volumes:
  mongo_data:
```

### Database Features

- **Persistent Volumes**: Data survives container restarts
- **Authentication**: Secured with username/password
- **Auto-restart**: Container automatically restarts on failure
- **Port Mapping**: Accessible on standard MongoDB port (27017)

---

## 🏛️ Architecture Deep Dive

### Client Architecture

The WPF client follows MVVM pattern with command-based operations:

- **Views**: XAML-based UI with data binding
- **ViewModels**: Business logic and state management
- **Commands**: Encapsulated operations (IDrawingCommand implementations)
- **Services**: TCP communication and external integrations
- **Factories**: Centralized object creation with dependency injection

### Server Architecture

The server implements a multi-layered approach:

- **TCP Server**: Handles incoming client connections
- **Request Handlers**: Process specific operations (CRUD on sketches)
- **Repository Layer**: MongoDB data access abstraction
- **Event System**: Real-time notifications and updates
- **Lock Manager**: Prevents concurrent editing conflicts

### Data Flow

1. **Client Action** → Command Pattern → TCP Message
2. **Server Processing** → Handler → Repository → MongoDB
3. **Response** → TCP → Client Update → UI Refresh
4. **Broadcasting** → Event System → All Connected Clients

### Shared Components (Common)

- **DTOs**: Data transfer objects for network communication
- **Models**: Core business entities shared across layers
- **Enums**: Type-safe constants and categories
- **Helpers**: Utility classes and extension methods

---

## � Usage Guide

### Drawing Operations

1. **Create Shapes**: Select tool from toolbar and draw on canvas
2. **Select Shapes**: Click on any shape to select and highlight it
3. **Clear Canvas**: Use Clear button to remove all shapes
4. **Save Work**: Upload button saves current sketch to database

### Collaboration Workflow

1. **Start Server**: Run server application first
2. **Connect Clients**: Launch multiple client instances
3. **Draw Together**: Changes appear in real-time across all clients
4. **Load Sketches**: Use Import to load existing drawings from database

### Administrative Tasks

- **Monitor Connections**: Server UI shows connected clients
- **Manage Database**: View and manage stored sketches
- **Configuration**: Adjust settings through Options window

## 🐛 Troubleshooting

### Common Issues

**MongoDB Connection Failed**

```bash
# Ensure Docker is running
docker --version

# Check if container is running
docker ps

# Restart containers if needed
docker-compose down
docker-compose up -d
```

**Client Can't Connect to Server**

- Verify server is running and listening
- Check firewall settings for TCP ports
- Ensure `Common/Constants/Ports.cs` matches server configuration

**Shapes Not Persisting**

- Confirm MongoDB container is healthy
- Check database credentials in `MongoConfig.cs`
- Verify database name matches configuration

**Performance Issues**

- Monitor number of connected clients (high concurrency)
- Check available system memory
- Consider MongoDB indexing for large datasets

### Debug Mode

To run in debug mode for detailed logging:

```cmd
dotnet run --project Server --configuration Debug
dotnet run --project Client --configuration Debug
```

---

## � Future Enhancements

### Planned Features

- **More Shape Types**: Polygons, curves, and freehand drawing
- **Styling Options**: Colors, line thickness, and fill patterns
- **Layer Management**: Multiple drawing layers with visibility controls
- **User Authentication**: Login system with user-specific sketches
- **Permissions System**: Read-only vs edit access for collaboration
- **Real-time Chat**: Communication between collaborative users
- **Version History**: Sketch versioning and rollback capabilities
- **Export Formats**: Save as PNG, SVG, or PDF files

### Technical Improvements

- **Performance Optimization**: Improved rendering for large sketches
- **Mobile Support**: Cross-platform client using .NET MAUI
- **Web Interface**: Browser-based client using Blazor
- **Microservices**: Split server into focused microservices
- **Event Sourcing**: Complete audit trail of all changes
- **Horizontal Scaling**: Support for multiple server instances

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Setup

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Style

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Write unit tests for new features

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Jonathan Kalush**

- GitHub: [@kalush666](https://github.com/kalush666)
- Project Link: [https://github.com/kalush666/PaintProject](https://github.com/kalush666/PaintProject)

---

_Built with ❤️ using .NET, WPF, and MongoDB_
