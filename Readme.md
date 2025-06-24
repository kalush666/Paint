# 🎨 Paint Project – Fullstack WPF + C# + MongoDB

This project is a fullstack paint application featuring a WPF client, a C# backend server, and MongoDB for persistent storage. It enables users to draw, save, and load shapes like lines, rectangles, and circles with real-time communication between the client and the server.

---

## 📁 Project Structure

```
PaintProject/
├── PaintProject.sln
├── run.bat
├── docker-compose.yml
│
├── Client/
│   ├── Client.csproj
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── AssemblyInfo.cs
│   ├── Commands/
│   │   ├── ClearCommand.cs
│   │   ├── IDrawingCommand.cs
│   │   ├── ImportCommand.cs
│   │   ├── OptionsCommand.cs
│   │   ├── ShapeSelectionCommand.cs
│   │   └── UploadCommand.cs
│   ├── Convertors/
│   │   ├── IUIShapeConvertor.cs
│   │   ├── JsonToShapeConvertor.cs
│   │   └── ShapeToUIConvertors.cs
│   ├── Enums/
│   │   └── CommandTypes.cs
│   ├── Factories/
│   │   ├── DrawingCommandFactory.cs
│   │   ├── ShapeFactory.cs
│   │   └── UIShapeFactory.cs
│   ├── Handlers/
│   │   └── DrawingHandler.cs
│   ├── Helpers/
│   │   ├── BrushMappingHelper.cs
│   │   ├── GeometryHelper.cs
│   │   └── ShapeSelectionHighlighter.cs
│   ├── Mappers/
│   │   └── SketchMapper.cs
│   ├── Models/
│   │   ├── Circle.cs
│   │   ├── Line.cs
│   │   ├── Rectangle.cs
│   │   ├── ShapeBase.cs
│   │   └── Sketch.cs
│   ├── Services/
│   │   ├── ClientCommunicationService.cs
│   │   ├── CommandServiceProvider.cs
│   │   └── ICommandServiceProvider.cs
│   ├── UIModels/
│   │   ├── UIBaseShape.cs
│   │   ├── UICircle.cs
│   │   ├── UILine.cs
│   │   └── UIRectangle.cs
│   └── Views/
│       ├── MainClientWindow/
│       │   ├── ClientWindow.xaml
│       │   └── ClientWindow.xaml.cs
│       └── Service Windows/
│           ├── Import Selection Window/
│           │   ├── ImportSelectionWindow.xaml
│           │   └── ImportSelectionWindow.xaml.cs
│           └── Options Window/
│               ├── OptionsWindow.xaml
│               └── OptionsWindow.xaml.cs
│
├── Common/
│   ├── Common.csproj
│   ├── Constants/
│   │   ├── Ports.cs
│   │   └── SketchFields.cs
│   ├── Convertors/
│   │   └── ObjectIdToJsonConvertor.cs
│   ├── DTO/
│   │   ├── ShapeDto.cs
│   │   └── SketchDto.cs
│   ├── Enums/
│   │   └── BasicShapeType.cs
│   ├── Errors/
│   │   └── AppErrors.cs
│   ├── Events/
│   │   └── LockHub.cs
│   ├── Helpers/
│   │   ├── RelayCommand.cs
│   │   ├── ResponseHelper.cs
│   │   └── Result.cs
│   └── Models/
│       └── Position.cs
│
└── Server/
    ├── Server.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Config/
    │   └── MongoConfig.cs
    ├── Enums/
    │   └── SketchEventType.cs
    ├── Events/
    │   ├── SketchEvent.cs
    │   └── SketchEventBus.cs
    ├── Factories/
    │   ├── IHandlerFactory.cs
    │   └── SketchRequestFactory.cs
    ├── Handlers/
    │   ├── GetAllNamesHandler.cs
    │   ├── GetAllSketchesHandler.cs
    │   ├── GetSpecificSketchHandler.cs
    │   ├── IRequestHandler.cs
    │   ├── IRequestProcessor.cs
    │   └── UploadSketchHandler.cs
    ├── Mappers/
    │   └── ServerSketchMappers.cs
    ├── Models/
    │   ├── ServerBaseShape.cs
    │   ├── ServerSketch.cs
    │   └── SketchEntry.cs
    ├── Repositories/
    │   └── MongoSketchStore.cs
    ├── Services/
    │   ├── LockManager.cs
    │   └── TcpSketchServer.cs
    ├── ViewModel/
    │   └── SketchListViewModel.cs
    └── Views/
        ├── ServerWindow.xaml
        └── ServerWindow.xaml.cs
```

---

## 🚀 How It Works

1. The **Client** is a WPF app where users can draw and manipulate shapes.
2. The **Server** handles communication, shape logic, and data storage.
3. Shapes are saved and retrieved from **MongoDB**, running in a Docker container.
4. Shared models and logic are reused across the client and server using the Common project.

---

## 🛠️ Technologies Used

* 💻 **WPF** (.NET 5.0) – Graphical client interface
* 🧠 **C# .NET** – Server logic and shared models
* 🧾 **MongoDB** – Shape data storage
* 🐳 **Docker** – Containerization for database
* 🔁 **JSON** – Data exchange format
* 🌐 **TCP** – Client-server communication

---

## ⚙️ `run.bat` – One-Click Launcher

This file launches the whole stack in one command:

```bat
@echo off
echo Starting MongoDB container...
docker-compose up -d

echo Starting Server...
start cmd /k "dotnet run --project Server"

timeout /t 2 >nul

echo Starting Client 1...
start cmd /k "dotnet run --project Client\Client.csproj"

timeout /t 1 >nul

echo Starting Client 2...
start cmd /k "dotnet run --project Client\Client.csproj"
```

✅ **Usage:** Just double-click `run.bat` to start everything!

* 🐳 Starts MongoDB via Docker.
* 🖥️ Runs the backend server.
* 🖼️ Opens multiple WPF clients.

---

## 📦 Features

* ✏️ Draw shapes (line, rectangle, circle)
* 💾 Save/load drawings from MongoDB
* ♻️ Real-time client-server communication via TCP
* 🏗️ Shared model logic via common class library
* 🧪 Import/export data from database
* 🔒 Lock management for concurrent access
* 🎯 Shape selection and highlighting

---

## 🔧 Docker Setup (MongoDB)

`docker-compose.yml`:

```yaml
version: '3.8'
services:
  mongodb:
    image: mongo
    container_name: paint-mongo
    ports:
      - "27017:27017"
    volumes:
      - mongo_data:/data/db
volumes:
  mongo_data:
```

---

## 🧠 Key Folders

| Folder    | Purpose                                                                    |
|-----------|----------------------------------------------------------------------------|
| `Client/` | WPF UI app for drawing shapes with command pattern implementation          |
| `Server/` | TCP server handling shape logic and MongoDB integration with management UI |
| `Common/` | Shared DTOs, models, enums, and utilities used by both client and server   |

---

## 📎 Notes

* 🟨 If shapes are not loading: make sure MongoDB is running and properly populated.
* 🔁 Data is serialized using `Newtonsoft.Json`.
* 🚪 Default MongoDB port is 27017.
* 🔌 Client-server communication uses TCP sockets.

---

## 👨‍💻 Author

Built by Jonathan Kalush – [GitHub](https://github.com/kalush666)