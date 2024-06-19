using CoffeeMachine.Db;

namespace CoffeeMachine.Managers;

internal class CoffeeManager
{
    private MainContext _db;

    internal CoffeeManager() 
    {
        _db = new MainContext();
    }

    public bool GetCoffee(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null && resources.CoffeeCurrent >= amount)
        {
            resources.CoffeeCurrent -= amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetCoffee(int amount) 
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null && (resources.CoffeeCurrent + amount) <= resources.CoffeeMax * 1000)
        {
            resources.CoffeeCurrent += amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetMaxCoffee(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null)
        {
            resources.CoffeeMax += amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
        }
    }
}
