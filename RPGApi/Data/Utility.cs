namespace RPGApi.Data
{
    public static class Utility
    {
        public static void CalculateHealth(Character character, int damage)
        {
            if (character.Health - damage > 100)
            {
                character.Health = 100;
            }
            else if (character.Health < damage)
            {
                character.Health = 0;
            }
            else
            {
                character.Health -= damage;
            }
        }
    }
}
