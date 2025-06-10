🎨 Paint Project – Fullstack WPF + C# + MongoDB

This project is a fullstack paint application featuring a WPF client, a C# backend server, and MongoDB for persistent storage. It enables users to draw, save, and load shapes like lines, rectangles, and circles with real-time communication between the client and the server.

📁 Project Structure

PaintProject/
├── .vs/
│   └── PaintProject/DesignTimeBuild/.dtbcache.v2
│   └── PaintProject/v16/.suo
│
├── Client/
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── AssemblyInfo.cs
│   ├── Client.csproj
│   ├── ClientWindow.xaml
│   ├── ClientWindow.xaml.cs
│   ├── OptionsWindow.xaml
│   ├── OptionsWindow.xaml.cs
│   ├── Enums/
│   │   └── BasicShapeType.cs
│   ├── Factories/
│   │   └── ShapeFactory.cs
│   ├── Models/
│   │   ├── Circle.cs
│   │   ├── Line.cs
│   │   ├── Position.cs
│   │   ├── Rectangle.cs
│   │   ├── ShapeBase.cs
│   │   └── Sketch.cs
│   ├── Services/
│   │   └── ClientCommunicationService.cs
│   └── bin/, obj/, *.dll, *.pdb, etc.
│
├── Server/
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── ServerWindow.xaml
│   ├── ServerWindow.xaml.cs
│   ├── MongoSketchStore.cs
│   ├── Server.csproj
│   ├── Handlers/
│   │   ├── UploadHandler.cs
│   │   └── DownloadHandler.cs
│   ├── Services/
│   │   ├── LockManager.cs
│   │   └── TcpSketchServer.cs
│   ├── Utils/
│   │   └── MongoConfig.cs
│   └── bin/, obj/, *.dll, *.pdb, etc.
│
├── docker-compose.yml
├── PaintProject.sln
└── run.bat

🚀 How It Works

The Client is a WPF app where users can draw and manipulate shapes.

The Server handles communication, shape logic, and data storage.

Shapes are saved and retrieved from MongoDB, running in a Docker container.

Shared models are reused across the client and server using a shared library.

🛠️ Technologies Used

💻 WPF (.NET) – Graphical client interface

🧠 C# .NET – Server logic and shared models

🧾 MongoDB – Shape data storage

🐳 Docker – Containerization for database

🔁 JSON – Data exchange format

⚙️ run.bat – One-Click Launcher

This file launches the whole stack in one command:

@echo off

REM Start MongoDB
Docker-compose up -d

REM Start backend server
start dotnet run --project Server

REM Wait for server to boot
Timeout /t 2 >nul

REM Start WPF client
start dotnet run --project Client

✅ Usage: Just double-click run.bat to start everything!

🐳 Starts MongoDB via Docker.

🖥️ Runs the backend server.

🖼️ Opens the WPF client.

📦 Features

✏️ Draw shapes (line, rectangle, circle...)

💾 Save/load drawings from MongoDB

♻️ Real-time client-server communication

🏗️ Shared model logic via common class library

🧪 Import/export data from database

🔧 Docker Setup (MongoDB)

docker-compose.yml:

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

🧠 Key Folders

Folder

Purpose

Client/

WPF UI app for drawing shapes

Server/

Handles shape logic and Mongo integration

📎 Notes

🟨 If shapes are not loading: make sure MongoDB is running and properly populated.

🔁 Data is serialized using Newtonsoft.Json.

💡 Extendable via ShapeFactory and Enums in Shared Models.

👨‍💻 Author

Built by Jonathan Kalush – GitHub

📜 License

MIT License © 2025

