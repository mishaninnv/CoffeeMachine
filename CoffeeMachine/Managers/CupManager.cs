using CoffeeMachine.Db;

namespace CoffeeMachine.Managers;

internal class CupManager
{
    private MainContext _db;

    internal CupManager()
    {
        _db = new MainContext();
    }

    public bool GetCup(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null && resources.CupsCurrent >= amount)
        {
            resources.CupsCurrent -= amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetCup(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null && (resources.CupsCurrent + amount) <= resources.CupsMax)
        {
            resources.CupsCurrent += amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetMaxCup(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null)
        {
            resources.CupsMax += amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
        }
    }
}
