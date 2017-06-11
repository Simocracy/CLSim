using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Simocracy.CLSim
{
	/// <summary>
	/// Interaktionslogik für "App.xaml"
	/// </summary>
	public partial class App : Application
	{

		[STAThread]
		public static void Main()
		{
			// Setup logger
			SimpleLog.SetLogFile(logDir: "SSS-Log", writeText: true, check: false);
			SimpleLog.Check($"{ProgramName} {ProgramVersion} started");

			// Load Settings
			//Settings.LoadSettings();

			// Open MainWindow
			App app = new App();
			try
			{
				app.InitializeComponent();
				MainWindow window = new MainWindow();
				app.Run(window);
			}
			catch (Exception e)
			{
				SimpleLog.Error(e.ToString());
			}

			app.Shutdown();
			SimpleLog.Info($"{ProgramName} {ProgramVersion} closed");
			SimpleLog.Flush();
			SimpleLog.StopLogging();
		}

		public static string ProgramVersion
		{
			get
			{
				var version = Assembly.GetExecutingAssembly().GetName().Version;
				var versionString = $"{version.Major}.{version.Minor}.{version.Revision}";
#if DEBUG
				versionString = $"{versionString} Debug Build";
#endif
				return versionString;
			}
		}

		public static string ProgramName => ((AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title;
	}
}
