using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.ObjectModel;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace RealEstateApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PropertyListPage : ContentPage
	{
		IRepository Repository;
		public ObservableCollection<PropertyListItem> PropertiesCollection { get; } = new ObservableCollection<PropertyListItem>();
		private Location _location;

		public Location Location
		{
			get { return _location; }
			set { _location = value; }
		}

		public PropertyListPage()
		{
			InitializeComponent();

			Repository = TinyIoCContainer.Current.Resolve<IRepository>();
			LoadProperties();
			BindingContext = this;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			LoadProperties();
		}

		void OnRefresh(object sender, EventArgs e)
		{
			var list = (ListView)sender;
			LoadProperties();
			list.IsRefreshing = false;
		}

		async void LoadProperties()
		{
			PropertiesCollection.Clear();
			var items = Repository.GetProperties();

			foreach (Property item in items)
			{
				PropertyListItem _p = new PropertyListItem(item);
				_p.Distance = await CalDistance(item);

				PropertiesCollection.Add(_p);
			}
		}
		private async Task<double> CalDistance(Property property)
		{
			double distance = new double();
			if (property.Latitude != null || property.Longitude != null)
			{
				if (Location == null)
				{
					try
					{
						this.Location = await Geolocation.GetLocationAsync();
					}
					catch (Exception)
					{

						throw;
					}
				}
				distance = Location.CalculateDistance((double)property.Latitude, (double)property.Longitude, DistanceUnits.Kilometers);
			}
			return distance;
		}
		private async void ItemsListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			await Navigation.PushAsync(new PropertyDetailPage(e.Item as PropertyListItem));
		}

		private async void AddProperty_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new AddEditPropertyPage());
		}

		private async void Sortbutton_Clicked(object sender, EventArgs e)
		{
			this.Location = await Geolocation.GetLastKnownLocationAsync();
			if (Location == null)
			{
				this.Location = await Geolocation.GetLocationAsync();
			}

		}
	}
}