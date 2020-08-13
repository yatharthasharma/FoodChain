/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create March 2019
 * @description
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Parse;

namespace FoodChain.Fragments
{
    public class CartFragment : Android.Support.V4.App.Fragment
    {
        private CreateCartFragment createCartFragment;
        private readonly List<ParseObject> cartsFetch = new List<ParseObject>();
        RecyclerView cartsView;
        CartFragment_Adapter recyclerAdapter;
        LinearLayoutManager layoutManager;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Activity.Title = "Shopping Carts";
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_main_cart, container, false);
            //RV
            cartsView = view.FindViewById<RecyclerView>(Resource.Id.fragment_main_cart);
            layoutManager = new LinearLayoutManager(Context);
            cartsView.SetLayoutManager(layoutManager);
            cartsView.SetItemAnimator(new DefaultItemAnimator());
            //ADAPTER
            recyclerAdapter = new CartFragment_Adapter(cartsFetch);
            FloatingActionButton fab = view.FindViewById<FloatingActionButton>(Resource.Id.fragment_main_cart_fab);
            fab.Click += FabOnClick;
            return view;
        }
        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            AlertDialog dialog = HelperFunctions.UseAlertBoxProgressBar(Context, "Loading...");
            await GetIndividualCarts();
            //await GetCollectiveCarts();
            recyclerAdapter.ItemClick += OnItemClick;
            cartsView.SetAdapter(recyclerAdapter);
            HelperFunctions.HideAlertBoxProgressBar(dialog);
        }

        /*private async Task GetCollectiveCarts()
        {
            var query = ParseObject.GetQuery("CollectiveShoppingCart").WhereEqualTo("owner", ParseUser.CurrentUser);
            IEnumerable<ParseObject> x = await query.FindAsync();
            foreach (var item in x)
            {
                cartsFetch.Add(item);
            }
        }*/

        private async Task GetIndividualCarts()
        {
            cartsFetch.Clear();
            var query = ParseObject.GetQuery("IndividualShoppingCart").WhereEqualTo("owner", ParseUser.CurrentUser);
            IEnumerable<ParseObject> x = await query.FindAsync();
            foreach (var item in x)
            {
                cartsFetch.Add(item);
            }
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            if (createCartFragment == null)
                createCartFragment = new CreateCartFragment();
            Android.Support.V4.App.FragmentManager fragmentManager = Activity.SupportFragmentManager;
            Android.Support.V4.App.FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_view_fragment, createCartFragment);
            fragmentTransaction.AddToBackStack(null);
            fragmentTransaction.Commit();
        }
        void OnItemClick(object sender, int position)
        {

        }
    }
}