using System;

using LanguageLearningTool.Models;

namespace LanguageLearningTool.ViewModels
{
	public class ItemDetailViewModel : ViewModelBase
	{
		public Item Item { get; set; }
		public ItemDetailViewModel(Item item = null)
		{
			Title = item?.Text;
			Item = item;
		}
	}
}
