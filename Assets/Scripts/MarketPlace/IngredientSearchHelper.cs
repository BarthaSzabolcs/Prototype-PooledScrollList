using System;
using System.Collections.Generic;
using System.Linq;

namespace BarthaSzabolcs.MarketPlace
{
    public static class IngredientSearchHelper
    {
        public static Dictionary<IngredientType, List<Ingredient>> 
            GroupByType(this IEnumerable<Ingredient> ingredients)
        {
            return ingredients
                .GroupBy(x => x.Type)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public static IOrderedEnumerable<Ingredient> 
            OrderBy(this IEnumerable<Ingredient> ingredients, string parameterName)
        {
            return parameterName switch
            {
                nameof(Ingredient.Ammount) => ingredients.OrderBy(x => x.Ammount),
                nameof(Ingredient.Price) => ingredients.OrderBy(x => x.Price),
                _ => ingredients.OrderBy(x => x.Name),
            };
        }

        public static IEnumerable<Ingredient> 
            FilterName(this IEnumerable<Ingredient> ingredients, string filterString)
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
    }
}
