/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create June 2019
 * @description
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;

namespace FoodChain
{
    class DatabaseOperations
    {
        // Fetch all categories from the database
        // Usage in Fragments/ShopFragment.cs
        public static async Task GetCategories(List<ParseObject> dataset)
        {
            dataset.Clear();
            var query = ParseObject.GetQuery("Category");
            IEnumerable<ParseObject> x = await query.FindAsync();
            foreach (var item in x)
            {
                dataset.Add(item);
            }
        }
        // Fetch the current category using object id and use getcategories() to retrieve items from it
        // Usage in Fragments/CategoryFragment.cs
        public static async Task GetCategoryInfo(string objectId, List<ParseObject> dataset)    
        {
            ParseQuery<ParseObject> query = ParseObject.GetQuery("Category");
            ParseObject result = await query.GetAsync(objectId);
            await GetItemsInCategory(result, dataset);
        }
        // Fetch items from the category provided
        // Usage in Fragments/CategoryFragment.cs
        public static async Task GetItemsInCategory(ParseObject category, List<ParseObject> dataset)       
        {
            dataset.Clear();
            ParseRelation<ParseObject> relation = category.Get<ParseRelation<ParseObject>>("products");
            //var relation = category.GetRelation<ParseObject>("products");
            IEnumerable<ParseObject> products = await relation.Query.FindAsync();
            foreach (var item in products)
            {
                dataset.Add(item);
            }
        }
        // Fetch and return the current item
        // Usage in Fragments/ItemFragment.cs
        public static async Task<ParseObject> GetItemInfo(string itemId)     // Get the current item using item id
        {
            ParseQuery<ParseObject> query = ParseObject.GetQuery("Product");
            ParseObject result = await query.GetAsync(itemId);
            return result;
        }
        // Fetch products/items similar to the current item to be displayed in 'More Like This...' column
        // Usage in Fragments/ItemFragment.cs
        public static async Task GetDataForMoreLikeThisDisplay(List<ParseObject> dataset)
        {
            dataset.Clear();
            var query = ParseObject.GetQuery("Product").Limit(6);
            IEnumerable<ParseObject> x = await query.FindAsync();
            foreach (var item in x)
            {
                dataset.Add(item);
            }
        }
        // Fetch items from a given individual cart
        // Usage in ...
        public static async Task GetIndividualCartItems(ParseObject cart, List<ParseObject> dataset)
        {
            dataset.Clear();
            ParseRelation<ParseObject> relation = cart.Get<ParseRelation<ParseObject>>("items");
            IEnumerable<ParseObject> products = await relation.Query.FindAsync();
            foreach (var item in products)
            {
                dataset.Add(item);
            }
        }

    }
}
