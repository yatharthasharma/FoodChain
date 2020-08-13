/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create February 2019
 * @description
*/

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FoodChain.Fragments;
using Parse;
using AlertDialog = Android.App.AlertDialog;

namespace FoodChain
{
    [Activity(Label = "Shop", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private Android.Support.V4.App.Fragment ShopFragment;
        private Android.Support.V4.App.Fragment NearbyFragment;
        private Android.Support.V4.App.Fragment CartFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ParseUser user = ParseUser.CurrentUser;
            if(user == null)
            {
                Intent intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);
            }
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            // mail button bottom right
            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            // fab.Click += FabOnClick;
            
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();
            

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            View headerView = navigationView.GetHeaderView(0);
            TextView emailMain = headerView.FindViewById<TextView>(Resource.Id.textViewEmailMain);
            TextView nameMain = headerView.FindViewById<TextView>(Resource.Id.textViewNameMain);
            emailMain.Text = user.Email;
            nameMain.Text = user.Username;
            if (savedInstanceState == null)
            {
                Android.Support.V4.App.FragmentManager tx = SupportFragmentManager;
                tx.BeginTransaction().Replace(Resource.Id.content_view_fragment, new ShopFragment()).Commit();
            }
        }
        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                int count = SupportFragmentManager.BackStackEntryCount;
                if (count == 0)
                    base.OnBackPressed();
                else
                    SupportFragmentManager.PopBackStack();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
        // mail button bottom right
        /*private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }*/
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            Android.Support.V4.App.FragmentManager fragmentManager = SupportFragmentManager;
            int id = menuItem.ItemId;
            if (id == Resource.Id.nav_shopping)
            {
                if (ShopFragment == null)
                {
                    ShopFragment = new ShopFragment();
                }
                fragmentManager.BeginTransaction().Replace(Resource.Id.content_view_fragment, ShopFragment).Commit();
            }
            else if (id == Resource.Id.nav_cart)
            {
                if (CartFragment == null)
                {
                    CartFragment = new CartFragment();
                }
                fragmentManager.BeginTransaction().Replace(Resource.Id.content_view_fragment, CartFragment).Commit();
            }
            else if (id == Resource.Id.nav_nearby)
            {
                if (NearbyFragment == null)
                {
                    NearbyFragment = new NearbyFragment();
                }
                fragmentManager.BeginTransaction().Replace(Resource.Id.content_view_fragment, NearbyFragment).Commit();
            }
            else if (id == Resource.Id.nav_share)
            {
                Toast.MakeText(this, "This functionality is WIP, please check back in later!", ToastLength.Short).Show();
            }
            else if (id == Resource.Id.nav_report)
            {
                Toast.MakeText(this, "This functionality is WIP, please check back in later!", ToastLength.Short).Show();
            }
            else if (id == Resource.Id.nav_language)
            {
                Toast.MakeText(this, "This functionality is WIP, please check back in later!", ToastLength.Short).Show();
            }
            else if (id == Resource.Id.nav_signout)
            {
                AlertDialog dialog;
                drawer.CloseDrawer(GravityCompat.Start);
                dialog = HelperFunctions.UseAlertBoxProgressBar(this, "Logging you out...");
                ParseUser.LogOut();
                Intent intent = new Intent(this, typeof(LoginActivity));
                HelperFunctions.HideAlertBoxProgressBar(dialog);
                intent.SetFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.ClearTask);
                StartActivity(intent);
            }
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}