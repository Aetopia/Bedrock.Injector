# Bedrock Injector
An experimental next-gen dynamic link library injector for Minecraft: Bedrock Edition.

## Why?
Injecting dynamic link libraries into Minecraft: Bedrock Edition is a complete mess.<br>
Messed up how exactly? A lot of publicly avaiable injectors/launchers on GitHub atleast use the following setup:

- Launch the game using:

   - `minecraft://` 

   - `explorer.exe shell:appsFolder\Microsoft.MinecraftUWP_8wekyb3d8bbwe!App`

- Enumerate all running processes to find a a process named `Minecraft.Windows.exe`.

- Keep track of the target process' module count till it reaches a certain threshold, this will indicate it has initialized to the desired extent.

- Inject desired dynamic link libraries.

Now this setup works as intended & gets the job done. 

The thing is that it can be greatly improved & made much more reliable:

- The first thing to tackle is how we launch & get hold of the game's process:

    - Instead of using the URI protocol or Windows Explorer, we can use `IApplicationActivationManager` to launch the game.

    - This interface gives the underlying process ID of the launched app avoiding the need to hunt for it.

- The second thing to tackle is somehow detect that the game is fully initialized to a desired extent:

    - Keeping track of a process' module count isn't a reliable way to determine the initialization state because:

        - A process' module count is an arbitrary metric to begin with & isn't consistent.

        - The module count doesn't represent the process' initialzation state logically. 

    - When the game starts, it creates the following file `resource_init_lock`:

        ```
        %LOCALAPPDATA%\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftpe\resource_init_lock
        ```

    - `resource_init_lock` is then deleted when the game reaches the title screen.

    - We can use `resource_init_lock` to determine the game's initialzation state:

        - If the file is present, this indicates the game hasn't reached the title screen.

        - If the file is absent, this indicates the game has reached or gone beyond the title screen.

    - The detection process can be implemented as follows:

        - If the game is already running & `resource_init_lock` doesn't exist, this indicates the game is fully initialized.

        - Wait for `resource_init_lock` to be deleted.

        - Once it is deleted, the game has reached the title screen & is fully initialized.

> [!TIP]
> Enabling debug mode is highly recommended since it prevents PLM from affecting Minecraft: Bedrock Edition.

## Building
1. Download the following:
    - [.NET SDK](https://dotnet.microsoft.com/en-us/download)
    - [.NET Framework 4.8.1 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net481-developer-pack-offline-installer)

2. Run the following command to compile:

    ```cmd
    dotnet publish "src\Bedrock.Injector.csproj"
    ```