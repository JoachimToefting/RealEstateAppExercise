using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();
		}
		#region 3.5
		//Stopper vibration mellem sider
		protected override void OnNavigating(ShellNavigatingEventArgs args)
		{
			base.OnNavigating(args);
			Vibration.Cancel();
		}
		#endregion
	}
}