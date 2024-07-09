# MOVEit File Transfer Application

**MOVEitFileTransfer** is a .NET application that monitors a local folder for new files and uploads them to MOVEit Transfer via its REST API.
It also supports deleting files from MOVEit Transfer when they are deleted from the local folder.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Getting Started](#getting-started)
  - [Requirements](#requirements)
  - [Installation](#installation)
  - [Configuration](#configuration)
- [Installation](#installation)

---

## Overview

MOVEitFileTransfer automates the process of transferring files between a local directory and MOVEit Transfer, a secure file transfer service. It monitors the local folder for new files, uploads them to MOVEit Transfer, and deletes them from MOVEit Transfer when they are deleted locally.

---

## Features

- **Automated Monitoring**: Monitors a designated local folder for new files.
- **Secure Uploads**: Automatically uploads new files to MOVEit Transfer using its REST API.
- **File Deletion Sync**: Deletes files from MOVEit Transfer when deleted locally.
- **Configuration Flexibility**: Configurable via `appsettings.json` for local folder path and MOVEit Transfer server details.

---

## Getting Started

### Configuration
To configure the application, insert your credentials into the `appsettings.json`.

   ```json
   {
     "MoveitServerUrl": "https://testserver.moveitcloud.com/api/v1",
     "LocalFolderPath": "path_to_your_local_folder",
     "Username": "your_moveit_username",
     "Password": "your_moveit_password"
   }
```


### Requirements


- [.NET Core SDK](https://dotnet.microsoft.com/download) installed on your machine.


 ### Installation

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/your/repository.git
   cd repository-name
   
2. **Build and Run the Application:**

3. **Navigate to the Specified Local Folder:**
   Open the file explorer and go to the directory specified in the `appsettings.json` configuration file.

4. **Add Files to MoveIt Transfer:**
   - Locate the files you want to transfer.
   - Copy or move these files into the specified folder. MoveIt will detect and initiate transfer based on its configuration.

5. **Delete Files from the Folder:**
   - Identify the files you wish to remove from the transfer process.
   - Delete these files directly from the specified folder. MoveIt will update its transfer queue accordingly.
