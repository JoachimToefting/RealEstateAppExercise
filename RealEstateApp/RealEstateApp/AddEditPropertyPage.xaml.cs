﻿using RealEstateApp.Models;
using RealEstateApp.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddEditPropertyPage : ContentPage
	{
		private IRepository Repository;

		#region PROPERTIES
		public ObservableCollection<Agent> Agents { get; }

		private Property _property;
		public Property Property
		{
			get => _property;
			set
			{
				_property = value;
				if (_property.AgentId != null)
				{
					SelectedAgent = Agents.FirstOrDefault(x => x.Id == _property?.AgentId);
				}

			}
		}

		private Agent _selectedAgent;

		public Agent SelectedAgent
		{
			get => _selectedAgent;
			set
			{
				if (Property != null)
				{
					_selectedAgent = value;
					Property.AgentId = _selectedAgent?.Id;
				}
			}
		}

		public string StatusMessage { get; set; }

		public Color StatusColor { get; set; } = Color.White;
		#endregion

		public AddEditPropertyPage(Property property = null)
		{
			InitializeComponent();

			Repository = TinyIoCContainer.Current.Resolve<IRepository>();
			Agents = new ObservableCollection<Agent>(Repository.GetAgents());

			if (property == null)
			{
				Title = "Add Property";
				Property = new Property();
			}
			else
			{
				Title = "Edit Property";
				Property = property;
			}
			BindingContext = this;
			#region 3.4
			Connectivity.ConnectivityChanged += ConnectionChanged;
			if (Connectivity.NetworkAccess != NetworkAccess.Internet)
			{
				DisplayAlert("Advarsel", "Intet innernet", "Ok");
			}
			else
			{
				GetHome.IsEnabled = true;
				GetLocation.IsEnabled = true;
			}
			#endregion
			#region 3.7
			Battery.BatteryInfoChanged += BatteryChange;
			#endregion
		}
		
		#region 3.7
		private void BatteryChange(object sender, System.EventArgs e)
		{
			if (Battery.ChargeLevel <= 0.2f)
			{
				if (Battery.PowerSource == BatteryPowerSource.AC)
				{
					StatusColor = Color.Green;
					StatusMessage = "Battery charging";
				}
				else if (Battery.EnergySaverStatus == EnergySaverStatus.On)
				{
					StatusColor = Color.Yellow;
					StatusMessage = "Battery energy saver";
				}
				else
				{
					StatusColor = Color.Red;
					StatusMessage = "Battery low";
				}
			}
			else
			{
				StatusMessage = "";
				StatusColor = Color.White;
			}
		}
		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Battery.BatteryInfoChanged -= BatteryChange;
		}
		#region Flash
		private bool FlashOn = false;
		private async void Flashlight_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				if (FlashOn)
				{
					await Flashlight.TurnOffAsync();
					FlashOn = false;
				}
				else
				{
					await Flashlight.TurnOnAsync();
					FlashOn = true;
				}
			}
			catch (FeatureNotSupportedException fnsEx)
			{
				StatusMessage = "Feature not supportet error: " + fnsEx.Message;
			}
			catch (FeatureNotEnabledException fneEx)
			{
				StatusMessage = "Feature not Enabled error: " + fneEx.Message;
			}
			catch (PermissionException pEx)
			{
				StatusMessage = "Permission denied error: " + pEx.Message;
			}
			catch (System.Exception Ex)
			{
				StatusMessage = "No location found error: " + Ex.Message;
			}
		}
		#endregion
		#endregion
		#region 3.4 event
		private void ConnectionChanged(object sender, System.EventArgs e)
		{
			if (Connectivity.NetworkAccess == NetworkAccess.Internet)
			{
				GetHome.IsEnabled = true;
				GetLocation.IsEnabled = true;
			}
			else
			{
				GetHome.IsEnabled = false;
				GetLocation.IsEnabled = false;
			}
		}
		#endregion

		private async void SaveProperty_Clicked(object sender, System.EventArgs e)
		{
			if (IsValid() == false)
			{
				StatusMessage = "Please fill in all required fields";
				StatusColor = Color.Red;
				#region 3.5
				Vibration.Vibrate(5000);
				#endregion
			}
			else
			{
				Repository.SaveProperty(Property);
				await Navigation.PopToRootAsync();
			}
		}

		public bool IsValid()
		{
			if (string.IsNullOrEmpty(Property.Address)
				|| Property.Beds == null
				|| Property.Price == null
				|| Property.AgentId == null)
				return false;

			return true;
		}

		private async void CancelSave_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PopToRootAsync();
		}
		private async void GetLocation_Clicked(object sender, System.EventArgs e)
		{
			GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Default);
			try
			{
				Property.Location = await Geolocation.GetLocationAsync(request);

				var preadress = await Geocoding.GetPlacemarksAsync(Property.Location.Latitude, Property.Location.Longitude);

				var pre = preadress.FirstOrDefault();

				Property.Address = preadress?.FirstOrDefault().ToString();
			}
			catch (FeatureNotSupportedException fnsEx)
			{
				StatusMessage = "Feature not supportet error: " + fnsEx.Message;
			}
			catch (FeatureNotEnabledException fneEx)
			{
				StatusMessage = "Feature not Enabled error: " + fneEx.Message;
			}
			catch (PermissionException pEx)
			{
				StatusMessage = "Permission denied error: " + pEx.Message;
			}
			catch (System.Exception Ex)
			{
				StatusMessage = "No location found error: " + Ex.Message;
			}

		}

		private async void GetHome_Clicked(object sender, System.EventArgs e)
		{
			try
			{
				IEnumerable<Location> locations = await Geocoding.GetLocationsAsync(Property.Address);

				Property.Location = locations?.FirstOrDefault();
			}
			catch (FeatureNotSupportedException fnsEx)
			{
				StatusMessage = "Feature not supportet error: " + fnsEx.Message;
			}
			catch (FeatureNotEnabledException fneEx)
			{
				StatusMessage = "Feature not Enabled error: " + fneEx.Message;
			}
			catch (PermissionException pEx)
			{
				StatusMessage = "Permission denied error: " + pEx.Message;
			}
			catch (System.Exception Ex)
			{
				StatusMessage = "No location found error: " + Ex.Message;
			}
		}
	}
}