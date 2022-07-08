using I4_QM_app.Helpers;
using I4_QM_app.Models;
using I4_QM_app.Services;
using LiteDB;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for AdditivesListPage.
    /// </summary>
    public class AdditivesViewModel : BaseViewModel
    {
        private readonly IDataService<Additive> additivesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditivesViewModel"/> class.
        /// </summary>
        /// <param name="additivesService">Additives Service.</param>
        public AdditivesViewModel(IDataService<Additive> additivesService)
        {
            Title = "Additives";
            Descending = true;
            Additives = new SortableObservableCollection<Additive>() { SortingSelector = i => i.Id, Descending = Descending };
            this.additivesService = additivesService ?? throw new ArgumentNullException(nameof(additivesService));

            LoadAdditivesCommand = new Command(async () => await ExecuteLoadAdditivesCommand());

            SortByCommand = new Command<string>(
                execute: async (string arg) =>
                {
                    arg = arg.Trim();

                    // works
                    if (arg == "Id")
                    {
                        await SortBy(i => i.Id);
                    }

                    if (arg == "Name")
                    {
                        await SortBy(i => i.Name);
                    }

                    // https://stackoverflow.com/questions/16213005/how-to-convert-a-lambdaexpression-to-typed-expressionfunct-t
                    // only works with id?! "Specified cast is not valid"
                    //if (typeof(Order).GetProperty(arg) != null)
                    //{
                    //    ParameterExpression parameter = Expression.Parameter(typeof(Order), "i");
                    //    MemberExpression memberExpression = Expression.Property(parameter, typeof(Order).GetProperty(arg));
                    //    LambdaExpression lambda = Expression.Lambda(memberExpression, parameter);

                    //    Console.WriteLine(lambda.ToString());

                    //    await SortBy((Func<Order, object>)lambda.Compile());
                    //}

                    // https://stackoverflow.com/questions/10655761/convert-string-into-func
                    // "compiler error"
                    //var str = "i => i." + arg;
                    //Console.WriteLine(str);
                    //var func = await CSharpScript.EvaluateAsync<Func<Order, object>>(str);
                    //await SortBy(func);
                });

            DisableCommand = new Command(execute: () => { }, canExecute: () => { return false; });
        }

        /// <summary>
        /// Gets Additives Collection (sortable).
        /// </summary>
        public SortableObservableCollection<Additive> Additives { get; }

        /// <summary>
        /// Gets command to load additives from db.
        /// </summary>
        public Command LoadAdditivesCommand { get; }

        /// <summary>
        /// Gets command to disable.
        /// </summary>
        public Command DisableCommand { get; }

        /// <summary>
        /// Gets command to sort collection.
        /// </summary>
        public Command SortByCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether collection is sorted descending or ascending.
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// Sets IsBusy when onAppearing.
        /// </summary>
        public void OnAppearing()
        {
            IsBusy = true;
        }

        /// <summary>
        /// Sort collection from predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        private async Task SortBy(Func<Additive, object> predicate)
        {
            Additives.SortingSelector = predicate;
            Additives.Descending = Descending;
            Descending = !Descending;
            await ExecuteLoadAdditivesCommand();
        }

        /// <summary>
        /// Load additives from db.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task ExecuteLoadAdditivesCommand()
        {
            IsBusy = true;
            var fs = App.DB.GetStorage<string>("myImages");

            try
            {
                Additives.Clear();
                var additives = await additivesService.GetItemsAsync();

                foreach (var additive in additives)
                {
                    Additives.Add(additive);
                    additive.Image = GetAdditiveImage(fs, additive.Id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Returns saved image for additive in filestorage or no_image.
        /// </summary>
        /// <param name="fs">ILiteStorage.</param>
        /// <param name="id">Id.</param>
        /// <returns>ImageSource.</returns>
        private ImageSource GetAdditiveImage(ILiteStorage<string> fs, string id)
        {
            LiteFileInfo<string> file = fs.FindById(id);

            if (file != null)
            {
                return ImageSource.FromStream(() => file.OpenRead());
            }
            else
            {
                return ImageSource.FromFile("no_image.png");
            }
        }
    }
}
