using System;
using System.Collections.Generic;
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
	public partial class SettingsPage : ContentPage
	{
		public float PitchValue { get; set; }
		public float VolumeValue { get; set; }
		public SettingsPage()
		{
			//Preferences.Clear("TextToSpeech");
			LoadSettings();
			this.BindingContext = this;
			InitializeComponent();
			
		}
		public void OnSave_Clicked(object sender, EventArgs e)
		{
			Preferences.Set(nameof(PitchValue), PitchValue, "TextToSpeech");
			Preferences.Set(nameof(VolumeValue), VolumeValue, "TextToSpeech");

		}
		protected override void OnAppearing()
		{
			base.OnAppearing();
			LoadSettings();
		}
		public void LoadSettings()
		{
			PitchValue = Preferences.Get(nameof(PitchValue), 1f, "TextToSpeech");
			VolumeValue = Preferences.Get(nameof(VolumeValue), 0.75f, "TextToSpeech");
		}
	}
}