using CoffeeMachine.Db;

namespace CoffeeMachine.Managers;

internal class WaterManager
{
    private MainContext _db;

    internal WaterManager()
    {
        _db = new MainContext();
    }

    public bool GetWater(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null && resources.WaterCurrent >= amount)
        {
            resources.WaterCurrent -= amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetWater(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null && (resources.WaterCurrent + amount)  <= resources.WaterMax * 1000)
        {
            resources.WaterCurrent += amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetMaxWater(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null)
        {
            resources.WaterMax += amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
        }
    }
}
