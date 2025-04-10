using UnityEngine;
using System;

public class MoneyController : MonoBehaviour, IMoneyController
{
    private float currentMoney = 50000f; // Starting money
    public float CurrentMoney {get { return currentMoney; } } // Property to get current money
    private float maxMoney = 999999999.99f;
    private float moneyPerSecond = 0f;
    public float MoneyPerSecond {get {return moneyPerSecond;}} // money earned per second from passive income
    private float expensePerSecond = 0.1f; // money spent per second from upkeep/etc.
    public float ExpensePerSecond {get {return expensePerSecond;}} // money spent per second from upkeep/etc.
    public float ChangePerSecond;
    public static event Action<float> OnMoneyChanged; // Event to notify when money changes
    public static event Action<float> OnMoneyPerSecondChanged; // Event to notify when money per second changes
    public static event Action<float> OnExpensePerSecondChanged; // Event to notify when expense per second changes
    public static event Action<float> OnChangePerSecondChanged; // Event to notify when change per second changes

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if(currentMoney > maxMoney - 0.01f)
        {
            currentMoney = maxMoney; // Cap the money to maxMoney
        }

        expensePerSecond+= 0.1f * Time.deltaTime; // Increase expense per second over time

        AddMoney(moneyPerSecond * Time.deltaTime); // Add money per second
        SubtractMoney(expensePerSecond * Time.deltaTime);
        CalculateChangePerSecond(); // Subtract expense per second
        Bankrupt(); // Check for bankruptcy
        
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney); // Notify subscribers about the change
    }
    public void SubtractMoney(float amount)
    {
        currentMoney -= amount;
        OnMoneyChanged?.Invoke(currentMoney); // Notify subscribers about the change
    }
    public void SetMoney(float amount)
    {
        currentMoney = amount;
        OnMoneyChanged?.Invoke(currentMoney); // Notify subscribers about the change
    }

    public void AddMoneyPerSecond(float amount)
    {
        moneyPerSecond += amount;
        OnMoneyPerSecondChanged?.Invoke(moneyPerSecond); // Notify subscribers about the change
    }
    public void SubtractMoneyPerSecond(float amount)
    {
        moneyPerSecond -= amount;
        OnMoneyPerSecondChanged?.Invoke(moneyPerSecond); // Notify subscribers about the change
    }
    public void AddExpensePerSecond(float amount)
    {
        expensePerSecond += amount;
        OnExpensePerSecondChanged?.Invoke(expensePerSecond); // Notify subscribers about the change
    }
    public void SubtractExpensePerSecond(float amount)
    {
        expensePerSecond -= amount;
        OnExpensePerSecondChanged?.Invoke(expensePerSecond); // Notify subscribers about the change
    }
    private void CalculateChangePerSecond()
    {
        ChangePerSecond = moneyPerSecond - expensePerSecond; // Calculate the change per second
        OnChangePerSecondChanged?.Invoke(ChangePerSecond); // Notify subscribers about the change
    }
    private void Bankrupt(){
        if(currentMoney <= -100000f){
            // Trigger bankruptcy event or logic here
            Debug.Log("Bankruptcy triggered!"); // Placeholder for bankruptcy logic
            // End the game
            // or reset the game state
        }
    }
}
