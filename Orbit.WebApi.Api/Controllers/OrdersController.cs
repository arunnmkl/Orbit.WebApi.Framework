using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Orbit.WebApi.Core.Filters;

namespace Orbit.WebApi.Api.Controllers
{
    /// <summary>
    /// Orders controller
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("api/Orders")]
    public class OrdersController : ApiController
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        [Route("All")]
        public IHttpActionResult GetAll()
        {
            return Ok(Order.GetOrders());
        }

        /// <summary>
        /// Posts the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        [OrbitAuthorization("RefreshToken", "Create")]
        public IHttpActionResult Post(Order order)
        {
            var result = Order.Create(order);
            if (result)
            {
                return Ok("Created successfully");
            }

            return BadRequest("Failed to create");
        }

        /// <summary>
        /// Posts the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        [Route("{orderId}")]
        [OrbitAuthorization("RefreshToken", "Read")]
        public IHttpActionResult Get(int orderId)
        {
            return Ok(Order.Read(orderId));
        }

        /// <summary>
        /// Posts the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        [OrbitAuthorization("RefreshToken", "Update")]
        public IHttpActionResult Put(Order order)
        {
            var result = Order.Update(order);
            if (result)
            {
                return Ok("Updated successfully");
            }

            return BadRequest("Failed to update");
        }

        /// <summary>
        /// Posts the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns> 
        [Route("{orderId}")]
        [OrbitAuthorization("RefreshToken", "Delete")]
        public IHttpActionResult Delete(int orderId)
        {
            var result = Order.Delete(orderId);
            if (result)
            {
                return Ok("Deleted successfully");
            }

            return BadRequest("Failed to delete");
        }
    }

    #region Helpers

    /// <summary>
    /// 
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        public int OrderID { get; set; }
        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        /// <value>
        /// The name of the customer.
        /// </value>
        public string CustomerName { get; set; }
        /// <summary>
        /// Gets or sets the shipper city.
        /// </summary>
        /// <value>
        /// The shipper city.
        /// </value>
        public string ShipperCity { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is shipped.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is shipped; otherwise, <c>false</c>.
        /// </value>
        public bool IsShipped { get; set; }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <returns></returns>
        public static IList<Order> GetOrders()
        {
            return Orders;
        }

        /// <summary>
        /// Creates the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public static bool Create(Order order)
        {
            try
            {
                Orders.Add(order);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Reads the specified order identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public static Order Read(int orderId)
        {
            try
            {
                return Orders.FirstOrDefault(o => o.OrderID == orderId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Updates the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public static bool Update(Order order)
        {
            try
            {
                var existingOrder = Orders.FirstOrDefault(o => o.OrderID == order.OrderID);
                existingOrder.CustomerName = order.CustomerName;
                existingOrder.ShipperCity = order.ShipperCity;
                existingOrder.IsShipped = order.IsShipped;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified order identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        public static bool Delete(int orderId)
        {
            try
            {
                var order = Orders.FirstOrDefault(o => o.OrderID == orderId);
                Orders.Remove(order);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        /// <value>
        /// The orders.
        /// </value>
        private static IList<Order> Orders
        {
            get
            {
                return new List<Order>
                {
                    new Order
                    {
                        OrderID = 10248
                        , CustomerName = "Taser Judah"
                        , ShipperCity = "Omen"
                        , IsShipped = true
                    },
                    new Order
                    {
                        OrderID = 10249
                        , CustomerName = "Ahmad Hassan"
                        , ShipperCity = "Dubai"
                        , IsShipped = false
                    },
                    new Order
                    {
                        OrderID = 10250
                        , CustomerName = "Tamer Yasser"
                        , ShipperCity = "Jeddah"
                        , IsShipped = false
                    },
                    new Order
                    {
                        OrderID = 10251
                        , CustomerName = "Lina Majid"
                        , ShipperCity = "Abu Dhabi"
                        , IsShipped = false
                    },
                    new Order
                    {
                        OrderID = 10252
                        , CustomerName = "Yasmeen Rami"
                        , ShipperCity = "Kuwait"
                        , IsShipped = true
                    }
                };
            }
        }
    }

    #endregion
}
