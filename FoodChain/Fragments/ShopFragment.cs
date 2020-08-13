/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create March 2019
 * @description
*/

using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Parse;

namespace FoodChain.Fragments
{
    public class ShopFragment : Android.Support.V4.App.Fragment
    {
        List<ParseObject> categoriesFetch = new List<ParseObject>();
        CategoryFragment categoryFragment;
        //private readonly List<string> hellotest = new List<string>() { "helo", "world", "this", "is" }; // string dataset for testing of recyclerview
        RecyclerView categoryCardView;
        ShopFragment_Adapter recyclerAdapter;
        GridLayoutManager layoutManager;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_main_shop, container, false);
            Activity.Title = "Shop by Category";
            //Recycler View settings
            categoryCardView = view.FindViewById<RecyclerView>(Resource.Id.fragment_main_shop);
            layoutManager = new GridLayoutManager(Context, 2);
            categoryCardView.SetLayoutManager(layoutManager);
            categoryCardView.SetItemAnimator(new DefaultItemAnimator());
            recyclerAdapter = new ShopFragment_Adapter(categoriesFetch);
            return view;
            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            AlertDialog dialog = HelperFunctions.UseAlertBoxProgressBar(Context, "Loading..."); // progress bar as the data is loaded from the DB
            await DatabaseOperations.GetCategories(categoriesFetch);    // fetch the data to display
            recyclerAdapter.ItemClick += OnItemClick;
            categoryCardView.SetAdapter(recyclerAdapter);
            HelperFunctions.HideAlertBoxProgressBar(dialog);    // hide the progress bar
        }
        void OnItemClick(object sender, int position)
        {
            Bundle arguments = new Bundle();
            if (categoryFragment == null)
                categoryFragment = new CategoryFragment();
            arguments.PutString("objectid",GetCategoryObjectId(position)[0]);
            arguments.PutString("categoryname", GetCategoryObjectId(position)[1]);
            categoryFragment.Arguments = arguments;
            Android.Support.V4.App.FragmentManager fragmentManager = Activity.SupportFragmentManager;
            Android.Support.V4.App.FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_view_fragment, categoryFragment);
            fragmentTransaction.AddToBackStack(null);
            fragmentTransaction.Commit();
        }
        // Get object ID of category that is clicked to retrieve information about it in the next fragment
        private string[] GetCategoryObjectId(int position)
        {
            string[] categoryInfo = { categoriesFetch.ElementAt<ParseObject>(position).ObjectId, categoriesFetch.ElementAt<ParseObject>(position).Get<string>("name") };
            return categoryInfo;
        }
    }
}