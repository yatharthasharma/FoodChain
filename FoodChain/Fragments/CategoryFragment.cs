/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create March 2019
 * @description
*/

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Parse;

namespace FoodChain.Fragments
{
    public class CategoryFragment : Android.Support.V4.App.Fragment
    {
        private ItemFragment itemFragment;
        private readonly List<ParseObject> itemsFetch = new List<ParseObject>();
        string categoryId, activityName;
        RecyclerView itemCardView;
        CategoryFragment_Adapter recyclerAdapter;
        GridLayoutManager layoutManager;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle arguments = Arguments;
            categoryId = arguments.GetString("objectid");
            activityName = arguments.GetString("categoryname");
            Activity.Title = activityName;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_main_category, container, false);
            //Recycler View settings
            itemCardView = view.FindViewById<RecyclerView>(Resource.Id.fragment_main_category);
            layoutManager = new GridLayoutManager(Context, 2);
            itemCardView.SetLayoutManager(layoutManager);
            itemCardView.SetItemAnimator(new DefaultItemAnimator());
            recyclerAdapter = new CategoryFragment_Adapter(itemsFetch);
            return view;
        }
        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            AlertDialog dialog = HelperFunctions.UseAlertBoxProgressBar(Context, "Loading...");
            await DatabaseOperations.GetCategoryInfo(categoryId, itemsFetch);       // fetch information from DB to be displayed
            recyclerAdapter.ItemClick += OnItemClick;
            itemCardView.SetAdapter(recyclerAdapter);
            HelperFunctions.HideAlertBoxProgressBar(dialog);
        }

        void OnItemClick(object sender, int position)
        {
            Bundle arguments = new Bundle();
            if (itemFragment == null)
                itemFragment = new ItemFragment();
            arguments.PutString("objectid", GetItemObjectId(position));
            arguments.PutString("activityName", activityName);  // CHANGE THIS to reflect the NAME OF THE ITEM instead of the category
            itemFragment.Arguments = arguments;
            Android.Support.V4.App.FragmentManager fragmentManager = Activity.SupportFragmentManager;
            Android.Support.V4.App.FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.content_view_fragment, itemFragment);
            fragmentTransaction.AddToBackStack(null);
            fragmentTransaction.Commit();
        }
        // Get object ID of item that is clicked to retrieve information about it in the next fragment
        private string GetItemObjectId(int position)
        {
            string itemInfo = itemsFetch.ElementAt<ParseObject>(position).ObjectId;
            return itemInfo;
        }
    }
}