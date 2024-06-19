using CoffeeMachine.Db;

namespace CoffeeMachine.Managers;

internal class MilkManager
{
    private MainContext _db;

    internal MilkManager()
    {
        _db = new MainContext();
    }

    public bool GetMilk(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null && resources.MilkCurrent >= amount)
        {
            resources.MilkCurrent -= amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetMilk(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null && (resources.MilkCurrent + amount) <= resources.MilkMax * 1000)
        {
            resources.MilkCurrent += amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetMaxMilk(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null)
        {
            resources.MilkMax += amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
        }
    }
}
