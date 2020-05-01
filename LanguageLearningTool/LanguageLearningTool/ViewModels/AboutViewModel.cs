using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;

using Xamarin.Forms;

namespace LanguageLearningTool.ViewModels
{
	public class AboutViewModel : BaseViewModel
	{
		public AboutViewModel()
		{
			Title = "About";

			OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
			PlayTestCommand = new Command(()=>
			{
				var assembly = typeof(App).GetTypeInfo().Assembly;
				var infos = assembly.GetManifestResourceNames();
				Stream audioStream = assembly.GetManifestResourceStream("LanguageLearningTool.Resources." + "test.mp3");

				var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
				player.Load(audioStream);
				player.Play();
			});
		}

		public ICommand OpenWebCommand { get; }
		public ICommand PlayTestCommand { get; }
	}
}