using CommunityToolkit.Mvvm.ComponentModel;

using Skr.Tebloman.Common.Data.Model;

namespace Skr.Tebloman.Ui.ViewModels
{
    public sealed class PlaceholderTagListItemViewModel : ObservableObject
    {
        public PlaceholderTag Item { get; }

        public PlaceholderTagListItemViewModel(PlaceholderTag item)
        {
            Item = item;
        }

        public override string ToString()
        {
            return Item.Pattern;
        }
    }
}
