using I4_QM_app.Views;
using I4_QM_app.Views.Recipes;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace I4_QM_app
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(OrderDetailPage), typeof(OrderDetailPage));
            Routing.RegisterRoute(nameof(HistoryDetailPage), typeof(HistoryDetailPage));
            Routing.RegisterRoute(nameof(RecipeDetailPage), typeof(RecipeDetailPage));
            Routing.RegisterRoute(nameof(FeedbackPage), typeof(FeedbackPage));
            Routing.RegisterRoute(nameof(NewRecipePage), typeof(NewRecipePage));
            Routing.RegisterRoute(nameof(TransformRecipePage), typeof(TransformRecipePage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        private async void OnLogoutItemClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    // TODO better place?
    namespace Controls
    {
        public class FlyoutItemIconFont : FlyoutItem
        {
            public static readonly BindableProperty IconGlyphProperty = BindableProperty.Create(nameof(IconGlyphProperty), typeof(string), typeof(FlyoutItemIconFont), string.Empty);
            public string IconGlyph
            {
                get { return (string)GetValue(IconGlyphProperty); }
                set { SetValue(IconGlyphProperty, value); }
            }
        }

        public class MenuItemIconFont : MenuItem
        {
            public static readonly BindableProperty IconGlyphProperty = BindableProperty.Create(nameof(IconGlyphProperty), typeof(string), typeof(FlyoutItemIconFont), string.Empty);
            public string IconGlyph
            {
                get { return (string)GetValue(IconGlyphProperty); }
                set { SetValue(IconGlyphProperty, value); }
            }
        }
    }

}


