using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    public static class SD
    {
        public const string DefaultFoodImage = "default_food.png";
        public const string MangerUser = "Manager";
        public const string KitcherUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";

        public const string ssShopingCartCount = "ssShopingCartCount";
        public const string ssCouponCode = "ssCouponCode";

        public const string StatusSubmitted = "Submitted";
        public const string StatusInProcess = "Being Prepared";
        public const string StatusReady = "Ready for Pickup";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";

        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double DiscountedPrice(Coupon couponDB, double originalOrderTotal)
        {
            if (couponDB == null)
            {
                return originalOrderTotal;
            }
            else
            {
                if (couponDB.MinimumAmount > originalOrderTotal)
                {
                    return originalOrderTotal;
                }
                else
                {
                    //everyting is valid
                    if (Convert.ToInt32(couponDB.CouponType) == (int)Coupon.ECouponType.Dollar)
                    {
                        //$10 0f 100
                        return Math.Round(originalOrderTotal - couponDB.Discount, 2);
                    }
                    if (Convert.ToInt32(couponDB.CouponType) == (int)Coupon.ECouponType.Percent)
                    {
                        //10 % of $100
                        return Math.Round(originalOrderTotal - (originalOrderTotal * couponDB.Discount / 100), 2);
                    }
                }
            }
            return originalOrderTotal;
        }


    }
}
