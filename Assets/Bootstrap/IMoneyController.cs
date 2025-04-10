public interface IMoneyController{
    float CurrentMoney { get; } // Property to get current money
    float MoneyPerSecond { get; } // Property to get money per second
    float ExpensePerSecond { get; } // Property to get expense per second

    void AddMoney(float amount); // Method to add money
    void SubtractMoney(float amount); // Method to subtract money
    void SetMoney(float amount); // Method to set money
}