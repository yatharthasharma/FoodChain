/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create March 2019
 * @description
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Parse;

namespace FoodChain.Fragments
{
    public class NearbyFragment : Android.Support.V4.App.Fragment
    {
        private List<ParseObject> shopsFetch = new List<ParseObject>();
        private RecyclerView shopCardView;
        private NearbyFragment_Adapter recyclerAdapter;
        private GridLayoutManager layoutManager;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.fragment_main_nearby, container, false);
            Activity.Title = "Nearby";
            //RV
            shopCardView = view.FindViewById<RecyclerView>(Resource.Id.fragment_main_nearby);
            layoutManager = new GridLayoutManager(Context, 1);
            shopCardView.SetLayoutManager(layoutManager);
            shopCardView.SetItemAnimator(new DefaultItemAnimator());
            //ADAPTER
            recyclerAdapter = new NearbyFragment_Adapter(shopsFetch);
            return view;
            //return base.OnCreateView(inflater, container, savedInstanceState);
        }
        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            AlertDialog dialog = HelperFunctions.UseAlertBoxProgressBar(Context, "Loading...");
            await GetShops();
            recyclerAdapter.ItemClick += OnItemClick;
            shopCardView.SetAdapter(recyclerAdapter);
            HelperFunctions.HideAlertBoxProgressBar(dialog);
        }
        public async Task GetShops()
        {
            shopsFetch.Clear();
            var query = ParseObject.GetQuery("Shop");
            IEnumerable<ParseObject> x = await query.FindAsync();
            foreach (var item in x)
            {
                shopsFetch.Add(item);
            }
        }
        void OnItemClick(object sender, int position)
        {

        }
    }
}