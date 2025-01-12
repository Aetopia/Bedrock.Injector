# Bedrock Injector
An experimental next-gen dynamic link library injector for Minecraft: Bedrock Edition.

## Features

- Inject multiple dynamic link libraries at once.

    - Inject your favorite clients, modifications & hacked clients in one go!

## Usage

- Import the dynamic link libraries to be injected.

- Organize dynamic link libraries based on load order.

- Click <kbd>▶</kbd> or <kbd>⬛</kbd> to launch & inject dynamic link libraries.

<hr>

- <kbd>📂</kbd> Import dynamic link libraries to injected.

- <kbd>🗑️</kbd> Remove selected dynamic link libraries.

- <kbd>✔️</kbd> Select all dynamic link libraries.

- <kbd>❌</kbd> Deselect all dynamic link libraries.

- <kbd>🔺</kbd> Move a dynamic link library up.

- <kbd>🔻</kbd> Move a dynamic link library down.

- <kbd>▶</kbd> Launches & injects dynamic link libraries.

- <kbd>⬛</kbd> Launches, inject dynamic link libraries & closes.

<hr>



## Building
1. Download the following:
    - [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - [.NET Framework 4.8.1 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net481-developer-pack-offline-installer)

2. Run the following command to compile:

    ```cmd
    dotnet publish "src\Bedrock.Injector.csproj"
    ```