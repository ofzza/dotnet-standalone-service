using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace ofzza.standaloneservice {

  /// <summary>
  /// Service identity
  /// </summary>
  public class StandaloneServiceIdentity {
    /// <summary>
    /// Service unique name
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Service title
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Service description
    /// </summary>
    public string Description { get; set; }
  }

  /// <summary>
  /// A stand alone service installer information
  /// </summary>
  [RunInstaller(true)]
  public class StandaloneServiceInstaller : Installer {

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="serviceIdentity">Service's identity information</param>
    /// <param name="serviceStartMode">Service start mode</param>
    /// <param name="serviceAccount">Service account</param>
    public StandaloneServiceInstaller (StandaloneServiceIdentity serviceIdentity, ServiceStartMode serviceStartMode = ServiceStartMode.Automatic, ServiceAccount serviceAccount = ServiceAccount.LocalSystem) {
      var processInstaller = new ServiceProcessInstaller();
      var serviceInstaller = new ServiceInstaller();

      // Set the privileges
      serviceInstaller.DisplayName = serviceIdentity.Name;
      serviceInstaller.StartType = serviceStartMode;
      processInstaller.Account = serviceAccount;

      // Must be the same as what was set in Program's constructor
      serviceInstaller.ServiceName = serviceIdentity.Id;
      this.Installers.Add(processInstaller);
      this.Installers.Add(serviceInstaller);
    }

  }

  /// <summary>
  /// Implements standalone service
  /// </summary>
  public partial class StandaloneService : ServiceBase {

    #region Constructors

    /// <summary>
    /// Dummy constructor to be overridden by call to 'StandaloneService(serviceIdentity)' constructor
    /// </summary>
    public StandaloneService () { }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="identity">Identity information</param>
    public StandaloneService (StandaloneServiceIdentity serviceIdentity) {
      // Set identity
      this.serviceIdentity = serviceIdentity;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets/Sets service identity
    /// </summary>
    StandaloneServiceIdentity serviceIdentity { get; set; }

    /// <summary>
    /// Holds main service excution thread
    /// </summary>
    protected Thread thread;

    #endregion

    #region Console entry point

    /// <summary>
    /// Defines main console entry point
    /// </summary>
    /// <param name="service">Overriden standalone service instance to run</param>
    /// <param name="args">Startup arguments</param>
    static public void Run (StandaloneService service, string[] args) {
      // Check environment (has console?)
      if (System.Environment.UserInteractive) {
        // Run service (in console)
        service.RunUserInteractive(args);
      } else {
        // Run service (with no interactivity)
        service.RunNonInteractive();
      }
    }

    /// <summary>
    /// Runs service (As console application)
    /// </summary>
    /// <param name="args">Startup arguments</param>
    public void RunUserInteractive (string[] args) {
      // Execute service (in console)
      this.ExecuteServiceFunctionality(args);
    }

    /// <summary>
    /// Runs service (with no interactivity)
    /// </summary>
    public void RunNonInteractive () {
      // Run service (as service)
      ServiceBase.Run(new ServiceBase[] { this });
    }

    #endregion

    #region Service entry points

    /// <summary>
    /// On service start
    /// </summary>
    /// <param name="args">Startup arguments</param>
    protected override void OnStart (string[] args) {
      // Start service
      if (this.thread == null) {
        this.thread = new Thread(() => this.ExecuteServiceFunctionality(args));
        thread.IsBackground = true;
        thread.Start();
      }
    }
    /// <summary>
    /// On service stop
    /// </summary>
    protected override void OnStop () {
      // Stop service
      if (this.thread.ThreadState != System.Threading.ThreadState.Stopped) {
        this.thread.Abort();
        this.thread = null;

      }
    }

    #endregion

    #region Service functionality implementation

    /// <summary>
    /// Extensible execution method, executes service functionality
    /// </summary>
    /// <param name="args">Startup arguments</param>
    protected virtual void ExecuteServiceFunctionality (string[] args) { }

    #endregion

    #region Service management

    /// <summary>
    /// Installs a standalone service
    /// </summary>
    /// <param name="arg">Startup argument needed to access the install path, used to restart as admin if needed (example: "--install")</param>
    public void Install (string arg = null) {
      try {
        // Execute install
        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetEntryAssembly().Location });
      } catch (Exception ex) {
        // Check if can run self and if already parented by self
        if (arg != null && !Process.GetCurrentProcess().StartInfo.Arguments.Contains("--child-process")) {
          // Initialize install process call (as admin)
          Process process = new Process() {
            StartInfo = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName, String.Format("{0} --child-process", arg)) {
              Verb = "runas",
              WindowStyle = ProcessWindowStyle.Hidden,
              UseShellExecute = true
            },
          };
          // Run install process
          process.Start();
        }
      }
    }

    /// <summary>
    /// Uninstalls a standalone service
    /// </summary>
    /// <param name="arg">Startup argument needed to access the uninstall path, used to restart as admin if needed (example: "--uninstall")</param>
    public void Uninstall (string arg = null) {
      try {
        // Execute uninstall
        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetEntryAssembly().Location });
      } catch (Exception ex) {
        // Check if can run self and if already parented by self
        if (arg != null && !Process.GetCurrentProcess().StartInfo.Arguments.Contains("--child-process")) {
          // Initialize install process call (as admin)
          Process process = new Process() {
            StartInfo = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName, String.Format("{0} --child-process", arg)) {
              Verb = "runas",
              WindowStyle = ProcessWindowStyle.Hidden,
              UseShellExecute = true
            },
          };
          // Run install process
          process.Start();
        }
      }
    }

    /// <summary>
    /// Starts installed standalone service
    /// </summary>
    public void Start () {
      // Initialize start process call (as admin)
      Process process = new Process() {
        StartInfo = new ProcessStartInfo("cmd.exe", String.Format("/c NET START \"{0}\"", this.serviceIdentity.Id)) {
          Verb = "runas",
          WindowStyle = ProcessWindowStyle.Hidden,
          UseShellExecute = true
        },
      };
      // Run start process
      process.Start();
    }

    /// <summary>
    /// Stops installed standalone service
    /// </summary>
    public void Stop () {
      // Initialize stop process call (as admin)
      Process process = new Process() {
        StartInfo = new ProcessStartInfo("cmd.exe", String.Format("/c NET STOP \"{0}\"", this.serviceIdentity.Id)) {
          Verb = "runas",
          WindowStyle = ProcessWindowStyle.Hidden,
          UseShellExecute = true
        },
      };
      // Run stop process
      process.Start();
    }

    #endregion

  }

}
