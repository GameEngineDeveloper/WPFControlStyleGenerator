using System.CodeDom.Compiler;
using System.Diagnostics;
using Application = System.Windows.Application;

namespace ButtonStyleGenerator;

public class App : Application
{
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "9.0.8.0")]
	public void InitializeComponent()
	{
		StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
	}
}
