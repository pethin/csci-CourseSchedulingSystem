# Command Line Interface (CLI)

The application provides a command line interface implemented using the
[CommandLineUtils](https://natemcmaster.github.io/CommandLineUtils/index.html) library.

## Using the CLI
### From Source
If using the project source, the commands can be run using the `dotnet` CLI.

1. Enter the project directory (not to be confused with the solution directory)
2. Run `dotnet run -- <COMMAND>`. Replace `<COMMAND>` with the command you want to run.

To get a list of possible commands that can be run: `dotnet run -- --help`.

### From Binary
If using the prebuilt binaries, the method is slightly different.

1. Enter the output directory (there should be a file named `CourseSchedulingSystem.dll`)
2. Run `dotnet .\CourseSchedulingSystem.dll <COMMAND>`. Replace `<COMMAND>` with the command you want to run.

To get a list of possible commands that can be run: `dotnet .\CourseSchedulingSystem.dll --help`.

## Adding a Command
1. Add a class in the `Commands` namespace. Look at `SeedCommand.cs` for an example.
2. Add the `Command` attribute to the class.
3. Implement the `ICommand` interface.
