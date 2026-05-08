using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace HealthUrWealth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        public void Todo()
        {
            //            select distinct TxnStatus from[dbo].[PaymentTransactions]
            //            TxnStatus
            //SUCCESS
            //TXN_FAILURE
            //Transaction Failed
            //prescription_pending
            //PENDING
            //NULL
            //RETURNED
            //SUCCESS(amount collected)
            //Checkout
            //CANCELED
            //PG_FORWARD_FAIL
            //Cancelled
            //FAIL


            //            select distinct PaymentStatus from[dbo].[PaymentTransactions]
            //            PaymentStatus
            //0
            //15
            //3
            //6
            //1
            //4
            //5
            //16


            //            select distinct ShipmentType from[dbo].[PaymentTransactions]
            //            ShipmentType
            //Normal
            //NULL


            //            select distinct ordercurrentstatus from[dbo].[PaymentTransactions]
            //            ordercurrentstatus
            //0
            //9
            //3
            //6
            //1
            //10
            //4
            //5
            //8



            //            select distinct Orderdeliverystatus from[dbo].[PaymentTransactions]
            //            Orderdeliverystatus
            //DELIVERY ATTEMPTED - PREMISES CLOSED
            //RTO Delivered
            //Incorrect Waybill number or No Information
            //LOST
            //Pending
            //Reached Back At The Seller City
            //Undelivered - EN - ROUTE
            //RTO Initiated
            //In Transit. Await delivery information
            //RTO_OFD
            //CONSIGNEE'S ADDRESS UNLOCATABLE/LANDMARK NEEDED
            //CONSIGNEE REFUSED TO ACCEPT
            //Undelivered - AT SOURCE HUB
            //RTO_NDR
            //RTO IN INTRANSIT
            //CONSIGNEE'S ADDRESS INCORRECT/INCORRECT
            //SHIPMENT ARRIVED
            //Undelivered
            //NULL
            //RTO
            //Dispatched
            //NETWORK DELAY, WILL IMPACT DELIVERY
            //Undelivered - AT DESTINATION HUB
            //DELIVERY DELAYED
            //PICKUP EMPLOYEE IS OUT TO P/ U SHIPMENT
            //Online shipment booked
            //Out for Delivery
            //NECESSARY CHARGES PENDING FROM CONSIGNEE
            //Delivered
            //In Transit
            //RETURNED TO ORIGIN AT SHIPPER'S REQUEST
            //BULK ORDER, REFUSED BY CONSIGNEE
            //NEED DEPARTMENT NAME / EXTENTION NUMBER
            //WRONG PINCODE, WILL IMPACT DELIVERY
            //RTO Acknowledged
            //PROHIBITED AREA - ENTRY RESTRICTED FOR DELIVERY
            //Pickup Generated
            //Canceled
            //CONSIGNEE OUT OF STATION
            //CONSIGNEE NOT AVAILABLE
            //Manifested
            //9
            //Shipped
            //NO SUCH CONSIGNEE AT THE GIVEN ADDRESS
            //SHIPMENT REDIRECTED ON SAME AWB
            //Out for Pickup
            //Not Picked



            //  select distinct[status] from[dbo].[CheckOutUserProductTransactions]
            //            status
            //0

            //select distinct orderstatus from[dbo].[CheckOutUserProductTransactions]
            //orderstatus
            //NULL


            //            select distinct TxnStatus from[dbo].[CheckOutPaymentTransactions]

            //            TxnStatus
            //SUCCESS
            //Checkout
            //Closed



            //            select distinct PaymentStatus from[dbo].[CheckOutPaymentTransactions]
            //            PaymentStatus
            //16



            //            select distinct ordercurrentstatus from[dbo].[CheckOutPaymentTransactions]
            //            ordercurrentstatus
            //6
            //10
            //11


            //            select distinct PaymentMode from[dbo].[CheckOutPaymentTransactions]

            //            PaymentMode
            //Paytm
            //Cash On Delivery


            //            select distinct OrderStatus from[dbo].[CheckOutPaymentTransactions]
            //            OrderStatus
            //Closed
            //NULL
            //Open


            //            select distinct status from[dbo].[UserProductTransactions]
            //            status
            //0

        }
    }
}
