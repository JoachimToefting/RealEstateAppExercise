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
		public enum Aspects
		{
			North = 0,
			East = 90,
			South = 180,
			West = 270
		}
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
		public CompassPage()
		{
			InitializeComponent();
			this.BindingContext = this;
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
		}
		private double CalculateRotation(double heading)
		{
			return 360-heading;
		}
		private Aspects CalculateAspect(double heading)
		{
			int secment = (int)(Math.Round(heading / 90) * 90);
			return (Aspects)secment;
		}
	}
}