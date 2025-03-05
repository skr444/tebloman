using CommunityToolkit.Mvvm.ComponentModel;

using Skr.Tebloman.Common.Data.Model;

namespace Skr.Tebloman.Ui.ViewModels
{
    public sealed class FragmentListItemViewModel : ObservableObject
    {
        public Fragment Item { get; }

        public FragmentListItemViewModel(Fragment fragment)
        {
            Item = fragment;
        }

        public override string ToString()
        {
            return Item.Name;
        }
    }
}
