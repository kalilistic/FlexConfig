using FlexConfig;

namespace TestClass;

public static class Tester
{
    public static Configuration TestMe(string configFilePath)
    {
        var config = new Configuration(configFilePath);
        var person = new Person
        {
            name = "Kal",
            favoriteGame = "Pokemon"
        };
        var pet = new Pet
        {
            name = "Mr. Meow",
            favoriteFood = "Tuna",
            owner = person
        };
        config.Load();
        config.Set("person", person);
        config.Set("pet", pet);
        config.Set("good", true);
        config.Set("bad", 10u);
        config.Save();
        return config;
    }
}
