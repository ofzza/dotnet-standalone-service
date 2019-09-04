# dotnet-standalone-service
Standalone service implementation for .NET framework allows for service to run in different contexts (CLI, as service) and to manage itself from CLI (install, uninstall, start, stop)

## Usage
```C#

/// <summary>
/// Defines service installer
/// </summary>
public class TestStandaloneServiceInstaller : StandaloneServiceInstaller {
  public TestStandaloneServiceInstaller() : base(TestStandaloneService.Identity) { }
}

/// <summary>
/// Defines main service class
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
    }
  }

}

```
