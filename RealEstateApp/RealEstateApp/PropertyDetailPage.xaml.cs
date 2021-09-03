using RealEstateApp.Models;
using RealEstateApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TinyIoC;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PropertyDetailPage : ContentPage
	{
		public PropertyDetailPage(PropertyListItem propertyListItem)
		{
			InitializeComponent();

			Property = propertyListItem.Property;

			IRepository Repository = TinyIoCContainer.Current.Resolve<IRepository>();
			Agent = Repository.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);

			TabCommand = new Command(OnTabbed);

			BindingContext = this;
		}

		public Agent Agent { get; set; }

		public Property Property { get; set; }
		public CancellationTokenSource Cts { get; set; }
		public ICommand TabCommand { get; }

		private async void EditProperty_Clicked(object sender, System.EventArgs e)
		{
			await Navigation.PushAsync(new AddEditPropertyPage(Property));
		}
		private async void TextReader_Clicked(object sender, System.EventArgs e)
		{
			if (Cts == null || Cts.IsCancellationRequested)
			{
				Cts = new CancellationTokenSource();
				TextReaderButton.BackgroundColor = Color.Red;
				TextReaderButton.Text = "\uf04d";
				await TextToSpeech.SpeakAsync(Property.Description, Cts.Token);
				TextReaderReset();
			}
			else
			{
				TextReaderReset();
			}

		}
		private void TextReaderReset()
		{
			TextReaderButton.BackgroundColor = Color.Green;
			TextReaderButton.Text = "\uf04b";
			Cts.Cancel();
		}

		private async void OnTabbed(object sender)
		{
			Property property = (Property)sender;
			await Navigation.PushAsync(new ImageListPage(property));
		}

		private async void Email_Tapped(object sender, System.EventArgs e)
		{
			var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var attachmentFilePath = Path.Combine(folder, "property.txt");
			File.WriteAllText(attachmentFilePath, $"{Property.Address}");

			EmailMessage message = new EmailMessage
			{
				Subject = "Olla",
				Body = "nice " + Property.Vendor.FullName,
				To = new List<string>() { Property.Vendor.Email },
				Attachments = new List<EmailAttachment>() { new EmailAttachment(attachmentFilePath) }
			};
			await Email.ComposeAsync(message);
		}

		private async void Phone_Tapped(object sender, System.EventArgs e)
		{
			string phone = "Phone";
			string sms = "SMS";
			string whatdo = await DisplayActionSheet("What to do", "Back", null, phone, sms);
			if (whatdo == phone)
			{
				try
				{
					PhoneDialer.Open(Property.Vendor.Phone);
				}
				catch (FeatureNotSupportedException ex)
				{
					//not supported
				}
			}
			else if (whatdo == sms)
			{
				try
				{
					SmsMessage msg = new SmsMessage
					{
						Body = "hey " + Property.Vendor.FullName + " angående " + Property.Address,
						Recipients = new List<string>() { Property.Vendor.Phone }
					};
					await Sms.ComposeAsync();
				}
				catch (FeatureNotSupportedException ex)
				{
					//not supported
				}
			}
		}

		private async void Map_Clicked(object sender, EventArgs e)
		{
			var location = new Location((double)Property.Latitude, (double)Property.Longitude);

			var options = new MapLaunchOptions()
			{
				Name = "Bo here",
				NavigationMode = NavigationMode.None
			};

			await location.OpenMapsAsync();
		}

		private async void Direction_Clicked(object sender, EventArgs e)
		{
			var options = new MapLaunchOptions()
			{
				Name = "Bo here",
				NavigationMode = NavigationMode.Driving
			};
			await Map.OpenAsync(new Location((double)Property.Latitude, (double)Property.Longitude), options);
		}

		private void Linkbtn_Clicked(object sender, EventArgs e)
		{
			BrowserLaunchOptions blo = new BrowserLaunchOptions()
			{
				LaunchMode = BrowserLaunchMode.SystemPreferred,
				PreferredControlColor = Color.Red,
				PreferredToolbarColor = Color.Blue,
				TitleMode = BrowserTitleMode.Show
			};
			Browser.OpenAsync("https://www.EucSyd.dk", blo);
		}
		private async void PDF_Clicked(object sender, EventArgs e)
		{
			await Launcher.OpenAsync(new OpenFileRequest
			{
				File = new ReadOnlyFile("contract.pdf")
			});
		}
	}
}