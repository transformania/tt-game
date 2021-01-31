namespace TT.Domain.Items.Services
{
    public static class PriceCalculator
    {
        public static int GetPriceToSoulbindNextItem(int itemCount)
        {
            var price = 0;
            if (itemCount == 1)
            {
                price = 100;
            }
            else if (itemCount > 1)
            {
                price = 100;
                for (var i = 0; i < itemCount - 1; i++)
                {
                    price *= 2 * 7 / 8;
                }
            }
            return price;
        }
    }
}
