﻿using RealEstateApp.Services;
using RealEstateApp.Services.Repository;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RealEstateApp
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			var container = TinyIoCContainer.Current;
			container.Register<IRepository, MockRepository>();

			MainPage = new AppShell();
		}


		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
			#region 3.5
			Vibration.Cancel();
			#endregion
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
		
	}
}
