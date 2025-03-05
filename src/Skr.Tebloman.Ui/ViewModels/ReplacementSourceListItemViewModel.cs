using CommunityToolkit.Mvvm.ComponentModel;

using Skr.Tebloman.Common.Data.Model;

namespace Skr.Tebloman.Ui.ViewModels
{
    public sealed class ReplacementSourceListItemViewModel : ObservableObject
    {
        public ReplacementSource Item { get; }

        public ReplacementSourceListItemViewModel(ReplacementSource item)
        {
            Item = item;
        }

        public override string ToString()
        {
            return Item.Name;
        }
    }
}
