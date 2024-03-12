using UnityEngine;
using UnityEngine.UI;

public class Player
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Health { get; set; }
    public string Attack { get; set; }
    public int ArmorClass { get; set; }
    public int Thac0 { get; set; }

    public int RollAttack()
    {
        // 模拟投掷20面的骰子
        return Random.Range(1, 21);
    }
    public int RollDamage()
	{
        string[] parts = Attack.ToLower().Split('d');
    
		if (parts.Length == 2 && int.TryParse(parts[0], out int numberOfDice) && int.TryParse(parts[1], out int facesOnDie))
		{
			int totalDamage = 0;

			for (int i = 0; i < numberOfDice; i++)
			{
				totalDamage += Random.Range(1, facesOnDie + 1);
			}

			return totalDamage;
		}

		// 处理无效的输入，返回一个默认值或者其他逻辑
		return 0;
	}

    public void GainExperience(int amount)
    {
        Experience += amount;
    }

    public void LevelUp()
    {
        if (Experience >= 100) // Example: Level up every 100 experience points per level
        {
            Level++;
            // Health += Random.Range(5, 11); // Example: Gain random health when leveling up
            Experience = 0;
        }
    }
}