/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create March 2019
 * @description
*/

using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Parse;

namespace FoodChain
{
    class CartFragment_Adapter : RecyclerView.Adapter
    {
        private readonly List<ParseObject> dataset;
        public override int ItemCount => dataset.Count;
        public event EventHandler<int> ItemClick;
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
        public CartFragment_Adapter(List<ParseObject> dataset)
        {
            this.dataset = dataset;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ViewHolder view = holder as ViewHolder;
            view.name.Text = dataset[position].Get<string>("name");
            view.members.Text = "Individual Cart";
            view.price.Text = "£" + dataset[position].Get<int>("price").ToString();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.cart_cardview, parent, false);
            ViewHolder viewHolder = new ViewHolder(view, OnClick);
            return viewHolder;
        }
        internal class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView name;
            public TextView members;
            public TextView price;
            public ViewHolder(View view, Action<int> listener) : base(view)
            {
                name = view.FindViewById<TextView>(Resource.Id.cart_cardview_category_name);
                members = view.FindViewById<TextView>(Resource.Id.cart_cardview_category_membernumber);
                price = view.FindViewById<TextView>(Resource.Id.cart_cardview_category_price);
                view.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}