using UnityEngine;

public class GameSettings
{
    public static int currentDay = 1;      // tracks which day we're on
    public static int difficulty = 0;      // 0=Easy, 1=Medium, 2=Hard, 3=Extreme

    // call this to update difficulty based on current day
    public static void UpdateDifficulty()
    {
        if (currentDay <= 3) difficulty = 0;  // Easy
        else if (currentDay <= 6) difficulty = 1;  // Medium
        else if (currentDay <= 9) difficulty = 2;  // Hard
        else difficulty = 3;  // Extreme
    }

    // returns max dishes a customer can order based on difficulty
    public static int GetMaxDishesPerCustomer()
    {
        if (difficulty == 0) return 1;
        if (difficulty == 1) return 2;
        if (difficulty == 2) return 3;
        return 4; // Extreme
    }
}
