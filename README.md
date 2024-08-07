# MOVEit File Transfer Application

**MOVEitFileTransfer** is a .NET application that monitors a local folder for new files and uploads them to MOVEit Transfer via its REST API. It also supports deleting and renaming files from MOVEit Transfer when changes are made to the local folder.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Configuration](#configuration)

---

## Overview

MOVEitFileTransfer automates the process of transferring files between a local directory and MOVEit Transfer, a secure file transfer service. It monitors the local folder for new files, uploads them to MOVEit Transfer, deletes and renames them from MOVEit Transfer when they are deleted locally.

---

## Features

- **Auto-Creation of Local Folder**: Automatically creates a designated local folder on the desktop if it does not exist.
- **Automated Monitoring**: Monitors a designated local folder for new files.
- **Secure Uploads**: Automatically uploads new files to MOVEit Transfer using its REST API.
- **File Deletion Sync**: Deletes files from MOVEit Transfer when deleted locally.
- **File Renaming**: Renames files on MOVEit Transfer when renamed in the local folder.
- **Configuration Flexibility**: Configurable via `appsettings.json` for local folder path and MOVEit Transfer server details.

---

## Requirements

- [.NET Core SDK](https://dotnet.microsoft.com/download) installed on your machine.

---

## Installation

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/Nikzr0/MOVEitFileTransfer.git
   cd MOVEitFileTransfer

2. **Build and Run the Application:**

3. **Enter your credentials:**
   - Navigate to MOVEitFileTransfer and entrey your credential to `appsettings.json` configuration file.

4. **Add Files to MoveIt Transfer:**
   - Locate the files you want to transfer.
   - Copy or move these files into the local folder on your desktop. MoveIt will detect and initiate transfer based on its configuration.

5. **Rename Files from the Folder:**
    - Identify the files you wish to rename.
    - Rename these files directly in the local folder. MOVEit will update its records accordingly.
  
5. **Delete Files from the Folder:**
   - Identify the files you wish to remove from the transfer process.
   - Delete these files directly from the local folder. MoveIt will update its transfer queue accordingly.
  
   ---

### Configuration
- To configure the application, insert your credentials into the `appsettings.json`.

```json
   {
     "MoveitServerUrl": "https://testserver.moveitcloud.com/api/v1",
     "Username": "your_moveit_username",
     "Password": "your_moveit_password"
   }
```
