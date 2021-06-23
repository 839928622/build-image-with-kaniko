using System;
using System.Collections.Generic;

namespace Core.Entities
{
  public   class CustomerBasket
    {
        public CustomerBasket()
        {
            
        }
        public CustomerBasket(string id)
        {
            Id = id;
        }
        /// <summary>
        /// generate by spa client
        /// </summary>
        public string Id { get; set; }

        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public int? DeliveryMethodId { get; set; }
        public string ClientSecret { get; set; }
        /// <summary>
        /// use it to update payment intent if user make change of delivery method / add or remove items in order
        /// </summary>
        public string PaymentIntentId { get; set; }

        public Decimal ShippingPrice { get; set; }

    }
}
