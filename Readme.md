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
├── Common/
│   ├── Errors/
│   │   └── AppErrors.cs
│   ├── Events/
│   │   └── LockHub.cs
│   ├── Helpers/
│   │   └── ResponseHelper.cs
│   └── Common.csproj
│
├── Client/
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── Client.csproj
│   ├── Commands/
│   │   ├── ClearCommand.cs
│   │   ├── IDrawingCommand.cs
│   │   ├── ImportCommand.cs
│   │   ├── OptionsCommand.cs
│   │   ├── ShapeSelectionCommand.cs
│   │   └── UploadCommand.cs
│   ├── Convertors/
│   │   └── JsonToShapeConvertor.cs
│   ├── Enums/
│   │   └── BasicShapeType.cs
│   ├── Factories/
│   │   ├── DrawingCommandFactory.cs
│   │   ├── ShapeFactory.cs
│   │   └── UIShapeFactory.cs
│   ├── Handlers/
│   │   └── DrawingHandler.cs
│   ├── Helpers/
│   │   └── CanvasGeometryHelper.cs
│   ├── Models/
│   │   ├── Circle.cs
│   │   ├── Line.cs
│   │   ├── Position.cs
│   │   ├── Rectangle.cs
│   │   ├── ShapeBase.cs
│   │   └── Sketch.cs
│   ├── Services/
│   │   └── ClientCommunicationService.cs
│   ├── UIModels/
│   ├── ViewModels/
│   └── Views/
│       ├── ClientWindow.xaml
│       ├── ClientWindow.xaml.cs
│       ├── ImportSelectionWindow.xaml
│       ├── ImportSelectionWindow.xaml.cs
│       ├── OptionsWindow.xaml
│       └── OptionsWindow.xaml.cs
│
├── Server/
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── Server.csproj
│   ├── Config/
│   │   └── MongoConfig.cs
│   ├── Handlers/
│   │   ├── DownloadHandler.cs
│   │   └── UploadHandler.cs
│   ├── Helpers/
│   │   └── SketchStoreNotifier.cs
│   ├── Repositories/
│   │   └── MongoSketchStore.cs
│   ├── Services/
│   │   ├── LockManager.cs
│   │   └── TcpSketchServer.cs
│   └── Views/
│       ├── ServerWindow.xaml
│       └── ServerWindow.xaml.cs
```

---

## 🚀 How It Works

1. The **Client** is a WPF app where users can draw and manipulate shapes.
2. The **Server** handles communication, shape logic, and data storage.
3. Shapes are saved and retrieved from **MongoDB**, running in a Docker container.
4. Shared models and logic are reused across the client and server using the Common project.

---

## 🛠️ Technologies Used

* 💻 **WPF** (.NET) – Graphical client interface
* 🧠 **C# .NET** – Server logic and shared models
* 🧾 **MongoDB** – Shape data storage
* 🐳 **Docker** – Containerization for database
* 🔁 **JSON** – Data exchange format

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
* 🖼️ Opens the WPF client.

---

## 📦 Features

* ✏️ Draw shapes (line, rectangle, circle...)
* 💾 Save/load drawings from MongoDB
* ♻️ Real-time client-server communication
* 🏗️ Shared model logic via common class library
* 🧪 Import/export data from database

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

| Folder    | Purpose                                                                      |
|-----------|------------------------------------------------------------------------------|
| `Client/` | WPF UI app for drawing shapes                                                |
| `Server/` | Handles shape logic and Mongo integration + UI for managing files/access     |
| `Common/` | Contains utilities, events, and shared errors used by both client and server |

---

## 📎 Notes

* 🟨 If shapes are not loading: make sure MongoDB is running and properly populated.
* 🔁 Data is serialized using `Newtonsoft.Json`.

---

## 👨‍💻 Author

Built by Jonathan Kalush – [GitHub](https://github.com/kalush666)
