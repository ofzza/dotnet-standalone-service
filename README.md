# dotnet-standalone-service
Standalone service for .NET framework allows you to implement services which:
- can run in different contexts (CLI, as service)
- can manage themselves from CLI (install, uninstall, start, stop)

## How to implement
```C#

/// <summary>
/// Defines service installer
/// </summary>
public class TestStandaloneServiceInstaller : StandaloneServiceInstaller {
  public TestStandaloneServiceInstaller() : base(TestStandaloneService.Identity) { }
}

/// <summary>
/// Defines main service/application class
/// </summary>
public class TestStandaloneService : StandaloneService {

  /// <summary>
  /// Defines main console entry point
  /// </summary>
  /// <param name="args">Startup arguments</param>
  static void Main(string[] args) {
    if (args.Contains("--install")) {
      // Install service
      (new TestStandaloneService()).Install("--install");
    } else if (args.Contains("--uninstall")) {
      // Uninstall service
      (new TestStandaloneService()).Uninstall("--uninstall");
    } else if (args.Contains("--start")) {
      // Start installed service
      (new TestStandaloneService()).Start();
    } else if (args.Contains("--stop")) {
      // Stop installed service
      (new TestStandaloneService()).Stop();
    } else {
      // Run service functionality as process
      StandaloneService.Run(new TestStandaloneService(), args);
    }
  }

  /// <summary>
  /// Service identity
  /// </summary>
  static public StandaloneServiceIdentity Identity = new StandaloneServiceIdentity () {
    Id = "test-service",
    Name = "Test standalone service",
    Description = "..."
  };
        
  /// <summary>
  /// Constructor
  /// </summary>
  public TestStandaloneService() : base(TestStandaloneService.Identity) { }

  /// <summary>
  /// Extensible execution method, executes service functionality
  /// </summary>
  /// <param name="args">Startup arguments</param>
  protected override void ExecuteServiceFunctionality(string[] args) {
    while (true) {
      // Do stuff ...
      Console.WriteLine("I'm working ...");
      Thread.Sleep(1000);
    }
  }

}

```

## How to use

#### Run from CLI
```cmd
> test.exe

I'm working ...
I'm working ...
I'm working ...
```

#### Install as service
```sh
> test.exe --install
```
*Will prompt for elevated privilages if needed.*

#### Uninstall service
```sh
> test.exe --uninstall
```
*Will prompt for elevated privilages if needed.*

#### Start service
```sh
> test.exe --start
```
*Will prompt for elevated privilages if needed.*

#### Stop service
```sh
> test.exe --stop
```
*Will prompt for elevated privilages if needed.*
