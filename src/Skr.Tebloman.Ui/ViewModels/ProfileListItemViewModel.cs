using CommunityToolkit.Mvvm.ComponentModel;
using Skr.Tebloman.Common.Data.Model;

namespace Skr.Tebloman.Ui.ViewModels
{
    public sealed class ProfileListItemViewModel : ObservableObject
    {
        public Profile Item { get; }

        public ProfileListItemViewModel(Profile profile)
        {
            Item = profile;
        }

        public override string ToString()
        {
            return Item.Name;
        }
    }
}
