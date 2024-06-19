using CoffeeMachine.Db;
using CoffeeMachine.Enums;
using CoffeeMachine.Managers;
using CoffeeMachine.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace CoffeeMachine;

public partial class AdminPanelWindow : Window
{
    private MainContext _db;
    private SugarManager _sugarManager;
    private WaterManager _waterManager;
    private CupManager _cupManager;
    private CoffeeManager _coffeeManager;
    private MilkManager _milkManager;

    public AdminPanelWindow()
    {
        _db = new MainContext();
        _waterManager = new WaterManager();
        _cupManager = new CupManager();
        _sugarManager = new SugarManager();
        _milkManager = new MilkManager();
        _coffeeManager = new CoffeeManager();
        
        InitializeComponent();

        UpdateResourcePanel();
        UpdateListViewItems();
        UpdateUnits();
    }

    /// <summary>Добавление нового рецепта в базу данных. </summary>
    private void Add_Click(object sender, RoutedEventArgs e)
    {
        var error = string.IsNullOrWhiteSpace(name.Text) ? "Введите - Название кофе." : string.Empty;
        error += string.IsNullOrWhiteSpace(price.Text) ? "Введите - Цену." : string.Empty;
        error += string.IsNullOrWhiteSpace(coffeeAmount.Text) ? "Введите: Кол-во кофе." : string.Empty;
        error += string.IsNullOrWhiteSpace(milkAmount.Text) ? "Введите: Кол-во молока." : string.Empty;
        error += string.IsNullOrWhiteSpace(waterAmount.Text) ? "Введите: Кол-во воды." : string.Empty;
        error += int.TryParse(price.Text, out var convertPrice) ? string.Empty : "Неверное значение: Цены.";
        error += int.TryParse(coffeeAmount.Text, out var convertCoffeeAmount) ? string.Empty : "Неверное значение: Кол-ва кофе.";
        error += int.TryParse(milkAmount.Text, out var convertMilkAmount) ? string.Empty : "Неверное значение: Кол-ва молока.";
        error += int.TryParse(waterAmount.Text, out var convertWaterAmount) ? string.Empty : "Неверное значение: Кол-ва воды.";
        error += _db.Coffee.FirstOrDefault(x => x.Name == name.Text) == null ? string.Empty : "Название напитка уже занято.";

        if (!string.IsNullOrEmpty(error))
        {
            description.Text = error;           
        }
        else
        {
            _db.Coffee.Add(new CoffeeModel() { Name = name.Text, Price = convertPrice, CoffeeAmount = convertCoffeeAmount, MilkAmount = convertMilkAmount, WaterAmount = convertWaterAmount });
            _db.SaveChanges();

            UpdateListViewItems();
            ClearCoffeeForm();
            description.Text = string.Empty;
        }
    }

    /// <summary>Удаление выбранного рецепта из базы данных.</summary>
    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        var selectedCoffee = listView.SelectedItem as CoffeeModel;
        if (selectedCoffee != null)
        {
            _db.Coffee.Remove(selectedCoffee);
            _db.SaveChanges();
            UpdateListViewItems();
        }
    }

    /// <summary>Очистка полей формы нового рецепта.</summary>
    private void ClearCoffeeForm()
    {
        name.Clear();
        price.Clear();
        coffeeAmount.Clear();
        milkAmount.Clear();
        waterAmount.Clear();
    }

    /// <summary>Обновление рецептов из базы данных.</summary>
    private void UpdateListViewItems() => listView.ItemsSource = _db.Coffee.AsNoTracking().ToList();

    /// <summary>Обновление значений ресурсной панели значениями из базы данных.</summary>
    private void UpdateResourcePanel()
    {
        var resource = _db.Resources.AsNoTracking().FirstOrDefault() ?? new ResourceModel();
        cupsCount.Text = resource.CupsCurrent.ToString();
        cupsCountMax.Text = resource.CupsMax.ToString();
        coffeeCount.Text = resource.CoffeeCurrent.ToString();
        coffeeCountMax.Text = resource.CoffeeMax.ToString();
        waterCount.Text = resource.WaterCurrent.ToString();
        waterCountMax.Text = resource.WaterMax.ToString();
        sugarCount.Text = resource.SugarCurrent.ToString();
        sugarCountMax.Text = resource.SugarMax.ToString();
        milkCount.Text = resource.MilkCurrent.ToString();
        milkCountMax.Text = resource.MilkMax.ToString();
    }

    /// <summary>Добавление ресурсов техником.</summary>
    private void ResourceAdd_Click(object sender, RoutedEventArgs e)
    {
        var error = string.Empty;
        try
        {
            if (int.TryParse(cupsCountAdd.Text, out var cupsCountAddConvert))
                error += _cupManager.SetCup(cupsCountAddConvert) ? string.Empty : "Добавляемое количество стаканов, превышает максимальный объем.";

            if (int.TryParse(cupsCountMaxAdd.Text, out var cupsCountMaxAddConvert))
                _cupManager.SetMaxCup(cupsCountMaxAddConvert);

            if (int.TryParse(coffeeCountAdd.Text, out var coffeeCountAddConvert))
                error += _coffeeManager.SetCoffee(coffeeCountAddConvert) ? string.Empty : "Добавляемое количество кофе, превышает максимальный объем.";

            if (int.TryParse(coffeeCountMaxAdd.Text, out var coffeeCountMaxAddConvert))
                _coffeeManager.SetMaxCoffee(coffeeCountMaxAddConvert);

            if (int.TryParse(waterCountAdd.Text, out var waterCountAddConvert))
                error += _waterManager.SetWater(waterCountAddConvert) ? string.Empty : "Добавляемое количество воды, превышает максимальный объем.";

            if (int.TryParse(waterCountMaxAdd.Text, out var waterCountMaxAddConvert))
                _waterManager.SetMaxWater(waterCountMaxAddConvert);

            if (int.TryParse(sugarCountAdd.Text, out var sugarCountAddConvert))
                error += _sugarManager.SetSugar(sugarCountAddConvert) ? string.Empty : "Добавляемое количество сахара, превышает максимальный объем.";

            if (int.TryParse(sugarCountMaxAdd.Text, out var sugarCountMaxAddConvert))
                _sugarManager.SetMaxSugar(sugarCountMaxAddConvert);

            if (int.TryParse(milkCountAdd.Text, out var milkCountAddConvert))
                error += _milkManager.SetMilk(milkCountAddConvert) ? string.Empty : "Добавляемое количество молока, превышает максимальный объем.";

            if (int.TryParse(milkCountMaxAdd.Text, out var milkCountMaxAddConvert))
                _milkManager.SetMaxMilk(milkCountMaxAddConvert);

            ClearResourceForm();
        }
        catch (Exception ex) 
        {
            error += ex.Message;
        }
        resourceDescription.Text = error;

        UpdateResourcePanel();
    }

    /// <summary>Очистка полей добавления ресурсов.</summary>
    private void ClearResourceForm()
    {
        cupsCountAdd.Text = string.Empty;
        cupsCountMaxAdd.Text = string.Empty;
        coffeeCountAdd.Text = string.Empty;
        coffeeCountMaxAdd.Text = string.Empty;
        waterCountAdd.Text = string.Empty;
        waterCountMaxAdd.Text = string.Empty;
        sugarCountAdd.Text = string.Empty;
        sugarCountMaxAdd.Text = string.Empty;
        milkCountAdd.Text = string.Empty;
        milkCountMaxAdd.Text = string.Empty;
    }

    /// <summary>Обновление перевода единиц из базы данных.</summary>
    private void UpdateUnits()
    {
        milkUnit.Text = _db.Units.FirstOrDefault(x => x.Name == UnitsEnum.MilkUnit.ToString())?.ConvertValue.ToString();
        sugarUnit.Text = _db.Units.FirstOrDefault(x => x.Name == UnitsEnum.SugarUnit.ToString())?.ConvertValue.ToString();
    }

    /// <summary>Запись в базу данных обновленных значений по переводу единиц.</summary>
    private void UnitUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(milkUnit.Text, out var milkUnitNewConvertValue))
        {
            var milkUnitDb = _db.Units.FirstOrDefault(x => x.Name == UnitsEnum.MilkUnit.ToString());
            milkUnitDb.ConvertValue = milkUnitNewConvertValue;
            _db.Units.Update(milkUnitDb);
        }

        if (int.TryParse(sugarUnit.Text, out var sugarUnitNewConvertValue))
        {
            var sugarUnitDb = _db.Units.FirstOrDefault(x => x.Name == UnitsEnum.SugarUnit.ToString());
            sugarUnitDb.ConvertValue = sugarUnitNewConvertValue;
            _db.Units.Update(sugarUnitDb);
        }

        _db.SaveChanges();
    }
}
