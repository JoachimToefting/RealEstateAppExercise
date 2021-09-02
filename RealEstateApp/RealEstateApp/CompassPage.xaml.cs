using RealEstateApp.Models;
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
	public partial class CompassPage : ContentPage
	{
		private Aspects _currentAspect;

		public Aspects CurrentAspect
		{
			get { return _currentAspect; }
			set { _currentAspect = value; }
		}

		private double _rotationAngle;

		public double RotationAngle
		{
			get { return _rotationAngle; }
			set { _rotationAngle = value; }
		}
		private double _currentHeading;

		public double CurrentHeading
		{
			get { return _currentHeading; }
			set { _currentHeading = value; }
		}
		private Property _property { get; set; }
		public CompassPage(Property property)
		{
			InitializeComponent();
			this.BindingContext = this;
			this._property = property;
		}
		#region Event handlers
		protected override void OnAppearing()
		{
			base.OnAppearing();
			Compass.ReadingChanged += CompassReader;
			Compass.Start(SensorSpeed.UI);
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Compass.ReadingChanged -= CompassReader;
			Compass.Stop();
		}
		#endregion
		private void CompassReader(object sender, CompassChangedEventArgs e)
		{
			CurrentHeading = e.Reading.HeadingMagneticNorth;
			RotationAngle = CalculateRotation(CurrentHeading);
			CurrentAspect = CalculateAspect(CurrentHeading);
			_property.Aspect = CurrentAspect;
		}
		private double CalculateRotation(double heading)
		{
			return 360-heading;
		}
		private Aspects CalculateAspect(double heading)
		{
			int secment = (int)(Math.Round(heading / 90) * 90);
			//North loop around
			if (secment == 360)
			{
				secment = 0;
			}
			return (Aspects)secment;
		}

		private async void Save_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync();
		}
	}
}