using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Syncfusion.EJ2.Base;
using System.Globalization;


namespace Grid.Pages
{

    public class IndexModel : PageModel
    {

        public int RecordCount => OrdersDetails.GetAllRecords().Count;

        public JsonResult OnPostDataSource([FromBody] DataManagerRequest dm)
        {
            var order = OrdersDetails.GetAllRecords();
            IEnumerable DataSource = order;

            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<OrdersDetails>().Count();
            if (dm.Group != null && dm.Group.Count > 0)
            {
                DataSource = operation.PerformGrouping(DataSource, dm.Group);
            }
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? new JsonResult(new { result = DataSource, count = count }) : new JsonResult(DataSource);
        }

        public JsonResult OnPostInsert([FromBody] CRUDAction<OrdersDetails> value)
        {
            OrdersDetails.GetAllRecords().Insert(0, value.value);
            return new JsonResult(value.value);
        }

        [NonAction]



        public JsonResult OnPostUpdate([FromBody] CRUDAction<OrdersDetails> value)
        {
            var ord = value.value;
            OrdersDetails val = OrdersDetails.GetAllRecords().Where(or => or.OrderID == ord.OrderID).FirstOrDefault();
            val.OrderID = ord.OrderID;
            val.EmployeeID = ord.EmployeeID;
            val.CustomerID = ord.CustomerID;
            val.CustomerName = ord.CustomerName;
            val.OrderDate = ord.OrderDate;
            val.Cost=ord.Cost;
            val.Freight=ord.Freight;
            val.Price=ord.Price;
            val.City = ord.City;
            val.Id = ord.Id;


            return new JsonResult(value.value);
        }



        public JsonResult OnPostDelete([FromBody] CRUDAction<OrdersDetails> value)
        {
            OrdersDetails.GetAllRecords().Remove(OrdersDetails.GetAllRecords().Where(or => or.OrderID == int.Parse(value.key.ToString())).FirstOrDefault());
            return new JsonResult(value);
        }

        public class CRUDAction<T> where T : class
        {
            public string action { get; set; }

            public string table { get; set; }

            public string keyColumn { get; set; }

            public object key { get; set; }

            public T value { get; set; }

            public List<T> added { get; set; }

            public List<T> changed { get; set; }

            public List<T> deleted { get; set; }

            public IDictionary<string, object> @params { get; set; }
        }

        public class OrdersDetails
        {
            private static readonly List<OrdersDetails> order = CreateRecords();

            public static List<OrdersDetails> GetAllRecords()
            {
                return order;
            }

            private static List<OrdersDetails> CreateRecords()
            {
                var customers = new[]
                {
                    (Id: 1, Name: "VINET", City: "Reims"),
                    (Id: 2, Name: "TOMSP", City: "Münster"),
                    (Id: 3, Name: "HANAR", City: "Rio de Janeiro"),
                    (Id: 4, Name: "VICTE", City: "Lyon"),
                    (Id: 5, Name: "SUPRD", City: "Charleroi"),
                    (Id: 6, Name: "RICSU", City: "Genève"),
                    (Id: 7, Name: "WELLI", City: "Resende"),
                    (Id: 8, Name: "HUNGO", City: "Cork"),
                    (Id: 9, Name: "FRANK", City: "München"),
                    (Id: 10, Name: "FOLKO", City: "Bräcke"),
                    (Id: 11, Name: "BERGS", City: "Luleå"),
                    (Id: 12, Name: "ALFKI", City: "Berlin")
                };
                var records = new List<OrdersDetails>(1200);

                for (var index = 1; index <= 1200; index++)
                {
                    var customer = customers[(index - 1) % customers.Length];
                    var cost = decimal.Round(45m + (index * 17.35m % 1400m), 2);

                    records.Add(new OrdersDetails
                    {
                        OrderID = 10247 + index,
                        CustomerID = customer.Id,
                        EmployeeID = ((index - 1) % 13) + 1,
                        CustomerName = customer.Name,
                        City = customer.City,
                        OrderDate = new DateTime(2023, 1, 1).AddDays((index - 1) % 730),
                        Id = index,
                        Freight = decimal.Round(5m + (index * 1.37m % 250m), 2),
                        Cost = cost,
                        Price = decimal.Round(cost * (1.15m + index % 20 / 100m), 2)
                    });
                }

                return records;
            }

            public int OrderID { get; set; }
            public int CustomerID { get; set; }
            public int? EmployeeID { get; set; }
            public decimal Cost { get; set; }
            public decimal Price { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public DateTime? OrderDate { get; set; }
            public int Id { get; set; }
            public decimal? Freight { get; set; }
        }
    }
}