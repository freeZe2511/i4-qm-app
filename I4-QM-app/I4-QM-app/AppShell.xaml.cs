using I4_QM_app.Views;
using I4_QM_app.Views.Recipes;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace I4_QM_app
{
    /// <summary>
    /// Main App Shell.
    /// </summary>
    public partial class AppShell : Xamarin.Forms.Shell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppShell"/> class.
        /// </summary>
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(OrderDetailPage), typeof(OrderDetailPage));
            Routing.RegisterRoute(nameof(HistoryDetailPage), typeof(HistoryDetailPage));
            Routing.RegisterRoute(nameof(RecipeDetailPage), typeof(RecipeDetailPage));
            Routing.RegisterRoute(nameof(FeedbackPage), typeof(FeedbackPage));
            Routing.RegisterRoute(nameof(NewRecipePage), typeof(NewRecipePage));
            Routing.RegisterRoute(nameof(TransformRecipePage), typeof(TransformRecipePage));
        }

        /// <summary>
        /// Handles log out.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">EventArgs.</param>
        private async void OnLogoutItemClicked(object sender, EventArgs e)
        {
            string userId = Preferences.Get("UserID", string.Empty);

            await App.ConnectionService.HandlePublishMessage("disconnected", userId);
            await Shell.Current.GoToAsync("//LoginPage");

            Preferences.Clear();
        }
    }
}