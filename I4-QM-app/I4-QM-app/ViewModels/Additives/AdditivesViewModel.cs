using I4_QM_app.Helpers;
using I4_QM_app.Models;
using LiteDB;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class AdditivesViewModel : BaseViewModel
    {
        public SortableObservableCollection<Additive> Additives { get; }

        public Command LoadAdditivesCommand { get; }

        public Command DisableCommand { get; }

        public Command SortByCommand { get; }

        public bool Descending { get; set; }

        public AdditivesViewModel()
        {
            Title = "Additives";
            Descending = true;
            Additives = new SortableObservableCollection<Additive>() { SortingSelector = i => i.Id, Descending = Descending };

            LoadAdditivesCommand = new Command(async () => await ExecuteLoadAdditivesCommand());

            SortByCommand = new Command<string>(
                execute: async (string arg) =>
                {
                    arg = arg.Trim();

                    // works
                    if (arg == "Id") await SortBy(i => i.Id);
                    if (arg == "Name") await SortBy(i => i.Name);

                    // https://stackoverflow.com/questions/16213005/how-to-convert-a-lambdaexpression-to-typed-expressionfunct-t
                    // only works with id?! Specified cast is not valid
                    //if (typeof(Order).GetProperty(arg) != null)
                    //{
                    //    ParameterExpression parameter = Expression.Parameter(typeof(Order), "i");
                    //    MemberExpression memberExpression = Expression.Property(parameter, typeof(Order).GetProperty(arg));
                    //    LambdaExpression lambda = Expression.Lambda(memberExpression, parameter);

                    //    Console.WriteLine(lambda.ToString());

                    //    await SortBy((Func<Order, object>)lambda.Compile());
                    //}

                    // https://stackoverflow.com/questions/10655761/convert-string-into-func
                    // compiler error
                    //var str = "i => i." + arg;
                    //Console.WriteLine(str);
                    //var func = await CSharpScript.EvaluateAsync<Func<Order, object>>(str);
                    //await SortBy(func);


                });

            DisableCommand = new Command(execute: () => { }, canExecute: () => { return false; });
        }

        public void OnAppearing()
        {
            IsBusy = true;
            //SelectedOrder = null;
        }

        private async Task SortBy(Func<Additive, object> predicate)
        {
            Additives.SortingSelector = predicate;
            Additives.Descending = Descending;
            Descending = !Descending;
            await ExecuteLoadAdditivesCommand();
        }

        private async Task ExecuteLoadAdditivesCommand()
        {
            IsBusy = true;

            try
            {
                Additives.Clear();
                var additives = await App.AdditivesDataService.GetItemsAsync();

                foreach (var additive in additives)
                {
                    Additives.Add(additive);

                    var fs = App.DB.GetStorage<string>("myImages");
                    LiteFileInfo<string> file = fs.FindById(additive.Id);

                    if (file != null)
                    {
                        additive.Image = ImageSource.FromStream(() => file.OpenRead());
                    }
                    else
                    {
                        additive.Image = ImageSource.FromFile("no_image.png");
                    }

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
    }
}
