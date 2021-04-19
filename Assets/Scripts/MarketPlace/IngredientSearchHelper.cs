using System.Collections.Generic;
using System.Linq;

namespace BarthaSzabolcs.MarketPlace
{
    /// <summary>
    /// A collection of extension methods used to search/filter/order the <see cref="Ingredient"/>s,
    /// to keep the logic in one place and allow chaining the class in LINQ style.
    /// </summary>
    public static class IngredientSearchHelper
    {
        /// <summary>
        /// Returns the list ordered by <paramref name="property"/>.
        /// </summary>
        public static IList<Ingredient>
            OrderByProperty(this IEnumerable<Ingredient> ingredients, MarketPlaceGUI.OrderProperty property, bool ascending)
        {
            if (ascending)
            {
                switch (property)
                {
                    case MarketPlaceGUI.OrderProperty.Ammount:
                        return ingredients.OrderBy(x => x.Ammount).ToList();

                    case MarketPlaceGUI.OrderProperty.Price:
                        return ingredients.OrderBy(x => x.Price).ToList();

                    case MarketPlaceGUI.OrderProperty.Name:
                    default:
                        return ingredients.OrderBy(x => x.Name).ToList();
                }
            }
            else
            {
                switch (property)
                {
                    case MarketPlaceGUI.OrderProperty.Ammount:
                        return ingredients.OrderByDescending(x => x.Ammount).ToList();

                    case MarketPlaceGUI.OrderProperty.Price:
                        return ingredients.OrderByDescending(x => x.Price).ToList();

                    case MarketPlaceGUI.OrderProperty.Name:
                    default:
                        return ingredients.OrderByDescending(x => x.Name).ToList();
                }
            }
        }

        /// <summary>
        /// Return items which name's contain <paramref name="filterString"/>.
        /// 
        /// <para>
        /// If <paramref name="filterString"/> is shorter than 3 characters,
        /// simply returns the original collection.
        /// </para>
        /// 
        /// </summary>
        public static IEnumerable<Ingredient>
            FilterByName(this IEnumerable<Ingredient> ingredients, string filterString)
        {
            if (filterString.Length >= 3)
            {
                return ingredients.Where(x => x.Name.Contains(filterString));
            }
            else
            {
                return ingredients;
            }
        }

        /// <summary>
        /// Returns items which type is set to <see langword="true"/> in <paramref name="types"/>.
        /// 
        /// <para>
        /// Returns the same collection, if every element of <paramref name="types"/> is <see langword="false"/>.
        /// </para>
        /// 
        /// </summary>
        public static IEnumerable<Ingredient>
            FilterByType(this IEnumerable<Ingredient> ingredients, Dictionary<IngredientType, bool> types)
        {
            // If there is atleast 1 active filter.
            if (types.ContainsValue(true))
            {
                // Returned the list filtered by Type.
                return ingredients.Where(x => types.ContainsKey(x.Type) && types[x.Type]);
            }
            else
            {
                // Return the original list.
                return ingredients;
            }
        }

        /// <summary>
        /// Count the items grouped by <see cref="IngredientType"/>.
        /// 
        /// <para>
        /// If there is no item for a given type, the type will not be present in the result.
        /// </para>
        /// 
        /// </summary>
        public static Dictionary<IngredientType, int>
            CountByType(this IEnumerable<Ingredient> ingredients)
        {
            return ingredients
                .GroupBy(x => x.Type)
                .Select(x => (Key: x.Key, Count: x.Count()))
                .ToDictionary(x => x.Key, x => x.Count);
        }
    }
}