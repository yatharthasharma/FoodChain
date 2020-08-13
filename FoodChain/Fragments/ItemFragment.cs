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
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Parse;

namespace FoodChain.Fragments
{
    public class ItemFragment : Android.Support.V4.App.Fragment
    {
        string itemId;
        ImageView itemImage;
        List<ParseObject> morelikethisData = new List<ParseObject>();
        List<ParseObject> individualCartsFetch = new List<ParseObject>();
        RecyclerView itemRecyclerView;
        ShopFragment_Adapter recyclerAdapter;
        LinearLayoutManager layoutManager;
        TextView itemQuantity, itemDescription, itemName;
        //private TextView itemPrice;
        Button addToCart;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Bundle arguments = Arguments;
            itemId = arguments.GetString("objectid");
            Activity.Title = arguments.GetString("activityName");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_main_item, container, false);
            itemName = view.FindViewById<TextView>(Resource.Id.fragment_main_item_name);
            itemImage = view.FindViewById<ImageView>(Resource.Id.fragment_main_item_image);
            //itemPrice = view.FindViewById<TextView>(Resource.Id.fragment_main_item_price);
            itemQuantity = view.FindViewById<TextView>(Resource.Id.fragment_main_item_quantity);
            itemDescription = view.FindViewById<TextView>(Resource.Id.fragment_main_item_itemdescription_info);
            addToCart = view.FindViewById<Button>(Resource.Id.fragment_main_item_addtocartbutton);
            // Recyclerview for 'More Like This' column -
            itemRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.fragment_main_item_recyclerview);
            layoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false);
            itemRecyclerView.SetLayoutManager(layoutManager);
            itemRecyclerView.SetItemAnimator(new DefaultItemAnimator());
            recyclerAdapter = new ShopFragment_Adapter(morelikethisData);
            return view;
        }

        public override async void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            AlertDialog dialog = HelperFunctions.UseAlertBoxProgressBar(Context, "Loading...");
            ParseObject product = await DatabaseOperations.GetItemInfo(itemId); // current item
            //itemPrice.Text = "£" + product.Get<int>("price").ToString();
            itemName.Text = product.Get<string>("name");
            itemQuantity.Text = product.Get<string>("quantity") + " - £" + product.Get<int>("price").ToString();
            itemDescription.Text = product.Get<string>("itemdescription");
            if (product.TryGetValue("image", out ParseFile image))                // since image might not be present, check if it actually exists
                itemImage.SetImageBitmap(HelperFunctions.GetImageBitmapFromUrl(image.Url.ToString()));
            await DatabaseOperations.GetDataForMoreLikeThisDisplay(morelikethisData);  // get items for 'more like this..'
            HelperFunctions.HideAlertBoxProgressBar(dialog);
            // RecyclerView setup
            recyclerAdapter.ItemClick += OnItemClick;
            itemRecyclerView.SetAdapter(recyclerAdapter);
            // 'Add to Cart' button click handler
            addToCart.Click += delegate
            {
                DialogWithRadio(Context, product);
            };

        }
        private async Task GetIndividualCarts(ParseObject product)
        {
            individualCartsFetch.Clear();
            // individual carts of the user - excluding the ones which already have the product/item in them that the user is trying to add 
            var query = ParseObject.GetQuery("IndividualShoppingCart").WhereEqualTo("owner", ParseUser.CurrentUser).WhereNotEqualTo("items", product);
            IEnumerable<ParseObject> x = await query.FindAsync();
            foreach (var item in x)
            {
                individualCartsFetch.Add(item);
            }
        }
        private async void DialogWithRadio(Context context, ParseObject product)
        {
            await GetIndividualCarts(product);
            List<string> stringFetch = new List<string>();
            foreach (ParseObject item in individualCartsFetch)
            {
                stringFetch.Add(item.Get<string>("name") + " - (Individual Cart)");
            }
            /*foreach (ParseObject item in collectiveCartsFetch)
            {
                stringFetch.Add(item.Get<string>("name") + " - (Collective Cart)");
            }*/
            int index = 0;
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetTitle("Add to Cart").SetSingleChoiceItems(stringFetch.ToArray(), 0, new EventHandler<DialogClickEventArgs>(delegate (object sender, DialogClickEventArgs e)
            {
                index = e.Which;
            })).SetCancelable(false);
            builder.SetPositiveButton("OK", async delegate
            {
                AlertDialog dialog = HelperFunctions.UseAlertBoxProgressBar(Context, "Loading...");
                var relation = individualCartsFetch[index].GetRelation<ParseObject>("items");   // get relation 'items' from the cart class from Parse
                relation.Add(product);  // add product to the cart
                individualCartsFetch[index]["price"] = product.Get<int>("price") + individualCartsFetch[index].Get<int>("price");   // update cart price
                await individualCartsFetch[index].SaveAsync();
                HelperFunctions.HideAlertBoxProgressBar(dialog);
                string message = "Item added to cart: " + individualCartsFetch[index].Get<string>("name");
                Toast.MakeText(Context, message, ToastLength.Short).Show();
            }).SetNegativeButton("Cancel", delegate
            {
                Toast.MakeText(Context, "Item not added to any cart!", ToastLength.Short).Show();
            })
            .SetNeutralButton("New Cart", delegate
            {
                UseAlertBoxEditTextBar(Context, "Create a Cart", product);
            });
            builder.Show();
        }
        private void UseAlertBoxEditTextBar(Context context, string title, ParseObject product)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            int index = 0; // arbitrary value
            string[] options = new string[]{ "Individual", "Collective" };
            View alertdialogview = Activity.LayoutInflater.Inflate(Resource.Layout.alertdialog_edittext, null);
            builder.SetTitle(title).SetCancelable(false).SetSingleChoiceItems(options, 0, new EventHandler<DialogClickEventArgs>(delegate (object sender, DialogClickEventArgs e)
            {
                index = e.Which;
            })).SetView(alertdialogview);
            TextInputEditText newCartName = alertdialogview.FindViewById<TextInputEditText>(Resource.Id.alertdialog_edittext_createcart_nametext);
            builder.SetPositiveButton("Create", async delegate
            {
                if (newCartName.Text == "")
                    Toast.MakeText(Context, "You did not specify a cart name!", ToastLength.Short).Show();
                else
                {
                    if (index == 0) // if 'Individual' cart selected
                    {
                        ParseObject newCart = new ParseObject("IndividualShoppingCart")
                        {
                            ["owner"] = ParseUser.CurrentUser,
                            ["name"] = newCartName.Text,
                            ["price"] = product.Get<int>("price")
                        };
                        await newCart.SaveAsync();
                        string messagee = "Item added to the individual cart: " + newCartName.Text;
                        Toast.MakeText(Context, messagee, ToastLength.Short).Show();
                    }
                    else if (index == 1)    // if 'Collective' cart selected
                    {

                    }
                }
            }).SetNegativeButton("Cancel", delegate
            {
                Toast.MakeText(Context, "Item not added to any cart!", ToastLength.Short).Show();
            });
            builder.Show();
        }
        // Handles clicks on 'More like this ...' cards
        void OnItemClick(object sender, int position)
        {

        }
    }
}