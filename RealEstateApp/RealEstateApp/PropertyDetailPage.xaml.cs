using RealEstateApp.Models;
using RealEstateApp.Services;
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
	}
}