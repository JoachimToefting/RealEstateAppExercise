using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImageListPage : ContentPage
	{
		private int position;
		public List<string> myImageUrls { get; }
		public int Position
		{
			get => position;
			set
			{
				if (value >= 0 && value <= 2)
				{
					position = value;
				}
				else if (value < 0)
				{
					position = 2;
				}
				else
				{
					position = 0;
				}
			}
		}
		public ImageListPage(Property property)
		{
			this.myImageUrls = property.ImageUrls;
			this.BindingContext = this;
			InitializeComponent();
			OnPropertyChanged(nameof(Position));
			
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();
			Accelerometer.ShakeDetected += OnShakingEvent;
			Accelerometer.Start(SensorSpeed.Fastest);
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Accelerometer.ShakeDetected -= OnShakingEvent;
			Accelerometer.Stop();
		}

		private void OnShakingEvent(object sender, System.EventArgs e)
		{
			Position++;
		}
	}
}