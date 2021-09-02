using RealEstateApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HeightCalculatorPage : ContentPage
	{
		public ObservableCollection<BarometerMeasurement> Measurements { get; set; }
		public double CurrentPressure { get; set; }
		public double CurrentAltitude { get; set; }
		public string LabelName { get; set; }
		private double SeaLevelPressure { get; set; }
		public HeightCalculatorPage()
		{
			InitializeComponent();
			SeaLevelPressure = 1025.1;
			Measurements = new ObservableCollection<BarometerMeasurement>();
			this.BindingContext = this;
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();
			Barometer.ReadingChanged += BaroChange;
			Barometer.Start(SensorSpeed.UI);
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Barometer.ReadingChanged -= BaroChange;
			Barometer.Stop();
		}
		private void BaroChange(object sender, BarometerChangedEventArgs e)
		{
			CurrentPressure = e.Reading.PressureInHectopascals;
			CurrentAltitude = 44307.694 * (1 - Math.Pow(CurrentPressure / SeaLevelPressure, 0.190284));
		}

		private void SaveButton_Clicked(object sender, EventArgs e)
		{
			BarometerMeasurement m = new BarometerMeasurement();
			m.Label = LabelName;
			m.Altitude = CurrentAltitude;
			m.Pressure = CurrentPressure;
			if (Measurements.Count > 0)
			{
				m.HeightChange = CurrentAltitude - Measurements.LastOrDefault().Altitude;
			}
			Measurements.Add(m);
			LabelName = string.Empty;
		}
	}
}