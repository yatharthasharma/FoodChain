/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create March 2019
 * @description
*/

using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Parse;

namespace FoodChain.Fragments
{
    public class CreateCartFragment : Android.Support.V4.App.Fragment
    {
        private CartFragment cartFragment;
        TextInputEditText cartName;
        RadioGroup cartType;
        RadioButton cartTypeSelected;
        Button create;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_createcart, container, false);
            create = view.FindViewById<Button>(Resource.Id.fragment_createcart_createbutton);
            create.Click += async delegate
            {
                cartType = view.FindViewById<RadioGroup>(Resource.Id.fragment_createcart_carttyperadio);
                cartName = view.FindViewById<TextInputEditText>(Resource.Id.fragment_createcart_nametext);
                cartTypeSelected = view.FindViewById<RadioButton>(cartType.CheckedRadioButtonId);
                if (cartName.Text == "")
                    cartName.Error = "Cart name cannot be empty";
                else
                {
                    if (cartTypeSelected.Text == "Individual")
                    {
                        ParseObject newCart = new ParseObject("IndividualShoppingCart")
                        {
                            ["owner"] = ParseUser.CurrentUser,
                            ["name"] = cartName.Text,
                            ["price"] = 0
                        };
                        await newCart.SaveAsync();
                        if (cartFragment == null)
                            cartFragment = new CartFragment();
                        Android.Support.V4.App.FragmentManager fragmentManager = Activity.SupportFragmentManager;
                        Android.Support.V4.App.FragmentTransaction fragmentTransaction = fragmentManager.BeginTransaction();
                        fragmentManager.PopBackStack();
                        fragmentTransaction.Replace(Resource.Id.content_view_fragment, cartFragment);
                        fragmentTransaction.Commit();
                    }
                    else if (cartTypeSelected.Text == "Collective")
                    {

                    }
                }
            };
            return view;
        }
    }
}