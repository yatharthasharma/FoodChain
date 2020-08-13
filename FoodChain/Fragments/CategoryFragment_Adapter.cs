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
    class CategoryFragment_Adapter : RecyclerView.Adapter
    {
        private readonly List<ParseObject> dataset;
        public override int ItemCount => dataset.Count;
        public event EventHandler<int> ItemClick;
        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
        public CategoryFragment_Adapter(List<ParseObject> dataset)
        {
            this.dataset = dataset;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ViewHolder view = holder as ViewHolder;
            view.categoryName.Text = dataset[position].Get<string>("name");
            if (dataset[position].TryGetValue("image", out ParseFile image))                // since image might not have been uploaded, check if it actually exists
                view.categoryImage.SetImageBitmap(HelperFunctions.GetImageBitmapFromUrl(image.Url.ToString()));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.cardview, parent, false);
            ViewHolder viewHolder = new ViewHolder(view, OnClick);
            return viewHolder;
        }
        internal class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView categoryName;
            public ImageView categoryImage;
            public ViewHolder(View view, Action<int> listener) : base(view)
            {
                categoryName = view.FindViewById<TextView>(Resource.Id.cardview_category_name);
                categoryImage = view.FindViewById<ImageView>(Resource.Id.cardview_category_image);
                view.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}