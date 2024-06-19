using CoffeeMachine.Db;

namespace CoffeeMachine.Managers;

internal class SugarManager
{
    private MainContext _db;

    internal SugarManager()
    {
        _db = new MainContext();
    }

    public bool GetSugar(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null && resources.SugarCurrent >= amount)
        {
            resources.SugarCurrent -= amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool SetSugar(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null && (resources.SugarCurrent + amount) <= resources.SugarMax * 1000)
        {
            resources.SugarCurrent += amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetMaxSugar(int amount)
    {
        var resources = _db.Resources.FirstOrDefault();
        if (resources != null)
        {
            resources.SugarMax += amount;
            _db.Resources.Update(resources);
            _db.SaveChanges();
        }
    }
}
