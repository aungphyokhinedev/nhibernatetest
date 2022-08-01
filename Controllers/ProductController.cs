using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;
namespace NHibernateTest.Controllers;

[ApiController]
[Route("[controller]")]
public class StoreController : ControllerBase
{
    private readonly ILogger<StoreController> _logger;
    private readonly IDbContext _db;

    public StoreController(ILogger<StoreController> logger,IDbContext db)
    {
        _logger = logger;
        _db = db;
    }

   
    [ServiceFilter(typeof(LogStatusAttribute))]
    [HttpGet(Name = "GetStores")]
    public IEnumerable<Object> Get()
    {
        using (var session = _db.Open())
        {
            var stores = session.Query<Store>().Select(s=> new {Name=s.Name, Products = s.Products.Select(p=>p.Name)}).Skip(1).Take(5)
              .ToList();
           
            return stores;
        }
    }

    [HttpPost(Name = "CreateProduct")]
    public void Post()
    {
        using (var session = _db.Open())

        {

            using (var transaction = session.BeginTransaction())
            {
                // create a couple of Stores each with some Products and Employees
                var barginBasin = new Store { Name = "Bargin Basin" };
                var superMart = new Store { Name = "SuperMart" };

                var potatoes = new Product { Name = "Potatoes", Price = 3.60 };
                var fish = new Product { Name = "Fish", Price = 4.49 };
                var milk = new Product { Name = "Milk", Price = 0.79 };
                var bread = new Product { Name = "Bread", Price = 1.29 };
                var cheese = new Product { Name = "Cheese", Price = 2.10 };
                var waffles = new Product { Name = "Waffles", Price = 2.41 };

                var daisy = new Employee { FirstName = "Daisy", LastName = "Harrison" };
                var jack = new Employee { FirstName = "Jack", LastName = "Torrance" };
                var sue = new Employee { FirstName = "Sue", LastName = "Walkters" };
                var bill = new Employee { FirstName = "Bill", LastName = "Taft" };
                var joan = new Employee { FirstName = "Joan", LastName = "Pope" };

                // add products to the stores, there's some crossover in the products in each
                // store, because the store-product relationship is many-to-many
                AddProductsToStore(barginBasin, potatoes, fish, milk, bread, cheese);
                AddProductsToStore(superMart, bread, cheese, waffles);

                // add employees to the stores, this relationship is a one-to-many, so one
                // employee can only work at one store at a time
                AddEmployeesToStore(barginBasin, daisy, jack, sue);
                AddEmployeesToStore(superMart, bill, joan);

                // save both stores, this saves everything else via cascading
                session.SaveOrUpdate(barginBasin);
                session.SaveOrUpdate(superMart);

                transaction.Commit();
            }
        }
    }

    private void AddProductsToStore(Store store, params Product[] products)
    {
        foreach (var product in products)
        {
            store.AddProduct(product);
        }
    }

    private void AddEmployeesToStore(Store store, params Employee[] employees)
    {
        foreach (var employee in employees)
        {
            store.AddEmployee(employee);
        }
    }
}
